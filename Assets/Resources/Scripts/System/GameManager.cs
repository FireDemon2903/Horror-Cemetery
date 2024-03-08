using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance.IsUnityNull()) Debug.LogError("GameManager was null");
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)         { Destroy(this); }
        else                                                { _instance = this; }

        // Add the method to the delegate
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    // Called whenever a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Do smth
    }

    public void LoadScene()
    {
        //SceneManager.LoadScene(0);
    }

}