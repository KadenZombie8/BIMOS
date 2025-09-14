using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles turning input and turn types
    /// </summary>
    public class VirtualTurning : MonoBehaviour
    {
        public event Action<Vector2> TurnEvent;
        
        [Tooltip("The speed (in degrees/second) turning occurs at")]
        public float TurnSpeed = 450f;

        [SerializeField]
        private InputActionReference _turnAction;

        private SnapTurn _snapTurn;
        private SmoothTurn _smoothTurn;
        private VirtualTurningMode _turningMode;
        public enum VirtualTurningMode
        {
            NoTurn,
            SnapTurn,
            SmoothTurn
        }
        public VirtualTurningMode TurningMode
        {
            get => _turningMode;
            set
            {
                _turningMode = value;
                switch (value)
                {
                    case VirtualTurningMode.NoTurn:
                        _snapTurn.enabled = false;
                        _smoothTurn.enabled = false;
                        break;
                    case VirtualTurningMode.SnapTurn:
                        _snapTurn.enabled = true;
                        _smoothTurn.enabled = false;
                        break;
                    case VirtualTurningMode.SmoothTurn:
                        _snapTurn.enabled = false;
                        _smoothTurn.enabled = true;
                        break;
                }
            }
        }

        private void Awake()
        {
            _snapTurn = GetComponent<SnapTurn>();
            _smoothTurn = GetComponent<SmoothTurn>();
            _turnAction.action.Enable();
        }

        private void OnEnable()
        {
            _turnAction.action.performed += OnTurn;
            _turnAction.action.canceled += OnTurn;
        }

        private void OnDisable()
        {
            _turnAction.action.performed -= OnTurn;
            _turnAction.action.canceled -= OnTurn;
        }

        private void OnTurn(InputAction.CallbackContext context) => TurnEvent?.Invoke(context.ReadValue<Vector2>());
    }
}