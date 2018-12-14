using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ItemButton : MonoBehaviour
{
    private Filebrowser filebrowser;
    
    [HideInInspector]
    public string path;

    public bool isFile = false;

    private void Start()
    {
        filebrowser = GameObject.Find("Filebrowser").GetComponent<Filebrowser>();
    }

    public void NextDirectory()
    {
        if (isFile)
        {
            switch (Filebrowser.pathType)
            {
                case Filebrowser.PathType.ImageFile:
                    Filebrowser.image.SetPath(path);
                    break;
                case Filebrowser.PathType.AudioFile:
                    Filebrowser.audio.SetPath(path);
                    break;
                default:
                    break;
            }
            SceneManager.LoadScene("CreateMapScene");
            return;
        }
        else if (Filebrowser.pathType == Filebrowser.PathType.Folder)
        {
            Filebrowser.folder.SetPath(path);
            var mapInfo = MapLoader.GetMapInfo(Filebrowser.folder.FileName);
            SceneManager.LoadScene("CreateMapScene");
        }

        filebrowser.paths.Add(path);
        ShowNewDirectories(path);
    }

    public void PrevDirectory()
    {
        if (filebrowser.paths.Count == 1)
            return;

        filebrowser.paths.Remove(filebrowser.paths.Last());
        ShowNewDirectories(filebrowser.paths.Last());
    }

    public void ShowNewDirectories(string path)
    {
        filebrowser.currentPath = path;

        GameObject[] directories = GameObject.FindGameObjectsWithTag("Directory");
        foreach (var directory in directories)
            Destroy(directory);

        filebrowser.ShowDirectoriesAndFiles(path);
    }
}
