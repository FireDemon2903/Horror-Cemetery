// Ignore Spelling: DMG

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Special zombie.
/// Links health with other nearby zombies
/// All who are part of the link, gets fraction of damage dealt to individuals
/// Upon Death, all linkes will terminate, and remaining enemies will no longer have shared health
/// </summary>
public class HealthLinkZombie : GermanSoldier
{
    public override float DMG { get; set; } = 1f;
    public override float Health { get; set; } = 20f;

    //faster than list
    private readonly HashSet<GermanSoldier> _soldiers = new();

    const float detectDisctance = 25f;

    public override void Awake()
    {
        base.Awake();

        foreach (var a in GetComponents<SphereCollider>())
        {
            if (a.isTrigger)
            {
                a.radius = detectDisctance;
                break;
            }
        }

        _soldiers.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<GermanSoldier>(out var soldier))
        {
            soldier.WasAttacked += OnSoldierTookDamage;
            _soldiers.Add(soldier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GermanSoldier a = other.GetComponent<GermanSoldier>();
        if (_soldiers.Contains(a))
        {
            a.WasAttacked -= OnSoldierTookDamage;
            _soldiers.Remove(a);
        }
    }

    private void OnSoldierTookDamage(float damage)
    {
        float fractionedDamage = damage / _soldiers.Count;
        foreach (GermanSoldier soldier in _soldiers)
        {
            soldier.Health -= fractionedDamage;
        }
    }

    public override void Die()
    {
        foreach (var soldier in _soldiers)
        {
            soldier.WasAttacked -= OnSoldierTookDamage;
            _soldiers.Remove(soldier);
        }

        base.Die();
    }

}
