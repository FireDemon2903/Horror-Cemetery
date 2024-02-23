using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{
    public void Interact(GameObject sender)
    {
        if (!sender.TryGetComponent<PlayerControler>(out var controler)) return;

        if (TryGetComponent<IPart>(out var part))
        {
            part.Collect(controler);
            print($"{sender.name} picked up a '{part.GetType()}'");
            Destroy(gameObject);
        }
    }
}
