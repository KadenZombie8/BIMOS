using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SocialPlatforms.Impl;

namespace KadenZombie8.BIMOS.Rig
{
    [RequireComponent(typeof(TwoBoneIKConstraint))]
    public class ElbowPrediction : MonoBehaviour
    {
        [SerializeField]
        private Transform _controller;

        private Transform _upperArmBone;
        private Transform _lowerArmBone;
        private Transform _handBone;

        private Transform _hint;

        private TwoBoneIKConstraint _constraint;
        private Transform _pelvis;

        private Vector3 _smoothElbowDirection;
        private readonly float _elbowSmoothing = 20f;

        private Vector3 _refRight;
        private Vector3 _refUp;
        private Vector3 _refForward;

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
            public WristAxis ForwardAxis;
            public float Angle;

            public Influencer(WristAxis rightAxis, WristAxis upAxis, WristAxis forwardAxis, float angle)
            {
                RightAxis = rightAxis;
                UpAxis = upAxis;
                ForwardAxis = forwardAxis;
                Angle = angle;
            }
        }

        readonly Influencer[] _influencers =
        {
            new(WristAxis.X, WristAxis.Y, WristAxis.Z, 20f),
            new(WristAxis.X, WristAxis.Yp, WristAxis.Zp, 130f),
            new(WristAxis.X, WristAxis.Z, WristAxis.Yp, 0f), // 2 values
            new(WristAxis.X, WristAxis.Zp, WristAxis.Y, 40f),

            new(WristAxis.Xp, WristAxis.Y, WristAxis.Zp, 0f), // 2 values
            new(WristAxis.Xp, WristAxis.Yp, WristAxis.Z, 180f), // 2 values
            new(WristAxis.Xp, WristAxis.Z, WristAxis.Y, 10f),
            new(WristAxis.Xp, WristAxis.Zp, WristAxis.Yp, 0f), // 2 values

            new(WristAxis.Y, WristAxis.X, WristAxis.Zp, 50f),
            new(WristAxis.Y, WristAxis.Xp, WristAxis.Z, 90f),
            new(WristAxis.Y, WristAxis.Z, WristAxis.X, 50f), // 2 values
            new(WristAxis.Y, WristAxis.Zp, WristAxis.Xp, 70f),

            new(WristAxis.Yp, WristAxis.X, WristAxis.Z, -45f),
            new(WristAxis.Yp, WristAxis.Xp, WristAxis.Zp, 90f),
            new(WristAxis.Yp, WristAxis.Z, WristAxis.Xp, -30f), // 2 values
            new(WristAxis.Yp, WristAxis.Zp, WristAxis.X, 20f), // 2 values

            new(WristAxis.Z, WristAxis.X, WristAxis.Y, 30f),
            new(WristAxis.Z, WristAxis.Xp, WristAxis.Yp, 70f),
            new(WristAxis.Z, WristAxis.Y, WristAxis.Xp, 20f),
            new(WristAxis.Z, WristAxis.Yp, WristAxis.X, 130f),

            new(WristAxis.Zp, WristAxis.X, WristAxis.Yp, 90f),
            new(WristAxis.Zp, WristAxis.Xp, WristAxis.Y, 90f),
            new(WristAxis.Zp, WristAxis.Y, WristAxis.X, 0f), // 2 values
            new(WristAxis.Zp, WristAxis.Yp, WristAxis.Xp, 130f)
        };

        Vector3 GetAxis(WristAxis axis)
        {
            return axis switch
            {
                WristAxis.X => _controller.right,
                WristAxis.Xp => -_controller.right,
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
            var shoulderWristDisplacement = _handBone.position - _upperArmBone.position;
            var shoulderWristDirection = shoulderWristDisplacement.normalized;

            var elbowOrigin = _upperArmBone.position + shoulderWristDirection * Vector3.Dot(_lowerArmBone.position - _upperArmBone.position, shoulderWristDirection);
            var elbowRadius = Vector3.Distance(elbowOrigin, _lowerArmBone.position);

            //Find elbow down
            var elbowDownRotation = Quaternion.FromToRotation(_pelvis.forward, shoulderWristDirection);
            var elbowDownDirection = elbowDownRotation * Vector3.down;

            _refForward = shoulderWristDirection;
            _refUp = -elbowDownDirection;
            _refRight = Vector3.Cross(_refUp, _refForward);

            //Predict elbow direction
            var accumulatedAngle = 0f;
            var accumulatedWeight = 0f;

            var bestScore = 0f;
            var elbowAngle = 0f;
            Influencer bestInfluencer = new();

            foreach (var influencer in _influencers)
            {
                var influencerRight = GetAxis(influencer.RightAxis);
                var influencerUp = GetAxis(influencer.UpAxis);
                var influencerForward = GetAxis(influencer.ForwardAxis);

                var weightRight = Vector3.Dot(influencerRight, _refRight);
                var weightUp = Vector3.Dot(influencerUp, _refUp);
                var weightForward = Vector3.Dot(influencerForward, _refForward);

                var weightCombined = (weightRight + weightUp + weightForward) / 3f;

                accumulatedAngle += weightCombined * influencer.Angle;
                accumulatedWeight += weightCombined;

                if (weightCombined > bestScore)
                {
                    bestScore = weightCombined;
                    elbowAngle = influencer.Angle;
                    bestInfluencer = influencer;
                }
            }

            print(bestInfluencer.RightAxis + ", " + bestInfluencer.UpAxis + ", " + bestInfluencer.ForwardAxis);

            //Check limits

            //var elbowAngle = accumulatedWeight > 0f
            //    ? accumulatedAngle / accumulatedWeight
            //    : 0f;

            var elbowDirection = elbowDownRotation * Quaternion.AngleAxis(-elbowAngle , shoulderWristDirection) * Vector3.down;

            //Position elbow
            //_smoothElbowDirection = Vector3.Slerp(_smoothElbowDirection, elbowDirection, Time.deltaTime * _elbowSmoothing);
            Quaternion elbowRotation = Quaternion.LookRotation(shoulderWristDirection, elbowDirection);

            _hint.position = elbowOrigin + elbowRotation * Vector3.up * elbowRadius;
        }
    }
}
