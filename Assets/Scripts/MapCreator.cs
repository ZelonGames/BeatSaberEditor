using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class MapCreator : MonoBehaviour
{
    public MapInfo _MapInfo { get; private set; }
    public Map _Map { get; private set; }

    public void Create()
    {
        CreateMap("Apa", "Gris", "Gorilla", 120, 12, 12, "cover.jpg", "niceEnvironment", false, new List<DifficultyLevel> { new DifficultyLevel("Expert", 1, "song.ogg", "Expert.json", 0, 0), new DifficultyLevel("Expert", 1, "song.ogg", "Expert.json", 0, 0), });
    }

    public void CreateMap(string songName, string songSubName, string authorName, int beatsPerMinute, int previewStartTime, int previewDuration, string coverImagePath, string environmentName, bool oneSaber, List<DifficultyLevel> difficultyLevels)
    {
        _MapInfo = new MapInfo(songName, songSubName, authorName, beatsPerMinute, previewStartTime, previewDuration, coverImagePath, environmentName, oneSaber, difficultyLevels);
        _Map = new Map("1.1", beatsPerMinute, 16, 10, new List<Note>());
        _Map.AddNote(1, Note.ColorType.Blue, Note.CutDirection.Down);
        _Map.AddNote(1.5f, Note.ColorType.Blue, Note.CutDirection.DownLeft);
        _Map.AddNote(2, Note.ColorType.Blue, Note.CutDirection.Left);
        _Map.AddNote(2.5f, Note.ColorType.Blue, Note.CutDirection.UpLeft);
        _Map.AddNote(3, Note.ColorType.Blue, Note.CutDirection.Up);

        string jsonData = JsonConvert.SerializeObject(_MapInfo);
        string mapJson = JsonConvert.SerializeObject(_Map);
    }
}
