using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Spawning;
using Mirror;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking
{
    [RequireComponent(typeof(BIMOSRig))]
    public class NetworkRig : NetworkBehaviour
    {
        public BIMOSRig Rig {
            get; private set;
        }
        private void Awake() {
            Rig = GetComponent<BIMOSRig>();
            Persistent.InvokeSpawn(Rig);
        }

        public override void OnStartAuthority() {
            base.OnStartAuthority();
            SpawnPointManager.Instance.LocalPlayer = Rig;
            SpawnPointManager.Instance.Respawn(Rig);
        }
    }
}
