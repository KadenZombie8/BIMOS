using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    public class ScreenModeCamera : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _lookReference;

        [SerializeField]
        private ScreenModeController _leftHand;

        [SerializeField]
        private ScreenModeController _rightHand;

        private readonly float _lookSensitivity = 0.1f;
        private readonly float _maxAngle = 90f;
        private readonly float _minAngle = -90f;
        private Vector2 _cameraRotation;

        private void Awake() => _lookReference.action.Enable();

        private void OnEnable() => Cursor.lockState = CursorLockMode.Locked;

        private void OnDisable() => Cursor.lockState = CursorLockMode.None;

        private void Update()
        {
            if (_leftHand.IsPositionUnlocked || _rightHand.IsPositionUnlocked) return;
            if (_leftHand.IsDepthUnlocked || _rightHand.IsDepthUnlocked) return;
            if (_leftHand.IsRotationUnlocked || _rightHand.IsRotationUnlocked) return;

            var look = _lookSensitivity * _lookReference.action.ReadValue<Vector2>();

            _cameraRotation += look;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, _minAngle, _maxAngle);
            var xRotation = Quaternion.AngleAxis(_cameraRotation.x, Vector3.up);
            var yRotation = Quaternion.AngleAxis(_cameraRotation.y, Vector3.left);

            transform.localRotation = xRotation * yRotation;
        }
    }
}
