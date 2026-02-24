using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Rising state
    /// </summary>
    public class RiseState : JumpState
    {
        private float _airTime;
        private readonly float _minAirTime = 0.05f;
        private float _maxCrouch;
        private float _legRiseTime;
        private readonly float _legRiseSpeed = 8f;
        private bool _hasOverridenLift;

        protected override void Enter()
        {
            _airTime = 0f;
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 2f;
            _maxCrouch = (Crouching.StandingLegHeight - Crouching.CrawlingLegHeight) / 3f;
            _legRiseTime = 3f / _legRiseSpeed;
            _hasOverridenLift = false;
        }

        protected override void Update()
        {
            _airTime += Time.deltaTime;

            if (Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.linearVelocity.y < -0.1f && _airTime > _minAirTime
                || _airTime > _legRiseTime)
            {
                StateMachine.ChangeState<FallState>();
                return;
                //float height = Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.position.y - StateMachine.Jumping.PhysicsRig.Colliders.LocomotionSphere.radius;
                //Debug.LogError("Height reached: " + height);
            }

            if (_airTime > _legRiseTime / 2f && _hasOverridenLift)
            {
                StateMachine.ChangeState<StandState>();
                return;
            }

            var crouchRate = Crouching.StandingLegHeight * _legRiseSpeed;
            var newLegHeight = Crouching.TargetLegHeight - crouchRate * Time.deltaTime;

            var crouchInput = Crouching.VirtualCrouching.CrouchInputMagnitude;
            if (crouchInput < 0f && Crouching.VirtualCrouching.IsCrouchChanging)
            {
                newLegHeight -= crouchInput * Crouching.VirtualCrouching.CrouchSpeed * Time.deltaTime;
                _hasOverridenLift = true;
            }

            Crouching.TargetLegHeight = Mathf.Clamp(newLegHeight, _maxCrouch, Crouching.StandingLegHeight);
        }

        protected override void Exit()
        {
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 1f;
        }
    }
}