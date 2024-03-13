using System.Collections.Generic;
using System.Linq;
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

    // Names of Areas to be used in �Load� objects
    public enum Scenenames
    {
        TestingAreaLoading, // Main
        TestingPlayer,
        TestingVision
    }

    // List of SubAreas
    public List<Transform> ActiveZoneTransitions;
    // The position of the last zone transition player used
    public Vector3 EnteredFrom;
    //public Quaternion RotationFrom;

    public GameObject playerObject;
    public enum Parts { GunBarrel, GunHandle, GunCyllinder, Gunpowder, Casing }

    Scene OldScene;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this)         { Destroy(gameObject);  return; }
        else                                                { _instance = this; }

        // Add the method to the delegate
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Spawn the player
        playerObject = Instantiate(Resources.Load<GameObject>(@"Prefabs/PlayerPlaceholder"));

        //EnteredFrom = new Vector3(0f, 5f, 0f);
    }

    // Called whenever a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unload the old scene
        if (OldScene.name != null) SceneManager.UnloadSceneAsync(OldScene);

        // Find ZoneTransitions
        SetZones();

        // Move important object to new scene
        move(scene);

        OldScene = scene;
    }

    public void LoadScene(string sceneName, LoadSceneMode mode)
    {
        SceneManager.LoadScene(sceneName, mode);
    }

    public void move(Scene scene)
    {
        SceneManager.MoveGameObjectToScene(gameObject, scene);
        SceneManager.MoveGameObjectToScene(playerObject, scene);

        if (SceneManager.GetActiveScene().name != "TestingAreaLoading")
        {
            // find entrancepoint
            Transform en = GameObject.FindWithTag("Respawn").transform;

            // move playter to entrancepoint
            playerObject.transform.position = en.position;
        }
        else
        {
            // Move player to the place they entered from in main
            playerObject.transform.position = EnteredFrom;
            //playerObject.transform.SetPositionAndRotation(EnteredFrom, RotationFrom);
        }
    }

    public static bool CanCraftItem<Parts>(List<Parts> ownedParts, params Parts[] requiredParts) { return requiredParts.All(part => ownedParts.Contains(part)); }

    void SetZones() { ActiveZoneTransitions.Clear(); ActiveZoneTransitions = GameObject.FindGameObjectsWithTag("ZoneTransition").Select(obj => obj.transform).ToList(); }

}