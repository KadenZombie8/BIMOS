using KadenZombie8.BIMOS.SDK;
using Mirror;
using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking
{
    public class NetworkBodyUnreliable : NetworkTransformUnreliable
    {
        public BIMOSBody Body {
            get; set;
        }
        private void Start() {
            Body = target.GetComponent<BIMOSBody>();
        }
        public override void OnSerialize(NetworkWriter writer, bool initialState) {
            base.OnSerialize(writer, initialState);
            if (initialState) {
                writer.WriteBody(Body);
            }
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState) {
            base.OnDeserialize(reader, initialState);
            if (initialState) {
                SetBody(reader.ReadBody());
            }
        }

        private void SetBody(BodyWriterReader.BodySnapshot bodySnapshot) {
            Body.UseGravity = bodySnapshot.UseGravity;
            Body.Immovable = bodySnapshot.Immovable;
            Body.Velocity = bodySnapshot.Velocity;
            Body.AngularVelocity = bodySnapshot.AngularVelocity;
        }
    }
    public static class BodyWriterReader {
        public struct BodySnapshot {
            public bool UseGravity;
            public bool Immovable;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
        }
        public static void WriteBody(this NetworkWriter writer, BIMOSBody value) {
            writer.WriteBool(value.UseGravity);
            writer.WriteBool(value.Immovable);
            writer.WriteVector3(value.Velocity);
            writer.WriteVector3(value.AngularVelocity);
        }
        public static BodySnapshot ReadBody(this NetworkReader reader) {
            BodySnapshot snapshot = new BodySnapshot();
            snapshot.UseGravity = reader.ReadBool();
            snapshot.Immovable = reader.ReadBool();
            snapshot.Velocity = reader.ReadVector3();
            snapshot.AngularVelocity = reader.ReadVector3();
            return snapshot;
        }
    }


}