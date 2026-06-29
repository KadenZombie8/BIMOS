using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Gamepad_Haptics : SettingBinding<float>
    {
        [SerializeField]
        private Hand _leftHand;

        [SerializeField]
        private Hand _rightHand;

        protected override void SettingUpdated(float value)
        {
            _leftHand.GamepadHaptics = value / 10f;
            _rightHand.GamepadHaptics = value / 10f;

            _leftHand.SendHapticImpulse(1f, 0.1f);
            _rightHand.SendHapticImpulse(1f, 0.1f);
        }
    }
}
