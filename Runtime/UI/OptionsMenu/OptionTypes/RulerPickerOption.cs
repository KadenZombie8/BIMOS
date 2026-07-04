using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class RulerPickerOption : Option<float>
    {
        private RulerPicker _rulerPicker;

        protected override void Awake()
        {
            _rulerPicker = GetComponentInChildren<RulerPicker>();
            base.Awake();
        }

        private void OnEnable() => _rulerPicker.OnValueChanged += Changed;

        private void OnDisable() => _rulerPicker.OnValueChanged -= Changed;

        protected override void SetUIValue(float value) => _rulerPicker.Value = value;
    }
}
