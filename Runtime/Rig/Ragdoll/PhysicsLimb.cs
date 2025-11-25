using UnityEngine;

namespace KadenZombie8.BIMOS.Ragdoll {
    [RequireComponent(typeof(Rigidbody)), DisallowMultipleComponent]
    public class PhysicsLimb : MonoBehaviour
    {
        public PhysicsRagdoll Ragdoll { get; private set; }
        public Rigidbody Rigidbody {
            get; private set;
        }
        public float forceMultiplier = 1;
        public Transform virtualLimb;
        public bool virtualFollowPhysics;
        private void Awake() {
            Ragdoll = GetComponentInParent<PhysicsRagdoll>();
            Rigidbody = GetComponent<Rigidbody>();
            Ragdoll.Limbs.Add(this);
        }

        private void FixedUpdate() {
            if(virtualFollowPhysics) virtualLimb.position = transform.position;

            Rigidbody.inertiaTensor = Vector3.one * Ragdoll.inertiaTensorResitance;

            Quaternion deltaRotation = virtualLimb.rotation *
              Quaternion.Inverse(transform.rotation);

            if (deltaRotation.w < 0)
                deltaRotation = Quaternion.Inverse(deltaRotation);

            deltaRotation.ToAngleAxis(out var angle, out var axis);
            axis.Normalize();
            angle *= Mathf.Deg2Rad;
            Vector3 angularVelocityError = angle / Time.fixedDeltaTime * axis;

            Vector3 torque = Ragdoll.proportionalGain * angularVelocityError - Ragdoll.derivativeGain *
              Rigidbody.angularVelocity;

            torque = Vector3.ClampMagnitude(torque, Ragdoll.maxStrength);

            Rigidbody.AddTorque(torque * forceMultiplier, ForceMode.Force);
        }

        public void SetVirtualBone(Transform newBone) => virtualLimb = newBone;
    }
}
