using NGUI.Meshing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class UIWidget : MonoBehaviour
{
    private UIGeometry __mGeom;
    private int globalIndex = -1;
    private bool gotCachedTransform;
    [SerializeField, HideInInspector]
    private bool mAlphaUnchecked;
    private bool mChangedCall = true;
    [SerializeField, HideInInspector]
    private Color mColor = Color.white;
    [SerializeField, HideInInspector]
    private int mDepth;
    private Vector3 mDiffPos;
    private Quaternion mDiffRot;
    private Vector3 mDiffScale;
    [NonSerialized]
    private bool mForcedChanged;
    [HideInInspector, SerializeField]
    private Material mMat;
    private UIPanel mPanel;
    [HideInInspector, SerializeField]
    private Pivot mPivot = Pivot.Center;
    protected bool mPlayMode = true;
    private Texture mTex;
    private Transform mTrans;
    private int mVisibleFlag = -1;
    private static Vector2[] tempVector2s = new Vector2[] { new Vector2(), new Vector2() };
    private static WidgetFlags[] tempWidgetFlags = new WidgetFlags[2];
    [NonSerialized]
    protected readonly WidgetFlags widgetFlags;

    protected UIWidget(WidgetFlags flags)
    {
        this.widgetFlags = flags;
    }

    protected void Awake()
    {
        this.mPlayMode = Application.isPlaying;
        Global.RegisterWidget(this);
    }

    protected void ChangedAuto()
    {
        this.mChangedCall = true;
    }

    private void CheckLayer()
    {
        if ((this.mPanel != null) && (this.mPanel.gameObject.layer != base.gameObject.layer))
        {
            Debug.LogWarning("You can't place widgets on a layer different than the UIPanel that manages them.\nIf you want to move widgets to a different layer, parent them to a new panel instead.", this);
            base.gameObject.layer = this.mPanel.gameObject.layer;
        }
    }

    private void CheckParent()
    {
        if (this.mPanel != null)
        {
            bool flag = true;
            for (Transform transform = this.cachedTransform.parent; transform != null; transform = transform.parent)
            {
                if (transform == this.mPanel.cachedTransform)
                {
                    break;
                }
                if (!this.mPanel.WatchesTransform(transform))
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
            {
                if (((byte) (this.widgetFlags & WidgetFlags.KeepsMaterial)) != 0x40)
                {
                    this.material = null;
                }
                this.mPanel = null;
                this.CreatePanel();
            }
        }
    }

    public static int CompareFunc(UIWidget left, UIWidget right)
    {
        if (left.mDepth > right.mDepth)
        {
            return 1;
        }
        if (left.mDepth < right.mDepth)
        {
            return -1;
        }
        return 0;
    }

    private void CreatePanel()
    {
        if (((this.mPanel == null) && base.enabled) && (base.gameObject.activeInHierarchy && (this.material != null)))
        {
            this.mPanel = UIPanel.Find(this.cachedTransform);
            if (this.mPanel != null)
            {
                this.CheckLayer();
                this.mPanel.AddWidget(this);
                this.mChangedCall = true;
            }
        }
    }

    protected static Vector2 DefaultPivot(Pivot pivot)
    {
        Vector2 vector;
        switch (pivot)
        {
            case Pivot.TopLeft:
                vector.x = 0f;
                vector.y = 0f;
                return vector;

            case Pivot.Top:
                vector.y = -0.5f;
                vector.x = -1f;
                return vector;

            case Pivot.TopRight:
                vector.y = 0f;
                vector.x = -1f;
                return vector;

            case Pivot.Left:
                vector.x = 0f;
                vector.y = 0.5f;
                return vector;

            case Pivot.Center:
                vector.x = -0.5f;
                vector.y = 0.5f;
                return vector;

            case Pivot.Right:
                vector.x = -1f;
                vector.y = 0.5f;
                return vector;

            case Pivot.BottomLeft:
                vector.x = 0f;
                vector.y = 1f;
                return vector;

            case Pivot.Bottom:
                vector.x = -0.5f;
                vector.y = 1f;
                return vector;

            case Pivot.BottomRight:
                vector.x = -1f;
                vector.y = 1f;
                return vector;
        }
        throw new NotImplementedException();
    }

    private void DefaultUpdate()
    {
        if (this.mPanel == null)
        {
            this.CreatePanel();
        }
        Vector3 localScale = this.cachedTransform.localScale;
        if (localScale.z != 1f)
        {
            localScale.z = 1f;
            this.mTrans.localScale = localScale;
        }
    }

    public void ForceReloadMaterial()
    {
        if (this.mMat != null)
        {
            if (this.mPanel != null)
            {
                this.mPanel.RemoveWidget(this);
            }
            this.mPanel = null;
            this.mTex = null;
            if (this.mMat != null)
            {
                this.CreatePanel();
            }
        }
    }

    protected virtual void GetCustomVector2s(int start, int end, WidgetFlags[] flags, Vector2[] v)
    {
        throw new NotSupportedException("Only call base.GetCustomVector2s when its something not supported by your implementation, otherwise the custructor for your class is incorrect in its usage.");
    }

    public void GetPivotOffsetAndRelativeSize(out Vector2 pivotOffset, out Vector2 relativeSize)
    {
        switch (((WidgetFlags) ((byte) (this.widgetFlags & (WidgetFlags.CustomPivotOffset | WidgetFlags.CustomRelativeSize)))))
        {
            case WidgetFlags.CustomPivotOffset:
                tempWidgetFlags[0] = WidgetFlags.CustomPivotOffset;
                this.GetCustomVector2s(0, 1, tempWidgetFlags, tempVector2s);
                pivotOffset = tempVector2s[0];
                relativeSize.x = relativeSize.y = 1f;
                break;

            case WidgetFlags.CustomRelativeSize:
                tempWidgetFlags[0] = WidgetFlags.CustomRelativeSize;
                this.GetCustomVector2s(0, 1, tempWidgetFlags, tempVector2s);
                relativeSize = tempVector2s[0];
                pivotOffset = DefaultPivot(this.mPivot);
                break;

            case (WidgetFlags.CustomPivotOffset | WidgetFlags.CustomRelativeSize):
                tempWidgetFlags[0] = WidgetFlags.CustomPivotOffset;
                tempWidgetFlags[1] = WidgetFlags.CustomRelativeSize;
                this.GetCustomVector2s(0, 2, tempWidgetFlags, tempVector2s);
                pivotOffset = tempVector2s[0];
                relativeSize = tempVector2s[1];
                break;

            default:
                pivotOffset = DefaultPivot(this.mPivot);
                relativeSize.x = relativeSize.y = 1f;
                break;
        }
    }

    public static void GlobalUpdate()
    {
        Global.WidgetUpdate();
    }

    public virtual void MakePixelPerfect()
    {
        Vector3 localScale = this.cachedTransform.localScale;
        int num = Mathf.RoundToInt(localScale.x);
        int num2 = Mathf.RoundToInt(localScale.y);
        localScale.x = num;
        localScale.y = num2;
        localScale.z = 1f;
        Vector3 localPosition = this.cachedTransform.localPosition;
        localPosition.z = Mathf.RoundToInt(localPosition.z);
        if (((num % 2) == 1) && (((this.pivot == Pivot.Top) || (this.pivot == Pivot.Center)) || (this.pivot == Pivot.Bottom)))
        {
            localPosition.x = Mathf.Floor(localPosition.x) + 0.5f;
        }
        else
        {
            localPosition.x = Mathf.Round(localPosition.x);
        }
        if (((num2 % 2) == 1) && (((this.pivot == Pivot.Left) || (this.pivot == Pivot.Center)) || (this.pivot == Pivot.Right)))
        {
            localPosition.y = Mathf.Ceil(localPosition.y) - 0.5f;
        }
        else
        {
            localPosition.y = Mathf.Round(localPosition.y);
        }
        this.cachedTransform.localPosition = localPosition;
        this.cachedTransform.localScale = localScale;
    }

    public virtual void MarkAsChanged()
    {
        this.mChangedCall = true;
        if ((((this.mPanel != null) && base.enabled) && (base.gameObject.activeInHierarchy && !Application.isPlaying)) && (this.material != null))
        {
            this.mPanel.AddWidget(this);
            this.CheckLayer();
        }
    }

    public void MarkAsChangedForced()
    {
        this.MarkAsChanged();
        this.mForcedChanged = true;
    }

    private void OnDestroy()
    {
        Global.UnregisterWidget(this);
        if (this.mPanel != null)
        {
            this.mPanel.RemoveWidget(this);
            this.mPanel = null;
        }
        this.__mGeom = null;
    }

    private void OnDisable()
    {
        Global.WidgetDisabled(this);
        if (((byte) (this.widgetFlags & WidgetFlags.KeepsMaterial)) != 0x40)
        {
            this.material = null;
        }
        else if (this.mPanel != null)
        {
            this.mPanel.RemoveWidget(this);
        }
        this.mPanel = null;
    }

    private void OnEnable()
    {
        Global.WidgetEnabled(this);
        this.mChangedCall = true;
        if (((byte) (this.widgetFlags & WidgetFlags.KeepsMaterial)) != 0x40)
        {
            this.mMat = null;
            this.mTex = null;
        }
        if ((this.mPanel != null) && (this.material != null))
        {
            this.mPanel.MarkMaterialAsChanged(this.material, false);
        }
    }

    public abstract void OnFill(MeshBuffer m);
    protected virtual void OnStart()
    {
    }

    public virtual bool OnUpdate()
    {
        return false;
    }

    private void Start()
    {
        this.OnStart();
        this.CreatePanel();
    }

    public bool UpdateGeometry(ref Matrix4x4 worldToPanel, bool parentMoved, bool generateNormals)
    {
        if (this.material == null)
        {
            return false;
        }
        UIGeometry mGeom = this.mGeom;
        if ((!this.OnUpdate() && !this.mChangedCall) && !this.mForcedChanged)
        {
            if (mGeom.hasVertices && parentMoved)
            {
                Matrix4x4 widgetToPanel = worldToPanel * this.cachedTransform.localToWorldMatrix;
                mGeom.Apply(ref widgetToPanel);
            }
            return false;
        }
        this.mChangedCall = false;
        this.mForcedChanged = false;
        mGeom.Clear();
        if (this.mAlphaUnchecked || !NGUITools.ZeroAlpha(this.mColor.a))
        {
            this.OnFill(mGeom.meshBuffer);
        }
        if (mGeom.hasVertices)
        {
            Vector3 vector;
            Vector2 vector2;
            Vector2 vector3;
            switch (((WidgetFlags) ((byte) (this.widgetFlags & (WidgetFlags.CustomPivotOffset | WidgetFlags.CustomRelativeSize)))))
            {
                case WidgetFlags.CustomPivotOffset:
                    tempWidgetFlags[0] = WidgetFlags.CustomPivotOffset;
                    this.GetCustomVector2s(0, 1, tempWidgetFlags, tempVector2s);
                    vector2 = tempVector2s[0];
                    vector3.x = vector3.y = 1f;
                    break;

                case WidgetFlags.CustomRelativeSize:
                    tempWidgetFlags[0] = WidgetFlags.CustomRelativeSize;
                    this.GetCustomVector2s(0, 1, tempWidgetFlags, tempVector2s);
                    vector3 = tempVector2s[0];
                    vector2 = DefaultPivot(this.mPivot);
                    break;

                case (WidgetFlags.CustomPivotOffset | WidgetFlags.CustomRelativeSize):
                    tempWidgetFlags[0] = WidgetFlags.CustomPivotOffset;
                    tempWidgetFlags[1] = WidgetFlags.CustomRelativeSize;
                    this.GetCustomVector2s(0, 2, tempWidgetFlags, tempVector2s);
                    vector2 = tempVector2s[0];
                    vector3 = tempVector2s[1];
                    break;

                default:
                    vector2 = DefaultPivot(this.mPivot);
                    vector3.x = vector3.y = 1f;
                    break;
            }
            vector.x = vector2.x * vector3.x;
            vector.y = vector2.y * vector3.y;
            vector.z = 0f;
            Matrix4x4 matrixx = worldToPanel * this.cachedTransform.localToWorldMatrix;
            mGeom.Apply(ref vector, ref matrixx);
        }
        return true;
    }

    public void WriteToBuffers(MeshBuffer m)
    {
        this.mGeom.WriteToBuffers(m);
    }

    public float alpha
    {
        get
        {
            return this.mColor.a;
        }
        set
        {
            Color mColor = this.mColor;
            mColor.a = value;
            this.color = mColor;
        }
    }

    public bool alphaUnchecked
    {
        get
        {
            return this.mAlphaUnchecked;
        }
        set
        {
            if (value)
            {
                if (!this.mAlphaUnchecked)
                {
                    this.mAlphaUnchecked = true;
                    if (NGUITools.ZeroAlpha(this.mColor.a))
                    {
                        this.mChangedCall = true;
                    }
                }
            }
            else if (this.mAlphaUnchecked)
            {
                this.mAlphaUnchecked = false;
                if (NGUITools.ZeroAlpha(this.mColor.a))
                {
                    this.mChangedCall = true;
                }
            }
        }
    }

    protected UIMaterial baseMaterial
    {
        get
        {
            return (UIMaterial) this.mMat;
        }
        set
        {
            UIMaterial mMat = (UIMaterial) this.mMat;
            if (mMat != value)
            {
                if ((mMat != null) && (this.mPanel != null))
                {
                    this.mPanel.RemoveWidget(this);
                }
                this.mPanel = null;
                this.mMat = (Material) value;
                this.mTex = null;
                if (this.mMat != null)
                {
                    this.CreatePanel();
                }
            }
        }
    }

    public Transform cachedTransform
    {
        get
        {
            if (!this.gotCachedTransform)
            {
                this.mTrans = base.transform;
                this.gotCachedTransform = true;
            }
            return this.mTrans;
        }
    }

    public bool changesQueued
    {
        get
        {
            return (this.mChangedCall || this.mForcedChanged);
        }
    }

    public Color color
    {
        get
        {
            return this.mColor;
        }
        set
        {
            if (this.mColor != value)
            {
                this.mColor = value;
                this.mChangedCall = true;
            }
        }
    }

    protected virtual UIMaterial customMaterial
    {
        get
        {
            throw new NotSupportedException();
        }
        set
        {
            throw new NotSupportedException();
        }
    }

    public int depth
    {
        get
        {
            return this.mDepth;
        }
        set
        {
            if (this.mDepth != value)
            {
                this.mDepth = value;
                if (this.mPanel != null)
                {
                    this.mPanel.MarkMaterialAsChanged(this.material, true);
                }
            }
        }
    }

    public bool keepMaterial
    {
        get
        {
            return (((byte) (this.widgetFlags & WidgetFlags.KeepsMaterial)) == 0x40);
        }
    }

    public Texture mainTexture
    {
        get
        {
            if (this.mTex == null)
            {
                UIMaterial material = this.material;
                if (material != null)
                {
                    this.mTex = material.mainTexture;
                }
            }
            return this.mTex;
        }
    }

    public UIMaterial material
    {
        get
        {
            if (((byte) (this.widgetFlags & WidgetFlags.CustomMaterialGet)) == 4)
            {
                return this.customMaterial;
            }
            return this.baseMaterial;
        }
        set
        {
            if (((byte) (this.widgetFlags & WidgetFlags.CustomMaterialSet)) == 8)
            {
                this.customMaterial = value;
            }
            else
            {
                this.baseMaterial = value;
            }
        }
    }

    private UIGeometry mGeom
    {
        get
        {
            if (this.__mGeom == null)
            {
            }
            return (this.__mGeom = new UIGeometry());
        }
    }

    public UIPanel panel
    {
        get
        {
            if (this.mPanel == null)
            {
                this.CreatePanel();
            }
            return this.mPanel;
        }
        set
        {
            this.mPanel = value;
        }
    }

    public Pivot pivot
    {
        get
        {
            return this.mPivot;
        }
        set
        {
            if (this.mPivot != value)
            {
                this.mPivot = value;
                this.mChangedCall = true;
            }
        }
    }

    public Vector2 pivotOffset
    {
        get
        {
            if (((byte) (this.widgetFlags & WidgetFlags.CustomPivotOffset)) == 1)
            {
                tempWidgetFlags[0] = WidgetFlags.CustomPivotOffset;
                this.GetCustomVector2s(0, 1, tempWidgetFlags, tempVector2s);
                return tempVector2s[0];
            }
            return DefaultPivot(this.mPivot);
        }
    }

    public Vector2 relativeSize
    {
        get
        {
            if (((byte) (this.widgetFlags & WidgetFlags.CustomRelativeSize)) == 2)
            {
                tempWidgetFlags[0] = WidgetFlags.CustomRelativeSize;
                this.GetCustomVector2s(0, 1, tempWidgetFlags, tempVector2s);
                return tempVector2s[0];
            }
            return Vector2.one;
        }
    }

    public int visibleFlag
    {
        get
        {
            return this.mVisibleFlag;
        }
        set
        {
            this.mVisibleFlag = value;
        }
    }

    [Obsolete("Use 'relativeSize' instead")]
    public Vector2 visibleSize
    {
        get
        {
            return this.relativeSize;
        }
    }

    private static class Global
    {
        public static void RegisterWidget(UIWidget widget)
        {
            if (!noGlobal)
            {
                UIGlobal.EnsureGlobal();
                if (widget.globalIndex == -1)
                {
                    widget.globalIndex = g.allWidgets.Count;
                    g.allWidgets.Add(widget);
                }
            }
        }

        public static void UnregisterWidget(UIWidget widget)
        {
            if (!noGlobal && (widget.globalIndex != -1))
            {
                g.allWidgets.RemoveAt(widget.globalIndex);
                int globalIndex = widget.globalIndex;
                int count = g.allWidgets.Count;
                while (globalIndex < count)
                {
                    g.allWidgets[globalIndex].globalIndex = globalIndex;
                    globalIndex++;
                }
                widget.globalIndex = -1;
            }
        }

        public static void WidgetDisabled(UIWidget widget)
        {
            if (!noGlobal)
            {
                g.enabledWidgets.Remove(widget);
            }
        }

        public static void WidgetEnabled(UIWidget widget)
        {
            if (!noGlobal)
            {
                g.enabledWidgets.Add(widget);
            }
        }

        public static void WidgetUpdate()
        {
            if (!noGlobal)
            {
                try
                {
                    g.enableWidgetsSwap.AddRange(g.enabledWidgets);
                    foreach (UIWidget widget in g.enableWidgetsSwap)
                    {
                        if ((widget != null) && widget.enabled)
                        {
                            widget.DefaultUpdate();
                        }
                    }
                }
                finally
                {
                    g.enableWidgetsSwap.Clear();
                }
            }
        }

        private static bool noGlobal
        {
            get
            {
                return !Application.isPlaying;
            }
        }

        public static class g
        {
            public static List<UIWidget> allWidgets = new List<UIWidget>();
            public static HashSet<UIWidget> enabledWidgets = new HashSet<UIWidget>();
            public static List<UIWidget> enableWidgetsSwap = new List<UIWidget>();

            static g()
            {
                UIGlobal.EnsureGlobal();
            }
        }
    }

    public enum Pivot
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Center,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    [Flags]
    protected enum WidgetFlags : byte
    {
        CustomBorder = 0x10,
        CustomMaterialGet = 4,
        CustomMaterialSet = 8,
        CustomPivotOffset = 1,
        CustomRelativeSize = 2,
        KeepsMaterial = 0x40,
        Reserved = 0x80
    }
}

