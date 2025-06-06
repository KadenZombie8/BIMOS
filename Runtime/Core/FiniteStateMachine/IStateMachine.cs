namespace KadenZombie8.BIMOS.Core.StateMachine
{
    public interface IStateMachine<T> where T : IStateMachine<T>
    {
        void ChangeState(IState<T> newState);
    }
}
