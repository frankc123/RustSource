using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[Serializable, ExecuteInEditMode, RequireComponent(typeof(BoxCollider))]
public abstract class dfControl : MonoBehaviour, IComparable<dfControl>
{
    [CompilerGenerated]
    private static Comparison<dfControl> <>f__am$cache4A;
    [CompilerGenerated]
    private static Func<FieldInfo, bool> <>f__am$cache4B;
    protected int cachedChildCount;
    private Plane[] cachedClippingPlanes = new Plane[4];
    private Vector3[] cachedCorners = new Vector3[4];
    protected float cachedPixelSize;
    protected Vector3 cachedPosition = ((Vector3) (Vector3.one * -3.402823E+38f));
    protected Quaternion cachedRotation = Quaternion.identity;
    protected Vector3 cachedScale = Vector3.one;
    [SerializeField]
    protected bool canFocus;
    [SerializeField]
    protected bool clipChildren;
    [SerializeField]
    protected Color32 color = new Color32(0xff, 0xff, 0xff, 0xff);
    protected dfList<dfControl> controls = dfList<dfControl>.Obtain();
    [SerializeField]
    protected Color32 disabledColor = new Color32(0xff, 0xff, 0xff, 0xff);
    [SerializeField]
    protected Vector2 hotZoneScale = Vector2.one;
    protected bool isControlInvalidated = true;
    protected bool isDisposing;
    [SerializeField]
    protected bool isEnabled = true;
    [SerializeField]
    protected bool isInteractive = true;
    [SerializeField]
    protected bool isLocalized;
    protected bool isMouseHovering;
    [SerializeField]
    protected bool isVisible = true;
    protected dfLanguageManager languageManager;
    protected bool languageManagerChecked;
    [SerializeField]
    protected AnchorLayout layout;
    protected dfGUIManager manager;
    [SerializeField]
    protected Vector2 maxSize = Vector2.zero;
    private const float MINIMUM_OPACITY = 0.0125f;
    [SerializeField]
    protected Vector2 minSize = Vector2.zero;
    protected dfControl parent;
    private bool performingLayout;
    [SerializeField]
    protected dfPivotPoint pivot;
    protected dfRenderData renderData;
    private bool rendering;
    [SerializeField]
    protected int renderOrder = -1;
    [SerializeField]
    protected Vector2 size = Vector2.zero;
    [SerializeField]
    protected int tabIndex = -1;
    private object tag;
    [SerializeField]
    protected string tooltip;
    private uint version;
    private static uint versionCounter;
    [SerializeField]
    protected int zindex = -1;

    [HideInInspector]
    public event PropertyChangedEventHandler<dfAnchorStyle> AnchorChanged;

    public event MouseEventHandler Click;

    [HideInInspector]
    public event PropertyChangedEventHandler<Color32> ColorChanged;

    [HideInInspector]
    public event ChildControlEventHandler ControlAdded;

    [HideInInspector]
    public event ChildControlEventHandler ControlRemoved;

    public event MouseEventHandler DoubleClick;

    public event DragEventHandler DragDrop;

    public event DragEventHandler DragEnd;

    public event DragEventHandler DragEnter;

    public event DragEventHandler DragLeave;

    public event DragEventHandler DragOver;

    public event DragEventHandler DragStart;

    public event FocusEventHandler EnterFocus;

    public event FocusEventHandler GotFocus;

    public event PropertyChangedEventHandler<bool> IsEnabledChanged;

    public event PropertyChangedEventHandler<bool> IsVisibleChanged;

    public event KeyPressHandler KeyDown;

    public event KeyPressHandler KeyPress;

    public event KeyPressHandler KeyUp;

    public event FocusEventHandler LeaveFocus;

    public event FocusEventHandler LostFocus;

    public event MouseEventHandler MouseDown;

    public event MouseEventHandler MouseEnter;

    public event MouseEventHandler MouseHover;

    public event MouseEventHandler MouseLeave;

    public event MouseEventHandler MouseMove;

    public event MouseEventHandler MouseUp;

    public event MouseEventHandler MouseWheel;

    public event ControlMultiTouchEventHandler MultiTouch;

    [HideInInspector]
    public event PropertyChangedEventHandler<float> OpacityChanged;

    [HideInInspector]
    public event PropertyChangedEventHandler<dfPivotPoint> PivotChanged;

    public event PropertyChangedEventHandler<Vector2> PositionChanged;

    public event PropertyChangedEventHandler<Vector2> SizeChanged;

    public event PropertyChangedEventHandler<int> TabIndexChanged;

    [HideInInspector]
    public event PropertyChangedEventHandler<int> ZOrderChanged;

    protected dfControl()
    {
    }

    public T AddControl<T>() where T: dfControl
    {
        return (T) this.AddControl(typeof(T));
    }

    public void AddControl(dfControl child)
    {
        if (child.transform == null)
        {
            throw new NullReferenceException("The child control does not have a Transform");
        }
        if (!this.controls.Contains(child))
        {
            this.controls.Add(child);
            child.parent = this;
            child.transform.parent = base.transform;
        }
        if (child.zindex == -1)
        {
            child.zindex = this.getMaxZOrder() + 1;
        }
        this.controls.Sort();
        this.OnControlAdded(child);
        child.Invalidate();
        this.Invalidate();
    }

    public dfControl AddControl(Type ControlType)
    {
        if (!typeof(dfControl).IsAssignableFrom(ControlType))
        {
            throw new InvalidCastException();
        }
        GameObject obj2 = new GameObject(ControlType.Name) {
            transform = { parent = base.transform },
            layer = base.gameObject.layer
        };
        Vector2 vector = (Vector2) ((this.Size * this.PixelsToUnits()) * 0.5f);
        obj2.transform.localPosition = new Vector3(vector.x, vector.y, 0f);
        dfControl child = obj2.AddComponent(ControlType) as dfControl;
        child.parent = this;
        child.zindex = -1;
        this.AddControl(child);
        return child;
    }

    protected internal Color32 ApplyOpacity(Color32 color)
    {
        float num = this.CalculateOpacity();
        color.a = (byte) (num * 255f);
        return color;
    }

    [HideInInspector]
    public virtual void Awake()
    {
        if (base.transform.parent != null)
        {
            dfControl component = base.transform.parent.GetComponent<dfControl>();
            if (component != null)
            {
                this.parent = component;
                component.AddControl(this);
            }
            if (this.controls == null)
            {
                this.updateControlHierarchy(false);
            }
            if (!Application.isPlaying)
            {
                this.PerformLayout();
            }
        }
    }

    public virtual void BringToFront()
    {
        if (this.parent == null)
        {
            this.GetManager().BringToFront(this);
        }
        else
        {
            this.parent.SetControlIndex(this, this.parent.controls.Count - 1);
        }
        this.Invalidate();
    }

    public virtual Vector2 CalculateMinimumSize()
    {
        return this.MinimumSize;
    }

    protected internal float CalculateOpacity()
    {
        if (this.parent == null)
        {
            return this.Opacity;
        }
        return (this.Opacity * this.parent.CalculateOpacity());
    }

    private static Vector3 closestPointOnLine(Vector3 start, Vector3 end, Vector3 test, bool clamp)
    {
        Vector3 rhs = test - start;
        Vector3 vector3 = end - start;
        Vector3 normalized = vector3.normalized;
        Vector3 vector4 = end - start;
        float magnitude = vector4.magnitude;
        float num2 = Vector3.Dot(normalized, rhs);
        if (clamp)
        {
            if (num2 < 0f)
            {
                return start;
            }
            if (num2 > magnitude)
            {
                return end;
            }
        }
        normalized = (Vector3) (normalized * num2);
        return (start + normalized);
    }

    public int CompareTo(dfControl other)
    {
        if (this.ZOrder >= 0)
        {
            return this.ZOrder.CompareTo(other.ZOrder);
        }
        if (other.ZOrder < 0)
        {
            return 0;
        }
        return 1;
    }

    public bool Contains(dfControl child)
    {
        return ((child != null) && child.transform.IsChildOf(base.transform));
    }

    public void Disable()
    {
        this.IsEnabled = false;
    }

