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
            new(WristAxis.X, WristAxis.Y, 0f), // 2 values
            new(WristAxis.X, WristAxis.Yp, 180f), // 2 values
            new(WristAxis.X, WristAxis.Z, 10f),
            new(WristAxis.X, WristAxis.Zp, 0f), // 2 values

            new(WristAxis.Xp, WristAxis.Y, 20f),
            new(WristAxis.Xp, WristAxis.Yp, 130f),
            new(WristAxis.Xp, WristAxis.Z, 0f), // 2 values
            new(WristAxis.Xp, WristAxis.Zp, 40f),

            new(WristAxis.Y, WristAxis.X, 90f),
            new(WristAxis.Y, WristAxis.Xp, 50f),
            new(WristAxis.Y, WristAxis.Z, 50f), // 2 values
            new(WristAxis.Y, WristAxis.Zp, 90f),

            new(WristAxis.Yp, WristAxis.X, 90f),
            new(WristAxis.Yp, WristAxis.Xp, -45f),
            new(WristAxis.Yp, WristAxis.Z, -30f), // 2 values
            new(WristAxis.Yp, WristAxis.Zp, 20f), // 2 values

            new(WristAxis.Z, WristAxis.X, 70f),
            new(WristAxis.Z, WristAxis.Xp, 30f),
            new(WristAxis.Z, WristAxis.Y, 20f),
            new(WristAxis.Z, WristAxis.Yp, 130f),

            new(WristAxis.Zp, WristAxis.X, 90f),
            new(WristAxis.Zp, WristAxis.Xp, 90f),
            new(WristAxis.Zp, WristAxis.Y, 0f), // 2 values
            new(WristAxis.Zp, WristAxis.Yp, 130f)
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
            var refRight = Vector3.Cross(shoulderToHandDirection, refUp);

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

                angleSum += weightProduct * influencer.Angle;
                weightSum += weightProduct;
            }

            // Predict elbow angle using influencer data average
            var predictedElbowAngle = 0f;
            if (weightSum > 0f)
                predictedElbowAngle = angleSum / weightSum;

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
