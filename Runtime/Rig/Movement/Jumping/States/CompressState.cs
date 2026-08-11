using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Jump pressed state
    /// </summary>
    public class CompressState : JumpState
    {
        private readonly float _bufferTime = 0.25f;
        private readonly float _compressDuration = 0.1f;
        private float _compressTime;
        private bool _jumpBuffer;
        private float _compressedHeight;

        protected override void Enter()
        {
            _jumpBuffer = false;
            _compressTime = 0f;
            _compressedHeight = 0f;

            Jumping.OnJump += BufferJump;

            if (!Jumping.LocomotionSphere.IsGrounded)
                return;

            Jumping.SetFeetMassMultiplier(2f);
        }

        protected override void Update()
        {
            _compressTime += Time.deltaTime;

            var compressSpeed = Jumping.AnticipationHeight / _compressDuration;
            var maxDelta = compressSpeed * Time.deltaTime;
            var compressedHeight = Mathf.MoveTowards(_compressedHeight, Jumping.AnticipationHeight, maxDelta);
            var delta = compressedHeight - _compressedHeight;
            _compressedHeight = compressedHeight;

            Crouching.MaxLegHeight -= delta;
            Crouching.MinLegHeight -= delta;
            Crouching.TargetLegHeight -= delta;

            if (Crouching.MinLegHeight < Crouching.CrawlingLegHeight)
                Crouching.MinLegHeight = Crouching.CrawlingLegHeight;

            if (_jumpBuffer && _compressTime > _bufferTime)
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

            Jumping.SetFeetMassMultiplier(1f);
        }

        private void BufferJump() => _jumpBuffer = true;
    }
}