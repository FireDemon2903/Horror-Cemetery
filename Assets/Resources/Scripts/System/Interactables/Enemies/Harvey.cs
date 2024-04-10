// Ignore Spelling: DMG

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using static GameManager;

/// <summary>
/// Unique mini-boss enemy. has the ability to raise fallen zombies into stronger, faster versions of themselves.
/// This enemy does not deal damage at the moment.
/// If a GermanSoldier wanders into the range of Harvey, then it becomes his minion.
/// It will continue to be resurrected by Harvey every x seconds, until Harvey is killed.
/// s
/// </summary>

// trigger collider
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

    public override float DMG { get; set; }
    public override float Health { get; set; }

    /// <summary>
    /// List of minions. dead and alive
    /// </summary>
    private HashSet<GermanSoldier> _minions = new();
    private HashSet<GermanSoldier> _minionsInRange = new();

    bool attackCooldown = false;
    RefreshCooldown RefreshAttack => () => attackCooldown = false;

    bool ressurectCooldown = false;
    float ressurectTimer = 5f;
    RefreshCooldown RefreshRessurect => () => ressurectCooldown = false;

    /// <summary>
    /// The detection dist for viewing both player and finding dead zombies.
    /// </summary>
    float detectDisctance = 100f;
    //float attackRange = 15f;

    private void Awake()
    {
        // Singleton
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        else { _instance = this; }
    }

    private void Update()
    {
        if (!ressurectCooldown)
        {
            RessurectMinions();
            StartCoroutine(RefreshRessurect.DelayedExecution(delay: ressurectTimer));
        }
        //if (!attackCooldown)
        //{
        //    StartCoroutine(RefreshAttack.DelayedExecution(delay: 5f));
        //}
    }

    // if a soldier enters the trigger collider, make it a minion
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<GermanSoldier>(out var g)) return;
        else
        {
            g.isHarveyMinion = true;
            if (!_minions.Contains(g)) _minions.Add(g);
            _minionsInRange.Add(g);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<GermanSoldier>(out var soldier))
            _minionsInRange.Remove(soldier);
    }

    // try res zombies.
    void RessurectMinions()
    {
        print("Resurrect!");

        ressurectCooldown = true;

        foreach (var g in _minionsInRange) { g.enabled = true; g.Revive(); }
    }
}
