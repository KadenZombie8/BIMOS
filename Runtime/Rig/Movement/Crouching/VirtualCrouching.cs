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
        [SerializeField]
        private InputActionReference _crouchAction;

        [Tooltip("The speed (in %/s) the legs can extend/retract at")]
        public float CrouchSpeed = 2.5f;

        public bool IsCrouchChanging { get; private set; }

        private Crouching _crouching;
        private Jumping _jumping;
        private float _crouchInputMagnitude;
        private bool _wasCrouchChanging;
        private IState<JumpStateMachine> _compressState;

        private void Crouch(InputAction.CallbackContext context)
        {
            _crouchInputMagnitude = context.ReadValue<Vector2>().y;
        }

        private void Awake()
        {
            _crouchAction.action.Enable();
            _crouching = GetComponent<Crouching>();
            _jumping = GetComponent<Jumping>();
        }

        private void OnEnable()
        {
            _crouchAction.action.performed += Crouch;
            _crouchAction.action.canceled += Crouch;
        }

        private void OnDisable()
        {
            _crouchAction.action.performed -= Crouch;
            _crouchAction.action.canceled -= Crouch;
        }

        private void Start()
        {
            _compressState = _jumping.StateMachine.GetState<CompressState>();
        }

        private void FixedUpdate()
        {
            IsCrouchChanging = Mathf.Abs(_crouchInputMagnitude) >= 0.75f;
            var isCompressed = _jumping.StateMachine.CurrentState == _compressState;

            if (IsCrouchChanging)
            {
                var fullHeight = _crouching.StandingLegHeight - _crouching.CrouchingLegHeight;
                _crouching.TargetLegHeight += _crouchInputMagnitude * CrouchSpeed * fullHeight * Time.fixedDeltaTime;
            }

            if (isCompressed)
            {
                var minLegHeight = _crouching.CrouchingLegHeight - _jumping.AnticipationHeight;
                var maxLegHeight = _crouching.StandingLegHeight - _jumping.AnticipationHeight;

                _crouching.TargetLegHeight = Mathf.Clamp(_crouching.TargetLegHeight, minLegHeight, maxLegHeight);
            }
            else
            {
                if (!IsCrouchChanging && _wasCrouchChanging)
                {
                    _crouching.TargetLegHeight = Mathf.Clamp(_crouching.TargetLegHeight, _crouching.CrouchingLegHeight, _crouching.StandingLegHeight);
                }
            }

            _wasCrouchChanging = IsCrouchChanging;
        }
    }
}