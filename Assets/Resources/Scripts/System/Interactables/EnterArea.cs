using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterArea : MonoBehaviour, IInteractable
{
    //public string SceneName;
    public GameManager.Scenenames Area;
    int IndexInGM => GameManager.Instance.ActiveZoneTransitions.IndexOf(transform.parent.transform);

    public void Interact(GameObject sender)
    {
        // Save mainArea gamestate

        // Save the position the player came from
        if (SceneManager.GetActiveScene().name == "TestingAreaLoading")
            GameManager.Instance.EnteredFrom = GameManager.Instance.ActiveZoneTransitions[IndexInGM].position;

        // Load Area
        GameManager.Instance.LoadScene(Area.SelectedName(false), LoadSceneMode.Additive);
    }
}
