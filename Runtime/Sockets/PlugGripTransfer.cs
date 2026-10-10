using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Grips;
using UnityEngine;

namespace KadenZombie8.BIMOS.Sockets
{
    [RequireComponent(typeof(GripTransfer), typeof(Socket))]
    public class PlugGripTransfer : MonoBehaviour
    {
        private GripTransfer _gripTransfer;

        private void Awake() => _gripTransfer = GetComponent<GripTransfer>();

        public void TransferPlugGrips(Plug plug)
        {
            var grips = plug.Rigidbody.GetComponentsInChildren<Grip>();
            foreach (var grip in grips)
                _gripTransfer.TransferGrip(grip);
        }
    }
}
