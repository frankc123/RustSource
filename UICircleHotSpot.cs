using System;
using UnityEngine;

public class UICircleHotSpot : UIHotSpot
{
    public Vector3 center;
    private const UIHotSpot.Kind kKind = UIHotSpot.Kind.Circle;
    public float radius;

    public UICircleHotSpot() : base(UIHotSpot.Kind.Circle, true)
    {
        this.radius = 0.5f;
    }

    internal Bounds? Internal_CalculateBounds(bool moved)
    {
        float x = this.radius * 2f;
        return new Bounds(this.center, new Vector3(x, x, 0f));
    }

    internal bool Internal_RaycastRef(Ray ray, ref UIHotSpot.Hit hit)
    {
        if (this.radius != 0f)
        {
            float num;
            Vector2 vector;
            Plane plane = new Plane(UIHotSpot.forward, this.center);
            if (!plane.Raycast(ray, out num))
            {
                hit = new UIHotSpot.Hit();
                return false;
            }
            hit.point = ray.GetPoint(num);
            hit.normal = !plane.GetSide(ray.origin) ? UIHotSpot.backward : UIHotSpot.forward;
            vector.x = hit.point.x - this.center.x;
            vector.y = hit.point.y - this.center.y;
            float f = (vector.x * vector.x) + (vector.y * vector.y);
            if (f < (this.radius * this.radius))
            {
                hit.distance = Mathf.Sqrt(f);
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = base.gizmoMatrix * Matrix4x4.TRS(this.center, Quaternion.identity, new Vector3(1f, 1f, 0.0001f));
        Gizmos.color = base.gizmoColor;
        Gizmos.DrawWireSphere(Vector3.zero, this.radius);
    }

    public float size
    {
        get
        {
            return (this.radius * 2f);
        }
        set
        {
            this.radius = value / 2f;
        }
    }
}

