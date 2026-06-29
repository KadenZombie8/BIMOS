using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles turning input and turn types
    /// </summary>
    [RequireComponent(typeof(SmoothTurn), typeof(SnapTurn))]
    public class VirtualTurning : MonoBehaviour
    {
        public event Action<float> TurnEvent;

        [HideInInspector]
        public float TurnRate = 4f;

        [SerializeField]
        private InputActionReference _turnAction;

        public ControllerRig ControllerRig;

        private SnapTurn _snapTurn;
        private SmoothTurn _smoothTurn;
        private VirtualTurningMode _turningMode;
        public enum VirtualTurningMode
        {
            NoTurn,
            SmoothTurn,
            SnapTurn
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
                    case VirtualTurningMode.SmoothTurn:
                        _snapTurn.enabled = false;
                        _smoothTurn.enabled = true;
                        break;
                    case VirtualTurningMode.SnapTurn:
                        _snapTurn.enabled = true;
                        _smoothTurn.enabled = false;
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

        private void OnTurn(InputAction.CallbackContext context) => TurnEvent?.Invoke(context.ReadValue<float>());
    }
}