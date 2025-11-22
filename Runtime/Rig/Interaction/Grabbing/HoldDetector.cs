using System;
using System.Collections.Generic;
using KadenZombie8.BIMOS.Sockets;
using UnityEngine;
using UnityEngine.Events;

namespace KadenZombie8.BIMOS.Rig
{
    /// <summary>
    /// Detects when an object (through multiple grabs) is first held or finally released.
    /// </summary>
    public class HoldDetector : MonoBehaviour
    {
        public UnityEvent OnFirstGrab;
        public UnityEvent OnFinalRelease;

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
            IgnorePlayerCollision(false);

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

            var rigidbody = plug.Rigidbody;
            var colliders = rigidbody.GetComponentsInChildren<Collider>();
            var grabbables = rigidbody.GetComponentsInChildren<Grabbable>();

            if (isAttaching)
            {
                foreach (var collider in colliders)
                    _colliders.Add(collider);

                foreach (var grabbable in grabbables)
                {
                    _grabbables.Add(grabbable);
                    grabbable.OnGrab.AddListener(CheckGrabbables);
                    grabbable.OnRelease.AddListener(CheckGrabbables);
                }
            }
            else
            {
                foreach (var collider in colliders)
                    foreach (var playerCollider in _playerColliders)
                        Physics.IgnoreCollision(collider, playerCollider, false);

                foreach (var collider in colliders)
                    _colliders.Remove(collider);

                foreach (var grabbable in grabbables)
                {
                    _grabbables.Remove(grabbable);
                    grabbable.OnGrab.RemoveListener(CheckGrabbables);
                    grabbable.OnRelease.RemoveListener(CheckGrabbables);
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
                    IgnorePlayerCollision(true);
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
                        IgnorePlayerCollision(true);
                        return;
                    }
                }
            }
            IgnorePlayerCollision(false);
        }

        private void IgnorePlayerCollision(bool ignore)
        {
            foreach (var collider in _colliders)
                foreach (var playerCollider in _playerColliders)
                    if (collider && playerCollider)
                        Physics.IgnoreCollision(collider, playerCollider, ignore);
        }
    }
}
