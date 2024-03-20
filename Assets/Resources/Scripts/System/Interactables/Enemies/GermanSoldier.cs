using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GermanSoldier : BaseEnemy
{
    public override void TakeDMG(PlayerControler DMGSource)
    {
        base.TakeDMG(DMGSource);
    }

    public override void DealDMG(PlayerControler DMGTarget)
    {
        base.DealDMG(DMGTarget);
    }

    private void FixedUpdate()
    {
        // If player in sight, chase

        gameObject.SightTest(PlayerControler.Instance.gameObject, detectDisctance);

        // else random movement
    }


    // Make sight test
    

}
