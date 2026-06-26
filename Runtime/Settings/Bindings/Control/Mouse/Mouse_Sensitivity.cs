using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Mouse_Sensitivity : SettingBinding<float>
    {
        [SerializeField]
        private ScreenModeCamera _camera;

        protected override void SettingUpdated(float value) => _camera.MouseSensitivity = value;
    }
}
