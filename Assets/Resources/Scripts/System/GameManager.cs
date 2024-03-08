using Unity.Collections;
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

    Transform MainSpawn;
    Transform[] SubareaSpawnsInMain;

    public GameObject playerObject;

    Scene OldScene;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)         { Destroy(gameObject);  return; }
        else                                                { _instance = this; }

        // Add the method to the delegate
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Spawn the player
        playerObject = Instantiate(Resources.Load<GameObject>(@"Prefabs/PlayerPlaceholder"), new Vector3(0f, 5f, 0f), Quaternion.identity);
    }

    // Called whenever a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Move important object to new scene
        move(scene);

        // Unload the old scene
        if (OldScene.name != null) SceneManager.UnloadSceneAsync(OldScene);

        OldScene = scene;
    }

    public void LoadScene(string sceneName, LoadSceneMode mode)
    {
        //move(SceneManager.GetSceneByName(sceneName));
        SceneManager.LoadScene(sceneName, mode);
    }

    public void move(Scene scene)
    {
        SceneManager.MoveGameObjectToScene(gameObject, scene);
        SceneManager.MoveGameObjectToScene(playerObject, scene);
    }



}