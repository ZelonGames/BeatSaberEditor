using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Filebrowser : MonoBehaviour
{
    public enum PathType
    {
        ImageFile,
        AudioFile,
        Folder,
    }

    public struct Path
    {
        public string _FilePath { get; private set; }

        public string NormalPath
        {
            get
            {
                return _FilePath.Replace("file://", "");
            }
        }

        public string FileName
        {
            get
            {
                return NormalPath.Split('/').Last().Split('\\').Last();
            }
        }

        public bool HasSelectedFile
        {
            get
            {
                return _FilePath != null;
            }
        }

        public void SetPath(string path)
        {
            _FilePath = "file://" + path;
        }

        public void ResetPath()
        {
            _FilePath = null;
        }
    }

    #region Fields

    [HideInInspector]
    public List<string> paths = new List<string>();

    public static PathType pathType = PathType.AudioFile;

    private Rect canvasRect;
    private Rect scrollViewRect;

    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject scrollViewContent;
    [SerializeField]
    private GameObject itemButtonPrefab;
    [SerializeField]
    private TextMeshProUGUI txtTitle;

    private string[] files;
    private string[] folders;

    [HideInInspector]
    public string currentPath = "";

    public bool browseCustomSongs = false;

    public static Path folder = new Path();
    public static Path image = new Path();
    public static Path audio = new Path();

    #endregion

    public static string CustomSongsPath
    {
        get
        {
            return Application.persistentDataPath + "/CustomSongs";
        }
    }

    public static void CreateCustomSongsPath()
    {
        if (!Directory.Exists(CustomSongsPath))
            Directory.CreateDirectory(CustomSongsPath);
    }

    private void Start()
    {
        if (browseCustomSongs)
            pathType = PathType.Folder;

        switch (pathType)
        {
            case PathType.ImageFile:
                txtTitle.text = "Select Image";
                break;
            case PathType.AudioFile:
                txtTitle.text = "Select Song";
                break;
            case PathType.Folder:
                txtTitle.text = "Song List";
                break;
            default:
                break;
        }
        CreateCustomSongsPath();

        currentPath = browseCustomSongs ? CustomSongsPath : "/";
        paths.Add(currentPath);
        canvasRect = canvas.GetComponent<RectTransform>().rect;
        scrollViewRect = scrollViewContent.GetComponent<RectTransform>().rect;
        ShowDirectoriesAndFiles(currentPath);
    }

    public static void ResetAllPaths()
    {
        folder.ResetPath();
        image.ResetPath();
        audio.ResetPath();
    }

    public void ShowDirectoriesAndFiles(string directory)
    {
        try
        {
            if (!browseCustomSongs)
                AddFiles();

            folders = Directory.GetDirectories(directory);
            for (int i = 0; i < folders.Length; i++)
            {
                try
                {
                    Directory.GetDirectories(folders[i]);
                    InstantiateItemButton(folders[i]);
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            InstantiateItemButton(ex.ToString());
        }
    }

    private void AddFiles()
    {
        switch (pathType)
        {
            case PathType.ImageFile:
                files = Directory.GetFiles(currentPath, "*.jpg");
                break;
            case PathType.AudioFile:
                files = Directory.GetFiles(currentPath, "*.ogg");
                break;
            default:
                break;
        }

        InstantiateButtonFiles();
    }

    private void InstantiateButtonFiles()
    {
        for (int i = 0; i < files.Length; i++)
            InstantiateItemButton(files[i], true);
    }

    private void InstantiateItemButton(string path, bool file = false)
    {
        GameObject button = Instantiate(itemButtonPrefab);
        button.transform.SetParent(scrollViewContent.transform);

        var buttonRect = button.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(scrollViewRect.width, buttonRect.rect.height);

        ItemButton item = button.GetComponentInChildren<ItemButton>();
        item.path = path;

        var btnText = button.GetComponentInChildren<Text>();
        var folders = path.Split('/');
        btnText.text = path.Split('/').Last().Split('\\').Last();

        button.tag = "Directory";

        if (file)
        {
            item.isFile = true;
            btnText.color = Color.white;
        }
    }
}