    private static float distanceFromLine(Vector3 start, Vector3 end, Vector3 test)
    {
        Vector3 rhs = start - end;
        Vector3 lhs = test - end;
        float num = Vector3.Dot(lhs, rhs);
        if (num <= 0f)
        {
            return Vector3.Distance(test, end);
        }
        float num2 = Vector3.Dot(rhs, rhs);
        if (num2 <= num)
        {
            return Vector3.Distance(test, start);
        }
        float num3 = num / num2;
        Vector3 b = end + ((Vector3) (num3 * rhs));
        return Vector3.Distance(test, b);
    }

    public void DoClick()
    {
        Camera camera = this.GetCamera();
        Vector3 position = camera.WorldToScreenPoint(this.GetCenter());
        Ray ray = camera.ScreenPointToRay(position);
        this.OnClick(new dfMouseEventArgs(this, dfMouseButtons.Left, 1, ray, position, 0f));
    }

    public void Enable()
    {
        this.IsEnabled = true;
    }

    private void ensureLayoutExists()
    {
        if (this.layout == null)
        {
            this.layout = new AnchorLayout(dfAnchorStyle.Left | dfAnchorStyle.Top, this);
        }
        else
        {
            this.layout.Attach(this);
        }
        for (int i = 0; (this.Controls != null) && (i < this.Controls.Count); i++)
        {
            if (this.controls[i] != null)
            {
                this.controls[i].ensureLayoutExists();
            }
        }
    }

    public dfControl Find(string Name)
    {
        if (base.name == Name)
        {
            return this;
        }
        this.updateControlHierarchy(true);
        for (int i = 0; i < this.controls.Count; i++)
        {
            dfControl control = this.controls[i];
            if (control.name == Name)
            {
                return control;
            }
        }
        for (int j = 0; j < this.controls.Count; j++)
        {
            dfControl control2 = this.controls[j].Find(Name);
            if (control2 != null)
            {
                return control2;
            }
        }
        return null;
    }

    public T Find<T>(string Name) where T: dfControl
    {
        if ((base.name == Name) && (this is T))
        {
            return (T) this;
        }
        this.updateControlHierarchy(true);
        for (int i = 0; i < this.controls.Count; i++)
        {
            T local = this.controls[i] as T;
            if ((local != null) && (local.name == Name))
            {
                return local;
            }
        }
        for (int j = 0; j < this.controls.Count; j++)
        {
            T local2 = this.controls[j].Find<T>(Name);
            if (local2 != null)
            {
                return local2;
            }
        }
        return null;
    }

    public void Focus()
    {
        if ((this.CanFocus && !this.HasFocus) && (this.IsEnabled && this.IsVisible))
        {
            dfGUIManager.SetFocus(this);
            this.Invalidate();
        }
    }

    public Bounds GetBounds()
    {
        Vector3[] corners = this.GetCorners();
        Vector3 center = corners[0] + ((Vector3) ((corners[3] - corners[0]) * 0.5f));
        Vector3 lhs = center;
        Vector3 vector3 = center;
        for (int i = 0; i < corners.Length; i++)
        {
            lhs = Vector3.Min(lhs, corners[i]);
            vector3 = Vector3.Max(vector3, corners[i]);
        }
        return new Bounds(center, vector3 - lhs);
    }

    public Camera GetCamera()
    {
        dfGUIManager manager = this.GetManager();
        if (manager == null)
        {
            Debug.LogError("The Manager hosting this control could not be determined");
            return null;
        }
        return manager.RenderCamera;
    }

    public Vector3 GetCenter()
    {
        return (base.transform.position + ((Vector3) (this.Pivot.TransformToCenter(this.Size) * this.PixelsToUnits())));
    }

