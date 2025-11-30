using KadenZombie8.BIMOS.Rig;
using KadenZombie8.Pooling;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Networking {
    [DefaultExecutionOrder(-1500)]
    public class NetworkManager : Mirror.NetworkManager {
        public static NetworkManager Instance {
            get; private set;
        }
        public override void Awake() {
            InitializeOnce();
            base.Awake();
        }

        public virtual void InitializeOnce() {
            SetProperties();
            Register();      
        }

        public virtual void SetProperties() {
        }

        public virtual void Register() {
                  
        }

        public UnityEvent onStartServer, onStopServer, onStartClient, onStopClient;
        public override void OnStartServer() => onStartServer?.Invoke();
        public override void OnStartClient() => onStartClient?.Invoke();
        public override void OnStopServer() => onStopServer?.Invoke();
        public override void OnStopClient() => onStopClient?.Invoke();

    }
}
