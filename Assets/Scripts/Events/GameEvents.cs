using WheelOfFortune.Data;
using WheelOfFortune.Wheel;

namespace WheelOfFortune.Events
{
    public static class GameEvents
    {
        public static readonly GameEvent<SpinResult> OnSpinCompleted = new();
        public static readonly GameEvent OnSpinButtonClicked = new();
        public static readonly GameEvent OnResultDisplayComplete = new();
        public static readonly GameEvent<ZoneType> OnZoneChanged = new();
        public static readonly GameEvent<int> OnSpinCountChanged = new();
        public static readonly GameEvent<float> OnMultiplierChanged = new();
        public static readonly GameEvent OnCollectAndExit = new();
        public static readonly GameEvent OnBombHit = new();
        public static readonly GameEvent OnGameOver = new();
        public static readonly GameEvent OnRestart = new();
        public static readonly GameEvent<int> OnCurrencyChanged = new();
    }
}
