using UnityEngine;
using static GameManager;

/// <summary>
/// Mini-boss
/// Immortal
/// Player cannot damage this enemy, only avoid it.
/// Will stalk player all the time when player is inside its domain(its scene)
/// It moves at constant speed, following the player whether it can see him or not.
/// When it sees the player, it stops for x-seconds, after y seconds it runs, and after z seconds it charges the player
/// </summary>
public class Harold : BaseEnemy
{
    public override float DMG { get; set; } = 1f;
    public override float Health { get; set; } = Mathf.Infinity;

    bool onScreen = false;

    MoveMode Move;

    float VisibleTimer = 0f;

    private void FixedUpdate()
    {
        if (onScreen)
        {
            // increment timer
            VisibleTimer += Time.fixedDeltaTime;

            // Update move-mode
            Move = VisibleTimer <= 0f ? Freeze : VisibleTimer >= 2.5f ? RunFromPlayer : VisibleTimer >= 5f ? ChasePlayer : ChasePlayer;
        }
        else
        {
            // if player cannot see this, then stalk
            Move = StalkPlayer;
        }

        Move?.Invoke();
    }

    private void OnBecameVisible() { onScreen = true; }
    private void OnBecameInvisible() { onScreen = false; VisibleTimer = 0f; }

    // TODO: implement

    /// <summary>
    /// Stop moving
    /// </summary>
    void Freeze()
    {

    }

    /// <summary>
    /// Slowly follow player
    /// </summary>
    void StalkPlayer()
    {

    }

    /// <summary>
    /// Follow the player, but faster
    /// </summary>
    void ChasePlayer()
    {

    }

    /// <summary>
    /// Follow the player, but faster and in opposite direction
    /// </summary>
    void RunFromPlayer()
    {

    }

}
