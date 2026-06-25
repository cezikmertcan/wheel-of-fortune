using UnityEngine;

namespace WheelOfFortune.Utilities
{
    public static class RewardFormatter
    {
        public static int ComputeValue(int baseValue, float multiplier) =>
            Mathf.RoundToInt(baseValue * multiplier);

        public static string FormatMultiplier(float multiplier, string prefix = "x") =>
            multiplier % 1f == 0f ? $"{prefix}{multiplier:F0}" : $"{prefix}{multiplier:F1}";
    }
}
