using UnityEngine;

public class Readable : MonoBehaviour, IInteractable
{
    public AudioClip clip;

    public void Interact(GameObject sender)
    {
        print($"This object was read by {sender.name}");

        sender.SendMessage("PlayClip", clip);


    }



}
