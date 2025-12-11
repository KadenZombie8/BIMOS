using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsArm : MonoBehaviour
    {
        public ArmPhysicsBone Shoulder;
        public ArmPhysicsBone UpperArm;
        public ArmPhysicsBone LowerArm;
        public HandPhysicsBone Hand;

        public abstract class Segment
        {
            public HumanBodyBones Bone;
            public ConfigurableJoint Joint;

            [HideInInspector]
            public Transform Target;

            protected Transform AnimationBone;

            private Transform _upperArmBone;
            private float _maxLength;

            public virtual void Initialize(Animator animator, HumanBodyBones upperArmBone)
            {
                AnimationBone = animator.GetBoneTransform(Bone);
                _upperArmBone = animator.GetBoneTransform(upperArmBone);

                _maxLength = Vector3.Distance(AnimationBone.position, _upperArmBone.position) - 0.002f;

                var linearLimit = Joint.linearLimit;
                linearLimit.limit = _maxLength;
                Joint.linearLimit = linearLimit;
            }

            public void UpdateJoint()
            {
                var pelvis = Joint.connectedBody;
                var pelvisToUpperArm = pelvis.transform.InverseTransformPoint(_upperArmBone.position);
                Joint.connectedAnchor = pelvisToUpperArm;

                var pelvisToTarget = pelvis.transform.InverseTransformPoint(Target.position);
                Joint.targetPosition = Vector3.ClampMagnitude(pelvisToTarget - pelvisToUpperArm, _maxLength);
                Joint.targetRotation = Quaternion.Inverse(pelvis.rotation) * Target.rotation;
            }
        }

        [Serializable]
        public class ArmPhysicsBone : Segment
        {
            public CapsuleCollider Collider;

            public override void Initialize(Animator animator, HumanBodyBones shoulderBone)
            {
                base.Initialize(animator, shoulderBone);

                Target = AnimationBone;

                var childBone = AnimationBone.GetChild(0);
                Collider.height = Vector3.Distance(childBone.position, AnimationBone.position) + Collider.radius * 2f;
                Collider.center = (Collider.height / 2f - Collider.radius) * Vector3.up;
            }
        }

        [Serializable]
        public class HandPhysicsBone : Segment
        {
            public Transform Controller;
            public Vector3 PositionOffset;
            public Quaternion RotationOffset;

            public override void Initialize(Animator animator, HumanBodyBones shoulderBone)
            {
                base.Initialize(animator, shoulderBone);

                Target = Controller;
            }
        }

        private void Start()
        {
            var rig = BIMOSRig.Instance;
            var animator = rig.AnimationRig.Animator;

            Shoulder.Initialize(animator, UpperArm.Bone);
            UpperArm.Initialize(animator, UpperArm.Bone);
            LowerArm.Initialize(animator, UpperArm.Bone);
            Hand.Initialize(animator, UpperArm.Bone);

            LowerArm.Collider.height -= LowerArm.Collider.radius;
            LowerArm.Collider.center += LowerArm.Collider.radius / 2f * Vector3.down;
        }

        private void FixedUpdate()
        {
            //Shoulder.UpdateJoint();
            //UpperArm.UpdateJoint();
            //LowerArm.UpdateJoint();
            Hand.UpdateJoint();
        }
    }
}
