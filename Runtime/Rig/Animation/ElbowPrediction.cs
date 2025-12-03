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
            new(WristAxis.X, WristAxis.Y, 20f),
            new(WristAxis.X, WristAxis.Yp, 130f),
            new(WristAxis.X, WristAxis.Z, 0f), // 2 values
            new(WristAxis.X, WristAxis.Zp, 40f),

            new(WristAxis.Xp, WristAxis.Y, 0f), // 2 values
            new(WristAxis.Xp, WristAxis.Yp, 180f), // 2 values
            new(WristAxis.Xp, WristAxis.Z, 10f),
            new(WristAxis.Xp, WristAxis.Zp, 0f), // 2 values

            new(WristAxis.Y, WristAxis.X, 50f),
            new(WristAxis.Y, WristAxis.Xp, 90f),
            new(WristAxis.Y, WristAxis.Z, 50f), // 2 values
            new(WristAxis.Y, WristAxis.Zp, 90f),

            new(WristAxis.Yp, WristAxis.X, -45f),
            new(WristAxis.Yp, WristAxis.Xp, 90f),
            new(WristAxis.Yp, WristAxis.Z, -30f), // 2 values
            new(WristAxis.Yp, WristAxis.Zp, 20f), // 2 values

            new(WristAxis.Z, WristAxis.X, 30f),
            new(WristAxis.Z, WristAxis.Xp, 70f),
            new(WristAxis.Z, WristAxis.Y, 20f),
            new(WristAxis.Z, WristAxis.Yp, 130f),

            new(WristAxis.Zp, WristAxis.X, 90f),
            new(WristAxis.Zp, WristAxis.Xp, 90f),
            new(WristAxis.Zp, WristAxis.Y, 0f), // 2 values
            new(WristAxis.Zp, WristAxis.Yp, 130f)
        };

        Vector3 GetAxis(WristAxis axis)
        {
            var isLeftHand = _handedness == Handedness.Left;
            return axis switch
            {
                WristAxis.X => isLeftHand ? _controller.right : -_controller.right,
                WristAxis.Xp => isLeftHand ? -_controller.right : _controller.right,
                WristAxis.Y => _controller.up,
                WristAxis.Yp => -_controller.up,
                WristAxis.Z => _controller.forward,
                WristAxis.Zp => -_controller.forward,
                _ => Vector3.zero
            };
        }

        private void Start()
        {
            _constraint = GetComponent<TwoBoneIKConstraint>();

            //Find arm bone lengths
            _upperArmBone = _constraint.data.root;
            _lowerArmBone = _constraint.data.mid;
            _handBone = _constraint.data.tip;
            _hint = _constraint.data.hint;

            _pelvis = BIMOSRig.Instance.PhysicsRig.Rigidbodies.Pelvis.transform;
        }

        private void LateUpdate()
        {
            //Find the elbow circle origin and radius
            var shoulderToHandDirection = (_handBone.position - _upperArmBone.position).normalized;

            var elbowOrigin = _upperArmBone.position + shoulderToHandDirection * Vector3.Dot(_lowerArmBone.position - _upperArmBone.position, shoulderToHandDirection);
            var elbowRadius = Vector3.Distance(elbowOrigin, _lowerArmBone.position);

            //Find elbow down
            var elbowDownRotation = Quaternion.FromToRotation(_pelvis.forward, shoulderToHandDirection);

            //Predict elbow direction
            var accumulatedAngle = 0f;
            var accumulatedWeight = 0f;

            var refForward = shoulderToHandDirection;
            var refUp = elbowDownRotation * Vector3.up;
            var refRight = Vector3.Cross(refUp, refForward);

            var isRightHand = _handedness == Handedness.Right;
            if (isRightHand) refRight *= -1f;

            foreach (var influencer in _influencers)
            {
                var influencerRight = GetAxis(influencer.RightAxis);
                var influencerUp = GetAxis(influencer.UpAxis);

                var weightRight = Mathf.Max(0f, Vector3.Dot(influencerRight, refRight));
                var weightUp = Mathf.Max(0f, Vector3.Dot(influencerUp, refUp));

                var weightCombined = weightRight * weightUp;

                accumulatedAngle += weightCombined * influencer.Angle;
                accumulatedWeight += weightCombined;
            }

            // TODO: Check limits

            var elbowAngle = accumulatedWeight > 0f
                ? accumulatedAngle / accumulatedWeight
                : 0f;

            if (isRightHand) elbowAngle *= -1f;

            var elbowDirection = elbowDownRotation * Quaternion.AngleAxis(-elbowAngle, shoulderToHandDirection) * Vector3.down;

            //Position elbow
            _smoothElbowDirection = Vector3.Slerp(_smoothElbowDirection, elbowDirection, Time.deltaTime * _elbowSmoothing);
            Quaternion elbowRotation = Quaternion.LookRotation(shoulderToHandDirection, _smoothElbowDirection);

            _hint.position = elbowOrigin + elbowRotation * Vector3.up * elbowRadius;
        }
    }
}
