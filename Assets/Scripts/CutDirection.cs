using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutDirection : MonoBehaviour
{
    public GameObject arrowCubeBluePrefab;
    public GameObject arrowCubeRedPrefab;
    public Note notePrefab;
    private bool hasSetCutDirection = false;

    public Tile tileParent { get; set; }
    public Note.CutDirection _CutDirection { get; private set; }

    public void SetCutDirection(Note.CutDirection cutDirection)
    {
        if (hasSetCutDirection)
            return;

        _CutDirection = cutDirection;

        hasSetCutDirection = true;
    }

    public Note.CutDirection GetCutDirection(float angle)
    {
        if (angle == 0)
            return Note.CutDirection.Right;
        else if (angle == 45)
            return Note.CutDirection.UpRight;
        else if (angle == 90)
            return Note.CutDirection.Up;
        else if (angle == 135)
            return Note.CutDirection.UpLeft;
        else if (angle == 180)
            return Note.CutDirection.Left;
        else if (angle == 225)
            return Note.CutDirection.DownLeft;
        else if (angle == 270)
            return Note.CutDirection.Down;
        else if (angle == 315)
            return Note.CutDirection.DownRight;
        else
            return Note.CutDirection.Dot;
    }

    public float? GetAngle(Note.CutDirection cutDirection)
    {
        switch (cutDirection)
        {
            case Note.CutDirection.Right:
                return 0;
            case Note.CutDirection.UpRight:
                return 45;
            case Note.CutDirection.Up:
                return 90;
            case Note.CutDirection.UpLeft:
                return 135;
            case Note.CutDirection.Left:
                return 180;
            case Note.CutDirection.DownLeft:
                return 225;
            case Note.CutDirection.Down:
                return 270;
            case Note.CutDirection.DownRight:
                return 315;
            case Note.CutDirection.Dot:
                return null;
            default:
                break;
        }

        return null;
    }

    public void AddNote()
    {
        var note = MapCreator._Map.AddNote(notePrefab, this, tileParent, MapEditorManager.Instance.CurrentTime, MapEditorManager.Instance.CurrentColor);

        GameObject arrowCube = MapEditorManager.Instance.CurrentColor == Note.ColorType.Blue ? Instantiate(arrowCubeBluePrefab) : Instantiate(arrowCubeRedPrefab);

        Vector2 coordinate = _3DGridGenerator.Instance.GetCoordinatePosition(new Vector2Int(note._lineIndex, note._lineLayer), arrowCube);

        arrowCube.transform.position = new Vector3(coordinate.x, (float)_3DGridGenerator.Instance.GetBeatPosition(MapEditorManager.Instance.CurrentTime), coordinate.y);
        arrowCube.transform.Rotate(new Vector3(0, 0, -1), GetAngle((Note.CutDirection)note._cutDirection).Value);
        arrowCube.transform.SetParent(GameObject.FindGameObjectWithTag("3DCanvas").transform, false);

        arrowCube.GetComponent<ArrowCube>().noteParent = note;
        note.arrowCube = arrowCube;
    }
}
