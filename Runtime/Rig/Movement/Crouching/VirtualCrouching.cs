using System;
using KadenZombie8.BIMOS.Core.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public float CrouchInputMagnitude { get; set; }

        public bool IsCrouchChanging => Mathf.Abs(CrouchInputMagnitude) >= 0.75f;

        public enum VirtualCrouchModeType
        {
            Continuous,
            Discrete,
            DiscreteToggle
        }
        public VirtualCrouchModeType VirtualCrouchMode = VirtualCrouchModeType.Continuous;

        private Crouching _crouching;
        private Jumping _jumping;
        private bool _wasCrouchChanging;
        private IState<JumpStateMachine> _standState;
        private IState<JumpStateMachine> _compressState;
        private IState<JumpStateMachine> _pushState;
        private ControllerRig _controllerRig;
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
            switch (VirtualCrouchMode)
            {
                case VirtualCrouchModeType.Continuous:
                    CrouchInputMagnitude = context.ReadValue<Vector2>().y;
                    break;
                case VirtualCrouchModeType.Discrete:
                    CrouchInputMagnitude = context.ReadValue<Vector2>().y;
                    CrouchInputMagnitude = CrouchInputMagnitude < 0f ? -1f : 1f;
                    break;
                case VirtualCrouchModeType.DiscreteToggle when context.performed:
                    _isCrouching = !_isCrouching;
                    CrouchInputMagnitude = _isCrouching ? -1f : 1f;
                    break;
            }
        }

        private void Start()
        {
            _standState = _jumping.StateMachine.GetState<StandState>();
            _compressState = _jumping.StateMachine.GetState<CompressState>();
            _pushState = _jumping.StateMachine.GetState<PushState>();
            _controllerRig = BIMOSRig.Instance.ControllerRig;
        }

        private void FixedUpdate()
        {
            var fullHeight = _crouching.StandingLegHeight - _crouching.CrouchingLegHeight;

            var isStanding = _jumping.StateMachine.CurrentState == _standState;
            if (VirtualCrouchMode == VirtualCrouchModeType.Continuous || isStanding)
                _crouching.TargetLegHeight += CrouchInputMagnitude * CrouchSpeed * fullHeight * Time.fixedDeltaTime;

            if (VirtualCrouchMode == VirtualCrouchModeType.DiscreteToggle)
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

            if (VirtualCrouchMode == VirtualCrouchModeType.Continuous)
            {
                minLegHeight = _crouching.CrawlingLegHeight;
                maxLegHeight = _crouching.StandingLegHeight + _crouching.TiptoesLegHeightGain;
            }
            else
            {
                var neckYDifference = _controllerRig.Transforms.Camera.position.y - _controllerRig.Transforms.HeadCameraOffset.position.y;
                minLegHeight = _crouching.CrawlingLegHeight - neckYDifference;
                maxLegHeight = _crouching.StandingLegHeight - neckYDifference - _controllerRig.Transforms.HeadCameraOffset.localPosition.y;
            }

            if (isCompressed)
            {
                minLegHeight -= _jumping.AnticipationHeight;
                maxLegHeight -= _jumping.AnticipationHeight;
            }
            else if (!IsCrouchChanging && _wasCrouchChanging)
            {
                minLegHeight = _crouching.CrouchingLegHeight;
                maxLegHeight = _crouching.StandingLegHeight;
            }

            _crouching.TargetLegHeight = Mathf.Clamp(_crouching.TargetLegHeight, minLegHeight, maxLegHeight);
            _wasCrouchChanging = IsCrouchChanging;
        }
    }
}