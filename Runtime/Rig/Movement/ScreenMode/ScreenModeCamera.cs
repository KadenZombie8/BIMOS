using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public class ScreenModeCamera : MonoBehaviour
    {
        public float MouseSensitivity = 5f;
        public float GamepadSensitivity = 5f;

        [SerializeField]
        private InputActionReference _lookReference;

        [SerializeField]
        private ScreenModeController _leftHand;

        [SerializeField]
        private ScreenModeController _rightHand;

        private readonly float _maxAngle = 90f;
        private readonly float _minAngle = -90f;
        private Vector2 _cameraRotation;

        public bool IsActive { get; set; }

        private void Awake() => _lookReference.action.Enable();

        private void Update()
        {
            if (!IsActive) return;
            if (_leftHand.IsPositionUnlocked || _rightHand.IsPositionUnlocked) return;
            if (_leftHand.IsRotationUnlocked || _rightHand.IsRotationUnlocked) return;
            if (_leftHand.IsUnlocked || _rightHand.IsUnlocked) return;

            var isGamepad = _lookReference.action.activeControl?.device is Gamepad;
            var lookSensitivity = isGamepad ? GamepadSensitivity : MouseSensitivity;
            var look = lookSensitivity * 0.02f * _lookReference.action.ReadValue<Vector2>();

            _cameraRotation += look;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, _minAngle, _maxAngle);
            var xRotation = Quaternion.AngleAxis(_cameraRotation.x, Vector3.up);
            var yRotation = Quaternion.AngleAxis(_cameraRotation.y, Vector3.left);

            transform.localRotation = xRotation * yRotation;
        }
    }
}
