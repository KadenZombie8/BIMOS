using KadenZombie8.Pooling;
using UnityEngine;

namespace KadenZombie8.BIMOS.SDK
{
    public class BIMOSPoolManager : PoolManager
    {
        public static BIMOSPoolManager BIMOSInstance
        {
            get; private set;
        }
        public PoolConfig EntitiesPoolConfig;
        public PoolConfig RigsPoolConfig;

        private void Start() {
            BIMOSInstance = this;
            RegisterPool(RigsPoolConfig);
            RegisterPool(EntitiesPoolConfig);
        }
    }
}
