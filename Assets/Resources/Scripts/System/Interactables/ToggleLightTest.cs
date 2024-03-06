using UnityEngine;

public class ToggleLightTest : MonoBehaviour, IInteractable
{
    public void Interact(GameObject sender) { sender.GetComponent<PlayerControler>().ToggleLightType(); }
}
