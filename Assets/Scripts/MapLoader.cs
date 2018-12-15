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
        {
            MapCreator._Map._notes.Add(new JsonNote(note._time, note._lineIndex, note._lineLayer, (Note.ColorType)note._type, (Note.CutDirection)note._cutDirection));
        }
        MapCreator._Map._notesObjects.Clear();
        return MapCreator._Map;
    }

    public static void LoadMap(Note notePrefab, GameObject blueCubePrefab, GameObject redCubePrefab)
    {
        foreach (var note in MapCreator._Map._notes)
            LoadNote(notePrefab, blueCubePrefab, redCubePrefab, (Note.CutDirection)note._cutDirection, new Vector2Int(note._lineIndex, note._lineLayer), note._time, (Note.ColorType)note._type);
    }

    private static void LoadNote(Note notePrefab, GameObject blueCubePrefab, GameObject redCubePrefab, Note.CutDirection cutDirection, Vector2Int tileCoordinate, double time, Note.ColorType color)
    {
        var note = MapCreator._Map.AddNote(notePrefab, cutDirection, tileCoordinate, time, color);

        GameObject arrowCube = color == Note.ColorType.Blue ? GameObject.Instantiate(blueCubePrefab) : GameObject.Instantiate(redCubePrefab);

        Vector2 coordinate = _3DGridGenerator.Instance.GetCoordinatePosition(new Vector2Int(note._lineIndex, note._lineLayer), arrowCube);

        arrowCube.transform.position = new Vector3(coordinate.x, (float)_3DGridGenerator.Instance.GetBeatPosition(time), coordinate.y);
        arrowCube.transform.Rotate(new Vector3(0, 0, -1), CutDirection.GetAngle((Note.CutDirection)note._cutDirection).Value);
        arrowCube.transform.SetParent(GameObject.FindGameObjectWithTag("3DCanvas").transform, false);

        note.arrowCube = arrowCube;
    }
}
