using System;
using System.Collections.Generic;
using KadenZombie8.BIMOS.Sockets;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class StorableItem : MonoBehaviour
    {
        public event Action OnStored;
        public event Action OnRetrieved;

        public string[] Tags = { "Light" };
        public RetrieveGrabbablesStruct RetrieveGrabbables;

        [Serializable]
        public struct RetrieveGrabbablesStruct
        {
            public Grabbable Left;
            public Grabbable Right;
        }

        public readonly HashSet<GameObject> GameObjects = new();

        private readonly Dictionary<Socket, (Action, Action)> _socketListeners = new();
        private readonly HashSet<Socket> _sockets = new();

        public event Action<GameObject> OnGameObjectAdded;
        public event Action<GameObject> OnGameObjectRemoved;

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
                void attachListener() => UpdateGameObjects(socket, true);
                void detachListener() => UpdateGameObjects(socket, false);

                _socketListeners[socket] = (attachListener, detachListener);

                socket.OnAttach += attachListener;
                socket.OnDetach += detachListener;
            }
        }

        private void OnDisable()
        {
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

        private void UpdateGameObjects(Socket socket, bool isAttaching)
        {
            var plug = socket.Plug;
            if (!plug) return;

            var rigidbody = plug.Rigidbody;
            if (!rigidbody) return;

            var plugGameObject = rigidbody.gameObject;
            if (isAttaching)
                GameObjects.Add(plugGameObject);
            else
                GameObjects.Remove(plugGameObject);
        }
    }
}
