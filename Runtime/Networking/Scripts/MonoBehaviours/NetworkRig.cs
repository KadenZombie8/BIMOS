using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Spawning;
using Mirror;
using System;
using System.Linq;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking
{
    [RequireComponent(typeof(BIMOSRig))]
    public class NetworkRig : NetworkBehaviour
    {
        public BIMOSRig Rig {
            get; private set;
        }
        [SyncVar] public HandInputReaderStruct LeftReaderStruct;
        [SyncVar] public HandInputReaderStruct RightReaderStruct;
        public HandInputReader LeftHandReader;
        public HandInputReader RightHandReader;
        private void Awake() {
            Rig = GetComponent<BIMOSRig>();
            Persistent.InvokeSpawn(Rig);
        }

        private void Start() {
            if (!isLocalPlayer && NetworkManager.IsNetworkActive) {
                var cameras = GetComponents<Camera>().ToList();
                foreach (var camera in cameras) {
                    Destroy(camera);
                }
                var components = Rig.PhysicsRig.GetComponents<MonoBehaviour>().ToList();
                foreach (var component in components) {
                    component.enabled = false;
                }
            }  
        }
        public override void OnStartLocalPlayer() {
            base.OnStartLocalPlayer();
            SpawnPointManager.Instance.LocalPlayer = Rig;
            SpawnPointManager.Instance.Respawn(Rig);
        }
        protected override void OnValidate() {
            base.OnValidate();
            syncDirection = SyncDirection.ServerToClient;
        }
        private void Update() {
            if (isLocalPlayer) {     
                var leftReader = new HandInputReaderStruct(LeftHandReader);
                var rightReader = new HandInputReaderStruct(RightHandReader);
                CmdUpdateReaders(leftReader, rightReader);
            }
            else {
                LeftReaderStruct.Deserialize(LeftHandReader);
                RightReaderStruct.Deserialize(RightHandReader);
            }
        }

        [Command]
        private void CmdUpdateReaders(HandInputReaderStruct leftReader, HandInputReaderStruct rightReader) {
            LeftReaderStruct = leftReader;
            RightReaderStruct = rightReader;
        }
    }

    [Serializable]
    public struct HandInputReaderStruct {
        public float Trigger, Grip;
        public bool
            TriggerTouched,
            ThumbrestTouched,
            PrimaryTouched,
            PrimaryButton,
            SecondaryTouched,
            SecondaryButton,
            ThumbstickTouched;
        public HandInputReaderStruct(HandInputReader reader) {
            Trigger = reader.Trigger;
            Grip = reader.Grip;
            TriggerTouched = reader.TriggerTouched;
            ThumbrestTouched = reader.ThumbrestTouched;
            PrimaryTouched = reader.PrimaryTouched;
            PrimaryButton = reader.PrimaryButton;
            SecondaryTouched = reader.SecondaryTouched;
            SecondaryButton = reader.SecondaryButton;
            ThumbstickTouched = reader.ThumbstickTouched;
        }
        public void Serialize(HandInputReader reader) {
            Trigger = reader.Trigger;
            Grip = reader.Grip;
            TriggerTouched = reader.TriggerTouched;
            ThumbrestTouched = reader.ThumbrestTouched;
            PrimaryTouched = reader.PrimaryTouched;
            PrimaryButton = reader.PrimaryButton;
            SecondaryTouched = reader.SecondaryTouched;
            SecondaryButton = reader.SecondaryButton;
            ThumbstickTouched = reader.ThumbstickTouched;
        }

        public void Deserialize(HandInputReader reader) {
            reader.Trigger = Trigger;
            reader.Grip = Grip;
            reader.TriggerTouched = TriggerTouched;
            reader.ThumbrestTouched = ThumbrestTouched;
            reader.PrimaryTouched = PrimaryTouched;
            reader.PrimaryButton = PrimaryButton;
            reader.SecondaryTouched = SecondaryTouched;
            reader.SecondaryButton = SecondaryButton;
            reader.ThumbstickTouched = ThumbstickTouched;
        }
    }
}
