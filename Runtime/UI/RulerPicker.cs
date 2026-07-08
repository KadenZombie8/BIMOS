using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KadenZombie8.BIMOS.UI
{
    [DefaultExecutionOrder(-1)]
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

        [SerializeField]
        private TMP_Text[] _labels;

        public float Value
        {
            get => _value;
            set
            {
                var normalizedPosition = new Vector2(Mathf.InverseLerp(_minValue, _maxValue, value), 0f);
                _scrollRect.normalizedPosition = normalizedPosition;
                OnScroll(normalizedPosition);
                _value = Mathf.Clamp(value, _minValue, _maxValue);
            }
        }

        private float _value;
        private float _truncatedValue;
        private Vector2 _viewportSize;

        private void Start()
        {
            Canvas.ForceUpdateCanvases();
            _viewportSize = _viewport.rect.size;
            UpdateSize();
        }

        private void UpdateSize()
        {

            var range = _maxValue - _minValue;
            var markCount = range / _increment + 4f;
            _ruler.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _viewportSize.y * markCount);

            var horizontalScale = _viewportSize.x / _viewportSize.y / 4f;
            _ruler.localScale = new(horizontalScale, 0.6f, 1f);
        }

        private void OnEnable() => _scrollRect.onValueChanged.AddListener(OnScroll);

        private void OnDisable() => _scrollRect.onValueChanged.RemoveListener(OnScroll);

        private void OnScroll(Vector2 normalizedPosition)
        {
            var value = Mathf.Lerp(_minValue, _maxValue, normalizedPosition.x);
            if (Mathf.Approximately(value, _value)) return;
            _value = value;
            UpdateLabels();
            OnValueChanged?.Invoke(_value);
        }

        private void UpdateLabels()
        {
            _truncatedValue = Mathf.Floor(_value / _increment) * _increment;

            for (int i = 0; i < _labels.Length; i++)
            {
                _labels[i].text = GetLabelText(2 - i);
                _labels[i].rectTransform.anchoredPosition = new(GetLabelPosition(i), 0f);
            }
        }

        private string GetLabelText(float index) => (_truncatedValue - index * _increment).ToString();

        private float GetLabelPosition(float index) => (_truncatedValue - _value + index * _increment) / _increment * _viewportSize.x / 4f;
    }
}
