using System;
using System.Collections.Generic;
using System.Linq;
using KadenZombie8.BIMOS.Sockets;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Keeps track of an assembly of rigidbodies
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class Item : MonoBehaviour
    {
        public event Action<GameObject> OnGameObjectAdded;
        public event Action<GameObject> OnGameObjectRemoved;

        public readonly HashSet<GameObject> GameObjects = new();

        private readonly HashSet<Collider> _colliders = new();
        private readonly HashSet<Grabbable> _grabbables = new();
        private readonly HashSet<Socket> _sockets = new();
        private readonly HashSet<Hand> _hands = new();

        private void Awake()
        {
            var body = Utilities.GetBody(transform, out _, out _);
            GameObjects.Add(body.gameObject);

            foreach (var grabbable in GetComponentsInChildren<Grabbable>())
                _grabbables.Add(grabbable);

            foreach (var socket in GetComponentsInChildren<Socket>())
                _sockets.Add(socket);
        }

        private void OnEnable()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                foreach (var collider in gameObject.GetComponentsInChildren<Collider>())
                    AddCollider(collider);
            }

            OnGameObjectAdded += GameObjectAdded;
            OnGameObjectRemoved += GameObjectRemoved;

            foreach (var grabbable in _grabbables)
            {
                grabbable.OnGrab?.AddListener(OnGrab);
                grabbable.OnRelease?.AddListener(OnRelease);
            }

            foreach (var socket in _sockets)
            {
                socket.Events.Align.OnStart?.AddListener(OnSocketAttach);
                socket.Events.Detach.OnStart?.AddListener(OnSocketDetach);
            }
        }

        private void OnDisable()
        {
            foreach (var collider in _colliders.ToArray())
                RemoveCollider(collider);

            OnGameObjectAdded -= GameObjectAdded;
            OnGameObjectRemoved -= GameObjectRemoved;

            foreach (var grabbable in _grabbables)
            {
                grabbable.OnGrab?.RemoveListener(OnGrab);
                grabbable.OnRelease?.RemoveListener(OnRelease);
            }

            foreach (var socket in _sockets)
            {
                socket.Events.Align.OnStart?.RemoveListener(OnSocketAttach);
                socket.Events.Detach.OnStart?.RemoveListener(OnSocketDetach);
            }
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

        private void AddCollider(Collider collider)
        {
            if (!_colliders.Add(collider)) return;
            foreach (var hand in _hands)
                IgnoreArmCollision(hand, collider, true);
        }

        private void RemoveCollider(Collider collider)
        {
            if (!_colliders.Remove(collider)) return;
            foreach (var hand in _hands)
                IgnoreArmCollision(hand, collider, false);
        }

        private void OnGrab(Hand hand)
        {
            if (!_hands.Add(hand)) return;
            foreach (var collider in _colliders) 
                IgnoreArmCollision(hand, collider, true);
        }

        private void OnRelease(Hand hand)
        {
            if (!_hands.Remove(hand)) return;
            foreach (var collider in _colliders)
                IgnoreArmCollision(hand, collider, false);
        }

        public void IgnoreArmCollision(Hand hand, Collider collider, bool ignore)
        {
            if (!collider) return;
            Physics.IgnoreCollision(collider, hand.ArmColliders.UpperArm, ignore);
            Physics.IgnoreCollision(collider, hand.ArmColliders.LowerArm, ignore);
            Physics.IgnoreCollision(collider, hand.ArmColliders.Hand, ignore);
        }

        private void OnSocketAttach(Plug plug)
        {
            var plugGameObject = GetPlugGameObject(plug);

            if (GameObjects.Add(plugGameObject))
                OnGameObjectAdded?.Invoke(plugGameObject);
        }

        private void OnSocketDetach(Plug plug)
        {
            var plugGameObject = GetPlugGameObject(plug);

            if (GameObjects.Remove(plugGameObject))
                OnGameObjectRemoved?.Invoke(plugGameObject);
        }

        private GameObject GetPlugGameObject(Plug plug)
        {
            var rigidbody = plug.Rigidbody;
            if (!rigidbody) return null;

            return rigidbody.gameObject;
        }
    }
}
