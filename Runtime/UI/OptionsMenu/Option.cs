using System;
using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Settings;
using UnityEngine;

namespace KadenZombie8.BIMOS.UI.Options
{
    public abstract class Option<T> : MonoBehaviour, IAppliable, IRevertible
    {
        private Setting<T> _setting;

        public event Action OnValueChanged;

        [SerializeField]
        protected string Key;
        
        private ApplyOptions _applyOptions;

        public bool IsSavedValue => _setting.IsSavedValue;

        public bool IsDefaultValue => _setting.IsDefaultValue;

        protected virtual void Awake()
        {
            BIMOSUtils.Settings.TryGetSetting(Key, out var setting);
            _setting = (Setting<T>)setting;
            _applyOptions = GetComponentInParent<ApplyOptions>();
            UpdateOptionValue();
            _setting.OnValueChanged += SettingValueChanged;
        }

        private void OnDestroy() => _setting.OnValueChanged -= SettingValueChanged;

        protected virtual void Changed(T value) => _setting.Value = value;

        private void SettingValueChanged(T _) => SettingUpdated();

        protected virtual void SettingUpdated()
        {
            OnValueChanged?.Invoke();
            SetUIValue(_setting.Value);
            RegisterApply();
        }

        public void Apply()
        {
            _setting.Save();
            Changed(_setting.Value);
        }

        public void Discard()
        {
            _setting.Discard();
            UpdateOptionValue();
        }

        public void Revert()
        {
            _setting.Revert();
            UpdateOptionValue();
        }

        private void UpdateOptionValue()
        {
            SetUIValue(_setting.Value);
            Changed(_setting.Value);
        }

        private void RegisterApply()
        {
            if (_setting.IsSavedValue)
                _applyOptions.UnregisterOption(this);
            else
                _applyOptions.RegisterOption(this);
        }

        protected abstract void SetUIValue(T value);
    }
}
