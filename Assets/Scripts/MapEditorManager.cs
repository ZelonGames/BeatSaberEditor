using System;
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

    #endregion

    #region Properties

    public Note.ColorType CurrentColor { get; private set; }
    public int Precision { get; private set; }
    public float CurrentTime { get; private set; }
    public bool Playing { get; private set; }

    public float CurrentTimeInSeconds
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
    }

    private void FixedUpdate()
    {
        Play(MapCreator._Map._beatsPerMinute);
    }

    #endregion

    #region Methods

    public void Play()
    {
        timer = 0;
        Playing = !Playing;
    }

    public void Play(int bpm)
    {
        if (!Playing)
            return;

        timer += Time.deltaTime;
        if (timer >= GetBPMInSeconds(MapCreator._Map._beatsPerMinute) / Precision)
        {
            ChangeTime(true);
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

    public void ChangeTime(bool forward)
    {
        ShowHideNotes(false);

        if (forward)
            CurrentTime += 1f / Precision;
        else
            CurrentTime -= 1f / Precision;

        UpdateBeatTimeText();

        ShowHideNotes(true);
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
