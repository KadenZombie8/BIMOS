using System;
using System.Collections.Generic;
using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Sockets;
using UnityEngine;

namespace KadenZombie8.BIMOS.Guns
{
    /// <summary>
    /// Makes the gun ignore player collision (good for two-handed weapons)
    /// </summary>
    public class PlayerCollisionIgnorer : MonoBehaviour
    {
        public event Action OnGrabbed;
        public event Action OnReleased;

        private readonly Dictionary<Socket, (Action, Action)> _socketListeners = new();

        private readonly HashSet<Grabbable> _grabbables = new();
        private readonly HashSet<Socket> _sockets = new();
        private readonly HashSet<Collider> _colliders = new();
        private readonly HashSet<Collider> _playerColliders = new();

        private void Awake()
        {
            foreach (var grabbable in GetComponentsInChildren<Grabbable>())
                _grabbables.Add(grabbable);
            foreach (var socket in GetComponentsInChildren<Socket>())
                _sockets.Add(socket);
            foreach (var collider in GetComponentsInChildren<Collider>())
                _colliders.Add(collider);
        }

        private void Start()
        {
            var colliders = BIMOSRig.Instance.PhysicsRig.Colliders;
            _playerColliders.Add(colliders.Head);
            _playerColliders.Add(colliders.Body);
            _playerColliders.Add(colliders.LocomotionSphere);
        }

        private void OnEnable()
        {
            foreach (var grabbable in _grabbables)
            {
                grabbable.OnGrab.AddListener(CheckGrabbables);
                grabbable.OnRelease.AddListener(CheckGrabbables);
            }

            foreach (var socket in _sockets)
            {
                void attachListener() => UpdateSocketColliders(socket, true);

                void detachListener() => UpdateSocketColliders(socket, false);

                _socketListeners[socket] = (attachListener, detachListener);

                socket.OnAttach += attachListener;
                socket.OnDetach += detachListener;
            }
        }

        private void OnDisable()
        {
            IgnorePlayerCollision(false, _colliders);

            foreach (var grabbable in _grabbables)
            {
                grabbable.OnGrab.RemoveListener(CheckGrabbables);
                grabbable.OnRelease.RemoveListener(CheckGrabbables);
            }

            foreach (var socket in _sockets)
            {
                if (_socketListeners.TryGetValue(socket, out var pair))
                {
                    socket.OnAttach -= pair.Item1;
                    socket.OnDetach -= pair.Item2;
                }
            }

            _socketListeners.Clear();
        }

        private void UpdateSocketColliders(Socket socket, bool isAttaching)
        {
            var plug = socket.Plug;
            if (!plug)
                return;

            var plugRigidbody = plug.Rigidbody;
            var plugColliders = plugRigidbody.GetComponentsInChildren<Collider>();
            var plugGrabbables = plugRigidbody.GetComponentsInChildren<Grabbable>();

            if (isAttaching)
            {
                foreach (var plugCollider in plugColliders)
                    _colliders.Add(plugCollider);

                foreach (var plugGrabbable in plugGrabbables)
                {
                    _grabbables.Add(plugGrabbable);
                    plugGrabbable.OnGrab.AddListener(CheckGrabbables);
                    plugGrabbable.OnRelease.AddListener(CheckGrabbables);
                }
            }
            else
            {
                IgnorePlayerCollision(false, plugColliders);

                foreach (var plugCollider in plugColliders)
                    _colliders.Remove(plugCollider);

                foreach (var plugGrabbable in plugGrabbables)
                {
                    _grabbables.Remove(plugGrabbable);
                    plugGrabbable.OnGrab.RemoveListener(CheckGrabbables);
                    plugGrabbable.OnRelease.RemoveListener(CheckGrabbables);
                }
            }

            CheckGrabbables();
        }

        private void CheckGrabbables()
        {
            foreach (var grabbable in _grabbables)
            {
                if (grabbable.LeftHand != grabbable.RightHand)
                {
                    IgnorePlayerCollision(true, _colliders);
                    return;
                }
            }
            foreach (var socket in _sockets)
            {
                var plug = socket.Plug;
                if (!plug)
                    continue;

                var rigidbody = plug.Rigidbody;
                var grabbables = rigidbody.GetComponentsInChildren<Grabbable>();
                foreach (var grabbable in grabbables)
                {
                    if (grabbable.LeftHand != grabbable.RightHand)
                    {
                        IgnorePlayerCollision(true, _colliders);
                        return;
                    }
                }
            }
            IgnorePlayerCollision(false, _colliders);
        }

        private void IgnorePlayerCollision(bool ignore, IEnumerable<Collider> colliders)
        {
            foreach (var collider in colliders)
                foreach (var playerCollider in _playerColliders)
                    if (collider && playerCollider)
                        Physics.IgnoreCollision(collider, playerCollider, ignore);
        }
    }
}
