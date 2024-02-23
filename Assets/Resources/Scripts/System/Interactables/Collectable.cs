using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{
    public void Interact(GameObject sender)
    {
        print($"Thjis object was picked up by {sender.name}");
    }
}
