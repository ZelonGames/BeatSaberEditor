public static class CreateMapSceneInfo
{
    public static string songName;
    public static string songSubname;
    public static string authorName;
    public static int? BPM = null;
    public static int? previewStartTime;
    public static int? previewDuration;
    public static string coverImage;

    public static string difficulty;
    public static string audioFileName;
    public static int? difficultyValue;
    public static int? startOffset;
    public static int? noteJumpSpeed;

    public static bool ContainsData
    {
        get
        {
            return songName != string.Empty || songSubname != string.Empty || authorName != string.Empty || BPM.HasValue || previewStartTime.HasValue || previewDuration.HasValue || coverImage != string.Empty ||
                difficulty != string.Empty || audioFileName != string.Empty || startOffset.HasValue || noteJumpSpeed.HasValue;
        }
    }
}