    private dfList<dfControl> getChildControls()
    {
        int childCount = base.transform.childCount;
        dfList<dfControl> list = dfList<dfControl>.Obtain();
        list.EnsureCapacity(childCount);
        for (int i = 0; i < childCount; i++)
        {
            Transform child = base.transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                dfControl component = child.GetComponent<dfControl>();
                if (component != null)
                {
                    list.Add(component);
                }
            }
        }
        return list;
    }

    protected internal virtual Plane[] GetClippingPlanes()
    {
        Vector3[] corners = this.GetCorners();
        Vector3 inNormal = base.transform.TransformDirection(Vector3.right);
        Vector3 vector2 = base.transform.TransformDirection(Vector3.left);
        Vector3 vector3 = base.transform.TransformDirection(Vector3.up);
        Vector3 vector4 = base.transform.TransformDirection(Vector3.down);
        this.cachedClippingPlanes[0] = new Plane(inNormal, corners[0]);
        this.cachedClippingPlanes[1] = new Plane(vector2, corners[1]);
        this.cachedClippingPlanes[2] = new Plane(vector3, corners[2]);
        this.cachedClippingPlanes[3] = new Plane(vector4, corners[0]);
        return this.cachedClippingPlanes;
    }

    public Vector3[] GetCorners()
    {
        float num = this.PixelsToUnits();
        Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
        Vector3 vector = this.pivot.TransformToUpperLeft(this.size);
        Vector3 vector2 = vector + new Vector3(this.size.x, 0f);
        Vector3 vector3 = vector + new Vector3(0f, -this.size.y);
        Vector3 vector4 = vector2 + new Vector3(0f, -this.size.y);
        this.cachedCorners[0] = localToWorldMatrix.MultiplyPoint((Vector3) (vector * num));
        this.cachedCorners[1] = localToWorldMatrix.MultiplyPoint((Vector3) (vector2 * num));
        this.cachedCorners[2] = localToWorldMatrix.MultiplyPoint((Vector3) (vector3 * num));
        this.cachedCorners[3] = localToWorldMatrix.MultiplyPoint((Vector3) (vector4 * num));
        return this.cachedCorners;
    }

    protected internal Vector2 GetHitPosition(dfMouseEventArgs args)
    {
        Vector2 vector;
        this.GetHitPosition(args.Ray, out vector);
        return vector;
    }

    public bool GetHitPosition(Ray ray, out Vector2 position)
    {
        position = (Vector2) (Vector2.one * -3.402823E+38f);
        Plane plane = new Plane(base.transform.TransformDirection(Vector3.back), base.transform.position);
        float enter = 0f;
        if (!plane.Raycast(ray, out enter))
        {
            return false;
        }
        Vector3 inPt = ray.origin + ((Vector3) (ray.direction * enter));
        Plane[] planeArray = !this.ClipChildren ? null : this.GetClippingPlanes();
        if ((planeArray != null) && (planeArray.Length > 0))
        {
            for (int i = 0; i < planeArray.Length; i++)
            {
                if (!planeArray[i].GetSide(inPt))
                {
                    return false;
                }
            }
        }
        Vector3[] corners = this.GetCorners();
        Vector3 start = corners[0];
        Vector3 end = corners[1];
        Vector3 vector4 = corners[2];
        Vector3 vector6 = closestPointOnLine(start, end, inPt, true) - start;
        Vector3 vector7 = end - start;
        float num3 = vector6.magnitude / vector7.magnitude;
        float x = this.size.x * num3;
        Vector3 vector8 = closestPointOnLine(start, vector4, inPt, true) - start;
        Vector3 vector9 = vector4 - start;
        num3 = vector8.magnitude / vector9.magnitude;
        float y = this.size.y * num3;
        position = new Vector2(x, y);
        return true;
    }

    internal bool GetIsVisibleRaw()
    {
        return this.isVisible;
    }

    [HideInInspector]
    protected internal string getLocalizedValue(string key)
    {
        if (!this.IsLocalized || !Application.isPlaying)
        {
            return key;
        }
        if (this.languageManager == null)
        {
            if (this.languageManagerChecked)
            {
                return key;
            }
            this.languageManagerChecked = true;
            this.languageManager = this.GetManager().GetComponent<dfLanguageManager>();
            if (this.languageManager == null)
            {
                return key;
            }
        }
        return this.languageManager.GetValue(key);
    }

    public dfGUIManager GetManager()
    {
        if ((this.manager != null) || !base.gameObject.activeInHierarchy)
        {
            return this.manager;
        }
        if ((this.parent != null) && (this.parent.manager != null))
        {
            return (this.manager = this.parent.manager);
        }
        for (GameObject obj2 = base.gameObject; obj2 != null; obj2 = obj2.transform.parent.gameObject)
        {
            dfGUIManager component = obj2.GetComponent<dfGUIManager>();
            if (component != null)
            {
                return (this.manager = component);
            }
            if (obj2.transform.parent == null)
            {
                break;
            }
        }
        dfGUIManager manager2 = Object.FindObjectsOfType(typeof(dfGUIManager)).FirstOrDefault<Object>() as dfGUIManager;
        if (manager2 != null)
        {
            return (this.manager = manager2);
        }
        return null;
    }

    private int getMaxZOrder()
    {
        int b = -1;
        for (int i = 0; i < this.controls.Count; i++)
        {
            b = Mathf.Max(this.controls[i].zindex, b);
        }
        return b;
    }

    private Vector3 getRelativePosition()
    {
        if (base.transform.parent == null)
        {
            return Vector3.zero;
        }
        if (this.parent != null)
        {
            float num = this.PixelsToUnits();
            Vector3 position = base.transform.parent.position;
            Vector3 vector2 = base.transform.position;
            Transform parent = base.transform.parent;
            Vector3 vector3 = parent.InverseTransformPoint((Vector3) (position / num)) + this.parent.pivot.TransformToUpperLeft(this.parent.size);
            Vector3 vector4 = parent.InverseTransformPoint((Vector3) (vector2 / num)) + this.pivot.TransformToUpperLeft(this.size);
            Vector3 vector = vector4 - vector3;
            return vector.Scale(1f, -1f, 1f);
        }
        dfGUIManager manager = this.GetManager();
        if (manager == null)
        {
            Debug.LogError("Cannot get position: View not found");
            return Vector3.zero;
        }
        float num2 = this.PixelsToUnits();
        Vector3 inPt = base.transform.position + ((Vector3) (this.pivot.TransformToUpperLeft(this.size) * num2));
        Plane[] clippingPlanes = manager.GetClippingPlanes();
        float x = clippingPlanes[0].GetDistanceToPoint(inPt) / num2;
        float y = clippingPlanes[3].GetDistanceToPoint(inPt) / num2;
        return new Vector3(x, y).RoundToInt();
    }

    public dfControl GetRootContainer()
    {
        dfControl parent = this;
        while (parent.Parent != null)
        {
            parent = parent.Parent;
        }
        return parent;
    }

    protected internal Vector3 getScaledDirection(Vector3 direction)
    {
        Vector3 localScale = this.GetManager().transform.localScale;
        direction = base.transform.TransformDirection(direction);
        return Vector3.Scale(direction, localScale);
    }

    public Rect GetScreenRect()
    {
        Camera camera = this.GetCamera();
        Vector3[] corners = this.GetCorners();
        Vector3 vector = camera.WorldToScreenPoint(corners[0]);
        Vector3 vector2 = camera.WorldToScreenPoint(corners[3]);
        return new Rect(vector.x, Screen.height - vector.y, vector2.x - vector.x, vector.y - vector2.y);
    }

    public void Hide()
    {
        this.IsVisible = false;
    }

    private void initializeControl()
    {
        if (this.renderOrder == -1)
        {
            this.renderOrder = this.ZOrder;
        }
        if (base.transform.parent != null)
        {
            dfControl component = base.transform.parent.GetComponent<dfControl>();
            if (component != null)
            {
                component.AddControl(this);
            }
        }
        this.ensureLayoutExists();
        this.Invalidate();
        base.collider.isTrigger = false;
        if (Application.isPlaying && (base.rigidbody == null))
        {
            Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
            rigidbody.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
            rigidbody.isKinematic = true;
            rigidbody.detectCollisions = false;
        }
        this.updateCollider();
    }

    public virtual void Invalidate()
    {
        this.updateVersion();
        this.isControlInvalidated = true;
        dfGUIManager manager = this.GetManager();
        if (manager != null)
        {
            manager.Invalidate();
        }
    }

    [HideInInspector]
    public virtual void LateUpdate()
    {
        if ((this.layout != null) && this.layout.HasPendingLayoutRequest)
        {
            this.layout.PerformLayout();
        }
    }

    public void Localize()
    {
        if (this.IsLocalized)
        {
            if (this.languageManager == null)
            {
                this.languageManager = this.GetManager().GetComponent<dfLanguageManager>();
                if (this.languageManager == null)
                {
                    return;
                }
            }
            this.OnLocalize();
        }
    }

    [HideInInspector]
    public void MakePixelPerfect(bool recursive = true)
    {
        this.size = this.size.RoundToInt();
        float num = this.PixelsToUnits();
        base.transform.position = (Vector3) (((Vector3) (base.transform.position / num)).RoundToInt() * num);
        this.cachedPosition = base.transform.localPosition;
        for (int i = 0; (i < this.controls.Count) && recursive; i++)
        {
            this.controls[i].MakePixelPerfect(true);
        }
        this.Invalidate();
    }

    [HideInInspector]
    protected internal virtual void OnAnchorChanged()
    {
        dfAnchorStyle anchorStyle = this.layout.AnchorStyle;
        this.Invalidate();
        this.ResetLayout(false, false);
        if (anchorStyle.IsAnyFlagSet(dfAnchorStyle.CenterVertical | dfAnchorStyle.CenterHorizontal))
        {
            this.PerformLayout();
        }
        if (this.AnchorChanged != null)
        {
            this.AnchorChanged(this, anchorStyle);
        }
    }

    [HideInInspector]
    public virtual void OnApplicationQuit()
    {
        this.RemoveAllEventHandlers();
    }

    protected internal virtual void OnClick(dfMouseEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnClick", objArray1);
            if (this.Click != null)
            {
                this.Click(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnClick(args);
        }
    }

    [HideInInspector]
    protected internal virtual void OnColorChanged()
    {
        this.Invalidate();
        if (this.ColorChanged != null)
        {
            this.ColorChanged(this, this.Color);
        }
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].OnColorChanged();
        }
    }

    [HideInInspector]
    protected internal virtual void OnControlAdded(dfControl child)
    {
        this.Invalidate();
        if (this.ControlAdded != null)
        {
            this.ControlAdded(this, child);
        }
        object[] args = new object[] { this, child };
        this.Signal("OnControlAdded", args);
    }

    [HideInInspector]
    protected internal virtual void OnControlRemoved(dfControl child)
    {
        this.Invalidate();
        if (this.ControlRemoved != null)
        {
            this.ControlRemoved(this, child);
        }
        object[] args = new object[] { this, child };
        this.Signal("OnControlRemoved", args);
    }

    [HideInInspector]
    public virtual void OnDestroy()
    {
        this.isDisposing = true;
        if (Application.isPlaying)
        {
            this.RemoveAllEventHandlers();
        }
        if (this.layout != null)
        {
            this.layout.Dispose();
        }
        if (((this.parent != null) && (this.parent.controls != null)) && (!this.parent.isDisposing && this.parent.controls.Remove(this)))
        {
            this.parent.cachedChildCount--;
            this.parent.OnControlRemoved(this);
        }
        for (int i = 0; i < this.controls.Count; i++)
        {
            if (this.controls[i].layout != null)
            {
                this.controls[i].layout.Dispose();
                this.controls[i].layout = null;
            }
            this.controls[i].parent = null;
        }
        this.controls.Release();
        if (this.manager != null)
        {
            this.manager.Invalidate();
        }
        if (this.renderData != null)
        {
            this.renderData.Release();
        }
        this.layout = null;
        this.manager = null;
        this.parent = null;
        this.cachedClippingPlanes = null;
        this.cachedCorners = null;
        this.renderData = null;
        this.controls = null;
    }

    [HideInInspector]
    public virtual void OnDisable()
    {
        try
        {
            this.Invalidate();
            if (this.renderData != null)
            {
                this.renderData.Release();
                this.renderData = null;
            }
            if (dfGUIManager.HasFocus(this))
            {
                dfGUIManager.SetFocus(null);
            }
            this.OnIsEnabledChanged();
        }
        catch
        {
        }
    }

    protected internal virtual void OnDoubleClick(dfMouseEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnDoubleClick", objArray1);
            if (this.DoubleClick != null)
            {
                this.DoubleClick(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnDoubleClick(args);
        }
    }

    internal virtual void OnDragDrop(dfDragEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnDragDrop", objArray1);
            if (!args.Used && (this.DragDrop != null))
            {
                this.DragDrop(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnDragDrop(args);
        }
    }

    internal virtual void OnDragEnd(dfDragEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnDragEnd", objArray1);
            if (!args.Used && (this.DragEnd != null))
            {
                this.DragEnd(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnDragEnd(args);
        }
    }

    internal virtual void OnDragEnter(dfDragEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnDragEnter", objArray1);
            if (!args.Used && (this.DragEnter != null))
            {
                this.DragEnter(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnDragEnter(args);
        }
    }

    internal virtual void OnDragLeave(dfDragEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnDragLeave", objArray1);
            if (!args.Used && (this.DragLeave != null))
            {
                this.DragLeave(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnDragLeave(args);
        }
    }

    internal virtual void OnDragOver(dfDragEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnDragOver", objArray1);
            if (!args.Used && (this.DragOver != null))
            {
                this.DragOver(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnDragOver(args);
        }
    }

    internal virtual void OnDragStart(dfDragEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnDragStart", objArray1);
            if (!args.Used && (this.DragStart != null))
            {
                this.DragStart(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnDragStart(args);
        }
    }

    [HideInInspector]
    public virtual void OnEnable()
    {
        if (Application.isPlaying)
        {
            base.collider.enabled = this.IsInteractive;
        }
        this.initializeControl();
        if ((this.controls == null) || (this.controls.Count == 0))
        {
            this.updateControlHierarchy(false);
        }
        if (Application.isPlaying && this.IsLocalized)
        {
            this.Localize();
        }
        this.OnIsEnabledChanged();
    }

    protected internal virtual void OnEnterFocus(dfFocusEventArgs args)
    {
        object[] objArray1 = new object[] { args };
        this.Signal("OnEnterFocus", objArray1);
        if (this.EnterFocus != null)
        {
            this.EnterFocus(this, args);
        }
    }

    protected internal virtual void OnGotFocus(dfFocusEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnGotFocus", objArray1);
            if (this.GotFocus != null)
            {
                this.GotFocus(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnGotFocus(args);
        }
    }

    [HideInInspector]
    protected internal virtual void OnIsEnabledChanged()
    {
        if (dfGUIManager.ContainsFocus(this) && !this.IsEnabled)
        {
            dfGUIManager.SetFocus(null);
        }
        this.Invalidate();
        object[] args = new object[] { this, this.IsEnabled };
        this.Signal("OnIsEnabledChanged", args);
        if (this.IsEnabledChanged != null)
        {
            this.IsEnabledChanged(this, this.isEnabled);
        }
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].OnIsEnabledChanged();
        }
    }

    [HideInInspector]
    protected internal virtual void OnIsVisibleChanged()
    {
        if (this.HasFocus && !this.IsVisible)
        {
            dfGUIManager.SetFocus(null);
        }
        this.Invalidate();
        object[] args = new object[] { this, this.IsVisible };
        this.Signal("OnIsVisibleChanged", args);
        if (this.IsVisibleChanged != null)
        {
            this.IsVisibleChanged(this, this.isVisible);
        }
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].OnIsVisibleChanged();
        }
    }

    protected internal virtual void OnKeyDown(dfKeyEventArgs args)
    {
        if (this.IsInteractive && !args.Used)
        {
            if (args.KeyCode == KeyCode.Tab)
            {
                this.OnTabKeyPressed(args);
            }
            if (!args.Used)
            {
                object[] objArray1 = new object[] { args };
                this.Signal("OnKeyDown", objArray1);
                if (this.KeyDown != null)
                {
                    this.KeyDown(this, args);
                }
            }
        }
        if (this.parent != null)
        {
            this.parent.OnKeyDown(args);
        }
    }

    protected internal virtual void OnKeyPress(dfKeyEventArgs args)
    {
        if (this.IsInteractive && !args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnKeyPress", objArray1);
            if (this.KeyPress != null)
            {
                this.KeyPress(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnKeyPress(args);
        }
    }

    protected internal virtual void OnKeyUp(dfKeyEventArgs args)
    {
        if (this.IsInteractive)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnKeyUp", objArray1);
            if (this.KeyUp != null)
            {
                this.KeyUp(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnKeyUp(args);
        }
    }

    protected internal virtual void OnLeaveFocus(dfFocusEventArgs args)
    {
        object[] objArray1 = new object[] { args };
        this.Signal("OnLeaveFocus", objArray1);
        if (this.LeaveFocus != null)
        {
            this.LeaveFocus(this, args);
        }
    }

    [HideInInspector]
    protected internal virtual void OnLocalize()
    {
    }

    protected internal virtual void OnLostFocus(dfFocusEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnLostFocus", objArray1);
            if (this.LostFocus != null)
            {
                this.LostFocus(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnLostFocus(args);
        }
    }

    protected internal virtual void OnMouseDown(dfMouseEventArgs args)
    {
        if ((((this.Opacity > 0.01f) && this.IsVisible) && (this.IsEnabled && this.CanFocus)) && !this.ContainsFocus)
        {
            this.Focus();
        }
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMouseDown", objArray1);
            if (this.MouseDown != null)
            {
                this.MouseDown(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMouseDown(args);
        }
    }

    protected internal virtual void OnMouseEnter(dfMouseEventArgs args)
    {
        this.isMouseHovering = true;
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMouseEnter", objArray1);
            if (this.MouseEnter != null)
            {
                this.MouseEnter(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMouseEnter(args);
        }
    }

    protected internal virtual void OnMouseHover(dfMouseEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMouseHover", objArray1);
            if (this.MouseHover != null)
            {
                this.MouseHover(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMouseHover(args);
        }
    }

    protected internal virtual void OnMouseLeave(dfMouseEventArgs args)
    {
        this.isMouseHovering = false;
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMouseLeave", objArray1);
            if (this.MouseLeave != null)
            {
                this.MouseLeave(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMouseLeave(args);
        }
    }

    protected internal virtual void OnMouseMove(dfMouseEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMouseMove", objArray1);
            if (this.MouseMove != null)
            {
                this.MouseMove(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMouseMove(args);
        }
    }

    protected internal virtual void OnMouseUp(dfMouseEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMouseUp", objArray1);
            if (this.MouseUp != null)
            {
                this.MouseUp(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMouseUp(args);
        }
    }

    protected internal virtual void OnMouseWheel(dfMouseEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMouseWheel", objArray1);
            if (this.MouseWheel != null)
            {
                this.MouseWheel(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMouseWheel(args);
        }
    }

    protected internal virtual void OnMultiTouch(dfTouchEventArgs args)
    {
        if (!args.Used)
        {
            object[] objArray1 = new object[] { args };
            this.Signal("OnMultiTouch", objArray1);
            if (this.MultiTouch != null)
            {
                this.MultiTouch(this, args);
            }
        }
        if (this.parent != null)
        {
            this.parent.OnMultiTouch(args);
        }
    }

    [HideInInspector]
    protected internal virtual void OnOpacityChanged()
    {
        this.Invalidate();
        if (this.OpacityChanged != null)
        {
            this.OpacityChanged(this, this.Opacity);
        }
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].OnOpacityChanged();
        }
    }

    [HideInInspector]
    protected internal virtual void OnPivotChanged()
    {
        this.Invalidate();
        if (this.Anchor.IsAnyFlagSet(dfAnchorStyle.CenterVertical | dfAnchorStyle.CenterHorizontal))
        {
            this.ResetLayout(false, false);
            this.PerformLayout();
        }
        if (this.PivotChanged != null)
        {
            this.PivotChanged(this, this.pivot);
        }
    }

    [HideInInspector]
    protected internal virtual void OnPositionChanged()
    {
        base.transform.hasChanged = false;
        if (this.renderData != null)
        {
            this.updateVersion();
            this.GetManager().Invalidate();
        }
        else
        {
            this.Invalidate();
        }
        this.ResetLayout(false, false);
        if (this.PositionChanged != null)
        {
            this.PositionChanged(this, this.Position);
        }
    }

    [HideInInspector]
    protected virtual void OnRebuildRenderData()
    {
    }

    protected internal virtual void OnResolutionChanged(Vector2 previousResolution, Vector2 currentResolution)
    {
        this.Invalidate();
        this.updateControlHierarchy(false);
        this.cachedPixelSize = 0f;
        Vector3 vector = (Vector3) (base.transform.localPosition / (2f / previousResolution.y));
        Vector3 vector2 = (Vector3) (vector * (2f / currentResolution.y));
        base.transform.localPosition = vector2;
        this.cachedPosition = vector2;
        this.layout.Attach(this);
        this.updateCollider();
        object[] args = new object[] { this, previousResolution, currentResolution };
        this.Signal("OnResolutionChanged", args);
    }

    [HideInInspector]
    protected internal virtual void OnSizeChanged()
    {
        this.updateCollider();
        this.Invalidate();
        this.ResetLayout(false, false);
        if (this.Anchor.IsAnyFlagSet(dfAnchorStyle.CenterVertical | dfAnchorStyle.CenterHorizontal))
        {
            this.PerformLayout();
        }
        if (this.SizeChanged != null)
        {
            this.SizeChanged(this, this.Size);
        }
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].PerformLayout();
        }
    }

    [HideInInspector]
    protected internal virtual void OnTabIndexChanged()
    {
        this.Invalidate();
        if (this.TabIndexChanged != null)
        {
            this.TabIndexChanged(this, this.tabIndex);
        }
    }

    protected virtual void OnTabKeyPressed(dfKeyEventArgs args)
    {
        List<dfControl> list = (from c in this.GetManager().GetComponentsInChildren<dfControl>()
            where (((c != this) && (c.TabIndex >= 0)) && (c.IsInteractive && c.CanFocus)) && c.IsVisible
            select c).ToList<dfControl>();
        if (list.Count != 0)
        {
            if (<>f__am$cache4A == null)
            {
                <>f__am$cache4A = delegate (dfControl lhs, dfControl rhs) {
                    if (lhs.TabIndex == rhs.TabIndex)
                    {
                        return lhs.RenderOrder.CompareTo(rhs.RenderOrder);
                    }
                    return lhs.TabIndex.CompareTo(rhs.TabIndex);
                };
            }
            list.Sort(<>f__am$cache4A);
            if (!args.Shift)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    dfControl control = list[i];
                    if (control.TabIndex >= this.TabIndex)
                    {
                        list[i].Focus();
                        args.Use();
                        return;
                    }
                }
                list[0].Focus();
                args.Use();
            }
            else
            {
                for (int j = list.Count - 1; j >= 0; j--)
                {
                    dfControl control2 = list[j];
                    if (control2.TabIndex <= this.TabIndex)
                    {
                        list[j].Focus();
                        args.Use();
                        return;
                    }
                }
                list[list.Count - 1].Focus();
                args.Use();
            }
        }
    }

    [HideInInspector]
    protected internal virtual void OnZOrderChanged()
    {
        this.Invalidate();
        if (this.ZOrderChanged != null)
        {
            this.ZOrderChanged(this, this.zindex);
        }
    }

    [HideInInspector]
    public virtual void PerformLayout()
    {
        if (!this.isDisposing && !this.performingLayout)
        {
            try
            {
                this.performingLayout = true;
                this.ensureLayoutExists();
                this.layout.PerformLayout();
                this.Invalidate();
            }
            finally
            {
                this.performingLayout = false;
            }
        }
    }

    protected internal float PixelsToUnits()
    {
        if (this.cachedPixelSize > float.Epsilon)
        {
            return this.cachedPixelSize;
        }
        dfGUIManager manager = this.GetManager();
        if (manager == null)
        {
            return 0.0026f;
        }
        return (this.cachedPixelSize = manager.PixelsToUnits());
    }

    [HideInInspector]
    protected internal void RaiseEvent(string eventName, params object[] args)
    {
        <RaiseEvent>c__AnonStorey51 storey = new <RaiseEvent>c__AnonStorey51 {
            eventName = eventName
        };
        FieldInfo info = Enumerable.Where<FieldInfo>(base.GetType().GetAllFields(), new Func<FieldInfo, bool>(storey.<>m__1A)).FirstOrDefault<FieldInfo>();
        if (info != null)
        {
            object obj2 = info.GetValue(this);
            if (obj2 != null)
            {
                ((Delegate) obj2).DynamicInvoke(args);
            }
        }
    }

    [HideInInspector]
    public void RebuildControlOrder()
    {
        bool flag = false;
        this.controls.Sort();
        for (int i = 0; i < this.controls.Count; i++)
        {
            if (this.controls[i].ZOrder != i)
            {
                flag = true;
                break;
            }
        }
        if (flag)
        {
            this.controls.Sort();
            for (int j = 0; j < this.controls.Count; j++)
            {
                this.controls[j].zindex = j;
            }
        }
    }

    [HideInInspector]
    internal void RemoveAllEventHandlers()
    {
        if (<>f__am$cache4B == null)
        {
            <>f__am$cache4B = f => typeof(Delegate).IsAssignableFrom(f.FieldType);
        }
        FieldInfo[] infoArray = Enumerable.Where<FieldInfo>(base.GetType().GetAllFields(), <>f__am$cache4B).ToArray<FieldInfo>();
        for (int i = 0; i < infoArray.Length; i++)
        {
            infoArray[i].SetValue(this, null);
        }
    }

    public void RemoveControl(dfControl child)
    {
        if (!this.isDisposing)
        {
            if (child.Parent == this)
            {
                child.parent = null;
            }
            if (this.controls.Remove(child))
            {
                this.OnControlRemoved(child);
                child.Invalidate();
                this.Invalidate();
            }
        }
    }

    [HideInInspector]
    protected internal void RemoveEventHandlers(string EventName)
    {
        <RemoveEventHandlers>c__AnonStorey52 storey = new <RemoveEventHandlers>c__AnonStorey52 {
            EventName = EventName
        };
        FieldInfo info = Enumerable.Where<FieldInfo>(base.GetType().GetAllFields(), new Func<FieldInfo, bool>(storey.<>m__1B)).FirstOrDefault<FieldInfo>();
        if (info != null)
        {
            info.SetValue(this, null);
        }
    }

    internal dfRenderData Render()
    {
        dfRenderData renderData;
        if (this.rendering)
        {
            return this.renderData;
        }
        try
        {
            this.rendering = true;
            bool isVisible = this.isVisible;
            bool flag2 = base.enabled && base.gameObject.activeSelf;
            if (!isVisible || !flag2)
            {
                return null;
            }
            if (this.renderData == null)
            {
                this.renderData = dfRenderData.Obtain();
                this.isControlInvalidated = true;
            }
            if (this.isControlInvalidated)
            {
                this.renderData.Clear();
                this.OnRebuildRenderData();
                this.updateCollider();
            }
            this.renderData.Transform = base.transform.localToWorldMatrix;
            renderData = this.renderData;
        }
        finally
        {
            this.rendering = false;
            this.isControlInvalidated = false;
        }
        return renderData;
    }

    [HideInInspector]
    public virtual void ResetLayout(bool recursive = false, bool force = false)
    {
        bool flag = this.IsPerformingLayout || this.IsLayoutSuspended;
        if (force || !flag)
        {
            this.ensureLayoutExists();
            this.layout.Attach(this);
            this.layout.Reset(force);
            if (recursive)
            {
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    this.controls[i].ResetLayout(false, false);
                }
            }
        }
    }

    [HideInInspector]
    public virtual void ResumeLayout()
    {
        this.ensureLayoutExists();
        this.layout.ResumeLayout();
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].ResumeLayout();
        }
    }

    public virtual void SendToBack()
    {
        if (this.parent == null)
        {
            this.GetManager().SendToBack(this);
        }
        else
        {
            this.parent.SetControlIndex(this, 0);
        }
        this.Invalidate();
    }

    protected internal void SetControlIndex(dfControl child, int zindex)
    {
        <SetControlIndex>c__AnonStorey53 storey = new <SetControlIndex>c__AnonStorey53 {
            zindex = zindex,
            child = child
        };
        dfControl control = this.controls.FirstOrDefault(new Func<dfControl, bool>(storey.<>m__1D));
        if (control != null)
        {
            control.zindex = this.controls.IndexOf(storey.child);
        }
        storey.child.zindex = storey.zindex;
        this.RebuildControlOrder();
    }

    private void setPositionInternal(Vector3 value)
    {
        value += this.pivot.UpperLeftToTransform(this.Size);
        value = (Vector3) (value * this.PixelsToUnits());
        Vector3 vector = value - this.cachedPosition;
        if (vector.sqrMagnitude > float.Epsilon)
        {
            Vector3 vector2 = value;
            base.transform.localPosition = vector2;
            this.cachedPosition = vector2;
            this.OnPositionChanged();
        }
    }

    private void setRelativePosition(Vector3 value)
    {
        if (base.transform.parent == null)
        {
            Debug.LogError("Cannot set relative position without a parent Transform.");
        }
        else
        {
            Vector3 vector5 = value - this.getRelativePosition();
            if (vector5.sqrMagnitude > float.Epsilon)
            {
                if (this.parent != null)
                {
                    Vector3 vector = (value.Scale(1f, -1f, 1f) + this.pivot.UpperLeftToTransform(this.size)) - this.parent.pivot.UpperLeftToTransform(this.parent.size);
                    vector = (Vector3) (vector * this.PixelsToUnits());
                    Vector3 vector6 = vector - base.transform.localPosition;
                    if (vector6.sqrMagnitude >= float.Epsilon)
                    {
                        base.transform.localPosition = vector;
                        this.cachedPosition = vector;
                        this.OnPositionChanged();
                    }
                }
                else
                {
                    dfGUIManager manager = this.GetManager();
                    if (manager == null)
                    {
                        Debug.LogError("Cannot get position: View not found");
                    }
                    else
                    {
                        Vector3 vector2 = manager.GetCorners()[0];
                        float num = this.PixelsToUnits();
                        value = (Vector3) (value.Scale(1f, -1f, 1f) * num);
                        Vector3 vector3 = (Vector3) (this.pivot.UpperLeftToTransform(this.Size) * num);
                        Vector3 vector4 = (vector2 + manager.transform.TransformDirection(value)) + vector3;
                        Vector3 vector7 = vector4 - this.cachedPosition;
                        if (vector7.sqrMagnitude > float.Epsilon)
                        {
                            base.transform.position = vector4;
                            this.cachedPosition = base.transform.localPosition;
                            this.OnPositionChanged();
                        }
                    }
                }
            }
        }
    }

    internal void setRenderOrder(ref int order)
    {
        this.renderOrder = ++order;
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].setRenderOrder(ref order);
        }
    }

    public void Show()
    {
        this.IsVisible = true;
    }

    protected internal bool Signal(string eventName, params object[] args)
    {
        return this.Signal(base.gameObject, eventName, args);
    }

    [HideInInspector]
    protected internal bool Signal(GameObject target, string eventName, params object[] args)
    {
        Component[] components = target.GetComponents(typeof(MonoBehaviour));
        if ((components == null) || ((target == base.gameObject) && (components.Length == 1)))
        {
            return false;
        }
        if ((args.Length == 0) || !object.ReferenceEquals(args[0], this))
        {
            object[] destinationArray = new object[args.Length + 1];
            Array.Copy(args, 0, destinationArray, 1, args.Length);
            destinationArray[0] = this;
            args = destinationArray;
        }
        Type[] types = new Type[args.Length];
        for (int i = 0; i < types.Length; i++)
        {
            if (args[i] == null)
            {
                types[i] = typeof(object);
            }
            else
            {
                types[i] = args[i].GetType();
            }
        }
        bool flag = false;
        for (int j = 0; j < components.Length; j++)
        {
            Component component = components[j];
            if (((component != null) && (component.GetType() != null)) && ((!(component is MonoBehaviour) || ((MonoBehaviour) component).enabled) && (component != this)))
            {
                MethodInfo info = component.GetType().GetMethod(eventName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, types, null);
                IEnumerator routine = null;
                if (info != null)
                {
                    routine = info.Invoke(component, args) as IEnumerator;
                    if (routine != null)
                    {
                        ((MonoBehaviour) component).StartCoroutine(routine);
                    }
                    flag = true;
                }
                else if (args.Length != 0)
                {
                    MethodInfo info2 = component.GetType().GetMethod(eventName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                    if (info2 != null)
                    {
                        routine = info2.Invoke(component, null) as IEnumerator;
                        if (routine != null)
                        {
                            ((MonoBehaviour) component).StartCoroutine(routine);
                        }
                        flag = true;
                    }
                }
            }
        }
        return flag;
    }

    protected internal bool SignalHierarchy(string eventName, params object[] args)
    {
        bool flag = false;
        for (Transform transform = base.transform; !flag && (transform != null); transform = transform.parent)
        {
            flag = this.Signal(transform.gameObject, eventName, args);
        }
        return flag;
    }

    [HideInInspector]
    public virtual void Start()
    {
    }

    [HideInInspector]
    public virtual void SuspendLayout()
    {
        this.ensureLayoutExists();
        this.layout.SuspendLayout();
        for (int i = 0; i < this.controls.Count; i++)
        {
            this.controls[i].SuspendLayout();
        }
    }

    protected internal Vector3 transformOffset(Vector3 offset)
    {
        Vector3 vector = (Vector3) (offset.x * this.getScaledDirection(Vector3.right));
        Vector3 vector2 = (Vector3) (offset.y * this.getScaledDirection(Vector3.down));
        return (Vector3) ((vector + vector2) * this.PixelsToUnits());
    }

    public void Unfocus()
    {
        if (this.ContainsFocus)
        {
            dfGUIManager.SetFocus(null);
        }
    }

    [HideInInspector]
    public virtual void Update()
    {
        Transform transform = base.transform;
        this.updateControlHierarchy(false);
        if (transform.hasChanged)
        {
            if (Application.isPlaying && (this.cachedScale != transform.localScale))
            {
                this.cachedScale = transform.localScale;
                this.Invalidate();
            }
            Vector3 vector = this.cachedPosition - transform.localPosition;
            if (vector.sqrMagnitude > float.Epsilon)
            {
                this.cachedPosition = transform.localPosition;
                this.OnPositionChanged();
            }
            if (this.cachedRotation != transform.localRotation)
            {
                this.cachedRotation = transform.localRotation;
                this.Invalidate();
            }
            transform.hasChanged = false;
        }
    }

    [HideInInspector]
    protected internal virtual void updateCollider()
    {
        if (!Application.isPlaying || this.isInteractive)
        {
            BoxCollider collider = base.collider as BoxCollider;
            if (collider == null)
            {
                collider = base.gameObject.AddComponent<BoxCollider>();
            }
            float num = this.PixelsToUnits();
            Vector2 size = (Vector2) (this.size * num);
            Vector3 vector2 = this.pivot.TransformToCenter(size);
            collider.size = new Vector3(size.x * this.hotZoneScale.x, size.y * this.hotZoneScale.y, 0.001f);
            collider.center = vector2;
            if (Application.isPlaying && !this.IsInteractive)
            {
                collider.enabled = false;
            }
            else
            {
                collider.enabled = base.enabled && this.IsVisible;
            }
        }
    }

    internal void updateControlHierarchy(bool force = false)
    {
        int childCount = base.transform.childCount;
        if (force || (childCount != this.cachedChildCount))
        {
            this.cachedChildCount = childCount;
            dfList<dfControl> list = this.getChildControls();
            for (int i = 0; i < list.Count; i++)
            {
                dfControl item = list[i];
                if (!this.controls.Contains(item))
                {
                    item.parent = this;
                    if (!Application.isPlaying)
                    {
                        item.ResetLayout(false, false);
                    }
                    this.OnControlAdded(item);
                    item.updateControlHierarchy(false);
                }
            }
            for (int j = 0; j < this.controls.Count; j++)
            {
                dfControl control2 = this.controls[j];
                if ((control2 == null) || !list.Contains(control2))
                {
                    this.OnControlRemoved(control2);
                    if ((control2 != null) && (control2.parent == this))
                    {
                        control2.parent = null;
                    }
                }
            }
            this.controls.Release();
            this.controls = list;
            this.RebuildControlOrder();
        }
    }

    protected internal void updateVersion()
    {
        this.version = ++versionCounter;
    }

    [SerializeField]
    public dfAnchorStyle Anchor
    {
        get
        {
            this.ensureLayoutExists();
            return this.layout.AnchorStyle;
        }
        set
        {
            this.ensureLayoutExists();
            if (value != this.layout.AnchorStyle)
            {
                this.layout.AnchorStyle = value;
                this.Invalidate();
                this.OnAnchorChanged();
            }
        }
    }

    public virtual bool CanFocus
    {
        get
        {
            return (this.canFocus && this.IsInteractive);
        }
        set
        {
            this.canFocus = value;
        }
    }

    public bool ClipChildren
    {
        get
        {
            return this.clipChildren;
        }
        set
        {
            if (value != this.clipChildren)
            {
                this.clipChildren = value;
                this.Invalidate();
            }
        }
    }

    public Color32 Color
    {
        get
        {
            return this.color;
        }
        set
        {
            if (!this.color.Equals(value))
            {
                this.color = value;
                this.OnColorChanged();
            }
        }
    }

    public virtual bool ContainsFocus
    {
        get
        {
            return dfGUIManager.ContainsFocus(this);
        }
    }

    public bool ContainsMouse
    {
        get
        {
            return this.isMouseHovering;
        }
    }

    public IList<dfControl> Controls
    {
        get
        {
            return this.controls;
        }
    }

    public Color32 DisabledColor
    {
        get
        {
            return this.disabledColor;
        }
        set
        {
            if (!value.Equals(this.disabledColor))
            {
                this.disabledColor = value;
                this.Invalidate();
            }
        }
    }

    public dfGUIManager GUIManager
    {
        get
        {
            return this.GetManager();
        }
    }

    public virtual bool HasFocus
    {
        get
        {
            return dfGUIManager.HasFocus(this);
        }
    }

    public float Height
    {
        get
        {
            return this.size.y;
        }
        set
        {
            this.Size = new Vector2(this.size.x, value);
        }
    }

    public Vector2 HotZoneScale
    {
        get
        {
            return this.hotZoneScale;
        }
        set
        {
            this.hotZoneScale = Vector2.Max(value, Vector2.zero);
            this.Invalidate();
        }
    }

    public bool IsEnabled
    {
        get
        {
            if (!base.enabled)
            {
                return false;
            }
            if ((base.gameObject != null) && !base.gameObject.activeSelf)
            {
                return false;
            }
            return ((this.parent == null) ? this.isEnabled : (this.isEnabled && this.parent.IsEnabled));
        }
        set
        {
            if (value != this.isEnabled)
            {
                this.isEnabled = value;
                this.OnIsEnabledChanged();
            }
        }
    }

    public virtual bool IsInteractive
    {
        get
        {
            return this.isInteractive;
        }
        set
        {
            if (this.HasFocus && !value)
            {
                dfGUIManager.SetFocus(null);
            }
            this.isInteractive = value;
        }
    }

    protected bool IsLayoutSuspended
    {
        get
        {
            return (this.performingLayout || ((this.layout != null) && this.layout.IsLayoutSuspended));
        }
    }

    public bool IsLocalized
    {
        get
        {
            return this.isLocalized;
        }
        set
        {
            this.isLocalized = value;
            if (value)
            {
                this.Localize();
            }
        }
    }

    protected bool IsPerformingLayout
    {
        get
        {
            return (this.performingLayout || ((this.layout != null) && this.layout.IsPerformingLayout));
        }
    }

    [SerializeField]
    public bool IsVisible
    {
        get
        {
            return ((this.parent != null) ? (this.isVisible && this.parent.IsVisible) : this.isVisible);
        }
        set
        {
            if (value != this.isVisible)
            {
                if (Application.isPlaying && !this.IsInteractive)
                {
                    base.collider.enabled = false;
                }
                else
                {
                    base.collider.enabled = value;
                }
                this.isVisible = value;
                this.OnIsVisibleChanged();
            }
        }
    }

    public Vector2 MaximumSize
    {
        get
        {
            return this.maxSize;
        }
        set
        {
            value = Vector2.Max(Vector2.zero, value.RoundToInt());
            if (value != this.maxSize)
            {
                this.maxSize = value;
                this.Invalidate();
            }
        }
    }

    public Vector2 MinimumSize
    {
        get
        {
            return this.minSize;
        }
        set
        {
            value = Vector2.Max(Vector2.zero, value.RoundToInt());
            if (value != this.minSize)
            {
                this.minSize = value;
                this.Invalidate();
            }
        }
    }

    public float Opacity
    {
        get
        {
            return (((float) this.color.a) / 255f);
        }
        set
        {
            value = Mathf.Max(0f, Mathf.Min(1f, value));
            float num = ((float) this.color.a) / 255f;
            if (value != num)
            {
                this.color.a = (byte) (value * 255f);
                this.OnOpacityChanged();
            }
        }
    }

    public dfControl Parent
    {
        get
        {
            return this.parent;
        }
    }

    public dfPivotPoint Pivot
    {
        get
        {
            return this.pivot;
        }
        set
        {
            if (value != this.pivot)
            {
                Vector3 position = this.Position;
                this.pivot = value;
                Vector3 vector2 = this.Position - position;
                this.SuspendLayout();
                this.Position = position;
                for (int i = 0; i < this.controls.Count; i++)
                {
                    dfControl local1 = this.controls[i];
                    local1.Position += vector2;
                }
                this.ResumeLayout();
                this.OnPivotChanged();
            }
        }
    }

    public Vector3 Position
    {
        get
        {
            Vector3 vector = (Vector3) (base.transform.localPosition / this.PixelsToUnits());
            return (vector + this.pivot.TransformToUpperLeft(this.Size));
        }
        set
        {
            this.setPositionInternal(value);
        }
    }

    public Vector3 RelativePosition
    {
        get
        {
            return this.getRelativePosition();
        }
        set
        {
            this.setRelativePosition(value);
        }
    }

    [HideInInspector]
    public int RenderOrder
    {
        get
        {
            return this.renderOrder;
        }
    }

    public Vector2 Size
    {
        get
        {
            return this.size;
        }
        set
        {
            value = Vector2.Max(this.CalculateMinimumSize(), value);
            value.x = (this.maxSize.x <= 0f) ? value.x : Mathf.Min(value.x, this.maxSize.x);
            value.y = (this.maxSize.y <= 0f) ? value.y : Mathf.Min(value.y, this.maxSize.y);
            Vector2 vector = value - this.size;
            if (vector.sqrMagnitude > float.Epsilon)
            {
                this.size = value;
                this.OnSizeChanged();
            }
        }
    }

    [HideInInspector]
    public int TabIndex
    {
        get
        {
            return this.tabIndex;
        }
        set
        {
            if (value != this.tabIndex)
            {
                this.tabIndex = Mathf.Max(-1, value);
                this.OnTabIndexChanged();
            }
        }
    }

    public object Tag
    {
        get
        {
            return this.tag;
        }
        set
        {
            this.tag = value;
        }
    }

    [SerializeField]
    public string Tooltip
    {
        get
        {
            return this.tooltip;
        }
        set
        {
            if (value != this.tooltip)
            {
                this.tooltip = value;
                this.Invalidate();
            }
        }
    }

    internal uint Version
    {
        get
        {
            return this.version;
        }
    }

    public float Width
    {
        get
        {
            return this.size.x;
        }
        set
        {
            this.Size = new Vector2(value, this.size.y);
        }
    }

    [HideInInspector]
    public int ZOrder
    {
        get
        {
            return this.zindex;
        }
        set
        {
            if (value != this.zindex)
            {
                this.zindex = Mathf.Max(-1, value);
                this.Invalidate();
                if (this.parent != null)
                {
                    this.parent.SetControlIndex(this, value);
                }
                this.OnZOrderChanged();
            }
        }
    }

    [CompilerGenerated]
    private sealed class <RaiseEvent>c__AnonStorey51
    {
        internal string eventName;

        internal bool <>m__1A(FieldInfo f)
        {
            return (f.Name == this.eventName);
        }
    }

    [CompilerGenerated]
    private sealed class <RemoveEventHandlers>c__AnonStorey52
    {
        internal string EventName;

        internal bool <>m__1B(FieldInfo f)
        {
            return (typeof(Delegate).IsAssignableFrom(f.FieldType) && (f.Name == this.EventName));
        }
    }

    [CompilerGenerated]
    private sealed class <SetControlIndex>c__AnonStorey53
    {
        internal dfControl child;
        internal int zindex;

        internal bool <>m__1D(dfControl c)
        {
            return ((c.zindex == this.zindex) && (c != this.child));
        }
    }

    [Serializable]
    protected class AnchorLayout
    {
        [SerializeField]
        protected dfAnchorStyle anchorStyle;
        private bool disposed;
        [SerializeField]
        protected dfAnchorMargins margins;
        [SerializeField]
        protected dfControl owner;
        private bool pendingLayoutRequest;
        private bool performingLayout;
        private int suspendLayoutCounter;

        internal AnchorLayout(dfAnchorStyle anchorStyle)
        {
            this.anchorStyle = anchorStyle;
        }

        internal AnchorLayout(dfAnchorStyle anchorStyle, dfControl owner) : this(anchorStyle)
        {
            this.Attach(owner);
            this.Reset(false);
        }

        internal void Attach(dfControl ownerControl)
        {
            this.owner = ownerControl;
        }

        internal void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.owner = null;
            }
        }

        private Vector2 getParentSize()
        {
            dfControl component = this.owner.transform.parent.GetComponent<dfControl>();
            if (component != null)
            {
                return component.Size;
            }
            return this.owner.GetManager().GetScreenSize();
        }

        private string getPath(dfControl owner)
        {
            StringBuilder builder = new StringBuilder(0x400);
            while (owner != null)
            {
                if (builder.Length > 0)
                {
                    builder.Insert(0, '/');
                }
                builder.Insert(0, owner.name);
                owner = owner.Parent;
            }
            return builder.ToString();
        }

        internal void PerformLayout()
        {
            if (!this.disposed)
            {
                if (this.suspendLayoutCounter > 0)
                {
                    this.pendingLayoutRequest = true;
                }
                else
                {
                    this.performLayoutInternal();
                }
            }
        }

        private void performLayoutAbsolute(Vector2 parentSize, Vector2 controlSize)
        {
            float left = this.margins.left;
            float top = this.margins.top;
            float num3 = left + controlSize.x;
            float num4 = top + controlSize.y;
            if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterHorizontal))
            {
                left = Mathf.RoundToInt((parentSize.x - controlSize.x) * 0.5f);
                num3 = Mathf.RoundToInt(left + controlSize.x);
            }
            else
            {
                if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Left))
                {
                    left = this.margins.left;
                    num3 = left + controlSize.x;
                }
                if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Right))
                {
                    num3 = parentSize.x - this.margins.right;
                    if (!this.anchorStyle.IsFlagSet(dfAnchorStyle.Left))
                    {
                        left = num3 - controlSize.x;
                    }
                }
            }
            if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterVertical))
            {
                top = Mathf.RoundToInt((parentSize.y - controlSize.y) * 0.5f);
                num4 = Mathf.RoundToInt(top + controlSize.y);
            }
            else
            {
                if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Top))
                {
                    top = this.margins.top;
                    num4 = top + controlSize.y;
                }
                if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Bottom))
                {
                    num4 = parentSize.y - this.margins.bottom;
                    if (!this.anchorStyle.IsFlagSet(dfAnchorStyle.Top))
                    {
                        top = num4 - controlSize.y;
                    }
                }
            }
            Vector2 vector = new Vector2(Mathf.Max((float) 0f, (float) (num3 - left)), Mathf.Max((float) 0f, (float) (num4 - top)));
            this.owner.Size = vector;
            this.owner.RelativePosition = new Vector3(left, top);
        }

        protected void performLayoutInternal()
        {
            if ((((this.margins != null) && !this.IsPerformingLayout) && (!this.IsLayoutSuspended && (this.owner != null))) && this.owner.gameObject.activeSelf)
            {
                try
                {
                    this.performingLayout = true;
                    this.pendingLayoutRequest = false;
                    Vector2 parentSize = this.getParentSize();
                    Vector2 size = this.owner.Size;
                    if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Proportional))
                    {
                        this.performLayoutProportional(parentSize, size);
                    }
                    else
                    {
                        this.performLayoutAbsolute(parentSize, size);
                    }
                }
                finally
                {
                    this.performingLayout = false;
                }
            }
        }

        private void performLayoutProportional(Vector2 parentSize, Vector2 controlSize)
        {
            float num = this.margins.left * parentSize.x;
            float num2 = this.margins.right * parentSize.x;
            float num3 = this.margins.top * parentSize.y;
            float num4 = this.margins.bottom * parentSize.y;
            Vector3 relativePosition = this.owner.RelativePosition;
            Vector2 vector2 = controlSize;
            if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Left))
            {
                relativePosition.x = num;
                if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Right))
                {
                    vector2.x = (this.margins.right - this.margins.left) * parentSize.x;
                }
            }
            else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Right))
            {
                relativePosition.x = num2 - controlSize.x;
            }
            else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterHorizontal))
            {
                relativePosition.x = (parentSize.x - controlSize.x) * 0.5f;
            }
            if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Top))
            {
                relativePosition.y = num3;
                if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Bottom))
                {
                    vector2.y = (this.margins.bottom - this.margins.top) * parentSize.y;
                }
            }
            else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Bottom))
            {
                relativePosition.y = num4 - controlSize.y;
            }
            else if (this.anchorStyle.IsFlagSet(dfAnchorStyle.CenterVertical))
            {
                relativePosition.y = (parentSize.y - controlSize.y) * 0.5f;
            }
            this.owner.Size = vector2;
            this.owner.RelativePosition = relativePosition;
            if (this.owner.GetManager().PixelPerfectMode)
            {
                this.owner.MakePixelPerfect(false);
            }
        }

        internal void Reset(bool force = false)
        {
            if (((this.owner != null) && (this.owner.transform.parent != null)) && (((force || (!this.IsPerformingLayout && !this.IsLayoutSuspended)) && (this.owner != null)) && this.owner.gameObject.activeSelf))
            {
                if (this.anchorStyle.IsFlagSet(dfAnchorStyle.Proportional))
                {
                    this.resetLayoutProportional();
                }
                else
                {
                    this.resetLayoutAbsolute();
                }
            }
        }

        private void resetLayoutAbsolute()
        {
            Vector3 relativePosition = this.owner.RelativePosition;
            Vector2 size = this.owner.Size;
            Vector2 vector3 = this.getParentSize();
            float x = relativePosition.x;
            float y = relativePosition.y;
            float num3 = (vector3.x - size.x) - x;
            float num4 = (vector3.y - size.y) - y;
            if (this.margins == null)
            {
                this.margins = new dfAnchorMargins();
            }
            this.margins.left = x;
            this.margins.right = num3;
            this.margins.top = y;
            this.margins.bottom = num4;
        }

        private void resetLayoutProportional()
        {
            Vector3 relativePosition = this.owner.RelativePosition;
            Vector2 size = this.owner.Size;
            Vector2 vector3 = this.getParentSize();
            float x = relativePosition.x;
            float y = relativePosition.y;
            float num3 = x + size.x;
            float num4 = y + size.y;
            if (this.margins == null)
            {
                this.margins = new dfAnchorMargins();
            }
            this.margins.left = x / vector3.x;
            this.margins.right = num3 / vector3.x;
            this.margins.top = y / vector3.y;
            this.margins.bottom = num4 / vector3.y;
        }

        internal void ResumeLayout()
        {
            bool flag = this.suspendLayoutCounter > 0;
            this.suspendLayoutCounter = Mathf.Max(0, this.suspendLayoutCounter - 1);
            if ((flag && (this.suspendLayoutCounter == 0)) && this.pendingLayoutRequest)
            {
                this.PerformLayout();
            }
        }

        internal void SuspendLayout()
        {
            this.suspendLayoutCounter++;
        }

        public override string ToString()
        {
            if (this.owner == null)
            {
                return "NO OWNER FOR ANCHOR";
            }
            dfControl parent = this.owner.parent;
            return string.Format("{0}.{1} - {2}", (parent == null) ? "SCREEN" : parent.name, this.owner.name, this.margins);
        }

        internal dfAnchorStyle AnchorStyle
        {
            get
            {
                return this.anchorStyle;
            }
            set
            {
                if (value != this.anchorStyle)
                {
                    this.anchorStyle = value;
                    this.Reset(false);
                }
            }
        }

        internal bool HasPendingLayoutRequest
        {
            get
            {
                return this.pendingLayoutRequest;
            }
        }

        internal bool IsLayoutSuspended
        {
            get
            {
                return (this.suspendLayoutCounter > 0);
            }
        }

        internal bool IsPerformingLayout
        {
            get
            {
                return this.performingLayout;
            }
        }
    }
}

