using KadenZombie8.BIMOS.Rig;
using UnityEngine;
using UnityEngine.Events;

namespace KadenZombie8.BIMOS.Sockets
{
    [AddComponentMenu("BIMOS/Plug")]
    public class Plug : MonoBehaviour
    {
        public UnityEvent OnAttach;
        public UnityEvent OnDetach;

        public string[] Tags;

        [HideInInspector]
        public Rigidbody Rigidbody;

        [HideInInspector]
        public Socket Socket;

        private void Awake() => Rigidbody = GetComponentInParent<Rigidbody>();

        public bool IsGrabbed()
        {
            foreach (Grabbable grab in Rigidbody.GetComponentsInChildren<Grabbable>())
                if (grab.LeftHand || grab.RightHand)
                    return true;

            return false;
        }

        public void Attach() => OnAttach?.Invoke();

        public void Detach() => OnDetach?.Invoke();

        public void AttemptDetach()
        {
            if (!Socket)
                return;

            Socket.Detach();
            OnDetach?.Invoke();
        }
    }
}
