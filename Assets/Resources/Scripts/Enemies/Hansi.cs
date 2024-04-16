// Ignore Spelling: DMG Hansi Hansi's

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using static GameManager;

/// <summary>
/// Mini-boss
/// Unique mini-boss enemy. Has the ability to raise fallen zombies into stronger, faster versions of themselves.
/// This enemy does not deal damage at the moment.
/// If a `GermanSoldier` wanders into the range of Hansi, then it becomes Hansi's minion.
/// It will continue to be resurrected by Hansi every `ressurectTimer` seconds, until Hansi is killed.
/// If the minion cannot see the player, then the minion will follow Hansi.
/// </summary>

[RequireComponent(typeof(SphereCollider))]
public class Hansi : BaseEnemy
{
    private static Hansi _instance;
    public static Hansi Instance
    {
        get
        {
            if (_instance.IsUnityNull()) Debug.LogError("Hansi was null");
            return _instance;
        }
    }

    //private float health = 10;
    public override float Health { get; set; } = 100f;

    //private float dmg = 1;
    public override float DMG { get; set; } = 1f;

    /// <summary>
    /// List of minions. dead and alive
    /// </summary>
    private readonly HashSet<GermanSoldier> _minionsInRange = new();

    bool ressurectCooldown = false;
    readonly float ressurectTimer = 5f;
    RefreshCooldown RefreshRessurect => () => ressurectCooldown = false;

    /// <summary>
    /// The detection dist for viewing both player and finding dead zombies.
    /// </summary>
    readonly float detectDisctance = 25f;

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

    private void Update()
    {
        if (!ressurectCooldown)
        {
            RessurectMinions();
            RefreshRessurect.DelayedExecution(delay: ressurectTimer);
        }
    }

    // if a soldier enters the trigger collider, make it a minion
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<GermanSoldier>(out var g)) return;
        else
        {
            g.isHarveyMinion = true;
            //if (!_minions.Contains(g)) _minions.Add(g);
            _minionsInRange.Add(g);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<GermanSoldier>(out var soldier))
            _minionsInRange.Remove(soldier);
    }

    void RessurectMinions()
    {
        foreach (var g in _minionsInRange) { g.enabled = true; g.Revive(); }
        ressurectCooldown = true;
    }
}
