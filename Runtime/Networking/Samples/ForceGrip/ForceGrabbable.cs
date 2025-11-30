using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    [RequireComponent(typeof(Grabbable))]
    public class ForceGrabbable : MonoBehaviour
    {
        public Grabbable Grabbable {
            get; private set;
        }
        public float speed = 10;

        private void Start() {
            Grabbable = GetComponent<Grabbable>();
        }
    }
}
