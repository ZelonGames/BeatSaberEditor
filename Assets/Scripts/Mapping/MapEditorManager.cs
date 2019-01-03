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
    private int currentTimeStampIndex = 0;

    [SerializeField]
    private GameObject timeline;
    [SerializeField]
    private TextMeshProUGUI txtPrecision;
    [SerializeField]
    private TextMeshProUGUI txtBeatTime;

    public List<Note> ShowedNotes { get; set; }

    public readonly int maxPrecision = 64;

    public delegate void UpdateNextAndPrevNote();

    #endregion

    #region Properties

    public Note.ItemType ItemType { get; private set; }
    public int Precision { get; private set; }
    public double NoteTimer { get; private set; }
    public bool Playing { get; private set; }

    public double CurrentTimeStamp
    {
        get
        {
            return timeStamps[currentTimeStampIndex];
        }
    }

    public double BeatCounter { get; private set; }

    public static MapEditorManager Instance { get; private set; }

    #endregion

    #region Events

    private void Start()
    {
        Instance = this;
        ItemType = Note.ItemType.Blue;
        Precision = 1;
        GetTimeStamps();
    }

    private void Update()
    {
        if (Playing)
        {
            NoteTimer += Time.deltaTime;

            if (NoteTimer >= CurrentTimeStamp)
            {
                HideShowedNotes();
                ShowNotes(currentTimeStampIndex);
                SetNextTimeStamp();
            }

            if (currentTimeStampIndex == timeStamps.Count)
                Playing = false;
        }
    }

    public void OnChangeTime(bool forward)
    {
        float beatLengthInSeconds = MapCreator._Map.BeatLenghtInSeconds;

        ShowHideNotes(false, BeatCounter);

        if (forward)
        {
            NoteTimer += beatLengthInSeconds / Precision;
            BeatCounter += 1d / Precision;
        }
        else
        {
            NoteTimer -= beatLengthInSeconds / Precision;
            BeatCounter -= 1d / Precision;
        }

        ShowHideNotes(true, BeatCounter);
    }

    public void OnPlay()
    {
        Playing = !Playing;

        if (!Playing)
        {
            BeatCounter = GetBeatTimeAfterPlaying();
            UpdateNoteTimerAfterPlaying(BeatCounter);
            PlayTween.Instance.StopMoving();

            HideShowedNotes();
            ShowHideNotes(true, BeatCounter);
        }
        else
            ShowHideNotes(false, BeatCounter);
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

    private void SetNextTimeStamp()
    {
        if (currentTimeStampIndex < timeStamps.Count - 1)
            currentTimeStampIndex++;
    }

    private void GetTimeStamps()
    {
        timeStamps.Clear();
        if (MapCreator._Map._notes == null)
            return;

        foreach (var notes in MapCreator._Map.NoteTimeChunks.Values)
        {
            double noteTime = MapCreator._Map.BeatLenghtInSeconds * notes.First()._time;
            timeStamps.Add(noteTime);
        }
    }

    public void ChangePrecision(int value)
    {
        Precision = value;
    }

    private void ShowNotes(int index)
    {
        if (index < 0 || index > MapCreator._Map.NoteTimeChunks.Count - 1)
            return;

        List<Note> notes = MapCreator._Map.NoteTimeChunks.Values[index];

        foreach (var note in notes)
            note.gameObject.SetActive(true);

        ShowedNotes = notes;
        currentTimeStampIndex = MapCreator._Map.NoteTimeChunks.Values.IndexOf(ShowedNotes);
    }

    private void HideShowedNotes()
    {
        if (ShowedNotes == null)
            return;

        foreach (var note in ShowedNotes)
            note.gameObject.SetActive(false);
    }

    public void ShowHideNotes(bool show, double beat)
    {
        if (!MapCreator._Map.NoteTimeChunks.ContainsKey(beat))
            return;

        List<Note> noteChunk = MapCreator._Map.NoteTimeChunks[beat];
        foreach (var notes in noteChunk)
            notes.gameObject.SetActive(show);

        if (show)
            ShowedNotes = noteChunk;

        currentTimeStampIndex = MapCreator._Map.NoteTimeChunks.Values.IndexOf(noteChunk);
    }

    public void UpdateNoteTimerAfterPlaying(double currentBeat)
    {
        NoteTimer = MapCreator._Map.BeatLenghtInSeconds * currentBeat;
    }

    public double GetCurrentNoteTimeInBeats()
    {
        return NoteTimer == 0 ? 0 : MapCreator._Map._beatsPerMinute * NoteTimer / 60f;
    }

    public double GetBeatTimeAfterPlaying()
    {
        double precisionValue = 1d / Precision;
        int multiplier = (int)(GetCurrentNoteTimeInBeats() / precisionValue);

        return (double)multiplier / Precision;
    }

    #endregion
}
