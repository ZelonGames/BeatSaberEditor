using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private bool loadingScreen;

    public void LoadScene(string scene)
    {
        if (loadingScreen)
        {
            LoadingScreen.sceneToLoad = scene;
            SceneManager.LoadScene("LoadingScene");
        }
        else
            SceneManager.LoadScene(scene);
    }
}
