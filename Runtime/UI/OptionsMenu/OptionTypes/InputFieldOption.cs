using TMPro;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class InputFieldOption : Option<string>
    {
        private TMP_InputField _inputField;

        protected override void Awake()
        {
            _inputField = GetComponentInChildren<TMP_InputField>();
            base.Awake();
        }

        private void OnEnable() => _inputField.onValueChanged.AddListener(Changed);

        private void OnDisable() => _inputField.onValueChanged.RemoveListener(Changed);

        protected override void SetUIValue(string value) => _inputField.text = value;
    }
}
