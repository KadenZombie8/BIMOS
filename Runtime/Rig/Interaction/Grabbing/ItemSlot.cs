using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KadenZombie8.BIMOS.Rig
{
    public class ItemSlot : MonoBehaviour
    {
        public event Action OnStore;
        public event Action OnRetrieve;

        public string[] Tags = { "Light", "Heavy" };
        public Storable StoredStorable;

        private readonly Dictionary<GrabHandler, Action> _grabHandlerListeners = new();
        private readonly Dictionary<Interactable, UnityAction> _triggerListeners = new();

        private class ItemPhysicsState
        {
            public Dictionary<Collider, bool> Colliders = new();
            public Dictionary<Rigidbody, bool> Rigidbodies = new();
            public Dictionary<ArticulationBody, bool> ArticulationBodies = new();
            public Dictionary<Renderer, bool> Renderers = new();
        }
        private readonly ItemPhysicsState _itemData = new();

        private void OnEnable()
        {
            foreach (var interactable in GetComponentsInChildren<Interactable>())
            {
                var grabbable = interactable.GetComponent<SnapGrabbable>();

                void action() => RetrieveItem(grabbable);
                _triggerListeners[interactable] = action;

                interactable.TriggerDownEvent.AddListener(action);
            }
        }

        private void OnDisable()
        {
            foreach (var interactable in GetComponentsInChildren<Interactable>())
                interactable.TriggerDownEvent.RemoveListener(_triggerListeners[interactable]);
        }

        private void OnTriggerEnter(Collider other)
        {
            var rigidbody = other.attachedRigidbody;
            if (!rigidbody) return;
            if (!rigidbody.TryGetComponent<GrabHandler>(out var grabHandler)) return;
            if (other.transform != grabHandler.GrabBounds) return;
            if (_grabHandlerListeners.TryGetValue(grabHandler, out var _)) return;

            void storeListener() => LookForStorableItem(grabHandler);

            _grabHandlerListeners[grabHandler] = storeListener;
            grabHandler.OnRelease += storeListener;
        }

        private void OnTriggerExit(Collider other)
        {
            var rigidbody = other.attachedRigidbody;
            if (!rigidbody) return;
            if (!rigidbody.TryGetComponent<GrabHandler>(out var grabHandler)) return;
            if (other.transform != grabHandler.GrabBounds) return;
            if (!_grabHandlerListeners.TryGetValue(grabHandler, out var storeListener)) return;

            grabHandler.OnRelease -= storeListener;
            _grabHandlerListeners.Remove(grabHandler);
        }

        private void LookForStorableItem(GrabHandler grabHandler)
        {
            if (!grabHandler.TryGetComponent<Hand>(out var hand)) return;

            var grab = hand.CurrentGrab;
            if (!grab) return;

            if (hand.OtherHand.CurrentGrab) return;

            var storable = grab.GetComponentInParent<Storable>();
            if (!storable) return;

            StoreItem(storable);
        }

        private void DisableItem(Item item)
        {
            HashSet<Collider> colliders = new();
            HashSet<Rigidbody> rigidbodies = new();
            HashSet<ArticulationBody> articulationBodies = new();
            HashSet<Renderer> renderers = new();

            foreach (var gameObject in item.GameObjects)
            {
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

        public void StoreItem(Storable storable)
        {
            if (StoredStorable) return;
            if (!storable.TryGetComponent<Item>(out var item)) return;
            if (!HasMatchingTag(storable)) return;

            DisableItem(item);

            StoredStorable = storable;
            OnStore?.Invoke();
        }

        public void RetrieveItem(SnapGrabbable grabbable)
        {
            if (!StoredStorable) return;
            if (!StoredStorable.TryGetComponent<Item>(out var storedItem)) return;

            EnableItem();

            var hand = grabbable.LeftHand ? grabbable.LeftHand : grabbable.RightHand;
            hand.GrabHandler.AttemptRelease();

            var retrieveGrabbable = hand.Handedness == Handedness.Left
                ? StoredStorable.RetrieveGrabbables.Left
                : StoredStorable.RetrieveGrabbables.Right;

            retrieveGrabbable.Grab(hand);

            StoredStorable = null;
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
    }
}
