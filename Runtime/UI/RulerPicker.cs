using System;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI
{
    public class RulerPicker : MonoBehaviour
    {
        public event Action<float> OnValueChanged;

        [SerializeField]
        private RectTransform _viewport;

        [SerializeField]
        private RectTransform _ruler;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private float _minValue = 0f;

        [SerializeField]
        private float _maxValue = 1f;

        [SerializeField]
        private float _increment = 10f;

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

        private void Start() => UpdateSize();

        private void UpdateSize()
        {
            Canvas.ForceUpdateCanvases();
            var viewportSize = _viewport.rect.size;

            var range = _maxValue - _minValue;
            var markCount = range / _increment + 4f;
            _ruler.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, viewportSize.y * markCount);

            var horizontalScale = viewportSize.x / viewportSize.y / 4f;
            _ruler.localScale = new(horizontalScale, 0.6f, 1f);
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
