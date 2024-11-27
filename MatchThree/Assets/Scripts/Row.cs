using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RomanDoliba.Board
{
    public sealed class Row : MonoBehaviour
    {
        [SerializeField] private Tile[] _tiles;
        public Tile[] Tiles => _tiles;
    }
}
