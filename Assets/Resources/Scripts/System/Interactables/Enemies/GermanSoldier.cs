// Ignore Spelling: DMG

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class GermanSoldier : MonoBehaviour, IDamage, IAlive
{
    public float Health { get; set; } = 10f;
    public float DMG { get; set; } = 1f;

    float detectDisctance = 50f;
    float attackRange = 15f;
    bool attackCooldown = false;

    bool playerInSight => gameObject.SightTest(PlayerController.Instance.gameObject, detectDisctance);
    bool playerInRange => Vector3.Distance(PlayerController.Instance.Position, gameObject.transform.position) < attackRange;

    MoveMode Move => MoveToPlayer;//playerInSight ? MoveToPlayer : RandomMovement;
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
        print(playerInRange);
        if (playerInRange && !attackCooldown)
        {
            print("Enemy Attacked!");
            // Attack
            DealDMG(PlayerController.Instance);

            // start refresh cool-down
            StartCoroutine(RefreshAttack.DelayedExecution(delay: 1f));
        }
    }

    public void TakeDMG(IDamage DMGSource)
    {
        if (DMGSource == null) return;

        if (Health - DMGSource.DMG <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void DealDMG(IAlive DMGTarget)
    {
        attackCooldown = true;

        // Deal direct damage, as target is known
        DMGTarget.TakeDMG(from: this);
    }

    void MoveToPlayer() { NavMeshAgent.SetDestination(PlayerController.Instance.Position); }

    void RandomMovement()
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

}
