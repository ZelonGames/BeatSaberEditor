using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class Map
{
    #region Fields

    [JsonIgnore]
    public List<Note> _notesObjects;
    public List<JsonNote> _notes;

    public string _version;
    public int _beatsPerMinute;
    public int _beatsPerBar;
    public int _noteJumpSpeed;
    public int _shuffle;
    public double _shufflePeriod = 0.5;

    #endregion

    #region Properties

    [JsonIgnore]
    public SortedList<double, List<Note>> NoteTimeChunks { get; private set; }

    [JsonIgnore]
    public float BeatLenghtInSeconds
    {
        get
        {
            return 60f / _beatsPerMinute;
        }
    }

    #endregion

    #region Constructors

    public Map(string _version, int _beatsPerMinute, int _beatsPerBar, int _noteJumpSpeed, List<Note> _notes)
    {
        NoteTimeChunks = new SortedList<double, List<Note>>();
        this._version = _version;
        this._beatsPerMinute = _beatsPerMinute;
        this._noteJumpSpeed = _noteJumpSpeed;
        this._notesObjects = _notes;
    }

    #endregion

    #region Methods

    public void RemoveNote(Note note)
    {
        _notesObjects.Remove(note);
        NoteTimeChunks[note._time].Remove(note);
        if (NoteTimeChunks[note._time].Count == 0)
            NoteTimeChunks.Remove(note._time);

        GameObject.Destroy(note.arrowCube);
        GameObject.Destroy(note.gameObject);
    }

    public void ClearNotes()
    {
        if (_notes != null)
            _notes.Clear();
        if (_notesObjects != null)
            _notesObjects.Clear();
    }

    public Note AddNote(Note notePrefab, CutDirection cutDirection, Tile tile, double _time, Note.ColorType color)
    {
        if (!NoteTimeChunks.ContainsKey(_time))
            NoteTimeChunks.Add(_time, new List<Note>());

        var note = GameObject.Instantiate(notePrefab);
        note.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("2DGrid").transform);
        note.gameObject.transform.Rotate(Vector3.forward, CutDirection.GetAngle(cutDirection._CutDirection).Value);
        note.gameObject.transform.position = tile.gameObject.transform.position;
        note.Set(GetBeatTime(_beatsPerMinute, 0, _time), tile.Coordinate.x, tile.Coordinate.y, color, cutDirection._CutDirection);

        NoteTimeChunks[_time].Add(note);
        _notesObjects.Add(note);
        var btnCutDirections = GameObject.FindGameObjectsWithTag("CutDirection");
        foreach (var btnCutDirection in btnCutDirections)
            GameObject.Destroy(btnCutDirection);

        return note;
    }

    public Note AddNote(Note notePrefab, Note.CutDirection cutDirection, Vector2Int coordinate, double _time, Note.ColorType color)
    {
        if (!NoteTimeChunks.ContainsKey(_time))
            NoteTimeChunks.Add(_time, new List<Note>());

        var note = GameObject.Instantiate(notePrefab);
        note.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("2DGrid").transform);
        note.gameObject.transform.Rotate(Vector3.forward, CutDirection.GetAngle(cutDirection).Value);
        note.gameObject.transform.position = GridGenerator.Instance.Tiles[coordinate].gameObject.transform.position;
        note.Set(GetBeatTime(_beatsPerMinute, 0, _time), coordinate.x, coordinate.y, color, cutDirection);

        NoteTimeChunks[_time].Add(note);
        _notesObjects.Add(note);
        var btnCutDirections = GameObject.FindGameObjectsWithTag("CutDirection");
        foreach (var btnCutDirection in btnCutDirections)
            GameObject.Destroy(btnCutDirection);

        note.gameObject.SetActive(false);

        return note;
    }


    public double AmountOfBeatsInSong()
    {
        return MusicPlayer.Instance.MusicLengthInSeconds() / BeatLenghtInSeconds;
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
        foreach (var note in NoteTimeChunks[time])
        {
            if (note._cutDirection == cutDirection)
                return note;
        }

        return null;
    }

    public Note GetNextNote(double currentTime)
    {
        return null;
    }

    #endregion
}