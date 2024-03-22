using UnityEngine;

public class ToggleLightTest : Interactor
{
    public void Toggle(GameObject sender) { sender.GetComponent<PlayerControler>().ToggleLightType(); }
}
