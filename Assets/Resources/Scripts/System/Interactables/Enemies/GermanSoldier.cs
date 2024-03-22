// Ignore Spelling: DMG

using System;
using UnityEngine;
using System.Collections;

public class GermanSoldier : MonoBehaviour, IDamage, IAlive
{
    public float Health { get; set; } = 10f;
    public float DMG { get; set; } = 0f;

    float detectDisctance = 50f;
    float attackRange = 15f;
    bool attackCooldown = false;

    bool playerInSight => gameObject.SightTest(PlayerController.Instance.gameObject, detectDisctance);
    bool playerInRange => Vector3.Distance(PlayerController.Instance.transform.position, gameObject.transform.position) < attackRange;

    delegate void MoveMode();
    delegate void RefreshCooldown();

    MoveMode Move => playerInSight ? MoveToPlayer : RandomMovement;
    RefreshCooldown RefreshAttack => ResetCoolDown;

    Rigidbody mRigidbody;

    float Speed = 0f;

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
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

    void ResetCoolDown() { attackCooldown = false; }

    public virtual void TakeDMG(IDamage DMGSource)
    {
        if (DMGSource == null) return;

        if (Health - DMGSource.DMG <= 0)
        {
            Destroy(gameObject);
        }
    }

    // TODO: Add collision/close-range damage
    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;

        
    }

    public virtual void DealDMG(IAlive DMGTarget)
    {
        attackCooldown = true;

        // Deal direct damage, as target is known
        DMGTarget.TakeDMG(from: this);
    }

    void MoveToPlayer()
    {
        var dir = PlayerController.Instance.transform.position - transform.position;

        mRigidbody.AddForce(dir.normalized * Speed, ForceMode.VelocityChange);
    }



    float minSpeed = 5;  // minimum range of speed to move
    float maxSpeed = 20;  // maximum range of speed to move
    float speed;     // speed is a constantly changing value from the random range of minSpeed and maxSpeed 

    float step = Mathf.PI / 60;
    float timeVar = 0;
    float rotationRange = 120;                  //  How far should the object rotate to find a new direction?
    float baseDirection = 0;

    Vector3 randomDirection;


    void RandomMovement()
    {
        // TODO
        randomDirection = new Vector3(0, Mathf.Sin(timeVar) * (rotationRange / 2) + baseDirection, 0); //   Moving at random angles 
        timeVar += step;
        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);              //      Change this range of numbers to change speed
        mRigidbody.AddForce(transform.forward * speed);
        transform.Rotate(10.0f * Time.deltaTime * randomDirection);
    }

}
