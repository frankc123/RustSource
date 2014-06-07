using NGUI.Meshing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Panel"), ExecuteInEditMode]
public class UIPanel : MonoBehaviour
{
    [NonSerialized]
    private UIPanelMaterialPropertyBlock _propertyBlock;
    [SerializeField]
    private UIPanel _rootPanel;
    public bool depthPass;
    public bool generateNormals;
    private int globalIndex = -1;
    private Ray lastRayTrace;
    private bool lastRayTraceInside;
    [SerializeField, HideInInspector]
    private bool manualPanelUpdate;
    private MeshBuffer mCacheBuffer = new MeshBuffer();
    private Camera mCam;
    private HashSet<UIMaterial> mChanged = new HashSet<UIMaterial>();
    private bool mChangedLastFrame;
    private bool mCheckVisibility;
    private OrderedDictionary mChildren = new OrderedDictionary();
    [SerializeField, HideInInspector]
    private UIDrawCall.Clipping mClipping;
    [HideInInspector, SerializeField]
    private Vector4 mClipRange = Vector4.zero;
    [SerializeField, HideInInspector]
    private Vector2 mClipSoftness = new Vector2(40f, 40f);
    private bool mCulled;
    private float mCullTime;
    [HideInInspector, SerializeField]
    private DebugInfo mDebugInfo = DebugInfo.Gizmos;
    private bool mDepthChanged;
    private int mDrawCallCount;
    private UIDrawCall mDrawCalls;
    private static List<UINode> mHierarchy = new List<UINode>();
    private HashSet<UIHotSpot> mHotSpots;
    private int mLayer = -1;
    private float mMatrixTime;
    private Vector2 mMax = Vector2.zero;
    private Vector2 mMin = Vector2.zero;
    private bool mRebuildAll;
    private List<Transform> mRemoved = new List<Transform>();
    private static float[] mTemp = new float[4];
    private Transform mTrans;
    private List<UIWidget> mWidgets = new List<UIWidget>();
    private Matrix4x4 mWorldToLocal = Matrix4x4.identity;
    public bool showInPanelTool = true;
    private int traceID;
    public bool widgetsAreStatic;

    private UINode AddTransform(Transform t)
    {
        UINode node = null;
        UINode node2 = null;
        while ((t != null) && (t != this.cachedTransform))
        {
            if (this.mChildren.Contains(t))
            {
                if (node2 == null)
                {
                    node2 = (UINode) this.mChildren[t];
                }
                return node2;
            }
            node = new UINode(t);
            if (node2 == null)
            {
                node2 = node;
            }
            this.mChildren.Add(t, node);
            t = t.parent;
        }
        return node2;
    }

    public void AddWidget(UIWidget w)
    {
        if (w != null)
        {
            UINode node = this.AddTransform(w.cachedTransform);
            if (node != null)
            {
                node.widget = w;
                if (!this.mWidgets.Contains(w))
                {
                    this.mWidgets.Add(w);
                    if (!this.mChanged.Contains(w.material))
                    {
                        this.mChanged.Add(w.material);
                        this.mChangedLastFrame = true;
                    }
                    this.mDepthChanged = true;
                }
            }
            else
            {
                Debug.LogError("Unable to find an appropriate UIRoot for " + NGUITools.GetHierarchy(w.gameObject) + "\nPlease make sure that there is at least one game object above this widget!", w.gameObject);
            }
        }
    }

    protected void Awake()
    {
        Global.RegisterPanel(this);
    }

    public Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
    {
        float num = this.clipRange.z * 0.5f;
        float num2 = this.clipRange.w * 0.5f;
        Vector2 minRect = new Vector2(min.x, min.y);
        Vector2 maxRect = new Vector2(max.x, max.y);
        Vector2 minArea = new Vector2(this.clipRange.x - num, this.clipRange.y - num2);
        Vector2 maxArea = new Vector2(this.clipRange.x + num, this.clipRange.y + num2);
        if (this.clipping == UIDrawCall.Clipping.SoftClip)
        {
            minArea.x += this.clipSoftness.x;
            minArea.y += this.clipSoftness.y;
            maxArea.x -= this.clipSoftness.x;
            maxArea.y -= this.clipSoftness.y;
        }
        return (Vector3) NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
    }

