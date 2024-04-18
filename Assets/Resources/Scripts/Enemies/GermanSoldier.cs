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

    readonly float detectDisctance = 50f;
    readonly float attackRange = 15f;
    bool attackCooldown = false;

    bool PlayerInSight => gameObject.SightTest(PlayerController.Instance.gameObject, detectDisctance);
    bool PlayerInRange => Vector3.Distance(PlayerController.Instance.Position, gameObject.transform.position) < attackRange;
    public bool isHansiMinion = false;
    bool IsDead => Health <= 0;

    // if the player is in sight, move to player. if this is a minion, move to Hansi. else idle movement
    MoveMode Move => PlayerInSight ? MoveToPlayer : isHansiMinion ? MoveToHansi : IdleMovement;
    RefreshCooldown RefreshAttack;

    public delegate void Attacked(float damage);
    public event Attacked WasAttacked;

    //Vector3 newStation => Instance.GetRandomPos();
    //Vector3 targetStation;

    public override void Awake()
    {
        base.Awake();

        mAgent.autoBraking = false;

        RefreshAttack = () => attackCooldown = false;
    }

    private void FixedUpdate()
    {
        Move?.Invoke();

        mRigidbody.DebugVelocity(Color.cyan);
        
        if (PlayerInRange && !attackCooldown)
        {
            print("Enemy Attacked!");
            // Attack
            DealDMG(PlayerController.Instance);
            attackCooldown = true;

            // start refresh cool-down
            RefreshAttack.DelayedExecution(delay: 1f);
        }


        // inefficient, fix later
        if (IsDead)
        {
            Die();
        }
    }

    void MoveToPlayer() { mAgent.SetDestination(PlayerController.Instance.Position); }
    void MoveToHansi()
    {
        try
        {
            mAgent.SetDestination(Hansi.Instance.transform.position);
        }
        catch (MissingReferenceException)
        {
            isHansiMinion = false;
        }
    }

    //TODO make soldier idle movement
    void IdleMovement()
    {
        //if (Vector3.Distance(NavMeshAgent.pathEndPosition, transform.position) < 2f)
        //{
        //    NavMeshAgent.SetDestination(Instance.GetRandomPos());
        //}
        //else if (!NavMeshAgent.hasPath) { NavMeshAgent.SetDestination(Instance.GetRandomPos());
        //}
    }

    public override void TakeDMG(IDamage DMGSource, float? dmg = null)
    {
        if (DMGSource == null) return;

        if (WasAttacked != null) { WasAttacked?.Invoke(dmg ?? DMGSource.DMG); }                 // if there are listeners, use them instead
        else { Health -= DMGSource.DMG; }                                                                    // if dmg has a value, then use that instead of the normal damage
    }

    public override void DealDMG(IAlive DMGTarget, float? dmg = null)
    {
        base.DealDMG(DMGTarget);

        attackCooldown = true;
    }

    public override void Die()
    {
        enabled = false;
        Renderer r = gameObject.GetComponent<Renderer>();
        r.material.color = Color.red;

        mAgent.ResetPath();
    }

    public void Revive()
    {
        if (IsDead)
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
