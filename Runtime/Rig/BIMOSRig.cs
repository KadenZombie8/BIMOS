using KadenZombie8.BIMOS.Rig.Movement;
using System;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [DefaultExecutionOrder(-1)]
    public class BIMOSRig : MonoBehaviour
    {
        public bool IsInitialized => ControllerRig != null && PhysicsRig != null && AnimationRig != null;
        public ControllerRig ControllerRig;
        public PhysicsRig PhysicsRig;
        public AnimationRig AnimationRig;
    }
}