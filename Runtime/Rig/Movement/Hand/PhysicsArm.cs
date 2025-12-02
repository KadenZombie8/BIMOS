using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsArm : MonoBehaviour
    {
        [SerializeField]
        private Handedness _handedness;

        [SerializeField]
        private ArmSegment _upperArm;

        [SerializeField]
        private ArmSegment _lowerArm;

        [Serializable]
        private struct ArmSegment
        {
            public CapsuleCollider Collider;
            public ConfigurableJoint Joint;
        }

        private Transform _animationUpperArm;
        private Transform _animationLowerArm;

        private void Start()
        {
            var rig = BIMOSRig.Instance;

            var animator = rig.AnimationRig.Animator;

            _animationUpperArm = _handedness == Handedness.Left ? animator.GetBoneTransform(HumanBodyBones.LeftUpperArm) : animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            _animationLowerArm = _handedness == Handedness.Left ? animator.GetBoneTransform(HumanBodyBones.LeftLowerArm) : animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            var animationHand = _handedness == Handedness.Left ? animator.GetBoneTransform(HumanBodyBones.LeftHand) : animator.GetBoneTransform(HumanBodyBones.RightHand);

            InitializeCollider(_upperArm, _animationUpperArm, _animationLowerArm);
            InitializeCollider(_lowerArm, _animationLowerArm, animationHand);
        }
        
        private void InitializeCollider(ArmSegment segment, Transform upperBone, Transform lowerBone)
        {
            var collider = segment.Collider;
            collider.height = Vector3.Distance(upperBone.position, lowerBone.position) + segment.Collider.radius * 2f;
            collider.center = (collider.height / 2f - segment.Collider.radius) * Vector3.up;
        }

        private void FixedUpdate()
        {
            TargetJoint(_upperArm, _animationUpperArm);
            TargetJoint(_lowerArm, _animationLowerArm);
        }

        private void TargetJoint(ArmSegment segment, Transform target)
        {
            var joint = segment.Joint;
            var connectedBody = joint.connectedBody;
            joint.targetPosition = connectedBody.transform.InverseTransformPoint(target.position);
            joint.targetRotation = Quaternion.Inverse(connectedBody.rotation) * target.rotation;
        }
    }
}
