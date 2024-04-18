// Ignore Spelling: DMG

using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

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

    const int ILLUSIONS = 16;
    const float SPAWNDIST = 100f;
    public bool IsReal = true;
    public Harry owner = null;

    bool throwCooldown = false;
    RefreshCooldown RefreshThrow;

    bool positionCooldown = false;
    RefreshCooldown ResetPos;

    MoveMode Move;

    internal readonly List<Harry> _harries = new();

    public override void Awake()
    {
        base.Awake();

        HarryPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Harry");
        GrenadePrefab = Resources.Load<GameObject>("Prefabs/Items/StickGrenade");

        RefreshThrow = () => throwCooldown = false;
        ResetPos = () => positionCooldown = false;

        IsReal = true;
    }


    private void Start()
    {
        if (IsReal) SpawnIllusions();
    }

    private void FixedUpdate()
    {
        if (!positionCooldown)
        {
            positionCooldown = true;
            ResetPos.DelayedExecution(5f);
            Move?.Invoke();
        }
    }

    private void Update()
    {
        if (!throwCooldown)
        {
            throwCooldown = true;
            RefreshThrow.DelayedExecution(Random.Range(.5f, 1.5f));
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
        Vector3[] vectors = Extensions.GetCirclePositions(gameObject.transform.position, SPAWNDIST, ILLUSIONS);

        // spawns illusions facing same dir as center
        for (int i = 0; i < ILLUSIONS; i++)
        {
            GameObject newIll = Instantiate(HarryPrefab, vectors[i], transform.rotation);
            newIll.ScaleThis(.75f);

            var h = newIll.GetComponent<Harry>();
            h.IsReal = false;
            //h.Move = h.MoveMeth;
            h.owner = this;

            _harries.Add(h);
        }
    }

    public override void Die()
    {
        print("h: death");

        // Destroy all illusions
        for (int i = 0; i < ILLUSIONS; i++)
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

        //throw up
        // TODO maybe scale by dist from player?
        vec.y += .15f;

        // normalize again
        vec = vec.normalized;

        // add force
        var rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(vec * 250f, ForceMode.VelocityChange);
        //rb.AddTorque(new Vector3(0, 500, 0), ForceMode.VelocityChange);
    }

    //todo move harry
    //void MoveMeth()
    //{
    //    Vector3 pos;
        
    //    try
    //    {
    //        //print(owner._harries.IndexOf(this));
    //        pos = owner._harries[_harries.IndexOf(this) + 1].gameObject.transform.position;
    //    }
    //    catch (IndexOutOfRangeException)
    //    {
    //        pos = owner._harries[0].gameObject.transform.position;
    //    }

    //    mAgent.SetDestination(pos);
    //}

}
