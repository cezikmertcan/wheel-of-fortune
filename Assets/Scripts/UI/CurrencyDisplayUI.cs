using TMPro;
using UnityEngine;
using WheelOfFortune.Core;
using WheelOfFortune.Events;

namespace WheelOfFortune.UI
{
    public class CurrencyDisplayUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;

        private void OnEnable()
        {
            GameEvents.OnCurrencyChanged.Subscribe(OnCurrencyChanged);
        }

        private void OnDisable()
        {
            GameEvents.OnCurrencyChanged.Unsubscribe(OnCurrencyChanged);
        }

        private void Start()
        {
            if (GameContext.CurrencyManager != null)
                _label.text = GameContext.CurrencyManager.Balance.ToString();
        }

        private void OnCurrencyChanged(int balance) => _label.text = balance.ToString();
    }
}
