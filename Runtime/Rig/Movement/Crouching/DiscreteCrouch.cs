using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public class DiscreteCrouch : MonoBehaviour
    {
        private Crouching _crouching;
        private VirtualCrouching _virtualCrouching;
        private ControllerRig _controllerRig;

        private void Awake()
        {
            _crouching = GetComponent<Crouching>();
            _virtualCrouching = GetComponent<VirtualCrouching>();
        }

        private void Start()
        {
            _controllerRig = BIMOSRig.Instance.ControllerRig;
        }

        private void FixedUpdate()
        {
            var neckYDifference = _controllerRig.Transforms.Camera.position.y - _controllerRig.Transforms.HeadCameraOffset.position.y;
            var minLegHeight = _crouching.CrawlingLegHeight - neckYDifference;
            var maxLegHeight = _crouching.StandingLegHeight - neckYDifference;

            var isCrouching = _virtualCrouching.IsCrouchChanging && _virtualCrouching.CrouchInputMagnitude < 0f;
            var sign = isCrouching ? -1f : 1f;
            var fullHeight = _crouching.StandingLegHeight - _crouching.CrouchingLegHeight;
            var crouchChange = sign * _virtualCrouching.CrouchSpeed * fullHeight * Time.fixedDeltaTime;

            _crouching.TargetLegHeight = Mathf.Clamp(_crouching.TargetLegHeight + crouchChange, minLegHeight, maxLegHeight);
        }
    }
}
