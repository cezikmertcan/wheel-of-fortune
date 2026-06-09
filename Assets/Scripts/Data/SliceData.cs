using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "SliceData", menuName = "WheelOfFortune/Slice Data")]
    public class SliceData : ScriptableObject
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private int _value;

        public ItemData Item => _item;
        public int Value => _value;

        public string Id => _item != null ? _item.Id : string.Empty;
        public Sprite IconSprite => _item != null ? _item.IconSprite : null;
    }
}
