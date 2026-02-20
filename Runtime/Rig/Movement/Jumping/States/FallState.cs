using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Falling state
    /// </summary>
    public class FallState : JumpState
    {
        private float _maxCrouch;
        private readonly float _legFallSpeed = 4f;

        protected override void Enter()
        {
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 2f;
            _maxCrouch = (Crouching.StandingLegHeight - Crouching.CrawlingLegHeight) / 3f;
        }

        protected override void Update()
        {
            if (Crouching.VirtualCrouching.IsCrouchChanging)
                StateMachine.ChangeState<StandState>();

            if (Jumping.LocomotionSphere.IsGrounded)
                StateMachine.ChangeState<RecoverState>();

            var crouchRate = Crouching.StandingLegHeight * _legFallSpeed;
            var newLegHeight = Crouching.TargetLegHeight + crouchRate * Time.deltaTime;

            Crouching.TargetLegHeight = Mathf.Clamp(newLegHeight, _maxCrouch, Crouching.StandingLegHeight);
        }

        protected override void Exit()
        {
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 1f;
        }
    }
}
