using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public static class TransformHelpers
{
    private static readonly Vector2[] upHeightTests;

    static TransformHelpers()
    {
        Vector2[] vectorArray1 = new Vector2[4];
        Vector2 vector = new Vector2 {
            y = -1000f
        };
        vectorArray1[0] = vector;
        Vector2 vector2 = new Vector2 {
            x = 5f,
            y = -1000f
        };
        vectorArray1[1] = vector2;
        Vector2 vector3 = new Vector2 {
            x = 30f,
            y = -2000f
        };
        vectorArray1[2] = vector3;
        Vector2 vector4 = new Vector2 {
            x = 200f,
            y = -4000f
        };
        vectorArray1[3] = vector4;
        upHeightTests = vectorArray1;
    }

    public static float Dist2D(Vector3 a, Vector3 b)
    {
        Vector2 vector;
        vector.x = b.x - a.x;
        vector.y = b.z - a.z;
        return Mathf.Sqrt((vector.x * vector.x) + (vector.y * vector.y));
    }

    public static void DropToGround(this Transform transform, bool useNormal)
    {
        Vector3 vector;
        Vector3 vector2;
        if (transform.GetGroundInfo(out vector, out vector2))
        {
            transform.position = vector;
            if (useNormal)
            {
                transform.rotation = Quaternion.LookRotation(vector2);
            }
        }
    }

    public static bool GetGroundInfo(this Transform transform, out Vector3 pos, out Vector3 normal)
    {
        return GetGroundInfoNoTransform(transform.position, out pos, out normal);
    }

    public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal)
    {
        return GetGroundInfo(startPos, 100f, out pos, out normal);
    }

    public static bool GetGroundInfo(Vector3 startPos, float range, out Vector3 pos, out Vector3 normal)
    {
        RaycastHit hit;
        startPos.y += 0.25f;
        Ray ray = new Ray(startPos, Vector3.down);
        if (Physics.Raycast(ray, out hit, range, -472317957))
        {
            pos = hit.point;
            normal = hit.normal;
            return true;
        }
        pos = startPos;
        normal = Vector3.up;
        return false;
    }

    public static bool GetGroundInfoNavMesh(Vector3 startPos, out Vector3 pos)
    {
        return GetGroundInfoNavMesh(startPos, out pos, 200f);
    }

    public static bool GetGroundInfoNavMesh(Vector3 startPos, out Vector3 pos, float maxVariationFallback)
    {
        return GetGroundInfoNavMesh(startPos, out pos, maxVariationFallback, -1);
    }

    private static bool GetGroundInfoNavMesh(Vector3 startPos, out NavMeshHit hit, float maxVariationFallback, int acceptMask)
    {
        Vector3 vector;
        Vector3 vector2;
        int passableMask = ~acceptMask;
        vector2.x = vector.x = startPos.x;
        vector2.z = vector.z = startPos.z;
        for (int i = 0; i < upHeightTests.Length; i++)
        {
            vector.y = startPos.y + upHeightTests[i].x;
            vector2.y = startPos.y + upHeightTests[i].y;
            if (NavMesh.Raycast(vector, vector2, out hit, passableMask))
            {
                return true;
            }
        }
        return NavMesh.SamplePosition(startPos, out hit, maxVariationFallback, acceptMask);
    }

    public static bool GetGroundInfoNavMesh(Vector3 startPos, out Vector3 pos, float maxVariationFallback, int acceptMask)
    {
        NavMeshHit hit;
        if (GetGroundInfoNavMesh(startPos, out hit, maxVariationFallback, acceptMask))
        {
            pos = hit.position;
            return true;
        }
        pos = startPos;
        return false;
    }

    public static bool GetGroundInfoNoTransform(Vector3 transformOrigin, out Vector3 pos, out Vector3 normal)
    {
        RaycastHit hit;
        Vector3 origin = transformOrigin;
        origin.y += 0.25f;
        Ray ray = new Ray(origin, Vector3.down);
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            pos = hit.point;
            normal = hit.normal;
            return true;
        }
        pos = transformOrigin;
        normal = Vector3.up;
        return false;
    }

    public static Quaternion GetGroundInfoRotation(Quaternion ang, Vector3 y)
    {
        Vector3 vector;
        Vector3 vector2;
        float magnitude = y.magnitude;
        if (Mathf.Approximately(magnitude, 0f))
        {
            y = Vector3.up;
            magnitude = 0f;
        }
        vector2.y = vector2.z = vector.x = vector.y = 0f;
        vector2.x = vector.z = magnitude;
        vector2 = (Vector3) (ang * vector2);
        vector = (Vector3) (ang * vector);
        float num2 = ((vector.x * y.x) + (vector.y * y.y)) + (vector.z * y.z);
        float num3 = ((vector2.x * y.x) + (vector2.y * y.y)) + (vector2.z * y.z);
        if ((num2 * num2) > (num3 * num3))
        {
            return LookRotationForcedUp(vector2, y);
        }
        return LookRotationForcedUp(vector, y);
    }

    public static bool GetGroundInfoTerrainOnly(Vector3 startPos, float range, out Vector3 pos, out Vector3 normal)
    {
        RaycastHit hit;
        startPos.y += 0.25f;
        Ray ray = new Ray(startPos, Vector3.down);
        if (Physics.Raycast(ray, out hit, range + 0.25f) && (hit.collider is TerrainCollider))
        {
            pos = hit.point;
            normal = hit.normal;
            return true;
        }
        pos = startPos;
        normal = Vector3.up;
        return false;
    }

    public static bool GetIDBaseFromCollider(Collider collider, out IDBase id)
    {
        if (collider == null)
        {
            id = null;
            return false;
        }
        id = IDBase.Get(collider);
        if (id != null)
        {
            return true;
        }
        Rigidbody attachedRigidbody = collider.attachedRigidbody;
        if (attachedRigidbody != null)
        {
            id = attachedRigidbody.GetComponent<IDBase>();
            return (bool) id;
        }
        return false;
    }

    public static bool GetIDMainFromCollider(Collider collider, out IDMain main)
    {
        IDBase base2;
        if (GetIDBaseFromCollider(collider, out base2))
        {
            main = base2.idMain;
            return (bool) main;
        }
        main = null;
        return false;
    }

    private static float InvSqrt(float x)
    {
        return (1f / Mathf.Sqrt(x));
    }

    private static float InvSqrt(float x, float y)
    {
        return (1f / Mathf.Sqrt((x * x) + (y * y)));
    }

    private static float InvSqrt(float x, float y, float z)
    {
        return (1f / Mathf.Sqrt(((x * x) + (y * y)) + (z * z)));
    }

    private static float InvSqrt(float x, float y, float z, float w)
    {
        return (1f / Mathf.Sqrt((((x * x) + (y * y)) + (z * z)) + (w * w)));
    }

    [DebuggerHidden]
    private static IEnumerable<Transform> IterateChildren(Transform parent, int iChild)
    {
        return new <IterateChildren>c__Iterator24 { parent = parent, iChild = iChild, <$>parent = parent, <$>iChild = iChild, $PC = -2 };
    }

    public static List<Transform> ListDecendantsByDepth(this Transform root)
    {
        return ((root.childCount != 0) ? new List<Transform>(IterateChildren(root, 0)) : new List<Transform>(0));
    }

    public static Quaternion LookRotationForcedUp(Quaternion rotation, Vector3 up)
    {
        Vector3 vector;
        Vector3 vector2;
        Vector3 vector3;
        Vector3 vector4;
        Vector3 vector5;
        float x = ((up.x * up.x) + (up.y * up.y)) + (up.z * up.z);
        if (x < float.Epsilon)
        {
            return rotation;
        }
        float num2 = InvSqrt(x);
        up.x *= num2;
        up.y *= num2;
        up.z *= num2;
        vector4.x = up.x;
        vector4.y = up.y;
        vector4.z = up.z;
        vector.z = vector5.x = 1f;
        vector.y = vector.x = vector5.z = vector5.y = 0f;
        vector = (Vector3) (rotation * vector);
        vector5 = (Vector3) (rotation * vector5);
        float num3 = ((vector.x * vector4.x) + (vector.y * vector4.y)) + (vector.z * vector4.z);
        float num4 = ((vector5.x * vector4.x) + (vector5.y * vector4.y)) + (vector5.z * vector4.z);
        if ((num3 * num3) > (num4 * num4))
        {
            vector2.x = vector4.x;
            vector2.y = vector4.y;
            vector2.z = vector4.z;
            vector3.x = vector5.x;
            vector3.y = vector5.y;
            vector3.z = vector5.z;
            vector.x = -((vector2.y * vector3.z) - (vector2.z * vector3.y));
            vector.y = -((vector2.z * vector3.x) - (vector2.x * vector3.z));
            vector.z = -((vector2.x * vector3.y) - (vector2.y * vector3.x));
            float num5 = InvSqrt(vector.x, vector.y, vector.z);
            vector2.x = num5 * vector.x;
            vector2.y = num5 * vector.y;
            vector2.z = num5 * vector.z;
        }
        else
        {
            vector2.x = vector.x;
            vector2.y = vector.y;
            vector2.z = vector.z;
        }
        vector3.x = vector4.x;
        vector3.y = vector4.y;
        vector3.z = vector4.z;
        vector5.x = (vector2.y * vector3.z) - (vector2.z * vector3.y);
        vector5.y = (vector2.z * vector3.x) - (vector2.x * vector3.z);
        vector5.z = (vector2.x * vector3.y) - (vector2.y * vector3.x);
        float num6 = InvSqrt(vector5.x, vector5.y, vector5.z);
        vector3.x = vector5.x * num6;
        vector3.y = vector5.y * num6;
        vector3.z = vector5.z * num6;
        vector2.x = vector4.x;
        vector2.y = vector4.y;
        vector2.z = vector4.z;
        vector.x = (vector2.y * vector3.z) - (vector2.z * vector3.y);
        vector.y = (vector2.z * vector3.x) - (vector2.x * vector3.z);
        vector.z = (vector2.x * vector3.y) - (vector2.y * vector3.x);
        if ((((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z)) < float.Epsilon)
        {
            return rotation;
        }
        return Quaternion.LookRotation(vector, up);
    }

    public static Quaternion LookRotationForcedUp(Vector3 forward, Vector3 up)
    {
        if (forward == up)
        {
            return Quaternion.LookRotation(up);
        }
        Vector3 rhs = Vector3.Cross(forward, up);
        forward = Vector3.Cross(up, rhs);
        if (forward == Vector3.zero)
        {
            forward = Vector3.forward;
        }
        return Quaternion.LookRotation(forward, up);
    }

    public static void SetLocalPositionX(this Transform transform, float x)
    {
        Vector3 localPosition = transform.localPosition;
        localPosition.x = x;
        transform.localPosition = localPosition;
    }

    public static void SetLocalPositionY(this Transform transform, float y)
    {
        Vector3 localPosition = transform.localPosition;
        localPosition.y = y;
        transform.localPosition = localPosition;
    }

    public static void SetLocalPositionZ(this Transform transform, float z)
    {
        Vector3 localPosition = transform.localPosition;
        localPosition.z = z;
        transform.localPosition = localPosition;
    }

    public static Vector3 TestBoxCorners(Vector3 origin, Quaternion rotation, Vector3 boxCenter, Vector3 boxSize, int layerMask = 0x400, int iterations = 7)
    {
        Vector3 vector;
        Vector3 vector2;
        Vector3 vector3;
        Vector3 vector4;
        boxSize.x = Mathf.Abs(boxSize.x) * 0.5f;
        boxSize.y = Mathf.Abs(boxSize.y) * 0.5f;
        boxSize.z = Mathf.Abs(boxSize.z) * 0.5f;
        vector.x = vector2.x = boxCenter.x - boxSize.x;
        vector3.x = vector4.x = boxCenter.x + boxSize.x;
        vector2.z = vector4.z = boxCenter.z - boxSize.z;
        vector.z = vector3.z = boxCenter.z + boxSize.z;
        vector.y = vector3.y = vector2.y = vector4.y = boxCenter.y + boxSize.y;
        vector = (Vector3) (rotation * vector);
        vector2 = (Vector3) (rotation * vector2);
        vector3 = (Vector3) (rotation * vector3);
        vector4 = (Vector3) (rotation * vector4);
        float magnitude = vector.magnitude;
        float distance = vector2.magnitude;
        float num3 = vector3.magnitude;
        float num4 = vector4.magnitude;
        float num5 = 1f / magnitude;
        float num6 = 1f / distance;
        float num7 = 1f / num3;
        float num8 = 1f / num4;
        Vector3 vector5 = (Vector3) (vector * num5);
        Vector3 vector6 = (Vector3) (vector2 * num6);
        Vector3 vector7 = (Vector3) (vector3 * num7);
        Vector3 vector8 = (Vector3) (vector4 * num8);
        Vector3 vector9 = Vector3.Lerp(Vector3.Lerp(vector, vector4, 0.5f), Vector3.Lerp(vector3, vector2, 0.5f), 0.5f);
        for (int i = 0; i < iterations; i++)
        {
            RaycastHit hit;
            RaycastHit hit2;
            RaycastHit hit3;
            RaycastHit hit4;
            Vector3 vector10 = origin + vector;
            Vector3 vector11 = origin + vector2;
            Vector3 vector12 = origin + vector3;
            Vector3 vector13 = origin + vector4;
            bool flag = Physics.Raycast(vector10, -vector5, out hit, magnitude, layerMask);
            bool flag2 = Physics.Raycast(vector11, -vector6, out hit2, distance, layerMask);
            bool flag3 = Physics.Raycast(vector12, -vector7, out hit3, num3, layerMask);
            bool flag4 = Physics.Raycast(vector13, -vector8, out hit4, num4, layerMask);
            if ((!flag && !flag2) && (!flag3 && !flag4))
            {
                return origin;
            }
            Vector3 from = !flag ? vector : (hit.point - origin);
            Vector3 to = !flag2 ? vector2 : (hit2.point - origin);
            Vector3 vector16 = !flag3 ? vector3 : (hit3.point - origin);
            Vector3 vector17 = !flag4 ? vector4 : (hit4.point - origin);
            Vector3 vector19 = Vector3.Lerp(Vector3.Lerp(from, vector17, 0.5f), Vector3.Lerp(vector16, to, 0.5f), 0.5f) - vector9;
            vector19.y = 0f;
            origin += (Vector3) (vector19 * 2.15f);
        }
        return origin;
    }

    public static Quaternion UpRotation(Vector3 up)
    {
        Vector3 vector;
        float num = Vector3.Dot(up, Vector3.forward);
        float num2 = Vector3.Dot(up, Vector3.right);
        if ((num * num) < (num2 * num2))
        {
            vector = Vector3.Cross(up, Vector3.forward);
        }
        else
        {
            vector = Vector3.Cross(up, Vector3.right);
        }
        return Quaternion.LookRotation(vector, up);
    }

    [CompilerGenerated]
    private sealed class <IterateChildren>c__Iterator24 : IDisposable, IEnumerator, IEnumerable, IEnumerable<Transform>, IEnumerator<Transform>
    {
        internal Transform $current;
        internal int $PC;
        internal int <$>iChild;
        internal Transform <$>parent;
        internal IEnumerator<Transform> <$s_229>__1;
        internal IEnumerator<Transform> <$s_230>__3;
        internal Transform <child>__0;
        internal Transform <sibling>__2;
        internal Transform <subChild>__4;
        internal int iChild;
        internal Transform parent;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 2:
                    try
                    {
                    }
                    finally
                    {
                        if (this.<$s_229>__1 == null)
                        {
                        }
                        this.<$s_229>__1.Dispose();
                    }
                    break;

                case 3:
                    try
                    {
                    }
                    finally
                    {
                        if (this.<$s_230>__3 == null)
                        {
                        }
                        this.<$s_230>__3.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    break;

                case 1:
                    if (this.<child>__0.childCount <= 0)
                    {
                        if (++this.iChild < this.parent.childCount)
                        {
                            break;
                        }
                        goto Label_01C6;
                    }
                    if ((this.iChild + 1) >= this.parent.childCount)
                    {
                        goto Label_011C;
                    }
                    this.<$s_229>__1 = TransformHelpers.IterateChildren(this.parent, ++this.iChild).GetEnumerator();
                    num = 0xfffffffd;
                    goto Label_00B2;

                case 2:
                    goto Label_00B2;

                case 3:
                    goto Label_0136;

                default:
                    goto Label_01CD;
            }
            this.<child>__0 = this.parent.GetChild(this.iChild);
            this.$current = this.<child>__0;
            this.$PC = 1;
            goto Label_01CF;
        Label_00B2:
            try
            {
                while (this.<$s_229>__1.MoveNext())
                {
                    this.<sibling>__2 = this.<$s_229>__1.Current;
                    this.$current = this.<sibling>__2;
                    this.$PC = 2;
                    flag = true;
                    goto Label_01CF;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_229>__1 == null)
                {
                }
                this.<$s_229>__1.Dispose();
            }
        Label_011C:
            this.<$s_230>__3 = TransformHelpers.IterateChildren(this.<child>__0, 0).GetEnumerator();
            num = 0xfffffffd;
        Label_0136:
            try
            {
                while (this.<$s_230>__3.MoveNext())
                {
                    this.<subChild>__4 = this.<$s_230>__3.Current;
                    this.$current = this.<subChild>__4;
                    this.$PC = 3;
                    flag = true;
                    goto Label_01CF;
                }
            }
            finally
            {
                if (!flag)
                {
                }
                if (this.<$s_230>__3 == null)
                {
                }
                this.<$s_230>__3.Dispose();
            }
        Label_01C6:
            this.$PC = -1;
        Label_01CD:
            return false;
        Label_01CF:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Transform> IEnumerable<Transform>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new TransformHelpers.<IterateChildren>c__Iterator24 { parent = this.<$>parent, iChild = this.<$>iChild };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<UnityEngine.Transform>.GetEnumerator();
        }

        Transform IEnumerator<Transform>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

