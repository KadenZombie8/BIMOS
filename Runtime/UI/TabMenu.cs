using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI
{
    public class TabMenu : MonoBehaviour
    {
        private Tab _currentTab;

        private readonly Dictionary<Toggle, UnityAction<bool>> _listeners = new();

        [SerializeField]
        private Transform _pageContainer;

        private void Awake()
        {
            foreach (Transform page in _pageContainer)
                page.gameObject.SetActive(false);
        }

        public void Register(Tab tab, Toggle toggle)
        {
            void Listener(bool isSelected)
            {
                if (isSelected) SetActiveTab(tab);
            }

            _listeners[toggle] = Listener;
            toggle.onValueChanged.AddListener(Listener);
        }

        public void Unregister(Toggle toggle)
        {
            if (_listeners.TryGetValue(toggle, out var listener))
            {
                toggle.onValueChanged.RemoveListener(listener);
                _listeners.Remove(toggle);
            }
        }

        private void SetActiveTab(Tab tab)
        {
            if (_currentTab)
                _currentTab.Page.SetActive(false);

            tab.Page.SetActive(true);
            _currentTab = tab;
        }
    }
}
