using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class CameraMount : MonoBehaviour
{
    [CompilerGenerated]
    private static Comparison<CameraMount> <>f__am$cache11;
    [SerializeField]
    private bool autoBind;
    [NonSerialized]
    private bool awoke;
    [NonSerialized]
    private bool bound;
    [PrefetchComponent]
    public Camera camera;
    [PrefetchComponent]
    public CameraFX cameraFX;
    public SharedCameraMode cameraMode;
    private static bool createdTemporaryCameraMount;
    private static bool creatingTemporaryCameraMount;
    [NonSerialized]
    private bool destroyed;
    [SerializeField]
    private int importance;
    public KindOfCamera kindOfCamera;
    private const string kTempMountGOName = "__-temp mount-__";
    private static CameraMount temporaryCameraMount;
    private static GameObject temporaryCameraMountGameObject;
    private static bool temporaryCameraMountHasParent;
    private static Transform temporaryCameraMountParent;
    private static CameraMount temporaryCameraMountSource;
    private static CameraMount top;

    private void Awake()
    {
        this.awoke = true;
        if (this.camera == null)
        {
            this.camera = base.camera;
        }
        this.camera.enabled = false;
        if (creatingTemporaryCameraMount)
        {
            temporaryCameraMount = this;
            if (temporaryCameraMountHasParent)
            {
                Transform transform = base.transform;
                transform.parent = temporaryCameraMountParent;
                Transform transform2 = temporaryCameraMountSource.transform;
                transform.localPosition = transform2.localPosition;
                transform.localRotation = transform2.localRotation;
                transform.localScale = transform2.localScale;
            }
            this.camera.CopyFrom(temporaryCameraMountSource.camera);
            this.cameraFX = temporaryCameraMountSource.cameraFX;
            if (temporaryCameraMountSource.open)
            {
                this.Bind();
            }
        }
        else if (this.autoBind)
        {
            this.Bind();
        }
    }

    private void Bind()
    {
        if (top == null)
        {
            top = this;
            SetMountActive();
        }
        else if (top.importance < this.importance)
        {
            SetMountInactive();
            queue.Push(top);
            top = this;
            SetMountActive();
        }
        else if ((queue.Count == 0) || (queue.Peek().importance <= this.importance))
        {
            queue.Push(this);
        }
        else
        {
            SORT_QUEUE(this);
        }
        this.bound = true;
    }

    public static void ClearTemporaryCameraMount()
    {
        if (createdTemporaryCameraMount && (temporaryCameraMount != null))
        {
            Object.Destroy(temporaryCameraMount);
            Object.Destroy(temporaryCameraMountGameObject);
            createdTemporaryCameraMount = false;
        }
    }

    public static CameraMount CreateTemporaryCameraMount(CameraMount copyFrom)
    {
        return CreateTemporaryCameraMount(copyFrom, null, false);
    }

    public static CameraMount CreateTemporaryCameraMount(CameraMount copyFrom, Transform parent)
    {
        return CreateTemporaryCameraMount(copyFrom, parent, (bool) parent);
    }

    private static CameraMount CreateTemporaryCameraMount(CameraMount copyFrom, Transform parent, bool hasParent)
    {
        if (creatingTemporaryCameraMount)
        {
            throw new InvalidOperationException("Invalid/unexpected call stack recursion");
        }
        ClearTemporaryCameraMount();
        try
        {
            creatingTemporaryCameraMount = true;
            temporaryCameraMountSource = copyFrom;
            temporaryCameraMountHasParent = hasParent;
            temporaryCameraMountParent = parent;
            Type[] components = new Type[] { typeof(CameraMount) };
            GameObject obj2 = new GameObject("__-temp mount-__", components) {
                hideFlags = HideFlags.DontSave
            };
            temporaryCameraMountGameObject = obj2;
        }
        finally
        {
            creatingTemporaryCameraMount = false;
            temporaryCameraMountSource = null;
            temporaryCameraMountHasParent = false;
            temporaryCameraMountParent = null;
            createdTemporaryCameraMount = (bool) temporaryCameraMount;
        }
        return temporaryCameraMount;
    }

    public void EnableTransition(float duration, TransitionFunction function)
    {
        if (!this.open)
        {
            this.open = true;
            if (this.isActiveMount)
            {
                CameraFX.TransitionNow(duration, function);
            }
        }
    }

    public void EnableTransitionSpeed(float metersPerSecond, TransitionFunction function)
    {
        if (!this.open)
        {
            Vector3 vector;
            this.open = true;
            if (this.isActiveMount && MountedCamera.GetPoint(out vector))
            {
                float num = Vector3.Distance(this.camera.worldToCameraMatrix.MultiplyPoint(Vector3.zero), vector);
                if (num != 0f)
                {
                    CameraFX.TransitionNow(num / metersPerSecond, function);
                }
            }
        }
    }

    private void OnDestroy()
    {
        this.destroyed = true;
        if (this.bound)
        {
            this.UnBind();
        }
    }

    private void OnNotActiveMount()
    {
    }

    public void OnPostMount(MountedCamera camera)
    {
    }

    public void OnPreMount(MountedCamera camera)
    {
    }

    private void OnSetActiveMount()
    {
    }

    private static void REMOVE_FROM_QUEUE(CameraMount rem)
    {
        try
        {
            int count = queue.Count;
            for (int i = 0; i < count; i++)
            {
                CameraMount item = queue.Pop();
                if (item != rem)
                {
                    WORK_LATE.list.Add(item);
                }
            }
            WORK_LATE.list.Reverse();
            foreach (CameraMount mount2 in WORK_LATE.list)
            {
                queue.Push(mount2);
            }
        }
        finally
        {
            WORK_LATE.list.Clear();
        }
    }

    private static void SetMountActive()
    {
        try
        {
            top.OnSetActiveMount();
        }
        catch (Exception exception)
        {
            Debug.LogError(exception, top);
        }
    }

    private static void SetMountInactive()
    {
        try
        {
            top.OnNotActiveMount();
        }
        catch (Exception exception)
        {
            Debug.LogError(exception, top);
        }
    }

    private static void SORT_QUEUE()
    {
        WORK_LATE.list.AddRange(queue);
        try
        {
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = (a, b) => a.importance.CompareTo(b.importance);
            }
            WORK_LATE.list.Sort(<>f__am$cache11);
            queue.Clear();
            foreach (CameraMount mount in WORK_LATE.list)
            {
                queue.Push(mount);
            }
        }
        finally
        {
            WORK_LATE.list.Clear();
        }
    }

    private static void SORT_QUEUE(CameraMount addExtra)
    {
        WORK_LATE.list.Add(addExtra);
        SORT_QUEUE();
    }

    private void UnBind()
    {
        if (top == this)
        {
            SetMountInactive();
            if (queue.Count > 0)
            {
                top = queue.Pop();
                SetMountActive();
            }
            else
            {
                top = null;
            }
        }
        else if (queue.Count > 1)
        {
            if (queue.Peek() == this)
            {
                queue.Pop();
            }
            else
            {
                REMOVE_FROM_QUEUE(this);
            }
        }
        else
        {
            queue.Pop();
        }
        this.bound = false;
    }

    public static CameraMount current
    {
        get
        {
            return top;
        }
    }

    [Obsolete("use the open property instead!", true)]
    public bool enabled
    {
        get
        {
            return this.open;
        }
        set
        {
            this.open = value;
        }
    }

    public bool isActiveMount
    {
        get
        {
            return (top == this);
        }
    }

    public bool open
    {
        get
        {
            return (!this.awoke ? this.autoBind : this.bound);
        }
        set
        {
            if (!this.destroyed)
            {
                if (this.awoke)
                {
                    if (this.bound != value)
                    {
                        if (this.bound)
                        {
                            this.UnBind();
                        }
                        else
                        {
                            this.Bind();
                        }
                    }
                }
                else
                {
                    this.autoBind = value;
                }
            }
        }
    }

    private static Stack<CameraMount> queue
    {
        get
        {
            return QUEUE_LATE.queue;
        }
    }

    private static class QUEUE_LATE
    {
        public static readonly Stack<CameraMount> queue = new Stack<CameraMount>();
    }

    private static class WORK_LATE
    {
        public static readonly List<CameraMount> list = new List<CameraMount>();
    }
}

