using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions
{
    public static bool SightTest(this GameObject me, GameObject other, float viewDist, int layer=0)
    {
        Debug.DrawRay(me.transform.position, (other.transform.position - me.transform.position).normalized * viewDist);

        return Physics.Raycast(me.transform.position, other.transform.position - me.transform.position, out var hitInfo, viewDist) && hitInfo.transform == other.transform;
    }
}
