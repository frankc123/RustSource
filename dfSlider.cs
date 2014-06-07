using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Slider")]
public class dfSlider : dfControl
{
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected dfControl fillIndicator;
    [SerializeField]
    protected dfProgressFillMode fillMode = dfProgressFillMode.Fill;
    [SerializeField]
    protected RectOffset fillPadding = new RectOffset();
    [SerializeField]
    protected float maxValue = 100f;
    [SerializeField]
    protected float minValue;
    [SerializeField]
    protected dfControlOrientation orientation;
    [SerializeField]
    protected float rawValue = 10f;
    [SerializeField]
    protected bool rightToLeft;
    [SerializeField]
    protected float scrollSize = 1f;
    [SerializeField]
    protected float stepSize = 1f;
    [SerializeField]
    protected dfControl thumb;
    [SerializeField]
    protected Vector2 thumbOffset = Vector2.zero;

    public event PropertyChangedEventHandler<float> ValueChanged;

    private static Vector3 closestPoint(Vector3 start, Vector3 end, Vector3 test, bool clamp)
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

    private Vector3[] getEndPoints(bool convertToWorld = false)
    {
        Vector3 vector = base.pivot.TransformToUpperLeft(base.Size);
        Vector3 vector2 = new Vector3(vector.x, vector.y - (this.size.y * 0.5f));
        Vector3 vector3 = vector2 + new Vector3(this.size.x, 0f);
        if (this.orientation == dfControlOrientation.Vertical)
        {
            vector2 = new Vector3(vector.x + (this.size.x * 0.5f), vector.y);
            vector3 = vector2 - new Vector3(0f, this.size.y);
        }
        if (convertToWorld)
        {
            float num = base.PixelsToUnits();
            Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
            vector2 = localToWorldMatrix.MultiplyPoint((Vector3) (vector2 * num));
            vector3 = localToWorldMatrix.MultiplyPoint((Vector3) (vector3 * num));
        }
        return new Vector3[] { vector2, vector3 };
    }

    private float getValueFromMouseEvent(dfMouseEventArgs args)
    {
        Vector3[] vectorArray = this.getEndPoints(true);
        Vector3 inPoint = vectorArray[0];
        Vector3 end = vectorArray[1];
        Plane plane = new Plane(base.transform.TransformDirection(Vector3.back), inPoint);
        Ray ray = args.Ray;
        float enter = 0f;
        if (!plane.Raycast(ray, out enter))
        {
            return this.rawValue;
        }
        Vector3 test = ray.origin + ((Vector3) (ray.direction * enter));
        Vector3 vector5 = closestPoint(inPoint, end, test, true) - inPoint;
        Vector3 vector6 = end - inPoint;
        float num2 = vector5.magnitude / vector6.magnitude;
        float num3 = this.minValue + ((this.maxValue - this.minValue) * num2);
        if ((this.orientation != dfControlOrientation.Vertical) && !this.rightToLeft)
        {
            return num3;
        }
        return (this.maxValue - num3);
    }

    public override void OnEnable()
    {
        if (this.size.magnitude < float.Epsilon)
        {
            base.size = new Vector2(100f, 25f);
        }
        base.OnEnable();
        this.updateValueIndicators(this.rawValue);
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        if (this.Orientation == dfControlOrientation.Horizontal)
        {
            if (args.KeyCode == KeyCode.LeftArrow)
            {
                this.Value -= this.ScrollSize;
                args.Use();
                return;
            }
            if (args.KeyCode == KeyCode.RightArrow)
            {
                this.Value += this.ScrollSize;
                args.Use();
                return;
            }
        }
        else
        {
            if (args.KeyCode == KeyCode.UpArrow)
            {
                this.Value -= this.ScrollSize;
                args.Use();
                return;
            }
            if (args.KeyCode == KeyCode.DownArrow)
            {
                this.Value += this.ScrollSize;
                args.Use();
                return;
            }
        }
        base.OnKeyDown(args);
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        if (!args.Buttons.IsSet(dfMouseButtons.Left))
        {
            base.OnMouseMove(args);
        }
        else
        {
            base.Focus();
            this.Value = this.getValueFromMouseEvent(args);
            args.Use();
            object[] objArray1 = new object[] { args };
            base.Signal("OnMouseDown", objArray1);
            object[] objArray2 = new object[] { this, args };
            base.RaiseEvent("MouseDown", objArray2);
        }
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        if (!args.Buttons.IsSet(dfMouseButtons.Left))
        {
            base.OnMouseMove(args);
        }
        else
        {
            this.Value = this.getValueFromMouseEvent(args);
            args.Use();
            object[] objArray1 = new object[] { args };
            base.Signal("OnMouseMove", objArray1);
            object[] objArray2 = new object[] { this, args };
            base.RaiseEvent("MouseMove", objArray2);
        }
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
        int num = (this.orientation != dfControlOrientation.Horizontal) ? 1 : -1;
        this.Value += (this.scrollSize * args.WheelDelta) * num;
        args.Use();
        object[] objArray1 = new object[] { args };
        base.Signal("OnMouseWheel", objArray1);
        object[] objArray2 = new object[] { this, args };
        base.RaiseEvent("MouseWheel", objArray2);
    }

    protected override void OnRebuildRenderData()
    {
        if (this.Atlas != null)
        {
            base.renderData.Material = this.Atlas.Material;
            this.renderBackground();
        }
    }

    protected internal override void OnSizeChanged()
    {
        base.OnSizeChanged();
        this.updateValueIndicators(this.rawValue);
    }

