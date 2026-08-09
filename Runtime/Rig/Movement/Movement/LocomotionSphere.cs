using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Controls a sphere Rigidbody that rolls to virtually move the player rig.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(Grounding))]
    public class LocomotionSphere : MonoBehaviour
    {
        public bool IsGrounded => _grounding.IsGrounded;

        private Rigidbody _rigidbody;
        private SphereCollider _collider;
        private Quaternion _linearToAngularRotation = Quaternion.Euler(0f, 90f, 0f);
        private Grounding _grounding;
        private readonly Vector3 _inertiaTensorRate = 60f * Vector3.one;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<SphereCollider>();
            _grounding = GetComponent<Grounding>();

            _rigidbody.solverIterations = 60;
            _rigidbody.solverVelocityIterations = 10;
            _rigidbody.maxAngularVelocity = Mathf.Infinity;
        }

        /// <summary>
        /// Sets the locomotion sphere's angular velocity to match the target linear velocity.
        /// </summary>
        /// <param name="targetLinearVelocity">The target linear velocity of the locomotion sphere.</param>
        public void RollFromLinearVelocity(Vector3 targetLinearVelocity)
        {
            var groundRotation = Quaternion.FromToRotation(Vector3.up, _grounding.GroundNormal);
            _rigidbody.angularVelocity = groundRotation * _linearToAngularRotation * targetLinearVelocity / _collider.radius;
            if (targetLinearVelocity.sqrMagnitude < 0.1f && !_grounding.IsSlipping)
                _rigidbody.inertiaTensor = Vector3.zero;
            else
                _rigidbody.inertiaTensor = _rigidbody.mass / 20f * Time.fixedDeltaTime * _inertiaTensorRate;
        }
    }
}