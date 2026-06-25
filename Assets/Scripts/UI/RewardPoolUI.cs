using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Core;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using WheelOfFortune.Utilities;
using WheelOfFortune.Wheel;

namespace WheelOfFortune.UI
{
    public class RewardPoolUI : MonoBehaviour
    {
        [Header("Row")]
        [SerializeField] private RewardRowUI _rowPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Exit")]
        [SerializeField] private Button _exitButton;
        [SerializeField] private Transform _exitButtonContainer;

        [Header("Particles")]
        [SerializeField] private Image _particlePrefab;
        [SerializeField] private RectTransform _particleContainer;
        [SerializeField] private RectTransform _spawnPoint;

        [Header("Settings")]
        [SerializeField] private int _minParticles = 3;
        [SerializeField] private int _maxParticles = 5;
        [SerializeField] private float _spawnRadius = 30f;
        [SerializeField] private float _particleDelay = 0.08f;
        [SerializeField] private float _particleDuration = 0.55f;
        [SerializeField] private float _particleMidHorizontalRange = 60f;
        [SerializeField] private float _particleMidVerticalMin = 30f;
        [SerializeField] private float _particleMidVerticalMax = 100f;

        private readonly Dictionary<string, RewardRowUI> _rows = new();
        private readonly List<Image> _particlePool = new();
        private ZoneType _currentZoneType = ZoneType.Normal;

        private bool CanShowExit => _rows.Count > 0 && _currentZoneType != ZoneType.Normal;

        private void Awake()
        {
            SetExitVisible(false);
        }

        private void OnEnable()
        {
            _exitButton.onClick.AddListener(OnExitClicked);
            GameEvents.OnBombHit.Subscribe(OnBombHit);
            GameEvents.OnResultDisplayComplete.Subscribe(OnResultDisplayComplete);
            GameEvents.OnZoneChanged.Subscribe(OnZoneChanged);
            GameEvents.OnSpinButtonClicked.Subscribe(OnSpinStarted);
            GameEvents.OnRestart.Subscribe(OnRestart);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(OnExitClicked);
            GameEvents.OnBombHit.Unsubscribe(OnBombHit);
            GameEvents.OnResultDisplayComplete.Unsubscribe(OnResultDisplayComplete);
            GameEvents.OnZoneChanged.Unsubscribe(OnZoneChanged);
            GameEvents.OnSpinButtonClicked.Unsubscribe(OnSpinStarted);
            GameEvents.OnRestart.Unsubscribe(OnRestart);
        }

        private void OnExitClicked()
        {
            if (_rows.TryGetValue(GameConstants.GoldItemId, out var goldRow)
                && GameContext.CurrencyManager != null)
            {
                GameContext.CurrencyManager.Add(goldRow.CurrentAmount);
            }

            GameEvents.OnCollectAndExit.Raise();
        }

        private void OnBombHit() => SetExitVisible(false);
        private void OnSpinStarted() => SetExitVisible(false);
        private void OnResultDisplayComplete() => SetExitVisible(CanShowExit);

        public void ClearAllRewards()
        {
            foreach (var row in _rows.Values)
                if (row) Destroy(row.gameObject);
            _rows.Clear();
        }

        private void OnRestart()
        {
            ClearAllRewards();
            SetExitVisible(false);
            _currentZoneType = ZoneType.Normal;
        }

        private void OnZoneChanged(ZoneType zone)
        {
            _currentZoneType = zone;
            SetExitVisible(CanShowExit);
        }

        public void SetExitVisible(bool visible)
        {
            if (_exitButtonContainer != null)
                _exitButtonContainer.gameObject.SetActive(visible);
        }

        public void AddReward(SpinResult result)
        {
            if (result.IsBomb) return;

            RewardRowUI row = GetOrCreateRow(result.Slice.Id, result.Slice.IconSprite);

            Canvas.ForceUpdateCanvases();
            ScrollToRow(row);
            Canvas.ForceUpdateCanvases();

            int finalValue = RewardFormatter.ComputeValue(result.Slice.Value, result.Multiplier);
            SpawnParticles(row, result.Slice.IconSprite, finalValue);
        }

        private RewardRowUI GetOrCreateRow(string itemId, Sprite icon)
        {
            if (_rows.TryGetValue(itemId, out var existing)) return existing;

            RewardRowUI row = Instantiate(_rowPrefab, _content);
            row.Setup(itemId, icon);
            _rows[itemId] = row;
            return row;
        }

        private void ScrollToRow(RewardRowUI row)
        {
            if (_scrollRect == null) return;

            RectTransform rowRect = (RectTransform)row.transform;
            RectTransform contentRect = _scrollRect.content;
            RectTransform viewportRect = (RectTransform)_scrollRect.viewport;

            float contentHeight = contentRect.rect.height;
            float viewportHeight = viewportRect.rect.height;

            if (contentHeight <= viewportHeight) return;

            Vector2 rowInContent = contentRect.InverseTransformPoint(rowRect.position);

            float scrollable = contentHeight - viewportHeight;
            float targetY = Mathf.Clamp(-rowInContent.y - viewportHeight * 0.5f, 0f, scrollable);

            _scrollRect.verticalNormalizedPosition = 1f - targetY / scrollable;
        }

        private Image GetPooledParticle()
        {
            foreach (var p in _particlePool)
            {
                if (p && !p.gameObject.activeSelf)
                {
                    p.gameObject.SetActive(true);
                    return p;
                }
            }

            Image newParticle = Instantiate(_particlePrefab, _particleContainer);
            _particlePool.Add(newParticle);
            return newParticle;
        }

        private void ReturnToPool(Image particle)
        {
            particle.DOKill();
            particle.gameObject.SetActive(false);
        }

        private void SpawnParticles(RewardRowUI targetRow, Sprite icon, int value)
        {
            int count = Mathf.Min(Random.Range(_minParticles, _maxParticles + 1), value);
            int arrived = 0;

            Vector3 start = _spawnPoint.position;
            Vector3 end = targetRow.IconRect.position;

            for (int i = 0; i < count; i++)
            {
                float delay = i * _particleDelay;

                Image particle = GetPooledParticle();
                if (icon) particle.sprite = icon;

                Vector2 randCircle = Random.insideUnitCircle * _spawnRadius;
                particle.transform.SetParent(_particleContainer);
                particle.transform.position = start + new Vector3(randCircle.x, randCircle.y, 0f);

                Vector3 mid = Vector3.Lerp(start, end, 0.5f)
                            + new Vector3(
                                Random.Range(-_particleMidHorizontalRange, _particleMidHorizontalRange),
                                Random.Range(_particleMidVerticalMin, _particleMidVerticalMax),
                                0f);

                particle.transform
                    .DOPath(new[] { start, mid, end }, _particleDuration, PathType.CatmullRom)
                    .SetDelay(delay)
                    .SetEase(Ease.InCubic)
                    .OnComplete(() =>
                    {
                        ReturnToPool(particle);
                        arrived++;

                        if (arrived < count) return;

                        targetRow.AddAmount(value, onComplete: () =>
                        {
                            SetExitVisible(CanShowExit);
                            GameEvents.OnResultDisplayComplete.Raise();
                        });
                    });
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_exitButton == null)
            {
                var buttons = GetComponentsInChildren<Button>(includeInactive: true);
                foreach (var btn in buttons)
                    if (btn.name.ToLower().Contains("exit")) { _exitButton = btn; break; }
            }
            if (_exitButtonContainer == null && _exitButton != null)
                _exitButtonContainer = _exitButton.transform.parent;
        }
#endif
    }
}
