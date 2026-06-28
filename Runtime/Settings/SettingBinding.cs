using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    [DefaultExecutionOrder(1)]
    public abstract class SettingBinding<T> : MonoBehaviour
    {
        [SerializeField]
        private string _key;

        protected Setting<T> Setting;

        protected virtual void Awake()
        {
            BIMOSUtils.Settings.TryGetSetting(_key, out var setting);
            Setting = (Setting<T>)setting;
            SettingUpdated(Setting.Value);
        }

        protected virtual void OnEnable()
        {
            Setting.OnValueChanged += SettingUpdated;
            Setting.OnValueSaved += SettingSaved;
        }

        protected virtual void OnDisable()
        {
            Setting.OnValueChanged -= SettingUpdated;
            Setting.OnValueSaved -= SettingSaved;
        }

        protected virtual void SettingUpdated(T value) { }

        protected virtual void SettingSaved(T value) { }
    }
}
