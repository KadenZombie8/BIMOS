using UnityEngine;

namespace KadenZombie8.BIMOS.UI.Options
{
    [RequireComponent(typeof(ApplyOptions))]
    public class ApplyButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject _button;

        private ApplyOptions _applyOptions;

        private void Awake() => _applyOptions = GetComponent<ApplyOptions>();

        private void OnEnable() => _applyOptions.OnOptionsChanged += UpdateButtonState;

        private void OnDisable() => _applyOptions.OnOptionsChanged -= UpdateButtonState;

        private void UpdateButtonState() => _button.SetActive(_applyOptions.HasUnsavedChanges);
    }
}
