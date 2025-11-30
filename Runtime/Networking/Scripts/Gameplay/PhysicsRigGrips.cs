using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;

namespace KadenZombie8.BIMOS.Networking
{
    [DisallowMultipleComponent]
    public class PhysicsRigGrips : MonoBehaviour
    {
        public PhysicsRig physicsRig;
        private void Start() {
            physicsRig.Colliders.Body.AddComponent<AutoGrabbable>();
            physicsRig.Colliders.Head.AddComponent<AutoGrabbable>();
            physicsRig.Colliders.LeftHand.AddComponent<AutoGrabbable>();
            physicsRig.Colliders.RightHand.AddComponent<AutoGrabbable>();
            physicsRig.Colliders.LocomotionSphere.AddComponent<AutoGrabbable>();
        }
    }
}
