using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace RomanDoliba.Board
{
    public sealed class Board : MonoBehaviour
    {
        public static Board Instance {get; private set;}
        [SerializeField] private AudioClip _popSound;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Row[] _rows;
        public Tile[,] Tiles {get; private set;}
        public int Width => Tiles.GetLength(dimension:0);
        public int Height => Tiles.GetLength(dimension:1);
        private readonly List<Tile> _selection = new List<Tile>();
        private const float TweenDuration = 0.25f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            Tiles = new Tile[_rows.Max(row => row.Tiles.Length), _rows.Length];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var tile = _rows[y].Tiles[x];
                    tile.X = x;
                    tile.Y = y;
                    tile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0, ItemDatabase.Items.Length)];
                    Tiles[x, y] = tile;
                }
            }
        }
        
        public async void Select(Tile tile)
        {
            var goToOriginColor = DOTween.Sequence();
            var highlightSequence = DOTween.Sequence();
            
            
            highlightSequence.Join(tile.icon.DOColor(Color.red, TweenDuration));
                            
            if (!_selection.Contains(tile))
            {
                if (_selection.Count > 0)
                {
                    if(Array.IndexOf(_selection[0].Neighbours, tile) != -1) _selection.Add(tile);  
                    goToOriginColor.Join(_selection[0].icon.DOColor(Color.white, TweenDuration));
                }
                else
                {
                    _selection.Add(tile);      
                    
                    highlightSequence.Play().Complete();
                }
            } 
            
            if (_selection.Count < 2) return;
            
            await Swap(_selection[0], _selection[1]);
            goToOriginColor.Play().Complete();

            if (CanPop())
            {
                Pop();
            }
            else
            {
                await Swap(_selection[0], _selection[1]);
            }
            
            _selection.Clear();
        }
        public async Task Swap(Tile tile1, Tile tile2)
        {
            var icon1 = tile1.icon;
            var icon2 = tile2.icon;

            var icon1Transform = icon1.transform;
            var icon2Transform = icon2.transform;

            var sequence = DOTween.Sequence();
            sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
                    .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

            await sequence.Play().AsyncWaitForCompletion();

            icon1Transform.SetParent(tile2.transform);
            icon2Transform.SetParent(tile1.transform);

            tile1.icon = icon2;
            tile2.icon = icon1;

            var tile1Item = tile1.Item;
            tile1.Item = tile2.Item;
            tile2.Item = tile1Item;
        }

        private bool CanPop()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) return true;
                }
            }
            return false;
        }
        private async void Pop()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var tile = Tiles[x, y];

                    var connectedTiles = tile.GetConnectedTiles();

                    if (connectedTiles.Skip(1).Count() < 2) continue;
                    
                    var deflateSequence = DOTween.Sequence();
                    
                    foreach (var connectedTile in connectedTiles)
                    {
                        deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                    }

                    _audioSource.PlayOneShot(_popSound);
                    ScoreCounter.Instance.Score += tile.Item.Value * connectedTiles.Count;

                    await deflateSequence.Play().AsyncWaitForCompletion();

                    var inflateSequence = DOTween.Sequence();
                    
                    foreach (var connectedTile in connectedTiles)
                    {
                        connectedTile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0, ItemDatabase.Items.Length)];

                        inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                    }

                    await inflateSequence.Play().AsyncWaitForCompletion();

                    x = 0;
                    y = 0;
                }
            }
        }
    }
}
