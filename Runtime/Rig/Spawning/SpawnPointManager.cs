using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Spawning
{
    /// <summary>
    /// Manages the current spawn point and respawning the player
    /// </summary>
    public class SpawnPointManager : MonoBehaviour
    {
        public static SpawnPointManager Instance { get; private set; }
        [Header("Runtime References")]
        public BIMOSRig LocalPlayer;

        public event Action OnRespawn;  

        [Header("References")]
        public SpawnPoint SpawnPoint;
        public BIMOSRig PlayerPrefab;

        public bool SingleRig = true;
        public bool AutoCreatePlayer = true;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if(AutoCreatePlayer)
                SpawnPlayer(true);

            if (!SpawnPoint)
            {
                SpawnPoint = FindFirstObjectByType<SpawnPoint>();
                if (!SpawnPoint)
                {
                    Debug.LogError("You must have at least one spawn point!");
                    return;
                }
            }
        }

        public void SpawnPlayer(bool overrideCurrentPlayer = false) {
            if (LocalPlayer != null && SingleRig)
                return;
            var player = Instantiate(PlayerPrefab);
            Persistent.InvokeSpawn(player);
            if(overrideCurrentPlayer)
                LocalPlayer = player;
        }

        private void Start() => Respawn();

        public void SetSpawnPoint(SpawnPoint spawnPoint) => SpawnPoint = spawnPoint;

        public void Respawn(BIMOSRig rig = null)
        {
            if (!rig)
                rig = LocalPlayer;
            TeleportToSpawnPoint(SpawnPoint.transform);

            OnRespawn?.Invoke();
            Persistent.InvokeRespawn(this, SpawnPoint, rig);
        }

        private void TeleportToSpawnPoint(Transform spawnPoint, BIMOSRig rig = null)
        {
            if (!rig)
                rig = LocalPlayer;
            rig.PhysicsRig.GrabHandlers.Left.AttemptRelease();
            rig.PhysicsRig.GrabHandlers.Right.AttemptRelease();

            var rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
            var rootPosition = rig.PhysicsRig.Rigidbodies.LocomotionSphere.position;
            foreach (var rigidbody in rigidbodies)
            {
                var offset = rigidbody.position - rootPosition; //Calculates the offset between the locoball and the rigidbody
                rigidbody.position = spawnPoint.position + offset; //Sets the rigidbody's position
                rigidbody.transform.position = spawnPoint.position + offset; //Sets the transform's position

                if (rigidbody.isKinematic)
                    continue;

                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            //Update the animation rig's position
            rig.AnimationRig.Transforms.Hips.position += spawnPoint.position - rootPosition;

            //Move the player's animated feet to the new position
            rig.AnimationRig.Feet.TeleportFeet();

            rig.ControllerRig.transform.rotation = transform.rotation;
        }
    }
}