using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static string sceneToLoad = "";

    [SerializeField]
    private TextMeshProUGUI loadingText;

    private void Start()
    {
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
