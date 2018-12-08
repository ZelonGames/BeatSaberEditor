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
    private TextMeshProUGUI txtPrecision;
    [SerializeField]
    private TextMeshProUGUI txtBeatTime;

    private double timer = 0;
    private double? playingPrecision;

    #endregion

    #region Properties

    public Note.ColorType CurrentColor { get; private set; }
    public int Precision { get; private set; }
    public double CurrentTime { get; private set; }
    public bool Playing { get; private set; }

    public double CurrentTimeInSeconds
    {
        get
        {
            return MapCreator._Map.BeatLenghtInSeconds * CurrentTime;
        }
    }

    public static MapEditorManager Instance { get; private set; }

    #endregion

    #region Events

    private void Start()
    {
        Instance = this;
        CurrentColor = Note.ColorType.Blue;
        Precision = 1;

        UpdateBeatTimeText();
    }

    private void FixedUpdate()
    {
        if (Playing)
            Play(MapCreator._Map._beatsPerMinute);
    }

    #endregion

    #region Methods

    public void Play()
    {
        timer = 0;
        Playing = !Playing;
        if (!Playing)
            playingPrecision = null;
    }

    public void Play(int bpm)
    {
        timer += Time.deltaTime;

        if (!playingPrecision.HasValue)
            SetPlayingPrecision();

        if (timer >= GetBPMInSeconds(MapCreator._Map._beatsPerMinute) / playingPrecision.Value)
        {
            ChangeTime(true, true);
            timer = 0;
        }
    }

    public void IncreasePrecision()
    {
        if (Precision < 64)
            Precision++;

        UpdatePrecisionText();
    }

    public void DecreasePrecision()
    {
        if (Precision > 1)
            Precision--;

        UpdatePrecisionText();
    }

    private void SetPlayingPrecision()
    {
        double? nextTime = null;

        if (MapCreator._Map.NoteTimeChunks.Count > 0 && MapCreator._Map.NoteTimeChunks.Count > MapCreator._Map.NoteTimeChunks.IndexOfKey(CurrentTime) + 1)
            nextTime = MapCreator._Map.NoteTimeChunks.Values[MapCreator._Map.NoteTimeChunks.IndexOfKey(CurrentTime) + 1].FirstOrDefault()._time;
        else
            nextTime = null;

        if (nextTime.HasValue && nextTime.Value > CurrentTime)
            playingPrecision = 1d / (nextTime.Value - CurrentTime);
        else
            playingPrecision = null;
    }

    public void ChangeTime(bool forward, bool autoPlay)
    {
        ShowHideNotes(false);

        if (forward)
            CurrentTime += autoPlay && playingPrecision.HasValue ? 1d / playingPrecision.Value : 1d / Precision;
        else
        {
            CurrentTime -= 1d / Precision;
            if (CurrentTime < 0)
                CurrentTime = 0;
        }

        UpdateBeatTimeText();

        if (autoPlay)
            SetPlayingPrecision();
        else
            playingPrecision = null;

        ShowHideNotes(true);
    }

    public void StepTime(bool forward)
    {
        ChangeTime(forward, false);
    }

    public void SwitchColor()
    {
        switch (CurrentColor)
        {
            case Note.ColorType.Red:
                CurrentColor = Note.ColorType.Blue;
                break;
            case Note.ColorType.Blue:
                CurrentColor = Note.ColorType.Red;
                break;
        }
    }

    public void ShowHideNotes(bool show)
    {
        if (!MapCreator._Map.NoteTimeChunks.ContainsKey(CurrentTime))
            return;

        foreach (var note in MapCreator._Map.NoteTimeChunks[CurrentTime])
        {
            note.gameObject.SetActive(show);
        }
    }

    private void UpdatePrecisionText()
    {
        txtPrecision.text = "Precision: 1/" + Precision;
    }

    private void UpdateBeatTimeText()
    {
        txtBeatTime.text = "Beat: " + Math.Round(CurrentTime, 2);
    }

    private double GetBPMInSeconds(int bpm)
    {
        return 60d / bpm;
    }

    #endregion
}