    private static bool CheckRayEnterClippingRect(Ray ray, Transform transform, Vector4 clipRange)
    {
        float num;
        Plane plane = new Plane(transform.forward, transform.position);
        if (plane.Raycast(ray, out num))
        {
            Vector3 point = transform.InverseTransformPoint(ray.GetPoint(num));
            clipRange.z = Mathf.Abs(clipRange.z);
            clipRange.w = Mathf.Abs(clipRange.w);
            Rect rect = new Rect(clipRange.x - (clipRange.z / 2f), clipRange.y - (clipRange.w / 2f), clipRange.z, clipRange.w);
            return rect.Contains(point);
        }
        return false;
    }

    public bool ConstrainTargetToBounds(Transform target, bool immediate)
    {
        AABBox targetBounds = NGUIMath.CalculateRelativeWidgetBounds(this.cachedTransform, target);
        return this.ConstrainTargetToBounds(target, ref targetBounds, immediate);
    }

    public bool ConstrainTargetToBounds(Transform target, ref AABBox targetBounds, bool immediate)
    {
        Vector3 vector = this.CalculateConstrainOffset(targetBounds.min, targetBounds.max);
        if (vector.magnitude <= 0f)
        {
            return false;
        }
        if (immediate)
        {
            target.localPosition += vector;
            targetBounds.center += vector;
            SpringPosition component = target.GetComponent<SpringPosition>();
            if (component != null)
            {
                component.enabled = false;
            }
        }
        else
        {
            SpringPosition position2 = SpringPosition.Begin(target.gameObject, target.localPosition + vector, 13f);
            position2.ignoreTimeScale = true;
            position2.worldSpace = false;
        }
        return true;
    }

    public bool Contains(UIDrawCall drawcall)
    {
        for (UIDrawCall.Iterator iterator = (UIDrawCall.Iterator) this.mDrawCalls; iterator.Has; iterator = iterator.Next)
        {
            if (object.ReferenceEquals(drawcall, iterator.Current))
            {
                return true;
            }
        }
        return false;
    }

    private void DefaultLateUpdate()
    {
        if (!this.manualPanelUpdate)
        {
            this.PanelUpdate(true);
        }
        else
        {
            this.FillUpdate();
        }
    }

    private void Delete(ref UIDrawCall.Iterator iter)
    {
        if (iter.Has)
        {
            UIDrawCall current = iter.Current;
            if (object.ReferenceEquals(current, this.mDrawCalls))
            {
                this.mDrawCalls = iter.Next.Current;
            }
            iter = iter.Next;
            current.LinkedList__Remove();
            this.mDrawCallCount--;
            NGUITools.DestroyImmediate(current.gameObject);
        }
    }

    private void Fill(UIMaterial mat)
    {
        int count = this.mWidgets.Count;
        while (count > 0)
        {
            if (this.mWidgets[--count] == null)
            {
                this.mWidgets.RemoveAt(count);
            }
        }
        int num2 = 0;
        int num3 = this.mWidgets.Count;
        while (num2 < num3)
        {
            UIWidget widget = this.mWidgets[num2];
            if ((widget.visibleFlag == 1) && (widget.material == mat))
            {
                if (this.GetNode(widget.cachedTransform) != null)
                {
                    widget.WriteToBuffers(this.mCacheBuffer);
                }
                else
                {
                    Debug.LogError("No transform found for " + NGUITools.GetHierarchy(widget.gameObject), this);
                }
            }
            num2++;
        }
        if (this.mCacheBuffer.vSize > 0)
        {
            UIDrawCall current = this.GetDrawCall(mat, true).Current;
            current.depthPass = this.depthPass;
            current.panelPropertyBlock = this.propertyBlock;
            current.Set(this.mCacheBuffer);
        }
        else
        {
            UIDrawCall.Iterator drawCall = this.GetDrawCall(mat, false);
            if (drawCall.Has)
            {
                this.Delete(ref drawCall);
            }
        }
        this.mCacheBuffer.Clear();
    }

    private void FillUpdate()
    {
        foreach (UIMaterial material in this.mChanged)
        {
            this.Fill(material);
        }
        this.UpdateDrawcalls();
        this.mChanged.Clear();
    }

    public static UIPanel Find(Transform trans)
    {
        return Find(trans, true);
    }

