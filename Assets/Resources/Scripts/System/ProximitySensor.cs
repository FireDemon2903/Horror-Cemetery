using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modified code from https://forum.unity.com/threads/how-would-i-check-if-gameobject-with-tag-is-in-range.872428/#post-5740114
/// </summary>
public class ProximitySensor : MonoBehaviour
{
    public string TagMask;
    private HashSet<GameObject> _inrange = new HashSet<GameObject>();

    void OnDisable()
    {
        _inrange.Clear();
    }

    void OnTriggerEnter(Collider c) //change to 2d for 2d
    {
        if (!c.gameObject.CompareTag(this.TagMask)) return;

        _inrange.Add(c.gameObject);
    }

    void OnTriggerExit(Collider c) //change to 2d for 2d
    {
        _inrange.Remove(c.gameObject);
    }

    public bool IsInRange(GameObject go)
    {
        return go != null && _inrange.Contains(go);
    }
}