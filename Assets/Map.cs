using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Map
{
    public string _version;
    public int _beatsPerMinute;
    public int _beatsPerBar;
    public int _noteJumpSpeed;
    public int _shuffle;
    public double _shufflePeriod = 0.5;
    public List<Note> _notes;

    public Map(string _version, int _beatsPerMinute, int _beatsPerBar, int _noteJumpSpeed, List<Note> _notes)
    {
        this._version = _version;
        this._beatsPerMinute = _beatsPerMinute;
        this._noteJumpSpeed = _noteJumpSpeed;
        this._notes = _notes;
    }

    public void AddNote(double _time, Note.ColorType color, Note.CutDirection cutDirection)
    {
        _notes.Add(new Note(GetBeatTime(_beatsPerMinute, 0, _time), 0, 0, color, cutDirection));
    }

    public double GetBeatTime(double bpm, double ms, double _time)
    {
        return GetMSInBeats(bpm, ms) + _time;
    }

    public double GetMSInBeats(double bpm, double ms)
    {
        return (bpm / 60000) * ms;
    }
}