    public static UIPanel Find(Transform trans, bool createIfMissing)
    {
        Transform transform = trans;
        UIPanel component = null;
        while ((component == null) && (trans != null))
        {
            component = trans.GetComponent<UIPanel>();
            if ((component != null) || (trans.parent == null))
            {
                break;
            }
            trans = trans.parent;
        }
        if ((createIfMissing && (component == null)) && (trans != transform))
        {
            component = trans.gameObject.AddComponent<UIPanel>();
            SetChildLayer(component.cachedTransform, component.gameObject.layer);
        }
        return component;
    }

    public static UIPanel FindRoot(Transform trans)
    {
        UIPanel panel = Find(trans);
        return ((panel == null) ? null : panel.RootPanel);
    }

    private int GetChangeFlag(UINode start)
    {
        int num2;
        int changeFlag = start.changeFlag;
        if (changeFlag != -1)
        {
            return changeFlag;
        }
        Transform parent = start.trans.parent;
        while (true)
        {
            if (!this.mChildren.Contains(parent))
            {
                break;
            }
            UINode item = (UINode) this.mChildren[parent];
            changeFlag = item.changeFlag;
            parent = parent.parent;
            if (changeFlag != -1)
            {
                goto Label_007D;
            }
            mHierarchy.Add(item);
        }
        changeFlag = 0;
    Label_007D:
        num2 = 0;
        int count = mHierarchy.Count;
        while (num2 < count)
        {
            UINode node2 = mHierarchy[num2];
            node2.changeFlag = changeFlag;
            num2++;
        }
        mHierarchy.Clear();
        return changeFlag;
    }

    private UIDrawCall.Iterator GetDrawCall(UIMaterial mat, bool createIfMissing)
    {
        for (UIDrawCall.Iterator iterator = (UIDrawCall.Iterator) this.mDrawCalls; iterator.Has; iterator = iterator.Next)
        {
            if (iterator.Current.material == mat)
            {
                return iterator;
            }
        }
        UIDrawCall call = null;
        if (createIfMissing)
        {
            call = new GameObject("_UIDrawCall [" + mat.name + "]") { hideFlags = HideFlags.DontSave, layer = base.gameObject.layer }.AddComponent<UIDrawCall>();
            call.material = mat;
            call.LinkedList__Insert(ref this.mDrawCalls);
            this.mDrawCallCount++;
        }
        return (UIDrawCall.Iterator) call;
    }

    private UINode GetNode(Transform t)
    {
        UINode node = null;
        if ((t != null) && this.mChildren.Contains(t))
        {
            node = (UINode) this.mChildren[t];
        }
        return node;
    }

    public static void GlobalUpdate()
    {
        Global.PanelUpdate();
    }

    internal bool InsideClippingRect(Ray ray, int traceID)
    {
        if (this.clipping == UIDrawCall.Clipping.None)
        {
            return true;
        }
        if (((traceID != this.traceID) || (ray.origin != this.lastRayTrace.origin)) || (ray.direction != this.lastRayTrace.direction))
        {
            this.traceID = traceID;
            this.lastRayTrace = ray;
            this.lastRayTraceInside = CheckRayEnterClippingRect(ray, base.transform, this.clipRange);
        }
        return this.lastRayTraceInside;
    }

    public bool IsVisible(UIWidget w)
    {
        if ((!w.enabled || !w.gameObject.activeInHierarchy) || (w.color.a < 0.001f))
        {
            return false;
        }
        if (this.mClipping == UIDrawCall.Clipping.None)
        {
            return true;
        }
        Vector2 relativeSize = w.relativeSize;
        Vector2 vector2 = Vector2.Scale(w.pivotOffset, relativeSize);
        Vector2 vector3 = vector2;
        vector2.x += relativeSize.x;
        vector2.y -= relativeSize.y;
        Transform cachedTransform = w.cachedTransform;
        Vector3 a = cachedTransform.TransformPoint((Vector3) vector2);
        Vector3 b = cachedTransform.TransformPoint((Vector3) new Vector2(vector2.x, vector3.y));
        Vector3 c = cachedTransform.TransformPoint((Vector3) new Vector2(vector3.x, vector2.y));
        Vector3 d = cachedTransform.TransformPoint((Vector3) vector3);
        return this.IsVisible(a, b, c, d);
    }

