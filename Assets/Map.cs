using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class Map
{
    [JsonIgnore]
    public Dictionary<double, List<Note>> noteTimeChunks = new Dictionary<double, List<Note>>();
    public List<Note> _notes = new List<Note>();

    public string _version;
    public int _beatsPerMinute;
    public int _beatsPerBar;
    public int _noteJumpSpeed;
    public int _shuffle;
    public double _shufflePeriod = 0.5;

    public Map(string _version, int _beatsPerMinute, int _beatsPerBar, int _noteJumpSpeed, List<Note> _notes)
    {
        this._version = _version;
        this._beatsPerMinute = _beatsPerMinute;
        this._noteJumpSpeed = _noteJumpSpeed;
        this._notes = _notes;
    }

    public void AddNote(Note notePrefab, CutDirection cutDirection, Tile tile, double _time, Note.ColorType color)
    {
        if (!noteTimeChunks.ContainsKey(_time))
            noteTimeChunks.Add(_time, new List<Note>());

        var note = GameObject.Instantiate(notePrefab);
        note.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("2DGrid").transform);
        note.gameObject.transform.Rotate(new Vector3(0, 0, 1), cutDirection.GetAngle(cutDirection._CutDirection).Value);
        note.gameObject.transform.position = tile.gameObject.transform.position;
        note.Set(GetBeatTime(_beatsPerMinute, 0, _time), 0, 0, color, cutDirection._CutDirection);

        noteTimeChunks[_time].Add(note);
        _notes.Add(note);
        var btnCutDirections = GameObject.FindGameObjectsWithTag("CutDirection");
        foreach (var btnCutDirection in btnCutDirections)
            GameObject.Destroy(btnCutDirection);
    }

    public double GetBeatTime(double bpm, double ms, double _time)
    {
        return GetMSInBeats(bpm, ms) + _time;
    }

    public double GetMSInBeats(double bpm, double ms)
    {
        return (bpm / 60000) * ms;
    }

    public Note GetNote(double time, int cutDirection)
    {
        foreach(var note in noteTimeChunks[time])
        {
            if (note._cutDirection == cutDirection)
                return note;
        }

        return null;
    }
}