using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ItemButton : MonoBehaviour
{
    private Filebrowser.Path pathHelper = new Filebrowser.Path();

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
            LoadingScreen.sceneToLoad = "CreateMapScene";
            SceneManager.LoadScene("LoadingScene");
            return;
        }
        else if (Filebrowser.pathType == Filebrowser.PathType.Folder)
        {
            Filebrowser.folder.SetPath(path);
            LoadingScreen.sceneToLoad = "CreateMapScene";
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            filebrowser.paths.Add(path);
            ShowNewDirectories(path);
        }
    }

    public void PrevDirectory()
    {
        if (filebrowser.paths.Count == 1)
            return;

        filebrowser.paths.Remove(filebrowser.paths.Last());
        ShowNewDirectories(filebrowser.paths.Last());
    }

    public void Delete()
    {
        if (!(isFile || filebrowser.browseCustomSongs))
            return;

        pathHelper.SetPath(path);

        CustomDIalogBox dialogBox = null;

        if (isFile)
            dialogBox = CustomDIalogBox.Show(pathHelper.FileName, 
                "Are you sure you want to delete the file: " + "\"" + pathHelper.FileName + "\"?");
        else if (filebrowser.browseCustomSongs)
            dialogBox = CustomDIalogBox.Show(pathHelper.FileName, 
                "Are you sure you want to delete the map: " + "\"" + pathHelper.FileName + "\"?");

        StartCoroutine(WaitForAnswer(dialogBox));
    }

    private IEnumerator WaitForAnswer(CustomDIalogBox dialogBox)
    {
        while (dialogBox.Answer.Equals(CustomDIalogBox.AnswerState.Thinking))
            yield return null;

        if (dialogBox.Answer.Equals(CustomDIalogBox.AnswerState.Yes))
        {
            if (isFile)
                File.Delete(path);
            else if (filebrowser.browseCustomSongs)
            {
                string[] files = Directory.GetFiles(path);
                foreach (var file in files)
                    File.Delete(file);

                Directory.Delete(path);
            }

            GameObject.Destroy(gameObject);
        }
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
