﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    public bool IsLoaded => audioSource.clip != null && audioSource.clip.loadState == AudioDataLoadState.Loaded;

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
        var www = new WWW("file://" + MapCreator.Instance.MapFolderPath(MapCreator._MapInfo.songName) + "/" + MapCreator._MapInfo.currentDifficulty.audioPath);
        audioSource.clip = www.GetAudioClip();
        audioSource.clip.name = "song.ogg";
        yield return www;
    }

    public void ToggleSong()
    {
        float offsetTime = MapCreator._MapInfo.currentDifficulty.offset > 0 ? MapCreator._MapInfo.currentDifficulty.offset * 0.001f : 0;
        audioSource.time = (float)MapEditorManager.Instance.CurrentTime + offsetTime;

        if (MapEditorManager.Instance.Playing)
            audioSource.Play();
        else
            audioSource.Stop();
    }

    public float MusicLengthInSeconds()
    {
        if (audioSource.clip == null)
            return 0;
        return audioSource.clip.length;
    }
}