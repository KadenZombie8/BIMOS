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

        private void Start() => TeleportToSpawnPoint(SpawnPoint.transform);

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
            var rootPosition = _player.PhysicsRig.Rigidbodies.LocomotionSphere.position + Vector3.down * _player.PhysicsRig.Colliders.LocomotionSphere.radius;
            var kneeOffset = Vector3.up * _player.PhysicsRig.Colliders.LocomotionSphere.radius;

            foreach (var rigidbody in rigidbodies)
            {
                if (rigidbody == _player.PhysicsRig.Rigidbodies.Knee)
                    continue;

                MoveRigidbody(rigidbody, spawnPoint, kneeOffset);
            }
            MoveRigidbody(_player.PhysicsRig.Rigidbodies.Knee, spawnPoint, kneeOffset);

            // Update the animation rig's position
            _player.AnimationRig.Transforms.HipsIK.position += spawnPoint.position - rootPosition;

            // Move the player's animated feet to the new position
            _player.AnimationRig.Feet.TeleportFeet();

            _player.ControllerRig.transform.position = _player.PhysicsRig.Rigidbodies.Pelvis.position;
            _player.AnimationRig.Head.UpdateCharacter();

            // Rotate the player to face the spawn point's forward direction
            var deltaCameraRotation = (_player.ControllerRig.HeadForwardRotation * Quaternion.Inverse(spawnPoint.rotation)).eulerAngles;
            _player.ControllerRig.transform.rotation *= Quaternion.Euler(0f, -deltaCameraRotation.y, 0f);
        }

        private void MoveRigidbody(Rigidbody rigidbody, Transform spawnPoint, Vector3 kneeOffset)
        {
            var knee = _player.PhysicsRig.Rigidbodies.Knee.transform;
            Quaternion forwardRotation = _player.ControllerRig.HeadForwardRotation;

            var positionOffset = Quaternion.Inverse(forwardRotation) * (rigidbody.position - knee.position);
            var newPosition = spawnPoint.rotation * positionOffset + kneeOffset;

            rigidbody.transform.position = newPosition; //Sets the rigidbody's transform position
            rigidbody.position = newPosition; //Sets the rigidbody's position

            var rotationOffset = Quaternion.Inverse(forwardRotation) * rigidbody.rotation;
            var newRotation = spawnPoint.rotation * rotationOffset;

            rigidbody.transform.rotation = newRotation; //Sets the rigidbody's transform rotation
            rigidbody.rotation = newRotation; //Sets the rigidbody's rotation

            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}