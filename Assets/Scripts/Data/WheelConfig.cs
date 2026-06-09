using System;
using System.Collections.Generic;
using UnityEngine;

namespace WheelOfFortune.Data
{
    [Serializable]
    public class WeightedSlice
    {
        public SliceData slice;
        [Min(0.01f)] public float weight = 1f;
    }

    [CreateAssetMenu(fileName = "WheelConfig", menuName = "WheelOfFortune/Wheel Config")]
    public class WheelConfig : ScriptableObject
    {
        [SerializeField] private List<WeightedSlice> _slices;
        [SerializeField] private int _pickCount = 8;
        [SerializeField] private bool _forceBomb = false;
        [SerializeField] private Sprite _bombSprite;

        [Header("Reward Multiplier Curve")]
        [SerializeField] private AnimationCurve _rewardMultiplierCurve = AnimationCurve.Linear(0, 1, 60, 3);

        public int PickCount => _pickCount;
        public bool ForceBomb => _forceBomb;
        public Sprite BombSprite => _bombSprite;

        public float GetRewardMultiplier(int spinCount) =>
            Mathf.Max(1f, _rewardMultiplierCurve.Evaluate(spinCount));

        public List<SliceData> GetSlices()
        {
            if (_forceBomb)
            {
                List<SliceData> picks = new List<SliceData>(_pickCount) { null };
                picks.AddRange(WeightedPick(_pickCount - 1));
                Shuffle(picks);
                return picks;
            }

            List<SliceData> result = WeightedPick(_pickCount);
            Shuffle(result);
            return result;
        }

        public int WeightedPickIndex(List<SliceData> candidates)
        {
            float total = 0f;
            foreach (var s in candidates) total += GetWeight(s);

            float rand = UnityEngine.Random.value * total;
            float cumulative = 0f;
            for (int i = 0; i < candidates.Count; i++)
            {
                cumulative += GetWeight(candidates[i]);
                if (rand <= cumulative) return i;
            }
            return candidates.Count - 1;
        }

        public float GetWeight(SliceData slice)
        {
            foreach (var ws in _slices)
                if (ws.slice == slice) return ws.weight;
            return 1f;
        }

        private List<SliceData> WeightedPick(int count)
        {
            if (_slices == null || _slices.Count == 0)
                return new List<SliceData>();

            List<SliceData> result = new List<SliceData>(count);

            float totalWeight = 0f;
            foreach (var ws in _slices) totalWeight += ws.weight;

            for (int i = 0; i < count; i++)
            {
                float rand = UnityEngine.Random.value * totalWeight;
                float cumulative = 0f;
                SliceData picked = _slices[0].slice;

                foreach (var ws in _slices)
                {
                    cumulative += ws.weight;
                    if (rand <= cumulative)
                    {
                        picked = ws.slice;
                        break;
                    }
                }
                result.Add(picked);
            }

            return result;
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
