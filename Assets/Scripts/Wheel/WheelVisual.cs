using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data;
using WheelOfFortune.Events;

namespace WheelOfFortune.Wheel
{
    public class WheelVisual : MonoBehaviour
    {
        [SerializeField] private Image _wheelImage;
        [SerializeField] private Image _indicatorImage;

        [Header("Wheel Zone Sprites")]
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _safeSprite;
        [SerializeField] private Sprite _superSprite;

        [Header("Indicator Zone Sprites")]
        [SerializeField] private Sprite _indicatorNormalSprite;
        [SerializeField] private Sprite _indicatorSafeSprite;
        [SerializeField] private Sprite _indicatorSuperSprite;

        private void OnEnable()
        {
            GameEvents.OnZoneChanged.Subscribe(OnZoneChanged);
        }

        private void OnDisable()
        {
            GameEvents.OnZoneChanged.Unsubscribe(OnZoneChanged);
        }

        private void OnZoneChanged(ZoneType zone)
        {
            ApplySprite(_wheelImage, zone switch
            {
                ZoneType.Safe => _safeSprite,
                ZoneType.Super => _superSprite,
                _ => _normalSprite,
            });

            ApplySprite(_indicatorImage, zone switch
            {
                ZoneType.Safe => _indicatorSafeSprite,
                ZoneType.Super => _indicatorSuperSprite,
                _ => _indicatorNormalSprite,
            });
        }

        private static void ApplySprite(Image image, Sprite sprite)
        {
            if (image != null && sprite != null)
                image.sprite = sprite;
        }
    }
}
