using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [RequireComponent(typeof(HoldDetector))]
    public class Storable : MonoBehaviour
    {
        public event Action OnStored;
        public event Action OnRetrieved;

        public string[] Tags = { "Light" };
        public RetrieveGrabbablesStruct RetrieveGrabbables;

        [HideInInspector]
        public Storable ParentStorable;

        [HideInInspector]
        public ItemSlot ItemSlot;

        [Serializable]
        public struct RetrieveGrabbablesStruct
        {
            public Grabbable Left;
            public Grabbable Right;
        }

        private HoldDetector _holdDetector;
        private Item _item;

        private void Awake()
        {
            _holdDetector = GetComponent<HoldDetector>();
            _item = GetComponent<Item>();
        }

        private void OnEnable()
        {
            _holdDetector.OnLastRelease.AddListener(LookForItemSlot);
            _item.OnGameObjectAdded += AddStorables;
            _item.OnGameObjectRemoved += RemoveStorables;
        }

        private void OnDisable()
        {
            _holdDetector.OnLastRelease.RemoveListener(LookForItemSlot);
            _item.OnGameObjectAdded -= AddStorables;
            _item.OnGameObjectRemoved -= RemoveStorables;
        }

        private void AddStorables(GameObject gameObject)
        {
            var storables = gameObject.GetComponentsInChildren<Storable>();
            foreach (var storable in storables)
                storable.ParentStorable = this;
        }

        private void RemoveStorables(GameObject gameObject)
        {
            var storables = gameObject.GetComponentsInChildren<Storable>();
            foreach (var storable in storables)
                storable.ParentStorable = null;
        }

        private void LookForItemSlot(Hand hand)
        {
            var handCollider = hand.PhysicsHandCollider;
            var handColliderTransform = handCollider.transform;
            var itemSlotColliders = Physics.OverlapBox(
                handColliderTransform.position,
                handCollider.size / 2f,
                handColliderTransform.rotation,
                Physics.AllLayers,
                QueryTriggerInteraction.Collide
            );

            float lowestDistance = Mathf.Infinity;
            ItemSlot highestRankItemSlot = null;

            foreach (var itemSlotCollider in itemSlotColliders)
            {
                if (!itemSlotCollider.TryGetComponent<ItemSlot>(out var itemSlot)) continue;

                var distance = (handColliderTransform.position - itemSlotCollider.transform.position).sqrMagnitude;
                if (distance > lowestDistance)
                    continue;

                lowestDistance = distance;
                highestRankItemSlot = itemSlot;
            }

            if (!highestRankItemSlot) return;

            if (ParentStorable)
            {
                highestRankItemSlot.StoreItem(ParentStorable);
            }

            TryStore(highestRankItemSlot);
        }

        public void TryStore(ItemSlot itemSlot)
        {
            if (ParentStorable)
                ParentStorable.TryStore(itemSlot);
            else
                itemSlot.StoreItem(this);
        }
    }
}
