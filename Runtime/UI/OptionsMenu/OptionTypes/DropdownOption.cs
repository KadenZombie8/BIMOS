using TMPro;

namespace KadenZombie8.BIMOS.UI.Options
{
    public class DropdownOption : Option<int>
    {
        private TMP_Dropdown _dropdown;

        protected override void Awake()
        {
            _dropdown = GetComponentInChildren<TMP_Dropdown>();
            base.Awake();
        }

        private void OnEnable() => _dropdown.onValueChanged.AddListener(Changed);

        private void OnDisable() => _dropdown.onValueChanged.RemoveListener(Changed);

        protected override void SetUIValue(int value) => _dropdown.value = value;
    }
}
