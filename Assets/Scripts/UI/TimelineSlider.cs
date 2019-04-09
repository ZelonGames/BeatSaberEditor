using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class TimelineSlider : MonoBehaviour
{
    private class ClockTime
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

    private ClockTime clockTime = new ClockTime();

    [SerializeField]
    private TextMeshProUGUI txtTime;

    private bool hasSetMaxValue = false;
    private bool shouldTriggerChangeTime = true;

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
            slider.value = (float)MapEditorManager.Instance.CurrentBeat;
    }

    public void OnValueChanged()
    {
        if (!MapEditorManager.Instance.Playing)
        {
            if (shouldTriggerChangeTime)
                MapEditorManager.Instance.ChangeTime(slider.value);
            shouldTriggerChangeTime = true;
        }

        UpdateClockTime();
    }

    public void SnapSliderToPrecision(float beat)
    {
        slider.value = beat.GetNearestRoundedDown(1f / MapEditorManager.Instance.Precision);
        shouldTriggerChangeTime = false;
    }

    private void UpdateClockTime()
    {
        clockTime.SetTime(MapCreator._Map.BeatLenghtInSeconds * slider.value);
        txtTime.text = clockTime.TimeFormat;
    }
}
