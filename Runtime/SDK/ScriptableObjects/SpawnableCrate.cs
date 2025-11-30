using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.SDK
{
    [CreateAssetMenu(fileName = "Spawnable Crate", menuName = "BIMOS/SDK/Crate")]
    public class SpawnableCrate : GameObjectCrate {
        public BasicBounds _bounds;
        public Bounds Bounds {
            get => new Bounds(_bounds._center, _bounds._size);
        }
    }

    [Serializable]
    public struct BasicBounds {
        public Vector3 _center;
        public Vector3 _size;
    }

    public abstract class GameObjectCrate : Crate<GameObject> {
    
    }
}
