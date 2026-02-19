using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles crouching
    /// </summary>
    [DefaultExecutionOrder(2)]
    public class Crouching : MonoBehaviour
    {
        public float TargetLegHeight;
        public VirtualCrouching VirtualCrouching { get; private set; }

        private BIMOSRig _rig;

        public float TiptoesLegHeightGain { get; private set; } = 0.2f;
        public float MaxStandingLegHeight => StandingLegHeight + TiptoesLegHeightGain;
        public float StandingLegHeight { get; private set; } = 1.3f;
        public float CrouchingLegHeight { get; private set; } = 0.4f;
        public float MinCrouchingLegHeight => CrouchingLegHeight - TiptoesLegHeightGain;
        public float CrawlingLegHeight { get; private set; } = 0f;
        public float MinLegHeight { get; set; }
        public float MaxLegHeight { get; set; }

        private void Start()
        {
            _rig = BIMOSRig.Instance;
            TargetLegHeight = StandingLegHeight;
            VirtualCrouching = GetComponent<VirtualCrouching>();

            MaxLegHeight = MaxStandingLegHeight;
            MinLegHeight = MinCrouchingLegHeight;
        }

        private void FixedUpdate()
        {
            ApplyCrouch();
            UpdateCollider(_rig.PhysicsRig.Colliders.Body,
                _rig.PhysicsRig.Rigidbodies.Knee.transform.position,
                _rig.ControllerRig.Transforms.HeadCameraOffset.position);
        }

        private void ApplyCrouch()
        {
            var fullHeight = MaxStandingLegHeight - CrawlingLegHeight;
            TargetLegHeight = Mathf.Clamp(TargetLegHeight, MinLegHeight, MaxLegHeight);
            _rig.PhysicsRig.Joints.Pelvis.targetPosition = new Vector3(0f, TargetLegHeight - fullHeight / 2f, 0f);
        }

        private static void UpdateCollider(CapsuleCollider collider, Vector3 to, Vector3 from)
        {
            collider.height = Vector3.Distance(to, from) + collider.radius * 2f;
            collider.transform.position = (to + from) / 2f;
        }
    }
}