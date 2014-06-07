using System;
using UnityEngine;

public class UIRectHotSpot : UIHotSpot
{
    public Vector3 center;
    private const float kEpsilon = 2.802597E-45f;
    private const UIHotSpot.Kind kKind = UIHotSpot.Kind.Rect;
    public Vector2 size;

    public UIRectHotSpot() : base(UIHotSpot.Kind.Rect, true)
    {
        this.size = Vector2.one;
    }

    internal Bounds? Internal_CalculateBounds(bool moved)
    {
        return new Bounds(this.center, (Vector3) this.size);
    }

    internal bool Internal_RaycastRef(Ray ray, ref UIHotSpot.Hit hit)
    {
        if ((this.size.x >= 2.802597E-45f) && (this.size.y >= 2.802597E-45f))
        {
            float num;
            Vector2 vector;
            hit.normal = UIHotSpot.forward;
            Plane plane = new Plane(UIHotSpot.forward, this.center);
            if (!plane.Raycast(ray, out num))
            {
                hit = new UIHotSpot.Hit();
                return false;
            }
            hit.point = ray.GetPoint(num);
            vector.x = (hit.point.x >= this.center.x) ? (hit.point.x - this.center.x) : (this.center.x - hit.point.x);
            vector.y = (hit.point.y >= this.center.y) ? (hit.point.y - this.center.y) : (this.center.y - hit.point.y);
            if (((vector.x * 2f) <= this.size.x) && ((vector.y * 2f) <= this.size.y))
            {
                hit.distance = Mathf.Sqrt((vector.x * vector.x) + (vector.y * vector.y));
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = base.gizmoMatrix;
        Gizmos.color = base.gizmoColor;
        Gizmos.DrawWireCube(this.center, (Vector3) this.size);
    }
}

