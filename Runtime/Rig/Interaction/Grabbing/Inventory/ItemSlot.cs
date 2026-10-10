using KadenZombie8.BIMOS.Rig.Grips;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class ItemSlot : MonoBehaviour
    {
        public event Action OnStore;
        public event Action OnRetrieve;

        public string[] Tags = { "Light", "Heavy" };
        public Storable StoredStorable;

        private class ItemPhysicsState
        {
            public Dictionary<Collider, bool> Colliders = new();
            public Dictionary<Rigidbody, bool> Rigidbodies = new();
            public Dictionary<ArticulationBody, bool> ArticulationBodies = new();
            public Dictionary<Renderer, bool> Renderers = new();
        }
        private readonly ItemPhysicsState _itemData = new();

        private Grip[] _itemSlotGrips;

        protected virtual void Awake()
        {
            _itemSlotGrips = GetComponentsInChildren<Grip>();
            SetItemSlotGripsEnabled(false);
        }

        private void DisableItem(Item item)
        {
            HashSet<Collider> colliders = new();
            HashSet<Rigidbody> rigidbodies = new();
            HashSet<ArticulationBody> articulationBodies = new();
            HashSet<Renderer> renderers = new();

            foreach (var gameObject in item.GameObjects)
            {
                if (!gameObject)
                {
                    item.GameObjects.Remove(gameObject);
                    continue;
                }

                foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                    colliders.Add(collider);
                foreach (var rigidbody in gameObject.GetComponentsInChildren<Rigidbody>())
                    rigidbodies.Add(rigidbody);
                foreach (var articulationBody in gameObject.GetComponentsInChildren<ArticulationBody>())
                    articulationBodies.Add(articulationBody);
                foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
                    renderers.Add(renderer);
            }

            _itemData.Colliders.Clear();
            _itemData.Rigidbodies.Clear();
            _itemData.ArticulationBodies.Clear();
            _itemData.Renderers.Clear();

            foreach (var collider in colliders)
            {
                _itemData.Colliders[collider] = collider.enabled;
                collider.enabled = false;
            }
            foreach (var rigidbody in rigidbodies)
            {
                _itemData.Rigidbodies[rigidbody] = rigidbody.isKinematic;
                rigidbody.isKinematic = true;
            }
            foreach (var articulationBody in articulationBodies)
            {
                if (!articulationBody.isRoot) continue;
                _itemData.ArticulationBodies[articulationBody] = articulationBody.immovable;
                articulationBody.immovable = true;
            }
            foreach (var renderer in renderers)
            {
                _itemData.Renderers[renderer] = renderer.enabled;
                renderer.enabled = false;
            }
        }

        private void EnableItem()
        {
            foreach (var pair in _itemData.Colliders)
                pair.Key.enabled = pair.Value;
            foreach (var pair in _itemData.Rigidbodies)
                pair.Key.isKinematic = pair.Value;
            foreach (var pair in _itemData.ArticulationBodies)
                pair.Key.immovable = pair.Value;
            foreach (var pair in _itemData.Renderers)
                pair.Key.enabled = pair.Value;
        }

        public virtual void StoreItem(Storable storable)
        {
            if (StoredStorable) return;
            if (!storable.TryGetComponent<Item>(out var item)) return;
            if (!HasMatchingTag(storable)) return;

            DisableItem(item);

            StoredStorable = storable;
            StoredStorable.ItemSlot = this;
            SetItemSlotGripsEnabled(true);
            OnStore?.Invoke();
            storable.Store();
        }

        private void AlignGrip(Hand hand, Grip grip)
        {
            var item = StoredStorable.GetComponent<Item>();
            grip.transform.GetPositionAndRotation(out var gripPosition, out var gripRotation);
            foreach (var gameObject in item.GameObjects)
            {
                var localPosition = Quaternion.Inverse(gripRotation) * (gameObject.transform.position - gripPosition);
                var worldPosition = hand.PalmTransform.TransformPoint(localPosition);

                var localRotation = Quaternion.Inverse(gripRotation) * gameObject.transform.rotation;
                var worldRotation = hand.PalmTransform.rotation * localRotation;

                if (gameObject.TryGetComponent<ArticulationBody>(out var articulationBody) && articulationBody.isRoot)
                {
                    articulationBody.transform.SetPositionAndRotation(worldPosition, worldRotation);
                    articulationBody.TeleportRoot(worldPosition, worldRotation);
                }

                if (gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    rigidbody.position = worldPosition;
                    rigidbody.rotation = worldRotation;
                    gameObject.transform.SetPositionAndRotation(worldPosition, worldRotation);
                }
            }
        }

        public virtual void RetrieveItem(TargetGrip grip)
        {
            if (!StoredStorable) return;
            if (!StoredStorable.TryGetComponent<Item>(out var storedItem)) return;

            var hand = grip.LeftHand ? grip.LeftHand : grip.RightHand;
            hand.GrabHandler.AttemptRelease();

            var retrieveGrip = hand.Handedness == Handedness.Left
                ? StoredStorable.RetrieveGrips.Left
                : StoredStorable.RetrieveGrips.Right;

            AlignGrip(hand, retrieveGrip);
            EnableItem();
            retrieveGrip.Grab(hand);

            StoredStorable.Retrieve();
            StoredStorable.ItemSlot = null;
            StoredStorable = null;
            SetItemSlotGripsEnabled(false);
            OnRetrieve?.Invoke();
        }

        private bool HasMatchingTag(Storable item)
        {
            foreach (var slotTag in Tags)
                foreach (var itemTag in item.Tags)
                    if (slotTag == itemTag)
                        return true;

            return false;
        }
        
        protected void SetItemSlotGripsEnabled(bool isEnabled)
        {
            foreach(var itemSlotGrip in _itemSlotGrips)
                itemSlotGrip.enabled = isEnabled;
        }

        protected void DestroyStoredItem()
        {
            if (!StoredStorable) return;
            Destroy(StoredStorable.gameObject);
            StoredStorable = null;

            _itemData.Colliders.Clear();
            _itemData.Rigidbodies.Clear();
            _itemData.ArticulationBodies.Clear();
            _itemData.Renderers.Clear();
        }
    }
}
