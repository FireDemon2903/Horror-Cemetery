using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class Extensions
{
    public static bool SightTest(this GameObject me, GameObject other, float viewDist=Mathf.Infinity)
    {
        Debug.DrawRay(me.transform.position, (other.transform.position - me.transform.position).normalized * viewDist);

        return Physics.Raycast(me.transform.position, other.transform.position - me.transform.position, out var hitInfo, viewDist) && hitInfo.transform == other.transform;
    }

    public static bool SightTest(this Vector3 me, Vector3 other, float viewDist = Mathf.Infinity)
    {
        Debug.DrawRay(me, (other - me).normalized * viewDist);

        return Physics.Raycast(me, other - me, out var hitInfo, viewDist) && hitInfo.point == other;
    }


    public static void DebugVelocity(this Rigidbody rb, Color color, bool write=false)
    {
        Debug.DrawRay(rb.position, rb.velocity, color);
        if (write) { Debug.Log($"{rb.gameObject.name}'s velocity: {rb.velocity}"); }
    }

#nullable enable
    /// <summary>
    /// Deal damage to the target, it is alive (implements IAlive)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static void TryDealDamage(this GameObject target, IDamage source, float? damage=null)
    {
        if (!target.IsUnityNull() && target.TryGetComponent(out IAlive living)) living.TakeDMG(from: source, damage);
    }

    /// <summary>
    /// Dynamically invokes the delegate after a set delay by starting coroutine in GameManager Instance. Params are optional.
    /// </summary>
    /// <param name="action">Delegate to be invoked</param>
    /// <param name="delay">Delay in seconds</param>
    /// <param name="args">Arguments for delegate</param>
    /// <returns>Void</returns>
    public static void DelayedExecution(this Delegate action, float delay, params object?[] args)
    {
        GameManager.Instance.StartCoroutine(Delay(action, delay, args));
    }

    /// <summary>
    /// Helper function for DelayedExecution
    /// </summary>
    /// <returns></returns>
    private static IEnumerator Delay(this Delegate action, float delay, params object?[] args)
    {
        yield return new WaitForSeconds(delay);
        action.DynamicInvoke(args);
    }
#nullable disable

    /// <summary>
    /// Stops the momentum of the body
    /// </summary>
    /// <param name="body"></param>
    public static void Stop(this Rigidbody body)
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Helper function that gets some points in 3D-space around the given center
    /// </summary>
    /// <param name="center">The center from which to make the points</param>
    /// <param name="radius">The distance from the center</param>
    /// <param name="numPositions">Number of new vectors to return</param>
    /// <returns></returns>
    public static Vector3[] GetCirclePositions(Vector3 center, float radius, int numPositions)
    {
        Vector3[] vectors = new Vector3[numPositions];

        for (int i = 0; i < numPositions; i++)
        {
            float angle = 2 * Mathf.PI * i / numPositions;
            float x = center.x + radius * (float)Math.Cos(angle);
            float y = center.y;
            float z = center.z + radius * (float)Math.Sin(angle);
            vectors[i] = new Vector3(x, y, z);
        }
        return vectors;
    }

    /// <summary>
    /// Scale an object by a factor
    /// </summary>
    /// <param name="object"></param>
    /// <param name="factor"></param>
    public static void ScaleThis(this GameObject @object, float factor)
    {
        @object.GetComponent<Transform>().localScale *= factor;
    }


}
