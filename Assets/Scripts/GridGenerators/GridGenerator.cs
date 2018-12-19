using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Canvas canvas;
    public Tile tilePrefab;

    public Dictionary<Vector2Int, Tile> Tiles { get; private set; }

    public static GridGenerator Instance { get; private set; }

    private Rect rect;
    private Rect canvasRect;
    private Rect tileRectTemplate = new Rect();

    [SerializeField]
    private int tileDistance = 4;
    [SerializeField]
    private int maxCutDirectionRadius = 120;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Tiles = new Dictionary<Vector2Int, Tile>();
        rect = gameObject.GetComponent<RectTransform>().rect;
        canvasRect = canvas.GetComponent<RectTransform>().rect;
        CreateGrid();
    }

    private void CreateGrid()
    {
        int tilesInX = 4;
        int tilesInY = 3;

        float tileSize = (rect.width - tileDistance * (tilesInX - 1)) / tilesInX;

        for (int x = 0; x < tilesInX; x++)
        {
            for (int y = 0; y < tilesInY; y++)
            {
                Tile newTile = Instantiate(tilePrefab);
                var tileRect = newTile.gameObject.GetComponent<RectTransform>();
                tileRect.sizeDelta = new Vector2(tileSize, tileSize);
                newTile.gameObject.transform.SetParent(gameObject.transform, false);
                newTile.gameObject.transform.position = new Vector3(
                    gameObject.transform.position.x - rect.width * 0.5f + tileRect.sizeDelta.x * 0.5f + tileRect.sizeDelta.x * x + tileDistance * x,
                    gameObject.transform.position.y + rect.height * 0.5f - tileRect.sizeDelta.y * 0.5f - tileRect.sizeDelta.y * y - tileDistance * y);
                var coordinate = new Vector2Int(x, tilesInY - 1 - y);
                newTile.SetCoordinate(coordinate);

                Tiles.Add(coordinate, newTile);
            }
        }

        tileRectTemplate.width = tileRectTemplate.height = tileSize;
    }

    public float GetMaxRadius(float cutDirectionRadius)
    {
        float radiusLeftSide = gameObject.transform.position.x - rect.width * 0.5f + tileRectTemplate.width * 0.5f + cutDirectionRadius;
        float radiusRightSide = canvasRect.width - (gameObject.transform.position.x + rect.width * 0.5f) + tileRectTemplate.width * 0.5f - cutDirectionRadius;

        return Mathf.Min(new float[] { radiusLeftSide, radiusRightSide, maxCutDirectionRadius });
    }
}
