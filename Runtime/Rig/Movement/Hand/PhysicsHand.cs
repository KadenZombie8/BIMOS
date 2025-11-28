using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsHand : MonoBehaviour
    {
        public Transform Target, Controller;
        public Vector3 TargetOffsetPosition;
        public Quaternion TargetOffsetRotation;

        [SerializeField]
        private float _pGain;

        [SerializeField]
        private float _dGain;

        private PDVector3 _pdVector3;

        [SerializeField]
        private BIMOSRig _player;

        private Rigidbody _rigidbody;
        private ConfigurableJoint _handJoint;
        private Rigidbody _pelvis;

        private void Start()
        {
            _player = GetComponentInParent<BIMOSRig>();

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.solverIterations = 60;
            _rigidbody.solverVelocityIterations = 10;

            TargetOffsetRotation = Quaternion.identity;
            _handJoint = GetComponent<ConfigurableJoint>();

            _pelvis = _handJoint.connectedBody;

            _pdVector3 = new PDVector3(_pGain, _dGain);
        }

        private void FixedUpdate()
        {
            _handJoint.targetPosition = _pelvis.transform.InverseTransformPoint(Target.position);
            _handJoint.targetRotation = Quaternion.Inverse(_pelvis.rotation) * Target.rotation;

            // SM Target Velocity Logic.
            //_pdVector3.UpdateProportionalGain(_pGain);
            //_pdVector3.UpdateDerivativeGain(_dGain);
            //_handJoint.targetVelocity = _pdVector3.CalculatePD(_handJoint.transform.position, Target.TransformPoint(TargetOffsetPosition),
            //    Time.fixedDeltaTime);
        }
    }
}