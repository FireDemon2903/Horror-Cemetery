// Ignore Spelling: DMG

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GameManager;

/// <summary>
/// Mini-boss
/// Illusionist
/// Upon entering combat, spawns illusions that also attacks the player
/// Ranged combatant that throws grenades
/// Only dies when real one is killed
/// Illusions can be destroyed
/// </summary>
public class Harry : BaseEnemy
{
    public override float DMG { get; set; } = 0f;               // throws grenades who do damage
    public override float Health { get; set; } = 10f;

    GameObject HarryPrefab;
    // maybe use object pooling?
    GameObject GrenadePrefab;

    const int numIllusions = 10;
    const float spawnDist = 10f;
    public bool IsReal = true;
    public Harry owner = null;

    bool throwCooldown = false;
    RefreshCooldown RefreshThrow;

    bool positionCooldown = false;
    RefreshCooldown ResetPos;

    MoveMode Move;

    internal readonly List<Harry> _harries;

    public override void Awake()
    {
        base.Awake();

        HarryPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Harry");
        GrenadePrefab = Resources.Load<GameObject>("Prefabs/Weapon/Grenade");

        RefreshThrow = () => throwCooldown = false;
        ResetPos = () => positionCooldown = false;
    }

    private void FixedUpdate()
    {
        if (positionCooldown)
        {
            positionCooldown = false;
            ResetPos.DelayedExecution(5f);
            Move?.Invoke();
        }
    }

    private void Update()
    {
        if (!throwCooldown)
        {
            throwCooldown = true;
            RefreshThrow.DelayedExecution(1f);
            Attack();
        }
    }

    //TODO assign move for Harry, when player gets close
    // player gets close
    // spawn illusions
    // assign move method to self and illusions
    // enable trigger collider on illusions so player can walk through them
    // other?

    void SpawnIllusions()
    {
        // get positions
        Vector3[] vectors = Extensions.GetCirclePositions(gameObject.transform.position, spawnDist, numIllusions);

        // spawns illusions facing same dir as center
        for (int i = 0; i < numIllusions; i++)
        {
            GameObject newIll = Instantiate(HarryPrefab, vectors[i], transform.rotation);
            var h = newIll.GetComponent<Harry>();
            h.IsReal = false;

            _harries.Add(h);
        }
    }

    public override void Die()
    {
        // Destroy all illusions
        for (int i = 0;i < numIllusions; i++)
        {
            Destroy(_harries[i].gameObject);
        }

        // destroy self
        base.Die();
    }

    //TODO tweak physics
    void Attack()
    {
        //spawn grenade
        GameObject grenade = Instantiate(GrenadePrefab, position: transform.position, transform.rotation);

        // get dir to player
        Vector3 vec = (PlayerController.Instance.Position - gameObject.transform.position).normalized;

        vec.y += 2f;

        vec = vec.normalized;

        // add force upwards and towards the player
        grenade.GetComponent<Rigidbody>().AddForce(vec * 20f, ForceMode.VelocityChange);
    }

    void MoveMeth()
    {
        Vector3 pos;

        try
        {
            pos = owner._harries[_harries.IndexOf(this) + 1].gameObject.transform.position;
        }
        catch (IndexOutOfRangeException)
        {
            pos = owner._harries[0].gameObject.transform.position;
        }

        mAgent.SetDestination(pos);
    }

}
