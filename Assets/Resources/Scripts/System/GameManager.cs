using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
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

    public GameObject Menu;                                     // menu object
    GameObject ObjectivePrefab;                                 // objective prefab to be instantiated
    public GameObject ObjectivesObj;                            // parent of objectives

    public GameObject EventSystemObject;                        // used for settings and other

    public delegate void MoveMode();                            // delegate used in enemies to dictate their movement method
    public delegate void RefreshCooldown();                     // delegate used to refresh cooldowns

    public delegate bool ObjectiveCondition();                  // boolean delegate for objective conditions
    public event Action OnObjectiveCompleted;                   // event Action multicast delegate to check wether objectives have been completed
    readonly List<GameObject> Objectives = new();               // list of objective gameobjects

    Transform[] positions;                                      // positions of gameobjects tagged "station". Refreshes every scene load

    // Names of Areas to be used in ´Load´ objects
    public enum Scenenames
    {
        MainBuildNotBlue,
        MainBuildCave,
        ArenaGermanSoldier,
        ArenaHealthLink,
        ArenaHansi,
        ArenaHarold,
        ArenaHarry,
        ArenaHarvey,
        TestingAreaLoading
    }

    public Vector3 PlayerSpawn => GameObject.FindWithTag("Respawn").transform.position;

    public List<Transform> ActiveZoneTransitions;           // List of SubAreas
    public Vector3 EnteredFrom;                             // The position of the last zone transition player used

    // Parts the player can pick up
    public enum Parts { GunBarrel, GunHandle, GunCyllinder, Gunpowder, Casing }

    Scene OldScene;                                         // reference to the "old" scene (see on scene loaded)

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

        // start pos in main
        EnteredFrom = new Vector3(886.299988f, 15.5500002f, 1360.43005f);

        Menu = MenuManager.Instance.Menu;
        ObjectivesObj = Menu.transform.Find("ObjectiveMenu").gameObject;
        ObjectivePrefab = Resources.Load<GameObject>(@"Prefabs/FolderObjectives/Objective");
    }

    public readonly Dictionary<string, GameObject> Locked = new();

    private void Start()
    {
        // create objectives
        NewObjective("Find gun barrel", condition: () => { return PlayerController.Instance.OwnedParts.Contains(Parts.GunBarrel); });
        NewObjective("Find gun cylinder", condition: () => { return PlayerController.Instance.OwnedParts.Contains(Parts.GunCyllinder); });
        NewObjective("Find gun handle", condition: () => { return PlayerController.Instance.OwnedParts.Contains(Parts.GunHandle); });
        NewObjective("Find bullet parts", condition: () => { return CanCraftItem(PlayerController.Instance.OwnedParts, Parts.Gunpowder, Parts.Casing); });
        NewObjective("Kill 10 random enemies", condition: () => { return PlayerController.Instance.killCount >= 10; });


        // testing

        GameObject a = null;
        Action action = null;
        ObjectiveCondition c = () => { return PlayerController.Instance.killCount > 0; };

        action = () =>
        {
            if (c())
            {
                print("Complete");
                Objectives.Remove(a);                        // Remove from list
                Locked.Remove("MainBuildNotBlue");
                UpdateObjectives();                                     // Update for player
                OnObjectiveCompleted -= action;                         // Unsubscribe
                Destroy(a);
            }
        };

        a = NewObjective(
            objectiveText: "Kill 1 to progress",
            action: action,
            condition: c
            );


        Locked.Add("MainBuildNotBlue", a);
    }

    private void FixedUpdate()
    {
        // check through the conditions
        OnObjectiveCompleted?.Invoke();
        print(FindAnyObjectByType(typeof(BaseEnemy)) == null);

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
        StartCoroutine(MoveToNewScene());

        OldScene = scene;
    }

    //HashSet<string> locked = new HashSet<string>();


    public void LoadScene(string sceneName, LoadSceneMode mode)
    {
        //if (!locked.Contains(sceneName))

        SceneManager.LoadScene(sceneName, mode);
    }

    public IEnumerator MoveToNewScene()
    {
        yield return new WaitForEndOfFrame();

        if (SceneManager.GetActiveScene().name != "MainBuildNotBlue")
        {
            // move player to spawn
            PlayerController.Instance.gameObject.transform.position = GameObject.FindWithTag("Respawn").transform.position;
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

    public GameObject NewObjective(string objectiveText, Action? action = null, ObjectiveCondition? condition = null)
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

        // if there is no action, but a condition, use base action
        if (action == null && condition != null)
        {
            action = () =>
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
            };
        }


        // subscribe to the event
        OnObjectiveCompleted += action;

        UpdateObjectives();

        return newObjective;
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

    // credit: https://forum.unity.com/threads/generating-a-random-position-on-navmesh.873364/#post-5796748
    /// <summary>
    /// Selects a random point on the game board (NavMesh).
    /// </summary>
    /// <returns>Vector3 of the random location.</returns>
    public static Vector3 GetRandomGameBoardLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;

        // pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = Random.Range(0, maxIndices);
        int secondVertexSelected = Random.Range(0, maxIndices);

        // spawn on verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];

        // eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x || (int)firstVertexPosition.z == (int)secondVertexPosition.z)
        {
            point = GetRandomGameBoardLocation(); // re-roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // select a random point on it
            point = Vector3.Lerp(firstVertexPosition, secondVertexPosition, Random.Range(0.05f, 0.95f));
        }

        return point;
    }

}