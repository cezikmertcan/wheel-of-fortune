using WheelOfFortune.Economy;
using WheelOfFortune.UI;

namespace WheelOfFortune.Core
{
    public static class GameContext
    {
        public static GameState state = GameState.Idle;
        public static GameManager GameManager { get; set; }
        public static UIManager UIManager { get; set; }
        public static ICurrencyManager CurrencyManager { get; set; }
    }

    public enum GameState
    {
        Idle,
        Spinning,
        ShowingReward,
        BombHit,
        Collected,
        GameOver,
    }
}
