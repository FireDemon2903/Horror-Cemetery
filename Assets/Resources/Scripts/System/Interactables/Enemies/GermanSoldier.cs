// Ignore Spelling: DMG

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class GermanSoldier : BaseEnemy
{
    private float health;
    public override float Health { get => health; set => health = value; }

    private float dmg;
    public override float DMG { get => dmg; set => dmg = value; }

    float detectDisctance = 50f;
    float attackRange = 15f;
    bool attackCooldown = false;

    bool playerInSight => gameObject.SightTest(PlayerController.Instance.gameObject, detectDisctance);
    bool playerInRange => Vector3.Distance(PlayerController.Instance.Position, gameObject.transform.position) < attackRange;
    public bool isHarveyMinion = false;
    bool isDead = false;

    // if the player is in sight, move to player. if this is a minion, move to Harvey. else idle movement
    MoveMode Move => playerInSight ? MoveToPlayer : isHarveyMinion ? MoveToHarvey : MoveToPlayer;//IdleMovement;
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

            // start refresh cool-down
            StartCoroutine(RefreshAttack.DelayedExecution(delay: 1f));
        }

        if (isDead)
        {
            enabled = false;
        }
    }

    void MoveToPlayer() { NavMeshAgent.SetDestination(PlayerController.Instance.Position); }
    void MoveToHarvey()
    {
        try
        {
            NavMeshAgent.SetDestination(Harvey.Instance.transform.position);
        }
        catch (MissingReferenceException)
        {
            isHarveyMinion = false;
        }
    }

    void IdleMovement()
    {
        //if (targetStation == null)
        //{
        //    targetStation = newStation;
        //}
        //else if (NavMeshAgent.destination != targetStation)
        //{
        //    NavMeshAgent.SetDestination(targetStation);
        //}
    }

    public override void TakeDMG(IDamage DMGSource)
    {
        if (DMGSource == null) return;

        if (Health - DMGSource.DMG <= 0)
        {
            //Destroy(gameObject);
            Die();
        }
    }

    public override void DealDMG(IAlive DMGTarget)
    {
        attackCooldown = true;

        // Deal direct damage, as target is known
        DMGTarget.TakeDMG(from: this);
    }

    void Die()
    {
        enabled = false;
        Renderer r = gameObject.GetComponent<Renderer>();
        r.material.color = Color.red;

        NavMeshAgent.isStopped = true;
    }

    public void Revive()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
        NavMeshAgent.isStopped = false;
    }

}
