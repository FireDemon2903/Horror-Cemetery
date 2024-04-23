using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class EnterArea : Interactor
{
    //public string SceneName;
    public GameManager.Scenenames Area;
    int IndexInGM => GameManager.Instance.ActiveZoneTransitions.IndexOf(transform.parent.transform);

    public void Enter()
    {
        if (GameManager.Instance.Locked.ContainsKey(Area.ToString()))
        {
            // tell player they cannot enter
            return;
        }

        // TODO: Save mainArea game-state

        // Save the position the player came from
        if (SceneManager.GetActiveScene().name == "MainBuildNotBlue")
        {
            print(IndexInGM);
            GameManager.Instance.EnteredFrom = GameManager.Instance.ActiveZoneTransitions[IndexInGM].position;
            //GameManager.Instance.RotationFrom = GameManager.Instance.ActiveZoneTransitions[IndexInGM].rotation;
        }

        // Load Area
        GameManager.Instance.LoadScene(Area.ToString(), LoadSceneMode.Additive);
    }
}
