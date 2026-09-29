using KadenZombie8.BIMOS.AnimationRigging;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Predict's the player's elbow location for the two-bone IK hint
    /// This heuristic method is based upon one shared by TundraFightSchool on YouTube <3
    /// </summary>
    [RequireComponent(typeof(LimitedTwoBoneIKConstraint))]
    public class ElbowPrediction : MonoBehaviour
    {
        [SerializeField]
        private Handedness _handedness;

        [SerializeField]
        private Transform _controller;

        [SerializeField]
        private Transform _pelvis;

        private Transform _upperArmBone;
        private Transform _lowerArmBone;
        private Transform _handBone;

        private Transform _hint;

        private LimitedTwoBoneIKConstraint _constraint;

        private Vector3 _targetElbowDirection;
        private Vector3 _smoothElbowDirection;
        private readonly float _elbowSmoothing = 15f;

        private enum WristAxis
        {
            down, up,
            forward, backwards,
            left, right
        }

        private struct Influencer
        {
            public WristAxis UpAxis;
            public WristAxis RightAxis;
            public float Angle;

            public Influencer(WristAxis rightAxis, WristAxis upAxis, float angle)
            {
                RightAxis = rightAxis;
                UpAxis = upAxis;
                Angle = angle;
            }
        }

        readonly Influencer[] _influencers =
        {
            // Base influencers

            new(WristAxis.down, WristAxis.forward, 30f),
            new(WristAxis.down, WristAxis.backwards, 20f),
            new(WristAxis.down, WristAxis.left, 20f),
            new(WristAxis.down, WristAxis.right, 40f),

            new(WristAxis.up, WristAxis.forward, 0f),
            new(WristAxis.up, WristAxis.backwards, 0f),
            new(WristAxis.up, WristAxis.left, 0f),
            new(WristAxis.up, WristAxis.right, 0f),

            new(WristAxis.forward, WristAxis.down, -60f),
            new(WristAxis.forward, WristAxis.up, -30f),
            new(WristAxis.forward, WristAxis.left, -20f),
            new(WristAxis.forward, WristAxis.right, -40f),

            new(WristAxis.backwards, WristAxis.down, 110f),
            new(WristAxis.backwards, WristAxis.up, 50f),
            new(WristAxis.backwards, WristAxis.left, 50f),
            new(WristAxis.backwards, WristAxis.right, 80f),

            new(WristAxis.left, WristAxis.down, -20),
            new(WristAxis.left, WristAxis.up, 70f),
            new(WristAxis.left, WristAxis.forward, 0f),
            new(WristAxis.left, WristAxis.backwards, 90f),

            new(WristAxis.right, WristAxis.down, -20f),
            new(WristAxis.right, WristAxis.up, 20f),
            new(WristAxis.right, WristAxis.forward, 10f),
            new(WristAxis.right, WristAxis.backwards, 30f),

            // Additional influencers

            new(WristAxis.down, WristAxis.backwards, 140f),
            new(WristAxis.down, WristAxis.left, 150f),
            new(WristAxis.up, WristAxis.backwards, 170f),
            new(WristAxis.up, WristAxis.left, 160f),
            new(WristAxis.up, WristAxis.right, 150f),
            new(WristAxis.forward, WristAxis.down, 140f),
            new(WristAxis.forward, WristAxis.left, 170f),
            new(WristAxis.forward, WristAxis.up, 140f),
            new(WristAxis.backwards, WristAxis.left, 140f),
            new(WristAxis.left, WristAxis.down, 90f),
            new(WristAxis.left, WristAxis.forward, 90f),
            new(WristAxis.right, WristAxis.down, 70f),
            new(WristAxis.right, WristAxis.backwards, 150f)
        };

        private bool IsRightHand => _handedness == Handedness.Right;

        Vector3 GetAxis(WristAxis axis)
        {
            var down = -_controller.up;
            var forward = _controller.forward;
            var left = -_controller.right;

            if (!IsRightHand) left = -left;

            return axis switch
            {
                WristAxis.down => down,
                WristAxis.up => -down,
                WristAxis.forward => forward,
                WristAxis.backwards => -forward,
                WristAxis.left => left,
                WristAxis.right => -left,
                _ => Vector3.zero
            };
        }

        private void Start()
        {
            _constraint = GetComponent<LimitedTwoBoneIKConstraint>();

            _upperArmBone = _constraint.data.root;
            _lowerArmBone = _constraint.data.mid;
            _handBone = _constraint.data.tip;
            _hint = _constraint.data.hint;
        }

        private void Update()
        {
            // Find the elbow circle origin and radius
            var shoulderToHandDirection = (_handBone.position - _upperArmBone.position).normalized;

            // Find elbow circle properties
            var elbowOrigin = _upperArmBone.position + shoulderToHandDirection * Vector3.Dot(_lowerArmBone.position - _upperArmBone.position, shoulderToHandDirection);

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

                var influencerAngle = influencer.Angle;
                if (!IsRightHand) influencerAngle *= -1f;
                var influencerDirection = elbowDownRotation * Quaternion.AngleAxis(influencerAngle, shoulderToHandDirection) * Vector3.down;
                var weightDot = Mathf.Max(0f, Vector3.Dot(_targetElbowDirection, influencerDirection));

                var weightProduct = weightRight * weightUp * weightDot;

                angleSum += weightProduct * influencerAngle;
                weightSum += weightProduct;
            }

            // Predict elbow angle using influencer data average
            var predictedElbowAngle = 0f;
            if (weightSum > 0f)
                predictedElbowAngle = angleSum / weightSum;

            // Calculate target elbow direction
            _targetElbowDirection = elbowDownRotation * Quaternion.AngleAxis(predictedElbowAngle, shoulderToHandDirection) * Vector3.down;

            // Smooth elbow direction
            _smoothElbowDirection = Vector3.Slerp(_smoothElbowDirection, _targetElbowDirection, Time.deltaTime * _elbowSmoothing);
            Quaternion elbowRotation = Quaternion.LookRotation(shoulderToHandDirection, _smoothElbowDirection);

            // Apply smoothed direction to hint
            _hint.position = elbowOrigin + elbowRotation * Vector3.up;
        }
    }
}
