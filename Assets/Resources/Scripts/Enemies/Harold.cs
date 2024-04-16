using System.Linq;
using UnityEngine;
using UnityEngine.AI;
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

    //bool onScreen = false;
    bool hasRunStation = false;

    MoveMode Move;

    NavMeshAgent Agent;

    float VisibleTimer = 0f;

    bool thisVisibleToPlayer => PlayerController.Instance.inSight.Contains(gameObject);

    //float chaseTimer = 0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.autoBraking = false;

        Move = StalkPlayer;
        ResetStats();
    }

    private void FixedUpdate()
    {
        //if (chaseTimer > 0f)
        //{
        //    Move = ChasePlayer;
        //}

        //print(Camera.main.WorldToScreenPoint(transform.position));


        if (thisVisibleToPlayer)
        {
            // increment timer
            VisibleTimer += Time.fixedDeltaTime;

            // Update move-mode
            Move = VisibleTimer <= 5 ? Freeze : VisibleTimer <= 10 ? RunFromPlayer : ChasePlayer;
        }
        else
        {
            // if player cannot see this, then stalk
            Move = StalkPlayer;
        }

        Move?.Invoke();
    }

    private void ResetStats()
    {
        VisibleTimer = 0f;

        Agent.speed = 80f * 3;
        Agent.angularSpeed = 120f * 3;
        Agent.acceleration = 300f * 3;

        hasRunStation = false;
    }

    private void IncreaseStats()
    {
        Agent.speed = 80f * 3;
        Agent.angularSpeed = 120f * 3;
        Agent.acceleration = 300f * 3;
    }

    /// <summary>
    /// Stop moving
    /// </summary>
    void Freeze()
    {
        print("freeze");
        Agent.isStopped = true;

        IncreaseStats();
    }

    /// <summary>
    /// Slowly follow player
    /// </summary>
    void StalkPlayer()
    {
        print("stalk");
        Agent.SetDestination(PlayerController.Instance.Position);
        ResetStats();
    }

    /// <summary>
    /// Follow the player, but faster
    /// </summary>
    void ChasePlayer()
    {
        print("chase");
        Agent.SetDestination(PlayerController.Instance.Position);
    }

    /// <summary>
    /// Follow the player, but faster and in opposite direction
    /// </summary>
    void RunFromPlayer()
    {
        print("run");

        Agent.isStopped = false;

        // find station
        if (!hasRunStation)
        {
            // cycle through the stations randomly, until one whom cannot be seen by the player is found
            while (!Instance.GetRandomPos(out var t).gameObject.SightTest(PlayerController.Instance.gameObject))
            {
                Agent.SetDestination(t.position);
                hasRunStation = true;
            }
        }
        else return;
    }
}
