using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Spawning
{
    /// <summary>
    /// Manages spawning the starter prop
    /// </summary>
    [RequireComponent(typeof(SpawnPointManager))]
    public class StarterPropManager : MonoBehaviour
    {
        [Tooltip("Inserts this prefab in front of player on respawn")]
        public GameObject StarterProp;

        public Vector3 StarterPropOffset = new(0, 1f, 1f);

        private GameObject _starterPropInstance;
        private SpawnPointManager _spawnPointManager;

        private void Awake() => _spawnPointManager = GetComponent<SpawnPointManager>();

        private void OnEnable() => _spawnPointManager.OnRespawn += Respawned;

        private void OnDisable() => _spawnPointManager.OnRespawn -= Respawned;

        public void SetStarterPropOffsetX(float x) => StarterPropOffset.x = x;

        public void SetStarterPropOffsetY(float y) => StarterPropOffset.y = y;

        public void SetStarterPropOffsetZ(float z) => StarterPropOffset.z = z;

        public void SetStarterProp(GameObject starterProp) => StarterProp = starterProp;

        private void Respawned()
        {
            Destroy(_starterPropInstance);
            if (!StarterProp)
                return;

            _starterPropInstance = Instantiate(
                StarterProp,
               _spawnPointManager.LocalPlayer.ControllerRig.transform.TransformPoint(StarterPropOffset),
                _spawnPointManager.LocalPlayer.ControllerRig.transform.rotation
            );
        }
    }
}
