using System;

namespace KadenZombie8.BIMOS.Core.StateMachine
{
    public interface IState<T> where T : IStateMachine<T>
    {
        event Action OnEnter;
        event Action OnUpdate;
        event Action OnExit;

        void EnterState();
        void UpdateState();
        void ExitState();
    }
}
