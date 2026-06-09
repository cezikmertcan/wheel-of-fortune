using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelOfFortune.UI
{
    public class RewardRowUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountText;

        public string ItemId { get; private set; }
        public RectTransform IconRect => (RectTransform)_icon.transform;

        private int _currentAmount;
        private Tween _countTween;

        public void Setup(string itemId, Sprite icon)
        {
            ItemId = itemId;
            _currentAmount = 0;
            _amountText.text = "0";
            if (_icon != null && icon != null) _icon.sprite = icon;
        }

        public void AddAmount(int value, TweenCallback onComplete = null)
        {
            _countTween?.Kill();

            int from = _currentAmount;
            int to = _currentAmount + value;
            _currentAmount = to;

            _countTween = DOTween
                .To(() => from, x => { from = x; _amountText.text = x.ToString(); }, to, 0.6f)
                .SetEase(Ease.OutQuad)
                .OnComplete(onComplete);
        }
    }
}
