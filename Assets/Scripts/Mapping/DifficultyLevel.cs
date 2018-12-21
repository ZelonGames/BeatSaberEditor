using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DifficultyLevel
{
    public string difficulty;
    public int difficultyRank;
    public string audioPath;
    public string jsonPath;
    public int offset;
    public int oldOffset;

    public DifficultyLevel(string difficulty, int difficultyRank, string audioPath, string jsonPath, int offset, int oldOffset)
    {
        this.difficulty = difficulty;
        this.difficultyRank = difficultyRank;
        this.audioPath = audioPath;
        this.jsonPath = jsonPath;
        this.offset = offset;
        this.oldOffset = oldOffset;
    }
}
