using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Special zombie.
/// Links health with other nearby zombies
/// All who are part of the link, shares a health-pool
/// When health-pool reaches 0, all linked enemies die
/// </summary>
public class HealthLinkZombie : GermanSoldier
{
    public override float DMG { get; set; } = 1f;
    public override float Health { get; set; } = 10f;

    private HashSet<GermanSoldier> soldiers;

    float detectDisctance = 25f;

    private void Awake()
    {
        foreach (var a in GetComponents<SphereCollider>())
        {
            if (a.isTrigger)
            {
                a.radius = detectDisctance;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<GermanSoldier>(out var soldier))
        {
            soldier.WasAttacked += OnSoldierTookDamage;
            soldiers.Add(soldier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GermanSoldier a = other.GetComponent<GermanSoldier>();
        if (soldiers.Contains(a))
        {
            a.WasAttacked -= OnSoldierTookDamage;
            soldiers.Remove(a);
        }
    }

    private void OnSoldierTookDamage(float damage)
    {
        float fractionedDamage = damage / soldiers.Count;
        foreach (GermanSoldier soldier in soldiers)
        {
            soldier.Health -= fractionedDamage;
        }
    }

}
