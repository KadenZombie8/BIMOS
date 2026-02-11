using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public class DiscreteCrouch : MonoBehaviour
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
            var isCrouching = _virtualCrouching.IsCrouchChanging && _virtualCrouching.CrouchInputMagnitude < 0f;
            var sign = isCrouching ? -1f : 1f;
            var fullHeight = _crouching.StandingLegHeight - _crouching.CrouchingLegHeight;
            var crouchChange = sign * _virtualCrouching.CrouchSpeed * fullHeight * Time.fixedDeltaTime;
            _crouching.TargetLegHeight = Mathf.Clamp(_crouching.TargetLegHeight + crouchChange, _crouching.CrawlingLegHeight, _crouching.StandingLegHeight);
        }
    }
}
