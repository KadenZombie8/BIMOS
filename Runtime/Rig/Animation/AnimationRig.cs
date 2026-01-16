using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace KadenZombie8.BIMOS.Rig
{
    public class AnimationRig : MonoBehaviour
    {
        public Feet Feet;

        [HideInInspector]
        public float AvatarEyeHeight; //The headset's default height above the floor

        [SerializeField]
        private Transform _eyeCenter;

        [SerializeField]
        private Transform _headCameraOffset;

        [SerializeField]
        Transform _leftWrist, _rightWrist;

        [SerializeField]
        Transform _leftPalm, _rightPalm;

        public AnimationRigTransforms Transforms;

        [SerializeField]
        private bool _shrinkHeadBone = true;

        [Header("Transforms")]

        [SerializeField]
        private AnimationRigConstraints _constraints;

        [HideInInspector]
        public Animator Animator;

        public Head Head;

        private RigBuilder _rigBuilder;

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
            _constraints.Head.data.constrainedObject = headBone;

            if (_shrinkHeadBone)
                headBone.localScale = Vector3.zero;

            // Arms
            Transform leftShoulder = Animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            if (leftShoulder)
                _constraints.LeftShoulder.data.constrainedObject = Animator.GetBoneTransform(HumanBodyBones.LeftShoulder);

            Transform rightShoulder = Animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            if (rightShoulder)
                _constraints.RightShoulder.data.constrainedObject = Animator.GetBoneTransform(HumanBodyBones.RightShoulder);

            _constraints.LeftArm.data.root = Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            _constraints.LeftArm.data.mid = Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            _constraints.LeftArm.data.tip = Animator.GetBoneTransform(HumanBodyBones.LeftHand);

            _constraints.RightArm.data.root = Animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            _constraints.RightArm.data.mid = Animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            _constraints.RightArm.data.tip = Animator.GetBoneTransform(HumanBodyBones.RightHand);

            // Hands
            Transform leftHand = Animator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform leftMiddleProximal = Animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
            GameObject leftPalm = new();
            leftPalm.transform.SetPositionAndRotation(
                Vector3.Lerp(leftHand.position, leftMiddleProximal.position, 0.5f),
                Quaternion.Lerp(leftHand.rotation, leftMiddleProximal.rotation, 0.5f)
            );
            _leftWrist.SetLocalPositionAndRotation(
                leftPalm.transform.InverseTransformPoint(leftHand.position),
                Quaternion.Inverse(leftPalm.transform.rotation) * leftHand.rotation
            );
            _leftPalm.SetLocalPositionAndRotation(
                leftHand.InverseTransformPoint(leftPalm.transform.position),
                Quaternion.Inverse(leftHand.rotation) * leftPalm.transform.rotation
            );
            Destroy(leftPalm);

            Transform rightHand = Animator.GetBoneTransform(HumanBodyBones.RightHand);
            Transform rightMiddleProximal = Animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
            GameObject rightPalm = new();
            rightPalm.transform.SetPositionAndRotation(
                Vector3.Lerp(rightHand.position, rightMiddleProximal.position, 0.5f),
                Quaternion.Lerp(rightHand.rotation, rightMiddleProximal.rotation, 0.5f)
            );
            _rightWrist.SetLocalPositionAndRotation(
                rightPalm.transform.InverseTransformPoint(rightHand.position),
                Quaternion.Inverse(rightPalm.transform.rotation) * rightHand.rotation
            );
            _rightPalm.SetLocalPositionAndRotation(
                rightHand.InverseTransformPoint(rightPalm.transform.position),
                Quaternion.Inverse(rightHand.rotation) * rightPalm.transform.rotation
            );
            Destroy(rightPalm);

            // Legs
            _constraints.LeftLeg.data.root = Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            _constraints.LeftLeg.data.mid = Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            _constraints.LeftLeg.data.tip = Animator.GetBoneTransform(HumanBodyBones.LeftFoot);

            _constraints.RightLeg.data.root = Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            _constraints.RightLeg.data.mid = Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            _constraints.RightLeg.data.tip = Animator.GetBoneTransform(HumanBodyBones.RightFoot);

            // Torso
            Transforms.Hips = Animator.GetBoneTransform(HumanBodyBones.Hips);
            _constraints.Hip.data.constrainedObject = Transforms.Hips;

            _constraints.Chest.data.constrainedObject = Transforms.Hips;
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
        public TwoBoneIKConstraint LeftArm;
        public TwoBoneIKConstraint RightArm;
        public TwoBoneIKConstraint LeftLeg;
        public TwoBoneIKConstraint RightLeg;
        public OverrideTransform Hip;
        public MultiAimConstraint Chest;
        public MultiParentConstraint Head;
    }
}
