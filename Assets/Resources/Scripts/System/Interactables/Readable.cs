using UnityEngine;

public class Readable : Interactor
{
    public AudioClip clip;

    public void Read(GameObject sender)
    {
        print($"{gameObject.name} was read by {sender.name}");
        sender.SendMessage("PlayClip", clip);
    }
}
