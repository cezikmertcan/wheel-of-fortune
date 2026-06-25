using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using WheelOfFortune.Utilities;

namespace WheelOfFortune.UI
{
    public class ZoneProgressUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ZoneCardUI _cardPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private RectTransform _container;

        [Header("Zone Intervals")]
        [SerializeField] private int _safeZoneInterval = 5;
        [SerializeField] private int _superZoneInterval = 30;

        [Header("Settings")]
        [SerializeField] private int _initialCardCount = 30;
        [SerializeField] private int _refillThreshold = 10;
        [SerializeField] private int _refillBatchSize = 30;
        [SerializeField] private float _slideDuration = 0.35f;

        private List<ZoneCardUI> _cards = new();
        private int _activeIndex = 0;
        private Tween _slideTween;

        public void Initialize(int safeZoneInterval, int superZoneInterval)
        {
            _safeZoneInterval = safeZoneInterval;
            _superZoneInterval = superZoneInterval;
            _activeIndex = 0;

            foreach (Transform child in _content) DestroyImmediate(child.gameObject);
            _cards.Clear();
            _content.anchoredPosition = Vector2.zero;

            AddCards(_initialCardCount);
            _cards[0].SetCurrent(true);
            SlideToCard(0, animate: false);
        }

        private void OnEnable() => GameEvents.OnSpinCountChanged.Subscribe(OnSpinCountChanged);

        private void OnDisable()
        {
            GameEvents.OnSpinCountChanged.Unsubscribe(OnSpinCountChanged);
            _slideTween?.Kill();
        }

        private void OnSpinCountChanged(int spinCount)
        {
            int remaining = _cards.Count - spinCount;
            if (remaining < _refillThreshold)
                AddCards(_refillBatchSize);

            if (_activeIndex < _cards.Count)
                _cards[_activeIndex].SetCurrent(false);

            _activeIndex = spinCount - 1;

            if (_activeIndex < _cards.Count)
                _cards[_activeIndex].SetCurrent(true);

            SlideToCard(_activeIndex, animate: true);
        }

        private void AddCards(int count)
        {
            int startSpin = _cards.Count + 1;
            for (int spin = startSpin; spin < startSpin + count; spin++)
            {
                ZoneCardUI card = Instantiate(_cardPrefab, _content);
                card.gameObject.name = $"Zone {spin}";
                card.Setup(spin, ZoneHelper.GetZoneType(spin, _safeZoneInterval, _superZoneInterval));
                _cards.Add(card);
            }
        }

        private void SlideToCard(int index, bool animate)
        {
            Canvas.ForceUpdateCanvases();

            if (_cards.Count == 0 || index >= _cards.Count) return;

            RectTransform cardRect = (RectTransform)_cards[index].transform;
            Vector2 cardInContainer = _container.InverseTransformPoint(cardRect.position);
            float targetX = _content.anchoredPosition.x - cardInContainer.x;

            _slideTween?.Kill();

            Vector2 target = new Vector2(targetX, _content.anchoredPosition.y);

            if (animate)
                _slideTween = DOTween
                    .To(() => _content.anchoredPosition, x => _content.anchoredPosition = x, target, _slideDuration)
                    .SetEase(Ease.OutCubic);
            else
                _content.anchoredPosition = target;
        }
    }
}
