using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class TimelineSlider : MonoBehaviour
{
    private class TimeClock
    {
        public string TimeFormat { get; private set; }
        private int minutes;
        private int seconds;

        public void SetTime(double currentTime)
        {
            double totalTime = GetMinutes(currentTime);
            minutes = (int)totalTime;

            double multiplier = totalTime - minutes;
            seconds = (int)(60 * multiplier);

            TimeFormat = minutes + " : " + seconds;
        }

        private double GetMinutes(double currentBeatTimeInSeconds)
        {
            return (currentBeatTimeInSeconds / 60d);
        }
    }

    private Slider slider;

    private TimeClock timeClock = new TimeClock();

    [SerializeField]
    private TextMeshProUGUI txtTime;

    private bool hasSetMaxValue = false;

    public static TimelineSlider Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    private void Update()
    {
        if (MusicPlayer.Instance.IsLoaded && !hasSetMaxValue)
        {
            slider.maxValue = (float)MapCreator._Map.GetAmountOfBeatsInSong();
            hasSetMaxValue = true;
        }

        if (hasSetMaxValue && MapEditorManager.Instance.Playing)
            UpdateSliderTime((float)MapEditorManager.Instance.GetCurrentNoteTimeInBeats());
    }

    public void OnValueChanged()
    {
        MapEditorManager.Instance.ChangeTime(slider.value);
        PlayTween.Instance.Step(slider.value);
    }

    public void UpdateSliderTime(float beat)
    {
        beat = (float)MapEditorManager.Instance.SnapBeatTimeToPrecision(beat, MapEditorManager.Instance.Precision);

        timeClock.SetTime(MapCreator._Map.BeatLenghtInSeconds * beat);
        txtTime.text = timeClock.TimeFormat;

        slider.value = beat;
    }
}
