using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Containers/Scrollable Panel"), ExecuteInEditMode]
public class dfScrollPanel : dfControl
{
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected bool autoLayout;
    [SerializeField]
    protected bool autoReset = true;
    [SerializeField]
    protected Color32 backgroundColor = Color.white;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected LayoutDirection flowDirection;
    [SerializeField]
    protected RectOffset flowPadding = new RectOffset();
    [SerializeField]
    protected dfScrollbar horzScroll;
    private bool initialized;
    private bool isMouseDown;
    private bool resetNeeded;
    private bool scrolling;
    private Vector2 scrollMomentum = Vector2.zero;
    [SerializeField]
    protected RectOffset scrollPadding = new RectOffset();
    [SerializeField]
    protected Vector2 scrollPosition = Vector2.zero;
    [SerializeField]
    protected int scrollWheelAmount = 10;
    [SerializeField]
    protected bool scrollWithArrowKeys;
    private Vector2 touchStartPosition = Vector2.zero;
    [SerializeField]
    protected bool useScrollMomentum;
    [SerializeField]
    protected dfScrollbar vertScroll;
    [SerializeField]
    protected dfControlOrientation wheelDirection;
    [SerializeField]
    protected bool wrapLayout;

    public event PropertyChangedEventHandler<Vector2> ScrollPositionChanged;

