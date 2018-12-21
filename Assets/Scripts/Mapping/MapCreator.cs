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
    private struct MapData
    {
        public string songName;
        public string songSubname;
        public string authorName;
        public int? bpm;
        public int? difficultyValue;
        public int? startOffset;
        public int? noteJumpSpeed;

        public void SetStandard<T>(ref T value, T newValue)
        {
            if (value == null)
                value = newValue;
        }

        public void ResetAll()
        {
            songName = null;
            songSubname = null;
            authorName = null;
            bpm = null;
            difficultyValue = null;
            startOffset = null;
            noteJumpSpeed = null;
        }
    }

    #region Fields

    [SerializeField]
    private Note notePrefab;
    [SerializeField]
    private GameObject blueCubePrefab;
    [SerializeField]
    private GameObject redCubePrefab;
    [SerializeField]
    private GameObject bombSpherePrefab;

    private static MapData mapData = new MapData();

    private InputField inputSongName = null;
    private InputField inputSongSubname = null;
    private InputField inputAuthorName = null;
    private InputField inputBPM = null;

    private Dropdown dropdownDifficulty = null;
    private InputField inputStartOffset = null;
    private InputField inputNoteJumpSpeed = null;

    private Text txtImageFileName = null;
    private Text txtAudioFilename = null;

    private static string selectedDifficultyName;
    private static string jsonInfo;
    private static string jsonMap;
    private Scene currentScene;

    #endregion

    #region Properties

    public static MapInfo _MapInfo { get; set; }
    public static Map _Map { get; set; }
    public static MapCreator Instance { get; private set; }

    #endregion

    private void Awake()
    {
        Instance = this;
        Input.simulateMouseWithTouches = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        Filebrowser.CreateCustomSongsPath();
    }

    private void Start()
    {
        if (currentScene.name == "EditingScene")
        {
            if (_Map != null && _MapInfo != null)
                MapLoader.LoadMap(notePrefab, bombSpherePrefab, blueCubePrefab, redCubePrefab);
        }
    }

    public void OnSceneUnloaded(Scene current)
    {
        if (current.name != "CreateMapScene")
            return;

        UpdateMapData();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene;
        if (scene.name == "CreateMapScene")
        {
            inputSongName = GameObject.Find("inputSongName").GetComponent<InputField>();
            inputSongSubname = GameObject.Find("inputSongSubname").GetComponent<InputField>();
            inputAuthorName = GameObject.Find("inputAuthorName").GetComponent<InputField>();
            inputBPM = GameObject.Find("inputBPM").GetComponent<InputField>();

            dropdownDifficulty = GameObject.Find("dropdownDifficulty").GetComponent<Dropdown>();
            inputStartOffset = GameObject.Find("inputStartOffset").GetComponent<InputField>();
            inputNoteJumpSpeed = GameObject.Find("inputNoteJumpSpeed").GetComponent<InputField>();

            txtImageFileName = GameObject.Find("txtImageFilename").GetComponent<Text>();
            txtAudioFilename = GameObject.Find("txtAudioFilename").GetComponent<Text>();

            txtImageFileName.text = Filebrowser.image.HasSelectedFile ? Filebrowser.image.FileName : ".jpg";
            txtAudioFilename.text = Filebrowser.audio.HasSelectedFile ? Filebrowser.audio.FileName : ".ogg";
            
            UpdateSelectedDifficulty();

            SetStandardMapInfo();
            SetInputFieldsToMapData();
        }
    }

    #region MapData

    private void SetStandardMapInfo()
    {
        mapData.SetStandard(ref mapData.bpm, 120);
        mapData.SetStandard(ref mapData.startOffset, 0);
        mapData.SetStandard(ref mapData.noteJumpSpeed, 12);
        mapData.SetStandard(ref mapData.difficultyValue, 0);

        if (_MapInfo != null)
        {
            mapData.songName = _MapInfo.songName;
            mapData.songSubname = _MapInfo.songSubName;
            mapData.authorName = _MapInfo.songAuthorName;
            mapData.bpm = _MapInfo.beatsPerMinute;
            
            UpdateDifficultyData();
        }
    }

    private void UpdateMapData()
    {
        mapData.songName = inputSongName.text;
        mapData.songSubname = inputSongSubname.text;
        mapData.authorName = inputAuthorName.text;
        mapData.bpm = Convert.ToInt32(inputBPM.text);
        mapData.difficultyValue = dropdownDifficulty.value;
        mapData.startOffset = Convert.ToInt32(inputStartOffset.text);
        mapData.noteJumpSpeed = Convert.ToInt32(inputNoteJumpSpeed.text);
    }

    private void SetInputFieldsToMapData()
    {
        if (mapData.songName != null)
            inputSongName.text = mapData.songName;
        if (mapData.songSubname != null)
            inputSongSubname.text = mapData.songSubname;
        if (mapData.authorName != null)
            inputAuthorName.text = mapData.authorName;
        if (mapData.bpm.HasValue)
            inputBPM.text = mapData.bpm.Value.ToString();
        if (mapData.difficultyValue.HasValue)
            dropdownDifficulty.value = mapData.difficultyValue.Value;
        if (mapData.startOffset.HasValue)
            inputStartOffset.text = mapData.startOffset.Value.ToString();
        if (mapData.noteJumpSpeed.HasValue)
            inputNoteJumpSpeed.text = mapData.noteJumpSpeed.Value.ToString();
    }

    private void UpdateSelectedDifficulty()
    {
        if (_MapInfo == null)
            return;

        selectedDifficultyName = _MapInfo.difficultyLevels.First().difficulty;
        foreach (var option in dropdownDifficulty.options)
        {
            if (option.text == selectedDifficultyName)
            {
                dropdownDifficulty.value = dropdownDifficulty.options.IndexOf(option);
                break;
            }
        }
    }

    public void UpdateDifficultyData()
    {
        string difficultyName = dropdownDifficulty.options[dropdownDifficulty.value].text;
        var jsonMap = MapLoader.GetJsonMap(_MapInfo.songName, difficultyName);
        if (jsonMap == null)
            return;

        _Map = jsonMap;
        DifficultyLevel difficulty = _MapInfo.GetDifficulty(difficultyName);
        txtAudioFilename.text = difficulty.audioPath;
        mapData.startOffset = difficulty.offset;
        mapData.noteJumpSpeed = _Map._noteJumpSpeed;
        inputStartOffset.text = mapData.startOffset.ToString();
        inputNoteJumpSpeed.text = mapData.noteJumpSpeed.ToString();
    }

    #endregion

    public void SaveInfo()
    {
        jsonInfo = JsonConvert.SerializeObject(_MapInfo);

        using (StreamWriter wr = new StreamWriter(MapFolderPath(_MapInfo.songName) + "/info.json", false))
        {
            wr.WriteLine(jsonInfo);
        }
    }

    public void SaveMap()
    {
        JsonSerializeMap();

        using (StreamWriter wr = new StreamWriter(MapFolderPath(_MapInfo.songName) + "/" + _MapInfo.currentDifficulty.difficulty + ".json", false))
        {
            wr.WriteLine(jsonMap);
        }

        SaveInfo();
    }

    private void JsonSerializeMap()
    {
        var jsonNotes = new List<JsonNote>();

        foreach (var note in _Map._notesObjects)
        {
            jsonNotes.Add(new JsonNote(note._time, note._lineIndex, note._lineLayer, (Note.ItemType)note._type, (Note.CutDirection)note._cutDirection));
        }

        _Map._notes = jsonNotes;
        jsonMap = JsonConvert.SerializeObject(_Map);
    }

    public void OnCreateMap()
    {
        UpdateMapData();

        selectedDifficultyName = GetSelectedDifficultyName();

        if (_MapInfo == null)
            _MapInfo = new MapInfo(inputSongName.text, inputSongSubname.text, inputAuthorName.text, Convert.ToInt32(inputBPM.text), 12, 12, "cover.jpg", "DefaultEnvironment", false, new List<DifficultyLevel> { new DifficultyLevel(selectedDifficultyName, 1, "song.ogg", selectedDifficultyName + ".json", 0, 0), });

        if (_Map == null)
            _Map = new Map("1.1", Convert.ToInt32(inputBPM.text), 16, Convert.ToInt32(inputNoteJumpSpeed.text), new List<Note>());


        if (!Directory.Exists(MapFolderPath(inputSongName.text)))
        {
            Directory.CreateDirectory(MapFolderPath(inputSongName.text));
            if (Filebrowser.image.HasSelectedFile)
                File.Copy(Filebrowser.image.NormalPath, MapFolderPath(inputSongName.text) + "/cover.jpg", true);
            if (Filebrowser.audio.HasSelectedFile)
                File.Copy(Filebrowser.audio.NormalPath, MapFolderPath(inputSongName.text) + "/song.ogg", true);
        }

        DifficultyLevel selectedDifficulty = _MapInfo.GetDifficulty(selectedDifficultyName);
        if (selectedDifficulty == null)
        {
            DifficultyLevel lastLevel = _MapInfo.difficultyLevels.Last();
            selectedDifficulty = new DifficultyLevel(selectedDifficultyName, lastLevel.difficultyRank, lastLevel.audioPath, lastLevel.jsonPath, lastLevel.offset, lastLevel.oldOffset);
            _MapInfo.difficultyLevels.Add(selectedDifficulty);
            if (_Map != null)
                _Map.ClearNotes();
        }

        _MapInfo.currentDifficulty = selectedDifficulty;
        _MapInfo.currentDifficulty.offset = mapData.startOffset.Value;
        _Map._noteJumpSpeed = mapData.noteJumpSpeed.Value;

        SaveInfo();
        mapData.ResetAll();
        SceneManager.LoadScene("EditingScene");
    }

    private void UpdateMapInfo(MapInfo mapInfo)
    {
        _MapInfo = new MapInfo(mapInfo.songName, mapInfo.songSubName, mapInfo.songAuthorName, mapInfo.beatsPerMinute, mapInfo.previewDuration, mapInfo.previewStartTime, mapInfo.coverImagePath, mapInfo.environmentName, mapInfo.oneSaber, mapInfo.difficultyLevels);
    }

    private void UpdateMap(Map jsonMap)
    {
        _Map = new Map(jsonMap._version, jsonMap._beatsPerMinute, jsonMap._beatsPerBar, jsonMap._noteJumpSpeed, jsonMap._notesObjects);
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
