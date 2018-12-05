using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip music;
    [SerializeField]
    private AudioSource audioSource;

    private void Start()
    {
        audioSource.clip = music;
    }

    private void Update()
    {

    }

    public void ToggleSong()
    {
        if (MapEditorManager.Instance.Playing)
            audioSource.Play(); 
        else
            audioSource.Stop();
    }
}
