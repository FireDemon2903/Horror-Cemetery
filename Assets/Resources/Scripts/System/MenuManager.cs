using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq.Expressions;

public class MenuManager : MonoBehaviour
{
    public float PlayerSens;

    bool isPaused = false;
    bool isPauseMenu => PauseMenu.activeSelf;

    public GameObject PauseMenu;

    public void SetRotationSens(float sens)
    {
        PlayerControler.Instance.RotationSens = sens;
    }

    public void ChangeGraphicsQuality(int quality) { QualitySettings.SetQualityLevel(quality); }

    public void StartGame() { SceneManager.LoadScene("TestingAreaLoading"); }

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
            print(isPaused);
            print(PauseMenu.name);
        }
        else if (isPaused && isPauseMenu)
        {
            Resume();
        }
    }

    public void Resume()
    {
        isPaused = !isPaused;

        print(isPaused);
        print(PauseMenu.name);

        // Close pause menu
        CloseMenu(PauseMenu);

        Time.timeScale = 1.0f;
    }

    public void Quit() { Application.Quit(); }
}
