using UnityEngine;
using WheelOfFortune.Events;
using WheelOfFortune.UI;
using WheelOfFortune.Wheel;

namespace WheelOfFortune.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private WheelController _wheelController;
        [SerializeField] private ZoneProgressUI _zoneProgressUI;

        private void Awake()
        {
            GameContext.state = GameState.Idle;
            GameContext.GameManager = this;
            _wheelController.Initialize();
            _zoneProgressUI.Initialize(_wheelController.SafeZoneInterval, _wheelController.SuperZoneInterval);
        }

        private void OnEnable()
        {
            GameEvents.OnSpinButtonClicked.Subscribe(OnSpinButtonClicked);
            GameEvents.OnSpinCompleted.Subscribe(OnSpinCompleted);
            GameEvents.OnResultDisplayComplete.Subscribe(OnResultDisplayComplete);
            GameEvents.OnCollectAndExit.Subscribe(OnCollectAndExit);
            GameEvents.OnGameOver.Subscribe(OnGameOver);
        }

        private void OnDisable()
        {
            GameEvents.OnSpinButtonClicked.Unsubscribe(OnSpinButtonClicked);
            GameEvents.OnSpinCompleted.Unsubscribe(OnSpinCompleted);
            GameEvents.OnResultDisplayComplete.Unsubscribe(OnResultDisplayComplete);
            GameEvents.OnCollectAndExit.Unsubscribe(OnCollectAndExit);
            GameEvents.OnGameOver.Unsubscribe(OnGameOver);
        }

        private void OnSpinButtonClicked()
        {
            if (GameContext.state != GameState.Idle) return;
            GameContext.state = GameState.Spinning;
            _wheelController.Spin();
        }

        private void OnSpinCompleted(SpinResult result)
        {
            GameContext.state = result.IsBomb ? GameState.BombHit : GameState.ShowingReward;
            GameContext.UIManager.ShowResult(result);
        }

        private void OnResultDisplayComplete()
        {
            _wheelController.PrepareNextSpin();
            GameContext.state = GameState.Idle;
        }

        private void OnCollectAndExit()
        {
            GameContext.state = GameState.Collected;
            GameContext.UIManager.ShowCollectResult();
        }

        private void OnGameOver()
        {
            GameContext.state = GameState.GameOver;
        }
    }
}
