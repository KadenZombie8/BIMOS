using System.Collections.Generic;
using System.Linq;
using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS.Guns
{
    /// <summary>
    /// Makes the item and any connected objects ignore player collision (good for two-handed weapons)
    /// </summary>
    [RequireComponent(typeof(Item))]
    public class PlayerCollisionIgnorer : MonoBehaviour
    {
        private readonly HashSet<Collider> _colliders = new();
        private readonly HashSet<Collider> _playerColliders = new();

        private Item _item;
        private bool _ignore;

        private void Awake() => _item = GetComponent<Item>();

        private void OnEnable()
        {
            foreach (GameObject gameObject in _item.GameObjects)
            {
                foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                    AddCollider(collider);
            }

            _item.OnGameObjectAdded += GameObjectAdded;
            _item.OnGameObjectRemoved += GameObjectRemoved;
        }

        private void OnDisable()
        {
            foreach (var collider in _colliders.ToArray())
                RemoveCollider(collider);

            _item.OnGameObjectAdded -= GameObjectAdded;
            _item.OnGameObjectRemoved -= GameObjectRemoved;
        }

        private void Start()
        {
            var colliders = BIMOSRig.Instance.PhysicsRig.Colliders;
            _playerColliders.Add(colliders.Head);
            _playerColliders.Add(colliders.Body);
            _playerColliders.Add(colliders.LocomotionSphere);
        }

        public void SetIgnorePlayerCollision(bool ignore)
        {
            _ignore = ignore;
            foreach (Collider collider in _colliders)
                IgnorePlayerCollision(collider, ignore);
        }

        private void GameObjectAdded(GameObject gameObject)
        {
            foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                AddCollider(collider);
        }

        private void GameObjectRemoved(GameObject gameObject)
        {
            foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                RemoveCollider(collider);
        }

        private void IgnorePlayerCollision(Collider collider, bool ignore)
        {
            foreach (var playerCollider in _playerColliders)
                if (collider && playerCollider)
                    Physics.IgnoreCollision(collider, playerCollider, ignore);
        }

        private void AddCollider(Collider collider)
        {
            _colliders.Add(collider);
            IgnorePlayerCollision(collider, _ignore);
        }

        private void RemoveCollider(Collider collider)
        {
            _colliders.Remove(collider);
            IgnorePlayerCollision(collider, false);
        }
    }
}
