using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo
{
    #region Fields

    public string songName;
    public string songSubName;
    public string songAuthorName;
    public int beatsPerMinute;
    public int previewStartTime;
    public int previewDuration;
    public string coverImagePath;
    public string environmentName;
    public bool oneSaber;

    public List<DifficultyLevel> difficultyLevels;

    #endregion

    public MapInfo(string songName, string songSubName, string songAuthorName, int beatsPerMinute, int previewStartTime, int previewDuration, string coverImagePath, string environmentName, bool oneSaber, List<DifficultyLevel> difficultyLevels)
    {
        this.songName = songName;
        this.songSubName = songSubName;
        this.songAuthorName = songAuthorName;
        this.beatsPerMinute = beatsPerMinute;
        this.previewStartTime = previewStartTime;
        this.previewDuration = previewDuration;
        this.coverImagePath = coverImagePath;
        this.environmentName = environmentName;
        this.oneSaber = oneSaber;
        this.difficultyLevels = difficultyLevels;
    }
}
