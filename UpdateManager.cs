using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/Internal/Update Manager")]
public class UpdateManager : MonoBehaviour
{
    private List<DestroyEntry> mDest = new List<DestroyEntry>();
    private static UpdateManager mInst;
    private List<UpdateEntry> mOnCoro = new List<UpdateEntry>();
    private List<UpdateEntry> mOnLate = new List<UpdateEntry>();
    private List<UpdateEntry> mOnUpdate = new List<UpdateEntry>();
    private float mTime;

    private void Add(MonoBehaviour mb, int updateOrder, OnUpdate func, List<UpdateEntry> list)
    {
        int num = 0;
        int count = list.Count;
        while (num < count)
        {
            UpdateEntry entry = list[num];
            if (entry.func == func)
            {
                return;
            }
            num++;
        }
        UpdateEntry item = new UpdateEntry {
            index = updateOrder,
            func = func,
            mb = mb,
            isMonoBehaviour = mb != null
        };
        list.Add(item);
        if (updateOrder != 0)
        {
            list.Sort(new Comparison<UpdateEntry>(UpdateManager.Compare));
        }
    }

    public static void AddCoroutine(MonoBehaviour mb, int updateOrder, OnUpdate func)
    {
        CreateInstance();
        mInst.Add(mb, updateOrder, func, mInst.mOnCoro);
    }

    public static void AddDestroy(Object obj, float delay)
    {
        if (obj != null)
        {
            if (Application.isPlaying)
            {
                if (delay > 0f)
                {
                    CreateInstance();
                    DestroyEntry item = new DestroyEntry {
                        obj = obj,
                        time = Time.realtimeSinceStartup + delay
                    };
                    mInst.mDest.Add(item);
                }
                else
                {
                    Object.Destroy(obj);
                }
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    public static void AddLateUpdate(MonoBehaviour mb, int updateOrder, OnUpdate func)
    {
        CreateInstance();
        mInst.Add(mb, updateOrder, func, mInst.mOnLate);
    }

    public static void AddUpdate(MonoBehaviour mb, int updateOrder, OnUpdate func)
    {
        CreateInstance();
        mInst.Add(mb, updateOrder, func, mInst.mOnUpdate);
    }

    private static int Compare(UpdateEntry a, UpdateEntry b)
    {
        if (a.index < b.index)
        {
            return 1;
        }
        if (a.index > b.index)
        {
            return 1;
        }
        return 0;
    }

    [DebuggerHidden]
    private IEnumerator CoroutineFunction()
    {
        return new <CoroutineFunction>c__Iterator4A { <>f__this = this };
    }

    private bool CoroutineUpdate()
    {
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        float delta = realtimeSinceStartup - this.mTime;
        if (delta >= 0.001f)
        {
            this.mTime = realtimeSinceStartup;
            this.UpdateList(this.mOnCoro, delta);
            bool isPlaying = Application.isPlaying;
            int count = this.mDest.Count;
            while (count > 0)
            {
                DestroyEntry entry = this.mDest[--count];
                if (!isPlaying || (entry.time < this.mTime))
                {
                    if (entry.obj != null)
                    {
                        NGUITools.Destroy(entry.obj);
                        entry.obj = null;
                    }
                    this.mDest.RemoveAt(count);
                }
            }
            if (((this.mOnUpdate.Count == 0) && (this.mOnLate.Count == 0)) && ((this.mOnCoro.Count == 0) && (this.mDest.Count == 0)))
            {
                NGUITools.Destroy(base.gameObject);
                return false;
            }
        }
        return true;
    }

    private static void CreateInstance()
    {
        if (mInst == null)
        {
            mInst = Object.FindObjectOfType(typeof(UpdateManager)) as UpdateManager;
            if ((mInst == null) && Application.isPlaying)
            {
                GameObject target = new GameObject("_UpdateManager");
                Object.DontDestroyOnLoad(target);
                mInst = target.AddComponent<UpdateManager>();
            }
        }
    }

    private void LateUpdate()
    {
        this.UpdateList(this.mOnLate, Time.deltaTime);
        if (!Application.isPlaying)
        {
            this.CoroutineUpdate();
        }
    }

    private void OnApplicationQuit()
    {
        Object.DestroyImmediate(base.gameObject);
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            this.mTime = Time.realtimeSinceStartup;
            base.StartCoroutine(this.CoroutineFunction());
        }
    }

    private void Update()
    {
        if (mInst != this)
        {
            NGUITools.Destroy(base.gameObject);
        }
        else
        {
            this.UpdateList(this.mOnUpdate, Time.deltaTime);
        }
    }

    private void UpdateList(List<UpdateEntry> list, float delta)
    {
        int count = list.Count;
        while (count > 0)
        {
            UpdateEntry entry = list[--count];
            if (entry.isMonoBehaviour)
            {
                if (entry.mb == null)
                {
                    list.RemoveAt(count);
                    continue;
                }
                if (!entry.mb.enabled || !entry.mb.gameObject.activeInHierarchy)
                {
                    continue;
                }
            }
            entry.func(delta);
        }
    }

    [CompilerGenerated]
    private sealed class <CoroutineFunction>c__Iterator4A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal UpdateManager <>f__this;

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
                case 1:
                    if (Application.isPlaying)
                    {
                        if (!this.<>f__this.CoroutineUpdate())
                        {
                            break;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;
                    }
                    break;

                default:
                    goto Label_0064;
            }
            this.$PC = -1;
        Label_0064:
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

    public class DestroyEntry
    {
        public Object obj;
        public float time;
    }

    public delegate void OnUpdate(float delta);

    public class UpdateEntry
    {
        public UpdateManager.OnUpdate func;
        public int index;
        public bool isMonoBehaviour;
        public MonoBehaviour mb;
    }
}

