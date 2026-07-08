using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Spectator_Output : SettingBinding<int>
    {
        [SerializeField]
        private GameObject _spectatorCamera;

        protected override void SettingUpdated(int value) => _spectatorCamera.SetActive(value == 1);
    }
}
