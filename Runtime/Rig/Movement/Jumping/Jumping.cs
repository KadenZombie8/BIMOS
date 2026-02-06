using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Handles the jumping mechanic. Controller of a state machine.
    /// </summary>
    public class Jumping : MonoBehaviour
    {
        public event Action OnJump;
        public event Action OnAnticipate;

        [SerializeField]
        private InputActionReference _jumpAction;

        public AnimationCurve JumpHeightCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1f);

        public PhysicsRig PhysicsRig;

        [HideInInspector]
        public LocomotionSphere LocomotionSphere;

        [HideInInspector]
        public Crouching Crouching;

        public JumpStateMachine StateMachine;

        /// <summary>
        /// The height the legs contract in preparation for a jump
        /// </summary>
        public float AnticipationHeight { get; private set; } = 0.4f;

        private void AnticipateJump(CallbackContext callbackContext) => OnAnticipate?.Invoke();


        private void Jump(CallbackContext callbackContext) => OnJump?.Invoke();

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
            Crouching = GetComponent<Crouching>();
        }

        private void Update() => StateMachine.UpdateState();
    }
}