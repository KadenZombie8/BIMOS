using System;
using UnityEngine;
using UnityEngine.XR;

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
        public ItemSlot ItemSlot;

        [Serializable]
        public struct RetrieveGrabbablesStruct
        {
            public Grabbable Left;
            public Grabbable Right;
        }

        private HoldDetector _holdDetector;

        private void Awake() => _holdDetector = GetComponent<HoldDetector>();

        private void OnEnable() => _holdDetector.OnLastRelease.AddListener(LookForItemSlot);

        private void OnDisable() => _holdDetector.OnLastRelease.RemoveListener(LookForItemSlot);

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
            highestRankItemSlot.StoreItem(this);
        }
    }
}
