using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Note
{
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

    public double _time;
    public int _lineIndex;
    public int _lineLayer;
    public int _type;
    public int _cutDirection;

    public Note(double _time, int _lineIndex, int _lineLayer, ColorType _type, CutDirection _cutDirection)
    {
        this._time = _time;
        this._lineIndex = _lineIndex;
        this._lineLayer = _lineLayer;
        this._type = (int)_type;
        this._cutDirection = (int)_cutDirection;
    }
}