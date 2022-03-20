using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    [SerializeField] private AudioClip collectSound;

    [SerializeField] private AudioSource audioSource;

    public Row[] rows;

    public Tile[,] Tiles { get; private set; }

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.25f;


    private void Awake() => Instance = this;

    private void Start()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                
                var randomItem = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                if(x > 1)
                {
                    if(Tiles[x - 1, y].Item == randomItem)
                    {
                        randomItem = Tiles[x - 2, y].Item;
                    }
                }

                if (y > 1)
                {
                    if (Tiles[x, y - 1].Item == randomItem)
                    {
                        randomItem = Tiles[x, y - 2].Item;
                    }
                }

                tile.Item = randomItem;
                

                Tiles[x, y] = tile;
            }
        }
    }


    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile)) 
        {
            if (_selection.Count > 0)
            {
                if (System.Array.IndexOf(_selection[0].Neighbours, tile) != -1) _selection.Add(tile);
            }
            else
            {
                _selection.Add(tile);
            }
        }

        if (_selection.Count < 2) return;

        await Swap(_selection[0], _selection[1]);


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

        await sequence.Play()
                      .AsyncWaitForCompletion();

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tileItem = tile1.Item;

        tile1.Item = tile2.Item;
        tile2.Item = tileItem;
    }

    private bool CanPop()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                if (tile.GetConnectedTiles().Skip(1).Count() >= 2 && tile.Avaible) return true;
            }
        }
        return false;
    }

    private async void Pop()
    {
        for(var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];

                if (!tile.Avaible) continue;

                var connectedTiles = tile.GetConnectedTiles();

                if (connectedTiles.Skip(1).Count() < 2) continue;

                var deflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles) deflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.zero, TweenDuration));

                audioSource.PlayOneShot(collectSound);

                ScoreCounter.Instance.Score += tile.Item.value * connectedTiles.Count;

                await deflateSequence.Play()
                                     .AsyncWaitForCompletion();

                var inflateSequence = DOTween.Sequence();

                foreach (var connectedTile in connectedTiles)
                {
                    var randomItem = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                    foreach(var neighbour in connectedTile.Neighbours)
                    {
                        if (neighbour == null) continue;

                        if(neighbour.Item == randomItem)
                        {
                            randomItem = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                        }
                    }

                    connectedTile.Item = randomItem;

                    inflateSequence.Join(connectedTile.icon.transform.DOScale(Vector3.one, TweenDuration));
                }



                await inflateSequence.Play()
                                     .AsyncWaitForCompletion();

                x = 0;
                y = 0;
            }
        }

        AppearUnavaibleTile();
    }

    private async void AppearUnavaibleTile()
    {
        var sequence1 = DOTween.Sequence();

        var x = Random.Range(0, Width);
        var y = Random.Range(0, Height);

        var tile = Tiles[x, y];
        var icon = tile.icon;

        sequence1.Join(icon.transform.DOScale(Vector3.zero, TweenDuration));

        await sequence1.Play()
                       .AsyncWaitForCompletion();

        icon.color = Color.black;

        var sequence2 = DOTween.Sequence();

        sequence2.Join(icon.transform.DOScale(Vector3.one, TweenDuration));

        await sequence2.Play()
                       .AsyncWaitForCompletion();

        tile.Avaible = false;
    }
}
