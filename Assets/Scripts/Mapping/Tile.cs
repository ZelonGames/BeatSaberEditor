using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private NotePlacer notePlacerPrefab;
    private static NotePlacer notePlacer = null;

    private bool hasSetCoordinate = false;

    public Vector2Int Coordinate { get; private set; }

    private void Start()
    {
    }

    public void SetCoordinate(Vector2Int coordinate)
    {
        if (hasSetCoordinate)
            return;

        Coordinate = coordinate;

        hasSetCoordinate = true;
    }

    public void TouchDown()
    {
        if (notePlacer == null)
        {
            notePlacer = Instantiate(notePlacerPrefab);
            notePlacer.Activate(this);
        }
        else
        {
            notePlacer.gameObject.SetActive(true);
            notePlacer.Move(this);
            notePlacer.MoveDownGameObjectIndex(notePlacer.gameObject);
        }
    }
}
