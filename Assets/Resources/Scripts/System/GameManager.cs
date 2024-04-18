using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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

    public delegate void MoveMode();
    public delegate void RefreshCooldown();

    public delegate bool ObjectiveCondition();
    public delegate bool t(string str);
    public event Action OnObjectiveCompleted;
    readonly List<GameObject> Objectives = new();

    Transform[] positions;

    // Names of Areas to be used in ´Load´ objects
    public enum Scenenames
    {
        TestingAreaLoading,
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

    public enum Parts { GunBarrel, GunHandle, GunCyllinder, Gunpowder, Casing }

    Scene OldScene;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        else { _instance = this; }

        DontDestroyOnLoad(gameObject);

        // Add the method to the delegate
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Spawn the player
        Instantiate(Resources.Load<GameObject>(@"Prefabs/PlayerVariant"));
        DontDestroyOnLoad(PlayerController.Instance.gameObject);

        EnteredFrom = PlayerSpawn;

        Menu = MenuManager.Instance.Menu;
        ObjectivesObj = Menu.transform.Find("ObjectiveMenu").gameObject;
        ObjectivePrefab = Resources.Load<GameObject>(@"Prefabs/FolderObjectives/Objective");
    }

    private void Update()
    {
        // check through the conditions
        OnObjectiveCompleted?.Invoke();

    }

    private void Start()
    {
        NewObjective("Find gun barrel", () => { return PlayerController.Instance.OwnedParts.Contains(Parts.GunBarrel); });
        NewObjective("Find gun cylinder", () => { return PlayerController.Instance.OwnedParts.Contains(Parts.GunCyllinder); });
        NewObjective("Find gun handle", () => { return PlayerController.Instance.OwnedParts.Contains(Parts.GunHandle); });
        NewObjective("Find bullet parts", () => { return CanCraftItem(PlayerController.Instance.OwnedParts, Parts.Gunpowder, Parts.Casing); });
        //NewObjective("Find notes")
        //NewObjective("")
    }

    // Called whenever a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unload the old scene
        if (OldScene.name != null) SceneManager.UnloadSceneAsync(OldScene);

        // Find ZoneTransitions
        SetZones();

        positions = GameObject.FindGameObjectsWithTag("Station").Select(x => x.transform).ToArray();

        // Move important object to new scene
        StartCoroutine(MoveToNewScene(scene));

        OldScene = scene;
    }

    public void LoadScene(string sceneName, LoadSceneMode mode)
    {
        SceneManager.LoadScene(sceneName, mode);
    }

    public IEnumerator MoveToNewScene(Scene scene)
    {
        yield return new WaitForEndOfFrame();

        if (SceneManager.GetActiveScene().name != "MainBuild")
        {
            // move player to spawn
            PlayerController.Instance.gameObject.transform.position = GameObject.FindWithTag("Respawn").transform.position;
            //print(GameObject.FindWithTag("Respawn").transform.position);
        }
        else
        {
            // Move player to the place they entered from in main
            PlayerController.Instance.gameObject.transform.position = EnteredFrom;
        }

        yield return null;
    }

    public static bool CanCraftItem<Parts>(List<Parts> ownedParts, params Parts[] requiredParts) { return requiredParts.All(part => ownedParts.Contains(part)); }

    void SetZones() { ActiveZoneTransitions.Clear(); ActiveZoneTransitions = GameObject.FindGameObjectsWithTag("ZoneTransition").Select(obj => obj.transform).ToList(); }

    public void NewObjective(string objectiveText, ObjectiveCondition condition)
    {
        //TODO tell user that there is new objective
        

        // Create new objective
        GameObject newObjective = Instantiate(ObjectivePrefab, ObjectivesObj.transform);

        // Add to list
        Objectives.Add(newObjective);

        // Set text
        newObjective.GetComponent<TextMeshProUGUI>().text = objectiveText;

        // Set position
        newObjective.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -75 - 50 * Objectives.Count);

        // Update container size
        ObjectivesObj.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100 + 50 * Objectives.Count);

        newObjective.name = objectiveText;

        void action()
        {
            // if the condition is true, remove from list and update objectives
            if (condition())
            {
                print("Completed: " + objectiveText);                   // Debug
                Objectives.Remove(newObjective);                        // Remove from list
                UpdateObjectives();                                     // Update for player
                OnObjectiveCompleted -= action;                         // Unsubscribe
                Destroy(newObjective);                                  // Destroy object
            }
        }

        // subscribe to the event
        OnObjectiveCompleted += action;

        UpdateObjectives();
    }

    private void UpdateObjectives()
    {
        // Update objective positions
        for (int i = 0; i < Objectives.Count; ++i)
        {
            Objectives[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -120 - 50 * i);
        }

        // Update container size
        ObjectivesObj.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 100 + 50 * Objectives.Count);
    }

#nullable enable
    public Transform GetRandomPos(out Transform? t) { t = positions[Random.Range(0, positions.Length - 1)]; return t; }
#nullable disable
}