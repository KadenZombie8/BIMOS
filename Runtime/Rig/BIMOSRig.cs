using System;
using KadenZombie8.BIMOS.Rig.Animation;
using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [DefaultExecutionOrder(-1)]
    public class BIMOSRig : MonoBehaviour
    {
        public event Action<BIMOSRig> OnDisabled;

        public ControllerRig ControllerRig;
        public AnimationRig AnimationRig;
        public PhysicsRig PhysicsRig;
        public AvatarRig AvatarRig;

        private void OnDisable() => OnDisabled?.Invoke(this);
    }
}