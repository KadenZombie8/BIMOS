using System;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS
{
    [RequireComponent(typeof(ScrollRect))]
    public class RulerPicker : MonoBehaviour
    {
        public event Action OnValueChanged;

        [SerializeField]
        private float _minimum = 0f;

        [SerializeField]
        private float _maximum = 1f;

        public float Value;

        private ScrollRect _scrollRect;

        private void Awake() => _scrollRect = GetComponent<ScrollRect>();

        private void OnEnable() => _scrollRect.onValueChanged.AddListener(OnScroll);

        private void OnDisable() => _scrollRect.onValueChanged.RemoveListener(OnScroll);

        private void OnScroll(Vector2 normalizedPosition)
        {
            Value = Mathf.Lerp(_minimum, _maximum, normalizedPosition.x);
            OnValueChanged?.Invoke();
            print(Value);
        }
    }
}
