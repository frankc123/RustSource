using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Scrollbar"), RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class dfScrollbar : dfControl
{
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected bool autoHide;
    [SerializeField]
    protected dfControl decButton;
    [SerializeField]
    protected dfControl incButton;
    [SerializeField]
    protected float increment = 1f;
    [SerializeField]
    protected float maxValue = 100f;
    [SerializeField]
    protected float minValue;
    [SerializeField]
    protected dfControlOrientation orientation;
    [SerializeField]
    protected float rawValue = 1f;
    [SerializeField]
    protected float scrollSize = 1f;
    [SerializeField]
    protected float stepSize = 1f;
    [SerializeField]
    protected dfControl thumb;
    private Vector3 thumbMouseOffset = Vector3.zero;
    [SerializeField]
    protected RectOffset thumbPadding = new RectOffset();
    [SerializeField]
    protected dfControl track;

    public event PropertyChangedEventHandler<float> ValueChanged;

    private float adjustValue(float value)
    {
        float a = Mathf.Max((float) (Mathf.Max((float) (this.maxValue - this.minValue), (float) 0f) - this.scrollSize), (float) 0f) + this.minValue;
        return Mathf.Max(Mathf.Min(a, value), this.minValue).Quantize(this.stepSize);
    }

    private void attachEvents()
    {
        if (Application.isPlaying)
        {
            if (this.IncButton != null)
            {
                this.IncButton.MouseDown += new MouseEventHandler(this.incrementPressed);
                this.IncButton.MouseHover += new MouseEventHandler(this.incrementPressed);
            }
            if (this.DecButton != null)
            {
                this.DecButton.MouseDown += new MouseEventHandler(this.decrementPressed);
                this.DecButton.MouseHover += new MouseEventHandler(this.decrementPressed);
            }
        }
    }

    public override Vector2 CalculateMinimumSize()
    {
        Vector2[] vectorArray = new Vector2[3];
        if (this.decButton != null)
        {
            vectorArray[0] = this.decButton.CalculateMinimumSize();
        }
        if (this.incButton != null)
        {
            vectorArray[1] = this.incButton.CalculateMinimumSize();
        }
        if (this.thumb != null)
        {
            vectorArray[2] = this.thumb.CalculateMinimumSize();
        }
        Vector2 zero = Vector2.zero;
        if (this.orientation == dfControlOrientation.Horizontal)
        {
            zero.x = (vectorArray[0].x + vectorArray[1].x) + vectorArray[2].x;
            float[] values = new float[] { vectorArray[0].y, vectorArray[1].y, vectorArray[2].y };
            zero.y = Mathf.Max(values);
        }
        else
        {
            float[] singleArray2 = new float[] { vectorArray[0].x, vectorArray[1].x, vectorArray[2].x };
            zero.x = Mathf.Max(singleArray2);
            zero.y = (vectorArray[0].y + vectorArray[1].y) + vectorArray[2].y;
        }
        return Vector2.Max(zero, base.CalculateMinimumSize());
    }

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

    private void decrementPressed(dfControl sender, dfMouseEventArgs args)
    {
        if (args.Buttons.IsSet(dfMouseButtons.Left))
        {
            this.Value -= this.IncrementAmount;
            args.Use();
        }
    }

    private void detachEvents()
    {
        if (Application.isPlaying)
        {
            if (this.IncButton != null)
            {
                this.IncButton.MouseDown -= new MouseEventHandler(this.incrementPressed);
                this.IncButton.MouseHover -= new MouseEventHandler(this.incrementPressed);
            }
            if (this.DecButton != null)
            {
                this.DecButton.MouseDown -= new MouseEventHandler(this.decrementPressed);
                this.DecButton.MouseHover -= new MouseEventHandler(this.decrementPressed);
            }
        }
    }

    private void doAutoHide()
    {
        if (this.autoHide && Application.isPlaying)
        {
            if (Mathf.CeilToInt(this.ScrollSize) >= Mathf.CeilToInt(this.maxValue - this.minValue))
            {
                base.Hide();
            }
            else
            {
                base.Show();
            }
        }
    }

    private float getValueFromMouseEvent(dfMouseEventArgs args)
    {
        Vector3[] corners = this.track.GetCorners();
        Vector3 inPoint = corners[0];
        Vector3 end = corners[(this.orientation != dfControlOrientation.Horizontal) ? 2 : 1];
        Plane plane = new Plane(base.transform.TransformDirection(Vector3.back), inPoint);
        Ray ray = args.Ray;
        float enter = 0f;
        if (!plane.Raycast(ray, out enter))
        {
            return this.rawValue;
        }
        Vector3 test = ray.origin + ((Vector3) (ray.direction * enter));
        if (args.Source == this.thumb)
        {
            test += this.thumbMouseOffset;
        }
        Vector3 vector5 = closestPoint(inPoint, end, test, true) - inPoint;
        Vector3 vector6 = end - inPoint;
        float num2 = vector5.magnitude / vector6.magnitude;
        return (this.minValue + ((this.maxValue - this.minValue) * num2));
    }

    private void incrementPressed(dfControl sender, dfMouseEventArgs args)
    {
        if (args.Buttons.IsSet(dfMouseButtons.Left))
        {
            this.Value += this.IncrementAmount;
            args.Use();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        this.detachEvents();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.detachEvents();
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        if (this.Orientation == dfControlOrientation.Horizontal)
        {
            if (args.KeyCode == KeyCode.LeftArrow)
            {
                this.Value -= this.IncrementAmount;
                args.Use();
                return;
            }
            if (args.KeyCode == KeyCode.RightArrow)
            {
                this.Value += this.IncrementAmount;
                args.Use();
                return;
            }
        }
        else
        {
            if (args.KeyCode == KeyCode.UpArrow)
            {
                this.Value -= this.IncrementAmount;
                args.Use();
                return;
            }
            if (args.KeyCode == KeyCode.DownArrow)
            {
                this.Value += this.IncrementAmount;
                args.Use();
                return;
            }
        }
        base.OnKeyDown(args);
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        if (args.Buttons.IsSet(dfMouseButtons.Left))
        {
            base.Focus();
        }
        if ((args.Source != this.incButton) && (args.Source != this.decButton))
        {
            if (((args.Source != this.track) && (args.Source != this.thumb)) || !args.Buttons.IsSet(dfMouseButtons.Left))
            {
                base.OnMouseDown(args);
            }
            else
            {
                if (args.Source == this.thumb)
                {
                    RaycastHit hit;
                    this.thumb.collider.Raycast(args.Ray, out hit, 1000f);
                    Vector3 vector = this.thumb.transform.position + this.thumb.Pivot.TransformToCenter(((Vector2) (this.thumb.Size * base.PixelsToUnits())));
                    this.thumbMouseOffset = vector - hit.point;
                }
                else
                {
                    this.updateFromTrackClick(args);
                }
                args.Use();
                object[] objArray1 = new object[] { args };
                base.Signal("OnMouseDown", objArray1);
            }
        }
    }

    protected internal override void OnMouseHover(dfMouseEventArgs args)
    {
        if (((args.Source != this.incButton) && (args.Source != this.decButton)) && (args.Source != this.thumb))
        {
            if ((args.Source != this.track) || !args.Buttons.IsSet(dfMouseButtons.Left))
            {
                base.OnMouseHover(args);
            }
            else
            {
                this.updateFromTrackClick(args);
                args.Use();
                object[] objArray1 = new object[] { args };
                base.Signal("OnMouseHover", objArray1);
            }
        }
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        if ((args.Source != this.incButton) && (args.Source != this.decButton))
        {
            if (((args.Source != this.track) && (args.Source != this.thumb)) || !args.Buttons.IsSet(dfMouseButtons.Left))
            {
                base.OnMouseMove(args);
            }
            else
            {
                this.Value = Mathf.Max(this.minValue, this.getValueFromMouseEvent(args) - (this.scrollSize * 0.5f));
                args.Use();
                object[] objArray1 = new object[] { args };
                base.Signal("OnMouseMove", objArray1);
            }
        }
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
        this.Value += this.IncrementAmount * -args.WheelDelta;
        args.Use();
        object[] objArray1 = new object[] { args };
        base.Signal("OnMouseWheel", objArray1);
    }

    protected override void OnRebuildRenderData()
    {
        this.updateThumb(this.rawValue);
        base.OnRebuildRenderData();
    }

    protected internal override void OnSizeChanged()
    {
        base.OnSizeChanged();
        this.updateThumb(this.rawValue);
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

    public override void Start()
    {
        base.Start();
        this.attachEvents();
    }

    private void updateFromTrackClick(dfMouseEventArgs args)
    {
        float num = this.getValueFromMouseEvent(args);
        if (num > (this.rawValue + this.scrollSize))
        {
            this.Value += this.scrollSize;
        }
        else if (num < this.rawValue)
        {
            this.Value -= this.scrollSize;
        }
    }

    private void updateThumb(float rawValue)
    {
        if (((base.controls.Count != 0) && (this.thumb != null)) && ((this.track != null) && base.IsVisible))
        {
            float num = this.maxValue - this.minValue;
            if ((num <= 0f) || (num <= this.scrollSize))
            {
                this.thumb.IsVisible = false;
            }
            else
            {
                this.thumb.IsVisible = true;
                float num2 = (this.orientation != dfControlOrientation.Horizontal) ? this.track.Height : this.track.Width;
                float y = (this.orientation != dfControlOrientation.Horizontal) ? Mathf.Max((this.scrollSize / num) * num2, this.thumb.MinimumSize.y) : Mathf.Max((this.scrollSize / num) * num2, this.thumb.MinimumSize.x);
                Vector2 vector = (this.orientation != dfControlOrientation.Horizontal) ? new Vector2(this.thumb.Width, y) : new Vector2(y, this.thumb.Height);
                if (this.Orientation == dfControlOrientation.Horizontal)
                {
                    vector.x -= this.thumbPadding.horizontal;
                }
                else
                {
                    vector.y -= this.thumbPadding.vertical;
                }
                this.thumb.Size = vector;
                float num4 = (rawValue - this.minValue) / (num - this.scrollSize);
                float num5 = num4 * (num2 - y);
                Vector3 vector2 = (this.orientation != dfControlOrientation.Horizontal) ? Vector3.up : Vector3.right;
                Vector3 vector3 = (this.Orientation != dfControlOrientation.Horizontal) ? new Vector3((this.track.Width - this.thumb.Width) * 0.5f, 0f) : new Vector3(0f, (this.track.Height - this.thumb.Height) * 0.5f);
                if (this.Orientation == dfControlOrientation.Horizontal)
                {
                    vector3.x = this.thumbPadding.left;
                }
                else
                {
                    vector3.y = this.thumbPadding.top;
                }
                if (this.thumb.Parent == this)
                {
                    this.thumb.RelativePosition = (this.track.RelativePosition + vector3) + ((Vector3) (vector2 * num5));
                }
                else
                {
                    this.thumb.RelativePosition = ((Vector3) (vector2 * num5)) + vector3;
                }
            }
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

    public bool AutoHide
    {
        get
        {
            return this.autoHide;
        }
        set
        {
            if (value != this.autoHide)
            {
                this.autoHide = value;
                this.Invalidate();
                this.doAutoHide();
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

    public dfControl DecButton
    {
        get
        {
            return this.decButton;
        }
        set
        {
            if (value != this.decButton)
            {
                this.decButton = value;
                this.Invalidate();
            }
        }
    }

    public dfControl IncButton
    {
        get
        {
            return this.incButton;
        }
        set
        {
            if (value != this.incButton)
            {
                this.incButton = value;
                this.Invalidate();
            }
        }
    }

    public float IncrementAmount
    {
        get
        {
            return this.increment;
        }
        set
        {
            value = Mathf.Max(0f, value);
            if (!Mathf.Approximately(value, this.increment))
            {
                this.increment = value;
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
                this.Value = this.Value;
                this.Invalidate();
                this.doAutoHide();
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
                this.Value = this.Value;
                this.Invalidate();
                this.doAutoHide();
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
                this.Value = this.Value;
                this.Invalidate();
                this.doAutoHide();
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
                this.Value = this.Value;
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
            }
        }
    }

    public RectOffset ThumbPadding
    {
        get
        {
            if (this.thumbPadding == null)
            {
                this.thumbPadding = new RectOffset();
            }
            return this.thumbPadding;
        }
        set
        {
            int num;
            if (this.orientation == dfControlOrientation.Horizontal)
            {
                num = 0;
                value.bottom = num;
                value.top = num;
            }
            else
            {
                num = 0;
                value.right = num;
                value.left = num;
            }
            if (!object.Equals(value, this.thumbPadding))
            {
                this.thumbPadding = value;
                this.updateThumb(this.rawValue);
            }
        }
    }

    public dfControl Track
    {
        get
        {
            return this.track;
        }
        set
        {
            if (value != this.track)
            {
                this.track = value;
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
            value = this.adjustValue(value);
            if (!Mathf.Approximately(value, this.rawValue))
            {
                this.rawValue = value;
                this.OnValueChanged();
            }
            this.updateThumb(this.rawValue);
        }
    }
}

