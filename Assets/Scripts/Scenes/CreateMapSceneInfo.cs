public static class CreateMapSceneInfo
{
    public static string songName = null;
    public static string songSubname = null;
    public static string authorName = null;
    public static int? BPM = null;
    public static int? previewStartTime = null;
    public static int? previewDuration = null;
    public static string coverImage = null;

    public static bool ContainsData
    {
        get
        {
            return songName != null || songSubname != null || authorName != null || BPM.HasValue || previewStartTime.HasValue || previewDuration.HasValue || coverImage != null;
        }
    }
}
