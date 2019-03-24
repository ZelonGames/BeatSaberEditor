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

    public static Map SetJsonMap(string songName, string difficulty)
    {
        string file = Filebrowser.CustomSongsPath + "/" + songName + "/" + difficulty + ".json";
        if (!File.Exists(file))
            return null;

        string json = File.ReadAllText(file);
        return MapCreator._Map = JsonConvert.DeserializeObject<Map>(json);
    }
}
