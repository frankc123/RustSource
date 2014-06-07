using Facepunch.MeshBatch;
using Facepunch.MeshBatch.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ExplosionHelper
{
    private const float kMaxZero = 1E-05f;

    public static Surface[] OverlapExplosion(Vector3 point, float explosionRadius, int findLayerMask = -1, int occludingLayerMask = -1, IDMain ignore = null)
    {
        Point point2 = new Point(point, explosionRadius, findLayerMask, occludingLayerMask, ignore);
        return point2.ToArray();
    }

    public static Surface[] OverlapExplosionSorted(Vector3 point, float explosionRadius, int findLayerMask = -1, int occludingLayerMask = -1, IDMain ignore = null)
    {
        Surface[] array = OverlapExplosion(point, explosionRadius, findLayerMask, occludingLayerMask, ignore);
        if (array.Length > 1)
        {
            Array.Sort<Surface>(array);
        }
        return array;
    }

    public static Surface[] OverlapExplosionUnique(Vector3 point, float explosionRadius, int findLayerMask = -1, int occludingLayerMask = -1, IDMain ignore = null)
    {
        Surface[] array = OverlapExplosion(point, explosionRadius, findLayerMask, occludingLayerMask, ignore);
        int length = array.Length;
        if (length > 1)
        {
            Array.Sort<Surface>(array);
            if (Unique.Filter(array, ref length))
            {
                Array.Resize<Surface>(ref array, length);
            }
        }
        return array;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Point : IEnumerable, IEnumerable<ExplosionHelper.Surface>
    {
        public readonly Vector3 point;
        public readonly float blastRadius;
        public readonly int overlapLayerMask;
        public readonly int raycastLayerMask;
        public readonly IDMain skip;
        public Point(Vector3 point, float blastRadius, int overlapLayerMask, int raycastLayerMask, IDMain skip)
        {
            this.point = point;
            this.blastRadius = blastRadius;
            this.overlapLayerMask = overlapLayerMask;
            this.raycastLayerMask = raycastLayerMask;
            this.skip = skip;
        }

        IEnumerator<ExplosionHelper.Surface> IEnumerable<ExplosionHelper.Surface>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private bool BoundsWork(ref Bounds bounds, ref ExplosionHelper.Work w)
        {
            w.boundsSquareDistance = bounds.SqrDistance(this.point);
            if (w.boundsSquareDistance > (this.blastRadius * this.blastRadius))
            {
                return false;
            }
            if (w.boundsSquareDistance <= 1E-05f)
            {
                w.boundsSquareDistance = 0f;
            }
            w.center = bounds.center;
            w.rayDir.x = w.center.x - this.point.x;
            w.rayDir.y = w.center.y - this.point.y;
            w.rayDir.z = w.center.z - this.point.z;
            w.squareDistanceToCenter = ((w.rayDir.x * w.rayDir.x) + (w.rayDir.y * w.rayDir.y)) + (w.rayDir.z * w.rayDir.z);
            if (w.squareDistanceToCenter > (this.blastRadius * this.blastRadius))
            {
                return false;
            }
            if (w.squareDistanceToCenter <= 9.999999E-11f)
            {
                w.distanceToCenter = w.squareDistanceToCenter = 0f;
                w.rayDistance = w.squareRayDistance = 0f;
                w.rayTest = false;
                w.boundsExtent = bounds.size;
                w.boundsExtent.x *= 0.5f;
                w.boundsExtent.y *= 0.5f;
                w.boundsExtent.z *= 0.5f;
                w.boundsExtentSquareMagnitude = ((w.boundsExtent.x * w.boundsExtent.x) + (w.boundsExtent.y * w.boundsExtent.y)) + (w.boundsExtent.z * w.boundsExtent.z);
                return true;
            }
            w.distanceToCenter = Mathf.Sqrt(w.squareDistanceToCenter);
            w.boundsExtent = bounds.size;
            w.boundsExtent.x *= 0.5f;
            w.boundsExtent.y *= 0.5f;
            w.boundsExtent.z *= 0.5f;
            w.boundsExtentSquareMagnitude = ((w.boundsExtent.x * w.boundsExtent.x) + (w.boundsExtent.y * w.boundsExtent.y)) + (w.boundsExtent.z * w.boundsExtent.z);
            w.squareRayDistance = w.boundsSquareDistance + w.boundsExtentSquareMagnitude;
            if (w.squareRayDistance > w.squareDistanceToCenter)
            {
                w.squareRayDistance = w.squareDistanceToCenter;
                w.rayDistance = w.distanceToCenter;
            }
            else
            {
                if (w.squareRayDistance <= 9.999999E-11f)
                {
                    w.rayDistance = w.squareRayDistance = 0f;
                    w.rayTest = false;
                    return true;
                }
                w.rayDistance = Mathf.Sqrt(w.squareRayDistance);
            }
            w.rayTest = true;
            return true;
        }

        private bool SurfaceForMeshBatchInstance(MeshBatchInstance instance, ref ExplosionHelper.Surface surface)
        {
            surface.idBase = instance;
            surface.idMain = surface.idBase.idMain;
            if ((surface.idMain == null) || (surface.idMain == this.skip))
            {
                surface = new ExplosionHelper.Surface();
                return false;
            }
            surface.bounds = instance.physicalBounds;
            if (this.BoundsWork(ref surface.bounds, ref surface.work))
            {
                if (surface.work.rayTest)
                {
                    bool flag;
                    MeshBatchInstance instance2;
                    if ((this.raycastLayerMask != 0) && MeshBatchPhysics.Raycast(this.point, surface.work.rayDir, surface.work.rayDistance, this.raycastLayerMask, out flag, out instance2))
                    {
                        if (flag && (instance2 == instance))
                        {
                            surface.blocked = false;
                        }
                        else
                        {
                            surface.blocked = true;
                        }
                    }
                    else
                    {
                        surface.blocked = false;
                    }
                }
                else
                {
                    surface.blocked = false;
                }
                return true;
            }
            surface = new ExplosionHelper.Surface();
            return false;
        }

        private bool SurfaceForCollider(Collider collider, ref ExplosionHelper.Surface surface)
        {
            if (!collider.enabled)
            {
                surface = new ExplosionHelper.Surface();
                return false;
            }
            surface.idBase = IDBase.Get(collider);
            if (surface.idBase == null)
            {
                surface = new ExplosionHelper.Surface();
                return false;
            }
            surface.idMain = surface.idBase.idMain;
            if ((surface.idMain == null) || (surface.idMain == this.skip))
            {
                surface = new ExplosionHelper.Surface();
                return false;
            }
            surface.bounds = collider.bounds;
            if (!this.BoundsWork(ref surface.bounds, ref surface.work))
            {
                return false;
            }
            if (this.raycastLayerMask != 0)
            {
                RaycastHit hit;
                surface.blocked = ((surface.work.rayTest && collider.Raycast(new Ray(this.point, surface.work.rayDir), out hit, surface.work.rayDistance)) && Physics.Raycast(this.point, surface.work.rayDir, out hit, hit.distance, this.raycastLayerMask)) && (hit.collider != collider);
            }
            return true;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(ref this, false);
        }

        public ExplosionHelper.Surface[] ToArray()
        {
            Enumerator enumerator = new Enumerator(ref this, true);
            return EnumeratorToArray.Build(ref enumerator);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IDisposable, IEnumerator, IEnumerator<ExplosionHelper.Surface>
        {
            private readonly ExplosionHelper.Point IN;
            private int colliderIndex;
            private bool inInstanceEnumerator;
            private MeshBatchPhysicalOutput output;
            private IEnumerator<MeshBatchInstance> overlapEnumerator;
            private Collider[] overlap;
            public ExplosionHelper.Surface current;
            private readonly bool _immediate;
            internal Enumerator(ref ExplosionHelper.Point point, bool immediate)
            {
                this._immediate = immediate;
                this.IN = point;
                this.colliderIndex = -1;
                this.inInstanceEnumerator = false;
                this.overlapEnumerator = null;
                this.output = null;
                this.overlap = null;
                this.current = new ExplosionHelper.Surface();
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.current;
                }
            }
            public ExplosionHelper.Surface Current
            {
                get
                {
                    return this.current;
                }
            }
            public bool MoveNext()
            {
            Label_007D:
                while (this.inInstanceEnumerator)
                {
                    if ((this._immediate || (this.output != null)) && this.overlapEnumerator.MoveNext())
                    {
                        MeshBatchInstance current = this.overlapEnumerator.Current;
                        if (this.IN.SurfaceForMeshBatchInstance(current, ref this.current))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        this.overlapEnumerator.Dispose();
                        this.overlapEnumerator = null;
                        this.inInstanceEnumerator = false;
                        this.output = null;
                    }
                }
                if (this.colliderIndex++ == -1)
                {
                    this.overlap = Physics.OverlapSphere(this.IN.point, this.IN.blastRadius, this.IN.overlapLayerMask);
                }
                while (this.colliderIndex < this.overlap.Length)
                {
                    if (this._immediate || (this.overlap[this.colliderIndex] != null))
                    {
                        if (this.overlap[this.colliderIndex].GetMeshBatchPhysicalOutput<MeshBatchPhysicalOutput>(out this.output))
                        {
                            this.inInstanceEnumerator = true;
                            this.overlapEnumerator = this.output.EnumerateOverlapSphereInstances(this.IN.point, this.IN.blastRadius).GetEnumerator();
                            goto Label_007D;
                        }
                        if (this.IN.SurfaceForCollider(this.overlap[this.colliderIndex], ref this.current))
                        {
                            return true;
                        }
                    }
                    this.colliderIndex++;
                }
                this.colliderIndex = this.overlap.Length;
                this.current = new ExplosionHelper.Surface();
                return false;
            }

            public void Dispose()
            {
                this.colliderIndex = -1;
                if (this.inInstanceEnumerator)
                {
                    this.inInstanceEnumerator = false;
                    this.overlapEnumerator.Dispose();
                }
                this.overlapEnumerator = null;
                this.output = null;
                this.overlap = null;
                this.current = new ExplosionHelper.Surface();
            }

            public void Reset()
            {
                this.Dispose();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EnumeratorToArray
        {
            private ExplosionHelper.Point.Enumerator enumerator;
            private ExplosionHelper.Surface[] array;
            private int length;
            private void RecurseInStackHeapToArray()
            {
                if (this.enumerator.MoveNext())
                {
                    ExplosionHelper.Surface current = this.enumerator.current;
                    this.length++;
                    this.RecurseInStackHeapToArray();
                    this.array[--this.length] = current;
                }
                else
                {
                    this.array = new ExplosionHelper.Surface[this.length];
                }
            }

            public static ExplosionHelper.Surface[] Build(ref ExplosionHelper.Point.Enumerator point_enumerator)
            {
                ExplosionHelper.Point.EnumeratorToArray array;
                array.enumerator = point_enumerator;
                array.length = 0;
                array.array = null;
                array.RecurseInStackHeapToArray();
                return array.array;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Surface : IEquatable<ExplosionHelper.Surface>, IComparable<ExplosionHelper.Surface>
    {
        public IDBase idBase;
        public IDMain idMain;
        public Bounds bounds;
        public ExplosionHelper.Work work;
        public bool blocked;
        public override bool Equals(object obj)
        {
            return ((obj is ExplosionHelper.Surface) && this.Equals((ExplosionHelper.Surface) obj));
        }

        public bool Equals(ExplosionHelper.Surface surface)
        {
            return ((((this.blocked == surface.blocked) && (this.bounds == surface.bounds)) && ((this.idBase == surface.idBase) && (this.idMain == surface.idMain))) && this.work.Equals(ref surface.work));
        }

        public bool Equals(ref ExplosionHelper.Surface surface)
        {
            return ((((this.blocked == surface.blocked) && (this.bounds == surface.bounds)) && ((this.idBase == surface.idBase) && (this.idMain == surface.idMain))) && this.work.Equals(ref surface.work));
        }

        public override string ToString()
        {
            return "Surface";
        }

        public override int GetHashCode()
        {
            return (this.bounds.GetHashCode() ^ ((this.idBase == null) ? 0 : this.idBase.GetHashCode()));
        }

        public int CompareTo(ExplosionHelper.Surface other)
        {
            int num = this.blocked.CompareTo(other.blocked);
            if (num == 0)
            {
                num = this.work.distanceToCenter.CompareTo(other.work.distanceToCenter);
                if (num == 0)
                {
                    num = this.work.boundsSquareDistance.CompareTo(other.work.squareDistanceToCenter);
                    if (num == 0)
                    {
                        num = this.work.rayDistance.CompareTo(other.work.rayDistance);
                    }
                }
            }
            return num;
        }
    }

    private static class Unique
    {
        private static readonly HashSet<IDMain> Set = new HashSet<IDMain>();

        public static bool Filter(ExplosionHelper.Surface[] array, ref int length)
        {
            int num = array.Length;
            try
            {
                for (int i = 0; i < num; i++)
                {
                    IDMain idMain = array[i].idMain;
                    if ((idMain != null) && !Set.Add(idMain))
                    {
                        int num3 = i;
                        while (++i < num)
                        {
                            idMain = array[i].idMain;
                            if ((array[i].idMain == null) || Set.Add(idMain))
                            {
                                array[num3++] = array[i];
                            }
                        }
                        length = num3;
                        return true;
                    }
                }
            }
            finally
            {
                Set.Clear();
            }
            return false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Work
    {
        public Vector3 center;
        public Vector3 rayDir;
        public Vector3 boundsExtent;
        public float boundsExtentSquareMagnitude;
        public float boundsSquareDistance;
        public float distanceToCenter;
        public float squareDistanceToCenter;
        public float rayDistance;
        public float squareRayDistance;
        public bool rayTest;
        public bool Equals(ref ExplosionHelper.Work w)
        {
            return ((((((this.squareDistanceToCenter == w.squareDistanceToCenter) && (this.boundsSquareDistance == w.boundsSquareDistance)) && ((this.boundsExtentSquareMagnitude == w.boundsExtentSquareMagnitude) && (this.distanceToCenter == w.distanceToCenter))) && (!this.rayTest ? !w.rayTest : (((w.rayTest && (this.squareRayDistance == w.squareRayDistance)) && (this.rayDistance == w.rayDistance)) && (this.rayDir == w.rayDir)))) && (this.center == w.center)) && (this.boundsExtent == w.boundsExtent));
        }
    }
}

