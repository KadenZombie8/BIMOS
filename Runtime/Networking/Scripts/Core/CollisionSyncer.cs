using Mirror;
using System;
using UnityEngine;
using KadenZombie8.BIMOS.SDK;
namespace KadenZombie8.BIMOS.Networking
{
    [RequireComponent(typeof(Collider))]
    public class CollisionSyncer : MonoBehaviourNetwork
    {
        public bool authorityOnCollision = false;
        public float thresholdImpulse = 0.1f;
        public override bool HostIsOwnerDefault => true;
        public BIMOSBody Body {
            get; set;
        }
        public Collider Collider { get; set; }
        private void Start() {
            Collider = GetComponent<Collider>();
            Body = GetComponentInParent<BIMOSBody>();
            if (!Body) {
                Debug.LogError("No BIMOS Body Found, Creating at collider's attached body", this);
                var body = Collider.attachedRigidbody ? (Component) Collider.attachedRigidbody : Collider.attachedArticulationBody;
                Body = body.AddComponent<BIMOSBody>();
                Body.Rigidbody = Collider.attachedRigidbody;
                Body.ArticulationBody = Collider.attachedArticulationBody;
                Debug.Log("Created BIMOS Body on  collider's attached body", Body);
                return;
            }
        }
        private void OnCollisionEnter(Collision collision) {
            if (collision.impulse.sqrMagnitude < thresholdImpulse)
                return;
            var info = new BodyInfo();
            info.Serialize(Body);
            CollisionCmd(info);
        }

        [Command(requiresAuthority = false)]
        public void CollisionCmd(BodyInfo info, NetworkConnectionToClient conn = null) {
            if (!authorityOnCollision && conn != connectionToClient)
                return;
            else if (authorityOnCollision) {
                netIdentity.RemoveClientAuthority();
                netIdentity.AssignClientAuthority(conn);
            }
            if (!NetworkClient.active) {
                info.Deserialize(Body);
            }
            CollisionRpc(info);
        }
        [ClientRpc]
        private void CollisionRpc(BodyInfo info) {
            info.Deserialize(Body);
        }
    }

    public struct BodyInfo {
        Vector3 pos, vel, angVel;
        Quaternion rot;
        public void Serialize(BIMOSBody body) {
            pos = body.transform.position;
            rot = body.transform.rotation;
            vel = body.Velocity;
            angVel = body.AngularVelocity;
        }
        public void Serialize(Transform transform) {
            pos = transform.position;
            rot = transform.rotation;
        }
        public void Deserialize(BIMOSBody body) {
            body.transform.position = pos;
            body.transform.rotation = rot;
            body.Velocity = vel;
            body.AngularVelocity = angVel;
        }
        public void Deserialize(Transform transform) {
            pos = transform.position = pos;
            rot = transform.rotation = rot;
        }
        public void Serialize(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel) {
            this.pos = pos;
            this.rot = rot;
            this.vel = vel;
            this.angVel = angVel;
        }
    }
}
