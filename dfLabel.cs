using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), AddComponentMenu("Daikon Forge/User Interface/Label"), ExecuteInEditMode]
public class dfLabel : dfControl, IDFMultiRender
{
    [SerializeField]
    protected UnityEngine.TextAlignment align;
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected bool autoHeight;
    [SerializeField]
    protected bool autoSize;
    [SerializeField]
    protected Color32 backgroundColor = Color.white;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected Color32 bottomColor = new Color32(0xff, 0xff, 0xff, 0xff);
    private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();
    [SerializeField]
    protected int charSpacing;
    [SerializeField]
    protected bool colorizeSymbols;
    [SerializeField]
    protected bool enableGradient;
    [SerializeField]
    protected dfFontBase font;
    [SerializeField]
    protected bool outline;
    [SerializeField]
    protected Color32 outlineColor = Color.black;
    [SerializeField]
    protected int outlineWidth = 1;
    [SerializeField]
    protected RectOffset padding = new RectOffset();
    [SerializeField]
    protected bool processMarkup;
    [SerializeField]
    protected bool shadow;
    [SerializeField]
    protected Color32 shadowColor = Color.black;
    [SerializeField]
    protected Vector2 shadowOffset = new Vector2(1f, -1f);
    private Vector2 startSize = Vector2.zero;
    [SerializeField]
    protected int tabSize = 0x30;
    [SerializeField]
    protected List<int> tabStops = new List<int>();
    [SerializeField]
    protected string text = "Label";
    private dfRenderData textRenderData;
    [SerializeField]
    protected float textScale = 1f;
    [SerializeField]
    protected dfTextScaleMode textScaleMode;
    [SerializeField]
    protected dfVerticalAlignment vertAlign;
    [SerializeField]
    protected bool wordWrap;

    public event PropertyChangedEventHandler<string> TextChanged;

    public override void Awake()
    {
        base.Awake();
        this.startSize = !Application.isPlaying ? Vector2.zero : base.Size;
    }

    public override Vector2 CalculateMinimumSize()
    {
        if (this.Font != null)
        {
            float x = (this.Font.FontSize * this.TextScale) * 0.75f;
            return Vector2.Max(base.CalculateMinimumSize(), new Vector2(x, x));
        }
        return base.CalculateMinimumSize();
    }

    private Vector2 getAutoSizeDefault()
    {
        float x = (this.maxSize.x <= float.Epsilon) ? 2.147484E+09f : this.maxSize.x;
        return new Vector2(x, (this.maxSize.y <= float.Epsilon) ? 2.147484E+09f : this.maxSize.y);
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
        if (this.autoSize)
        {
            return 1f;
        }
        return (base.Size.y / this.startSize.y);
    }

    private Vector3 getVertAlignOffset(dfFontRendererBase textRenderer)
    {
        float num = base.PixelsToUnits();
        Vector2 vector = (Vector2) (textRenderer.MeasureString(this.text) * num);
        Vector3 vectorOffset = textRenderer.VectorOffset;
        float num2 = (base.Height - this.padding.vertical) * num;
        if (vector.y < num2)
        {
            dfVerticalAlignment vertAlign = this.vertAlign;
            if (vertAlign != dfVerticalAlignment.Middle)
            {
                if (vertAlign == dfVerticalAlignment.Bottom)
                {
                    vectorOffset.y -= num2 - vector.y;
                }
                return vectorOffset;
            }
            vectorOffset.y -= (num2 - vector.y) * 0.5f;
        }
        return vectorOffset;
    }

