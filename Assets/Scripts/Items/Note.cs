using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    [HideInInspector]
    public GameObject arrowCube = null;

    public enum CutDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        UpLeft = 4,
        UpRight = 5,
        DownLeft = 6,
        DownRight = 7,
        Dot = 8,
    }

    public enum ColorType
    {
        Red = 0,
        Blue = 1,
    }

    [HideInInspector]
    public double _time;
    [HideInInspector]
    public int _lineIndex;
    [HideInInspector]
    public int _lineLayer;
    [HideInInspector]
    public int _type;
    [HideInInspector]
    public int _cutDirection;

    private void Start()
    {/*
        gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("2DGrid").transform);
        gameObject.transform.Rotate(Vector3.forward, CutDirectionButton.GetAngle((CutDirection)_cutDirection).Value);
        var coordinate = new Vector2Int(_lineIndex, _lineLayer);
        gameObject.transform.position = GridGenerator.Instance.Tiles[coordinate].gameObject.transform.position;*/
    }

    public void Set(double _time, int _lineIndex, int _lineLayer, ColorType _type, CutDirection _cutDirection)
    {
        this._time = _time;
        this._lineIndex = _lineIndex;
        this._lineLayer = _lineLayer;
        this._type = (int)_type;
        this._cutDirection = (int)_cutDirection;

        var image = gameObject.GetComponent<Image>();
        switch (_type)
        {
            case ColorType.Red:
                image.color = Color.red;
                break;
            case ColorType.Blue:
                image.color = Color.blue;
                break;
            default:
                break;
        }
    }
}

public class JsonNote
{
    [HideInInspector]
    public double _time;
    [HideInInspector]
    public int _lineIndex;
    [HideInInspector]
    public int _lineLayer;
    [HideInInspector]
    public int _type;
    [HideInInspector]
    public int _cutDirection;

    public JsonNote(double _time, int _lineIndex, int _lineLayer, Note.ColorType _type, Note.CutDirection _cutDirection)
    {
        this._time = _time;
        this._lineIndex = _lineIndex;
        this._lineLayer = _lineLayer;
        this._type = (int)_type;
        this._cutDirection = (int)_cutDirection;
    }
}