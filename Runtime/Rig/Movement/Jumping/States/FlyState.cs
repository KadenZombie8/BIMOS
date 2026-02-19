using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Airborne state
    /// </summary>
    public class FlyState : JumpState
    {
        private bool _isFalling;
        private float _airTime;
        private readonly float _minAirTime = 0.05f;
        private float _maxCrouch;
        private bool _hasOverridenLift;

        protected override void Enter()
        {
            _airTime = 0f;
            _isFalling = false;
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 2f;
            _maxCrouch = (Crouching.StandingLegHeight - Crouching.CrawlingLegHeight) / 3f;
            _hasOverridenLift = false;
        }

        protected override void Update()
        {
            _airTime += Time.deltaTime;

            if (Crouching.VirtualCrouching.IsCrouchChanging)
            {
                _hasOverridenLift = true;
                Jumping.PhysicsRig.Joints.Pelvis.massScale = 1f;
            }

            if (Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.linearVelocity.y < -0.1f && !_isFalling && _airTime > _minAirTime)
            {
                _isFalling = true;
                //float height = Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.position.y - StateMachine.Jumping.PhysicsRig.Colliders.LocomotionSphere.radius;
                //Debug.LogError("Height reached: " + height);
            }

            if (_airTime > _minAirTime && Jumping.LocomotionSphere.IsGrounded)
            {
                StateMachine.ChangeState<RecoverState>();
                return;
            }

            if (!_hasOverridenLift)
            {
                var sign = _isFalling ? -1f : 1f;
                var crouchRate = Crouching.StandingLegHeight * sign * 8f;
                if (_isFalling)
                    crouchRate *= 0.5f;
                var newLegHeight = Crouching.TargetLegHeight - crouchRate * Time.deltaTime;

                Crouching.TargetLegHeight = Mathf.Clamp(newLegHeight, _maxCrouch, Crouching.StandingLegHeight);
            }
        }

        protected override void Exit()
        {
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 1f;
        }
    }
}