using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using static GameManager;
using UnityEngine;


/// <summary>
/// Dead squad-mate, killed by shadow monster
/// when in certain range, lower player's attack-speed, and does very minor damage to player.
/// When too far away, quickly dash/teleport to position x-distance from player.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class Harvey : BaseEnemy
{
    private static Harvey _instance;
    public static Harvey Instance
    {
        get
        {
            if (_instance.IsUnityNull()) Debug.LogError("Harvey was null");
            return _instance;
        }
    }

    private float health = 10f;
    public override float Health { get => health; set => health = value; }

    private float dmg = 1000f;
    public override float DMG { get => dmg; set => dmg = value; }

    private float DOTDamage = 10f;

    MoveMode Move => Move1;

    float detectDisctance = 25f;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        else { _instance = this; }


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
            DealDMG(PlayerController.Instance);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            DealDMG(DMGTarget: PlayerController.Instance, DOTDamage * Time.fixedDeltaTime);
            //print("player took damage: " + DOTDamage * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == PlayerController.Instance.gameObject)
        {
            // dist to player
            Vector3 directionToPlayer = PlayerController.Instance.transform.position - transform.position;

            // target pos
            Vector3 targetPosition = PlayerController.Instance.Position - directionToPlayer.normalized * 75f;

            // Move
            transform.position = targetPosition;
        }
    }

    void Move1()
    {

    }

}
