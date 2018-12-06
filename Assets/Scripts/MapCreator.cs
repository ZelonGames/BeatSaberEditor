using System;
using System.IO;
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

    private Dropdown dropdownDifficulty = null;
    private InputField inputAudioFileName = null;
    private InputField inputStartOffset = null;
    private InputField inputNoteJumpSpeed = null;

    private string jsonInfo;
    private string jsonMap;

    #endregion

    #region Properties

    public MapInfo _MapInfo { get; private set; }

    private string MapFolderPath
    {
        get
        {
            return Application.persistentDataPath + "/CustomSongs/" + CreateMapSceneInfo.songName;
        }
    }

    public static Map _Map { get; private set; }
    public static MapCreator Instance { get; private set; }

    #endregion

    private void Awake()
    {
        Instance = this;
        Input.simulateMouseWithTouches = true;
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (!Directory.Exists(Application.persistentDataPath + "/CustomSongs"))
            Directory.CreateDirectory(Application.persistentDataPath + "/CustomSongs");
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

        dropdownDifficulty = GameObject.Find("dropdownDifficulty").GetComponent<Dropdown>();
        inputAudioFileName = GameObject.Find("inputAudioFileName").GetComponent<InputField>();
        inputStartOffset = GameObject.Find("inputStartOffset").GetComponent<InputField>();
        inputNoteJumpSpeed = GameObject.Find("inputNoteJumpSpeed").GetComponent<InputField>();

        if (CreateMapSceneInfo.ContainsData)
            UpdateInputFields();
    }

    public void SaveInfo()
    {
        using (StreamWriter wr = new StreamWriter(MapFolderPath + "/info.json", true))
        {
            wr.WriteLine(jsonInfo);
        }
    }

    public void SaveMap()
    {
        using (StreamWriter wr = new StreamWriter(MapFolderPath + "/" + CreateMapSceneInfo.difficulty + ".json", true))
        {
            wr.WriteLine(jsonInfo);
        }
    }

    public void Create()
    {
        try
        {
            UpdateCreateMapSceneInfo();
        }
        catch
        {

        }

        if (CreateMapSceneInfo.ContainsData)
        {
            CreateMap(CreateMapSceneInfo.songName, CreateMapSceneInfo.songSubname, CreateMapSceneInfo.authorName,
                CreateMapSceneInfo.BPM.Value, CreateMapSceneInfo.previewStartTime.Value, CreateMapSceneInfo.previewDuration.Value, CreateMapSceneInfo.coverImage,
                "DefaultEnvironment", false, new List<DifficultyLevel> { new DifficultyLevel(dropdownDifficulty.options[dropdownDifficulty.value].text, 1, inputAudioFileName.text, dropdownDifficulty.options[dropdownDifficulty.value].text + ".json", 0, 0), });

            SceneManager.LoadScene("EditingScene");
        }
    }

    private void UpdateCreateMapSceneInfo()
    {
        CreateMapSceneInfo.songName = inputSongName.text;
        CreateMapSceneInfo.songSubname = inputSongSubname.text;
        CreateMapSceneInfo.authorName = inputAuthorName.text;
        CreateMapSceneInfo.BPM = Convert.ToInt32(inputBPM.text);
        CreateMapSceneInfo.previewStartTime = 12;// Convert.ToInt32(inputPreviewStartTime.text);
        CreateMapSceneInfo.previewDuration = 12;// Convert.ToInt32(inputPreviewDuration.text);
        CreateMapSceneInfo.coverImage = inputCoverImage.text;

        CreateMapSceneInfo.difficulty = dropdownDifficulty.options[dropdownDifficulty.value].text;
        CreateMapSceneInfo.difficultyValue = dropdownDifficulty.value;
        CreateMapSceneInfo.startOffset = Convert.ToInt32(inputStartOffset.text);
        CreateMapSceneInfo.noteJumpSpeed = Convert.ToInt32(inputNoteJumpSpeed.text);
    }

    private void UpdateInputFields()
    {
        inputSongName.text = CreateMapSceneInfo.songName;
        inputSongSubname.text = CreateMapSceneInfo.songSubname;
        inputAuthorName.text = CreateMapSceneInfo.authorName;
        inputBPM.text = CreateMapSceneInfo.BPM.ToString();
        //inputPreviewStartTime.text = CreateMapSceneInfo.previewStartTime.ToString();
        //inputPreviewDuration.text = CreateMapSceneInfo.previewDuration.ToString();
        inputCoverImage.text = CreateMapSceneInfo.coverImage;

        dropdownDifficulty.value = CreateMapSceneInfo.difficultyValue.Value;
        inputAudioFileName.text = CreateMapSceneInfo.audioFileName;
        inputStartOffset.text = CreateMapSceneInfo.startOffset.Value.ToString();
        inputNoteJumpSpeed.text = CreateMapSceneInfo.noteJumpSpeed.Value.ToString();
    }

    public void CreateMap(string songName, string songSubName, string authorName, int beatsPerMinute, int previewStartTime, int previewDuration, string coverImagePath, string environmentName, bool oneSaber, List<DifficultyLevel> difficultyLevels)
    {
        _MapInfo = new MapInfo(songName, songSubName, authorName, beatsPerMinute, previewStartTime, previewDuration, coverImagePath, environmentName, oneSaber, difficultyLevels);
        _Map = new Map("1.1", beatsPerMinute, 16, 10, new List<Note>());

        jsonInfo = JsonConvert.SerializeObject(_MapInfo);
        jsonMap = JsonConvert.SerializeObject(_Map);

        if (!Directory.Exists(MapFolderPath))
            Directory.CreateDirectory(MapFolderPath);

        SaveInfo();
    }
}
