using UnityEngine;

/// <summary>
/// Interface for an interactible object
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Action that should happen, when interacted with
    /// </summary>
    /// <param name="sender">Typically 'Player' GameObject</param>
    void Interact(GameObject sender);
}

/// <summary>
/// Interface for a part
/// </summary>
public interface IPart
{
    /// <summary>
    /// Method to collect a part
    /// </summary>
    /// <param name="playerControler">PlayerControler script</param>
    void Collect(PlayerControler playerControler);
}

/// <summary>
/// Interface for enemy scripts
/// </summary>
public interface IEnemy
{

}