using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Video_FieldOfView : SettingBinding<float>
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private string _controlTypeKey;

        private Setting<int> _controlTypeSetting;

        protected override void Awake()
        {
            base.Awake();
            BIMOSUtils.Settings.TryGetSetting(_controlTypeKey, out var setting);
            _controlTypeSetting = (Setting<int>)setting;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _controlTypeSetting.OnValueSaved += ControlTypeUpdated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _controlTypeSetting.OnValueSaved -= ControlTypeUpdated;
        }

        protected override void SettingUpdated(float value) => _camera.fieldOfView = value;

        private void ControlTypeUpdated(int value)
        {
            if (value != 0)
                _camera.fieldOfView = Setting.Value;
        }
    }
}
