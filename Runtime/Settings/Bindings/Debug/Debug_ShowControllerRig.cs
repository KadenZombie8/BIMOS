using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Debug_ShowControllerRig : SettingBinding<bool>
    {
        [SerializeField]
        private GameObject _leftController;

        [SerializeField]
        private GameObject _rightController;

        protected override void SettingUpdated(bool value)
        {
            _leftController.SetActive(value);
            _rightController.SetActive(value);
        }
    }
}
