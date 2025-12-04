using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Predict's the player's elbow location for the two-bone IK hint
    /// This heuristic method is based upon one shared by TundraFightSchool on YouTube <3
    /// </summary>
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    public class ElbowPrediction : MonoBehaviour
    {
        [SerializeField]
        private Handedness _handedness;

        [SerializeField]
        private Transform _controller;

        private Transform _upperArmBone;
        private Transform _lowerArmBone;
        private Transform _handBone;

        private Transform _hint;

        private TwoBoneIKConstraint _constraint;
        private Transform _pelvis;

        private Vector3 _smoothElbowDirection;
        private readonly float _elbowSmoothing = 10f;

        private enum ElbowState
        {
            Outward,
            Inward
        }

        private ElbowState _elbowState;

        private enum WristAxis
        {
            X, Xp,
            Y, Yp,
            Z, Zp
        }

        private struct Influencer
        {
            public WristAxis UpAxis;
            public WristAxis RightAxis;
            public float InnerAngle;
            public float OuterAngle;

            public Influencer(WristAxis rightAxis, WristAxis upAxis, float innerAngle, float? outerAngle = null)
            {
                RightAxis = rightAxis;
                UpAxis = upAxis;
                InnerAngle = innerAngle;
                OuterAngle = outerAngle ?? innerAngle;
            }
        }

        readonly Influencer[] _influencers =
        {
            new(WristAxis.X, WristAxis.Y, 30f),
            new(WristAxis.X, WristAxis.Yp, 20f, 140f),
            new(WristAxis.X, WristAxis.Z, 20f, 150f),
            new(WristAxis.X, WristAxis.Zp, 30f),

            new(WristAxis.Xp, WristAxis.Y, 0f),
            new(WristAxis.Xp, WristAxis.Yp, 0f, 170f),
            new(WristAxis.Xp, WristAxis.Z, 0f, 160f),
            new(WristAxis.Xp, WristAxis.Zp, 0f, 150f),

            new(WristAxis.Y, WristAxis.X, -60f, 140f),
            new(WristAxis.Y, WristAxis.Xp, -30f),
            new(WristAxis.Y, WristAxis.Z, -20f, 170f),
            new(WristAxis.Y, WristAxis.Zp, -40f),

            new(WristAxis.Yp, WristAxis.X, 90f),
            new(WristAxis.Yp, WristAxis.Xp, 50f),
            new(WristAxis.Yp, WristAxis.Z, 50f, 140f),
            new(WristAxis.Yp, WristAxis.Zp, 50f),

            new(WristAxis.Z, WristAxis.X, -20, 140f),
            new(WristAxis.Z, WristAxis.Xp, 70f),
            new(WristAxis.Z, WristAxis.Y, 0f, 90f),
            new(WristAxis.Z, WristAxis.Yp, 90f),

            new(WristAxis.Zp, WristAxis.X, -20f, 70f),
            new(WristAxis.Zp, WristAxis.Xp, 20f),
            new(WristAxis.Zp, WristAxis.Y, 10f),
            new(WristAxis.Zp, WristAxis.Yp, 30f, 150f)
        };

        private bool IsRightHand => _handedness == Handedness.Right;

        Vector3 GetAxis(WristAxis axis)
        {
            var x = _controller.right;
            var y = _controller.up;
            var z = _controller.forward;

            if (!IsRightHand) x = -x;

            return axis switch
            {
                WristAxis.X => x,
                WristAxis.Xp => -x,
                WristAxis.Y => y,
                WristAxis.Yp => -y,
                WristAxis.Z => z,
                WristAxis.Zp => -z,
                _ => Vector3.zero
            };
        }

        private void Start()
        {
            _constraint = GetComponent<TwoBoneIKConstraint>();

            _upperArmBone = _constraint.data.root;
            _lowerArmBone = _constraint.data.mid;
            _handBone = _constraint.data.tip;
            _hint = _constraint.data.hint;

            _pelvis = BIMOSRig.Instance.PhysicsRig.Rigidbodies.Pelvis.transform;
        }

        private void LateUpdate()
        {
            // Find the elbow circle origin and radius
            var shoulderToHandDirection = (_handBone.position - _upperArmBone.position).normalized;

            // Find elbow circle properties
            var elbowOrigin = _upperArmBone.position + shoulderToHandDirection * Vector3.Dot(_lowerArmBone.position - _upperArmBone.position, shoulderToHandDirection);
            var elbowRadius = Vector3.Distance(elbowOrigin, _lowerArmBone.position);

            // Find elbow down
            var elbowDownRotation = Quaternion.FromToRotation(_pelvis.forward, shoulderToHandDirection);

            // Get reference vectors
            var refUp = elbowDownRotation * Vector3.up;
            var refRight = Vector3.Cross(refUp, shoulderToHandDirection);

            if (!IsRightHand) refRight *= -1f;

            // Process influencer data
            var angleSum = 0f;
            var weightSum = 0f;

            foreach (var influencer in _influencers)
            {
                var influencerRight = GetAxis(influencer.RightAxis);
                var influencerUp = GetAxis(influencer.UpAxis);

                var weightRight = Mathf.Max(0f, Vector3.Dot(influencerRight, refRight));
                var weightUp = Mathf.Max(0f, Vector3.Dot(influencerUp, refUp));

                var weightProduct = weightRight * weightUp;

                var influencerAngle = _elbowState == ElbowState.Inward ? influencer.InnerAngle : influencer.OuterAngle;

                angleSum += weightProduct * influencerAngle;
                weightSum += weightProduct;
            }

            // Predict elbow angle using influencer data average
            var predictedElbowAngle = 0f;
            if (weightSum > 0f)
                predictedElbowAngle = angleSum / weightSum;

            _elbowState = predictedElbowAngle > 60f ? ElbowState.Outward : ElbowState.Inward;

            if (!IsRightHand) predictedElbowAngle *= -1f;

            // Calculate target elbow direction
            var elbowDirection = elbowDownRotation * Quaternion.AngleAxis(predictedElbowAngle, shoulderToHandDirection) * Vector3.down;

            // Smooth elbow direction
            _smoothElbowDirection = Vector3.Slerp(_smoothElbowDirection, elbowDirection, Time.deltaTime * _elbowSmoothing);
            Quaternion elbowRotation = Quaternion.LookRotation(shoulderToHandDirection, _smoothElbowDirection);

            // Apply smoothed direction to hint
            _hint.position = elbowOrigin + elbowRotation * Vector3.up * elbowRadius;
        }
    }
}
