using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsArm : MonoBehaviour
    {
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

            protected Transform UpperArmBone;
            protected float MaxLength;

            public virtual void Initialize(Animator animator, HumanBodyBones upperArmBone)
            {
                AnimationBone = animator.GetBoneTransform(Bone);
                UpperArmBone = animator.GetBoneTransform(upperArmBone);

                MaxLength = Vector3.Distance(AnimationBone.position, UpperArmBone.position) - 0.002f;

                var linearLimit = Joint.linearLimit;
                linearLimit.limit = MaxLength;
                Joint.linearLimit = linearLimit;
            }

            public virtual void UpdateJoint()
            {
                var pelvis = Joint.connectedBody;
                var pelvisToUpperArm = pelvis.transform.InverseTransformPoint(UpperArmBone.position);
                Joint.connectedAnchor = pelvisToUpperArm;

                var pelvisToTarget = pelvis.transform.InverseTransformPoint(Target.position);
                Joint.targetPosition = Vector3.ClampMagnitude(pelvisToTarget - Joint.connectedAnchor, MaxLength);
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
                RotationOffset = Quaternion.identity;
            }

            public override void UpdateJoint()
            {
                var pelvis = Joint.connectedBody;
                var pelvisToUpperArm = pelvis.transform.InverseTransformPoint(UpperArmBone.position);
                Joint.connectedAnchor = pelvisToUpperArm;

                var targetPosition = Target.TransformPoint(PositionOffset);
                var targetRotation = Target.rotation * RotationOffset;

                var pelvisToTarget = pelvis.transform.InverseTransformPoint(targetPosition);
                Joint.targetPosition = Vector3.ClampMagnitude(pelvisToTarget - Joint.connectedAnchor, MaxLength);
                Joint.targetRotation = Quaternion.Inverse(pelvis.rotation) * targetRotation;
            }
        }

        private void Start()
        {
            var rig = BIMOSRig.Instance;
            var animator = rig.AnimationRig.Animator;

            UpperArm.Initialize(animator, UpperArm.Bone);
            LowerArm.Initialize(animator, UpperArm.Bone);
            Hand.Initialize(animator, UpperArm.Bone);

            LowerArm.Collider.height -= LowerArm.Collider.radius;
            LowerArm.Collider.center += LowerArm.Collider.radius / 2f * Vector3.down;
        }

        private void FixedUpdate() => Hand.UpdateJoint();

        private void LateUpdate()
        {
            UpperArm.UpdateJoint();
            LowerArm.UpdateJoint();
        }
    }
}
