using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterArea : MonoBehaviour, IInteractable
{
    public string Area;

    public void Interact(GameObject sender)
    {
        // Save mainArea gamestate

        // load subarea
        GameManager.Instance.LoadScene(Area, LoadSceneMode.Additive);
    }
}
