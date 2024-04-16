using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mini-boss
/// Illusionist
/// Upon entering combat, spawns illusions that also attacks the player
/// Only dies when real one is killed
/// Illusions can be destroyed
/// </summary>
public class Harry : BaseEnemy
{
    public override float DMG { get; set; } = 1f;
    public override float Health { get; set; } = 10f;

    private bool IsReal;


    void SpawnIllusions()
    {

    }

}
