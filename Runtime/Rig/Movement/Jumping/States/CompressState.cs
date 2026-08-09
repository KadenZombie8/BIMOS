using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Jump pressed state
    /// </summary>
    public class CompressState : JumpState
    {
        private readonly float _minCompressTime = 0.25f;
        private float _compressTime;
        private bool _jumpBuffer;

        protected override void Enter()
        {
            _jumpBuffer = false;
            _compressTime = 0f;

            Jumping.OnJump += BufferJump;

            Crouching.MinLegHeight = Crouching.CrouchingLegHeight - Jumping.AnticipationHeight;
            Crouching.MaxLegHeight = Crouching.StandingLegHeight - Jumping.AnticipationHeight;
            Crouching.TargetLegHeight -= Jumping.AnticipationHeight;

            if (!Jumping.LocomotionSphere.IsGrounded)
                return;

            Crouching.GroundingForce = 2000f;
        }

        protected override void Update()
        {
            _compressTime += Time.deltaTime;

            if (_jumpBuffer && _compressTime > _minCompressTime)
            {
                if (Jumping.LocomotionSphere.IsGrounded)
                    StateMachine.ChangeState<PushState>();
                else
                    StateMachine.ChangeState<RecoverState>();
            }
        }

        protected override void Exit()
        {
            Jumping.OnJump -= BufferJump;

            Crouching.MinLegHeight = Crouching.MinCrouchingLegHeight;
            Crouching.MaxLegHeight = Crouching.MaxStandingLegHeight;

            Crouching.GroundingForce = 0f;
        }

        private void BufferJump() => _jumpBuffer = true;
    }
}