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
    public override float DMG { get { return 0; } set { } }
    public override float Health { get; set; }

    private HashSet<GermanSoldier> soldiers;




}
