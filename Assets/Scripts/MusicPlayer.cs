using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip music;
    [SerializeField]
    private AudioSource audioSource;

    public float MusicLengthInSeconds
    {
        get
        {
            return music.length;
        }
    }

    public static MusicPlayer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource.clip = music;
    }

    public void ToggleSong()
    {
        audioSource.time = (float)MapEditorManager.Instance.CurrentTimeInSeconds;
        
        if (MapEditorManager.Instance.Playing)
            audioSource.Play(); 
        else
            audioSource.Stop();
    }
}
