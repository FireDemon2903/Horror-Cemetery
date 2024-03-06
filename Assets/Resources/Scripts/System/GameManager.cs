using Unity.VisualScripting;
using UnityEngine;

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


    }

    // Update is called once per frame
    void Update()
    {
        
    }

}