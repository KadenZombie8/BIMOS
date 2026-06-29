using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class VR_RealHeight : SettingBinding<float>
    {
        [SerializeField]
        private ControllerRig _controllerRig;

        protected override void SettingSaved(float value) => _controllerRig.UpdateRealHeight(value);
    }
}
