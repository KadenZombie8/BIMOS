using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    [DefaultExecutionOrder(1)]
    public class ScreenModeController : MonoBehaviour
    {
        public bool IsPositionUnlocked { get; private set; }
        public bool IsRotationUnlocked { get; private set; }

        [SerializeField]
        private InputActionReference _moveReference;

        [SerializeField]
        private InputActionReference _scrollReference;

        [SerializeField]
        private InputActionReference _unlockPositionReference;

        [SerializeField]
        private InputActionReference _resetPositionReference;

        [SerializeField]
        private InputActionReference _unlockRotationReference;

        [SerializeField]
        private InputActionReference _resetRotationReference;

        [SerializeField]
        private InputActionReference _cycleReference;

        [SerializeField]
        private bool _isLeftHand;

        private Transform _camera;

        private enum LockState
        {
            Position,
            Rotation
        }
        private LockState _lockState;
        private readonly int _lockStateCount = Enum.GetValues(typeof(LockState)).Length;

        private Vector3 _defaultPosition = new(-0.2f, -0.1f, 0.4f);
        private Vector3 _position;
        private Vector3 _centerPosition = new(-0.2f, 0f, 0.3f);
        private Vector3 _dimensions = new(0.5f, 0.5f, 0.5f);

        private Vector3 _defaultRotation = new(0f, 0f, 0f);
        private Vector3 _rotation;

        private readonly float _moveSensitivity = 0.0005f;
        private readonly float _rotateSensitivity = 0.1f;

        private void Awake()
        {
            if (!_isLeftHand)
            {
                _defaultPosition.x *= -1f;
                _defaultRotation.x *= -1f;
                _centerPosition.x *= -1f;
            }

            _position = _defaultPosition;
            _rotation = _defaultRotation;

            _camera = Camera.main.transform;

            _unlockPositionReference.action.Enable();
            _scrollReference.action.Enable();
            _unlockRotationReference.action.Enable();
            _cycleReference.action.Enable();
        }

        private void OnEnable()
        {
            _unlockPositionReference.action.performed += UnlockPosition;
            _unlockPositionReference.action.canceled += LockPosition;

            _resetPositionReference.action.performed += ResetPosition;

            _unlockRotationReference.action.performed += UnlockRotation;
            _unlockRotationReference.action.canceled += LockRotation;

            _resetRotationReference.action.performed += ResetRotation;

            _cycleReference.action.performed += Cycle;
        }

        private void OnDisable()
        {
            _unlockPositionReference.action.performed -= UnlockPosition;
            _unlockPositionReference.action.canceled -= LockPosition;

            _resetPositionReference.action.performed -= ResetPosition;

            _unlockRotationReference.action.performed -= UnlockRotation;
            _unlockRotationReference.action.canceled -= LockRotation;

            _resetRotationReference.action.performed -= ResetRotation;

            _cycleReference.action.performed -= Cycle;
        }

        private void UnlockPosition(InputAction.CallbackContext _) => IsPositionUnlocked = true;

        private void LockPosition(InputAction.CallbackContext _) => IsPositionUnlocked = false;

        private void ResetPosition(InputAction.CallbackContext _) => _position = _defaultPosition;

        private void UnlockRotation(InputAction.CallbackContext _)
        {
            _lockState = LockState.Position;
            IsRotationUnlocked = true;
        }

        private void LockRotation(InputAction.CallbackContext _) => IsRotationUnlocked = false;

        private void ResetRotation(InputAction.CallbackContext _) => _rotation = _defaultRotation;

        private void Cycle(InputAction.CallbackContext _) => _lockState = (LockState)(((int)_lockState + 1) % _lockStateCount);

        private void Update()
        {
            if (IsPositionUnlocked)
            {
                if (!IsRotationUnlocked)
                {
                    switch (_lockState)
                    {
                        case LockState.Position:
                            PositionHand();
                            break;
                        case LockState.Rotation:
                            RotateHand();
                            break;
                    }
                }
                else PositionHand();
            }

            if (IsRotationUnlocked) RotateHand();

            transform.SetPositionAndRotation(
                _camera.TransformPoint(_position),
                _camera.rotation * Quaternion.Euler(new(_rotation.y, _rotation.z, -_rotation.x))
            );
        }

        private void PositionHand()
        {
            var move = _moveReference.action.ReadValue<Vector2>() * _moveSensitivity;
            var scroll = _scrollReference.action.ReadValue<float>() * _moveSensitivity * 40f;
            _position.x += move.x;
            _position.y += move.y;
            _position.z += scroll;

            _position.x = Mathf.Clamp(_position.x, _centerPosition.x - _dimensions.x / 2f, _centerPosition.x + _dimensions.x / 2f);
            _position.y = Mathf.Clamp(_position.y, _centerPosition.y - _dimensions.y / 2f, _centerPosition.y + _dimensions.y / 2f);
            _position.z = Mathf.Clamp(_position.z, _centerPosition.z - _dimensions.z / 2f, _centerPosition.z + _dimensions.z / 2f);
        }

        private void RotateHand()
        {
            var sign = 1f; // _isLeftHand ? -1f : 1f;
            var rotateInput = _rotateSensitivity * _moveReference.action.ReadValue<Vector2>();
            var scroll = _scrollReference.action.ReadValue<float>() * _rotateSensitivity * 80f;
            var rotate = new Vector3(rotateInput.x * sign, rotateInput.y, scroll * sign);

            _rotation += rotate;

            _rotation.x = Mathf.Clamp(_rotation.x, -180f, 180f);
            _rotation.y = Mathf.Clamp(_rotation.y, -180f, 180f);
            _rotation.z = Mathf.Clamp(_rotation.z, -90f, 90f);
        }
    }
}
