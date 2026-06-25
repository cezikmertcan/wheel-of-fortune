using UnityEngine;
using WheelOfFortune.Core;
using WheelOfFortune.Data;
using WheelOfFortune.Events;

namespace WheelOfFortune.Economy
{
    public class PlayerPrefsCurrencyService : MonoBehaviour, ICurrencyManager
    {
        private const string BalanceKey = "wof_currency";

        [SerializeField] private int _reviveCost = GameConstants.ReviveCurrencyCost;

        public int Balance => PlayerPrefs.GetInt(BalanceKey, GameConstants.StartingCurrencyBalance);
        public int ReviveCost => _reviveCost;

        private void Awake()
        {
            GameContext.CurrencyManager = this;
        }

        public bool TrySpend(int cost)
        {
            if (Balance < cost) return false;
            PlayerPrefs.SetInt(BalanceKey, Balance - cost);
            PlayerPrefs.Save();
            GameEvents.OnCurrencyChanged.Raise(Balance);
            return true;
        }

        public void Add(int amount)
        {
            PlayerPrefs.SetInt(BalanceKey, Balance + amount);
            PlayerPrefs.Save();
            GameEvents.OnCurrencyChanged.Raise(Balance);
        }
    }
}
