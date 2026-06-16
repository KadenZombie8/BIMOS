using KadenZombie8.BIMOS.Core.StateMachine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public abstract class JumpState : BaseState<JumpStateMachine>
    {
        protected Jumping Jumping { get; private set; }
        protected Crouching Crouching { get; private set; }

        public override void OnStart()
        {
            base.OnStart();
            Jumping = StateMachine.Jumping;
            Crouching = Jumping.Crouching;
        }
    }
}
