using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS.Sockets
{
    [RequireComponent(typeof(GrabbableTransfer), typeof(Socket))]
    public class PlugGrabbableTransfer : MonoBehaviour
    {
        private GrabbableTransfer _grabbableTransfer;

        private void Awake() => _grabbableTransfer = GetComponent<GrabbableTransfer>();

        public void TransferPlugGrabbables(Plug plug)
        {
            var grabbables = plug.Rigidbody.GetComponentsInChildren<Grabbable>();
            foreach (var grabbable in grabbables)
                _grabbableTransfer.TransferGrabbable(grabbable);
        }
    }
}
