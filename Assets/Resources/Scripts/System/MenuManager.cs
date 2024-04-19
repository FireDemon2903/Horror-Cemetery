using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    public static MenuManager Instance
    {
        get
        {
            if (_instance.IsUnityNull()) Debug.LogError("GameManager was null");
            return _instance;
        }
    }

    public float PlayerSens;

    bool isPaused = false;
    bool isPauseMenu => PauseMenu.activeSelf;

    public GameObject PauseMenu;

    public GameObject Menu;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        else { _instance = this; }


        Menu = GameObject.Find("Menu");
        
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(Menu);
    }


    public void SetRotationSens(float sens)
    {
        PlayerController.Instance.RotationSens = sens;
    }

    public void ChangeGraphicsQuality(int quality) { QualitySettings.SetQualityLevel(quality); }

    public void StartGame() { SceneManager.LoadScene("MainBuildNotBlue"); } //TestingAreaLoading

    public void OpenMenu(GameObject Menu) { Menu.SetActive(true); }
    public void CloseMenu(GameObject Menu) { Menu.SetActive(false); }

    public void OnPause()
    {
        if (!isPaused)
        {
            isPaused = !isPaused;
            Time.timeScale -= Time.timeScale;
        
            // Open pause menu
            OpenMenu(PauseMenu);
        }
        else if (isPaused && isPauseMenu)
        {
            Resume();
        }
    }

    public void Resume()
    {
        isPaused = !isPaused;

        // Close pause menu
        CloseMenu(PauseMenu);

        Time.timeScale = 1.0f;
    }

    public void Quit() { Application.Quit(); }

    public void OnToggleObjectives(InputValue value)
    {
        GameManager.Instance.ObjectivesObj.SetActive(!GameManager.Instance.ObjectivesObj.activeSelf);
    }
}
