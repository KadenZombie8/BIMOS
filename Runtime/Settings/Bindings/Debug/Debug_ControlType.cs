using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Rig.Movement;
using KadenZombie8.BIMOS.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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

        [SerializeField]
        private ControllerRig _controllerRig;

        [SerializeField]
        private string _realHeightKey = "VR_RealHeight";

        private void Start()
        {
            if (Setting.Value == 0)
                StartCoroutine(StartXR());
            else
                _controllerRig.UpdateRealHeight(180f);
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

            BIMOSUtils.Settings.TryGetSetting(_realHeightKey, out var setting);
            var heightSetting = (Setting<float>)setting;
            var height = heightSetting.Load();
            _controllerRig.UpdateRealHeight(height);

            var manager = XRGeneralSettings.Instance.Manager;
            if (manager.activeLoader) yield break;
            yield return manager.InitializeLoader();
            if (!manager.activeLoader) yield break;
            manager.StartSubsystems();
        }

        private void StopXR()
        {
            _controllerRig.UpdateRealHeight(180f);
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
