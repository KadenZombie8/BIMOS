using System;
using KadenZombie8.BIMOS.Core.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles virtual crouching.
    /// </summary>
    public class VirtualCrouching : MonoBehaviour
    {
        public InputActionReference CrouchAction;

        [Tooltip("The speed (in %/s) the legs can extend/retract at")]
        public float CrouchSpeed = 2.5f;

        [SerializeField]
        private ControllerRig _controllerRig;

        public float CrouchInputMagnitude { get; set; }

        public bool IsCrouchChanging => CrouchInputMagnitude != 0f;

        public enum VirtualCrouchModeType
        {
            Continuous,
            Discrete,
            DiscreteToggle
        }
        private VirtualCrouchModeType _virtualCrouchMode;

        private Crouching _crouching;
        private Jumping _jumping;
        private IState<JumpStateMachine> _standState;
        private IState<JumpStateMachine> _compressState;
        private IState<JumpStateMachine> _pushState;
        private bool _isCrouching;

        private void Awake()
        {
            CrouchAction.action.Enable();
            _crouching = GetComponent<Crouching>();
            _jumping = GetComponent<Jumping>();
        }

        private void OnEnable()
        {
            CrouchAction.action.performed += Crouch;
            CrouchAction.action.canceled += Crouch;
        }

        private void OnDisable()
        {
            CrouchAction.action.performed -= Crouch;
            CrouchAction.action.canceled -= Crouch;
        }

        private void Crouch(InputAction.CallbackContext context)
        {
            var device = context.action.activeControl.device;

            if (device is XRController)
                _virtualCrouchMode = VirtualCrouchModeType.Continuous;
            else if (device is Keyboard)
                _virtualCrouchMode = VirtualCrouchModeType.Discrete;
            else if (device is Gamepad)
                _virtualCrouchMode = VirtualCrouchModeType.DiscreteToggle;

            CrouchInputMagnitude = context.ReadValue<float>();

            switch (_virtualCrouchMode)
            {
                case VirtualCrouchModeType.Discrete:
                    CrouchInputMagnitude = CrouchInputMagnitude < 0f ? -1f : 1f;
                    break;
                case VirtualCrouchModeType.DiscreteToggle:
                    if (context.performed) _isCrouching = !_isCrouching;
                    CrouchInputMagnitude = _isCrouching ? -1f : 1f;
                    break;
            }
        }

        private void Start()
        {
            _standState = _jumping.StateMachine.GetState<StandState>();
            _compressState = _jumping.StateMachine.GetState<CompressState>();
            _pushState = _jumping.StateMachine.GetState<PushState>();
        }

        private void FixedUpdate()
        {
            var fullHeight = _crouching.StandingLegHeight - _crouching.CrouchingLegHeight;

            var isStanding = _jumping.StateMachine.CurrentState == _standState;
            if (_virtualCrouchMode == VirtualCrouchModeType.Continuous || isStanding)
                _crouching.TargetLegHeight += CrouchInputMagnitude * CrouchSpeed * fullHeight * Time.fixedDeltaTime;

            if (_virtualCrouchMode == VirtualCrouchModeType.DiscreteToggle)
            {
                var isPushing = _jumping.StateMachine.CurrentState == _pushState;
                if (isPushing)
                {
                    _isCrouching = false;
                    CrouchInputMagnitude = 1f;
                }
            }

            var isCompressed = _jumping.StateMachine.CurrentState == _compressState;

            float minLegHeight;
            float maxLegHeight;

            if (_virtualCrouchMode == VirtualCrouchModeType.Continuous)
            {
                minLegHeight = _crouching.CrawlingLegHeight;
                maxLegHeight = _crouching.StandingLegHeight + _crouching.TiptoesLegHeightGain;
            }
            else
            {
                minLegHeight = _jumping.LocomotionSphere.IsGrounded ? _crouching.CrouchingLegHeight : _crouching.CrawlingLegHeight;
                maxLegHeight = _crouching.StandingLegHeight;
            }

            if (isCompressed)
            {
                minLegHeight = _crouching.MinLegHeight;
                maxLegHeight = _crouching.MaxLegHeight;
            }
            else
            {
                if (CrouchInputMagnitude >= 0f && _jumping.LocomotionSphere.IsGrounded)
                    minLegHeight = _crouching.CrouchingLegHeight;

                if (CrouchInputMagnitude <= 0f)
                    maxLegHeight = _crouching.StandingLegHeight;
            }

            if (_virtualCrouchMode != VirtualCrouchModeType.Continuous)
            {
                var neckYDifference = _controllerRig.Transforms.Camera.position.y - _controllerRig.Transforms.HeadCameraOffset.position.y;

                maxLegHeight -= neckYDifference;
                minLegHeight -= neckYDifference;
            }

            _crouching.TargetLegHeight = Mathf.Clamp(_crouching.TargetLegHeight, minLegHeight, maxLegHeight);
        }
    }
}