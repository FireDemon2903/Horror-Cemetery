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
    /// <param name="playerControler">PlayerController script</param>
    void Collect(PlayerController playerControler);
}

/// <summary>
/// Interface for sources of damage
/// </summary>
public interface IDamage
{
    /// <summary>
    /// The default damage this object does when dealing damage
    /// </summary>
    float DMG { get; set; }

    /// <summary>
    /// Method called to damage another target
    /// </summary>
    /// <param name="DMGTarget"></param>
    void DealDMG(IAlive DMGTarget, float? dmg=null);
}

/// <summary>
/// Interface for things that are "alive" (has hp and can die)
/// </summary>
public interface IAlive
{
    /// <summary>
    /// This instances current health
    /// </summary>
    float Health { get; set; }

    /// <summary>
    /// Method called to damage another instance of alive
    /// </summary>
    /// <param name="from">Other instance that attacked this (the source of the damage)</param>
    void TakeDMG(IDamage from, float? dmg=null);

    /// <summary>
    /// What should happen when entity dies
    /// </summary>
    void Die();

}