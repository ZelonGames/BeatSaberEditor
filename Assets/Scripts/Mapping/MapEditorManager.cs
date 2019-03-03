using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditorManager : MonoBehaviour
{
    #region Fields

    public List<double> timeStamps = new List<double>();

    [SerializeField]
    private GameObject timeline;
    [SerializeField]
    private TextMeshProUGUI txtPrecision;
    [SerializeField]
    private TextMeshProUGUI txtBeatTime;

    private List<Note> notesToShow = null;

    public readonly int maxPrecision = 64;

    public delegate void UpdateNextAndPrevNote();

    #endregion

    #region Properties

    public List<Note> ShowedNotes { get; set; }
    public Note.ItemType ItemType { get; private set; }

    public double NoteTimer { get; private set; }

    public double CurrentBeat => NoteTimer == 0 ? 0 : MapCreator._Map._beatsPerMinute * NoteTimer / 60f + Map.GetMSInBeats(MapCreator._Map._beatsPerMinute, MapCreator._MapInfo.currentDifficulty.offset);

    public int Precision { get; private set; }
    public bool Playing { get; private set; }

    public static MapEditorManager Instance { get; private set; }

    #endregion

    #region Events

    private void Start()
    {
        Instance = this;
        ItemType = Note.ItemType.Blue;
        Precision = 1;
        LoadTimeStamps();
    }

    private void Update()
    {
        if (Playing)
        {
            NoteTimer += Time.deltaTime;

            if (ShouldShowNotes(notesToShow))
            {
                if (ShowedNotes != null)
                    HideNotes(ShowedNotes);

                ShowNotes(notesToShow);
                notesToShow = GetNextNotes();
            }
        }
    }

    public void OnChangeTime(bool forward)
    {
        if (Playing)
            return;

        double jumpLength = 1d / Precision;
        ChangeTime(CurrentBeat + (forward ? jumpLength : -jumpLength));
    }

    public void OnPlay()
    {
        Playing = !Playing;

        if (!Playing)
        {
            PlayTween.Instance.StopMoving();
            ChangeTime(GetSnappedPrecisionBeatTime(CurrentBeat, Precision));
        }
        else
        {
            notesToShow = GetClosestNotes();
        }
    }

    public void OnBlueArrowSelected()
    {
        ItemType = Note.ItemType.Blue;
    }

    public void OnRedArrowSelected()
    {
        ItemType = Note.ItemType.Red;
    }

    public void OnBombSelected()
    {
        ItemType = Note.ItemType.Bomb;
    }

    #endregion

    #region Methods

    public void ShowNotes(List<Note> notes)
    {
        if (notes == null)
            return;

        foreach (var note in notes)
            note.gameObject.SetActive(true);

        ShowedNotes = notes;
    }

    public void HideNotes(List<Note> notes)
    {
        if (notes == null)
            return;

        foreach (var note in notes)
            note.gameObject.SetActive(false);
    }

    public void ChangeTime(double beat)
    {
        double prevNoteTimer = NoteTimer;
        double beatLengthInSeconds = MapCreator._Map.BeatLenghtInSeconds;
        NoteTimer = beatLengthInSeconds * beat;

        TimelineSlider.Instance.SnapSliderToPrecision((float)beat);
        PlayTween.Instance.Step((float)GetSnappedPrecisionBeatTime(beat, Precision));

        notesToShow = GetClosestNotes();

        if (ShouldShowNotes(notesToShow, 1/64d))
        {
            if (ShowedNotes != null)
                HideNotes(ShowedNotes);

            ShowNotes(notesToShow);
        }
    }

    public void ChangePrecision(int value)
    {
        Precision = value;
    }

    private void LoadTimeStamps()
    {
        timeStamps.Clear();
        if (MapCreator._Map._notes == null)
            return;

        foreach (var notes in MapCreator._Map.NotesOnSameTime.Values)
        {
            double noteTime = MapCreator._Map.BeatLenghtInSeconds * notes.First()._time;
            timeStamps.Add(noteTime);
        }
    }

    private List<Note> GetClosestNotes()
    {
        if (MapCreator._Map.NotesOnSameTime.Count == 0)
            return null;

        var notesOnSameTime = MapCreator._Map.NotesOnSameTime.Values;
        List<Note> closestNotes = notesOnSameTime.First();
        double currentTime = CurrentBeat;

        for (int i = 1; i < notesOnSameTime.Count; i++)
        {
            double comparedTime = notesOnSameTime[i].First()._time;

            if (Math.Abs(comparedTime - currentTime) < Math.Abs(currentTime - closestNotes.First()._time))
                closestNotes = notesOnSameTime[i];
        }

        return closestNotes;
    }

    private List<Note> GetNextNotes()
    {
        SortedList<double, List<Note>> noteTimeChunkcs = MapCreator._Map.NotesOnSameTime;
        int nextIndex = noteTimeChunkcs.Values.IndexOf(ShowedNotes) + 1;

        if (nextIndex <= noteTimeChunkcs.Count)
            return noteTimeChunkcs.Values[nextIndex];

        return null;
    }

    private List<Note> GetPreviousNotes()
    {
        int previousIndex = MapCreator._Map.NotesOnSameTime.Values.IndexOf(ShowedNotes) - 1;
        if (previousIndex > 0 && MapCreator._Map.NotesOnSameTime.Values.Count > 0)
            return MapCreator._Map.NotesOnSameTime.Values[previousIndex];

        return null;
    }

    public double GetSnappedPrecisionBeatTime(double beat, double precision)
    {
        double precisionValue = 1d / precision;
        int multiplier = (int)(beat / precisionValue);

        return multiplier / precision;
    }

    private bool ShouldShowNotes(List<Note> notes, double? precision = null)
    {
        if (notes == null)
            return false;

        double noteTime = precision.HasValue ? notesToShow.First()._time.GetNearestRoundedDown(precision.Value) : notesToShow.First()._time;
        return CurrentBeat >= noteTime;
    }

    #endregion
}
