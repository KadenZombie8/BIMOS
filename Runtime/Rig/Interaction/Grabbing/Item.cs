using System;
using System.Collections.Generic;
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

        private readonly HashSet<Socket> _sockets = new();

        private void Awake()
        {
            var body = Utilities.GetBody(transform, out _, out _);
            GameObjects.Add(body.gameObject);

            foreach (var socket in GetComponentsInChildren<Socket>())
                _sockets.Add(socket);
        }

        private void OnEnable()
        {
            foreach (var socket in _sockets)
            {
                socket.Events.Attach.OnStart?.AddListener(OnSocketAttach);
                socket.Events.Detach.OnStart?.AddListener(OnSocketDetach);
            }
        }

        private void OnDisable()
        {
            foreach (var socket in _sockets)
            {
                socket.Events.Attach.OnStart?.RemoveListener(OnSocketAttach);
                socket.Events.Detach.OnStart?.RemoveListener(OnSocketDetach);
            }
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
