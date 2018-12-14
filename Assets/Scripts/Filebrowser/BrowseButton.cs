using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrowseButton : MonoBehaviour
{
    [SerializeField]
    private Filebrowser.PathType fileType;

    public void Browse()
    {
        Filebrowser.pathType = fileType;
        SceneManager.LoadScene("FilebrowserScene");
    }
}
