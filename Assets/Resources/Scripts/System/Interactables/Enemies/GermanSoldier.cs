// Ignore Spelling: DMG

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class GermanSoldier : BaseEnemy
{
    private float health = 10f;
    public override float Health { get => health; set => health = value; }

    private float dmg = 1f;
    public override float DMG { get => dmg; set => dmg = value; }

    // Canonically starts at one, because zombie :D
    private int timesRevived = 1;

    float detectDisctance = 50f;
    float attackRange = 15f;
    bool attackCooldown = false;

    bool playerInSight => gameObject.SightTest(PlayerController.Instance.gameObject, detectDisctance);
    bool playerInRange => Vector3.Distance(PlayerController.Instance.Position, gameObject.transform.position) < attackRange;
    public bool isHarveyMinion = false;
    bool isDead => Health <= 0;

    // if the player is in sight, move to player. if this is a minion, move to Harvey. else idle movement
    MoveMode Move => playerInSight ? MoveToPlayer : isHarveyMinion ? MoveToHarvey : IdleMovement;
    RefreshCooldown RefreshAttack => () => attackCooldown = false;

    NavMeshAgent NavMeshAgent { get; set; }

    Rigidbody mRigidbody;

    Vector3 newStation => Instance.GetRandomPos();
    Vector3 targetStation;

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        Move?.Invoke();
        
        mRigidbody.DebugVelocity(Color.cyan);
        
        if (playerInRange && !attackCooldown)
        {
            print("Enemy Attacked!");
            // Attack
            DealDMG(PlayerController.Instance);
            attackCooldown = true;

            // start refresh cool-down
            StartCoroutine(RefreshAttack.DelayedExecution(delay: 1f));
        }

        // inefficient, fix later
        if (isDead)
        {
            Die();
        }
    }

    void MoveToPlayer() { NavMeshAgent.SetDestination(PlayerController.Instance.Position); }
    void MoveToHarvey()
    {
        try
        {
            NavMeshAgent.SetDestination(Hansi.Instance.transform.position);
        }
        catch (MissingReferenceException)
        {
            isHarveyMinion = false;
        }
    }

    void IdleMovement()
    {
        //if (Vector3.Distance(NavMeshAgent.pathEndPosition, transform.position) < 2f)
        //{
        //    NavMeshAgent.SetDestination(Instance.GetRandomPos());
        //}
        //else if (!NavMeshAgent.hasPath) { NavMeshAgent.SetDestination(Instance.GetRandomPos());
        //}
    }

    public override void DealDMG(IAlive DMGTarget, float? dmg = null)
    {
        base.DealDMG(DMGTarget);

        attackCooldown = true;
    }

    void Die()
    {
        enabled = false;
        Renderer r = gameObject.GetComponent<Renderer>();
        r.material.color = Color.red;

        NavMeshAgent.ResetPath();
    }

    public void Revive()
    {
        if (isDead)
        {
            timesRevived += 1;
            health = 10f;       // Reset base health

            // increase attributes
            health *= timesRevived;
            dmg *= Math.Max(0, timesRevived / 3);

            // change colour
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
