using System;
using UnityEngine;

public class UIBoxHotSpot : UIHotSpot
{
    public Vector3 center;
    private const UIHotSpot.Kind kKind = UIHotSpot.Kind.Box;
    public Vector3 size;

    public UIBoxHotSpot() : base(UIHotSpot.Kind.Box, true)
    {
    }

    internal Bounds? Internal_CalculateBounds(bool moved)
    {
        return new Bounds(this.center, this.size);
    }

    internal bool Internal_RaycastRef(Ray ray, ref UIHotSpot.Hit hit)
    {
        Bounds bounds = new Bounds(this.center, this.size);
        if (bounds.IntersectRay(ray, out hit.distance))
        {
            hit.point = ray.GetPoint(hit.distance);
            hit.normal = -ray.direction;
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = base.gizmoMatrix;
        Gizmos.color = base.gizmoColor;
        Gizmos.DrawWireCube(this.center, this.size);
    }
}