    protected internal virtual void OnValueChanged()
    {
        this.Invalidate();
        this.updateValueIndicators(this.rawValue);
        object[] args = new object[] { this.Value };
        base.SignalHierarchy("OnValueChanged", args);
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, this.Value);
        }
    }

    protected internal virtual void renderBackground()
    {
        if (this.Atlas != null)
        {
            dfAtlas.ItemInfo info = this.Atlas[this.backgroundSprite];
            if (info != null)
            {
                Color32 color = base.ApplyOpacity(!base.IsEnabled ? base.disabledColor : base.color);
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

    public override void Start()
    {
        base.Start();
        this.updateValueIndicators(this.rawValue);
    }

    private void updateValueIndicators(float rawValue)
    {
        if (this.thumb != null)
        {
            Vector3[] vectorArray = this.getEndPoints(true);
            Vector3 vector = vectorArray[1] - vectorArray[0];
            float num = this.maxValue - this.minValue;
            float num2 = ((rawValue - this.minValue) / num) * vector.magnitude;
            Vector3 vector2 = (Vector3) (this.thumbOffset * base.PixelsToUnits());
            Vector3 vector3 = (vectorArray[0] + ((Vector3) (vector.normalized * num2))) + vector2;
            if ((this.orientation == dfControlOrientation.Vertical) || this.rightToLeft)
            {
                vector3 = (vectorArray[1] + ((Vector3) (-vector.normalized * num2))) + vector2;
            }
            this.thumb.Pivot = dfPivotPoint.MiddleCenter;
            this.thumb.transform.position = vector3;
        }
        if (this.fillIndicator != null)
        {
            RectOffset fillPadding = this.FillPadding;
            float num3 = (rawValue - this.minValue) / (this.maxValue - this.minValue);
            Vector3 vector4 = new Vector3((float) fillPadding.left, (float) fillPadding.top);
            Vector2 vector5 = base.size - new Vector2((float) fillPadding.horizontal, (float) fillPadding.vertical);
            dfSprite fillIndicator = this.fillIndicator as dfSprite;
            if ((fillIndicator != null) && (this.fillMode == dfProgressFillMode.Fill))
            {
                fillIndicator.FillAmount = num3;
            }
            else if (this.orientation == dfControlOrientation.Horizontal)
            {
                vector5.x = (base.Width * num3) - fillPadding.horizontal;
            }
            else
            {
                vector5.y = (base.Height * num3) - fillPadding.vertical;
            }
            this.fillIndicator.Size = vector5;
            this.fillIndicator.RelativePosition = vector4;
        }
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

    public dfProgressFillMode FillMode
    {
        get
        {
            return this.fillMode;
        }
        set
        {
            if (value != this.fillMode)
            {
                this.fillMode = value;
                this.Invalidate();
            }
        }
    }

    public RectOffset FillPadding
    {
        get
        {
            if (this.fillPadding == null)
            {
                this.fillPadding = new RectOffset();
            }
            return this.fillPadding;
        }
        set
        {
            if (!object.Equals(value, this.fillPadding))
            {
                this.fillPadding = value;
                this.updateValueIndicators(this.rawValue);
                this.Invalidate();
            }
        }
    }

    public float MaxValue
    {
        get
        {
            return this.maxValue;
        }
        set
        {
            if (value != this.maxValue)
            {
                this.maxValue = value;
                if (this.rawValue > value)
                {
                    this.Value = value;
                }
                this.Invalidate();
            }
        }
    }

    public float MinValue
    {
        get
        {
            return this.minValue;
        }
        set
        {
            if (value != this.minValue)
            {
                this.minValue = value;
                if (this.rawValue < value)
                {
                    this.Value = value;
                }
                this.Invalidate();
            }
        }
    }

    public dfControlOrientation Orientation
    {
        get
        {
            return this.orientation;
        }
        set
        {
            if (value != this.orientation)
            {
                this.orientation = value;
                this.Invalidate();
                this.updateValueIndicators(this.rawValue);
            }
        }
    }

    public dfControl Progress
    {
        get
        {
            return this.fillIndicator;
        }
        set
        {
            if (value != this.fillIndicator)
            {
                this.fillIndicator = value;
                this.Invalidate();
                this.updateValueIndicators(this.rawValue);
            }
        }
    }

    public bool RightToLeft
    {
        get
        {
            return this.rightToLeft;
        }
        set
        {
            if (value != this.rightToLeft)
            {
                this.rightToLeft = value;
                this.updateValueIndicators(this.rawValue);
            }
        }
    }

    public float ScrollSize
    {
        get
        {
            return this.scrollSize;
        }
        set
        {
            value = Mathf.Max(0f, value);
            if (value != this.scrollSize)
            {
                this.scrollSize = value;
                this.Invalidate();
            }
        }
    }

    public float StepSize
    {
        get
        {
            return this.stepSize;
        }
        set
        {
            value = Mathf.Max(0f, value);
            if (value != this.stepSize)
            {
                this.stepSize = value;
                this.Value = this.rawValue.Quantize(value);
                this.Invalidate();
            }
        }
    }

    public dfControl Thumb
    {
        get
        {
            return this.thumb;
        }
        set
        {
            if (value != this.thumb)
            {
                this.thumb = value;
                this.Invalidate();
                this.updateValueIndicators(this.rawValue);
            }
        }
    }

    public Vector2 ThumbOffset
    {
        get
        {
            return this.thumbOffset;
        }
        set
        {
            if (Vector2.Distance(value, this.thumbOffset) > float.Epsilon)
            {
                this.thumbOffset = value;
                this.updateValueIndicators(this.rawValue);
            }
        }
    }

    public float Value
    {
        get
        {
            return this.rawValue;
        }
        set
        {
            value = Mathf.Max(this.minValue, Mathf.Min(this.maxValue, value)).Quantize(this.stepSize);
            if (!Mathf.Approximately(value, this.rawValue))
            {
                this.rawValue = value;
                this.OnValueChanged();
            }
        }
    }
}