    public override void Invalidate()
    {
        base.Invalidate();
        if ((this.Font != null) && this.Font.IsValid)
        {
            bool flag = this.size.sqrMagnitude <= float.Epsilon;
            if ((this.autoSize || this.autoHeight) || flag)
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    if (flag)
                    {
                        base.Size = new Vector2(150f, 24f);
                    }
                    if (this.AutoSize || this.AutoHeight)
                    {
                        base.Height = Mathf.CeilToInt(this.Font.LineHeight * this.TextScale);
                    }
                }
                else
                {
                    using (dfFontRendererBase base2 = this.obtainRenderer())
                    {
                        Vector2 vector = base2.MeasureString(this.text).RoundToInt();
                        if (this.AutoSize || flag)
                        {
                            base.size = vector + new Vector2((float) this.padding.horizontal, (float) this.padding.vertical);
                        }
                        else if (this.AutoHeight)
                        {
                            base.size = new Vector2(this.size.x, vector.y + this.padding.vertical);
                        }
                    }
                }
            }
        }
    }

    private dfFontRendererBase obtainRenderer()
    {
        bool flag = base.Size.sqrMagnitude <= float.Epsilon;
        Vector2 vector = base.Size - new Vector2((float) this.padding.horizontal, (float) this.padding.vertical);
        Vector2 vector2 = (!this.autoSize && !flag) ? vector : this.getAutoSizeDefault();
        if (this.autoHeight)
        {
            vector2 = new Vector2(vector.x, 2.147484E+09f);
        }
        float discreteValue = base.PixelsToUnits();
        Vector3 vector3 = (Vector3) ((base.pivot.TransformToUpperLeft(base.Size) + new Vector3((float) this.padding.left, (float) -this.padding.top)) * discreteValue);
        float num2 = this.TextScale * this.getTextScaleMultiplier();
        dfFontRendererBase textRenderer = this.Font.ObtainRenderer();
        textRenderer.WordWrap = this.WordWrap;
        textRenderer.MaxSize = vector2;
        textRenderer.PixelRatio = discreteValue;
        textRenderer.TextScale = num2;
        textRenderer.CharacterSpacing = this.CharacterSpacing;
        textRenderer.VectorOffset = vector3.Quantize(discreteValue);
        textRenderer.MultiLine = true;
        textRenderer.TabSize = this.TabSize;
        textRenderer.TabStops = this.TabStops;
        textRenderer.TextAlign = !this.autoSize ? this.TextAlignment : UnityEngine.TextAlignment.Left;
        textRenderer.ColorizeSymbols = this.ColorizeSymbols;
        textRenderer.ProcessMarkup = this.ProcessMarkup;
        textRenderer.DefaultColor = !base.IsEnabled ? base.DisabledColor : base.Color;
        textRenderer.BottomColor = !this.enableGradient ? null : new Color32?(this.BottomColor);
        textRenderer.OverrideMarkupColors = !base.IsEnabled;
        textRenderer.Opacity = base.CalculateOpacity();
        textRenderer.Outline = this.Outline;
        textRenderer.OutlineSize = this.OutlineSize;
        textRenderer.OutlineColor = this.OutlineColor;
        textRenderer.Shadow = this.Shadow;
        textRenderer.ShadowColor = this.ShadowColor;
        textRenderer.ShadowOffset = this.ShadowOffset;
        dfDynamicFont.DynamicFontRenderer renderer = textRenderer as dfDynamicFont.DynamicFontRenderer;
        if (renderer != null)
        {
            renderer.SpriteAtlas = this.Atlas;
            renderer.SpriteBuffer = base.renderData;
        }
        if (this.vertAlign != dfVerticalAlignment.Top)
        {
            textRenderer.VectorOffset = this.getVertAlignOffset(textRenderer);
        }
        return textRenderer;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        bool flag = (this.Font != null) && this.Font.IsValid;
        if (Application.isPlaying && !flag)
        {
            this.Font = base.GetManager().DefaultFont;
        }
        if (this.size.sqrMagnitude <= float.Epsilon)
        {
            base.Size = new Vector2(150f, 25f);
        }
    }

    protected internal override void OnLocalize()
    {
        base.OnLocalize();
        this.Text = base.getLocalizedValue(this.text);
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

    protected internal virtual void renderBackground()
    {
        if (this.Atlas != null)
        {
            dfAtlas.ItemInfo info = this.Atlas[this.backgroundSprite];
            if (info != null)
            {
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

    public dfList<dfRenderData> RenderMultiple()
    {
        dfList<dfRenderData> buffers;
        try
        {
            if (((this.Atlas == null) || (this.Font == null)) || (!base.isVisible || !this.Font.IsValid))
            {
                return null;
            }
            if (base.renderData == null)
            {
                base.renderData = dfRenderData.Obtain();
                this.textRenderData = dfRenderData.Obtain();
                base.isControlInvalidated = true;
            }
            if (!base.isControlInvalidated)
            {
                for (int i = 0; i < this.buffers.Count; i++)
                {
                    this.buffers[i].Transform = base.transform.localToWorldMatrix;
                }
                return this.buffers;
            }
            this.buffers.Clear();
            base.renderData.Clear();
            base.renderData.Material = this.Atlas.Material;
            base.renderData.Transform = base.transform.localToWorldMatrix;
            this.buffers.Add(base.renderData);
            this.textRenderData.Clear();
            this.textRenderData.Material = this.Atlas.Material;
            this.textRenderData.Transform = base.transform.localToWorldMatrix;
            this.buffers.Add(this.textRenderData);
            this.renderBackground();
            if (string.IsNullOrEmpty(this.Text))
            {
                if (this.AutoSize || this.AutoHeight)
                {
                    base.Height = Mathf.CeilToInt(this.Font.LineHeight * this.TextScale);
                }
                return this.buffers;
            }
            bool flag = this.size.sqrMagnitude <= float.Epsilon;
            using (dfFontRendererBase base2 = this.obtainRenderer())
            {
                base2.Render(this.text, this.textRenderData);
                if (this.AutoSize || flag)
                {
                    base.Size = (base2.RenderedSize + new Vector2((float) this.padding.horizontal, (float) this.padding.vertical)).CeilToInt();
                }
                else if (this.AutoHeight)
                {
                    base.Size = new Vector2(this.size.x, base2.RenderedSize.y + this.padding.vertical).CeilToInt();
                }
            }
            this.updateCollider();
            buffers = this.buffers;
        }
        finally
        {
            base.isControlInvalidated = false;
        }
        return buffers;
    }

    public override void Update()
    {
        if (this.autoSize)
        {
            this.autoHeight = false;
        }
        if (this.Font == null)
        {
            this.Font = base.GetManager().DefaultFont;
        }
        base.Update();
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

    public bool AutoHeight
    {
        get
        {
            return (this.autoHeight && !this.autoSize);
        }
        set
        {
            if (value != this.autoHeight)
            {
                if (value)
                {
                    this.autoSize = false;
                }
                this.autoHeight = value;
                this.Invalidate();
            }
        }
    }

    public bool AutoSize
    {
        get
        {
            return this.autoSize;
        }
        set
        {
            if (value != this.autoSize)
            {
                if (value)
                {
                    this.autoHeight = false;
                }
                this.autoSize = value;
                this.Invalidate();
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

    public Color32 BottomColor
    {
        get
        {
            return this.bottomColor;
        }
        set
        {
            if (!this.bottomColor.Equals(value))
            {
                this.bottomColor = value;
                this.OnColorChanged();
            }
        }
    }

    public int CharacterSpacing
    {
        get
        {
            return this.charSpacing;
        }
        set
        {
            value = Mathf.Max(0, value);
            if (value != this.charSpacing)
            {
                this.charSpacing = value;
                this.Invalidate();
            }
        }
    }

    public bool ColorizeSymbols
    {
        get
        {
            return this.colorizeSymbols;
        }
        set
        {
            if (value != this.colorizeSymbols)
            {
                this.colorizeSymbols = value;
                this.Invalidate();
            }
        }
    }

    public dfFontBase Font
    {
        get
        {
            if (this.font == null)
            {
                dfGUIManager manager = base.GetManager();
                if (manager != null)
                {
                    this.font = manager.DefaultFont;
                }
            }
            return this.font;
        }
        set
        {
            if (value != this.font)
            {
                this.font = value;
                this.Invalidate();
            }
        }
    }

    public bool Outline
    {
        get
        {
            return this.outline;
        }
        set
        {
            if (value != this.outline)
            {
                this.outline = value;
                this.Invalidate();
            }
        }
    }

    public Color32 OutlineColor
    {
        get
        {
            return this.outlineColor;
        }
        set
        {
            if (!value.Equals(this.outlineColor))
            {
                this.outlineColor = value;
                this.Invalidate();
            }
        }
    }

    public int OutlineSize
    {
        get
        {
            return this.outlineWidth;
        }
        set
        {
            value = Mathf.Max(0, value);
            if (value != this.outlineWidth)
            {
                this.outlineWidth = value;
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
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.padding))
            {
                this.padding = value;
                this.Invalidate();
            }
        }
    }

    public bool ProcessMarkup
    {
        get
        {
            return this.processMarkup;
        }
        set
        {
            if (value != this.processMarkup)
            {
                this.processMarkup = value;
                this.Invalidate();
            }
        }
    }

    public bool Shadow
    {
        get
        {
            return this.shadow;
        }
        set
        {
            if (value != this.shadow)
            {
                this.shadow = value;
                this.Invalidate();
            }
        }
    }

    public Color32 ShadowColor
    {
        get
        {
            return this.shadowColor;
        }
        set
        {
            if (!value.Equals(this.shadowColor))
            {
                this.shadowColor = value;
                this.Invalidate();
            }
        }
    }

    public Vector2 ShadowOffset
    {
        get
        {
            return this.shadowOffset;
        }
        set
        {
            if (value != this.shadowOffset)
            {
                this.shadowOffset = value;
                this.Invalidate();
            }
        }
    }

    public bool ShowGradient
    {
        get
        {
            return this.enableGradient;
        }
        set
        {
            if (value != this.enableGradient)
            {
                this.enableGradient = value;
                this.Invalidate();
            }
        }
    }

    public int TabSize
    {
        get
        {
            return this.tabSize;
        }
        set
        {
            value = Mathf.Max(0, value);
            if (value != this.tabSize)
            {
                this.tabSize = value;
                this.Invalidate();
            }
        }
    }

    public List<int> TabStops
    {
        get
        {
            return this.tabStops;
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
            value = value.Replace(@"\t", "\t").Replace(@"\n", "\n");
            if (!string.Equals(value, this.text))
            {
                this.text = base.getLocalizedValue(value);
                this.OnTextChanged();
            }
        }
    }

    public UnityEngine.TextAlignment TextAlignment
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

    public float TextScale
    {
        get
        {
            return this.textScale;
        }
        set
        {
            value = Mathf.Max(0.1f, value);
            if (!Mathf.Approximately(this.textScale, value))
            {
                this.textScale = value;
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

    public dfVerticalAlignment VerticalAlignment
    {
        get
        {
            return this.vertAlign;
        }
        set
        {
            if (value != this.vertAlign)
            {
                this.vertAlign = value;
                this.Invalidate();
            }
        }
    }

    public bool WordWrap
    {
        get
        {
            return this.wordWrap;
        }
        set
        {
            if (value != this.wordWrap)
            {
                this.wordWrap = value;
                this.Invalidate();
            }
        }
    }
}

