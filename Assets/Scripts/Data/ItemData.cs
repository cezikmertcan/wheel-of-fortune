using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "WheelOfFortune/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _iconSprite;

        public string Id => _id;
        public Sprite IconSprite => _iconSprite;

        public override string ToString() =>
            $"[ItemData id={_id}]";
    }
}
