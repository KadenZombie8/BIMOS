using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles the jumping mechanic. Controller of a state machine.
    /// </summary>
    [RequireComponent(typeof(Crouching))]
    public class Jumping : MonoBehaviour
    {
        public event Action OnJump;
        public event Action OnAnticipate;

        [SerializeField]
        private InputActionReference _jumpAction;

        public AnimationCurve JumpHeightCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);

        public PhysicsRig PhysicsRig;

        public LocomotionSphere LocomotionSphere { get; private set; }

        [HideInInspector]
        public Crouching Crouching;

        public JumpStateMachine StateMachine;

        /// <summary>
        /// The height the legs contract in preparation for a jump
        /// </summary>
        public float AnticipationHeight { get; private set; } = 0.3f;

        private Rigidbody _feetRigidbody;
        private Rigidbody _pelvisRigidbody;

        private float _defaultFeetMass;
        private float _defaultPelvisMass;

        public void SetFeetMassMultiplier(float multiplier)
        {
            _feetRigidbody.mass = _defaultFeetMass * multiplier;
            var massLoss = _defaultFeetMass * (1f - multiplier);
            _pelvisRigidbody.mass = _defaultPelvisMass + massLoss;
        }

        private void AnticipateJump(CallbackContext callbackContext)
        {
            OnAnticipate?.Invoke();
        }

        private void Jump(CallbackContext callbackContext)
        {
            OnJump?.Invoke();
        }

        private void OnEnable()
        {
            _jumpAction.action.performed += AnticipateJump;
            _jumpAction.action.canceled += Jump;
            _jumpAction.action.Enable();
        }

        private void OnDisable()
        {
            _jumpAction.action.performed -= AnticipateJump;
            _jumpAction.action.canceled -= Jump;
            _jumpAction.action.Disable();
        }

        private void Start()
        {
            LocomotionSphere = PhysicsRig.Movement.LocomotionSphere;

            _feetRigidbody = PhysicsRig.Rigidbodies.LocomotionSphere;
            _pelvisRigidbody = PhysicsRig.Rigidbodies.Pelvis;

            _defaultFeetMass = _feetRigidbody.mass;
            _defaultPelvisMass = _pelvisRigidbody.mass;

            Crouching = GetComponent<Crouching>();
        }

        private void Update() => StateMachine.UpdateState();
    }
}