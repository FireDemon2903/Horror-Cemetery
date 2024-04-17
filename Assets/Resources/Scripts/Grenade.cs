using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour, IDamage
{
    float timer = 2f;
    const float radius = 200f;
    const float force = 10_000f;

    public float DMG { get; set; } = 100f;

    private void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        // get colliders
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Rigidbody>(out var rb))
            {
                print("boom");
                rb.AddExplosionForce(force, gameObject.transform.position, radius);

                // attempt to deal damage
                collider.gameObject.TryDealDamage(source: this);
            }
        }
    }

    [Obsolete("Not used by grenades, as they use TryDealDamage extension")]
    public void DealDMG(IAlive DMGTarget, float? damage = null)
    {
        DMGTarget.TakeDMG(from: this, dmg: damage);
    }

}
