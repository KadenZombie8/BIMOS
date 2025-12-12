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

        private readonly Dictionary<Socket, (Action<Plug>, Action<Plug>)> _socketListeners = new();
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
                void attachListener(Plug _) => OnSocketChanged(socket, true);
                void detachListener(Plug _) => OnSocketChanged(socket, false);

                _socketListeners[socket] = (attachListener, detachListener);

                socket.OnAttachStart += attachListener;
                socket.OnDetachStart += detachListener;
            }
        }

        private void OnDisable()
        {
            foreach (var socket in _sockets)
            {
                if (_socketListeners.TryGetValue(socket, out var pair))
                {
                    socket.OnAttachStart -= pair.Item1;
                    socket.OnDetachStart -= pair.Item2;
                }
            }

            _socketListeners.Clear();
        }

        private void OnSocketChanged(Socket socket, bool isAttaching)
        {
            var plug = socket.Plug;
            if (!plug) return;

            var rigidbody = plug.Rigidbody;
            if (!rigidbody) return;

            var plugGameObject = rigidbody.gameObject;
            if (isAttaching)
            {
                if (GameObjects.Add(plugGameObject))
                    OnGameObjectAdded?.Invoke(plugGameObject);
            }
            else
            {
                if (GameObjects.Remove(plugGameObject))
                    OnGameObjectRemoved?.Invoke(plugGameObject);
            }
        }
    }
}
