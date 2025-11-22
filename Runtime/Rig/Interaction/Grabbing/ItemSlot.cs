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
        public StorableItem StoredItem;

        private readonly Dictionary<GrabHandler, Action> _grabHandlerListeners = new();
        private readonly Dictionary<Interactable, UnityAction> _triggerListeners = new();

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

            void storeListener() => LookForStorableItem(grabHandler);

            _grabHandlerListeners[grabHandler] = storeListener;
            grabHandler.OnRelease += storeListener;
        }

        private void OnTriggerExit(Collider other)
        {
            var rigidbody = other.attachedRigidbody;
            if (!rigidbody) return;
            if (!rigidbody.TryGetComponent<GrabHandler>(out var grabHandler)) return;
            if (!_grabHandlerListeners.TryGetValue(grabHandler, out var storeListener)) return;

            grabHandler.OnRelease -= storeListener;
            _grabHandlerListeners.Remove(grabHandler);
        }

        private void LookForStorableItem(GrabHandler grabHandler)
        {
            if (!grabHandler.TryGetComponent<Hand>(out var hand)) return;

            var grab = hand.CurrentGrab;
            if (!grab) return;

            var item = grab.transform.GetComponentInParent<StorableItem>();
            if (!item) return;

            StoreItem(item);
        }

        public void StoreItem(StorableItem item)
        {
            if (StoredItem) return;
            if (!HasMatchingTag(item)) return;

            foreach (var gameObject in item.GameObjects)
                gameObject.SetActive(false);

            StoredItem = item;
            OnStore?.Invoke();
        }

        public void RetrieveItem(SnapGrabbable grabbable)
        {
            if (!StoredItem) return;

            foreach (var gameObject in StoredItem.GameObjects)
                gameObject.SetActive(true);

            var hand = grabbable.LeftHand ? grabbable.LeftHand : grabbable.RightHand;
            hand.GrabHandler.AttemptRelease();

            var retrieveGrabbable = hand.Handedness == Handedness.Left
                ? StoredItem.RetrieveGrabbables.Left
                : StoredItem.RetrieveGrabbables.Right;

            retrieveGrabbable.Grab(hand);

            StoredItem = null;
            OnRetrieve?.Invoke();
        }

        private bool HasMatchingTag(StorableItem item)
        {
            foreach (var slotTag in Tags)
                foreach (var itemTag in item.Tags)
                    if (slotTag == itemTag)
                        return true;

            return false;
        }
    }
}
