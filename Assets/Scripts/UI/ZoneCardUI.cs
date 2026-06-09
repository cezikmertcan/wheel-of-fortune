using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;

namespace WheelOfFortune.UI
{
    public class ZoneCardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private Image _background;

        [Header("Colors")]
        [SerializeField] private Color _normalColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        [SerializeField] private Color _normalCurrentColor = new Color(0.20f, 0.20f, 0.20f, 1f);
        [SerializeField] private Color _safeColor = new Color(0.30f, 0.85f, 0.40f, 1f);
        [SerializeField] private Color _safeCurrentColor = new Color(0.10f, 0.55f, 0.20f, 1f);
        [SerializeField] private Color _superColor = new Color(0.95f, 0.55f, 0.15f, 1f);
        [SerializeField] private Color _superCurrentColor = new Color(0.75f, 0.30f, 0.05f, 1f);

        [Header("Scale Animation")]
        [SerializeField] private float _scaleUpValue = 1.2f;
        [SerializeField] private float _scaleDuration = 0.2f;

        [Header("Background Sprites")]
        [SerializeField] private Sprite _normalBG;
        [SerializeField] private Sprite _safeBG;
        [SerializeField] private Sprite _superBG;

        private ZoneType _zoneType;

        public void Setup(int spinNumber, ZoneType zoneType)
        {
            _zoneType = zoneType;
            _numberText.text = spinNumber.ToString();
            _numberText.color = ZoneColor(zoneType);

            if (_background != null)
            {
                _background.sprite = ZoneBGSprite(zoneType);
                _background.enabled = false;
            }
        }

        public void SetCurrent(bool isCurrent)
        {
            _numberText.color = isCurrent ? ZoneCurrentColor(_zoneType) : ZoneColor(_zoneType);

            if (_background != null)
                _background.enabled = isCurrent;

            transform.DOKill();
            float targetScale = isCurrent ? _scaleUpValue : 1f;
            transform.DOScale(targetScale, _scaleDuration).SetEase(Ease.OutBack);
        }

        private Color ZoneColor(ZoneType zone) => zone switch
        {
            ZoneType.Safe => _safeColor,
            ZoneType.Super => _superColor,
            _ => _normalColor,
        };

        private Color ZoneCurrentColor(ZoneType zone) => zone switch
        {
            ZoneType.Safe => _safeCurrentColor,
            ZoneType.Super => _superCurrentColor,
            _ => _normalCurrentColor,
        };

        private Sprite ZoneBGSprite(ZoneType zone) => zone switch
        {
            ZoneType.Safe => _safeBG,
            ZoneType.Super => _superBG,
            _ => _normalBG,
        };
    }
}
