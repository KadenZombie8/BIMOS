using KadenZombie8.BIMOS.Rig.Animation;
using KadenZombie8.BIMOS.Rig.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [DefaultExecutionOrder(10)]
    [RequireComponent(typeof(Animator))]
    public class MatchPose : MonoBehaviour
    {
        [SerializeField]
        private AnimationRig _animationRig;

        [SerializeField]
        private PhysicsRig _physicsRig;

        private readonly Dictionary<Transform, Transform> _boneMapping = new();

        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();

            AssignPhysicsBone(HumanBodyBones.LeftUpperArm, _physicsRig.Rigidbodies.LeftArm.UpperArm);
            AssignPhysicsBone(HumanBodyBones.LeftLowerArm, _physicsRig.Rigidbodies.LeftArm.LowerArm);
            AssignPhysicsBone(HumanBodyBones.LeftHand, _physicsRig.Rigidbodies.LeftArm.Hand);

            AssignPhysicsBone(HumanBodyBones.RightUpperArm, _physicsRig.Rigidbodies.RightArm.UpperArm);
            AssignPhysicsBone(HumanBodyBones.RightLowerArm, _physicsRig.Rigidbodies.RightArm.LowerArm);
            AssignPhysicsBone(HumanBodyBones.RightHand, _physicsRig.Rigidbodies.RightArm.Hand);

            foreach (HumanBodyBones bone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) continue;

                var avatarBone = _animator.GetBoneTransform(bone);
                var animationBone = _animationRig.Animator.GetBoneTransform(bone);
                
                if (avatarBone && animationBone)
                    _boneMapping.TryAdd(avatarBone, animationBone);
            }
        }

        private void LateUpdate()
        {
            foreach (var bone in _boneMapping)
            {
                bone.Key.SetPositionAndRotation(bone.Value.position, bone.Value.rotation);
            }
        }

        private void AssignPhysicsBone(HumanBodyBones bone, Rigidbody rigidbody)
        {
            var animationBone = _animationRig.Animator.GetBoneTransform(bone);
            var physicsBone = rigidbody.transform;
            _boneMapping.Add(animationBone, physicsBone);
        }
    }
}
