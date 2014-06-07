using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class UIHotSpot : MonoBehaviour
{
    private Bounds _bounds;
    private Bounds _lastBoundsEntered;
    private readonly bool configuredInLocalSpace;
    private int index = -1;
    private bool justAdded;
    private const float k2Cos45 = 1.414214f;
    private const float kCos45 = 0.7071068f;
    public readonly Kind kind;
    private const Kind kKindFlag_2D = Kind.Circle;
    private const Kind kKindFlag_3D = Kind.Sphere;
    private const Kind kKindFlag_Axial = Kind.Rect;
    private const Kind kKindFlag_Convex = Kind.Convex;
    private const Kind kKindFlag_Radial = Kind.Circle;
    private Matrix4x4 lastLocal;
    private Matrix4x4 lastWorld;
    protected static readonly Plane localPlane = new Plane(Vector3.back, Vector3.zero);
    private bool once;
    private UIPanel panel;
    private Matrix4x4 toLocal;
    private Matrix4x4 toWorld;

    protected UIHotSpot(Kind kind, bool configuredInLocalSpace)
    {
        this.kind = kind;
        this.configuredInLocalSpace = configuredInLocalSpace;
    }

    public bool As(out UIBoxHotSpot cast)
    {
        if (this.kind == Kind.Box)
        {
            cast = (UIBoxHotSpot) this;
            return true;
        }
        cast = null;
        return false;
    }

    public bool As(out UIBrushHotSpot cast)
    {
        if (this.kind == Kind.Brush)
        {
            cast = (UIBrushHotSpot) this;
            return true;
        }
        cast = null;
        return false;
    }

    public bool As(out UICircleHotSpot cast)
    {
        if (this.kind == Kind.Circle)
        {
            cast = (UICircleHotSpot) this;
            return true;
        }
        cast = null;
        return false;
    }

    public bool As(out UIConvexHotSpot cast)
    {
        if (this.kind == Kind.Convex)
        {
            cast = (UIConvexHotSpot) this;
            return true;
        }
        cast = null;
        return false;
    }

    public bool As(out UIRectHotSpot cast)
    {
        if (this.kind == Kind.Rect)
        {
            cast = (UIRectHotSpot) this;
            return true;
        }
        cast = null;
        return false;
    }

    public bool As(out UISphereHotSpot cast)
    {
        if (this.kind == Kind.Sphere)
        {
            cast = (UISphereHotSpot) this;
            return true;
        }
        cast = null;
        return false;
    }

    public bool ClosestRaycast(Ray ray, ref Hit hit)
    {
        Hit hit2;
        if (this.Raycast(ray, out hit2) && (hit2.distance < hit.distance))
        {
            hit = hit2;
            return true;
        }
        return false;
    }

    public static void ConvertRaycastHit(ref Ray ray, ref RaycastHit raycastHit, out Hit hit)
    {
        hit.collider = raycastHit.collider;
        hit.hotSpot = hit.collider.GetComponent<UIHotSpot>();
        hit.isCollider = hit.hotSpot == 0;
        if (hit.isCollider)
        {
            hit.panel = UIPanel.Find(hit.collider.transform);
        }
        else
        {
            hit.panel = (hit.hotSpot.panel == null) ? UIPanel.Find(hit.collider.transform) : hit.hotSpot.panel;
        }
        hit.ray = ray;
        hit.distance = raycastHit.distance;
        hit.point = raycastHit.point;
        hit.normal = raycastHit.normal;
    }

    private bool DisableHotSpot()
    {
        return Global.Remove(this);
    }

    private bool DoRaycastRef(Ray ray, ref Hit hit)
    {
        switch (this.kind)
        {
            case Kind.Circle:
                return ((UICircleHotSpot) this).Internal_RaycastRef(ray, ref hit);

            case Kind.Rect:
                return ((UIRectHotSpot) this).Internal_RaycastRef(ray, ref hit);

            case Kind.Convex:
                return ((UIConvexHotSpot) this).Internal_RaycastRef(ray, ref hit);

            case Kind.Sphere:
                return ((UISphereHotSpot) this).Internal_RaycastRef(ray, ref hit);

            case Kind.Box:
                return ((UIBoxHotSpot) this).Internal_RaycastRef(ray, ref hit);

            case Kind.Brush:
                return ((UIBrushHotSpot) this).Internal_RaycastRef(ray, ref hit);
        }
        throw new NotImplementedException();
    }

    private bool EnableHotSpot()
    {
        return Global.Add(this);
    }

    protected virtual void HotSpotInit()
    {
    }

    private bool LocalRaycastRef(Ray worldRay, ref Hit hit)
    {
        Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
        Vector3 origin = worldToLocalMatrix.MultiplyPoint(worldRay.origin);
        Ray ray = new Ray(origin, worldToLocalMatrix.MultiplyVector(worldRay.direction));
        Hit invalid = Hit.invalid;
        if (this.DoRaycastRef(ray, ref invalid))
        {
            worldToLocalMatrix = base.transform.localToWorldMatrix;
            hit.point = worldToLocalMatrix.MultiplyPoint(invalid.point);
            hit.normal = worldToLocalMatrix.MultiplyVector(invalid.normal);
            hit.ray = worldRay;
            hit.distance = Vector3.Dot(worldRay.direction, hit.point - worldRay.origin);
            hit.hotSpot = this;
            hit.panel = this.panel;
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        if (this.panel != null)
        {
            UIPanel panel = this.panel;
            this.panel = null;
            UIPanel.UnregisterHotSpot(panel, this);
        }
    }

    protected void OnDisable()
    {
        if (this.once)
        {
            this.DisableHotSpot();
        }
    }

    protected void OnEnable()
    {
        if ((this.panel != null) && this.panel.enabled)
        {
            this.EnableHotSpot();
        }
    }

    internal void OnPanelDestroy()
    {
        UIPanel panel = this.panel;
        this.panel = null;
        if ((base.enabled && (panel != null)) && panel.enabled)
        {
            this.OnPanelDisable();
        }
    }

    internal void OnPanelDisable()
    {
        if (base.enabled)
        {
            this.DisableHotSpot();
        }
    }

    internal void OnPanelEnable()
    {
        if (base.enabled)
        {
            this.EnableHotSpot();
        }
    }

    public bool Raycast(Ray ray, out Hit hit)
    {
        hit = Hit.invalid;
        return this.RaycastRef(ray, ref hit);
    }

    public static bool Raycast(Ray ray, out Hit hit, float distance)
    {
        return Global.Raycast(ray, out hit, distance);
    }

    public bool RaycastRef(Ray ray, ref Hit hit)
    {
        if (this.configuredInLocalSpace)
        {
            return this.LocalRaycastRef(ray, ref hit);
        }
        return this.DoRaycastRef(ray, ref hit);
    }

    private void SetBounds(bool moved, Bounds bounds, bool worldEquals)
    {
        if (this.configuredInLocalSpace)
        {
            if ((this._lastBoundsEntered != bounds) || !worldEquals)
            {
                this._lastBoundsEntered = bounds;
                AABBox.Transform3x4(ref bounds, ref this.toWorld, out this._bounds);
            }
        }
        else
        {
            this._lastBoundsEntered = bounds;
            this._bounds = bounds;
        }
    }

    private void Start()
    {
        this.panel = UIPanel.Find(base.transform);
        if (this.panel != null)
        {
            UIPanel.RegisterHotSpot(this.panel, this);
        }
        else
        {
            Debug.LogWarning("Did not find panel!", this);
        }
    }

    public UIBoxHotSpot asBox
    {
        get
        {
            return ((this.kind != Kind.Box) ? null : ((UIBoxHotSpot) this));
        }
    }

    public UIBrushHotSpot asBrush
    {
        get
        {
            return ((this.kind != Kind.Brush) ? null : ((UIBrushHotSpot) this));
        }
    }

    public UICircleHotSpot asCircle
    {
        get
        {
            return ((this.kind != Kind.Circle) ? null : ((UICircleHotSpot) this));
        }
    }

    public UIConvexHotSpot asConvex
    {
        get
        {
            return ((this.kind != Kind.Convex) ? null : ((UIConvexHotSpot) this));
        }
    }

    public UIRectHotSpot asRect
    {
        get
        {
            return ((this.kind != Kind.Rect) ? null : ((UIRectHotSpot) this));
        }
    }

    public UISphereHotSpot asSphere
    {
        get
        {
            return ((this.kind != Kind.Sphere) ? null : ((UISphereHotSpot) this));
        }
    }

    protected static Vector3 backward
    {
        get
        {
            Vector3 vector;
            vector.x = 0f;
            vector.y = 0f;
            vector.z = 1f;
            return vector;
        }
    }

    private UIBoxHotSpot boxUS
    {
        get
        {
            return (UIBoxHotSpot) this;
        }
    }

    private UIBrushHotSpot brushUS
    {
        get
        {
            return (UIBrushHotSpot) this;
        }
    }

    public Vector3 center
    {
        get
        {
            Kind kind = this.kind;
            if (kind != Kind.Circle)
            {
                if (kind == Kind.Rect)
                {
                    return ((UIRectHotSpot) this).center;
                }
                if (kind == Kind.Sphere)
                {
                    return ((UISphereHotSpot) this).center;
                }
                if (kind == Kind.Box)
                {
                    return ((UIBoxHotSpot) this).center;
                }
                return new Vector3();
            }
            return ((UICircleHotSpot) this).center;
        }
        set
        {
            switch (this.kind)
            {
                case Kind.Circle:
                    ((UICircleHotSpot) this).center = value;
                    break;

                case Kind.Rect:
                    ((UIRectHotSpot) this).center = value;
                    break;

                case Kind.Sphere:
                    ((UISphereHotSpot) this).center = value;
                    break;

                case Kind.Box:
                    ((UIBoxHotSpot) this).center = value;
                    break;
            }
        }
    }

    private UICircleHotSpot circleUS
    {
        get
        {
            return (UICircleHotSpot) this;
        }
    }

    private UIConvexHotSpot convexUS
    {
        get
        {
            return (UIConvexHotSpot) this;
        }
    }

    protected static Vector3 forward
    {
        get
        {
            Vector3 vector;
            vector.x = 0f;
            vector.y = 0f;
            vector.z = -1f;
            return vector;
        }
    }

    protected Color gizmoColor
    {
        get
        {
            return Color.green;
        }
    }

    protected Matrix4x4 gizmoMatrix
    {
        get
        {
            if (this.index == -1)
            {
                return (!this.configuredInLocalSpace ? Matrix4x4.identity : base.transform.localToWorldMatrix);
            }
            return (!this.configuredInLocalSpace ? Matrix4x4.identity : this.toWorld);
        }
    }

    public bool isBox
    {
        get
        {
            return (this.kind == Kind.Box);
        }
    }

    public bool isBrush
    {
        get
        {
            return (this.kind == Kind.Brush);
        }
    }

    public bool isCircle
    {
        get
        {
            return (this.kind == Kind.Circle);
        }
    }

    public bool isConvex
    {
        get
        {
            return (this.kind == Kind.Convex);
        }
    }

    public bool isRect
    {
        get
        {
            return (this.kind == Kind.Rect);
        }
    }

    public bool isSphere
    {
        get
        {
            return (this.kind == Kind.Sphere);
        }
    }

    private UIRectHotSpot rectUS
    {
        get
        {
            return (UIRectHotSpot) this;
        }
    }

    public Vector3 size
    {
        get
        {
            Vector3 vector;
            Kind kind = this.kind;
            if (kind != Kind.Circle)
            {
                if (kind == Kind.Rect)
                {
                    return (Vector3) ((UIRectHotSpot) this).size;
                }
                if (kind != Kind.Sphere)
                {
                    if (kind == Kind.Box)
                    {
                        return ((UIBoxHotSpot) this).size;
                    }
                    return new Vector3();
                }
            }
            else
            {
                vector.x = ((UICircleHotSpot) this).radius * 2f;
                vector.y = vector.x;
                vector.z = 0f;
                return vector;
            }
            vector.x = ((UICircleHotSpot) this).radius * 1.414214f;
            vector.y = vector.z = vector.x;
            return vector;
        }
        set
        {
            switch (this.kind)
            {
                case Kind.Circle:
                    value.y *= 0.7071068f;
                    value.x *= 0.7071068f;
                    ((UICircleHotSpot) this).radius = Mathf.Sqrt((value.x * value.x) + (value.y * value.y)) / 2f;
                    break;

                case Kind.Rect:
                    ((UIRectHotSpot) this).size = new Vector2(value.x, value.y);
                    break;

                case Kind.Sphere:
                    value.z *= 0.7071068f;
                    value.y *= 0.7071068f;
                    value.x *= 0.7071068f;
                    ((UISphereHotSpot) this).radius = Mathf.Sqrt(((value.x * value.x) + (value.y * value.y)) + (value.z * value.z)) / 2f;
                    break;

                case Kind.Box:
                    ((UIBoxHotSpot) this).size = value;
                    break;
            }
        }
    }

    private UISphereHotSpot sphereUS
    {
        get
        {
            return (UISphereHotSpot) this;
        }
    }

    public UIPanel uipanel
    {
        get
        {
            return this.panel;
        }
    }

    public Vector3 worldCenter
    {
        get
        {
            Vector3 center;
            Kind kind = this.kind;
            if (kind != Kind.Circle)
            {
                if (kind != Kind.Rect)
                {
                    if (kind != Kind.Sphere)
                    {
                        if (kind != Kind.Box)
                        {
                            throw new NotImplementedException();
                        }
                        center = ((UIBoxHotSpot) this).center;
                    }
                    else
                    {
                        center = ((UISphereHotSpot) this).center;
                    }
                    goto Label_0079;
                }
            }
            else
            {
                center = ((UICircleHotSpot) this).center;
                goto Label_0079;
            }
            center = ((UIRectHotSpot) this).center;
        Label_0079:
            return base.transform.TransformPoint(center);
        }
    }

    private static class Global
    {
        private static bool allAny;
        private static Bounds allBounds;
        private static int allCount;
        private static bool anyAddedRecently;
        private static bool anyRemovedRecently;
        private static List<UIBoxHotSpot> Box;
        private static List<UIBrushHotSpot> Brush;
        private static List<UICircleHotSpot> Circle;
        private static List<UIConvexHotSpot> Convex;
        private static int lastStepFrame = -2147483648;
        private static List<UIRectHotSpot> Rect;
        private static List<UISphereHotSpot> Sphere;
        private static bool validBounds;

        public static bool Add(UIHotSpot hotSpot)
        {
            if (hotSpot.index != -1)
            {
                return false;
            }
            switch (hotSpot.kind)
            {
                case UIHotSpot.Kind.Circle:
                    Circle.Add((UICircleHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Rect:
                    Rect.Add((UIRectHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Convex:
                    Convex.Add((UIConvexHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Sphere:
                    Sphere.Add((UISphereHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Box:
                    Box.Add((UIBoxHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Brush:
                    Brush.Add((UIBrushHotSpot) hotSpot);
                    break;

                default:
                    throw new NotImplementedException();
            }
            hotSpot.justAdded = true;
            if (!hotSpot.once)
            {
                hotSpot.HotSpotInit();
                hotSpot.once = true;
            }
            return true;
        }

        private static bool DoRaycast(Ray ray, out UIHotSpot.Hit hit, float dist)
        {
            float num;
            hit = UIHotSpot.Hit.invalid;
            UIHotSpot.Hit invalid = UIHotSpot.Hit.invalid;
            bool flag = true;
            Vector3 origin = ray.origin;
            int allCount = UIHotSpot.Global.allCount;
            if (Circle.any)
            {
                for (int i = 0; i < Circle.count; i++)
                {
                    UICircleHotSpot spot = Circle.array[i];
                    if ((spot._bounds.Contains(origin) || (spot._bounds.IntersectRay(ray, out num) && (num < dist))) && ((spot.panel.InsideClippingRect(ray, lastStepFrame) && spot.RaycastRef(ray, ref invalid)) && (invalid.distance < dist)))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        dist = invalid.distance;
                        hit = invalid;
                        if (--allCount == 0)
                        {
                            return true;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return !flag;
                    }
                }
            }
            if (Rect.any)
            {
                for (int j = 0; j < Rect.count; j++)
                {
                    UIRectHotSpot spot2 = Rect.array[j];
                    if ((spot2._bounds.Contains(origin) || (spot2._bounds.IntersectRay(ray, out num) && (num < dist))) && ((spot2.panel.InsideClippingRect(ray, lastStepFrame) && spot2.RaycastRef(ray, ref invalid)) && (invalid.distance < dist)))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        dist = invalid.distance;
                        hit = invalid;
                        if (--allCount == 0)
                        {
                            return true;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return !flag;
                    }
                }
            }
            if (Convex.any)
            {
                for (int k = 0; k < Convex.count; k++)
                {
                    UIConvexHotSpot spot3 = Convex.array[k];
                    if ((spot3._bounds.Contains(origin) || (spot3._bounds.IntersectRay(ray, out num) && (num < dist))) && ((spot3.panel.InsideClippingRect(ray, lastStepFrame) && spot3.RaycastRef(ray, ref invalid)) && (invalid.distance < dist)))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        dist = invalid.distance;
                        hit = invalid;
                        if (--allCount == 0)
                        {
                            return true;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return !flag;
                    }
                }
            }
            if (Sphere.any)
            {
                for (int m = 0; m < Sphere.count; m++)
                {
                    UISphereHotSpot spot4 = Sphere.array[m];
                    if ((spot4._bounds.Contains(origin) || (spot4._bounds.IntersectRay(ray, out num) && (num < dist))) && ((spot4.panel.InsideClippingRect(ray, lastStepFrame) && spot4.RaycastRef(ray, ref invalid)) && (invalid.distance < dist)))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        dist = invalid.distance;
                        hit = invalid;
                        if (--allCount == 0)
                        {
                            return true;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return !flag;
                    }
                }
            }
            if (Box.any)
            {
                for (int n = 0; n < Box.count; n++)
                {
                    UIBoxHotSpot spot5 = Box.array[n];
                    if ((spot5._bounds.Contains(origin) || (spot5._bounds.IntersectRay(ray, out num) && (num < dist))) && ((spot5.panel.InsideClippingRect(ray, lastStepFrame) && spot5.RaycastRef(ray, ref invalid)) && (invalid.distance < dist)))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        dist = invalid.distance;
                        hit = invalid;
                        if (--allCount == 0)
                        {
                            return true;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return !flag;
                    }
                }
            }
            if (Brush.any)
            {
                for (int num8 = 0; num8 < Brush.count; num8++)
                {
                    UIBrushHotSpot spot6 = Brush.array[num8];
                    if ((spot6._bounds.Contains(origin) || (spot6._bounds.IntersectRay(ray, out num) && (num < dist))) && ((spot6.panel.InsideClippingRect(ray, lastStepFrame) && spot6.RaycastRef(ray, ref invalid)) && (invalid.distance < dist)))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        dist = invalid.distance;
                        hit = invalid;
                        if (--allCount == 0)
                        {
                            return true;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return !flag;
                    }
                }
            }
            throw new InvalidOperationException("Something is messed up. this line should never execute.");
        }

        private static Bounds? DoStep()
        {
            bool flag;
            bool flag2;
            Transform transform;
            bool flag4;
            Bounds bounds = new Bounds();
            bool flag3 = true;
            int allCount = UIHotSpot.Global.allCount;
            if (Circle.any)
            {
                for (int i = 0; i < Circle.count; i++)
                {
                    UICircleHotSpot spot = Circle.array[i];
                    transform = spot.transform;
                    spot.lastWorld = spot.toWorld;
                    spot.toWorld = transform.localToWorldMatrix;
                    spot.lastLocal = spot.toLocal;
                    spot.toLocal = transform.worldToLocalMatrix;
                    flag2 = !(flag4 = !spot.justAdded) || !(flag4 = MatrixEquals(ref spot.toWorld, ref spot.lastWorld));
                    flag = spot.justAdded || (!spot.configuredInLocalSpace ? flag2 : !MatrixEquals(ref spot.toLocal, ref spot.lastLocal));
                    Bounds? nullable = spot.Internal_CalculateBounds(flag);
                    spot.SetBounds(flag2, !nullable.HasValue ? spot._bounds : nullable.Value, flag4);
                    spot.justAdded = false;
                    if (spot._bounds.size != Vector3.zero)
                    {
                        if (!flag3)
                        {
                            bounds.Encapsulate(spot._bounds);
                            if (--allCount == 0)
                            {
                                return new Bounds?(bounds);
                            }
                        }
                        else
                        {
                            if (--allCount == 0)
                            {
                                return new Bounds?(spot._bounds);
                            }
                            flag3 = false;
                            bounds = spot._bounds;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return null;
                    }
                }
            }
            if (Rect.any)
            {
                for (int j = 0; j < Rect.count; j++)
                {
                    UIRectHotSpot spot2 = Rect.array[j];
                    transform = spot2.transform;
                    spot2.lastWorld = spot2.toWorld;
                    spot2.toWorld = transform.localToWorldMatrix;
                    spot2.lastLocal = spot2.toLocal;
                    spot2.toLocal = transform.worldToLocalMatrix;
                    flag2 = !(flag4 = !spot2.justAdded) || !(flag4 = MatrixEquals(ref spot2.toWorld, ref spot2.lastWorld));
                    flag = spot2.justAdded || (!spot2.configuredInLocalSpace ? flag2 : !MatrixEquals(ref spot2.toLocal, ref spot2.lastLocal));
                    Bounds? nullable3 = spot2.Internal_CalculateBounds(flag);
                    spot2.SetBounds(flag2, !nullable3.HasValue ? spot2._bounds : nullable3.Value, flag4);
                    spot2.justAdded = false;
                    if (spot2._bounds.size != Vector3.zero)
                    {
                        if (!flag3)
                        {
                            bounds.Encapsulate(spot2._bounds);
                            if (--allCount == 0)
                            {
                                return new Bounds?(bounds);
                            }
                        }
                        else
                        {
                            if (--allCount == 0)
                            {
                                return new Bounds?(spot2._bounds);
                            }
                            flag3 = false;
                            bounds = spot2._bounds;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return null;
                    }
                }
            }
            if (Convex.any)
            {
                for (int k = 0; k < Convex.count; k++)
                {
                    UIConvexHotSpot spot3 = Convex.array[k];
                    transform = spot3.transform;
                    spot3.lastWorld = spot3.toWorld;
                    spot3.toWorld = transform.localToWorldMatrix;
                    spot3.lastLocal = spot3.toLocal;
                    spot3.toLocal = transform.worldToLocalMatrix;
                    flag2 = !(flag4 = !spot3.justAdded) || !(flag4 = MatrixEquals(ref spot3.toWorld, ref spot3.lastWorld));
                    flag = spot3.justAdded || (!spot3.configuredInLocalSpace ? flag2 : !MatrixEquals(ref spot3.toLocal, ref spot3.lastLocal));
                    Bounds? nullable5 = spot3.Internal_CalculateBounds(flag);
                    spot3.SetBounds(flag2, !nullable5.HasValue ? spot3._bounds : nullable5.Value, flag4);
                    spot3.justAdded = false;
                    if (spot3._bounds.size != Vector3.zero)
                    {
                        if (!flag3)
                        {
                            bounds.Encapsulate(spot3._bounds);
                            if (--allCount == 0)
                            {
                                return new Bounds?(bounds);
                            }
                        }
                        else
                        {
                            if (--allCount == 0)
                            {
                                return new Bounds?(spot3._bounds);
                            }
                            flag3 = false;
                            bounds = spot3._bounds;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return null;
                    }
                }
            }
            if (Sphere.any)
            {
                for (int m = 0; m < Sphere.count; m++)
                {
                    UISphereHotSpot spot4 = Sphere.array[m];
                    transform = spot4.transform;
                    spot4.lastWorld = spot4.toWorld;
                    spot4.toWorld = transform.localToWorldMatrix;
                    spot4.lastLocal = spot4.toLocal;
                    spot4.toLocal = transform.worldToLocalMatrix;
                    flag2 = !(flag4 = !spot4.justAdded) || !(flag4 = MatrixEquals(ref spot4.toWorld, ref spot4.lastWorld));
                    flag = spot4.justAdded || (!spot4.configuredInLocalSpace ? flag2 : !MatrixEquals(ref spot4.toLocal, ref spot4.lastLocal));
                    Bounds? nullable7 = spot4.Internal_CalculateBounds(flag);
                    spot4.SetBounds(flag2, !nullable7.HasValue ? spot4._bounds : nullable7.Value, flag4);
                    spot4.justAdded = false;
                    if (spot4._bounds.size != Vector3.zero)
                    {
                        if (!flag3)
                        {
                            bounds.Encapsulate(spot4._bounds);
                            if (--allCount == 0)
                            {
                                return new Bounds?(bounds);
                            }
                        }
                        else
                        {
                            if (--allCount == 0)
                            {
                                return new Bounds?(spot4._bounds);
                            }
                            flag3 = false;
                            bounds = spot4._bounds;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return null;
                    }
                }
            }
            if (Box.any)
            {
                for (int n = 0; n < Box.count; n++)
                {
                    UIBoxHotSpot spot5 = Box.array[n];
                    transform = spot5.transform;
                    spot5.lastWorld = spot5.toWorld;
                    spot5.toWorld = transform.localToWorldMatrix;
                    spot5.lastLocal = spot5.toLocal;
                    spot5.toLocal = transform.worldToLocalMatrix;
                    flag2 = !(flag4 = !spot5.justAdded) || !(flag4 = MatrixEquals(ref spot5.toWorld, ref spot5.lastWorld));
                    flag = spot5.justAdded || (!spot5.configuredInLocalSpace ? flag2 : !MatrixEquals(ref spot5.toLocal, ref spot5.lastLocal));
                    Bounds? nullable9 = spot5.Internal_CalculateBounds(flag);
                    spot5.SetBounds(flag2, !nullable9.HasValue ? spot5._bounds : nullable9.Value, flag4);
                    spot5.justAdded = false;
                    if (spot5._bounds.size != Vector3.zero)
                    {
                        if (!flag3)
                        {
                            bounds.Encapsulate(spot5._bounds);
                            if (--allCount == 0)
                            {
                                return new Bounds?(bounds);
                            }
                        }
                        else
                        {
                            if (--allCount == 0)
                            {
                                return new Bounds?(spot5._bounds);
                            }
                            flag3 = false;
                            bounds = spot5._bounds;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return null;
                    }
                }
            }
            if (Brush.any)
            {
                for (int num7 = 0; num7 < Brush.count; num7++)
                {
                    UIBrushHotSpot spot6 = Brush.array[num7];
                    transform = spot6.transform;
                    spot6.lastWorld = spot6.toWorld;
                    spot6.toWorld = transform.localToWorldMatrix;
                    spot6.lastLocal = spot6.toLocal;
                    spot6.toLocal = transform.worldToLocalMatrix;
                    flag2 = !(flag4 = !spot6.justAdded) || !(flag4 = MatrixEquals(ref spot6.toWorld, ref spot6.lastWorld));
                    flag = spot6.justAdded || (!spot6.configuredInLocalSpace ? flag2 : !MatrixEquals(ref spot6.toLocal, ref spot6.lastLocal));
                    Bounds? nullable11 = spot6.Internal_CalculateBounds(flag);
                    spot6.SetBounds(flag2, !nullable11.HasValue ? spot6._bounds : nullable11.Value, flag4);
                    spot6.justAdded = false;
                    if (spot6._bounds.size != Vector3.zero)
                    {
                        if (!flag3)
                        {
                            bounds.Encapsulate(spot6._bounds);
                            if (--allCount == 0)
                            {
                                return new Bounds?(bounds);
                            }
                        }
                        else
                        {
                            if (--allCount == 0)
                            {
                                return new Bounds?(spot6._bounds);
                            }
                            flag3 = false;
                            bounds = spot6._bounds;
                        }
                    }
                    else if (--allCount == 0)
                    {
                        return null;
                    }
                }
            }
            throw new InvalidOperationException("Something is messed up. this line should never execute.");
        }

        private static bool MatrixEquals(ref Matrix4x4 a, ref Matrix4x4 b)
        {
            return ((((((a.m03 == b.m03) && (a.m12 == b.m13)) && ((a.m23 == b.m23) && (a.m00 == b.m00))) && (((a.m11 == b.m11) && (a.m22 == b.m22)) && ((a.m01 == b.m01) && (a.m12 == b.m12)))) && ((((a.m20 == b.m20) && (a.m02 == b.m02)) && ((a.m10 == b.m10) && (a.m21 == b.m21))) && (((a.m30 == b.m30) && (a.m31 == b.m31)) && (a.m32 == b.m32)))) && (a.m33 == b.m33));
        }

        public static bool Raycast(Ray ray, out UIHotSpot.Hit hit, float distance)
        {
            float num2;
            if (!allAny)
            {
                hit = UIHotSpot.Hit.invalid;
                return false;
            }
            int frameCount = Time.frameCount;
            if (((lastStepFrame != frameCount) || anyRemovedRecently) || anyAddedRecently)
            {
                Step();
                anyRemovedRecently = anyAddedRecently = false;
            }
            lastStepFrame = frameCount;
            if (!validBounds)
            {
                hit = UIHotSpot.Hit.invalid;
                return false;
            }
            if (allBounds.Contains(ray.origin))
            {
                num2 = 0f;
            }
            else
            {
                if (!allBounds.IntersectRay(ray, out num2) || (num2 > distance))
                {
                    hit = UIHotSpot.Hit.invalid;
                    return false;
                }
                if (num2 != 0f)
                {
                    ray.origin = ray.GetPoint(num2 - 0.001f);
                    num2 = 0f;
                }
            }
            return DoRaycast(ray, out hit, distance);
        }

        public static bool Remove(UIHotSpot hotSpot)
        {
            if (hotSpot.index == -1)
            {
                return false;
            }
            switch (hotSpot.kind)
            {
                case UIHotSpot.Kind.Circle:
                    Circle.Erase((UICircleHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Rect:
                    Rect.Erase((UIRectHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Convex:
                    Convex.Erase((UIConvexHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Sphere:
                    Sphere.Erase((UISphereHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Box:
                    Box.Erase((UIBoxHotSpot) hotSpot);
                    break;

                case UIHotSpot.Kind.Brush:
                    Brush.Erase((UIBrushHotSpot) hotSpot);
                    break;

                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        public static void Step()
        {
            if (allAny)
            {
                Bounds? nullable = DoStep();
                validBounds = nullable.HasValue;
                if (validBounds)
                {
                    allBounds = nullable.Value;
                }
            }
            else
            {
                validBounds = false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct List<THotSpot> where THotSpot: UIHotSpot
        {
            public THotSpot[] array;
            public int count;
            public int capacity;
            public bool any;
            public void Add(THotSpot hotSpot)
            {
                hotSpot.index = this.count++;
                if (hotSpot.index == this.capacity)
                {
                    this.capacity += 8;
                    Array.Resize<THotSpot>(ref this.array, this.capacity);
                }
                this.array[hotSpot.index] = hotSpot;
                this.any = true;
                if (UIHotSpot.Global.allCount++ == 0)
                {
                    UIHotSpot.Global.allAny = true;
                }
                UIHotSpot.Global.anyAddedRecently = true;
            }

            public void Erase(THotSpot hotSpot)
            {
                UIHotSpot.Global.allCount--;
                if (--this.count == hotSpot.index)
                {
                    this.array[hotSpot.index] = null;
                    this.any = this.count > 0;
                    if (!this.any)
                    {
                        UIHotSpot.Global.allAny = UIHotSpot.Global.allCount > 0;
                    }
                }
                else
                {
                    THotSpot local;
                    this.array[hotSpot.index] = local = this.array[this.count];
                    local.index = hotSpot.index;
                    this.array[this.count] = null;
                }
                hotSpot.index = -1;
                UIHotSpot.Global.anyRemovedRecently = true;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Hit
    {
        public UIHotSpot hotSpot;
        public Collider collider;
        public UIPanel panel;
        public Vector3 point;
        public Vector3 normal;
        public Ray ray;
        public float distance;
        public bool isCollider;
        public static readonly UIHotSpot.Hit invalid;
        static Hit()
        {
            UIHotSpot.Hit hit = new UIHotSpot.Hit {
                distance = float.PositiveInfinity,
                ray = new Ray(),
                point = new Vector3(),
                normal = new Vector3()
            };
            invalid = hit;
        }

        public GameObject gameObject
        {
            get
            {
                return (!this.isCollider ? ((this.hotSpot == null) ? null : this.hotSpot.gameObject) : this.collider.gameObject);
            }
        }
        public Transform transform
        {
            get
            {
                return (!this.isCollider ? ((this.hotSpot == null) ? null : this.hotSpot.transform) : this.collider.transform);
            }
        }
        public Component component
        {
            get
            {
                return (!this.isCollider ? ((Component) this.hotSpot) : ((Component) this.collider));
            }
        }
    }

    public enum Kind
    {
        Box = 0x81,
        Brush = 130,
        Circle = 0,
        Convex = 2,
        Rect = 1,
        Sphere = 0x80
    }
}

