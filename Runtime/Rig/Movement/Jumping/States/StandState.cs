using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// State where the player is not engaged with jumping
    /// </summary>
    public class StandState : JumpState
    {
        protected override void Enter() => Jumping.OnAnticipate += AnticipateJump;

        protected override void Exit() => Jumping.OnAnticipate -= AnticipateJump;

        private void AnticipateJump() => StateMachine.ChangeState<CompressState>();
    }
}
