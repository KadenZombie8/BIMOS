using System;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI
{
    public class RulerPicker : MonoBehaviour
    {
        public event Action<float> OnValueChanged;

        [SerializeField]
        private RectTransform _ruler;

        [SerializeField]
        private float _minValue = 0f;

        [SerializeField]
        private float _maxValue = 1f;

        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);
                _scrollRect.horizontalNormalizedPosition = Mathf.InverseLerp(_minValue, _maxValue, _value);
            }
        }

        private float _value;

        [SerializeField]
        private ScrollRect _scrollRect;

        private void Awake()
        {
            //_ruler.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
        }

        private void OnEnable() => _scrollRect.onValueChanged.AddListener(OnScroll);

        private void OnDisable() => _scrollRect.onValueChanged.RemoveListener(OnScroll);

        private void OnScroll(Vector2 normalizedPosition)
        {
            _value = Mathf.Lerp(_minValue, _maxValue, normalizedPosition.x);
            OnValueChanged?.Invoke(_value);
        }
    }
}
