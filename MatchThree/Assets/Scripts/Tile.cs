using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RomanDoliba.Board
{
    public class Tile : MonoBehaviour
    {
        public Image icon;
        public Button button;
        private Item _item;
        public int X {get; set;}
        public int Y {get; set;}
        public Item Item
        {
            get => _item;
            set
            {
                if (_item == value)
                {
                    return;
                }
                else
                {
                    _item = value;
                }
                icon.sprite = _item.Sprite;
            }
        }
        public Tile Left => X > 0 ? Board.Instance.Tiles[X - 1, Y] : null;
        public Tile Top => Y > 0 ? Board.Instance.Tiles[X, Y - 1] : null;
        public Tile Right => X < Board.Instance.Width - 1 ? Board.Instance.Tiles[X + 1, Y] : null;
        public Tile Bottom => Y < Board.Instance.Height - 1 ? Board.Instance.Tiles[X, Y + 1] : null;

        public Tile[] Neighbours => new[]
        {
            Left,
            Top,
            Right,
            Bottom,
        };

        private void Start()
        {
            button.onClick.AddListener(call:() => Board.Instance.Select(this));
        }

        public List<Tile> GetConnectedTiles(List<Tile> exclude = null)
        {
            var result = new List<Tile> {this,};

            if (exclude == null)
            {
                exclude  = new List<Tile> {this, };
            }
            else
            {
                exclude.Add(this);
            }

            foreach (var neighbour in Neighbours)
            {
                if (neighbour == null || exclude.Contains(neighbour) || neighbour.Item != Item) continue;
                
                result.AddRange(neighbour.GetConnectedTiles(exclude));
            }
            return result;
        }
    }
}
