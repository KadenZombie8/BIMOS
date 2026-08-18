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

        private readonly Dictionary<Transform, Transform> _animationMapping = new();

        private readonly Dictionary<HumanBodyBones, Transform> _physicsMapping = new();

        private readonly List<Transform> _fingerMapping = new();

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

                Transform targetBone;

                if (_physicsMapping.ContainsKey(bone))
                {
                    targetBone = _physicsMapping[bone];
                }
                else
                {
                    var boneIndex = (int)bone;
                    if (boneIndex >= 24 && boneIndex <= 53)
                    {
                        _fingerMapping.Add(avatarBone);
                        targetBone = _animationRig.Animator.GetBoneTransform(bone);
                    }
                    else
                    {
                        targetBone = _animationRig.Animator.GetBoneTransform(bone);
                    }
                }

                if (avatarBone && targetBone)
                    _animationMapping.TryAdd(avatarBone, targetBone);
            }

            foreach (var bone in _animationMapping)
            {
                print(bone.Key.name);
            }
        }

        private void LateUpdate()
        {
            foreach (var bone in _animationMapping)
            {
                var avatarBone = bone.Key;
                if (_fingerMapping.Contains(avatarBone))
                {
                    bone.Key.SetLocalPositionAndRotation(bone.Value.localPosition, bone.Value.localRotation);
                }
                else
                {
                    bone.Key.SetPositionAndRotation(bone.Value.position, bone.Value.rotation);
                }
            }
        }

        private void AssignPhysicsBone(HumanBodyBones bone, Rigidbody rigidbody)
        {
            var physicsBone = rigidbody.transform;
            _physicsMapping.Add(bone, physicsBone);
        }
    }
}
