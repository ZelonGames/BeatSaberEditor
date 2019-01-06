using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    private float? offsetTimer = null;

    public float MusicLengthInSeconds()
    {
        if (audioSource.clip == null)
            return 0;
        return audioSource.clip.length;
    }

    public bool IsLoaded
    {
        get
        {
            return audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Loaded;
        }
    }

    public static MusicPlayer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(LoadAudio());
    }

    private void Update()
    {
        if (offsetTimer.HasValue)
        {
            if (offsetTimer > 0)
                offsetTimer -= Time.deltaTime;
            else if (IsLoaded && !audioSource.isPlaying)
                audioSource.Play();
        }
    }

    private IEnumerator LoadAudio()
    {
        var www = new WWW("file://" + MapCreator.Instance.MapFolderPath(MapCreator._MapInfo.songName) + "/song.ogg");
        audioSource.clip = www.GetAudioClip();
        audioSource.clip.name = "song.ogg";
        yield return www;
    }

    public void ToggleSong()
    {
        float startTime = (float)MapEditorManager.Instance.NoteTimer + MapCreator._MapInfo.currentDifficulty.offset * 0.001f;
        audioSource.time = startTime;
        if (startTime < 0)
            offsetTimer = -startTime;
        else
            offsetTimer = null;

        if (MapEditorManager.Instance.Playing)
        {
            if (!offsetTimer.HasValue)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
            offsetTimer = null;
        }
    }
}