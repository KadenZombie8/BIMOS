using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Spawning;
using KadenZombie8.BIMOS.SDK;
using KadenZombie8.Pooling;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KadenZombie8.BIMOS.Networking {
    public class NetworkSpawnPointManager : SpawnPointManager {
        public Dictionary<uint, BIMOSRig> RigCache {
            get; internal set;
        } = new();
        public bool PhysicsRigGrips = true;

        public override void Start() {
            base.Start();
            NetworkManager.Instance.onStartClient.AddListener(()=>SpawnPlayer());
        }

        public override void SpawnPlayer(bool overrideCurrentPlayer = false) {
            try {
                NetworkManager.Instance.autoCreatePlayer = false;
                NetworkManager.Instance.playerPrefab = PlayerPrefab.gameObject;
                if (NetworkClient.ready)
                    NetworkClient.AddPlayer();
            }
            catch {
                base.SpawnPlayer(overrideCurrentPlayer);
            }
        }
    }
}