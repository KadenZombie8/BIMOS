using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class SliderInputFieldConnection : MonoBehaviour
    {
        private Slider _slider;
        private TMP_InputField _inputField;

        [SerializeField]
        private string _format;

        [SerializeField]
        private float _multiplier = 1f;

        private void Awake()
        {
            _slider = GetComponentInChildren<Slider>();
            _inputField = GetComponentInChildren<TMP_InputField>();
            UpdateInputFieldText(_slider.value);
        }

        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(UpdateInputFieldText);
            _inputField.onEndEdit.AddListener(UpdateSliderValue);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(UpdateInputFieldText);
            _inputField.onEndEdit.RemoveListener(UpdateSliderValue);
        }

        private void UpdateInputFieldText(float sliderValue)
        {
            sliderValue *= _multiplier;
            _inputField.text = sliderValue.ToString(_format);
        }

        private void UpdateSliderValue(string stringValue)
        {
            float sliderValue;
            if (float.TryParse(stringValue, out float inputValue))
                sliderValue = inputValue * _multiplier;
            else
                sliderValue = _slider.value;

            sliderValue = Mathf.Clamp(sliderValue, _slider.minValue, _slider.maxValue);
            _slider.value = sliderValue;
            UpdateInputFieldText(_slider.value);
        }
    }
}
