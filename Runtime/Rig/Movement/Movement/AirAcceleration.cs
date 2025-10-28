using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles character's acceleration while in the air
    /// </summary>
    public class AirAcceleration : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The acceleration in air")]
        private float _airAcceleration = 3f;

        [SerializeField]
        private InputActionReference _moveAction;

        public LocomotionSphere LocomotionSphere { get; private set; }

        private Transform _mainCameraTransform;
        private Vector2 _moveDirection;
        private PhysicsRigRigidbodies _physicsRigRigidbodies;

        private void Awake()
        {
            _mainCameraTransform = Camera.main.transform;
            LocomotionSphere = GetComponentInChildren<LocomotionSphere>();

            _moveAction.action.Enable();
        }

        private void Start() => _physicsRigRigidbodies = BIMOSRig.Instance.PhysicsRig.Rigidbodies;

        private void OnEnable()
        {
            _moveAction.action.performed += OnMove;
            _moveAction.action.canceled += OnMove;
        }

        private void OnDisable()
        {
            _moveAction.action.performed -= OnMove;
            _moveAction.action.canceled -= OnMove;
        }

        private void OnMove(InputAction.CallbackContext context) => _moveDirection = context.ReadValue<Vector2>();

        private void FixedUpdate() => Move();

        private void Move()
        {
            if (LocomotionSphere.IsGrounded)
                return;

            Vector3 horizontalVelocity = new(
                _physicsRigRigidbodies.LocomotionSphere.linearVelocity.x,
                0f,
                _physicsRigRigidbodies.LocomotionSphere.linearVelocity.z
            );

            if (horizontalVelocity.sqrMagnitude > 1f)
                return;

            var headYaw = Quaternion.LookRotation(Vector3.Cross(_mainCameraTransform.right, Vector3.up));
            var targetLinearVelocity = headYaw * new Vector3(_moveDirection.x, 0, _moveDirection.y) * _airAcceleration;
            
            _physicsRigRigidbodies.LocomotionSphere.AddForce(targetLinearVelocity, ForceMode.Acceleration);
            _physicsRigRigidbodies.Knee.AddForce(targetLinearVelocity, ForceMode.Acceleration);
            _physicsRigRigidbodies.Pelvis.AddForce(targetLinearVelocity, ForceMode.Acceleration);
            _physicsRigRigidbodies.Head.AddForce(targetLinearVelocity, ForceMode.Acceleration);
        }
    }
}
