using Facepunch.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class TreeUnpack : ThrottledTask, IProgress
{
    private TreeUnpackGroup currentGroup;
    private Mesh currentMesh;
    [NonSerialized]
    private int currentTreeIndex;
    private IEnumerator<TreeUnpackGroup> groupEnumerator;
    private IEnumerator<Mesh> meshEnumerator;
    [NonSerialized]
    private int totalTrees;
    [SerializeField]
    private TreeUnpackGroup[] unpackGroups;

    private void Awake()
    {
        base.Awake();
        base.StartCoroutine("DoUnpack");
    }

    [DebuggerHidden]
    private IEnumerator DoUnpack()
    {
        return new <DoUnpack>c__IteratorD { <>f__this = this };
    }

    private bool MoveNext()
    {
        if (this.meshEnumerator != null)
        {
            while (this.meshEnumerator.MoveNext())
            {
                this.currentTreeIndex++;
                this.currentMesh = this.meshEnumerator.Current;
                if (this.currentMesh != null)
                {
                    return true;
                }
            }
        }
        if (this.groupEnumerator.MoveNext())
        {
            this.currentGroup = this.groupEnumerator.Current;
            this.meshEnumerator = this.currentGroup.meshes.GetEnumerator();
            return this.MoveNext();
        }
        return false;
    }

    public float progress
    {
        get
        {
            return ((this.totalTrees <= 0) ? 1f : (((float) this.currentTreeIndex) / ((float) this.totalTrees)));
        }
    }

    [CompilerGenerated]
    private sealed class <DoUnpack>c__IteratorD : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal TreeUnpackGroup[] <$s_119>__0;
        internal int <$s_120>__1;
        internal TreeUnpack <>f__this;
        internal GameObject <col>__4;
        internal TreeUnpackGroup <grp>__2;
        internal MeshCollider <mc>__5;
        internal ThrottledTask.Timer <timer>__3;

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
                    this.<>f__this.totalTrees = 0;
                    this.<>f__this.currentTreeIndex = 0;
                    this.<$s_119>__0 = this.<>f__this.unpackGroups;
                    this.<$s_120>__1 = 0;
                    while (this.<$s_120>__1 < this.<$s_119>__0.Length)
                    {
                        this.<grp>__2 = this.<$s_119>__0[this.<$s_120>__1];
                        this.<>f__this.totalTrees += this.<grp>__2.meshes.Length;
                        this.<$s_120>__1++;
                    }
                    this.<>f__this.Working = true;
                    this.<>f__this.groupEnumerator = this.<>f__this.unpackGroups.GetEnumerator();
                    this.<timer>__3 = this.<>f__this.Begin;
                    while (this.<>f__this.MoveNext())
                    {
                        Type[] components = new Type[] { typeof(MeshCollider) };
                        GameObject obj2 = new GameObject(string.Empty, components) {
                            hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy,
                            tag = this.<>f__this.currentGroup.tag,
                            layer = (this.<>f__this.currentGroup.layer != 0) ? this.<>f__this.currentGroup.layer : 10
                        };
                        this.<col>__4 = obj2;
                        this.<mc>__5 = (MeshCollider) this.<col>__4.collider;
                        this.<mc>__5.smoothSphereCollisions = this.<>f__this.currentGroup.spherical;
                        this.<mc>__5.sharedMesh = this.<>f__this.currentMesh;
                        if (this.<timer>__3.Continue)
                        {
                            continue;
                        }
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;
                    Label_01C9:
                        this.<timer>__3 = this.<>f__this.Begin;
                    }
                    this.<>f__this.Working = false;
                    Object.Destroy(this.<>f__this);
                    this.$PC = -1;
                    break;

                case 1:
                    goto Label_01C9;
            }
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
}

