using UnityEngine;
using UnityEngine.Animations.Rigging;

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

            public Influencer(WristAxis upAxis, WristAxis rightAxis, float angle)
            {
                UpAxis = upAxis;
                RightAxis = rightAxis;
                Angle = angle;
            }
        }

        readonly Influencer[] _influencers =
        {
            new(WristAxis.X, WristAxis.Y, 50f),
            new(WristAxis.X, WristAxis.Yp, -45f),
            new(WristAxis.X, WristAxis.Z, 30f),
            new(WristAxis.X, WristAxis.Zp, 90f),
            new(WristAxis.Xp, WristAxis.Y, 90f),
            new(WristAxis.Xp, WristAxis.Yp, 90f),
            new(WristAxis.Xp, WristAxis.Z, 70f),
            new(WristAxis.Xp, WristAxis.Zp, 90f),
            new(WristAxis.Y, WristAxis.X, 20f),
            new(WristAxis.Y, WristAxis.Xp, 0f),
            new(WristAxis.Y, WristAxis.Z, 20f),
            new(WristAxis.Y, WristAxis.Zp, 0f), // 2 values
            new(WristAxis.Yp, WristAxis.X, 130f),
            new(WristAxis.Yp, WristAxis.Xp, 180f), // 2 values
            new(WristAxis.Yp, WristAxis.Z, 130f),
            new(WristAxis.Yp, WristAxis.Zp, 130f),
            new(WristAxis.Z, WristAxis.X, 0f), // 2 values
            new(WristAxis.Z, WristAxis.Xp, 10f),
            new(WristAxis.Z, WristAxis.Y, 50f), // 2 values
            new(WristAxis.Z, WristAxis.Yp, -30f), // 2 values
            new(WristAxis.Zp, WristAxis.X, 40f),
            new(WristAxis.Zp, WristAxis.Xp, 0f), // 2 values
            new(WristAxis.Zp, WristAxis.Y, 70f),
            new(WristAxis.Zp, WristAxis.Yp, 20f), // 2 values
        };

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
            Vector3 shoulderWristDisplacement = _handBone.position - _upperArmBone.position;
            Vector3 shoulderWristDirection = shoulderWristDisplacement.normalized;
            Vector3 elbowOrigin = _upperArmBone.position + shoulderWristDirection * Vector3.Dot(_lowerArmBone.position - _upperArmBone.position, shoulderWristDirection);
            float elbowRadius = Vector3.Distance(elbowOrigin, _lowerArmBone.position);

            //Find elbow down
            Quaternion elbowDownRotation = Quaternion.FromToRotation(_pelvis.forward, shoulderWristDirection);
            Vector3 elbowDownDirection = elbowDownRotation * Vector3.down;

            //Dot products
            float elbowForwardDot = Vector3.Dot(elbowDownDirection, _controller.forward);
            float elbowRightDot = Vector3.Dot(elbowDownDirection, _controller.right);
            float elbowUpDot = Vector3.Dot(elbowDownDirection, _controller.up);

            //Predict elbow direction
            Vector3 elbowDirection = Vector3.zero;

            #region dot product calculations
            float controllerRightShoulderWristDot = Vector3.Dot(_controller.right , shoulderWristDirection);
            float rightForwardInfluence = Mathf.Max(controllerRightShoulderWristDot, 0);
            float rightBackInfluence = -Mathf.Min(controllerRightShoulderWristDot, 0);

            elbowDirection += Mathf.Max(0, elbowUpDot) * rightForwardInfluence * (-_controller.forward);
            elbowDirection += Mathf.Max(0, -elbowUpDot) * rightForwardInfluence * (-_controller.up - _controller.forward);
            elbowDirection += Mathf.Max(0, elbowForwardDot) * rightForwardInfluence * (_controller.forward);
            elbowDirection += Mathf.Max(0, -elbowForwardDot) * rightForwardInfluence * (-_controller.up - _controller.forward);

            elbowDirection += Mathf.Max(0, elbowUpDot) * rightBackInfluence * (_controller.forward);
            elbowDirection += Mathf.Max(0, -elbowUpDot) * rightBackInfluence * (-_controller.up - _controller.forward);
            elbowDirection += Mathf.Max(0, elbowForwardDot) * rightBackInfluence * (-_controller.up + _controller.forward * 0.5f);
            elbowDirection += Mathf.Max(0, -elbowForwardDot) * rightBackInfluence * (-_controller.forward);

            float controllerUpShoulderWristDot = Vector3.Dot(_controller.up, shoulderWristDirection);
            float upForwardInfluence = Mathf.Max(controllerUpShoulderWristDot, 0);
            float upBackInfluence = -Mathf.Min(controllerUpShoulderWristDot, 0);

            elbowDirection += Mathf.Max(0, elbowRightDot) * upForwardInfluence * (_controller.right - _controller.forward * 0.5f);
            elbowDirection += Mathf.Max(0, -elbowRightDot) * upForwardInfluence * (-_controller.right - _controller.forward);
            elbowDirection += Mathf.Max(0, elbowForwardDot) * upForwardInfluence * (-_controller.right * 0.5f - _controller.forward);
            elbowDirection += Mathf.Max(0, -elbowForwardDot) * upForwardInfluence * (-_controller.forward);

            elbowDirection += Mathf.Max(0, elbowRightDot) * upBackInfluence * (-_controller.forward);
            elbowDirection += Mathf.Max(0, -elbowRightDot) * upBackInfluence * (-_controller.right - _controller.forward);
            elbowDirection += Mathf.Max(0, elbowForwardDot) * upBackInfluence * (-_controller.forward);
            elbowDirection += Mathf.Max(0, -elbowForwardDot) * upBackInfluence * (-_controller.forward);

            float controllerForwardShoulderWristDot = Vector3.Dot(_controller.forward, shoulderWristDirection);
            float forwardForwardInfluence = Mathf.Max(controllerForwardShoulderWristDot, 0);
            float forwardBackInfluence = -Mathf.Min(controllerForwardShoulderWristDot, 0);

            elbowDirection += forwardForwardInfluence * Mathf.Max(0, elbowRightDot) * (_controller.right - _controller.up);
            elbowDirection += forwardForwardInfluence * Mathf.Max(0, -elbowRightDot) * (-_controller.right);
            elbowDirection += forwardForwardInfluence * Mathf.Max(0, elbowUpDot) * (_controller.right - _controller.up);
            elbowDirection += forwardForwardInfluence * Mathf.Max(0, -elbowUpDot) * (-_controller.right - _controller.up);

            elbowDirection += forwardBackInfluence * Mathf.Max(0, elbowRightDot) * (_controller.right);
            elbowDirection += forwardBackInfluence * Mathf.Max(0, -elbowRightDot) * (-_controller.right);
            elbowDirection += forwardBackInfluence * Mathf.Max(0, elbowUpDot) * (-_controller.right);
            elbowDirection += forwardBackInfluence * Mathf.Max(0, -elbowUpDot) * (_controller.right - _controller.up);
            #endregion

            //Check limits
            float elbowAngle = Vector3.SignedAngle(elbowDirection, elbowDownDirection, shoulderWristDirection);
            elbowDirection = elbowDownRotation * Quaternion.AngleAxis(-elbowAngle , shoulderWristDirection) * Vector3.down;

            //Position elbow
            _smoothElbowDirection = Vector3.Slerp(_smoothElbowDirection, elbowDirection, Time.deltaTime * _elbowSmoothing);
            Quaternion elbowRotation = Quaternion.LookRotation(shoulderWristDirection, _smoothElbowDirection);

            _hint.position = elbowOrigin + elbowRotation * Vector3.up * elbowRadius;
        }
    }
}
