using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [SerializeField] private List<Sprite> tileSprite = new List<Sprite>();
    [SerializeField] private GameObject tile;

    [SerializeField] private int xSize, ySize;

    private void Start()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        float xOffset = tile.transform.localScale.x * 8.0f;
        float yOffset = tile.transform.localScale.y * 8.0f;

        float xStart = -(xOffset * (xSize - 1)) / 2;
        float yStart = -(yOffset * (ySize - 1)) / 2;

        Debug.Log(xStart + " " + yStart + " " + xOffset + " " + yOffset);

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                GameObject currentTile = Instantiate(tile, new Vector3(xStart + (xOffset * x), yStart + (yOffset * y)), tile.transform.rotation);

                currentTile.GetComponent<Tile>().SetSprite(GenerateTileSprite());
            }
        }
    }

    private Sprite GenerateTileSprite()
    {
        int index = Random.Range(0, tileSprite.Count - 1);
        return tileSprite[index];
    }
}
