using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Canvas canvas;
    public Tile tilePrefab;

    public List<Tile> tiles = new List<Tile>();

    void Start()
    {
        CreateGrid(4);
    }

    private void CreateGrid(int distance)
    {
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var tileRect = tilePrefab.GetComponent<RectTransform>().rect;

        int amount = 4;

        float totalWidth = tileRect.width * amount + distance * (amount - 1);
        float distanceFromCenter = (canvasRect.width - totalWidth) * 0.5f;

        for (int i = 0; i < amount; i++)
        {
            var newTile = Instantiate(tilePrefab);
            newTile.gameObject.transform.SetParent(gameObject.transform, false);
            newTile.transform.position = new Vector2(tileRect.width * 0.5f + tileRect.width * i + distance * i + distanceFromCenter, canvasRect.height - tileRect.height * 0.5f - 100);
            tiles.Add(newTile);
        }
    }

    void Update()
    {

    }
}
