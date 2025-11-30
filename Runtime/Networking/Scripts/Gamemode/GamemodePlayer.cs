using KadenZombie8.BIMOS;
using KadenZombie8.BIMOS.Rig;
using Mirror;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking.Samples {
    public class GamemodePlayer : NetworkBehaviour {
        public float MaxHealth = 30;
        public float Health { get; set; } = new();
        public uint Kills { get; set; } = new();
        public uint Deaths { get; set; } = new();
        public BIMOSRig rig;
        public static GamemodePlayer LocalRig = new();
        private void Start() {
            rig = GetComponent<BIMOSRig>();
            if (!LocalRig)
                return;
            LocalRig = this;
        }

        public void NewDeath() {
            Deaths++;
            RespawnRpc();
        }

        public void RespawnRpc() {
            GamemodeMarker.SpawnPlayer(this);
        }

        public void RequestKill() {
            Kills++;
        }

        public void RequestDamage(float damage) {
            Health -= damage;
            if (Health <= 0) {
                NewDeath();
            }
            Health = Mathf.Clamp(Health, 0, MaxHealth);
        }
    }
}