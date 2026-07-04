using System;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class RulerPicker : MonoBehaviour
    {
        public event Action<float> OnValueChanged;

        [SerializeField]
        public float MinValue = 0f;

        [SerializeField]
        public float MaxValue = 1f;

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
            }
        }

        private float _value;

        private ScrollRect _scrollRect;

        private void Awake() => _scrollRect = GetComponent<ScrollRect>();

        private void OnEnable() => _scrollRect.onValueChanged.AddListener(OnScroll);

        private void OnDisable() => _scrollRect.onValueChanged.RemoveListener(OnScroll);

        private void OnScroll(Vector2 normalizedPosition)
        {
            _value = Mathf.Lerp(MinValue, MaxValue, normalizedPosition.x);
            OnValueChanged?.Invoke(_value);
            print(_value);
        }
    }
}
