using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Core;
using WheelOfFortune.Events;
using WheelOfFortune.Wheel;

namespace WheelOfFortune.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private RewardPoolUI _rewardPool;
        [SerializeField] private BombResultPanel _bombResultPanel;
        [SerializeField] private GameObject _collectResultPanel;
        [SerializeField] private GameObject _gameOverPanel;

        [Header("Settings")]
        [SerializeField] private float _spinButtonEnableDelay = 0.5f;

        private void Awake()
        {
            GameContext.UIManager = this;
            _spinButton.onClick.AddListener(OnSpinButtonClicked);
            _restartButton.onClick.AddListener(OnRestartClicked);
            _restartButton.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnResultDisplayComplete.Subscribe(OnResultDisplayComplete);
            GameEvents.OnCollectAndExit.Subscribe(OnCollectAndExit);
            GameEvents.OnGameOver.Subscribe(OnGameOver);
        }

        private void OnDisable()
        {
            GameEvents.OnResultDisplayComplete.Unsubscribe(OnResultDisplayComplete);
            GameEvents.OnCollectAndExit.Unsubscribe(OnCollectAndExit);
            GameEvents.OnGameOver.Unsubscribe(OnGameOver);
        }

        private void OnDestroy()
        {
            _spinButton.onClick.RemoveListener(OnSpinButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        private void OnResultDisplayComplete() =>
            DOVirtual.DelayedCall(_spinButtonEnableDelay, () => _spinButton.interactable = true);

        private void OnCollectAndExit() => _restartButton.gameObject.SetActive(true);

        private void OnGameOver()
        {
            _rewardPool.ClearAllRewards();
            _restartButton.gameObject.SetActive(true);
            if (_gameOverPanel) _gameOverPanel.SetActive(true);
        }

        public void ShowResult(SpinResult result)
        {
            if (result.IsBomb)
            {
                GameEvents.OnBombHit.Raise();
                _bombResultPanel.Show();
                return;
            }

            _rewardPool.AddReward(result);
        }

        public void ShowCollectResult()
        {
            if (_collectResultPanel) _collectResultPanel.SetActive(true);
            _rewardPool.SetExitVisible(false);
        }

        private void OnSpinButtonClicked()
        {
            _spinButton.interactable = false;
            GameEvents.OnSpinButtonClicked.Raise();
        }

        private void OnRestartClicked()
        {
            _spinButton.interactable = true;
            _restartButton.gameObject.SetActive(false);
            if (_collectResultPanel) _collectResultPanel.SetActive(false);
            if (_gameOverPanel) _gameOverPanel.SetActive(false);
            GameEvents.OnRestart.Raise();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var buttons = GetComponentsInChildren<Button>(includeInactive: true);
            foreach (var btn in buttons)
            {
                string n = btn.name.ToLower();
                if (_spinButton == null && n.Contains("spin")) _spinButton = btn;
                if (_restartButton == null && n.Contains("restart")) _restartButton = btn;
            }
        }
#endif
    }
}
