using KadenZombie8.BIMOS.Rig.Movement;
using KadenZombie8.BIMOS.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Debug_ControlType : SettingBinding<int>
    {
        [SerializeField]
        private MenuToggleVR _menuToggleVR;

        [SerializeField]
        private MenuToggleFlatscreen _menuToggleFlatscreen;

        [SerializeField]
        private ScreenModeCamera _screenModeCamera;

        private void Start()
        {
            if (Setting.Value == 0)
                StartCoroutine(StartXR());
        }

        protected override void SettingSaved(int value)
        {
            if (value == 0)
                StartCoroutine(StartXR());
            else
                StopXR();
        }

        private IEnumerator StartXR()
        {
            _menuToggleFlatscreen.SetMenuOpen(false);
            _screenModeCamera.enabled = false;
            var manager = XRGeneralSettings.Instance.Manager;
            if (manager.activeLoader) yield break;
            yield return manager.InitializeLoader();
            if (!manager.activeLoader) yield break;
            manager.StartSubsystems();
        }

        private void StopXR()
        {
            _menuToggleVR.SetMenuOpen(false);
            _screenModeCamera.enabled = true;
            var manager = XRGeneralSettings.Instance.Manager;
            if (!manager.activeLoader) return;
            manager.StopSubsystems();
            manager.DeinitializeLoader();
        }

        private void OnDestroy() => StopXR();
    }
}
