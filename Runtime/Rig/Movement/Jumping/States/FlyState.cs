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

        protected override void Enter()
        {
            _airTime = 0f;
            _isFalling = false;
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 2f;
        }

        protected override void Update()
        {
            if (_airTime > _minAirTime && Jumping.LocomotionSphere.IsGrounded)
                StateMachine.ChangeState<StandState>();

            _airTime += Time.deltaTime;

            var sign = _isFalling ? -1f : 1f;
            var newLegHeight = Crouching.TargetLegHeight - Crouching.StandingLegHeight * Time.deltaTime * sign * 4f;
            if (newLegHeight > (Crouching.StandingLegHeight - Crouching.CrawlingLegHeight) / 3f)
                Crouching.TargetLegHeight = newLegHeight;

            if (newLegHeight >= Crouching.StandingLegHeight)
                StateMachine.ChangeState<StandState>();

            if (Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.linearVelocity.y < -0.1f && !_isFalling && _airTime > _minAirTime)
            {
                _isFalling = true;
                //float height = Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.position.y - StateMachine.Jumping.PhysicsRig.Colliders.LocomotionSphere.radius;
                //Debug.LogError("Height reached: " + height);
            }
        }

        protected override void Exit()
        {
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 1f;
        }
    }
}