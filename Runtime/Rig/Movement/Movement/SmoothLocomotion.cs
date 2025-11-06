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

        [SerializeField]
        private InputActionReference _moveAction;

        [SerializeField]
        private InputActionReference _runAction;

        public LocomotionSphere LocomotionSphere { get; private set; }

        private Vector2 _moveDirection;

        public float WalkSpeed { get; set; }

        public bool IsRunning { get; private set; }

        /// <summary>
        /// The product of this and the walk speed is the run speed
        /// </summary>
        public float RunSpeedMultiplier { get; set; } = 2f;

        private void Awake()
        {
            LocomotionSphere = GetComponentInChildren<LocomotionSphere>();

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
        }

        private void OnDisable()
        {
            _moveAction.action.performed -= OnMove;
            _moveAction.action.canceled -= OnMove;

            _runAction.action.performed -= OnToggleRun;
        }

        private void OnMove(InputAction.CallbackContext context) => _moveDirection = context.ReadValue<Vector2>();
        private void OnToggleRun(InputAction.CallbackContext context) => IsRunning = !IsRunning;

        private void FixedUpdate() => Move();

        private void Move()
        {
            if (_moveDirection.magnitude < 0.1f)
                IsRunning = false;

            var currentSpeed = WalkSpeed;
            if (IsRunning)
                currentSpeed *= RunSpeedMultiplier;

            var targetLinearVelocity = _controllerRig.HeadForwardRotation * new Vector3(_moveDirection.x, 0, _moveDirection.y) * currentSpeed;
            LocomotionSphere.RollFromLinearVelocity(targetLinearVelocity);
        }

        /// <summary>
        /// Resets walk speed to avatar's walk speed
        /// </summary>
        public void ResetWalkSpeed() => WalkSpeed = _defaultWalkSpeed;
    }
}
