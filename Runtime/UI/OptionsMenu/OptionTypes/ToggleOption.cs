using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class ToggleOption : Option<bool>
    {
        private Toggle _toggle;

        protected override void Awake()
        {
            _toggle = GetComponentInChildren<Toggle>();
            base.Awake();
        }

        private void OnEnable() => _toggle.onValueChanged.AddListener(Changed);

        private void OnDisable() => _toggle.onValueChanged.RemoveListener(Changed);

        protected override void SetUIValue(bool value) => _toggle.isOn = value;
    }
}
