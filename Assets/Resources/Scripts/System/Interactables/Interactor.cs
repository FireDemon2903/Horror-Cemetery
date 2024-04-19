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
        // Area entrance
        else if (TryGetComponent<EnterArea>(out var area))
        {
            // Enter area
            area.Enter();
        }
    }
}