    private bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        this.UpdateTransformMatrix();
        a = this.mWorldToLocal.MultiplyPoint3x4(a);
        b = this.mWorldToLocal.MultiplyPoint3x4(b);
        c = this.mWorldToLocal.MultiplyPoint3x4(c);
        d = this.mWorldToLocal.MultiplyPoint3x4(d);
        mTemp[0] = a.x;
        mTemp[1] = b.x;
        mTemp[2] = c.x;
        mTemp[3] = d.x;
        float num = Mathf.Min(mTemp);
        float num2 = Mathf.Max(mTemp);
        mTemp[0] = a.y;
        mTemp[1] = b.y;
        mTemp[2] = c.y;
        mTemp[3] = d.y;
        float num3 = Mathf.Min(mTemp);
        float num4 = Mathf.Max(mTemp);
        if (num2 < this.mMin.x)
        {
            return false;
        }
        if (num4 < this.mMin.y)
        {
            return false;
        }
        if (num > this.mMax.x)
        {
            return false;
        }
        if (num3 > this.mMax.y)
        {
            return false;
        }
        return true;
    }

    public bool ManualPanelUpdate()
    {
        if (this.manualPanelUpdate && base.enabled)
        {
            this.PanelUpdate(false);
            return true;
        }
        return false;
    }

    public void MarkMaterialAsChanged(UIMaterial mat, bool sort)
    {
        if (mat != null)
        {
            if (sort)
            {
                this.mDepthChanged = true;
            }
            if (this.mChanged.Add(mat))
            {
                this.mChangedLastFrame = true;
            }
        }
    }

    protected void OnDestroy()
    {
        Global.UnregisterPanel(this);
        if (this.mHotSpots != null)
        {
            HashSet<UIHotSpot> mHotSpots = this.mHotSpots;
            this.mHotSpots = null;
            foreach (UIHotSpot spot in mHotSpots)
            {
                spot.OnPanelDestroy();
            }
        }
    }

    protected void OnDisable()
    {
        Global.PanelDisabled(this);
        if (this.mHotSpots != null)
        {
            foreach (UIHotSpot spot in this.mHotSpots)
            {
                spot.OnPanelDisable();
            }
        }
        UIDrawCall.Iterator mDrawCalls = (UIDrawCall.Iterator) this.mDrawCalls;
        while (mDrawCalls.Has)
        {
            UIDrawCall current = mDrawCalls.Current;
            mDrawCalls = mDrawCalls.Next;
            NGUITools.DestroyImmediate(current.gameObject);
        }
        this.mDrawCalls = null;
        this.mChanged.Clear();
        this.mChildren.Clear();
    }

    protected void OnEnable()
    {
        Global.PanelEnabled(this);
        if (this.mHotSpots != null)
        {
            foreach (UIHotSpot spot in this.mHotSpots)
            {
                spot.OnPanelEnable();
            }
        }
        int num = 0;
        int count = this.mWidgets.Count;
        while (num < count)
        {
            this.AddWidget(this.mWidgets[num]);
            num++;
        }
        this.mRebuildAll = true;
    }

    private void PanelUpdate(bool letFill)
    {
        this.UpdateTransformMatrix();
        this.UpdateTransforms();
        if (this.mLayer != base.gameObject.layer)
        {
            this.mLayer = base.gameObject.layer;
            UICamera camera = UICamera.FindCameraForLayer(this.mLayer);
            this.mCam = (camera == null) ? NGUITools.FindCameraForLayer(this.mLayer) : camera.cachedCamera;
            SetChildLayer(this.cachedTransform, this.mLayer);
            for (UIDrawCall.Iterator iterator = (UIDrawCall.Iterator) this.mDrawCalls; iterator.Has; iterator = iterator.Next)
            {
                iterator.Current.gameObject.layer = this.mLayer;
            }
        }
        this.UpdateWidgets();
        if (this.mDepthChanged)
        {
            this.mDepthChanged = false;
            this.mWidgets.Sort(new Comparison<UIWidget>(UIWidget.CompareFunc));
        }
        if (letFill)
        {
            this.FillUpdate();
        }
        else
        {
            this.UpdateDrawcalls();
        }
        this.mRebuildAll = false;
    }

    public void Refresh()
    {
        base.BroadcastMessage("Update", SendMessageOptions.DontRequireReceiver);
        this.DefaultLateUpdate();
    }

    internal static void RegisterHotSpot(UIPanel panel, UIHotSpot hotSpot)
    {
        if (panel.mHotSpots == null)
        {
            panel.mHotSpots = new HashSet<UIHotSpot>();
        }
        if (panel.mHotSpots.Add(hotSpot))
        {
            if (panel.enabled)
            {
                hotSpot.OnPanelEnable();
            }
            else
            {
                hotSpot.OnPanelDisable();
            }
        }
    }

    private void RemoveTransform(Transform t)
    {
        if (t != null)
        {
            while (this.mChildren.Contains(t))
            {
                this.mChildren.Remove(t);
                t = t.parent;
                if (((t == null) || (t == this.mTrans)) || (t.childCount > 1))
                {
                    break;
                }
            }
        }
    }

    public void RemoveWidget(UIWidget w)
    {
        if (w != null)
        {
            UINode node = this.GetNode(w.cachedTransform);
            if (node != null)
            {
                if ((node.visibleFlag == 1) && !this.mChanged.Contains(w.material))
                {
                    this.mChanged.Add(w.material);
                    this.mChangedLastFrame = true;
                }
                this.RemoveTransform(w.cachedTransform);
            }
            this.mWidgets.Remove(w);
        }
    }

    private static void SetChildLayer(Transform t, int layer)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            if (child.GetComponent<UIPanel>() == null)
            {
                child.gameObject.layer = layer;
                SetChildLayer(child, layer);
            }
        }
    }

    protected void Start()
    {
        this.mLayer = base.gameObject.layer;
        UICamera camera = UICamera.FindCameraForLayer(this.mLayer);
        this.mCam = (camera == null) ? NGUITools.FindCameraForLayer(this.mLayer) : camera.cachedCamera;
    }

    internal static void UnregisterHotSpot(UIPanel panel, UIHotSpot hotSpot)
    {
        if (((panel.mHotSpots != null) && panel.mHotSpots.Remove(hotSpot)) && panel.enabled)
        {
            hotSpot.OnPanelDisable();
        }
    }

    public void UpdateDrawcalls()
    {
        Vector4 zero = Vector4.zero;
        if (this.mClipping != UIDrawCall.Clipping.None)
        {
            zero = new Vector4(this.mClipRange.x, this.mClipRange.y, this.mClipRange.z * 0.5f, this.mClipRange.w * 0.5f);
        }
        if (zero.z == 0f)
        {
            zero.z = Screen.width * 0.5f;
        }
        if (zero.w == 0f)
        {
            zero.w = Screen.height * 0.5f;
        }
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsWebPlayer:
            case RuntimePlatform.WindowsEditor:
                zero.x -= 0.5f;
                zero.y += 0.5f;
                break;
        }
        Vector3 position = this.cachedTransform.position;
        Quaternion rotation = this.cachedTransform.rotation;
        Vector3 lossyScale = this.cachedTransform.lossyScale;
        UIDrawCall.Iterator mDrawCalls = (UIDrawCall.Iterator) this.mDrawCalls;
        while (mDrawCalls.Has)
        {
            UIDrawCall current = mDrawCalls.Current;
            mDrawCalls = mDrawCalls.Next;
            current.clipping = this.mClipping;
            current.clipRange = zero;
            current.clipSoftness = this.mClipSoftness;
            current.depthPass = this.depthPass;
            current.panelPropertyBlock = this.propertyBlock;
            Transform transform = current.transform;
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = lossyScale;
        }
    }

    private void UpdateTransformMatrix()
    {
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        if ((realtimeSinceStartup == 0f) || (this.mMatrixTime != realtimeSinceStartup))
        {
            this.mMatrixTime = realtimeSinceStartup;
            this.mWorldToLocal = this.cachedTransform.worldToLocalMatrix;
            if (this.mClipping != UIDrawCall.Clipping.None)
            {
                Vector2 vector = new Vector2(this.mClipRange.z, this.mClipRange.w);
                if (vector.x == 0f)
                {
                    vector.x = (this.mCam != null) ? this.mCam.pixelWidth : ((float) Screen.width);
                }
                if (vector.y == 0f)
                {
                    vector.y = (this.mCam != null) ? this.mCam.pixelHeight : ((float) Screen.height);
                }
                vector = (Vector2) (vector * 0.5f);
                this.mMin.x = this.mClipRange.x - vector.x;
                this.mMin.y = this.mClipRange.y - vector.y;
                this.mMax.x = this.mClipRange.x + vector.x;
                this.mMax.y = this.mClipRange.y + vector.y;
            }
        }
    }

    private void UpdateTransforms()
    {
        this.mChangedLastFrame = false;
        bool flag = false;
        bool flag2 = Time.realtimeSinceStartup > this.mCullTime;
        if (!this.widgetsAreStatic || (flag2 != this.mCulled))
        {
            int num = 0;
            int count = this.mChildren.Count;
            while (num < count)
            {
                UINode node = (UINode) this.mChildren[num];
                if (node.trans == null)
                {
                    this.mRemoved.Add(node.trans);
                }
                else if (node.HasChanged())
                {
                    node.changeFlag = 1;
                    flag = true;
                }
                else
                {
                    node.changeFlag = -1;
                }
                num++;
            }
            int num3 = 0;
            int num4 = this.mRemoved.Count;
            while (num3 < num4)
            {
                this.mChildren.Remove(this.mRemoved[num3]);
                num3++;
            }
            this.mRemoved.Clear();
        }
        if (!this.mCulled && flag2)
        {
            this.mCheckVisibility = true;
        }
        if ((this.mCheckVisibility || flag) || this.mRebuildAll)
        {
            int num5 = 0;
            int num6 = this.mChildren.Count;
            while (num5 < num6)
            {
                UINode start = (UINode) this.mChildren[num5];
                if (start.widget != null)
                {
                    int num7 = 1;
                    if (flag2 || flag)
                    {
                        if (start.changeFlag == -1)
                        {
                            start.changeFlag = this.GetChangeFlag(start);
                        }
                        if (flag2)
                        {
                            num7 = (!this.mCheckVisibility && (start.changeFlag != 1)) ? start.visibleFlag : (!this.IsVisible(start.widget) ? 0 : 1);
                        }
                    }
                    if (start.visibleFlag != num7)
                    {
                        start.changeFlag = 1;
                    }
                    if ((start.changeFlag == 1) && ((num7 == 1) || (start.visibleFlag != 0)))
                    {
                        start.visibleFlag = num7;
                        UIMaterial item = start.widget.material;
                        if (!this.mChanged.Contains(item))
                        {
                            this.mChanged.Add(item);
                            this.mChangedLastFrame = true;
                        }
                    }
                }
                num5++;
            }
        }
        this.mCulled = flag2;
        this.mCheckVisibility = false;
    }

    private void UpdateWidgets()
    {
        int num = 0;
        int count = this.mChildren.Count;
        while (num < count)
        {
            UINode node = (UINode) this.mChildren[num];
            UIWidget widget = node.widget;
            if (((node.visibleFlag == 1) && (widget != null)) && (widget.UpdateGeometry(ref this.mWorldToLocal, node.changeFlag == 1, this.generateNormals) && !this.mChanged.Contains(widget.material)))
            {
                this.mChanged.Add(widget.material);
                this.mChangedLastFrame = true;
            }
            node.changeFlag = 0;
            num++;
        }
    }

    public bool WatchesTransform(Transform t)
    {
        return ((t == this.cachedTransform) || this.mChildren.Contains(t));
    }

    public Transform cachedTransform
    {
        get
        {
            if (this.mTrans == null)
            {
                this.mTrans = base.transform;
            }
            return this.mTrans;
        }
    }

    public bool changedLastFrame
    {
        get
        {
            return this.mChangedLastFrame;
        }
    }

    public UIDrawCall.Clipping clipping
    {
        get
        {
            return this.mClipping;
        }
        set
        {
            if (this.mClipping != value)
            {
                this.mCheckVisibility = true;
                this.mClipping = value;
                this.UpdateDrawcalls();
            }
        }
    }

    public Vector4 clipRange
    {
        get
        {
            return this.mClipRange;
        }
        set
        {
            if (this.mClipRange != value)
            {
                this.mCullTime = (this.mCullTime != 0f) ? (Time.realtimeSinceStartup + 0.15f) : 0.001f;
                this.mCheckVisibility = true;
                this.mClipRange = value;
                this.UpdateDrawcalls();
            }
        }
    }

    public Vector2 clipSoftness
    {
        get
        {
            return this.mClipSoftness;
        }
        set
        {
            if (this.mClipSoftness != value)
            {
                this.mClipSoftness = value;
                this.UpdateDrawcalls();
            }
        }
    }

    public DebugInfo debugInfo
    {
        get
        {
            return this.mDebugInfo;
        }
        set
        {
            if (this.mDebugInfo != value)
            {
                this.mDebugInfo = value;
                UIDrawCall.Iterator mDrawCalls = (UIDrawCall.Iterator) this.mDrawCalls;
                HideFlags flags = (this.mDebugInfo != DebugInfo.Geometry) ? HideFlags.HideAndDontSave : (HideFlags.NotEditable | HideFlags.DontSave);
                while (mDrawCalls.Has)
                {
                    GameObject gameObject = mDrawCalls.Current.gameObject;
                    mDrawCalls = mDrawCalls.Next;
                    gameObject.SetActive(false);
                    gameObject.hideFlags = flags;
                    gameObject.SetActive(true);
                }
            }
        }
    }

    public int drawCallCount
    {
        get
        {
            return this.mDrawCallCount;
        }
    }

    public UIDrawCall.Iterator drawCalls
    {
        get
        {
            return (UIDrawCall.Iterator) this.mDrawCalls;
        }
    }

    public bool manUp
    {
        get
        {
            return this.manualPanelUpdate;
        }
    }

    public UIPanelMaterialPropertyBlock propertyBlock
    {
        get
        {
            return this._propertyBlock;
        }
        set
        {
            this._propertyBlock = value;
        }
    }

    public UIPanel RootPanel
    {
        get
        {
            return ((this._rootPanel == null) ? this : this._rootPanel);
        }
        set
        {
            if (value == this)
            {
                this._rootPanel = null;
            }
            else
            {
                this._rootPanel = value;
            }
        }
    }

    public List<UIWidget> widgets
    {
        get
        {
            return this.mWidgets;
        }
    }

    public enum DebugInfo
    {
        None,
        Gizmos,
        Geometry
    }

    private static class Global
    {
        public static void PanelDisabled(UIPanel panel)
        {
            if (!noGlobal)
            {
                g.allEnabled.Remove(panel);
            }
        }

        public static void PanelEnabled(UIPanel panel)
        {
            if (!noGlobal)
            {
                g.allEnabled.Add(panel);
            }
        }

        public static void PanelUpdate()
        {
            if (!noGlobal)
            {
                try
                {
                    g.allEnableSwap.AddRange(g.allEnabled);
                    foreach (UIPanel panel in g.allEnableSwap)
                    {
                        if ((panel != null) && panel.enabled)
                        {
                            panel.DefaultLateUpdate();
                        }
                    }
                }
                finally
                {
                    g.allEnableSwap.Clear();
                }
            }
        }

        public static void RegisterPanel(UIPanel panel)
        {
            if (!noGlobal)
            {
                UIGlobal.EnsureGlobal();
                if (panel.globalIndex == -1)
                {
                    panel.globalIndex = g.allPanels.Count;
                    g.allPanels.Add(panel);
                }
            }
        }

        public static void UnregisterPanel(UIPanel panel)
        {
            if (!noGlobal && (panel.globalIndex != -1))
            {
                g.allPanels.RemoveAt(panel.globalIndex);
                int globalIndex = panel.globalIndex;
                int count = g.allPanels.Count;
                while (globalIndex < count)
                {
                    g.allPanels[globalIndex].globalIndex = globalIndex;
                    globalIndex++;
                }
                panel.globalIndex = -1;
            }
        }

        public static bool noGlobal
        {
            get
            {
                return !Application.isPlaying;
            }
        }

        private static class g
        {
            public static HashSet<UIPanel> allEnabled = new HashSet<UIPanel>();
            public static List<UIPanel> allEnableSwap = new List<UIPanel>();
            public static List<UIPanel> allPanels = new List<UIPanel>();

            static g()
            {
                UIGlobal.EnsureGlobal();
            }
        }
    }
}

