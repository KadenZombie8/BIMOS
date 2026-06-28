using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class VR_TurnRate : SettingBinding<float>
    {
        [SerializeField]
        private VirtualTurning _virtualTurning;

        protected override void SettingUpdated(float value) => _virtualTurning.TurnRate = value * 60f;
    }
}
