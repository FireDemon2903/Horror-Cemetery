using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour, IDamage
{
    [Range(0f, 10f)] float timer = 5f;
    const float RADIUS = 100f;
    const float FORCE = 2_000f;

    //todo tweak grenade damage
    public float DMG { get; set; } = 0f; //1f;

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
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, RADIUS);

        foreach (Collider collider in colliders)
        {

            if (collider.TryGetComponent<Rigidbody>(out var rb))
            {
                if (collider.TryGetComponent<Harry>(out var _))
                {
                    rb.AddExplosionForce(FORCE * .2f, gameObject.transform.position, RADIUS, upwardsModifier: 3, ForceMode.Impulse);
                }
                rb.AddExplosionForce(FORCE, gameObject.transform.position, RADIUS, upwardsModifier: 3, ForceMode.Impulse);

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
