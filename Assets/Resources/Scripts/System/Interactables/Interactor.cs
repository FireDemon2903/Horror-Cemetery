using UnityEngine;

/// <summary>
/// Intermediate class for interacting with vcarious objects
/// </summary>
public class Interactor : MonoBehaviour, IInteractable
{
    public void Interact(GameObject sender)
    {
        if (!sender.TryGetComponent<PlayerControler>(out var controler)) return;

        // Collectible part
        if (TryGetComponent<IPart>(out var part))
        {
            // Colect the part
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
        // Enemy
        //else if (TryGetComponent<IEnemy>(out var enemy))
        //{
        //    // Interact with the enemy
        //    //enemy.TakeDMG(controler);  damage shoulld be done elsewhere
        //}
    }
}
