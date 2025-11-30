using UnityEngine;

namespace KadenZombie8.BIMOS.SDK {
    [DisallowMultipleComponent]
    public class BIMOSBody : MonoBehaviour
    {
        public Rigidbody Rigidbody {
            get; set;
        }
        public ArticulationBody ArticulationBody {
            get; set;
        }
        public bool HasPhysicsBody => Rigidbody != null || ArticulationBody != null;
        public bool UseGravity {
            get {
                return Rigidbody != null ? Rigidbody.useGravity : ArticulationBody.useGravity;
            }

            set {
                if (Rigidbody != null)
                    Rigidbody.useGravity = value;
                else
                    ArticulationBody.useGravity = value;
            }
        }
        public bool Immovable {
            get {
                return Rigidbody != null ? Rigidbody.isKinematic : ArticulationBody.immovable;
            }

            set {
                if (Rigidbody != null)
                    Rigidbody.isKinematic = value;
                else
                    ArticulationBody.immovable = value;
            }
        }
        public Vector3 Velocity {
            get {
                return Rigidbody != null ? Rigidbody.linearVelocity : ArticulationBody.linearVelocity;
            }

            set {
                if (Rigidbody != null)
                    Rigidbody.linearVelocity = value;
                else
                    ArticulationBody.linearVelocity = value;
            }
        }
        public Vector3 AngularVelocity {
            get {
                return Rigidbody != null ? Rigidbody.angularVelocity : ArticulationBody.angularVelocity;
            }

            set {
                if (Rigidbody != null)
                    Rigidbody.angularVelocity = value;
                else
                    ArticulationBody.angularVelocity = value;
            }
        }

        public void AddForce(Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode) {
            if (Rigidbody != null)
                Rigidbody.AddForce(force, mode);
            else
                ArticulationBody.AddForce(force, mode);
        }
        public void AddTorque(Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode) {
            if (Rigidbody != null)
                Rigidbody.AddTorque(force, mode);
            else
                ArticulationBody.AddTorque(force, mode);
        }

        private void Start() {
            ArticulationBody = this.GetComponent<ArticulationBody>();
            Rigidbody = this.AddOrGetComponent<Rigidbody>();
        }
    }
}
