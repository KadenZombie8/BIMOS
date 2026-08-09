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

        [SerializeField]
        private Grounding _grounding;

        [SerializeField]
        private Rigidbody _locomotionSphere;

        [SerializeField]
        private Rigidbody _knee;

        [SerializeField]
        private Rigidbody _pelvis;

        [SerializeField]
        private Transform _headCameraOffset;

        [SerializeField]
        private CapsuleCollider _bodyCollider;

        [SerializeField]
        private ConfigurableJoint _legJoint;

        public float TiptoesLegHeightGain { get; private set; } = 0.2f;
        public float MaxStandingLegHeight => StandingLegHeight + TiptoesLegHeightGain;
        public float StandingLegHeight { get; private set; } = 1.3f;
        public float CrouchingLegHeight { get; private set; } = 0.4f;
        public float MinCrouchingLegHeight => CrouchingLegHeight - TiptoesLegHeightGain;
        public float CrawlingLegHeight { get; private set; } = 0f;
        public float MinLegHeight { get; set; }
        public float MaxLegHeight { get; set; }
        public float LegHeightRange => MaxStandingLegHeight - CrawlingLegHeight;

        private float _previousLegHeight = 0f;

        public float LegHeight => _pelvis.position.y - _knee.position.y;

        public float GroundingForce { get; set; }

        private readonly float _maxGroundingImpulse = 30f;

        private readonly float _maxReactionImpulse = 5f;

        private void Start()
        {
            TargetLegHeight = StandingLegHeight;
            VirtualCrouching = GetComponent<VirtualCrouching>();

            MaxLegHeight = MaxStandingLegHeight;
            MinLegHeight = MinCrouchingLegHeight;

            _previousLegHeight = LegHeight;
        }

        private void FixedUpdate()
        {
            ApplyCrouch();
            UpdateCollider(_bodyCollider,
                _knee.position,
                _headCameraOffset.position);

            if (_grounding.IsGrounded) ApplyGroundingForce();

            _previousLegHeight = LegHeight;
        }

        private void ApplyCrouch()
        {
            TargetLegHeight = Mathf.Clamp(TargetLegHeight, MinLegHeight, MaxLegHeight);
            _legJoint.targetPosition = new Vector3(0f, TargetLegHeight - LegHeightRange / 2f, 0f);
        }

        private static void UpdateCollider(CapsuleCollider collider, Vector3 to, Vector3 from)
        {
            collider.height = Vector3.Distance(to, from) + collider.radius * 2f;
            collider.transform.position = (to + from) / 2f;
        }

        private void ApplyGroundingForce()
        {
            var legHeightSpeed = (LegHeight - _previousLegHeight) / Time.fixedDeltaTime;
            var groundingStrength = legHeightSpeed / LegHeightRange * GroundingForce;

            if (groundingStrength > 0f) return;

            var groundingImpulse = groundingStrength * _grounding.GroundNormal;
            groundingImpulse = Vector3.ClampMagnitude(groundingImpulse, _maxGroundingImpulse);

            var reactionImpulse = -groundingImpulse;
            reactionImpulse = Vector3.ClampMagnitude(reactionImpulse, _maxReactionImpulse);

            _locomotionSphere.AddForceAtPosition(groundingImpulse, _grounding.GroundPoint, ForceMode.Impulse);
            BodyUtilities.AddForceAtPosition(_grounding.GroundBody, reactionImpulse, _grounding.GroundPoint, ForceMode.Impulse);
        }
    }
}