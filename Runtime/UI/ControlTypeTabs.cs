using System;
using System.Collections.Generic;
using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI
{
    /// <summary>
    /// Enables and disables tabs depending on if flatscreen mode is active.
    /// </summary>
    public class ControlTypeTabs : MonoBehaviour
    {
        [SerializeField]
        private Tabs _flatscreenTabs;

        [SerializeField]
        private Setting<int> _setting;

        [SerializeField]
        private Toggle _initialSpectatorToggle;

        [SerializeField]
        private Toggle _initialFlatscreenToggle;

        private void Awake()
        {
            BIMOSUtils.Settings.TryGetSetting("Debug_ControlType", out var setting);
            _setting = (Setting<int>)setting;
            bool isFlatscreen = _setting.Value == 1;
            UpdateTabs(isFlatscreen);
            if (isFlatscreen)
                _initialFlatscreenToggle.Select();
            else
                _initialSpectatorToggle.Select();
        }

        private void OnEnable() => _setting.OnValueChanged += OnValueChanged;

        private void OnDisable() => _setting.OnValueChanged -= OnValueChanged;

        private void OnValueChanged(int value)
        {
            bool isFlatscreen = value == 1;
            UpdateTabs(isFlatscreen);
        }

        public void UpdateTabs(bool isFlatscreen)
        {
            foreach (var tab in _flatscreenTabs.Enable)
                tab.SetActive(isFlatscreen);
            foreach (var tab in _flatscreenTabs.Disable)
                tab.SetActive(!isFlatscreen);
        }
    }

    [Serializable]
    public struct Tabs
    {
        public List<GameObject> Enable;
        public List<GameObject> Disable;
    }
}
