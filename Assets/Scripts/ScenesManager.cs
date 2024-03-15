using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] private string startScene;

    private static string _currentActiveScene;

    public event Action OnStartSceneLoaded;
    public event Action OnNewSceneLoaded;
    
    public static ScenesManager instance;

    private bool isStartSceneLoaded = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    void Start()
    {
        // Launch the starting scene at startup
        SceneManager.LoadScene(startScene, LoadSceneMode.Additive);
        _currentActiveScene = startScene;
        
        SceneManager.sceneLoaded += InvokeSceneLoaded;
    }

    // InvokeEvents when a scene is loaded
    private void InvokeSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isStartSceneLoaded == false)
        {
            OnStartSceneLoaded?.Invoke();
        }
        else
        {
            OnNewSceneLoaded?.Invoke();
        }
    }
    
    // Unload current scene and load new scene
    public void LoadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(_currentActiveScene);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        _currentActiveScene = sceneName;
    }
    
    // called when the game is terminated
    void OnDisable()
    {
        SceneManager.sceneLoaded -= InvokeSceneLoaded;
    }
}
