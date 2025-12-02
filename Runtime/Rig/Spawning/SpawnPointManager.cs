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

        public event Action OnRespawn;

        public SpawnPoint SpawnPoint;

        private BIMOSRig _player;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _player = BIMOSRig.Instance;

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

        public void SetSpawnPoint(SpawnPoint spawnPoint) => SpawnPoint = spawnPoint;

        public void Respawn()
        {
            TeleportToSpawnPoint(SpawnPoint.transform);

            OnRespawn?.Invoke();
        }

        private void TeleportToSpawnPoint(Transform spawnPoint)
        {
            _player.PhysicsRig.GrabHandlers.Left.AttemptRelease();
            _player.PhysicsRig.GrabHandlers.Right.AttemptRelease();

            var rigidbodies = _player.PhysicsRig.transform.GetComponentsInChildren<Rigidbody>();
            var rootPosition = _player.PhysicsRig.Rigidbodies.LocomotionSphere.position;

            foreach (var rigidbody in rigidbodies)
            {
                var offset = rigidbody.position - rootPosition; //Calculates the offset between the locoball and the rigidbody
                rigidbody.position = spawnPoint.position + offset; //Sets the rigidbody's position

                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            //Update the animation rig's position
            _player.AnimationRig.Transforms.Hips.position += spawnPoint.position - rootPosition;

            //Move the player's animated feet to the new position
            _player.AnimationRig.Feet.TeleportFeet();

            var deltaCameraRotation = (_player.ControllerRig.HeadForwardRotation * Quaternion.Inverse(spawnPoint.rotation)).eulerAngles;
            _player.ControllerRig.BaseForwardRotation *= Quaternion.Euler(0f, -deltaCameraRotation.y, 0f);
        }
    }
}