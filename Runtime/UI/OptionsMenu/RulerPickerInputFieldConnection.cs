using TMPro;
using UnityEngine;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class RulerPickerInputFieldConnection : MonoBehaviour
    {
        private RulerPicker _rulerPicker;
        private TMP_InputField _inputField;

        [SerializeField]
        private string _format;

        [SerializeField]
        private float _multiplier = 1f;

        private void Awake()
        {
            _rulerPicker = GetComponentInChildren<RulerPicker>();
            _inputField = GetComponentInChildren<TMP_InputField>();
            UpdateInputFieldText(_rulerPicker.Value);
        }

        private void OnEnable()
        {
            _rulerPicker.OnValueChanged += UpdateInputFieldText;
            _inputField.onEndEdit.AddListener(UpdateRulerPickerValue);
        }

        private void OnDisable()
        {
            _rulerPicker.OnValueChanged -= UpdateInputFieldText;
            _inputField.onEndEdit.RemoveListener(UpdateRulerPickerValue);
        }

        private void UpdateInputFieldText(float rulerPickerValue)
        {
            rulerPickerValue *= _multiplier;
            _inputField.text = rulerPickerValue.ToString(_format);
        }

        private void UpdateRulerPickerValue(string stringValue)
        {
            float rulerPickerValue;
            if (float.TryParse(stringValue, out float inputValue))
                rulerPickerValue = inputValue * _multiplier;
            else
                rulerPickerValue = _rulerPicker.Value;

            _rulerPicker.Value = rulerPickerValue;
            UpdateInputFieldText(_rulerPicker.Value);
        }
    }
}
