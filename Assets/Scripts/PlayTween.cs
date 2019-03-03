using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTween : MonoBehaviour
{
    public static PlayTween Instance { get; private set; }

    private int distance = 800;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.transform.position = GetBeatPosition();
    }

    public void Move()
    {
        var destination = GetBeatPosition(MapCreator._Map.GetAmountOfBeatsInSong());

        if (MapEditorManager.Instance.Playing)
            iTween.MoveTo(gameObject, iTween.Hash("position", destination, "time",
                MusicPlayer.Instance.MusicLengthInSeconds() - MapCreator._Map.BeatLenghtInSeconds * MapEditorManager.Instance.CurrentBeat, 
                "easetype", iTween.EaseType.linear));
    }

    public void StopMoving()
    {
        iTween.Stop();
        gameObject.transform.position = GetBeatPosition();
    }

    public void Step(float beat)
    {
        gameObject.transform.position = GetBeatPosition(beat);
    }

    private Vector3 GetBeatPosition()
    {
        return new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 
            (float)_3DGridGenerator.Instance.GetBeatPosition(MapEditorManager.Instance.CurrentBeat) - distance);
    }

    private Vector3 GetBeatPosition(double beat)
    {
        return new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 
            (float)_3DGridGenerator.Instance.GetBeatPosition(beat) - distance);
    }
}
