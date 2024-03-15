using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public float PlayerSens;

    GameObject? LastMenu;

    public void SetRotationSens(float sens)
    {
        /*PlayerControler.Instance.RotationSens = sens; */ print(sens);

        // TODO: Call player instance to change sens

        //PlayerControler.Instance.RotationSens = sens;

        // save the sens
        PlayerSens = sens;
    }

    public void ChangeGraphicsQuality(int quality) { QualitySettings.SetQualityLevel(quality); }

    //public void StartGame() { SceneManager.LoadScene(""); }

    public void OpenMenu(GameObject Menu)
    {
        Menu.SetActive(true);
    }

    public void CloseMenu(GameObject Menu) { Menu.SetActive(false); }

    public void Pause()
    {
        Time.timeScale -= Time.timeScale;
        
        // Open pause menu
        //OpenMenu(null);
    }

    public void UnPause()
    {
        Time.timeScale = 1.0f;

        // Close pause menu
        //CloseMenu(null);
    }

    public void Quit() { Application.Quit(); }
}
