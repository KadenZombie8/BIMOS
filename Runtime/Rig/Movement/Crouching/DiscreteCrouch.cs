using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public class DiscreteCrouch : MonoBehaviour
    {
        private Crouching _crouching;
        private VirtualCrouching _virtualCrouching;
        private ControllerRig _controllerRig;
        private bool _isCrouching;

        private void Awake()
        {
            _crouching = GetComponent<Crouching>();
            _virtualCrouching = GetComponent<VirtualCrouching>();
        }

        private void OnEnable()
        {
            _virtualCrouching.CrouchAction.action.performed += Crouch;
            _virtualCrouching.CrouchAction.action.canceled += Crouch;
        }

        private void OnDisable()
        {
            _virtualCrouching.CrouchAction.action.performed -= Crouch;
            _virtualCrouching.CrouchAction.action.canceled -= Crouch;
        }

        private void Crouch(InputAction.CallbackContext context)
        {
            _virtualCrouching.CrouchInputMagnitude = context.ReadValue<Vector2>().y;
            _isCrouching = _virtualCrouching.IsCrouchChanging && _virtualCrouching.CrouchInputMagnitude < 0f;
        }

        private void Start() => _controllerRig = BIMOSRig.Instance.ControllerRig;

        private void FixedUpdate()
        {
            var neckYDifference = _controllerRig.Transforms.Camera.position.y - _controllerRig.Transforms.HeadCameraOffset.position.y;
            var minLegHeight = _crouching.CrawlingLegHeight - neckYDifference;
            var maxLegHeight = _crouching.StandingLegHeight - neckYDifference;

            var sign = _isCrouching ? -1f : 1f;
            _virtualCrouching.CrouchInputMagnitude = sign;
        }
    }
}
