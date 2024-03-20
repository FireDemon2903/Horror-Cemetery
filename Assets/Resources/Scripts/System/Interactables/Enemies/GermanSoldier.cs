using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GermanSoldier : BaseEnemy
{
    MoveDelegate Move => playerInSight ? MoveToPlayer : RandomMovement;
    //MoveDelegate Move => RandomMovement;

    Rigidbody mRigidbody;

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
    }

    public override void TakeDMG(PlayerControler DMGSource)
    {
        base.TakeDMG(DMGSource);
    }

    public override void DealDMG(PlayerControler DMGTarget)
    {
        base.DealDMG(DMGTarget);
    }

    private void FixedUpdate()
    {
        print(playerInSight);
        //print(gameObject.SightTest(PlayerControler.Instance.gameObject, detectDisctance));
        Move?.Invoke();
    }

    void MoveToPlayer()
    {
        var dir = PlayerControler.Instance.transform.position - transform.position;

        mRigidbody.AddForce(dir);
    }

    


    float minSpeed = 1;  // minimum range of speed to move
    float maxSpeed = 10;  // maximum range of speed to move
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
        speed = Random.Range(minSpeed, maxSpeed);              //      Change this range of numbers to change speed
        mRigidbody.AddForce(transform.forward * speed);
        transform.Rotate(randomDirection * Time.deltaTime * 10.0f);
    }
}
