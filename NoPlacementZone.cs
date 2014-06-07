using System;
using System.Collections.Generic;
using UnityEngine;

public class NoPlacementZone : MonoBehaviour
{
    public static List<NoPlacementZone> _zones;

    public static void AddZone(NoPlacementZone zone)
    {
        if (_zones == null)
        {
            _zones = new List<NoPlacementZone>();
        }
        if (!_zones.Contains(zone))
        {
            _zones.Add(zone);
        }
    }

    public void Awake()
    {
        AddZone(this);
    }

    public float GetRadius()
    {
        return base.transform.localScale.x;
    }

    public void OnDestroy()
    {
        RemoveZone(this);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0.3f, 0.1f);
        Gizmos.DrawSphere(base.transform.position, this.GetRadius());
        Gizmos.color = Color.green;
        Gizmos.DrawCube(base.transform.position, (Vector3) (Vector3.one * 0.5f));
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0.3f, 0.8f);
        Gizmos.DrawWireSphere(base.transform.position, this.GetRadius());
        Gizmos.color = Color.green;
        Gizmos.DrawCube(base.transform.position, (Vector3) (Vector3.one * 0.5f));
    }

    public static void RemoveZone(NoPlacementZone zone)
    {
        if (_zones.Contains(zone))
        {
            _zones.Remove(zone);
        }
    }

    public static bool ValidPos(Vector3 pos)
    {
        foreach (NoPlacementZone zone in _zones)
        {
            if (Vector3.Distance(pos, zone.transform.position) <= zone.GetRadius())
            {
                return false;
            }
        }
        return true;
    }
}

