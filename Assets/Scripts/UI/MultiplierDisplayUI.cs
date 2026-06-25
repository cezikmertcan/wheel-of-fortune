using TMPro;
using UnityEngine;
using WheelOfFortune.Events;
using WheelOfFortune.Utilities;

namespace WheelOfFortune.UI
{
    public class MultiplierDisplayUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _prefix = "x";

        private void OnEnable() => GameEvents.OnMultiplierChanged.Subscribe(OnMultiplierChanged);
        private void OnDisable() => GameEvents.OnMultiplierChanged.Unsubscribe(OnMultiplierChanged);

        private void OnMultiplierChanged(float multiplier)
        {
            if (!_label) return;
            _label.text = RewardFormatter.FormatMultiplier(multiplier, _prefix);
        }
    }
}
