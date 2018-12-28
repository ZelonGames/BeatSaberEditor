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

    private List<float> timeStamps = new List<float>();
    private int currentTimeStampIndex = 0;

    [SerializeField]
    private GameObject timeline;
    [SerializeField]
    private TextMeshProUGUI txtPrecision;
    [SerializeField]
    private TextMeshProUGUI txtBeatTime;

    #endregion

    #region Properties

    public Note.ItemType ItemType { get; private set; }
    public int Precision { get; private set; }
    public double NoteTimer { get; private set; }
    private double bpmInSeconds;
    public bool Playing { get; private set; }

    public float CurrentTimeStamp
    {
        get
        {
            return timeStamps[currentTimeStampIndex];
        }
    }

    public double CurrentNoteTimeInBeats
    {
        get
        {
            return NoteTimer == 0 ? 0 : MapCreator._Map._beatsPerMinute * NoteTimer / 60d;
        }
    }

    public static MapEditorManager Instance { get; private set; }

    #endregion

    #region Events

    private void Start()
    {
        Instance = this;
        ItemType = Note.ItemType.Blue;
        Precision = 1;

        bpmInSeconds = GetBPMInSeconds(MapCreator._Map._beatsPerMinute);
        GetTimeStamps();
    }

    private void Update()
    {
        if (Playing)
        {
            NoteTimer += Time.deltaTime;

            if (NoteTimer >= CurrentTimeStamp)
            {
                ShowHideNotes(false, currentTimeStampIndex - 1);
                ShowHideNotes(true, currentTimeStampIndex);
                SetNextTimeStamp();
            }

            if (currentTimeStampIndex == timeStamps.Count)
                Playing = false;
        }
    }

    public void OnPlay()
    {
        Playing = !Playing;
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
        if (currentTimeStampIndex < timeStamps.Count)
            currentTimeStampIndex++;
    }

    private void GetTimeStamps()
    {
        timeStamps.Clear();
        if (MapCreator._Map._notes == null)
            return;

        foreach (var notes in MapCreator._Map.NoteTimeChunks.Values)
        {
            float noteTime = (float)(bpmInSeconds * notes.First()._time);
            timeStamps.Add(noteTime);
        }
    }

    public void ChangePrecision(int value)
    {
        Precision = value;
    }

    public void ChangeTime(bool forward)
    {
        ShowHideNotes(false, currentTimeStampIndex);
        ShowHideNotes(false);

        if (forward)
            NoteTimer += (float)bpmInSeconds / Precision;
        else
            NoteTimer -= (float)bpmInSeconds / Precision;

        ShowHideNotes(true);
    }

    public void ShowHideNotes(bool show, int index)
    {
        if (index < 0)
            return;

        foreach (var note in MapCreator._Map.NoteTimeChunks.Values[index])
            note.gameObject.SetActive(show);
    }

    public void ShowHideNotes(bool show)
    {
        var currentNoteTimeInBeats = CurrentNoteTimeInBeats;

        if (!MapCreator._Map.NoteTimeChunks.ContainsKey(currentNoteTimeInBeats))
            return;

        foreach (var note in MapCreator._Map.NoteTimeChunks[currentNoteTimeInBeats])
            note.gameObject.SetActive(show);
    }

    private void UpdatePrecisionText()
    {
        txtPrecision.text = "Precision: 1/" + Precision;
    }

    private double GetBPMInSeconds(int bpm)
    {
        return 60d / bpm;
    }

    #endregion
}
