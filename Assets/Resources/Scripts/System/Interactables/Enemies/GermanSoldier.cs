// Ignore Spelling: DMG

using UnityEngine;

public class GermanSoldier : MonoBehaviour, IDamage, IAlive
{
    public float Health { get; set; } = 10f;
    public float DMG { get; set; } = 10f;

    internal float detectDisctance = 50f;

    internal bool playerInSight => gameObject.SightTest(PlayerController.Instance.gameObject, detectDisctance);

    internal delegate void MoveMode();

    MoveMode Move => playerInSight ? MoveToPlayer : RandomMovement;

    public virtual void TakeDMG(IDamage DMGSource)
    {
        print("Damage received" + DMGSource.DMG);

        if (DMGSource == null) return;

        if (Health - DMGSource.DMG <= 0)
        {
            Destroy(gameObject);
        }
    }

    // TODO: Add collision/close-range damage



    public virtual void DealDMG(IAlive DMGTarget)
    {
        // Deal direct damage, as target is known
        DMGTarget.TakeDMG(from: this);
    }

    Rigidbody mRigidbody;

    float Speed = 2f;

    private void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move?.Invoke();

        mRigidbody.DebugVelocity(Color.cyan);
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
        speed = Random.Range(minSpeed, maxSpeed);              //      Change this range of numbers to change speed
        mRigidbody.AddForce(transform.forward * speed);
        transform.Rotate(10.0f * Time.deltaTime * randomDirection);
    }

}
