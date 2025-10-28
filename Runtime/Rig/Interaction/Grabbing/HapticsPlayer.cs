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
        public struct HapticSettingsStruct
        {
            public float Amplitude;
            public float Duration;
        }

        public HapticSettingsStruct HapticSettings = new()
        {
            Amplitude = 0.5f,
            Duration = 0.1f
        };

        public void Play()
        {
            if (!_grabbableHapticsHandler) return;
            _grabbableHapticsHandler.SendHapticImpulse(HapticSettings.Amplitude, HapticSettings.Duration);
        }
    }
}