    private void attachEvents(dfControl control)
    {
        control.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.childIsVisibleChanged);
        control.PositionChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.ZOrderChanged += new PropertyChangedEventHandler<int>(this.childOrderChanged);
    }

    [HideInInspector]
    private void AutoArrange()
    {
        this.SuspendLayout();
        try
        {
            this.scrollPadding = this.ScrollPadding.ConstrainPadding();
            this.flowPadding = this.FlowPadding.ConstrainPadding();
            float x = (this.scrollPadding.left + this.flowPadding.left) - this.scrollPosition.x;
            float y = (this.scrollPadding.top + this.flowPadding.top) - this.scrollPosition.y;
            float b = 0f;
            float num4 = 0f;
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfControl control = base.controls[i];
                if (((control.IsVisible && control.enabled) && control.gameObject.activeSelf) && ((control != this.horzScroll) && (control != this.vertScroll)))
                {
                    if (this.wrapLayout)
                    {
                        if (this.flowDirection == LayoutDirection.Horizontal)
                        {
                            if ((x + control.Width) >= (this.size.x - this.scrollPadding.right))
                            {
                                x = this.scrollPadding.left + this.flowPadding.left;
                                y += num4;
                                num4 = 0f;
                            }
                        }
                        else if (((y + control.Height) + this.flowPadding.vertical) >= (this.size.y - this.scrollPadding.bottom))
                        {
                            y = this.scrollPadding.top + this.flowPadding.top;
                            x += b;
                            b = 0f;
                        }
                    }
                    Vector2 vector = new Vector2(x, y);
                    control.RelativePosition = (Vector3) vector;
                    float a = control.Width + this.flowPadding.horizontal;
                    float num7 = control.Height + this.flowPadding.vertical;
                    b = Mathf.Max(a, b);
                    num4 = Mathf.Max(num7, num4);
                    if (this.flowDirection == LayoutDirection.Horizontal)
                    {
                        x += a;
                    }
                    else
                    {
                        y += num7;
                    }
                }
            }
            this.updateScrollbars();
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    private Vector2 calculateMinChildPosition()
    {
        float maxValue = float.MaxValue;
        float a = float.MaxValue;
        for (int i = 0; i < base.controls.Count; i++)
        {
            dfControl control = base.controls[i];
            if (control.enabled && control.gameObject.activeSelf)
            {
                Vector3 vector = control.RelativePosition.FloorToInt();
                maxValue = Mathf.Min(maxValue, vector.x);
                a = Mathf.Min(a, vector.y);
            }
        }
        return new Vector2(maxValue, a);
    }

    private Vector2 calculateViewSize()
    {
        Vector2 vector = new Vector2((float) this.scrollPadding.horizontal, (float) this.scrollPadding.vertical).RoundToInt();
        Vector2 rhs = base.Size.RoundToInt() - vector;
        if (!base.IsVisible || (base.controls.Count == 0))
        {
            return rhs;
        }
        Vector2 vector3 = (Vector2) (Vector2.one * 3.402823E+38f);
        Vector2 vector4 = (Vector2) (Vector2.one * -3.402823E+38f);
        for (int i = 0; i < base.controls.Count; i++)
        {
            dfControl control = base.controls[i];
            if (!Application.isPlaying || control.IsVisible)
            {
                Vector2 lhs = control.RelativePosition.RoundToInt();
                Vector2 vector6 = lhs + control.Size.RoundToInt();
                vector3 = Vector2.Min(lhs, vector3);
                vector4 = Vector2.Max(vector6, vector4);
            }
        }
        return (Vector2.Max(vector4, rhs) - vector3);
    }

    public void CenterChildControls()
    {
        if (base.controls.Count != 0)
        {
            Vector2 lhs = (Vector2) (Vector2.one * 3.402823E+38f);
            Vector2 vector2 = (Vector2) (Vector2.one * -3.402823E+38f);
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfControl control = base.controls[i];
                Vector2 relativePosition = control.RelativePosition;
                Vector2 rhs = relativePosition + control.Size;
                lhs = Vector2.Min(lhs, relativePosition);
                vector2 = Vector2.Max(vector2, rhs);
            }
            Vector2 vector5 = vector2 - lhs;
            Vector2 vector6 = (Vector2) ((base.Size - vector5) * 0.5f);
            for (int j = 0; j < base.controls.Count; j++)
            {
                dfControl control2 = base.controls[j];
                control2.RelativePosition = (control2.RelativePosition - lhs) + vector6;
            }
        }
    }

    private void childControlInvalidated(dfControl control, Vector2 value)
    {
        this.onChildControlInvalidatedLayout();
    }

    private void childIsVisibleChanged(dfControl control, bool value)
    {
        this.onChildControlInvalidatedLayout();
    }

    private void childOrderChanged(dfControl control, int value)
    {
        this.onChildControlInvalidatedLayout();
    }

    private void detachEvents(dfControl control)
    {
        control.IsVisibleChanged -= new PropertyChangedEventHandler<bool>(this.childIsVisibleChanged);
        control.PositionChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.ZOrderChanged -= new PropertyChangedEventHandler<int>(this.childOrderChanged);
    }

    public void FitToContents()
    {
        if (base.controls.Count != 0)
        {
            Vector2 zero = Vector2.zero;
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfControl control = base.controls[i];
                Vector2 rhs = control.RelativePosition + control.Size;
                zero = Vector2.Max(zero, rhs);
            }
            base.Size = zero + new Vector2((float) this.scrollPadding.right, (float) this.scrollPadding.bottom);
        }
    }

    protected internal override Plane[] GetClippingPlanes()
    {
        if (!base.ClipChildren)
        {
            return null;
        }
        Vector3[] corners = base.GetCorners();
        Vector3 inNormal = base.transform.TransformDirection(Vector3.right);
        Vector3 vector2 = base.transform.TransformDirection(Vector3.left);
        Vector3 vector3 = base.transform.TransformDirection(Vector3.up);
        Vector3 vector4 = base.transform.TransformDirection(Vector3.down);
        float num = base.PixelsToUnits();
        RectOffset scrollPadding = this.ScrollPadding;
        corners[0] += (Vector3) (((inNormal * scrollPadding.left) * num) + ((vector4 * scrollPadding.top) * num));
        corners[1] += (Vector3) (((vector2 * scrollPadding.right) * num) + ((vector4 * scrollPadding.top) * num));
        corners[2] += (Vector3) (((inNormal * scrollPadding.left) * num) + ((vector3 * scrollPadding.bottom) * num));
        return new Plane[] { new Plane(inNormal, corners[0]), new Plane(vector2, corners[1]), new Plane(vector3, corners[2]), new Plane(vector4, corners[0]) };
    }

    private void horzScroll_ValueChanged(dfControl control, float value)
    {
        this.ScrollPosition = new Vector2(value, this.ScrollPosition.y);
    }

    [HideInInspector]
    private void initialize()
    {
        if (!this.initialized)
        {
            this.initialized = true;
            if (Application.isPlaying)
            {
                if (this.horzScroll != null)
                {
                    this.horzScroll.ValueChanged += new PropertyChangedEventHandler<float>(this.horzScroll_ValueChanged);
                }
                if (this.vertScroll != null)
                {
                    this.vertScroll.ValueChanged += new PropertyChangedEventHandler<float>(this.vertScroll_ValueChanged);
                }
            }
            if ((this.resetNeeded || this.autoLayout) || this.autoReset)
            {
                this.Reset();
            }
            this.Invalidate();
            this.ScrollPosition = Vector2.zero;
            this.updateScrollbars();
        }
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        this.initialize();
        if (this.resetNeeded && base.IsVisible)
        {
            this.resetNeeded = false;
            if (this.autoReset || this.autoLayout)
            {
                this.Reset();
            }
        }
    }

    [HideInInspector]
    private void onChildControlInvalidatedLayout()
    {
        if (!this.scrolling && !base.IsLayoutSuspended)
        {
            if (this.autoLayout)
            {
                this.AutoArrange();
            }
            this.updateScrollbars();
            this.Invalidate();
        }
    }

    protected internal override void OnControlAdded(dfControl child)
    {
        base.OnControlAdded(child);
        this.attachEvents(child);
        if (this.autoLayout)
        {
            this.AutoArrange();
        }
    }

    protected internal override void OnControlRemoved(dfControl child)
    {
        base.OnControlRemoved(child);
        if (child != null)
        {
            this.detachEvents(child);
        }
        if (this.autoLayout)
        {
            this.AutoArrange();
        }
        else
        {
            this.updateScrollbars();
        }
    }

    public override void OnDestroy()
    {
        if (this.horzScroll != null)
        {
            this.horzScroll.ValueChanged -= new PropertyChangedEventHandler<float>(this.horzScroll_ValueChanged);
        }
        if (this.vertScroll != null)
        {
            this.vertScroll.ValueChanged -= new PropertyChangedEventHandler<float>(this.vertScroll_ValueChanged);
        }
        this.horzScroll = null;
        this.vertScroll = null;
    }

    internal override void OnDragStart(dfDragEventArgs args)
    {
        base.OnDragStart(args);
        if (args.Used)
        {
            this.isMouseDown = false;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (base.size == Vector2.zero)
        {
            this.SuspendLayout();
            Camera camera = base.GetCamera();
            base.Size = new Vector3(camera.pixelWidth / 2f, camera.pixelHeight / 2f);
            this.ResumeLayout();
        }
        if (this.autoLayout)
        {
            this.AutoArrange();
        }
        this.updateScrollbars();
    }

    protected internal override void OnGotFocus(dfFocusEventArgs args)
    {
        if (args.Source != this)
        {
            this.ScrollIntoView(args.Source);
        }
        base.OnGotFocus(args);
    }

    protected internal override void OnIsVisibleChanged()
    {
        base.OnIsVisibleChanged();
        if (base.IsVisible && (this.autoReset || this.autoLayout))
        {
            this.Reset();
            this.updateScrollbars();
        }
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        if (!this.scrollWithArrowKeys || args.Used)
        {
            base.OnKeyDown(args);
        }
        else
        {
            float x = (this.horzScroll == null) ? 1f : this.horzScroll.IncrementAmount;
            float y = (this.vertScroll == null) ? 1f : this.vertScroll.IncrementAmount;
            if (args.KeyCode == KeyCode.LeftArrow)
            {
                this.ScrollPosition += new Vector2(-x, 0f);
                args.Use();
            }
            else if (args.KeyCode == KeyCode.RightArrow)
            {
                this.ScrollPosition += new Vector2(x, 0f);
                args.Use();
            }
            else if (args.KeyCode == KeyCode.UpArrow)
            {
                this.ScrollPosition += new Vector2(0f, -y);
                args.Use();
            }
            else if (args.KeyCode == KeyCode.DownArrow)
            {
                this.ScrollPosition += new Vector2(0f, y);
                args.Use();
            }
            base.OnKeyDown(args);
        }
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        base.OnMouseDown(args);
        this.touchStartPosition = args.Position;
        this.isMouseDown = true;
    }

    protected internal override void OnMouseEnter(dfMouseEventArgs args)
    {
        base.OnMouseEnter(args);
        this.touchStartPosition = args.Position;
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        if (((args is dfTouchEventArgs) || this.isMouseDown) && !args.Used)
        {
            Vector2 vector2 = args.Position - this.touchStartPosition;
            if (vector2.magnitude > 5f)
            {
                Vector2 vector = args.MoveDelta.Scale(-1f, 1f);
                this.ScrollPosition += vector;
                this.scrollMomentum = (Vector2) ((this.scrollMomentum + vector) * 0.5f);
                args.Use();
            }
        }
        base.OnMouseMove(args);
    }

    protected internal override void OnMouseUp(dfMouseEventArgs args)
    {
        base.OnMouseUp(args);
        this.isMouseDown = false;
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
        try
        {
            if (!args.Used)
            {
                float num = (this.wheelDirection != dfControlOrientation.Horizontal) ? ((this.vertScroll == null) ? ((float) this.scrollWheelAmount) : this.vertScroll.IncrementAmount) : ((this.horzScroll == null) ? ((float) this.scrollWheelAmount) : this.horzScroll.IncrementAmount);
                if (this.wheelDirection == dfControlOrientation.Horizontal)
                {
                    this.ScrollPosition = new Vector2(this.scrollPosition.x - (num * args.WheelDelta), this.scrollPosition.y);
                    this.scrollMomentum = new Vector2(-num * args.WheelDelta, 0f);
                }
                else
                {
                    this.ScrollPosition = new Vector2(this.scrollPosition.x, this.scrollPosition.y - (num * args.WheelDelta));
                    this.scrollMomentum = new Vector2(0f, -num * args.WheelDelta);
                }
                args.Use();
                object[] objArray1 = new object[] { args };
                base.Signal("OnMouseWheel", objArray1);
            }
        }
        finally
        {
            base.OnMouseWheel(args);
        }
    }

    protected override void OnRebuildRenderData()
    {
        if ((this.Atlas != null) && !string.IsNullOrEmpty(this.backgroundSprite))
        {
            dfAtlas.ItemInfo info = this.Atlas[this.backgroundSprite];
            if (info != null)
            {
                base.renderData.Material = this.Atlas.Material;
                Color32 color = base.ApplyOpacity(this.BackgroundColor);
                dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                    atlas = this.atlas,
                    color = color,
                    fillAmount = 1f,
                    offset = base.pivot.TransformToUpperLeft(base.Size),
                    pixelsToUnits = base.PixelsToUnits(),
                    size = base.Size,
                    spriteInfo = info
                };
                if ((info.border.horizontal == 0) && (info.border.vertical == 0))
                {
                    dfSprite.renderSprite(base.renderData, options);
                }
                else
                {
                    dfSlicedSprite.renderSprite(base.renderData, options);
                }
            }
        }
    }

    protected internal override void OnResolutionChanged(Vector2 previousResolution, Vector2 currentResolution)
    {
        base.OnResolutionChanged(previousResolution, currentResolution);
        this.resetNeeded = true;
    }

    protected internal void OnScrollPositionChanged()
    {
        this.Invalidate();
        object[] args = new object[] { this.ScrollPosition };
        base.SignalHierarchy("OnScrollPositionChanged", args);
        if (this.ScrollPositionChanged != null)
        {
            this.ScrollPositionChanged(this, this.ScrollPosition);
        }
    }

    protected internal override void OnSizeChanged()
    {
        base.OnSizeChanged();
        if (this.autoReset || this.autoLayout)
        {
            this.Reset();
        }
        else
        {
            Vector2 lhs = this.calculateMinChildPosition();
            if ((lhs.x > this.scrollPadding.left) || (lhs.y > this.scrollPadding.top))
            {
                lhs -= new Vector2((float) this.scrollPadding.left, (float) this.scrollPadding.top);
                lhs = Vector2.Max(lhs, Vector2.zero);
                this.scrollChildControls((Vector3) lhs);
            }
            this.updateScrollbars();
        }
    }

    public void Reset()
    {
        try
        {
            this.SuspendLayout();
            if (this.autoLayout)
            {
                Vector2 scrollPosition = this.ScrollPosition;
                this.ScrollPosition = Vector2.zero;
                this.AutoArrange();
                this.ScrollPosition = scrollPosition;
            }
            else
            {
                this.scrollPadding = this.ScrollPadding.ConstrainPadding();
                Vector3 vector2 = ((Vector3) this.calculateMinChildPosition()) - new Vector3((float) this.scrollPadding.left, (float) this.scrollPadding.top);
                for (int i = 0; i < base.controls.Count; i++)
                {
                    dfControl local1 = base.controls[i];
                    local1.RelativePosition -= vector2;
                }
                this.scrollPosition = Vector2.zero;
            }
            this.Invalidate();
            this.updateScrollbars();
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    private void scrollChildControls(Vector3 delta)
    {
        try
        {
            this.scrolling = true;
            delta = delta.Scale(1f, -1f, 1f);
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfControl control = base.controls[i];
                control.Position = (control.Position - delta).RoundToInt();
            }
        }
        finally
        {
            this.scrolling = false;
        }
    }

    public void ScrollIntoView(dfControl control)
    {
        Rect rect = new Rect(this.scrollPosition.x + this.scrollPadding.left, this.scrollPosition.y + this.scrollPadding.top, this.size.x - this.scrollPadding.horizontal, this.size.y - this.scrollPadding.vertical).RoundToInt();
        Vector3 relativePosition = control.RelativePosition;
        Vector2 size = control.Size;
        while (!base.controls.Contains(control))
        {
            control = control.Parent;
            relativePosition += control.RelativePosition;
        }
        Rect other = new Rect(this.scrollPosition.x + relativePosition.x, this.scrollPosition.y + relativePosition.y, size.x, size.y).RoundToInt();
        if (!rect.Contains(other))
        {
            Vector2 scrollPosition = this.scrollPosition;
            if (other.xMin < rect.xMin)
            {
                scrollPosition.x = other.xMin - this.scrollPadding.left;
            }
            else if (other.xMax > rect.xMax)
            {
                scrollPosition.x = (other.xMax - Mathf.Max(this.size.x, size.x)) + this.scrollPadding.horizontal;
            }
            if (other.y < rect.y)
            {
                scrollPosition.y = other.yMin - this.scrollPadding.top;
            }
            else if (other.yMax > rect.yMax)
            {
                scrollPosition.y = (other.yMax - Mathf.Max(this.size.y, size.y)) + this.scrollPadding.vertical;
            }
            this.ScrollPosition = scrollPosition;
            this.scrollMomentum = Vector2.zero;
        }
    }

    public void ScrollToBottom()
    {
        this.ScrollPosition = new Vector2(this.scrollPosition.x, 2.147484E+09f);
    }

    public void ScrollToLeft()
    {
        this.ScrollPosition = new Vector2(0f, this.scrollPosition.y);
    }

    public void ScrollToRight()
    {
        this.ScrollPosition = new Vector2(2.147484E+09f, this.scrollPosition.y);
    }

    public void ScrollToTop()
    {
        this.ScrollPosition = new Vector2(this.scrollPosition.x, 0f);
    }

    public override void Update()
    {
        base.Update();
        if ((this.useScrollMomentum && !this.isMouseDown) && (this.scrollMomentum.sqrMagnitude > float.Epsilon))
        {
            this.ScrollPosition += this.scrollMomentum;
        }
        if ((base.isControlInvalidated && this.autoLayout) && base.IsVisible)
        {
            this.AutoArrange();
            this.updateScrollbars();
        }
        this.scrollMomentum = (Vector2) (this.scrollMomentum * (0.95f - Time.deltaTime));
    }

    [HideInInspector]
    private void updateScrollbars()
    {
        Vector2 vector = this.calculateViewSize();
        Vector2 vector2 = base.Size - new Vector2((float) this.scrollPadding.horizontal, (float) this.scrollPadding.vertical);
        if (this.horzScroll != null)
        {
            this.horzScroll.MinValue = 0f;
            this.horzScroll.MaxValue = vector.x;
            this.horzScroll.ScrollSize = vector2.x;
            this.horzScroll.Value = Mathf.Max(0f, this.scrollPosition.x);
        }
        if (this.vertScroll != null)
        {
            this.vertScroll.MinValue = 0f;
            this.vertScroll.MaxValue = vector.y;
            this.vertScroll.ScrollSize = vector2.y;
            this.vertScroll.Value = Mathf.Max(0f, this.scrollPosition.y);
        }
    }

    private void vertScroll_ValueChanged(dfControl control, float value)
    {
        this.ScrollPosition = new Vector2(this.scrollPosition.x, value);
    }

    public dfAtlas Atlas
    {
        get
        {
            if (this.atlas == null)
            {
                dfGUIManager manager = base.GetManager();
                if (manager != null)
                {
                    return (this.atlas = manager.DefaultAtlas);
                }
            }
            return this.atlas;
        }
        set
        {
            if (!dfAtlas.Equals(value, this.atlas))
            {
                this.atlas = value;
                this.Invalidate();
            }
        }
    }

    public bool AutoLayout
    {
        get
        {
            return this.autoLayout;
        }
        set
        {
            if (value != this.autoLayout)
            {
                this.autoLayout = value;
                this.Reset();
            }
        }
    }

    public bool AutoReset
    {
        get
        {
            return this.autoReset;
        }
        set
        {
            if (value != this.autoReset)
            {
                this.autoReset = value;
                if (value)
                {
                    this.Reset();
                }
            }
        }
    }

    public Color32 BackgroundColor
    {
        get
        {
            return this.backgroundColor;
        }
        set
        {
            if (!object.Equals(value, this.backgroundColor))
            {
                this.backgroundColor = value;
                this.Invalidate();
            }
        }
    }

    public string BackgroundSprite
    {
        get
        {
            return this.backgroundSprite;
        }
        set
        {
            if (value != this.backgroundSprite)
            {
                this.backgroundSprite = value;
                this.Invalidate();
            }
        }
    }

    public override bool CanFocus
    {
        get
        {
            return ((base.IsEnabled && base.IsVisible) || base.CanFocus);
        }
    }

    public LayoutDirection FlowDirection
    {
        get
        {
            return this.flowDirection;
        }
        set
        {
            if (value != this.flowDirection)
            {
                this.flowDirection = value;
                this.Reset();
            }
        }
    }

    public RectOffset FlowPadding
    {
        get
        {
            if (this.flowPadding == null)
            {
                this.flowPadding = new RectOffset();
            }
            return this.flowPadding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.flowPadding))
            {
                this.flowPadding = value;
                this.Reset();
            }
        }
    }

    public dfScrollbar HorzScrollbar
    {
        get
        {
            return this.horzScroll;
        }
        set
        {
            this.horzScroll = value;
            this.updateScrollbars();
        }
    }

    public RectOffset ScrollPadding
    {
        get
        {
            if (this.scrollPadding == null)
            {
                this.scrollPadding = new RectOffset();
            }
            return this.scrollPadding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.scrollPadding))
            {
                this.scrollPadding = value;
                this.Reset();
            }
        }
    }

    public Vector2 ScrollPosition
    {
        get
        {
            return this.scrollPosition;
        }
        set
        {
            Vector2 vector = this.calculateViewSize();
            Vector2 vector2 = new Vector2(this.size.x - this.scrollPadding.horizontal, this.size.y - this.scrollPadding.vertical);
            value = Vector2.Min(vector - vector2, value);
            value = Vector2.Max(Vector2.zero, value);
            value = value.RoundToInt();
            Vector2 vector4 = value - this.scrollPosition;
            if (vector4.sqrMagnitude > float.Epsilon)
            {
                Vector2 vector3 = value - this.scrollPosition;
                this.scrollPosition = value;
                this.scrollChildControls((Vector3) vector3);
                this.updateScrollbars();
            }
            this.OnScrollPositionChanged();
        }
    }

    public int ScrollWheelAmount
    {
        get
        {
            return this.scrollWheelAmount;
        }
        set
        {
            this.scrollWheelAmount = value;
        }
    }

    public bool ScrollWithArrowKeys
    {
        get
        {
            return this.scrollWithArrowKeys;
        }
        set
        {
            this.scrollWithArrowKeys = value;
        }
    }

    public bool UseScrollMomentum
    {
        get
        {
            return this.useScrollMomentum;
        }
        set
        {
            this.useScrollMomentum = value;
            this.scrollMomentum = Vector2.zero;
        }
    }

    public dfScrollbar VertScrollbar
    {
        get
        {
            return this.vertScroll;
        }
        set
        {
            this.vertScroll = value;
            this.updateScrollbars();
        }
    }

    public dfControlOrientation WheelScrollDirection
    {
        get
        {
            return this.wheelDirection;
        }
        set
        {
            this.wheelDirection = value;
        }
    }

    public bool WrapLayout
    {
        get
        {
            return this.wrapLayout;
        }
        set
        {
            if (value != this.wrapLayout)
            {
                this.wrapLayout = value;
                this.Reset();
            }
        }
    }

    public enum LayoutDirection
    {
        Horizontal,
        Vertical
    }
}

