using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Spawning
{
    public class SpawnPoint : MonoBehaviour
    {
        private void Awake()
        {
            if (TryGetComponent<Renderer>(out var renderer))
                Destroy(renderer);
            if (TryGetComponent<MeshFilter>(out var meshFilter))
                Destroy(meshFilter);
        }
    }
}