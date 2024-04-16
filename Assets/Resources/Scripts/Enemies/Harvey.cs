using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static GameManager;

/// <summary>
/// Dead squad-mate turned endgame boss, killed by shadow monster
/// when in certain range, halves player's attack-speed and movement-speed.
/// Dies minor DOT to player while in range.
/// When too far away, quickly teleport to position x-distance on other side of player.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class Harvey : BaseEnemy
{
    private static Harvey _instance;
    public static Harvey Instance { get { if (_instance.IsUnityNull()) Debug.LogError("Harvey was null"); return _instance; } }

    private float health = 10f;
    public override float Health { get => health; set => health = value; }

    private float dmg = 0f;
    public override float DMG { get => dmg; set => dmg = value; }

    private readonly float DOTDamage = 1f;
    private readonly float AccelerationSpeed = 20f;

    private Rigidbody mRigidbody;

    MoveMode Move => Move1;

    readonly float detectDisctance = 25f;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        else { _instance = this; }

        mRigidbody = GetComponent<Rigidbody>();

        foreach (var a in GetComponents<SphereCollider>())
        {
            if (a.isTrigger)
            {
                a.radius = detectDisctance;
                break;
            }
        }
    }

    public override void TakeDMG(IDamage DMGSource, float? dmg=null)
    {
        base.TakeDMG(DMGSource);

        if (Health <= 0)
        {
            //Reset player DMG when this dies
            PlayerController.Instance.DMGMult = 1f;
        }
    }

    private void Update()
    {
        Move.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            PlayerController.Instance.DMGMult = .5f;
            print("Reduced player damage");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == PlayerController.Instance.gameObject)
        {
            // on impact, kill player
            DealDMG(PlayerController.Instance);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            //DealDMG(DMGTarget: PlayerController.Instance, DOTDamage * Time.fixedDeltaTime);
            //print("player took damage: " + DOTDamage * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            // get pos behind player
            Vector3 targetPosition = PlayerController.Instance.gameObject.transform.position - PlayerController.Instance.gameObject.transform.forward * 150f;

            // Kill momentum
            mRigidbody.KillVelocityAndAngular();

            // Move to the target position
            transform.position = targetPosition;
        }
    }

    void Move1()
    {
        // find direction to player
        Vector3 dir = (PlayerController.Instance.Position - transform.position).normalized;

        // accelerate
        mRigidbody.AddForce(dir * AccelerationSpeed, ForceMode.Acceleration);
    }

}
