using Facepunch.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class SlowSpawn : ThrottledTask, IProgress
{
    [SerializeField]
    private string findSequence = "_decor_";
    [NonSerialized]
    private int iter = -1;
    [NonSerialized]
    private int iter_end;
    [SerializeField]
    private Mesh[] meshes;
    [HideInInspector, SerializeField]
    private int[] meshIndex;
    [HideInInspector, SerializeField]
    private Vector4[] ps;
    [SerializeField, HideInInspector]
    private Quaternion[] r;
    [SerializeField]
    private SpawnFlags runtimeLoad = SpawnFlags.Collider;
    [SerializeField]
    private Material sharedMaterial;

    [DebuggerHidden]
    public IEnumerable<GameObject> SpawnAll(SpawnFlags SpawnFlags = 7, HideFlags HideFlags = 9)
    {
        return new <SpawnAll>c__Iterator3C { SpawnFlags = SpawnFlags, HideFlags = HideFlags, <$>SpawnFlags = SpawnFlags, <$>HideFlags = HideFlags, <>f__this = this, $PC = -2 };
    }

    [DebuggerHidden]
    private IEnumerator Start()
    {
        return new <Start>c__Iterator3B { <>f__this = this };
    }

    public int Count
    {
        get
        {
            return ((this.ps != null) ? this.ps.Length : 0);
        }
    }

    public int CountSpawned
    {
        get
        {
            return (!base.Working ? ((this.iter != -1) ? this.Count : 0) : this.iter);
        }
    }

    public InstanceParameters this[int i]
    {
        get
        {
            return new InstanceParameters(this, i);
        }
    }

    public float progress
    {
        get
        {
            return (!base.Working ? (((this.iter != -1) || !base.enabled) ? 1f : 0f) : ((float) (((double) this.iter) / ((double) this.Count))));
        }
    }

    [CompilerGenerated]
    private sealed class <SpawnAll>c__Iterator3C : IDisposable, IEnumerator, IEnumerable, IEnumerable<GameObject>, IEnumerator<GameObject>
    {
        internal GameObject $current;
        internal int $PC;
        internal UnityEngine.HideFlags <$>HideFlags;
        internal SlowSpawn.SpawnFlags <$>SpawnFlags;
        internal SlowSpawn <>f__this;
        internal Exception <e>__2;
        internal int <i>__0;
        internal GameObject <newSpawn>__1;
        internal UnityEngine.HideFlags HideFlags;
        internal SlowSpawn.SpawnFlags SpawnFlags;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<i>__0 = 0;
                    goto Label_00A0;

                case 1:
                    break;

                default:
                    goto Label_00BD;
            }
        Label_0092:
            this.<i>__0++;
        Label_00A0:
            if (this.<i>__0 < this.<>f__this.Count)
            {
                try
                {
                    this.<newSpawn>__1 = this.<>f__this[this.<i>__0].Spawn(this.SpawnFlags, this.HideFlags);
                }
                catch (Exception exception)
                {
                    this.<e>__2 = exception;
                    Debug.LogException(this.<e>__2);
                    goto Label_0092;
                }
                this.$current = this.<newSpawn>__1;
                this.$PC = 1;
                return true;
            }
            this.$PC = -1;
        Label_00BD:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new SlowSpawn.<SpawnAll>c__Iterator3C { <>f__this = this.<>f__this, SpawnFlags = this.<$>SpawnFlags, HideFlags = this.<$>HideFlags };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<UnityEngine.GameObject>.GetEnumerator();
        }

        GameObject IEnumerator<GameObject>.Current
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

    [CompilerGenerated]
    private sealed class <Start>c__Iterator3B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal SlowSpawn <>f__this;
        internal Exception <e>__1;
        internal ThrottledTask.Timer <timer>__0;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if (this.<>f__this.Working || (++this.<>f__this.iter >= (this.<>f__this.iter_end = this.<>f__this.Count)))
                    {
                        this.$PC = -1;
                        goto Label_013C;
                    }
                    this.<>f__this.Working = true;
                    break;

                case 1:
                    break;

                default:
                    goto Label_013C;
            }
            this.<timer>__0 = this.<>f__this.Begin;
            do
            {
                try
                {
                    this.<>f__this[this.<>f__this.iter].Spawn(this.<>f__this.runtimeLoad, HideFlags.NotEditable | HideFlags.HideInHierarchy);
                }
                catch (Exception exception)
                {
                    this.<e>__1 = exception;
                    Debug.LogException(this.<e>__1, this.<>f__this);
                }
                if (++this.<>f__this.iter >= this.<>f__this.iter_end)
                {
                    this.<>f__this.Working = false;
                    goto Label_013C;
                }
            }
            while (this.<timer>__0.Continue);
            this.$current = null;
            this.$PC = 1;
            return true;
        Label_013C:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
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

    [StructLayout(LayoutKind.Sequential)]
    public struct InstanceParameters
    {
        public const SlowSpawn.SpawnFlags DefaultSpawnFlags = SlowSpawn.SpawnFlags.All;
        public const HideFlags DefaultHideFlags = (HideFlags.NotEditable | HideFlags.HideInHierarchy);
        public readonly Vector3 Position;
        public readonly Vector3 Scale;
        public readonly Quaternion Rotation;
        public readonly UnityEngine.Mesh Mesh;
        public readonly Material SharedMaterial;
        public readonly int Layer;
        public readonly int Index;
        public InstanceParameters(SlowSpawn SlowSpawn, int Index)
        {
            this.Index = Index;
            this.Layer = SlowSpawn.gameObject.layer;
            Vector4 vector = SlowSpawn.ps[Index];
            this.Position.x = vector.x;
            this.Position.y = vector.y;
            this.Position.z = vector.z;
            this.Scale.x = this.Scale.y = this.Scale.z = vector.w;
            this.Rotation = SlowSpawn.r[Index];
            this.Mesh = SlowSpawn.meshes[SlowSpawn.meshIndex[Index]];
            this.SharedMaterial = SlowSpawn.sharedMaterial;
        }

        public MeshCollider AddCollider(GameObject go)
        {
            MeshCollider collider = go.AddComponent<MeshCollider>();
            collider.sharedMesh = this.Mesh;
            return collider;
        }

        public MeshRenderer AddRenderer(GameObject go)
        {
            MeshRenderer renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = this.SharedMaterial;
            return renderer;
        }

        public MeshFilter AddMeshFilter(GameObject go)
        {
            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = this.Mesh;
            return filter;
        }

        private SlowSpawn.SpawnFlags AddTo(GameObject go, SlowSpawn.SpawnFlags spawnFlags, bool safe)
        {
            SlowSpawn.SpawnFlags flags = 0;
            if (((spawnFlags & SlowSpawn.SpawnFlags.MeshFilter) == SlowSpawn.SpawnFlags.MeshFilter) && (safe || (go.GetComponent<MeshFilter>() == null)))
            {
                flags |= SlowSpawn.SpawnFlags.MeshFilter;
                this.AddMeshFilter(go);
            }
            if (((spawnFlags & SlowSpawn.SpawnFlags.Renderer) == SlowSpawn.SpawnFlags.Renderer) && (safe || (go.renderer == null)))
            {
                flags |= SlowSpawn.SpawnFlags.Renderer;
                this.AddRenderer(go);
            }
            if (((spawnFlags & SlowSpawn.SpawnFlags.Collider) == SlowSpawn.SpawnFlags.Collider) && (safe || (go.collider == null)))
            {
                flags |= SlowSpawn.SpawnFlags.Collider;
                this.AddCollider(go);
            }
            return flags;
        }

        public SlowSpawn.SpawnFlags AddTo(GameObject go, SlowSpawn.SpawnFlags spawnFlags = 7)
        {
            return this.AddTo(go, spawnFlags, false);
        }

        public GameObject Spawn(SlowSpawn.SpawnFlags spawnFlags = 7, HideFlags HideFlags = 9)
        {
            GameObject obj3 = new GameObject(string.Empty) {
                hideFlags = HideFlags,
                layer = this.Layer
            };
            obj3.transform.position = this.Position;
            obj3.transform.rotation = this.Rotation;
            GameObject go = obj3;
            this.AddTo(go, spawnFlags, true);
            return go;
        }
    }

    [Flags]
    public enum SpawnFlags
    {
        All = 7,
        Collider = 1,
        Graphics = 6,
        MeshFilter = 4,
        Renderer = 2
    }
}

