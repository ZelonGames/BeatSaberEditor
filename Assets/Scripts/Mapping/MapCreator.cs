using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class MapCreator : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Note notePrefab;
    [SerializeField]
    private GameObject blueCubePrefab;
    [SerializeField]
    private GameObject redCubePrefab;
    [SerializeField]
    private GameObject bombSpherePrefab;

    private InputField inputSongName = null;
    private InputField inputSongSubname = null;
    private InputField inputAuthorName = null;
    private InputField inputBPM = null;

    private Dropdown dropdownDifficulty = null;
    private InputField inputStartOffset = null;
    private InputField inputNoteJumpSpeed = null;

    private Text txtImageFileName = null;
    private Text txtAudioFilename = null;

    [SerializeField]
    private string currentSceneName;

    #endregion

    #region Properties

    public static MapInfo _MapInfo { get; set; }
    public static Map _Map { get; set; }
    public static MapCreator Instance { get; private set; }

    #endregion

    #region Events

    private void Awake()
    {
        Instance = this;
        Input.simulateMouseWithTouches = true;
        Filebrowser.CreateCustomSongsPath();
    }

    private void Start()
    {
        if (currentSceneName == "CreateMapScene")
        {
            inputSongName = GameObject.Find("inputSongName").GetComponent<InputField>();
            inputSongSubname = GameObject.Find("inputSongSubname").GetComponent<InputField>();
            inputAuthorName = GameObject.Find("inputAuthorName").GetComponent<InputField>();
            inputBPM = GameObject.Find("inputBPM").GetComponent<InputField>();

            txtImageFileName = GameObject.Find("txtImageFilename").GetComponent<Text>();
            txtAudioFilename = GameObject.Find("txtAudioFilename").GetComponent<Text>();

            dropdownDifficulty = GameObject.Find("dropdownDifficulty").GetComponent<Dropdown>();
            inputStartOffset = GameObject.Find("inputStartOffset").GetComponent<InputField>();
            inputNoteJumpSpeed = GameObject.Find("inputNoteJumpSpeed").GetComponent<InputField>();

            if (Filebrowser.folder._FilePath != null)
            {
                MapLoader.SetJsonMapInfo(Filebrowser.folder.FileName);
                MapLoader.SetJsonMap(_MapInfo.songName, _MapInfo.currentDifficulty.difficulty);
            }

            inputSongName.text = _MapInfo.songName;
            inputSongSubname.text = _MapInfo.songSubName;
            inputAuthorName.text = _MapInfo.authorName;
            inputBPM.text = _Map._beatsPerMinute.ToString();

            txtImageFileName.text = Filebrowser.image.HasSelectedFile ? Filebrowser.image.FileName : ".jpg";
            txtAudioFilename.text = Filebrowser.audio.HasSelectedFile ? Filebrowser.audio.FileName : ".ogg";

            dropdownDifficulty.value = dropdownDifficulty.options.IndexOf(dropdownDifficulty.options.Where(x => x.text == _MapInfo.currentDifficulty.difficulty).First());
            inputStartOffset.text = _MapInfo.currentDifficulty.offset.ToString();
            inputNoteJumpSpeed.text = _Map._noteJumpSpeed.ToString();

        }
        else if (currentSceneName == "EditingScene")
        {
            if (_Map != null && _MapInfo != null)
                MapLoader.LoadMap(notePrefab, bombSpherePrefab, blueCubePrefab, redCubePrefab);
        }
    }

    public void OnCreateMap()
    {
        string mapFolderPath = MapFolderPath(inputSongName.text);

        if (!Directory.Exists(mapFolderPath))
        {
            Directory.CreateDirectory(mapFolderPath);
            if (Filebrowser.image.HasSelectedFile)
                File.Copy(Filebrowser.image.NormalPath, mapFolderPath + "/cover.jpg", true);
            if (Filebrowser.audio.HasSelectedFile)
                File.Copy(Filebrowser.audio.NormalPath, mapFolderPath + "/song.ogg", true);
        }

        DifficultyLevel selectedDifficulty = _MapInfo.GetDifficulty(dropdownDifficulty.options[dropdownDifficulty.value].text);
        _MapInfo.currentDifficulty = selectedDifficulty;
        _MapInfo.currentDifficulty.offset = Convert.ToInt32(inputStartOffset.text);
        _Map._noteJumpSpeed = Convert.ToInt32(inputNoteJumpSpeed.text);
    }

    #endregion

    public void SaveInfo()
    {
        _MapInfo.currentDifficulty.oldOffset = _MapInfo.currentDifficulty.offset;
        string jsonInfo = JsonConvert.SerializeObject(_MapInfo);

        using (StreamWriter wr = new StreamWriter(MapFolderPath(_MapInfo.songName) + "/info.json", false))
            wr.WriteLine(jsonInfo);
    }

    public void SaveMap()
    {
        _Map._notes.ForEach(x => x._time += Map.GetMSInBeats(_Map._beatsPerMinute, _MapInfo.currentDifficulty.offset));

        string jsonMap = JsonConvert.SerializeObject(_Map);

        using (StreamWriter wr = new StreamWriter(MapFolderPath(_MapInfo.songName) + "/" + _MapInfo.currentDifficulty.difficulty + ".json", false))
            wr.WriteLine(jsonMap);

        SaveInfo();
    }

    public string MapFolderPath(string songName)
    {
        return Application.persistentDataPath + "/CustomSongs/" + songName;
    }

    private string GetSelectedDifficultyName()
    {
        return dropdownDifficulty.options[dropdownDifficulty.value].text;
    }
    private string GetSelectedDifficultyName(int value)
    {
        return dropdownDifficulty.options[value].text;
    }
}
