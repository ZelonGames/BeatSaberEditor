using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditorManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtPrecision;

    public Note.ColorType CurrentColor { get; private set; }
    public int Precision { get; private set; }
    public float CurrentTime { get; private set; }

    public static MapEditorManager Instance { get; private set; }

    void Start()
    {
        CurrentTime = 1;
        Instance = this;
        CurrentColor = Note.ColorType.Blue;
        Precision = 1;
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
}
