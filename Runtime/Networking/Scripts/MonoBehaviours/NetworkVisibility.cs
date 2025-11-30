using System;
using System.Linq;
using UnityEngine;

namespace Mirror {
    [DisallowMultipleComponent]
    public class NetworkVisibility : NetworkBehaviour {
        public GameObjectRef[] targets;
        public bool IsVisible {
            get; private set;
        }
        public override void OnStartAuthority() {
            base.OnStartAuthority();
            UpdateVisibility(true);
        }
        public override void OnStopAuthority() {
            base.OnStopAuthority();
            UpdateVisibility(false);
        }

        public override void OnStartClient() {
            base.OnStartClient();
            UpdateVisibility(isOwned);
        }

        public void UpdateVisibility(bool visible) {
            IsVisible = visible;
            targets?.ToList().ForEach(x => x.gameObject.SetActive(visible ^ x.inverted));
        }

        [Serializable]
        public struct GameObjectRef {
            public GameObject gameObject;
            public bool inverted;
        }
    }
}