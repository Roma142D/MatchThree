using UnityEngine;
namespace RomanDoliba.Board
{
    [CreateAssetMenu(fileName = "Item", menuName = "MatchThree/Item")]
    public sealed class Item : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private int _value;
        public Sprite Sprite => _sprite;
    }
}
