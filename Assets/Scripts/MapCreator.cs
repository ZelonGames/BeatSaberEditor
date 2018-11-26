using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MapCreator : MonoBehaviour
{
    #region Fields

    private InputField inputSongName = null;
    private InputField inputSongSubname = null;
    private InputField inputAuthorName = null;
    private InputField inputBPM = null;
    private InputField inputPreviewStartTime = null;
    private InputField inputPreviewDuration = null;
    private InputField inputCoverImage = null;

    #endregion

    #region Properties

    public MapInfo _MapInfo { get; private set; }
    public static Map _Map { get; private set; }

    public static MapCreator Instance { get; private set; }

    #endregion

    private void Awake()
    {
        Instance = this;
        Input.simulateMouseWithTouches = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "CreateMapScene")
            return;

        inputSongName = GameObject.Find("inputSongName").GetComponent<InputField>();
        inputSongSubname = GameObject.Find("inputSongSubname").GetComponent<InputField>();
        inputAuthorName = GameObject.Find("inputAuthorName").GetComponent<InputField>();
        inputBPM = GameObject.Find("inputBPM").GetComponent<InputField>();
        //inputPreviewStartTime = GameObject.Find("inputPreviewStartTime").GetComponent<InputField>();
        //inputPreviewDuration = GameObject.Find("inputPreviewDuration").GetComponent<InputField>();
        inputCoverImage = GameObject.Find("inputCoverImage").GetComponent<InputField>();

        if (CreateMapSceneInfo.ContainsData)
        {
            inputSongName.text = CreateMapSceneInfo.songName;
            inputSongSubname.text = CreateMapSceneInfo.songSubname;
            inputAuthorName.text = CreateMapSceneInfo.authorName;
            inputBPM.text = CreateMapSceneInfo.BPM.ToString();
            //inputPreviewStartTime.text = CreateMapSceneInfo.previewStartTime.ToString();
            //inputPreviewDuration.text = CreateMapSceneInfo.previewDuration.ToString();
            inputCoverImage.text = CreateMapSceneInfo.coverImage;
        }
    }

    private void Start()
    {

    }

    public void Create()
    {
        CreateMapSceneInfo.songName = inputSongName.text;
        CreateMapSceneInfo.songSubname = inputSongSubname.text;
        CreateMapSceneInfo.authorName = inputAuthorName.text;
        CreateMapSceneInfo.BPM = Convert.ToInt32(inputBPM.text);
        CreateMapSceneInfo.previewStartTime = 12;// Convert.ToInt32(inputPreviewStartTime.text);
        CreateMapSceneInfo.previewDuration = 12;// Convert.ToInt32(inputPreviewDuration.text);
        CreateMapSceneInfo.coverImage = inputCoverImage.text;

        CreateMap(CreateMapSceneInfo.songName, CreateMapSceneInfo.songSubname, CreateMapSceneInfo.authorName,
            CreateMapSceneInfo.BPM.Value, CreateMapSceneInfo.previewStartTime.Value, CreateMapSceneInfo.previewDuration.Value, CreateMapSceneInfo.coverImage, 
            "DefaultEnvironment", false, new List<DifficultyLevel> { new DifficultyLevel("Expert", 1, "song.ogg", "Expert.json", 0, 0), new DifficultyLevel("Expert", 1, "song.ogg", "Expert.json", 0, 0), });
        SceneManager.LoadScene("EditingScene");
    }

    public void CreateMap(string songName, string songSubName, string authorName, int beatsPerMinute, int previewStartTime, int previewDuration, string coverImagePath, string environmentName, bool oneSaber, List<DifficultyLevel> difficultyLevels)
    {
        _MapInfo = new MapInfo(songName, songSubName, authorName, beatsPerMinute, previewStartTime, previewDuration, coverImagePath, environmentName, oneSaber, difficultyLevels);
        _Map = new Map("1.1", beatsPerMinute, 16, 10, new List<Note>());

        string jsonData = JsonConvert.SerializeObject(_MapInfo);
        string mapJson = JsonConvert.SerializeObject(_Map);
    }
}
