using TMPro;
using UnityEngine;
using WheelOfFortune.Events;

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
            if (_label == null) return;
            _label.text = multiplier % 1f == 0f
                ? $"{_prefix}{multiplier:F0}"
                : $"{_prefix}{multiplier:F1}";
        }
    }
}
