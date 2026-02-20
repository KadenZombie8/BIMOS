using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public class RecoverState : StandState
    {
        protected override void Update()
        {
            var fullHeight = Crouching.StandingLegHeight - Crouching.CrouchingLegHeight;
            Crouching.TargetLegHeight += Crouching.VirtualCrouching.CrouchSpeed * fullHeight * Time.deltaTime;

            if (Crouching.TargetLegHeight > Crouching.StandingLegHeight)
                StateMachine.ChangeState<StandState>();

            Debug.Log(Crouching.VirtualCrouching.CrouchInputMagnitude);
            if (Crouching.VirtualCrouching.IsCrouchChanging)
                StateMachine.ChangeState<StandState>();
        }
    }
}
