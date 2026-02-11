using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public class ContinuousCrouch : MonoBehaviour
    {
        private Crouching _crouching;
        private VirtualCrouching _virtualCrouching;

        private void Awake()
        {
            _crouching = GetComponent<Crouching>();
            _virtualCrouching = GetComponent<VirtualCrouching>();
        }

        private void FixedUpdate()
        {
            if (_virtualCrouching.IsCrouchChanging)
            {
                var fullHeight = _crouching.StandingLegHeight - _crouching.CrouchingLegHeight;
                _crouching.TargetLegHeight += _virtualCrouching.CrouchInputMagnitude * _virtualCrouching.CrouchSpeed * fullHeight * Time.fixedDeltaTime;
            }
        }
    }
}
