using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS.Settings
{
    public class BIMOSSettings
    {
        private readonly Dictionary<string, ISetting> _settings = new();

        #region Settings
        // VR
        public Setting<float> VRRealHeight = new("VR_RealHeight", 180f);
        public Setting<int> VRLocomotionDirection = new("VR_LocomotionDirection", 0);
        public Setting<int> VRLocomotionJoystick = new("VR_LocomotionJoystick", 0);
        public Setting<int> VRTurnMode = new("VR_TurnMode", 2);
        public Setting<float> VRTurnRate = new("VR_TurnRate", 4f);
        public Setting<int> VRGripType = new("VR_GripType", 0);
        public Setting<float> VRHaptics = new("VR_Haptics", 10f);

        // Mouse
        public Setting<float> MouseSensitivity = new("Mouse_Sensitivity", 5f);
        public Setting<int> MouseGripType = new("Mouse_GripType", 0);

        // Gamepad
        public Setting<float> GamepadSensitivity = new("Gamepad_Sensitivity", 5f);
        public Setting<int> GamepadGripType = new("Gamepad_GripType", 1);
        public Setting<float> GamepadHaptics = new("Gamepad_Haptics", 10f);

        // Audio
        public Setting<float> AudioMixingGlobal = new("Audio_Mixing_Global", 8f);
        public Setting<float> AudioMixingMusic = new("Audio_Mixing_Music", 8f);
        public Setting<float> AudioMixingSFX = new("Audio_Mixing_SFX", 8f);

        // Video
        public Setting<float> FlatscreenFieldOfView = new("Video_FieldOfView", 60f);
        public Setting<int> Quality = new("Video_Quality", QualitySettings.count / 2);

        // Spectator
        public Setting<int> SpectatorOutput = new("Spectator_Output", 0);
        public Setting<int> SpectatorEye = new("Spectator_Eye", 0);
        public Setting<int> SpectatorCameraVisual = new("Spectator_CameraVisual", 0);
        public Setting<float> SpectatorFieldOfView = new("Spectator_FieldOfView", 90f);
        public Setting<float> SpectatorSmoothing = new("Spectator_Smoothing", 90f);

        // Debug
        public Setting<int> ControlType = new("Debug_ControlType", 0);
        public Setting<bool> ShowColliders = new("Debug_ShowColliders", false);
        #endregion

        public BIMOSSettings()
        {
            foreach (var field in GetType().GetFields())
            {
                if (field.GetValue(this) is ISetting setting)
                _settings.Add(setting.Key, setting);
            }
        }

        public bool TryGetSetting(string key, out ISetting setting) => _settings.TryGetValue(key, out setting);
    }
}
