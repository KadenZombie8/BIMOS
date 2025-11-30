using KadenZombie8.BIMOS.SDK;
using Mirror;
using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking
{
    [RequireComponent(typeof(BIMOSBody))]
    public class NetworkBodyUnreliable : NetworkTransformUnreliable
    {
        public BIMOSBody Body {
            get; set;
        }
        private void Start() {
            Body = target.GetComponent<BIMOSBody>();
        }
        public override void OnSerialize(NetworkWriter writer, bool initialState) {
            if (initialState) {
                writer.WriteVector3(GetPosition());
                writer.WriteQuaternion(GetRotation());
                writer.WriteBody(Body);
            }
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState) {
            if (initialState) {
                SetPosition(reader.ReadVector3());
                SetRotation(reader.ReadQuaternion());
                reader.ReadBody(Body);
            }
        }
    }
    public static class BodyWriterReader {
        public static void WriteBody(this NetworkWriter writer, BIMOSBody value) {
            writer.WriteBool(value.UseGravity);
            writer.WriteBool(value.Immovable);
            writer.WriteVector3(value.Velocity);
            writer.WriteVector3(value.AngularVelocity);
        }
        public static BIMOSBody ReadBody(this NetworkReader reader, BIMOSBody value) {
            value.UseGravity = reader.ReadBool();
            value.Immovable = reader.ReadBool();
            value.Velocity = reader.ReadVector3();
            value.AngularVelocity = reader.ReadVector3();
            return value;
        }
    }


}