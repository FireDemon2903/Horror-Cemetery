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
public interface IEnemy : IAlive
{
    /// <summary>
    /// Method called to damage an enemy instance
    /// </summary>
    /// <param name="playerControler">Player that attacked the enemy (source of damage)</param>
    void TakeDMG(PlayerControler playerControler);
    /// <summary>
    /// Method callled to damage another target
    /// </summary>
    /// <param name="DMGTarget"></param>
    void DealDMG(PlayerControler DMGTarget);
}

/// <summary>
/// Interface for things that are "alive" (has hp and dmg)
/// </summary>
public interface IAlive
{
    float Health { get; set; }
    float DMG { get; set; }
}