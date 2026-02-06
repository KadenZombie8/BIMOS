using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Jump released state
    /// </summary>
    public class PushState : JumpState
    {
        private float _pushTime;
        private float _targetVelocity;
        private float _timeToRise;

        protected override void Enter()
        {
            _pushTime = 0f;

            // Calculate target jump height
            var minAnticipationHeight = Crouching.CrouchingLegHeight - Jumping.AnticipationHeight;
            var maxAnticipationHeight = Crouching.StandingLegHeight - Jumping.AnticipationHeight;
            var legHeight = Jumping.PhysicsRig.Rigidbodies.Pelvis.position.y - Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.position.y;
            var crouchingAmount = 1f - (legHeight - minAnticipationHeight) / (maxAnticipationHeight - minAnticipationHeight);
            var jumpHeight = Jumping.JumpHeightCurve.Evaluate(crouchingAmount);

            // Return to full standing height
            var difference = Crouching.StandingLegHeight - Crouching.TargetLegHeight;
            Crouching.TargetLegHeight = Crouching.StandingLegHeight;

            // Don't allow jumping midair
            if (!Jumping.LocomotionSphere.IsGrounded)
                return;

            // Prevent early liftoff
            Jumping.PhysicsRig.Joints.Pelvis.massScale = 0.01f;

            // Calculate required velocity to reach target jump height
            _targetVelocity = Mathf.Sqrt(2f * -Physics.gravity.y * jumpHeight);
            Jumping.PhysicsRig.Joints.Pelvis.targetVelocity = Vector3.up * _targetVelocity;
            _timeToRise = difference / _targetVelocity;

            // Set position spring to 0 so only controlled by target velocity
            var riseJointDrive = Jumping.PhysicsRig.JointDrives.Pelvis;
            riseJointDrive.positionSpring = 0f;
            Jumping.PhysicsRig.Joints.Pelvis.xDrive
                = Jumping.PhysicsRig.Joints.Pelvis.yDrive
                = Jumping.PhysicsRig.Joints.Pelvis.zDrive
                = riseJointDrive;
        }

        protected override void Update()
        {
            // Wait for player to stand up before jumping
            if (_pushTime > _timeToRise)
                StateMachine.ChangeState<FlyState>();

            _pushTime += Time.deltaTime;
        }

        protected override void Exit()
        {
            Jumping.PhysicsRig.Joints.Pelvis.targetVelocity = Vector3.zero;

            // Reset position spring so not controlled by target velocity
            Jumping.PhysicsRig.Joints.Pelvis.xDrive
                = Jumping.PhysicsRig.Joints.Pelvis.yDrive
                = Jumping.PhysicsRig.Joints.Pelvis.zDrive
                = Jumping.PhysicsRig.JointDrives.Pelvis;

            // Don't allow jumping midair
            if (!Jumping.LocomotionSphere.IsGrounded)
                return;

            // Add jump velocity
            var jumpVelocity = Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.linearVelocity;
            jumpVelocity.y += _targetVelocity;

            StateMachine.Jumping.PhysicsRig.Rigidbodies.LocomotionSphere.linearVelocity = jumpVelocity;
            StateMachine.Jumping.PhysicsRig.Rigidbodies.Knee.linearVelocity = jumpVelocity;
            StateMachine.Jumping.PhysicsRig.Rigidbodies.Pelvis.linearVelocity = jumpVelocity;
            StateMachine.Jumping.PhysicsRig.Rigidbodies.Head.linearVelocity = jumpVelocity;

            Jumping.PhysicsRig.Joints.Pelvis.massScale = 1f;
        }
    }
}