using System;
using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    public class HapticsPlayer : MonoBehaviour
    {
        [SerializeField]
        private GrabbableHapticsHandler _grabbableHapticsHandler;

        [Serializable]
        protected struct HapticSettingsStruct
        {
            public float Amplitude;
            public float Duration;
        }

        [SerializeField]
        protected HapticSettingsStruct HapticSettings = new()
        {
            Amplitude = 0.5f,
            Duration = 0.1f
        };

        protected void Play()
        {
            if (!_grabbableHapticsHandler) return;
            _grabbableHapticsHandler.SendHapticImpulse(HapticSettings.Amplitude, HapticSettings.Duration);
        }
    }
}
