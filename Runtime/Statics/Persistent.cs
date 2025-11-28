using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Spawning;
using System;

namespace KadenZombie8.BIMOS
{
    public static class Persistent
    {
        public static event Action<BIMOSRig> OnSpawn;
        public static event Action<SpawnPointManager, SpawnPoint, BIMOSRig> OnRespawn;

        public static void InvokeSpawn(BIMOSRig rig)
        {
            OnSpawn?.Invoke(rig);
        }

        public static void InvokeRespawn(SpawnPointManager manager, SpawnPoint point, BIMOSRig rig) {
            OnRespawn?.Invoke(manager, point, rig);
        }
    }
}
