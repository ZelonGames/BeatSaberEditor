using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class MapLoader
{
    public static MapInfo GetMapInfo(string songName)
    {
        string file = Filebrowser.CustomSongsPath + "/" + songName + "/info.json";
        if (!File.Exists(file))
            return null;

        string json = File.ReadAllText(file);
        MapCreator._MapInfo = JsonConvert.DeserializeObject<MapInfo>(json);
        return MapCreator._MapInfo;
    }

    public static Map GetJsonMap(string songName, string difficulty)
    {
        string file = Filebrowser.CustomSongsPath + "/" + songName + "/" + difficulty + ".json";
        if (!File.Exists(file))
            return null;

        string json = File.ReadAllText(file);
        MapCreator._Map = JsonConvert.DeserializeObject<Map>(json);
        MapCreator._Map._notes = new List<JsonNote>();
        foreach(var note in MapCreator._Map._notesObjects)
            MapCreator._Map._notes.Add(new JsonNote(note._time, note._lineIndex, note._lineLayer, (Note.ColorType)note._type, (Note.CutDirection)note._cutDirection));

        MapCreator._Map._notesObjects.Clear();
        return MapCreator._Map;
    }

    public static void LoadMap(Note notePrefab, GameObject blueCubePrefab, GameObject redCubePrefab)
    {
        if (MapCreator._Map._notes == null)
            return;

        foreach (var note in MapCreator._Map._notes)
            MapCreator._Map.AddNote(notePrefab, blueCubePrefab, redCubePrefab, (Note.CutDirection)note._cutDirection, new Vector2Int(note._lineIndex, note._lineLayer), note._time, (Note.ColorType)note._type, false);
    }
}
