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

        [SerializeField]
        private AnimationCurve _curve;

        private Transform _upperArmBone;
        private Transform _lowerArmBone;
        private Transform _handBone;

        private Transform _hint;

        private LimitedTwoBoneIKConstraint _constraint;

        private Vector3 _targetElbowDirection;
        private Vector3 _smoothElbowDirection;
        private readonly float _elbowSmoothing = 15f;

        private enum HandAxis
        {
            Ulnar, Thumb,
            Fingers, Wrist,
            Palm, Dorsum
        }

        private struct Influencer
        {
            public HandAxis UpAxis;
            public HandAxis RightAxis;
            public float Angle;

            public Influencer(HandAxis rightAxis, HandAxis upAxis, float angle)
            {
                RightAxis = rightAxis;
                UpAxis = upAxis;
                Angle = angle;
            }
        }

        readonly Influencer[] _influencers =
        {
            // Base influencers

            new(HandAxis.Ulnar, HandAxis.Fingers, 30f),
            new(HandAxis.Ulnar, HandAxis.Wrist, 20f),
            new(HandAxis.Ulnar, HandAxis.Palm, 20f),
            new(HandAxis.Ulnar, HandAxis.Dorsum, 40f),

            new(HandAxis.Thumb, HandAxis.Fingers, 0f),
            new(HandAxis.Thumb, HandAxis.Wrist, 0f),
            new(HandAxis.Thumb, HandAxis.Palm, 0f),
            new(HandAxis.Thumb, HandAxis.Dorsum, 0f),

            new(HandAxis.Fingers, HandAxis.Ulnar, -60f),
            new(HandAxis.Fingers, HandAxis.Thumb, -30f),
            new(HandAxis.Fingers, HandAxis.Palm, -20f),
            new(HandAxis.Fingers, HandAxis.Dorsum, -40f),

            new(HandAxis.Wrist, HandAxis.Ulnar, 110f),
            new(HandAxis.Wrist, HandAxis.Thumb, 50f),
            new(HandAxis.Wrist, HandAxis.Palm, 50f),
            new(HandAxis.Wrist, HandAxis.Dorsum, 80f),

            new(HandAxis.Palm, HandAxis.Ulnar, -20),
            new(HandAxis.Palm, HandAxis.Thumb, 70f),
            new(HandAxis.Palm, HandAxis.Fingers, 0f),
            new(HandAxis.Palm, HandAxis.Wrist, 90f),

            new(HandAxis.Dorsum, HandAxis.Ulnar, -20f),
            new(HandAxis.Dorsum, HandAxis.Thumb, 20f),
            new(HandAxis.Dorsum, HandAxis.Fingers, 10f),
            new(HandAxis.Dorsum, HandAxis.Wrist, 30f),

            // Additional influencers

            new(HandAxis.Ulnar, HandAxis.Wrist, 140f),
            new(HandAxis.Ulnar, HandAxis.Palm, 150f),
            new(HandAxis.Thumb, HandAxis.Wrist, 170f),
            new(HandAxis.Thumb, HandAxis.Palm, 160f),
            new(HandAxis.Thumb, HandAxis.Dorsum, 150f),
            new(HandAxis.Fingers, HandAxis.Ulnar, 140f),
            new(HandAxis.Fingers, HandAxis.Palm, 170f),
            new(HandAxis.Fingers, HandAxis.Thumb, 140f),
            new(HandAxis.Wrist, HandAxis.Palm, 140f),
            new(HandAxis.Palm, HandAxis.Ulnar, 90f),
            new(HandAxis.Palm, HandAxis.Fingers, 90f),
            new(HandAxis.Dorsum, HandAxis.Ulnar, 70f),
            new(HandAxis.Dorsum, HandAxis.Wrist, 150f)
        };

        private bool IsRightHand => _handedness == Handedness.Right;

        Vector3 GetAxis(HandAxis axis)
        {
            var thumb = _controller.up;
            var fingers = _controller.forward;
            var palm = -_controller.right;

            if (!IsRightHand) palm = -palm;

            return axis switch
            {
                HandAxis.Ulnar => -thumb,
                HandAxis.Thumb => thumb,
                HandAxis.Fingers => fingers,
                HandAxis.Wrist => -fingers,
                HandAxis.Palm => palm,
                HandAxis.Dorsum => -palm,
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

                var influencerDirection = Quaternion.AngleAxis(influencerAngle, shoulderToHandDirection) * elbowDownRotation * Vector3.down;
                var weightDot = Mathf.Max(0f, Vector3.Dot(_targetElbowDirection, influencerDirection));

                var weightProduct = _curve.Evaluate(weightRight * weightUp * weightDot);

                angleSum += weightProduct * influencerAngle;
                weightSum += weightProduct;
            }

            // Predict elbow angle using influencer data average
            var predictedElbowAngle = 0f;
            if (weightSum > 0f)
                predictedElbowAngle = angleSum / weightSum;

            // Calculate target elbow direction
            _targetElbowDirection = Quaternion.AngleAxis(predictedElbowAngle, shoulderToHandDirection) * elbowDownRotation * Vector3.down;

            // Smooth elbow direction
            _smoothElbowDirection = Vector3.Slerp(_smoothElbowDirection, _targetElbowDirection, Time.deltaTime * _elbowSmoothing);
            Quaternion elbowRotation = Quaternion.LookRotation(shoulderToHandDirection, _smoothElbowDirection);

            // Apply smoothed direction to hint
            _hint.position = elbowOrigin + elbowRotation * Vector3.up;
        }
    }
}
