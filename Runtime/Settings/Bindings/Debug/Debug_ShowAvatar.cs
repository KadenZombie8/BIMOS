using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Debug_ShowAvatar : SettingBinding<bool>
    {
        [SerializeField]
        private Transform _character;

        protected override void SettingUpdated(bool value)
        {
            foreach (var renderer in _character.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = value;
            }
        }
    }
}
