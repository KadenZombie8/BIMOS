using UnityEngine;

namespace KadenZombie8.BIMOS.Rig {
    /// <summary>
    /// Sets the ammo pouch to spawn the correct ammo prefab
    /// </summary>
    [RequireComponent(typeof(HoldDetector))]
    public class AmmoPouchUpdater : MonoBehaviour
    {
        [SerializeField]
        private GameObject _ammoPrefab;

        private HoldDetector _holdDetector;

        private void Awake() => _holdDetector = GetComponent<HoldDetector>();

        private void OnEnable()
        {
            _holdDetector.OnFirstGrab?.AddListener(UpdateAmmoPrefab);
            _holdDetector.OnLastRelease?.AddListener(TryUpdateToOtherHand);
        }

        private void OnDisable()
        {
            _holdDetector.OnFirstGrab?.RemoveListener(UpdateAmmoPrefab);
            _holdDetector.OnLastRelease?.RemoveListener(TryUpdateToOtherHand);
        }

        public void UpdateAmmoPrefab(Hand _) => AmmoPouch.Instance.SetAmmoPrefab(_ammoPrefab);

        private void TryUpdateToOtherHand(Hand hand)
        {
            var otherHand = hand.OtherHand;

            var grabbable = otherHand.CurrentGrab;
            if (!grabbable) return;

            var ammoPouchUpdater = grabbable.GetComponentInParent<AmmoPouchUpdater>();
            if (!ammoPouchUpdater) return;

            ammoPouchUpdater.UpdateAmmoPrefab(otherHand);
        }
    }
}
