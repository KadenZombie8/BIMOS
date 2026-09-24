using KadenZombie8.BIMOS.Rig.Movement;
using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class PhysicsArm : MonoBehaviour
    {
        public ArmPhysicsBone UpperArm;
        public LowerArmPhysicsBone LowerArm;
        public HandPhysicsBone Hand;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private VirtualTurning _virtualTurning;

        [Serializable]
        public abstract class Segment
        {
            public HumanBodyBones Bone;
            public ConfigurableJoint Joint;

            [HideInInspector]
            public Transform Target;

            protected Transform AnimationBone;
            protected Transform UpperArmBone;
            protected float MaxLength;

            protected Vector3 PreviousPosition;
            protected Quaternion PreviousRotation;

            protected VirtualTurning VirtualTurning;

            public virtual void Initialize(Animator animator, HumanBodyBones upperArmBone, VirtualTurning virtualTurning)
            {
                AnimationBone = animator.GetBoneTransform(Bone);
                UpperArmBone = animator.GetBoneTransform(upperArmBone);

                MaxLength = Vector3.Distance(AnimationBone.position, UpperArmBone.position) - 0.002f;

                var linearLimit = Joint.linearLimit;
                linearLimit.limit = MaxLength;
                Joint.linearLimit = linearLimit;

                VirtualTurning = virtualTurning;
            }

            public virtual void UpdateJoint()
            {
                var parent = Joint.connectedBody;
                var pelvisToUpperArm = parent.transform.InverseTransformPoint(UpperArmBone.position);
                Joint.connectedAnchor = pelvisToUpperArm;

                var pelvisToTarget = parent.transform.InverseTransformPoint(Target.position);
                Joint.targetPosition = pelvisToTarget - Joint.connectedAnchor;
                Joint.targetRotation = Quaternion.Inverse(parent.rotation) * Target.rotation;

                Target.GetPositionAndRotation(out var currentPosition, out var currentRotation);

                Joint.targetVelocity = CalculateVelocity(parent, currentPosition, ref PreviousPosition);
                Joint.targetAngularVelocity = CalculateAngularVelocity(parent, currentRotation, ref PreviousRotation);
            }

            protected Vector3 CalculateVelocity(Rigidbody parent, Vector3 currentPosition, ref Vector3 previousPosition)
            {
                var displacement = currentPosition - previousPosition;
                var worldVelocity = displacement / Time.fixedDeltaTime - parent.linearVelocity;

                var rotation = Quaternion.AngleAxis(VirtualTurning.FixedTurnRate, Vector3.up);
                var localPosition = Target.position - VirtualTurning.ControllerRig.transform.position;

                worldVelocity += rotation * localPosition - localPosition;

                var localVelocity = parent.transform.InverseTransformDirection(worldVelocity);

                previousPosition = currentPosition;
                return localVelocity;
            }

            protected Vector3 CalculateAngularVelocity(Rigidbody parent, Quaternion currentRotation, ref Quaternion previousRotation)
            {
                var deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);
                deltaRotation.ToAngleAxis(out var angle, out var axis);

                if (angle > 180f)
                    angle -= 360f;

                var angularDisplacement = angle * Mathf.Deg2Rad * axis;
                var worldAngularVelocity = angularDisplacement / Time.fixedDeltaTime - parent.angularVelocity;

                var localAngularVelocity = parent.transform.InverseTransformDirection(worldAngularVelocity);

                previousRotation = currentRotation;
                return localAngularVelocity;
            }
        }

        [Serializable]
        public class ArmPhysicsBone : Segment
        {
            public CapsuleCollider Collider;

            public override void Initialize(Animator animator, HumanBodyBones shoulderBone, VirtualTurning virtualTurning)
            {
                base.Initialize(animator, shoulderBone, virtualTurning);
                Target = AnimationBone;

                var childBone = AnimationBone.GetChild(0);
                Collider.height = Vector3.Distance(childBone.position, AnimationBone.position) + Collider.radius * 2f;
                Collider.center = (Collider.height / 2f - Collider.radius) * Vector3.up;

                Joint.connectedAnchor = AnimationBone.localPosition;
            }
        }

        [Serializable]
        public class LowerArmPhysicsBone : Segment
        {
            public CapsuleCollider Collider;

            public override void Initialize(Animator animator, HumanBodyBones shoulderBone, VirtualTurning virtualTurning)
            {
                base.Initialize(animator, shoulderBone, virtualTurning);
                Target = AnimationBone;

                var childBone = AnimationBone.GetChild(0);
                Collider.height = Vector3.Distance(childBone.position, AnimationBone.position) + Collider.radius * 2f;
                Collider.center = (Collider.height / 2f - Collider.radius) * Vector3.up;

                Joint.connectedAnchor = AnimationBone.localPosition;
            }

            public override void UpdateJoint()
            {
                var parent = Joint.connectedBody;
                var parentToTarget = parent.transform.InverseTransformPoint(Target.position);

                Joint.targetPosition = parentToTarget - Joint.connectedAnchor;
                Joint.targetRotation = Quaternion.Inverse(parent.rotation) * Target.rotation;

                Target.GetPositionAndRotation(out var currentPosition, out var currentRotation);

                Joint.targetVelocity = CalculateVelocity(parent, currentPosition, ref PreviousPosition);
                Joint.targetAngularVelocity = CalculateAngularVelocity(parent, currentRotation, ref PreviousRotation);
            }
        }

        [Serializable]
        public class HandPhysicsBone : Segment
        {
            public Transform Controller;
            public Vector3 PositionOffset;
            public Quaternion RotationOffset;
            public ConfigurableJoint LockJoint;

            public override void Initialize(Animator animator, HumanBodyBones shoulderBone, VirtualTurning virtualTurning)
            {
                base.Initialize(animator, shoulderBone, virtualTurning);
                Target = Controller;
                RotationOffset = Quaternion.identity;

                LockJoint.connectedAnchor = AnimationBone.localPosition;
            }

            public override void UpdateJoint()
            {
                var parent = Joint.connectedBody;

                var targetPosition = Target.TransformPoint(PositionOffset);
                var targetRotation = Target.rotation * RotationOffset;

                var parentToTarget = parent.transform.InverseTransformPoint(targetPosition);

                Joint.targetPosition = parentToTarget - Joint.connectedAnchor;
                Joint.targetRotation = Quaternion.Inverse(parent.rotation) * targetRotation;

                Target.GetPositionAndRotation(out var currentPosition, out var currentRotation);

                Joint.targetVelocity = CalculateVelocity(parent, currentPosition, ref PreviousPosition);
                Joint.targetAngularVelocity = CalculateAngularVelocity(parent, currentRotation, ref PreviousRotation);
            }
        }

        private void Start()
        {
            UpperArm.Initialize(_animator, UpperArm.Bone, _virtualTurning);
            LowerArm.Initialize(_animator, UpperArm.Bone, _virtualTurning);
            Hand.Initialize(_animator, UpperArm.Bone, _virtualTurning);

            LowerArm.Collider.height -= LowerArm.Collider.radius;
            LowerArm.Collider.center += LowerArm.Collider.radius / 2f * Vector3.down;
        }

        private void FixedUpdate()
        {
            Hand.UpdateJoint();
        }

        private void LateUpdate()
        {
            UpperArm.UpdateJoint();
            LowerArm.UpdateJoint();
        }
    }
}
