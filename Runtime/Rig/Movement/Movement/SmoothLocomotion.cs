using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles character's virtual movement with smooth locomotion
    /// </summary>
    public class SmoothLocomotion : MonoBehaviour
    {
        private ControllerRig _controllerRig;

        [SerializeField]
        [Tooltip("The walk speed of the character")]
        private float _defaultWalkSpeed = 1.5f;

        public enum RunModeType
        {
            Toggle,
            Hold
        }
        public RunModeType RunMode = RunModeType.Toggle;

        [SerializeField]
        private InputActionReference _moveAction;

        [SerializeField]
        private InputActionReference _runAction;

        public LocomotionSphere LocomotionSphere { get; private set; }

        private Vector2 _moveDirection;

        private Crouching _crouching;

        public float WalkSpeed { get; set; }

        public bool IsRunning { get; private set; }
        private bool _isRunPressed;

        /// <summary>
        /// The product of this and the walk speed is the run speed
        /// </summary>
        public float RunSpeedMultiplier = 2f;

        /// <summary>
        /// The product of this and the walk speed is the crouch speed
        /// </summary>
        public float CrouchSpeedMultiplier = 0.5f;

        private void Awake()
        {
            LocomotionSphere = GetComponentInChildren<LocomotionSphere>();
            _crouching = GetComponent<Crouching>();

            _moveAction.action.Enable();
            _runAction.action.Enable();

            ResetWalkSpeed();
        }

        private void Start() => _controllerRig = BIMOSRig.Instance.ControllerRig;

        private void OnEnable()
        {
            _moveAction.action.performed += OnMove;
            _moveAction.action.canceled += OnMove;

            _runAction.action.performed += OnToggleRun;
            _runAction.action.canceled += OnToggleRun;
        }

        private void OnDisable()
        {
            _moveAction.action.performed -= OnMove;
            _moveAction.action.canceled -= OnMove;

            _runAction.action.performed -= OnToggleRun;
            _runAction.action.canceled -= OnToggleRun;
        }

        private void OnMove(InputAction.CallbackContext context) => _moveDirection = context.ReadValue<Vector2>();
        private void OnToggleRun(InputAction.CallbackContext context)
        {
            if (RunMode == RunModeType.Toggle)
            {
                if (context.performed)
                    IsRunning = !IsRunning;
            }
            else
            {
                _isRunPressed = context.performed;
            }
        }

        private void FixedUpdate() => Move();

        private void Move()
        {
            var cameraHeight = _controllerRig.Transforms.Camera.position.y - LocomotionSphere.transform.position.y;
            var crouchingHeight = (_crouching.CrouchingLegHeight + _crouching.StandingLegHeight) / 2f;
            var isCrouching = cameraHeight < crouchingHeight;

            if (RunMode == RunModeType.Hold) IsRunning = _isRunPressed;

            if (RunMode == RunModeType.Toggle && _moveDirection.magnitude < 0.1f || isCrouching)
                IsRunning = false;

            var currentSpeed = WalkSpeed;
            if (IsRunning)
                currentSpeed *= RunSpeedMultiplier;
            else if (isCrouching)
                currentSpeed *= CrouchSpeedMultiplier;

            var targetLinearVelocity = _controllerRig.HeadForwardRotation * new Vector3(_moveDirection.x, 0, _moveDirection.y) * currentSpeed;
            LocomotionSphere.RollFromLinearVelocity(targetLinearVelocity);
        }

        /// <summary>
        /// Resets walk speed to avatar's walk speed
        /// </summary>
        public void ResetWalkSpeed() => WalkSpeed = _defaultWalkSpeed;
    }
}
