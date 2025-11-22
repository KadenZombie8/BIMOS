using KadenZombie8.BIMOS.Rig;
using System;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    public class ArmController : MonoBehaviour
    {
        public ArmPhysicsControllers Controllers;
        public AnimationRig AnimationRig;
        private Animator _animator;
        private void Awake() {
            _animator = AnimationRig.GetComponentInChildren<Animator>();
            SetPhysicsArmTarget(Controllers.LeftUpperArm, HumanBodyBones.LeftUpperArm);
            SetPhysicsArmTarget(Controllers.LeftLowerArm, HumanBodyBones.LeftLowerArm);
            SetPhysicsArmTarget(Controllers.RightUpperArm, HumanBodyBones.RightUpperArm);
            SetPhysicsArmTarget(Controllers.RightLowerArm, HumanBodyBones.RightLowerArm);
        }
        public void SetPhysicsArmTarget(PhysicsHand physicsHand, HumanBodyBones bodyBone) {
            physicsHand.transform.position = _animator.GetBoneTransform(bodyBone).position;
            physicsHand.Target = _animator.GetBoneTransform(bodyBone);
            physicsHand.Controller = _animator.GetBoneTransform(bodyBone);
        }
    }
    [Serializable]
    public struct ArmPhysicsControllers {
        public PhysicsHand LeftUpperArm;
        public PhysicsHand LeftLowerArm;
        public PhysicsHand RightUpperArm;
        public PhysicsHand RightLowerArm;
    }
}
