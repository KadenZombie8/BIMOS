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

            Jumping.PhysicsRig.Joints.Pelvis.massScale = 0.01f;
        }

        protected override void Update()
        {
            _compressTime += Time.deltaTime;

            if (_jumpBuffer && _compressTime > _minCompressTime)
                StateMachine.ChangeState<PushState>(); 
        }

        protected override void Exit()
        {
            Jumping.OnJump -= BufferJump;

            Crouching.MinLegHeight = Crouching.MinCrouchingLegHeight;
            Crouching.MaxLegHeight = Crouching.MaxStandingLegHeight;

            Jumping.PhysicsRig.Joints.Pelvis.massScale = 1f;
        }

        private void BufferJump() => _jumpBuffer = true;
    }
}