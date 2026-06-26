using System.Collections;
using UnityEngine.XR.Management;

namespace KadenZombie8.BIMOS.Settings.Bindings
{
    public class Debug_ControlType : SettingBinding<int>
    {
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
            var manager = XRGeneralSettings.Instance.Manager;
            if (manager.activeLoader) yield break;
            yield return manager.InitializeLoader();
            if (!manager.activeLoader) yield break;
            manager.StartSubsystems();
        }

        private void StopXR()
        {
            var manager = XRGeneralSettings.Instance.Manager;
            if (!manager.activeLoader) return;
            manager.StopSubsystems();
            manager.DeinitializeLoader();
        }

        private void OnDestroy() => StopXR();
    }
}
