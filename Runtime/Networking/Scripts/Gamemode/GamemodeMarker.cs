using KadenZombie8.BIMOS.Rig.Spawning;
using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking.Samples
{
    public class GamemodeMarker : MonoBehaviour
    {
        public static List<GamemodeMarker> Markers { get; set; } = new();

        private void OnEnable() {
            Markers.Add(this);
        }
        private void OnDisable() {
            Markers.Remove(this);
        }

        public static void SpawnPlayer(GamemodePlayer player, int position = -1) {
            SpawnPointManager.Instance.TeleportToSpawnPoint(Markers.GetRandomItem().transform, player.rig);
        }
    }
}
