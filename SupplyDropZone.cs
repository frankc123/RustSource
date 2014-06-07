using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SupplyDropZone : MonoBehaviour
{
    public static List<SupplyDropZone> _dropZones;
    public float radius = 100f;

    public void Awake()
    {
        Object.Destroy(base.gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(base.transform.position, this.radius);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(base.transform.position, this.radius);
    }
}

