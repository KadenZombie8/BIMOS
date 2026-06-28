using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class SliderInputFieldConnection : MonoBehaviour
    {
        private Slider _slider;
        private TMP_InputField _inputField;

        private SliderOption _sliderOption;

        [SerializeField]
        private string _format;

        private void Awake()
        {
            _slider = GetComponentInChildren<Slider>();
            _inputField = GetComponentInChildren<TMP_InputField>();
            _sliderOption = GetComponentInParent<SliderOption>();
            OnSliderValueChanged(_slider.value);
        }

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
            _inputField.onEndEdit.AddListener(OnInputFieldValueChanged);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            _inputField.onEndEdit.RemoveListener(OnInputFieldValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            var settingValue = _sliderOption.ToSettingValue(value);
            _inputField.text = settingValue.ToString(_format);
        }

        private void OnInputFieldValueChanged(string stringValue)
        {
            if (!float.TryParse(stringValue, out float value)) value = _slider.value;
            value = Mathf.Clamp(value, _slider.minValue, _slider.maxValue);
            var sliderValue = _sliderOption.ToSliderValue(value);
            _slider.value = sliderValue;
        }
    }
}
