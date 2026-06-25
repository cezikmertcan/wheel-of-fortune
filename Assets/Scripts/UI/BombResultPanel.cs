using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Core;
using WheelOfFortune.Events;

namespace WheelOfFortune.UI
{
    public class BombResultPanel : MonoBehaviour
    {
        [SerializeField] private Button _reviveButton;
        [SerializeField] private Button _giveUpButton;

        private void OnEnable()
        {
            _reviveButton.onClick.AddListener(OnRevive);
            _giveUpButton.onClick.AddListener(OnGiveUp);
        }

        private void OnDisable()
        {
            _reviveButton.onClick.RemoveListener(OnRevive);
            _giveUpButton.onClick.RemoveListener(OnGiveUp);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _reviveButton.interactable = GameContext.CurrencyManager != null
                && GameContext.CurrencyManager.Balance >= GameContext.CurrencyManager.ReviveCost;
        }

        private void OnRevive()
        {
            if (GameContext.CurrencyManager == null
                || !GameContext.CurrencyManager.TrySpend(GameContext.CurrencyManager.ReviveCost))
                return;

            gameObject.SetActive(false);
            GameEvents.OnResultDisplayComplete.Raise();
        }

        private void OnGiveUp()
        {
            gameObject.SetActive(false);
            GameEvents.OnGameOver.Raise();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var buttons = GetComponentsInChildren<Button>(includeInactive: true);
            foreach (var btn in buttons)
            {
                string n = btn.name.ToLower();
                if (_reviveButton == null && n.Contains("revive")) _reviveButton = btn;
                if (_giveUpButton == null && (n.Contains("giveup") || n.Contains("give_up") || n.Contains("give"))) _giveUpButton = btn;
            }
        }
#endif
    }
}
