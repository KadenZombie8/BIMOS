using System;
using KadenZombie8.BIMOS.Rig;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    public class HapticsPlayer : MonoBehaviour
    {
        [SerializeField]
        private GripHapticsHandler _gripHapticsHandler;

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
            if (!_gripHapticsHandler) return;
            _gripHapticsHandler.SendHapticImpulse(HapticSettings.Amplitude, HapticSettings.Duration);
        }
    }
}
