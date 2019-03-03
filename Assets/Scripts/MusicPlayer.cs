using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

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

    private IEnumerator LoadAudio()
    {
        var www = new WWW("file://" + MapCreator.Instance.MapFolderPath(MapCreator._MapInfo.songName) + "/song.ogg");
        audioSource.clip = www.GetAudioClip();
        audioSource.clip.name = "song.ogg";
        yield return www;
    }

    public void ToggleSong()
    {
        audioSource.time = (float)MapEditorManager.Instance.NoteTimer;

        if (MapEditorManager.Instance.Playing)
            audioSource.Play();
        else
            audioSource.Stop();
    }
}