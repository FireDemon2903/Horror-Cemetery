using System;
using System.Collections;
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

    /// <summary>
    /// Deal damage to the target, it is alive (implements IAlive)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static void TryDealDamage(this GameObject target, IDamage source)
    {
        if (target != null && target.TryGetComponent(out IAlive enemy)) enemy.TakeDMG(from: source);
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

    /// <summary>
    /// Stops the momentum of the body
    /// </summary>
    /// <param name="body"></param>
    public static void Stop(this Rigidbody body)
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
    }

}
