using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// State where the player is not engaged with jumping
    /// </summary>
    public class StandState : JumpState
    {
        private bool _isStanding = false;

        protected override void Enter()
        {
            _isStanding = false;
            Jumping.OnAnticipate += AnticipateJump;
        }

        protected override void Update()
        {
            if (_isStanding) return;

            var fullHeight = Crouching.StandingLegHeight - Crouching.CrouchingLegHeight;
            Crouching.TargetLegHeight += Crouching.VirtualCrouching.CrouchSpeed * fullHeight * Time.deltaTime;

            if (Crouching.TargetLegHeight > Crouching.StandingLegHeight)
                _isStanding = true;
        }

        protected override void Exit()
        {
            Jumping.OnAnticipate -= AnticipateJump;
        }

        private void AnticipateJump() => StateMachine.ChangeState<CompressState>();
    }
}
