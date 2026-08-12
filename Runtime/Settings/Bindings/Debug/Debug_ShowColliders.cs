using KadenZombie8.BIMOS.Rig.Movement;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Debug_ShowColliders : SettingBinding<bool>
    {
        [SerializeField]
        private DebugShapes _debugShapes;

        protected override void SettingUpdated(bool value) => _debugShapes.enabled = value;
    }
}
