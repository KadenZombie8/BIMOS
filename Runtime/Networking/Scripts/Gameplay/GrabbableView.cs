using UnityEngine;
using KadenZombie8.BIMOS.Rig;
using System.Collections.Generic;
using Mirror;
namespace KadenZombie8.BIMOS.Networking {
    [RequireComponent(typeof(Grabbable))]
    public class GrabbableView : MonoBehaviourNetwork {
        public static Dictionary<int, GrabbableView> Views { get; private set; } = new Dictionary<int, GrabbableView>();
        public static int NextId { get; private set; } = 0;
        public Grabbable Grabbable {
            get; set;
        }
        public List<NetworkConnectionToClient> OwnershipQueue { get; private set; } = new();

        public override bool HostIsOwnerDefault => true;

        private void Awake() {
            Grabbable = GetComponent<Grabbable>();
            Grabbable.OnGrab.AddListener(OnGrabbed);
            Grabbable.OnRelease.AddListener(OnReleased);
        }
        [Client]
        private void OnGrabbed() {
            SendGripTakeoverCmd();
        }
        [Client]
        private void OnReleased() {
            SendGripReleaseCmd();
        }

        [Command(requiresAuthority = false)]
        private void SendGripTakeoverCmd(NetworkConnectionToClient conn = null) {
            if (!OwnershipQueue.Contains(conn))
                OwnershipQueue.Add(conn);
            AssignOwnership();
        }
        [Command(requiresAuthority = false)]
        private void SendGripReleaseCmd(NetworkConnectionToClient conn = null) {
            if (OwnershipQueue.Contains(conn))
                OwnershipQueue.Remove(conn);
            AssignOwnership();
        }

        [Server]
        private void AssignOwnership() {
            if (OwnershipQueue.Count <= 0 || connectionToClient == OwnershipQueue[0])
                return;
            netIdentity.RemoveClientAuthority();
            netIdentity.AssignClientAuthority(OwnershipQueue[0]);
        }
    }
}