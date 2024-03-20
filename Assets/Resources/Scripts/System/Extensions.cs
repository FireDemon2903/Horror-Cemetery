using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions
{
    public static void SightTest(this GameObject me, GameObject other, float dist)
    {
        // detectDistance is your detect radius.
        if (Vector3.Distance(me.transform.position, other.transform.position) <= dist)
        {
            new Ray(other.transform.position, me.transform.position - other.transform.position);
            // if ray on wall , ignore it.
            // todo
        }
    }
}
