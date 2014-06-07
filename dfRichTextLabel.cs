using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Rich Text Label"), ExecuteInEditMode, RequireComponent(typeof(BoxCollider))]
public class dfRichTextLabel : dfControl, IDFMultiRender
{
    [SerializeField]
    protected dfMarkupTextAlign align;
    [SerializeField]
    protected bool allowScrolling;
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string blankTextureSprite;
    private dfList<dfRenderData> buffers = new dfList<dfRenderData>();
    private static dfRenderData clipBuffer = new dfRenderData(0x20);
    private dfList<dfMarkupElement> elements;
    [SerializeField]
    protected dfDynamicFont font;
    [SerializeField]
    protected int fontSize = 0x10;
    [SerializeField]
    protected UnityEngine.FontStyle fontStyle;
    [SerializeField]
    protected dfScrollbar horzScrollbar;
    private bool initialized;
    private bool isMarkupInvalidated = true;
    private bool isMouseDown;
    [SerializeField]
    protected int lineheight = 0x10;
    private Vector2 mouseDownScrollPosition = Vector2.zero;
    private dfMarkupTag mouseDownTag;
    [SerializeField]
    protected bool preserveWhitespace;
    private Vector2 scrollMomentum = Vector2.zero;
    private Vector2 scrollPosition = Vector2.zero;
    private Vector2 startSize = Vector2.zero;
    [SerializeField]
    protected string text = "Rich Text Label";
    [SerializeField]
    protected dfTextScaleMode textScaleMode;
    private Vector2 touchStartPosition = Vector2.zero;
    [SerializeField]
    protected bool useScrollMomentum;
    [SerializeField]
    protected dfScrollbar vertScrollbar;
    private dfMarkupBox viewportBox;

    public event LinkClickEventHandler LinkClicked;

    public event PropertyChangedEventHandler<Vector2> ScrollPositionChanged;

    public event PropertyChangedEventHandler<string> TextChanged;

    public override void Awake()
    {
        base.Awake();
        this.startSize = base.Size;
    }

    private void clipToViewport(dfRenderData renderData)
    {
        Plane[] clippingPlanes = this.GetClippingPlanes();
        Material material = renderData.Material;
        Matrix4x4 transform = renderData.Transform;
        clipBuffer.Clear();
        dfClippingUtil.Clip(clippingPlanes, renderData, clipBuffer);
        renderData.Clear();
        renderData.Merge(clipBuffer, false);
        renderData.Material = material;
        renderData.Transform = transform;
    }

    private void gatherRenderBuffers(dfMarkupBox box, dfList<dfRenderData> buffers)
    {
        dfIntersectionType type = this.getViewportIntersection(box);
        if (type != dfIntersectionType.None)
        {
            dfRenderData renderData = box.Render();
            if (renderData != null)
            {
                if ((renderData.Material == null) && (this.atlas != null))
                {
                    renderData.Material = this.atlas.Material;
                }
                float num = base.PixelsToUnits();
                Vector2 vector = -this.scrollPosition.Scale(1f, -1f).RoundToInt();
                Vector3 vector2 = ((Vector3) (vector + box.GetOffset().Scale(1f, -1f))) + base.pivot.TransformToUpperLeft(base.Size);
                dfList<Vector3> vertices = renderData.Vertices;
                Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
                for (int j = 0; j < renderData.Vertices.Count; j++)
                {
                    vertices[j] = localToWorldMatrix.MultiplyPoint((Vector3) ((vector2 + vertices[j]) * num));
                }
                if (type == dfIntersectionType.Intersecting)
                {
                    this.clipToViewport(renderData);
                }
                buffers.Add(renderData);
            }
            for (int i = 0; i < box.Children.Count; i++)
            {
                this.gatherRenderBuffers(box.Children[i], buffers);
            }
        }
    }

    private float getTextScaleMultiplier()
    {
        if ((this.textScaleMode == dfTextScaleMode.None) || !Application.isPlaying)
        {
            return 1f;
        }
        if (this.textScaleMode == dfTextScaleMode.ScreenResolution)
        {
            return (((float) Screen.height) / ((float) base.manager.FixedHeight));
        }
        return (base.Size.y / this.startSize.y);
    }

