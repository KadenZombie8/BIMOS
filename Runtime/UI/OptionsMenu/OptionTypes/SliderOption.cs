using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class SliderOption : Option<float>
    {
        private Slider _slider;

        protected override void Awake()
        {
            _slider = GetComponentInChildren<Slider>();
            base.Awake();
        }

        private void OnEnable() => _slider.onValueChanged.AddListener(Changed);

        private void OnDisable() => _slider.onValueChanged.RemoveListener(Changed);

        protected override void SetUIValue(float value) => _slider.value = value;
    }
}
