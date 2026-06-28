using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class SliderOption : Option<float>
    {
        private Slider _slider;

        [SerializeField]
        private float _multiplier = 1f;

        protected override void Awake()
        {
            _slider = GetComponentInChildren<Slider>();
            base.Awake();
        }

        private void OnEnable() => _slider.onValueChanged.AddListener(Changed);

        private void OnDisable() => _slider.onValueChanged.RemoveListener(Changed);

        public float ToSettingValue(float value) => value * _multiplier;

        public float ToSliderValue(float value) => value / _multiplier;

        protected override void SetUIValue(float value) => _slider.value = ToSliderValue(value);

        protected override void Changed(float value) => base.Changed(ToSettingValue(value));
    }
}
