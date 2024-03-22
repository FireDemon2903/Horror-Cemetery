using UnityEngine;

public static class Extensions
{
    public static bool SightTest(this GameObject me, GameObject other, float viewDist, int layer=0)
    {
        Debug.DrawRay(me.transform.position, (other.transform.position - me.transform.position).normalized * viewDist);

        return Physics.Raycast(me.transform.position, other.transform.position - me.transform.position, out var hitInfo, viewDist) && hitInfo.transform == other.transform;
    }

    public static void DebugVelocity(this Rigidbody rb, Color color, bool write=false)
    {
        Debug.DrawRay(rb.position, rb.velocity, color);
        if (write) { Debug.Log($"{rb.gameObject.name}'s velocity: {rb.velocity}"); }
    }

    /// <summary>
    /// Deal damage to the target, it it is alive (implements IAlive)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static void TryDealDamage(this GameObject target, IDamage source)
    {
        if (target.TryGetComponent(out IAlive enemy)) enemy.TakeDMG(from: source);
    }

}
