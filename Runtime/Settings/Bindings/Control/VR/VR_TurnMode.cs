using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class VR_TurnMode : SettingBinding<int>
    {
        [SerializeField]
        private VirtualTurning _virtualTurning;

        protected override void SettingUpdated(int value)
        {
            _virtualTurning.TurningMode = (VirtualTurning.VirtualTurningMode)value;
        }
    }
}
