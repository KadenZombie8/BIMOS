using UnityEngine;

namespace KadenZombie8.BIMOS.UI.Options
{
    [DefaultExecutionOrder(1)]
    public class RevertToDefaultButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject _button;

        private IRevertible _option;

        private void Awake() => _option = GetComponentInChildren<IRevertible>();

        private void OnEnable()
        {
            UpdateButtonState();
            _option.OnValueChanged += UpdateButtonState;
        }

        private void OnDisable() => _option.OnValueChanged -= UpdateButtonState;

        private void UpdateButtonState() => _button.SetActive(!_option.IsDefaultValue);
    }
}
