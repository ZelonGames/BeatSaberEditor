using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTween : MonoBehaviour
{
    private void Start()
    {
        //gameObject.transform.position = GetCameraBeatPosition();
    }

    public void Move()
    {
        var destination = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, (float)_3DGridGenerator.Instance.GetBeatPosition(MapCreator._Map.AmountOfBeatsInSong()));

        if (MapEditorManager.Instance.Playing)
            iTween.MoveTo(gameObject, iTween.Hash("position", destination, "time", MusicPlayer.Instance.MusicLengthInSeconds, "easetype", iTween.EaseType.linear));
        else
        {
            iTween.Stop();
            gameObject.transform.position = GetBeatPosition();
        }
    }

    public void Step()
    {
        gameObject.transform.position = GetBeatPosition();
    }

    private Vector3 GetBeatPosition()
    {
        return new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, (float)_3DGridGenerator.Instance.GetBeatPosition(MapEditorManager.Instance.CurrentTime));
    }
}
