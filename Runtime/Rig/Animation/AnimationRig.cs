using KadenZombie8.BIMOS.AnimationRigging;
using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.Rig.Animation
{
    public class AnimationRig : MonoBehaviour
    {
        public LegAnimation Feet;

        [HideInInspector]
        public float AvatarEyeHeight; //The headset's default height above the floor

        [SerializeField]
        private Transform _eyeCenter;

        [SerializeField]
        private Transform _headCameraOffset;

        [SerializeField]
        Transform _leftWrist, _rightWrist;

        public AnimationRigTransforms Transforms;

        [SerializeField]
        private bool _shrinkHeadBone = true;

        [Header("Transforms")]

        public AnimationRigConstraints Constraints;

        [HideInInspector]
        public Animator Animator;

        public Head Head;

        private RigBuilder _rigBuilder;

        public Transform PalmOffset;

        public void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            _rigBuilder = GetComponentInChildren<RigBuilder>();
            UpdateConstraints();
            _rigBuilder.Build();
        }

        private void UpdateConstraints()
        {
            // Head
            var headBone = Animator.GetBoneTransform(HumanBodyBones.Head);
            var neckBone = Animator.GetBoneTransform(HumanBodyBones.Neck);
            if (neckBone)
                headBone = neckBone;

            AvatarEyeHeight = _eyeCenter.localPosition.y;
            _headCameraOffset.localPosition = _eyeCenter.InverseTransformPoint(headBone.position);

            Transforms.Head = headBone;
            Constraints.Head.data.constrainedObject = headBone;

            if (_shrinkHeadBone)
                headBone.localScale = Vector3.zero;

            // Arms
            Transform leftShoulder = Animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            if (leftShoulder)
                Constraints.LeftShoulder.data.constrainedObject = Animator.GetBoneTransform(HumanBodyBones.LeftShoulder);

            Transform rightShoulder = Animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            if (rightShoulder)
                Constraints.RightShoulder.data.constrainedObject = Animator.GetBoneTransform(HumanBodyBones.RightShoulder);

            Constraints.LeftArm.data.root = Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            Constraints.LeftArm.data.mid = Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            Constraints.LeftArm.data.tip = Animator.GetBoneTransform(HumanBodyBones.LeftHand);

            Constraints.RightArm.data.root = Animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Constraints.RightArm.data.mid = Animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            Constraints.RightArm.data.tip = Animator.GetBoneTransform(HumanBodyBones.RightHand);

            ApplyPalmOffset();

            // Legs
            Constraints.LeftLeg.data.root = Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            Constraints.LeftLeg.data.mid = Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            Constraints.LeftLeg.data.tip = Animator.GetBoneTransform(HumanBodyBones.LeftFoot);

            Constraints.RightLeg.data.root = Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            Constraints.RightLeg.data.mid = Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            Constraints.RightLeg.data.tip = Animator.GetBoneTransform(HumanBodyBones.RightFoot);

            // Torso
            Transforms.Hips = Animator.GetBoneTransform(HumanBodyBones.Hips);
            Constraints.Hip.data.constrainedObject = Transforms.Hips;

            Constraints.Chest.data.constrainedObject = Transforms.Hips;
        }

        public void ApplyPalmOffset()
        {
            var rightHand = Animator.GetBoneTransform(HumanBodyBones.RightHand);

            _rightWrist.SetLocalPositionAndRotation(
                PalmOffset.transform.InverseTransformPoint(rightHand.position),
                Quaternion.Inverse(PalmOffset.transform.rotation) * rightHand.rotation
            );

            var rightPalm = _rightWrist.parent;

            var mirroredForward = rightPalm.InverseTransformDirection(-_rightWrist.forward);
            var mirroredUp = rightPalm.InverseTransformDirection(_rightWrist.up);

            var rotation = Quaternion.LookRotation(mirroredForward, mirroredUp);

            var position = _rightWrist.localPosition;
            position.x *= -1f;

            _leftWrist.SetLocalPositionAndRotation(
                position,
                rotation
            );
        }
    }

    [Serializable]
    public struct AnimationRigTransforms
    {
        public Transform Character;
        public Transform LeftFootTarget;
        public Transform RightFootTarget;
        public Transform LeftFootAnchor;
        public Transform RightFootAnchor;
        public Transform Hips;
        public Transform HipsIK;
        public Transform Head;
    }

    [Serializable]
    public struct AnimationRigConstraints
    {
        public MultiAimConstraint LeftShoulder;
        public MultiAimConstraint RightShoulder;
        public LimitedTwoBoneIKConstraint LeftArm;
        public LimitedTwoBoneIKConstraint RightArm;
        public LimitedTwoBoneIKConstraint LeftLeg;
        public LimitedTwoBoneIKConstraint RightLeg;
        public OverrideTransform Hip;
        public MultiAimConstraint Chest;
        public MultiParentConstraint Head;
    }
}
