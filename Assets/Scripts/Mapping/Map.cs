using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class Map
{
    #region Fields

    public string _version;
    public int _beatsPerMinute;
    public int _beatsPerBar;
    public int _noteJumpSpeed;
    public int _shuffle;
    public double _shufflePeriod = 0.5;

    public List<JsonEvent> _events;
    public List<JsonNote> _notes;

    #endregion

    #region Properties

    public List<float> timeStamps = new List<float>();

    [JsonIgnore]
    public SortedList<float, List<Note>> NotesOnSameTime { get; private set; }
    [JsonIgnore]
    public SortedList<float, List<Note>> NoteChunks { get; private set; }
    public float ChunkBeatSize => 1;

    [JsonIgnore]
    public float BeatLenghtInSeconds => 60f / _beatsPerMinute;

    [JsonIgnore]
    public bool IsLoaded { get; private set; }

    #endregion

    #region Constructors

    public Map()
    {
        NotesOnSameTime = new SortedList<float, List<Note>>();
        NoteChunks = new SortedList<float, List<Note>>();
        this._events = new List<JsonEvent>();
        this._notes = new List<JsonNote>();

        this._version = "1.0";
        this._beatsPerMinute = 0;
        this._noteJumpSpeed = 0;
    }

    public Map(string _version, int _beatsPerMinute, int _beatsPerBar, int _noteJumpSpeed, List<JsonNote> _notes, List<JsonEvent> _events) : this()
    {
        this._version = _version;
        this._beatsPerMinute = _beatsPerMinute;
        this._noteJumpSpeed = _noteJumpSpeed;
        this._notes = _notes;
        this._events = _events;
    }

    #endregion

    #region Methods

    public static Map GetNewEmptyMapFrom(Map map)
    {
        return new Map(map._version, map._beatsPerMinute, map._beatsPerBar, map._noteJumpSpeed, new List<JsonNote>(), new List<JsonEvent>());
    }

    public void RemoveNote(Note note)
    {
        int timeStampIndex = MapCreator._Map.NotesOnSameTime.Values.IndexOf(MapEditorManager.Instance.ShowedNotes);

        NotesOnSameTime[note._time].Remove(note);
        if (NotesOnSameTime[note._time].Count == 0)
        {
            NotesOnSameTime.Remove(note._time);
            timeStamps.RemoveAt(timeStampIndex);
        }

        GameObject.Destroy(note.arrowCube);
        GameObject.Destroy(note.gameObject);
    }

    public void Load(Note notePrefab, GameObject bombPrefab, GameObject blueCubePrefab, GameObject redCubePrefab)
    {
        foreach (var note in MapCreator._Map._notes)
            AddNote(notePrefab, bombPrefab, blueCubePrefab, redCubePrefab, (Note.CutDirection)note._cutDirection, new Vector2Int((int)note._lineIndex, (int)note._lineLayer), note._time, (Note.ItemType)note._type, false);

        LoadTimeStamps();

        IsLoaded = true;
    }

    public void UnLoad(bool resetFileBrowser)
    {
        MapCreator._Map = null;
        MapCreator._MapInfo = null;
        if (resetFileBrowser)
            Filebrowser.ResetAllPaths();

        IsLoaded = false;
    }

    public Note AddNote(Note notePrefab, GameObject bombSpherePrefab, GameObject blueCubePrefab, GameObject redCubePrefab, Note.CutDirection cutDirection, Vector2Int coordinate, float time, Note.ItemType type, bool active = false)
    {
        time -= GetMSInBeats(MapCreator._Map._beatsPerMinute, MapCreator._MapInfo.currentDifficulty.oldOffset);

        if (!NotesOnSameTime.ContainsKey(time))
            NotesOnSameTime.Add(time, new List<Note>());

        Note note = Note.Instantiate(notePrefab, bombSpherePrefab, blueCubePrefab, redCubePrefab, cutDirection, coordinate, time, type, active);
        NotesOnSameTime[time].Add(note);

        if (timeStamps.IndexOf(BeatLenghtInSeconds * note._time) < 0)
            timeStamps.Add(BeatLenghtInSeconds * note._time);

        float chunkTime = time.GetNearestRoundedDown(ChunkBeatSize);
        if (!NoteChunks.ContainsKey(chunkTime))
            NoteChunks.Add(chunkTime, new List<Note>());

        NoteChunks[chunkTime].Add(note);

        return note;
    }

    public List<Note> GetCurrentChunk(float currentBeat)
    {
        return NoteChunks[currentBeat.GetNearestRoundedDown(ChunkBeatSize)];
    }

    public SortedList<float, List<Note>> GetNotesBetween(float startBeat, float endBeat)
    {
        var foundNotes = new SortedList<float, List<Note>>();

        foreach (var notes in NotesOnSameTime)
        {
            if (notes.Key >= startBeat && notes.Key <= endBeat)
                foundNotes.Add(notes.Key, notes.Value);
            if (notes.Key > endBeat)
                break;
        }

        return foundNotes;
    }

    public Note GetNote(float time, int cutDirection)
    {
        return NotesOnSameTime[time].Where(x => x._cutDirection == cutDirection).FirstOrDefault();
    }

    public static float GetMSInBeats(float bpm, float ms)
    {
        return (bpm / 60000) * ms;
    }

    public float GetAmountOfBeatsInSong()
    {
        return MusicPlayer.Instance.MusicLengthInSeconds() / BeatLenghtInSeconds;
    }

    private void LoadTimeStamps()
    {
        timeStamps.Clear();
        if (MapCreator._Map._notes == null)
            return;

        foreach (var notes in MapCreator._Map.NotesOnSameTime.Values)
        {
            if (notes.Count == 0)
                continue;

            float noteTime = MapCreator._Map.BeatLenghtInSeconds * notes.First()._time;
            timeStamps.Add(noteTime);
        }
    }

    #endregion
}