using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

        [Header("Settings")]
        [SerializeField] private float _spinButtonEnableDelay = 0.5f;

        private void Awake()
        {
            GameContext.UIManager = this;
            _spinButton.onClick.AddListener(OnSpinButtonClicked);
            _restartButton.onClick.AddListener(OnRestartClicked);
            _restartButton.gameObject.SetActive(false);
            GameEvents.OnResultDisplayComplete.Subscribe(OnResultDisplayComplete);
            GameEvents.OnCollectAndExit.Subscribe(OnCollectAndExit);
            GameEvents.OnGameOver.Subscribe(OnGameOver);
        }

        private void OnDestroy()
        {
            _spinButton.onClick.RemoveListener(OnSpinButtonClicked);
            _restartButton.onClick.RemoveListener(OnRestartClicked);
            GameEvents.OnResultDisplayComplete.Unsubscribe(OnResultDisplayComplete);
            GameEvents.OnCollectAndExit.Unsubscribe(OnCollectAndExit);
            GameEvents.OnGameOver.Unsubscribe(OnGameOver);
        }

        private void OnResultDisplayComplete() =>
            DOVirtual.DelayedCall(_spinButtonEnableDelay, () => _spinButton.interactable = true);

        private void OnCollectAndExit() => _restartButton.gameObject.SetActive(true);
        private void OnGameOver() => _restartButton.gameObject.SetActive(true);

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
            if (_collectResultPanel != null)
                _collectResultPanel.SetActive(true);
            _rewardPool.SetExitVisible(false);
        }

        private void OnSpinButtonClicked()
        {
            _spinButton.interactable = false;
            GameEvents.OnSpinButtonClicked.Raise();
        }

        private void OnRestartClicked() =>
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

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
