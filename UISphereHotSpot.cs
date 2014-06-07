using System;
using UnityEngine;

public class UISphereHotSpot : UIHotSpot
{
    public Vector3 center;
    private const UIHotSpot.Kind kKind = UIHotSpot.Kind.Sphere;
    public float radius;

    public UISphereHotSpot() : base(UIHotSpot.Kind.Sphere, true)
    {
        this.radius = 0.5f;
    }

    internal Bounds? Internal_CalculateBounds(bool moved)
    {
        float x = this.radius * 2f;
        return new Bounds(this.center, new Vector3(x, x, x));
    }

    internal bool Internal_RaycastRef(Ray ray, ref UIHotSpot.Hit hit)
    {
        float num;
        float num2;
        IntersectHint message = Intersect3D.RayCircleInfiniteForward(ray, this.center, this.radius, out num, out num2);
        switch (message)
        {
            case IntersectHint.Touching:
            case IntersectHint.Thru:
            case IntersectHint.Entry:
                hit.distance = Mathf.Min(num, num2);
                if ((hit.distance >= 0f) || ((hit.distance = Mathf.Max(num, num2)) >= 0f))
                {
                    hit.point = ray.GetPoint(hit.distance);
                    hit.normal = Vector3.Normalize(hit.point - this.center);
                    return true;
                }
                return false;
        }
        Debug.Log(message, this);
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = base.gizmoMatrix;
        Gizmos.color = base.gizmoColor;
        Gizmos.DrawWireSphere(this.center, this.radius);
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

