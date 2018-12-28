using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MapEditorManager))]
public class btnPlay : MonoBehaviour
{
    private Image imageComponent = null;
    [SerializeField]
    private MapEditorManager mapeditorManager;
    private MusicPlayer musicPlayer = null;
    [SerializeField]
    private Sprite playSprite;
    [SerializeField]
    private Sprite pauseSprite;

    private void Start()
    {
        imageComponent = gameObject.GetComponent<Image>();
        musicPlayer = mapeditorManager.gameObject.GetComponent<MusicPlayer>();
        UpdateSprite();
    }

    public void OnPlay()
    {
        mapeditorManager.OnPlay();
        musicPlayer.ToggleSong();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        imageComponent.sprite = mapeditorManager.Playing ? pauseSprite : playSprite;
    }
}
