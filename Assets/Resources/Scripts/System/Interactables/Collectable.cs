using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{
    public void Interact(GameObject sender)
    {
        print($"Thjis object was picked up by {sender.name}");
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
