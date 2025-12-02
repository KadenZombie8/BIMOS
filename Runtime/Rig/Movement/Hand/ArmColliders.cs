using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class ArmColliders : MonoBehaviour
    {
        [SerializeField]
        private Handedness _handedness;

        [SerializeField]
        private CapsuleCollider _upperArmCollider;

        [SerializeField]
        private CapsuleCollider _lowerArmCollider;

        private Transform _animationUpperArm;
        private Transform _animationLowerArm;

        private void Start()
        {
            var rig = BIMOSRig.Instance;

            var animator = rig.AnimationRig.Animator;

            _animationUpperArm = _handedness == Handedness.Left ? animator.GetBoneTransform(HumanBodyBones.LeftUpperArm) : animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            _animationLowerArm = _handedness == Handedness.Left ? animator.GetBoneTransform(HumanBodyBones.LeftLowerArm) : animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            var animationHand = _handedness == Handedness.Left ? animator.GetBoneTransform(HumanBodyBones.LeftHand) : animator.GetBoneTransform(HumanBodyBones.RightHand);

            _upperArmCollider.height = Vector3.Distance(_animationUpperArm.position, _animationLowerArm.position)
                + _upperArmCollider.radius * 2f;
            _upperArmCollider.center = (_upperArmCollider.height / 2f - _upperArmCollider.radius) * Vector3.up;

            _lowerArmCollider.height = Vector3.Distance(_animationLowerArm.position, animationHand.position)
                + _lowerArmCollider.radius * 2f;
            _lowerArmCollider.center = (_lowerArmCollider.height / 2f - _lowerArmCollider.radius) * Vector3.up;
        }

        private void FixedUpdate()
        {
            _upperArmCollider.transform.SetPositionAndRotation(_animationUpperArm.position, _animationUpperArm.rotation);
            _lowerArmCollider.transform.SetPositionAndRotation(_animationLowerArm.position, _animationLowerArm.rotation);
        }
    }
}
