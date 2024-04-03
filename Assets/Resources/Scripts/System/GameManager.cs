using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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

    public GameObject Menu;
    GameObject ObjectivePrefab;
    public GameObject ObjectivesObj;

    public GameObject EventSystemObject;

    List<GameObject> Objectives = new();

    public delegate void MoveMode();
    public delegate void RefreshCooldown();
    public delegate void DoNew();
    // does not work...
    //public static RefreshCooldown Refresh => (ref bool b) => b = false;

    // Names of Areas to be used in ´Load´ objects
    public enum Scenenames
    {
        TestingAreaLoading, // Main
        TestingPlayer,
        TestingVision,
        MainBuild
    }

    public Vector3 PlayerSpawn => GameObject.FindWithTag("Respawn").transform.position;

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

        EnteredFrom = PlayerSpawn;

        Menu = MenuManager.Instance.Menu;
        ObjectivesObj = Menu.transform.Find("ObjectiveMenu").gameObject;
        ObjectivePrefab = Resources.Load<GameObject>(@"Prefabs/FolderObjectives/Objective");
    }

    private void Start()
    {
        NewObjective("test1");
        NewObjective("test2");
        NewObjective("test3");
        NewObjective("test4");
        NewObjective("this is a long test to see what happens when the text is too long and it has to wrap it :)");
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

        if (SceneManager.GetActiveScene().name != "MainBuild")
        {
            // move player to spawn
            playerObject.transform.position = PlayerSpawn;
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

    public void NewObjective(string objectiveText)
    {
        // Create new objective
        GameObject newObjective = Instantiate(ObjectivePrefab, ObjectivesObj.transform);

        // Add to list
        Objectives.Add(newObjective);

        // Set position
        newObjective.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -75 - 50 * (Objectives.Count));

        // Set text
        newObjective.GetComponent<TextMeshProUGUI>().text = objectiveText;

        // Update container size
        ObjectivesObj.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100 + 50 * Objectives.Count);
    }

    // TODO: get random enemy target pos for random movement
    public Vector3 GetRandomPos()
    {
        Vector3[] positions = GameObject.FindGameObjectsWithTag("Station").Select(x => x.transform.position).ToArray();

        return positions[Random.Range(0, positions.Length - 1)];
    }
}