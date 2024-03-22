using UnityEngine;

/// <summary>
/// Intermediate class for interacting with various objects
/// </summary>
public class Interactor : MonoBehaviour, IInteractable
{
    public void Interact(GameObject sender)
    {
        if (!sender.TryGetComponent<PlayerController>(out var controler)) return;

        // Collectible part
        if (TryGetComponent<IPart>(out var part))
        {
            // Collect the part
            part.Collect(controler);
            print($"{sender.name} picked up a '{part.GetType()}'");
            
            // Destroy the GO
            Destroy(gameObject);
        }
        // Readable object
        else if (TryGetComponent<Readable>(out var read))
        {
            // Read the thing
            read.Read(sender);
        }
        else if (TryGetComponent<ToggleLightTest>(out var lightTest))
        {
            lightTest.Toggle(sender: sender);
        }

        else if (TryGetComponent<EnterArea>(out var area))
        {
            area.Enter();
        }
    }
}
