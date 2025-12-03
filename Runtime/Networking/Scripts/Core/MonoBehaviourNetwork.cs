using Mirror;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking {

    [DefaultExecutionOrder(-1)]
    public abstract class MonoBehaviourNetwork : NetworkBehaviour {
        public abstract bool HostIsOwnerDefault {
            get;
        }
        public override void OnStartClient() {
            if (HostIsOwnerDefault && NetworkServer.active) {
                netIdentity.RemoveClientAuthority();
                netIdentity.AssignClientAuthority(NetworkServer.localConnection);
            }
        }
    }
}