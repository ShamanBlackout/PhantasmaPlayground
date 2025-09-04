using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlaygroundManageScenes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string sceneName;
    void Start()
    {


    }

    /// <summary>
    /// Method to load a scene by name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadNextScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be null or empty.");
            return;
        }

        switch (sceneName)
        {
            case "maingame":
                SceneManager.LoadScene(sceneName);
                Debug.Log("Loading Scene1");
                break;
            case "exit":
#if UNITY_EDITOR

                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                Debug.Log("Exiting the game.");
                break;
            default:
                Debug.LogError($"Scene '{sceneName}' not recognized.");
                return;
        }

    }
}
