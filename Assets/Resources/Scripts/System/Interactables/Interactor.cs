using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

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
        // Enemy
        else if (TryGetComponent<IEnemy>(out var enemy))
        {
            // Interact with the enemy
            //enemy.TakeDMG(controler);  damage shoulld be done elsewhere
        }
    }
}
