using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class MapLoader
{
    public static void SetJsonMapInfo(string songName)
    {
        string file = Filebrowser.CustomSongsPath + "/" + songName + "/info.json";
        if (!File.Exists(file))
            return;

        string json = File.ReadAllText(file);
        MapCreator._MapInfo = JsonConvert.DeserializeObject<MapInfo>(json);
        MapCreator._MapInfo.currentDifficulty = MapCreator._MapInfo.difficultyLevels.Last();
    }

    public static void SetJsonMap(string songName, string difficulty)
    {
        string file = Filebrowser.CustomSongsPath + "/" + songName + "/" + difficulty + ".json";
        if (!File.Exists(file))
            return;

        string json = File.ReadAllText(file);
        MapCreator._Map = JsonConvert.DeserializeObject<Map>(json);
    }

    public static void LoadMap(Note notePrefab, GameObject bombPrefab, GameObject blueCubePrefab, GameObject redCubePrefab)
    {
        foreach (var note in MapCreator._Map._notes)
            MapCreator._Map.AddNote(notePrefab, bombPrefab, blueCubePrefab, redCubePrefab, (Note.CutDirection)note._cutDirection, new Vector2Int((int)note._lineIndex, (int)note._lineLayer), note._time, (Note.ItemType)note._type, false);
    }
}
