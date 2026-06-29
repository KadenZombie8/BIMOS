using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class VR_GripType : SettingBinding<int>
    {
        [SerializeField]
        private HandInputReader _leftHandInputReader;

        [SerializeField]
        private HandInputReader _rightHandInputReader;

        protected override void SettingUpdated(int value)
        {
            var toggleGrip = value == 1;
            _leftHandInputReader.VRToggleGrip = toggleGrip;
            _rightHandInputReader.VRToggleGrip = toggleGrip;
        }
    }
}
