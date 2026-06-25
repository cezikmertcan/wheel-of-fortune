using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using WheelOfFortune.Data;
using WheelOfFortune.Events;
using WheelOfFortune.Utilities;

namespace WheelOfFortune.Wheel
{
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private Transform _wheelTransform;
        [SerializeField] private List<WheelSlice> _slices;
        [SerializeField] private float _minSpinDuration = 3f;
        [SerializeField] private float _maxSpinDuration = 5f;

        [Header("Zone Configs")]
        [SerializeField] private WheelConfig _normalConfig;
        [SerializeField] private WheelConfig _safeConfig;
        [SerializeField] private WheelConfig _superConfig;

        [Header("Zone Intervals")]
        [SerializeField] private int _safeZoneInterval = 5;
        [SerializeField] private int _superZoneInterval = 30;

        [Header("Spin Settings")]
        [SerializeField] private int _minFullSpins = 2;
        [SerializeField] private int _maxFullSpins = 6;

        [Header("Bomb Percent Curve")]
        [SerializeField] private AnimationCurve _bombPercentCurve = AnimationCurve.Linear(0, 25, 20, 75);

        public int SafeZoneInterval => _safeZoneInterval;
        public int SuperZoneInterval => _superZoneInterval;

        private Tween _spinTween;
        private int _spinCount;
        private ZoneType _currentZoneType = ZoneType.Normal;
        private List<SliceData> _activeSlices;

        public void Initialize()
        {
            _spinCount = 1;
            _currentZoneType = ZoneType.Normal;
            SetupSlices(_currentZoneType);
            GameEvents.OnZoneChanged.Raise(_currentZoneType);
        }

        private void SetupSlices(ZoneType zone)
        {
            WheelConfig config = GetConfig(zone);
            if (config == null) return;

            _activeSlices = config.GetSlices();

            for (int i = 0; i < _slices.Count && i < _activeSlices.Count; i++)
                _slices[i].Setup(_activeSlices[i], config.BombSprite);

            GameEvents.OnMultiplierChanged.Raise(config.GetRewardMultiplier(_spinCount));
        }

        public void PrepareNextSpin()
        {
            _spinCount++;
            GameEvents.OnSpinCountChanged.Raise(_spinCount);

            ZoneType newZone = ZoneHelper.GetZoneType(_spinCount, _safeZoneInterval, _superZoneInterval);
            bool zoneChanged = newZone != _currentZoneType;
            _currentZoneType = newZone;
            SetupSlices(_currentZoneType);
            if (zoneChanged)
                GameEvents.OnZoneChanged.Raise(_currentZoneType);
        }

        public void Spin() => SpinToIndex(PickTargetSlice(excludeBombs: false));

        private void SpinToIndex(int targetIndex)
        {
            float multiplier = GetConfig(_currentZoneType).GetRewardMultiplier(_spinCount);
            SpinResult result = new SpinResult(_activeSlices[targetIndex], targetIndex, multiplier);

            float sliceAngle = 360f / _activeSlices.Count;
            float targetCenter = (360f - targetIndex * sliceAngle) % 360f;

            float currentZ = _wheelTransform.eulerAngles.z;
            float normalizedZ = ((currentZ % 360f) + 360f) % 360f;
            _wheelTransform.rotation = Quaternion.Euler(0f, 0f, normalizedZ);

            float clockwiseDelta = (normalizedZ + targetCenter) % 360f;
            if (clockwiseDelta < 0.01f) clockwiseDelta = 360f;

            float fullSpins = Random.Range(_minFullSpins, _maxFullSpins + 1) * 360f;
            float finalZ = normalizedZ - fullSpins - clockwiseDelta;
            float duration = Random.Range(_minSpinDuration, _maxSpinDuration);

            _spinTween?.Kill();
            _spinTween = _wheelTransform
                .DORotate(new Vector3(0f, 0f, finalZ), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuint)
                .OnComplete(() => GameEvents.OnSpinCompleted.Raise(result));
        }

        public float GetBombPercent(int spinCount) =>
            Mathf.Clamp(_bombPercentCurve.Evaluate(spinCount), 0f, 100f);

        private WheelConfig GetConfig(ZoneType zone) => zone switch
        {
            ZoneType.Safe => _safeConfig,
            ZoneType.Super => _superConfig,
            _ => _normalConfig,
        };

        private int PickTargetSlice(bool excludeBombs)
        {
            var bombIndices = new List<int>();
            var rewardIndices = new List<int>();
            var rewardSlices = new List<SliceData>();

            for (int i = 0; i < _activeSlices.Count; i++)
            {
                if (_activeSlices[i] == null) bombIndices.Add(i);
                else { rewardIndices.Add(i); rewardSlices.Add(_activeSlices[i]); }
            }

            if (rewardIndices.Count == 0) return 0;

            if (excludeBombs || bombIndices.Count == 0)
            {
                int picked = GetConfig(_currentZoneType).WeightedPickIndex(rewardSlices);
                return rewardIndices[picked];
            }

            float bombPct = GetBombPercent(_spinCount);
            if (Random.value < bombPct / 100f)
                return bombIndices[Random.Range(0, bombIndices.Count)];

            int rewardPicked = GetConfig(_currentZoneType).WeightedPickIndex(rewardSlices);
            return rewardIndices[rewardPicked];
        }

        private void OnDestroy()
        {
            _spinTween?.Kill();
        }
    }
}
