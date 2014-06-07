using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Progress Bar"), RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class dfProgressBar : dfControl
{
    [SerializeField]
    protected bool actAsSlider;
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected dfProgressFillMode fillMode;
    [SerializeField]
    protected float maxValue = 1f;
    [SerializeField]
    protected float minValue;
    [SerializeField]
    protected RectOffset padding = new RectOffset();
    [SerializeField]
    protected Color32 progressColor = Color.white;
    [SerializeField]
    protected string progressSprite;
    [SerializeField]
    protected float rawValue = 0.25f;

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
        Vector3 vector2 = new Vector3(vector.x + this.padding.left, vector.y - (this.size.y * 0.5f));
        Vector3 vector3 = vector2 + new Vector3(this.size.x - this.padding.right, 0f);
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
        return (this.minValue + ((this.maxValue - this.minValue) * num2));
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        try
        {
            if (this.actAsSlider)
            {
                float num = (this.maxValue - this.minValue) * 0.1f;
                if (args.KeyCode == KeyCode.LeftArrow)
                {
                    this.Value -= num;
                    args.Use();
                }
                else if (args.KeyCode == KeyCode.RightArrow)
                {
                    this.Value += num;
                    args.Use();
                }
            }
        }
        finally
        {
            base.OnKeyDown(args);
        }
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        try
        {
            if (this.actAsSlider && args.Buttons.IsSet(dfMouseButtons.Left))
            {
                base.Focus();
                this.Value = this.getValueFromMouseEvent(args);
                args.Use();
            }
        }
        finally
        {
            base.OnMouseDown(args);
        }
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        try
        {
            if (this.actAsSlider && args.Buttons.IsSet(dfMouseButtons.Left))
            {
                this.Value = this.getValueFromMouseEvent(args);
                args.Use();
            }
        }
        finally
        {
            base.OnMouseMove(args);
        }
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
        try
        {
            if (this.actAsSlider)
            {
                float num = (this.maxValue - this.minValue) * 0.1f;
                this.Value += num * Mathf.RoundToInt(-args.WheelDelta);
                args.Use();
            }
        }
        finally
        {
            base.OnMouseWheel(args);
        }
    }

    protected override void OnRebuildRenderData()
    {
        if (this.Atlas != null)
        {
            base.renderData.Material = this.Atlas.Material;
            this.renderBackground();
            this.renderProgressFill();
        }
    }

    protected internal virtual void OnValueChanged()
    {
        this.Invalidate();
        object[] args = new object[] { this.Value };
        base.SignalHierarchy("OnValueChanged", args);
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, this.Value);
        }
    }

    private void renderBackground()
    {
        if (this.Atlas != null)
        {
            dfAtlas.ItemInfo info = this.Atlas[this.backgroundSprite];
            if (info != null)
            {
                Color32 color = base.ApplyOpacity(!base.IsEnabled ? base.DisabledColor : base.Color);
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

    private void renderProgressFill()
    {
        if (this.Atlas != null)
        {
            dfAtlas.ItemInfo info = this.Atlas[this.progressSprite];
            if (info != null)
            {
                Vector3 vector = new Vector3((float) this.padding.left, (float) -this.padding.top);
                Vector2 vector2 = new Vector2(this.size.x - this.padding.horizontal, this.size.y - this.padding.vertical);
                float num = 1f;
                float num2 = this.maxValue - this.minValue;
                float num3 = (this.rawValue - this.minValue) / num2;
                dfProgressFillMode fillMode = this.fillMode;
                if ((fillMode == dfProgressFillMode.Stretch) && ((vector2.x * num3) < info.border.horizontal))
                {
                }
                if (fillMode == dfProgressFillMode.Fill)
                {
                    num = num3;
                }
                else
                {
                    vector2.x = Mathf.Max((float) info.border.horizontal, vector2.x * num3);
                }
                Color32 color = base.ApplyOpacity(!base.IsEnabled ? base.DisabledColor : this.ProgressColor);
                dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                    atlas = this.atlas,
                    color = color,
                    fillAmount = num,
                    offset = base.pivot.TransformToUpperLeft(base.Size) + vector,
                    pixelsToUnits = base.PixelsToUnits(),
                    size = vector2,
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

    private void setDefaultSize(string spriteName)
    {
        if (this.Atlas != null)
        {
            dfAtlas.ItemInfo info = this.Atlas[spriteName];
            if ((base.size == Vector2.zero) && (info != null))
            {
                base.Size = info.sizeInPixels;
            }
        }
    }

    public bool ActAsSlider
    {
        get
        {
            return this.actAsSlider;
        }
        set
        {
            this.actAsSlider = value;
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
                this.setDefaultSize(value);
                this.Invalidate();
            }
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

    public RectOffset Padding
    {
        get
        {
            if (this.padding == null)
            {
                this.padding = new RectOffset();
            }
            return this.padding;
        }
        set
        {
            if (!object.Equals(value, this.padding))
            {
                this.padding = value;
                this.Invalidate();
            }
        }
    }

    public Color32 ProgressColor
    {
        get
        {
            return this.progressColor;
        }
        set
        {
            if (!object.Equals(value, this.progressColor))
            {
                this.progressColor = value;
                this.Invalidate();
            }
        }
    }

    public string ProgressSprite
    {
        get
        {
            return this.progressSprite;
        }
        set
        {
            if (value != this.progressSprite)
            {
                this.progressSprite = value;
                this.Invalidate();
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
            value = Mathf.Max(this.minValue, Mathf.Min(this.maxValue, value));
            if (!Mathf.Approximately(value, this.rawValue))
            {
                this.rawValue = value;
                this.OnValueChanged();
            }
        }
    }
}

