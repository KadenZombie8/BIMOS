using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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

        private void Crouch(InputAction.CallbackContext context) => _virtualCrouching.CrouchInputMagnitude = context.ReadValue<Vector2>().y;
    }
}
