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

    [SerializeField]
    private GameObject timeline;
    [SerializeField]
    private TextMeshProUGUI txtPrecision;
    [SerializeField]
    private TextMeshProUGUI txtBeatTime;

    private SortedList<float, List<Note>> notesToShow = null;
    private List<Note> nextNotesToShow = null;

    public readonly int maxPrecision = 64;

    public static float minValue = 1f / 64;

    #endregion

    #region Properties

    public List<Note> ShowedNotes { get; set; }
    public Note.ItemType ItemType { get; private set; }

    public float CurrentTime { get; private set; }

    public float CurrentBeat => CurrentTime == 0 ? 0 : (MapCreator._Map._beatsPerMinute * CurrentTime) / 60f;

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
    }

    private void Update()
    {
        if (Playing)
        {
            CurrentTime += Time.deltaTime;

            if (_3DGridGenerator.Instance._Grid.shouldUpdate(CurrentBeat))
                _3DGridGenerator.Instance._Grid.Update();

            if (ShouldShowNotes(nextNotesToShow))
            {
                if (ShowedNotes != null)
                    HideNotes(ShowedNotes);

                ShowNotes(nextNotesToShow);
                nextNotesToShow = GetNextNotes();
            }
        }
    }

    public void OnChangeTime(bool forward)
    {
        if (Playing)
            return;

        float jumpLength = 1f / Precision;
        ChangeTime(CurrentBeat + (forward ? jumpLength : -jumpLength), true);
    }

    public void OnPlay()
    {
        Playing = !Playing;

        if (!Playing)
        {
            PlayTween.Instance.StopMoving();
            ChangeTime(CurrentBeat.GetNearestRoundedDown(1f / Precision));
        }
        else
            nextNotesToShow = GetClosestNotes();
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

    public void UpdateNotesToShow()
    {
        notesToShow = MapCreator._Map.GetNotesBetween(_3DGridGenerator.Instance._Grid.FirstBeatGrid.Beat, _3DGridGenerator.Instance._Grid.LastBeatGrid.Beat);

        foreach (var notes in notesToShow.Values)
            notes.ForEach(x => x.arrowCube.SetActive(true));
    }

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

    public void ChangeTime(float beat, bool manuallyChangingTime = false)
    {
        if (beat < 0)
            return;

        bool forward = beat > CurrentBeat;
        float prevNoteTimer = CurrentTime;
        float beatLengthInSeconds = MapCreator._Map.BeatLenghtInSeconds;

        int jumpDistance = Math.Abs((int)beat - (int)CurrentBeat);
        CurrentTime = beatLengthInSeconds * beat;

        if (jumpDistance > 1 || _3DGridGenerator.Instance._Grid.shouldUpdate(CurrentBeat, forward))
            _3DGridGenerator.Instance._Grid.Update(jumpDistance, forward);

        TimelineSlider.Instance.SnapSliderToPrecision(beat);
        PlayTween.Instance.Step(beat);

        nextNotesToShow = GetClosestNotes();

        if (ShowedNotes != null)
            HideNotes(ShowedNotes);

        if (ShouldShowNotes(nextNotesToShow, manuallyChangingTime))
            ShowNotes(nextNotesToShow);
    }

    public void ChangePrecision(int value)
    {
        Precision = value;
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
            float comparedTime = notesOnSameTime[i].First()._time;

            if (Math.Abs(comparedTime - currentTime) < Math.Abs(currentTime - closestNotes.First()._time))
                closestNotes = notesOnSameTime[i];
        }

        return closestNotes;
    }

    private List<Note> GetNextNotes()
    {
        SortedList<float, List<Note>> noteTimeChunkcs = MapCreator._Map.NotesOnSameTime;
        int nextIndex = noteTimeChunkcs.Values.IndexOf(ShowedNotes) + 1;

        if (nextIndex < noteTimeChunkcs.Count)
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
    
    private bool ShouldShowNotes(List<Note> notes, bool manuallyChangingTime = false)
    {
        if (notes == null)
            return false;

        Note note = notes.First();
        float noteTime = note._time;

        return manuallyChangingTime ? Math.Abs(CurrentBeat - noteTime) <= minValue : CurrentBeat >= noteTime;
    }

    #endregion
}
