using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Grips;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace KadenZombie8.BIMOS.Sockets
{
    [AddComponentMenu("BIMOS/Plug")]
    public class Plug : MonoBehaviour
    {
        [Serializable]
        public struct PlugEvents
        {
            public PlugAnimationEvents Align;
            public PlugAnimationEvents Attach;
            public PlugAnimationEvents Detach;
        }

        [Serializable]
        public struct PlugAnimationEvents
        {
            public UnityEvent<Socket> OnStart;
            public UnityEvent<Socket> OnEnd;
        }

        public PlugEvents Events;

        public string[] Tags;

        [HideInInspector]
        public Rigidbody Rigidbody;

        [HideInInspector]
        public Socket Socket;

        private void Awake() => Rigidbody = GetComponentInParent<Rigidbody>();

        public bool IsGrabbed()
        {
            foreach (Grip grab in Rigidbody.GetComponentsInChildren<Grip>())
                if (grab.LeftHand || grab.RightHand)
                    return true;

            return false;
        }

        public void AttemptDetach()
        {
            if (!Socket) return;
            Socket.Detach();
        }
    }
}
