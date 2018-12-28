using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private Note notePrefab;
    [SerializeField]
    private GameObject arrowCubeBluePrefab;
    [SerializeField]
    private GameObject arrowCubeRedPrefab;
    [SerializeField]
    private GameObject bombSpherePrefab;

    [SerializeField]
    private NotePlacer notePlacerPrefab;
    private static NotePlacer notePlacer = null;

    private bool hasSetCoordinate = false;

    public Vector2Int Coordinate { get; private set; }

    public void SetCoordinate(Vector2Int coordinate)
    {
        if (hasSetCoordinate)
            return;

        Coordinate = coordinate;

        hasSetCoordinate = true;
    }

    public void TouchDown()
    {
        switch (MapEditorManager.Instance.ItemType)
        {
            case Note.ItemType.Red:
            case Note.ItemType.Blue:
                PlaceNotes();
                break;
            case Note.ItemType.Bomb:
                PlaceBombs();
                break;
            default:
                break;
        }
    }

    private void PlaceBombs()
    {
        MapCreator._Map.AddNote(notePrefab, bombSpherePrefab, arrowCubeBluePrefab, arrowCubeRedPrefab, Note.CutDirection.Up, Coordinate, MapEditorManager.Instance.CurrentNoteTimeInBeats, MapEditorManager.Instance.ItemType, true);
    }

    private void PlaceNotes()
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
