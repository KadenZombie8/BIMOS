using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [Serializable]
    public struct LeftRightGrabbables
    {
        public Grabbable Left;
        public Grabbable Right;
    }

    [RequireComponent(typeof(HoldDetector))]
    public class Storable : MonoBehaviour
    {
        public event Action OnStored;
        public event Action OnRetrieved;

        public string[] Tags = { "Light" };
        public LeftRightGrabbables RetrieveGrabbables;

        [HideInInspector]
        public Storable ParentStorable;

        [HideInInspector]
        public ItemSlot ItemSlot;

        [HideInInspector]
        public HoldDetector HoldDetector;
        private Item _item;

        private void Awake()
        {
            HoldDetector = GetComponent<HoldDetector>();
            _item = GetComponent<Item>();
        }

        private void OnEnable()
        {
            HoldDetector.OnLastRelease?.AddListener(LookForItemSlot);
            _item.OnGameObjectAdded += AddStorables;
            _item.OnGameObjectRemoved += RemoveStorables;
        }

        private void OnDisable()
        {
            HoldDetector.OnLastRelease?.RemoveListener(LookForItemSlot);
            _item.OnGameObjectAdded -= AddStorables;
            _item.OnGameObjectRemoved -= RemoveStorables;
        }

        private void AddStorables(GameObject gameObject)
        {
            var storables = gameObject.GetComponentsInChildren<Storable>();
            foreach (var storable in storables)
            {
                storable.ParentStorable = this;
                storable.HoldDetector.enabled = false;
            }
        }

        private void RemoveStorables(GameObject gameObject)
        {
            var storables = gameObject.GetComponentsInChildren<Storable>();
            foreach (var storable in storables)
            {
                storable.ParentStorable = null;
                storable.HoldDetector.enabled = true;
            }
        }

        private void LookForItemSlot(Hand hand)
        {
            var handCollider = hand.ArmColliders.Hand;
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

            highestRankItemSlot.StoreItem(GetRootStorable(this));
        }

        private Storable GetRootStorable(Storable storable)
        {
            var parentStorable = storable.ParentStorable;
            if (parentStorable)
                return GetRootStorable(parentStorable);
            else
                return storable;
        }

        public void Store() => OnStored?.Invoke();

        public void Retrieve() => OnRetrieved?.Invoke();
    }
}
