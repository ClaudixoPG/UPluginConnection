using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    //get the index of the current scene
    public int GetCurrentSceneIndex()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }

    //get the name of the current scene
    public string GetCurrentSceneName()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    //make a function that will load the next scene
    public void LoadNextScene()
    {
        //get the current scene index
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        //load the next scene       
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
       
    }

    //make a function that will load the first scene
    public void LoadFirstScene()
    {
        //load the first scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    //make a function that will load the scene with the given index
    public void LoadScene(int sceneIndex)
    {
        //load the scene with the given index
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
    public void LoadScene(string sceneName)
    {
        //load the scene with the given index
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    //make a function that will quit the game
    public void QuitGame()
    {
        //quit the game
        Application.Quit();
    }
}
