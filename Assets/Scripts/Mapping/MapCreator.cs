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

    private int defaultBPM = 120;
    private int defaultNJS = 12;

    private bool shouldTriggerOnDifficultyChanged = true;

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
            if (Filebrowser.folder._FilePath != null)
            {
                if (_Map != null && _Map.IsLoaded)
                    _Map.UnLoad();

                if (_MapInfo == null)
                    MapLoader.SetJsonMapInfo(Filebrowser.folder.FileName);
                if (_Map == null)
                    MapLoader.SetJsonMap(_MapInfo.songName, _MapInfo.currentDifficulty.difficulty);
            }

            inputSongName = GameObject.Find("inputSongName").GetComponent<InputField>();
            inputSongSubname = GameObject.Find("inputSongSubname").GetComponent<InputField>();
            inputAuthorName = GameObject.Find("inputAuthorName").GetComponent<InputField>();
            inputBPM = GameObject.Find("inputBPM").GetComponent<InputField>();

            txtImageFileName = GameObject.Find("txtImageFilename").GetComponent<Text>();
            txtAudioFilename = GameObject.Find("txtAudioFilename").GetComponent<Text>();

            dropdownDifficulty = GameObject.Find("dropdownDifficulty").GetComponent<Dropdown>();
            inputStartOffset = GameObject.Find("inputStartOffset").GetComponent<InputField>();
            inputNoteJumpSpeed = GameObject.Find("inputNoteJumpSpeed").GetComponent<InputField>();

            // Happens when creating a new map
            if (_MapInfo == null)
                _MapInfo = new MapInfo();
            if (_Map == null)
                _Map = new Map();

            UpdateInputFields();
        }
        else if (currentSceneName == "EditingScene")
            _Map.Load(notePrefab, bombSpherePrefab, blueCubePrefab, redCubePrefab);
    }

    public void OnUnloadMap()
    {
        _Map.UnLoad();
    }

    public void OnUpdateMap()
    {
        _MapInfo.songName = inputSongName.text;
        _MapInfo.songSubName = inputSongSubname.text;
        _MapInfo.authorName = inputAuthorName.text;
        if (_Map != null)
            _Map._beatsPerMinute = Convert.ToInt32(inputBPM.text);

        if (_MapInfo.currentDifficulty != null)
        {
            _MapInfo.currentDifficulty.offset = StringToInt(inputStartOffset.text);
            _MapInfo.currentDifficulty.audioPath = txtAudioFilename.text;
        }

        if (_Map != null)
            _Map._noteJumpSpeed = StringToInt(inputNoteJumpSpeed.text);
        _MapInfo.coverImagePath = txtImageFileName.text;
    }

    public void OnDifficultyChanged()
    {
        if (!Filebrowser.folder.HasSelectedFile || !shouldTriggerOnDifficultyChanged)
        {
            shouldTriggerOnDifficultyChanged = true;
            return;
        }

        MapLoader.SetJsonMapInfo(Filebrowser.folder.FileName);
        if (MapLoader.SetJsonMap(_MapInfo.songName, GetSelectedDifficultyName()) == null)
            _MapInfo.currentDifficulty = null;

        UpdateInputFields();
    }

    public void OnCreateMap()
    {
        string mapFolderPath = MapFolderPath(inputSongName.text);

        if (!Directory.Exists(mapFolderPath))
            Directory.CreateDirectory(mapFolderPath);

        if (Filebrowser.image.HasSelectedFile)
            File.Copy(Filebrowser.image.NormalPath, mapFolderPath + "/" + txtImageFileName.text, true);
        if (Filebrowser.audio.HasSelectedFile)
            File.Copy(Filebrowser.audio.NormalPath, mapFolderPath + "/" + txtAudioFilename.text, true);

        if (_MapInfo.currentDifficulty == null)
        {
            DifficultyLevel selectedDifficulty = GetNewSelectedDifficulty();
            _MapInfo.difficultyLevels.Add(selectedDifficulty);
            _MapInfo.currentDifficulty = selectedDifficulty;
            _Map = Map.GetNewEmptyMapFrom(_Map);
        }

        _MapInfo.currentDifficulty.offset = Convert.ToInt32(inputStartOffset.text);
        _Map._noteJumpSpeed = Convert.ToInt32(inputNoteJumpSpeed.text);
    }

    #endregion

    private void UpdateInputFields()
    {
        if (_MapInfo == null)
        {
            UpdateInputFieldsToDefault();
            return;
        }

        inputSongName.text = _MapInfo.songName;
        inputSongSubname.text = _MapInfo.songSubName;
        inputAuthorName.text = _MapInfo.authorName;
        inputBPM.text = _Map != null ? _Map._beatsPerMinute.ToString() : defaultBPM.ToString();

        if (_MapInfo.currentDifficulty != null)
        {
            shouldTriggerOnDifficultyChanged = false;
            dropdownDifficulty.value = dropdownDifficulty.options.IndexOf(dropdownDifficulty.options.Where(x => x.text == _MapInfo.currentDifficulty.difficulty).FirstOrDefault());
            inputStartOffset.text = _MapInfo.currentDifficulty.offset.ToString();
            txtAudioFilename.text = Filebrowser.audio.HasSelectedFile ? Filebrowser.audio.FileName : _MapInfo.currentDifficulty.audioPath;
            inputNoteJumpSpeed.text = _Map != null ? _Map._noteJumpSpeed.ToString() : defaultNJS.ToString();
        }
        else
            UpdateInputFieldsToDefault();

        txtImageFileName.text = Filebrowser.image.HasSelectedFile ? Filebrowser.image.FileName : _MapInfo.coverImagePath;
    }

    private void UpdateInputFieldsToDefault()
    {
        inputBPM.text = defaultBPM.ToString();
        inputNoteJumpSpeed.text = defaultNJS.ToString();
        inputStartOffset.text = "0";
        txtAudioFilename.text = "";
        txtImageFileName.text = "";
    }

    public void SaveMapInfo()
    {
        _MapInfo.currentDifficulty.oldOffset = _MapInfo.currentDifficulty.offset;
        string jsonInfo = JsonConvert.SerializeObject(_MapInfo);

        using (StreamWriter wr = new StreamWriter(MapFolderPath(_MapInfo.songName) + "/info.json", false))
            wr.WriteLine(jsonInfo);
    }

    public void SaveMap()
    {
        _Map._notes.ForEach(x => x._time += Map.GetMSInBeats(_Map._beatsPerMinute, _MapInfo.currentDifficulty.offset));
        _MapInfo.currentDifficulty.oldOffset = _MapInfo.currentDifficulty.offset;

        string jsonMap = JsonConvert.SerializeObject(_Map);

        using (StreamWriter wr = new StreamWriter(MapFolderPath(_MapInfo.songName) + "/" + _MapInfo.currentDifficulty.difficulty + ".json", false))
            wr.WriteLine(jsonMap);

        SaveMapInfo();
    }

    private DifficultyLevel GetNewSelectedDifficulty()
    {
        return new DifficultyLevel(GetSelectedDifficultyName(), _MapInfo.difficultyLevels.Count - 1, txtAudioFilename.text, GetSelectedDifficultyName() + ".json", Convert.ToInt32(inputStartOffset.text), 0);
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

    private int StringToInt(string text)
    {
        try
        {
            return Convert.ToInt32(text);
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
