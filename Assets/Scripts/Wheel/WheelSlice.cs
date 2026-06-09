using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;
using WheelOfFortune.Events;

namespace WheelOfFortune.Wheel
{
    public class WheelSlice : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _valueText;

        public SliceData Data { get; private set; }

        private float _multiplier = 1f;

        private void OnEnable() => GameEvents.OnMultiplierChanged.Subscribe(OnMultiplierChanged);
        private void OnDisable() => GameEvents.OnMultiplierChanged.Unsubscribe(OnMultiplierChanged);

        public void Setup(SliceData data, Sprite bombSprite = null)
        {
            Data = data;

            bool isBomb = data == null;

            if (_icon != null)
                _icon.sprite = isBomb ? bombSprite : data.IconSprite;

            UpdateValueText();
        }

        private void OnMultiplierChanged(float multiplier)
        {
            _multiplier = multiplier;
            UpdateValueText();
        }

        private void UpdateValueText()
        {
            if (_valueText == null) return;
            if (Data == null) { _valueText.text = string.Empty; return; }
            _valueText.text = Mathf.RoundToInt(Data.Value * _multiplier).ToString();
        }
    }
}
