using TMPro;
using UnityEngine;

namespace KadenZombie8.BIMOS.UI.Options
{
    [RequireComponent(typeof(TMP_Text))]
    public class UnappliedLabel : MonoBehaviour
    {
        private TMP_Text _text;
        private IAppliable _option;
        private string _originalText;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _originalText = _text.text;
            _option = GetComponentInParent<IAppliable>();
        }

        private void OnEnable()
        {
            UpdateLabel();
            _option.OnValueChanged += UpdateLabel;
        }

        private void OnDisable() => _option.OnValueChanged -= UpdateLabel;

        private void UpdateLabel() => _text.text = _option.IsSavedValue ? _originalText : $"{_originalText}<b>*</b>";
    }
}
