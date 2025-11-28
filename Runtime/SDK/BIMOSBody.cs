using UnityEngine;

namespace KadenZombie8.BIMOS.SDK {
    [DisallowMultipleComponent]
    public class BIMOSBody : MonoBehaviour
    {
        public Rigidbody Rigidbody {
            get;set;
        }
        public ArticulationBody ArticulationBody {
            get; set;
        }
        public bool HasPhysicsBody => Rigidbody || ArticulationBody;
        public bool UseGravity {
            get {
                return Rigidbody ? Rigidbody.useGravity : ArticulationBody.useGravity;
            }
            set {
                if (Rigidbody)
                    Rigidbody.useGravity = value;
                else
                    ArticulationBody.useGravity = value;
            }
        }
        public bool Immovable {
            get {
                return Rigidbody ? Rigidbody.isKinematic : ArticulationBody.immovable;
            }
            set {
                if (Rigidbody)
                    Rigidbody.isKinematic = value;
                else
                    ArticulationBody.immovable = value;
            }
        }
        public Vector3 Velocity {
            get {
                return Rigidbody ? Rigidbody.linearVelocity : ArticulationBody.linearVelocity;
            }
            set {
                if(Rigidbody) Rigidbody.linearVelocity = value;
                else ArticulationBody.linearVelocity = value;
            }
        }
        public Vector3 AngularVelocity {
            get {
                return Rigidbody ? Rigidbody.angularVelocity : ArticulationBody.angularVelocity;
            }
            set {
                if (Rigidbody)
                    Rigidbody.angularVelocity = value;
                else
                    ArticulationBody.angularVelocity = value;
            }
        }
        
        public void AddForce(Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode) {
            if (Rigidbody)
                Rigidbody.AddForce(force, mode);
            else
                ArticulationBody.AddForce(force, mode);
        }
        public void AddTorque(Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode) {
            if (Rigidbody)
                Rigidbody.AddTorque(force, mode);
            else
                ArticulationBody.AddTorque(force, mode);
        }

        private void Start() {
            if(!Rigidbody && !ArticulationBody) Rigidbody = gameObject.AddComponent<Rigidbody>();
        }
    }
}
