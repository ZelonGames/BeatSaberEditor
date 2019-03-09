using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class MapInfo
{
    #region Fields

    public string songName;
    public string songSubName;
    public string authorName;
    public int beatsPerMinute;
    public int previewStartTime;
    public int previewDuration;
    public string coverImagePath;
    public string environmentName;
    public bool oneSaber;

    public List<DifficultyLevel> difficultyLevels;

    [JsonIgnore]
    public DifficultyLevel currentDifficulty;

    #endregion

    public MapInfo(string songName, string songSubName, string authorName, int beatsPerMinute, int previewStartTime, int previewDuration, string coverImagePath, string environmentName, bool oneSaber, List<DifficultyLevel> difficultyLevels)
    {
        this.songName = songName;
        this.songSubName = songSubName;
        this.authorName = authorName;
        this.beatsPerMinute = beatsPerMinute;
        this.previewStartTime = previewStartTime;
        this.previewDuration = previewDuration;
        this.coverImagePath = coverImagePath;
        this.environmentName = environmentName;
        this.oneSaber = oneSaber;
        this.difficultyLevels = difficultyLevels;
    }

    public DifficultyLevel GetDifficulty(string difficultyName)
    {
        return difficultyLevels.Where(x => x.difficulty == difficultyName).FirstOrDefault();
    }
}