    private dfIntersectionType getViewportIntersection(dfMarkupBox box)
    {
        if (box.Display == dfMarkupDisplayType.none)
        {
            return dfIntersectionType.None;
        }
        Vector2 size = base.Size;
        Vector2 vector2 = box.GetOffset() - this.scrollPosition;
        Vector2 vector3 = vector2 + box.Size;
        if ((vector3.x <= 0f) || (vector3.y <= 0f))
        {
            return dfIntersectionType.None;
        }
        if ((vector2.x >= size.x) || (vector2.y >= size.y))
        {
            return dfIntersectionType.None;
        }
        if (((vector2.x >= 0f) && (vector2.y >= 0f)) && ((vector3.x <= size.x) && (vector3.y <= size.y)))
        {
            return dfIntersectionType.Inside;
        }
        return dfIntersectionType.Intersecting;
    }

    private dfMarkupTag hitTestTag(dfMouseEventArgs args)
    {
        Vector2 point = base.GetHitPosition(args) + this.scrollPosition;
        dfMarkupBox box = this.viewportBox.HitTest(point);
        if (box == null)
        {
            return null;
        }
        dfMarkupElement parent = box.Element;
        while ((parent != null) && !(parent is dfMarkupTag))
        {
            parent = parent.Parent;
        }
        return (parent as dfMarkupTag);
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
                if (this.horzScrollbar != null)
                {
                    this.horzScrollbar.ValueChanged += new PropertyChangedEventHandler<float>(this.horzScroll_ValueChanged);
                }
                if (this.vertScrollbar != null)
                {
                    this.vertScrollbar.ValueChanged += new PropertyChangedEventHandler<float>(this.vertScroll_ValueChanged);
                }
            }
            this.Invalidate();
            this.ScrollPosition = Vector2.zero;
            this.updateScrollbars();
        }
    }

    public override void Invalidate()
    {
        base.Invalidate();
        this.isMarkupInvalidated = true;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        this.initialize();
    }

    internal override void OnDragEnd(dfDragEventArgs args)
    {
        base.OnDragEnd(args);
        this.isMouseDown = false;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (this.size.sqrMagnitude <= float.Epsilon)
        {
            base.Size = new Vector2(320f, 200f);
            int num = 0x10;
            this.LineHeight = num;
            this.FontSize = num;
        }
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        if (args.Used)
        {
            base.OnKeyDown(args);
        }
        else
        {
            int fontSize = this.FontSize;
            int num2 = this.FontSize;
            if (args.KeyCode == KeyCode.LeftArrow)
            {
                this.ScrollPosition += new Vector2((float) -fontSize, 0f);
                args.Use();
            }
            else if (args.KeyCode == KeyCode.RightArrow)
            {
                this.ScrollPosition += new Vector2((float) fontSize, 0f);
                args.Use();
            }
            else if (args.KeyCode == KeyCode.UpArrow)
            {
                this.ScrollPosition += new Vector2(0f, (float) -num2);
                args.Use();
            }
            else if (args.KeyCode == KeyCode.DownArrow)
            {
                this.ScrollPosition += new Vector2(0f, (float) num2);
                args.Use();
            }
            base.OnKeyDown(args);
        }
    }

    protected internal override void OnLocalize()
    {
        base.OnLocalize();
        this.Text = base.getLocalizedValue(this.text);
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        base.OnMouseDown(args);
        this.mouseDownTag = this.hitTestTag(args);
        this.mouseDownScrollPosition = this.scrollPosition;
        this.scrollMomentum = Vector2.zero;
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
        base.OnMouseMove(args);
        if (this.allowScrolling && ((args is dfTouchEventArgs) || this.isMouseDown))
        {
            Vector2 vector2 = args.Position - this.touchStartPosition;
            if (vector2.magnitude > 5f)
            {
                Vector2 vector = args.MoveDelta.Scale(-1f, 1f);
                this.ScrollPosition += vector;
                this.scrollMomentum = (Vector2) ((this.scrollMomentum + vector) * 0.5f);
            }
        }
    }

    protected internal override void OnMouseUp(dfMouseEventArgs args)
    {
        base.OnMouseUp(args);
        this.isMouseDown = false;
        if ((Vector2.Distance(this.scrollPosition, this.mouseDownScrollPosition) <= 2f) && (this.hitTestTag(args) == this.mouseDownTag))
        {
            dfMarkupTag mouseDownTag = this.mouseDownTag;
            while ((mouseDownTag != null) && !(mouseDownTag is dfMarkupTagAnchor))
            {
                mouseDownTag = mouseDownTag.Parent as dfMarkupTag;
            }
            if (mouseDownTag is dfMarkupTagAnchor)
            {
                object[] objArray1 = new object[] { mouseDownTag };
                base.Signal("OnLinkClicked", objArray1);
                if (this.LinkClicked != null)
                {
                    this.LinkClicked(this, mouseDownTag as dfMarkupTagAnchor);
                }
            }
        }
        this.mouseDownTag = null;
        this.mouseDownScrollPosition = this.scrollPosition;
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
        try
        {
            if (!args.Used && this.allowScrolling)
            {
                int num = !this.UseScrollMomentum ? 3 : 1;
                float num2 = (this.vertScrollbar == null) ? ((float) (this.FontSize * num)) : this.vertScrollbar.IncrementAmount;
                this.ScrollPosition = new Vector2(this.scrollPosition.x, this.scrollPosition.y - (num2 * args.WheelDelta));
                this.scrollMomentum = new Vector2(0f, -num2 * args.WheelDelta);
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

    protected internal void OnScrollPositionChanged()
    {
        base.Invalidate();
        object[] args = new object[] { this.ScrollPosition };
        base.SignalHierarchy("OnScrollPositionChanged", args);
        if (this.ScrollPositionChanged != null)
        {
            this.ScrollPositionChanged(this, this.ScrollPosition);
        }
    }

    protected internal void OnTextChanged()
    {
        this.Invalidate();
        object[] args = new object[] { this.text };
        base.Signal("OnTextChanged", args);
        if (this.TextChanged != null)
        {
            this.TextChanged(this, this.text);
        }
    }

    private void processMarkup()
    {
        this.releaseMarkupReferences();
        this.elements = dfMarkupParser.Parse(this, this.text);
        float num = this.getTextScaleMultiplier();
        int num2 = Mathf.CeilToInt(this.FontSize * num);
        int num3 = Mathf.CeilToInt(this.LineHeight * num);
        dfMarkupStyle style = new dfMarkupStyle {
            Host = this,
            Atlas = this.Atlas,
            Font = this.Font,
            FontSize = num2,
            FontStyle = this.FontStyle,
            LineHeight = num3,
            Color = (Color) base.ApplyOpacity(base.Color),
            Opacity = base.CalculateOpacity(),
            Align = this.TextAlignment,
            PreserveWhitespace = this.preserveWhitespace
        };
        dfMarkupBox box = new dfMarkupBox(null, dfMarkupDisplayType.block, style) {
            Size = base.Size
        };
        this.viewportBox = box;
        for (int i = 0; i < this.elements.Count; i++)
        {
            dfMarkupElement element = this.elements[i];
            if (element != null)
            {
                element.PerformLayout(this.viewportBox, style);
            }
        }
    }

    private void releaseMarkupReferences()
    {
        this.mouseDownTag = null;
        if (this.viewportBox != null)
        {
            this.viewportBox.Release();
        }
        if (this.elements != null)
        {
            for (int i = 0; i < this.elements.Count; i++)
            {
                this.elements[i].Release();
            }
            this.elements.Release();
        }
    }

    public dfList<dfRenderData> RenderMultiple()
    {
        dfList<dfRenderData> buffers;
        if (!base.isVisible || (this.Font == null))
        {
            return null;
        }
        if (!base.isControlInvalidated && (this.viewportBox != null))
        {
            this.buffers.Clear();
            this.gatherRenderBuffers(this.viewportBox, this.buffers);
            return this.buffers;
        }
        try
        {
            if (this.isMarkupInvalidated)
            {
                this.isMarkupInvalidated = false;
                this.processMarkup();
            }
            this.viewportBox.FitToContents(false);
            this.updateScrollbars();
            this.buffers.Clear();
            this.gatherRenderBuffers(this.viewportBox, this.buffers);
            buffers = this.buffers;
        }
        finally
        {
            base.isControlInvalidated = false;
        }
        return buffers;
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
        if ((this.useScrollMomentum && !this.isMouseDown) && (this.scrollMomentum.magnitude > 0.1f))
        {
            this.ScrollPosition += this.scrollMomentum;
            this.scrollMomentum = (Vector2) (this.scrollMomentum * (0.95f - Time.deltaTime));
        }
    }

    private void updateScrollbars()
    {
        if (this.horzScrollbar != null)
        {
            this.horzScrollbar.MinValue = 0f;
            this.horzScrollbar.MaxValue = this.ContentSize.x;
            this.horzScrollbar.ScrollSize = base.Size.x;
            this.horzScrollbar.Value = this.ScrollPosition.x;
        }
        if (this.vertScrollbar != null)
        {
            this.vertScrollbar.MinValue = 0f;
            this.vertScrollbar.MaxValue = this.ContentSize.y;
            this.vertScrollbar.ScrollSize = base.Size.y;
            this.vertScrollbar.Value = this.ScrollPosition.y;
        }
    }

    private void vertScroll_ValueChanged(dfControl control, float value)
    {
        this.ScrollPosition = new Vector2(this.scrollPosition.x, value);
    }

    public bool AllowScrolling
    {
        get
        {
            return this.allowScrolling;
        }
        set
        {
            this.allowScrolling = value;
            if (!value)
            {
                this.ScrollPosition = Vector2.zero;
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

    public string BlankTextureSprite
    {
        get
        {
            return this.blankTextureSprite;
        }
        set
        {
            if (value != this.blankTextureSprite)
            {
                this.blankTextureSprite = value;
                this.Invalidate();
            }
        }
    }

    public Vector2 ContentSize
    {
        get
        {
            if (this.viewportBox != null)
            {
                return this.viewportBox.Size;
            }
            return base.Size;
        }
    }

    public dfDynamicFont Font
    {
        get
        {
            return this.font;
        }
        set
        {
            if (value != this.font)
            {
                this.font = value;
                this.LineHeight = value.FontSize;
                this.Invalidate();
            }
        }
    }

    public int FontSize
    {
        get
        {
            return this.fontSize;
        }
        set
        {
            value = Mathf.Max(6, value);
            if (value != this.fontSize)
            {
                this.fontSize = value;
                this.Invalidate();
            }
            this.LineHeight = value;
        }
    }

    public UnityEngine.FontStyle FontStyle
    {
        get
        {
            return this.fontStyle;
        }
        set
        {
            if (value != this.fontStyle)
            {
                this.fontStyle = value;
                this.Invalidate();
            }
        }
    }

    public dfScrollbar HorizontalScrollbar
    {
        get
        {
            return this.horzScrollbar;
        }
        set
        {
            this.horzScrollbar = value;
            this.updateScrollbars();
        }
    }

    public int LineHeight
    {
        get
        {
            return this.lineheight;
        }
        set
        {
            value = Mathf.Max(this.FontSize, value);
            if (value != this.lineheight)
            {
                this.lineheight = value;
                this.Invalidate();
            }
        }
    }

    public bool PreserveWhitespace
    {
        get
        {
            return this.preserveWhitespace;
        }
        set
        {
            if (value != this.preserveWhitespace)
            {
                this.preserveWhitespace = value;
                this.Invalidate();
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
            if (!this.allowScrolling)
            {
                value = Vector2.zero;
            }
            Vector2 lhs = this.ContentSize - base.Size;
            value = Vector2.Min(lhs, value);
            value = Vector2.Max(Vector2.zero, value);
            value = value.RoundToInt();
            Vector2 vector2 = value - this.scrollPosition;
            if (vector2.sqrMagnitude > float.Epsilon)
            {
                this.scrollPosition = value;
                this.updateScrollbars();
                this.OnScrollPositionChanged();
            }
        }
    }

    public string Text
    {
        get
        {
            return this.text;
        }
        set
        {
            value = base.getLocalizedValue(value);
            if (!string.Equals(this.text, value))
            {
                this.text = value;
                this.scrollPosition = Vector2.zero;
                this.Invalidate();
                this.OnTextChanged();
            }
        }
    }

    public dfMarkupTextAlign TextAlignment
    {
        get
        {
            return this.align;
        }
        set
        {
            if (value != this.align)
            {
                this.align = value;
                this.Invalidate();
            }
        }
    }

    public dfTextScaleMode TextScaleMode
    {
        get
        {
            return this.textScaleMode;
        }
        set
        {
            this.textScaleMode = value;
            this.Invalidate();
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

    public dfScrollbar VerticalScrollbar
    {
        get
        {
            return this.vertScrollbar;
        }
        set
        {
            this.vertScrollbar = value;
            this.updateScrollbars();
        }
    }

    [dfEventCategory("Markup")]
    public delegate void LinkClickEventHandler(dfRichTextLabel sender, dfMarkupTagAnchor tag);
}

