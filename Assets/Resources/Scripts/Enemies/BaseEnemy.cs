// Ignore Spelling: Rigidbody

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent (typeof(Rigidbody))]
public abstract class BaseEnemy : MonoBehaviour, IDamage, IAlive
{
    public abstract float DMG { get; set; }
    public abstract float Health { get; set; }

    [DoNotSerialize] public Rigidbody mRigidbody;
    [DoNotSerialize] public NavMeshAgent mAgent;

    public virtual void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void TakeDMG(IDamage DMGSource, float? dmg=null)
    {
        if (DMGSource == null) return;

        // if dmg has a value, then use that instead of the normal damage
        Health -= dmg ?? DMGSource.DMG;

        if (Health <= 0) Die();
    }

    public virtual void DealDMG(IAlive DMGTarget, float? dmg = null)
    {
        DMGTarget.TakeDMG(from: this, dmg: dmg);
    }

    //todo make ragdoll death :)
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}