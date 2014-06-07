using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Button")]
public class dfButton : dfInteractiveBase, IDFMultiRender
{
    [SerializeField]
    protected bool autoSize;
    private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();
    [SerializeField]
    protected Color32 disabledText = Color.white;
    [SerializeField]
    protected Color32 focusColor = Color.white;
    [SerializeField]
    protected Color32 focusText = Color.white;
    [SerializeField]
    protected dfFontBase font;
    [SerializeField]
    protected dfControl group;
    [SerializeField]
    protected Color32 hoverColor = Color.white;
    [SerializeField]
    protected Color32 hoverText = Color.white;
    [SerializeField]
    protected RectOffset padding = new RectOffset();
    [SerializeField]
    protected Color32 pressedColor = Color.white;
    [SerializeField]
    protected string pressedSprite;
    [SerializeField]
    protected Color32 pressedText = Color.white;
    [SerializeField]
    protected Color32 shadowColor = Color.black;
    [SerializeField]
    protected Vector2 shadowOffset = new Vector2(1f, -1f);
    private Vector2 startSize = Vector2.zero;
    [SerializeField]
    protected ButtonState state;
    [SerializeField]
    protected string text = string.Empty;
    [SerializeField]
    protected UnityEngine.TextAlignment textAlign = UnityEngine.TextAlignment.Center;
    [SerializeField]
    protected Color32 textColor = Color.white;
    private dfRenderData textRenderData;
    [SerializeField]
    protected float textScale = 1f;
    [SerializeField]
    protected dfTextScaleMode textScaleMode;
    [SerializeField]
    protected bool textShadow;
    [SerializeField]
    protected dfVerticalAlignment vertAlign = dfVerticalAlignment.Middle;
    [SerializeField]
    protected bool wordWrap;

    public event PropertyChangedEventHandler<ButtonState> ButtonStateChanged;

    private void autoSizeToText()
    {
        if (((this.Font != null) && this.Font.IsValid) && !string.IsNullOrEmpty(this.Text))
        {
            using (dfFontRendererBase base2 = this.obtainTextRenderer())
            {
                Vector2 vector = base2.MeasureString(this.Text);
                Vector2 vector2 = new Vector2(vector.x + this.padding.horizontal, vector.y + this.padding.vertical);
                base.Size = vector2;
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
        this.startSize = base.Size;
    }

    protected override Color32 getActiveColor()
    {
        switch (this.State)
        {
            case ButtonState.Focus:
                return this.FocusBackgroundColor;

            case ButtonState.Hover:
                return this.HoverBackgroundColor;

            case ButtonState.Pressed:
                return this.PressedBackgroundColor;

            case ButtonState.Disabled:
                return base.DisabledColor;
        }
        return base.Color;
    }

    protected internal override dfAtlas.ItemInfo getBackgroundSprite()
    {
        if (base.Atlas == null)
        {
            return null;
        }
        dfAtlas.ItemInfo info = null;
        switch (this.state)
        {
            case ButtonState.Default:
                info = base.atlas[base.backgroundSprite];
                break;

            case ButtonState.Focus:
                info = base.atlas[base.focusSprite];
                break;

            case ButtonState.Hover:
                info = base.atlas[base.hoverSprite];
                break;

            case ButtonState.Pressed:
                info = base.atlas[this.pressedSprite];
                break;

            case ButtonState.Disabled:
                info = base.atlas[base.disabledSprite];
                break;
        }
        if (info == null)
        {
            info = base.atlas[base.backgroundSprite];
        }
        return info;
    }

    private Color32 getTextColorForState()
    {
        if (!base.IsEnabled)
        {
            return this.DisabledTextColor;
        }
        switch (this.state)
        {
            case ButtonState.Default:
                return this.TextColor;

            case ButtonState.Focus:
                return this.FocusTextColor;

            case ButtonState.Hover:
                return this.HoverTextColor;

            case ButtonState.Pressed:
                return this.PressedTextColor;

            case ButtonState.Disabled:
                return this.DisabledTextColor;
        }
        return Color.white;
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
        if (this.AutoSize)
        {
            this.autoSizeToText();
        }
    }

    private dfFontRendererBase obtainTextRenderer()
    {
        Vector2 vector = base.Size - new Vector2((float) this.padding.horizontal, (float) this.padding.vertical);
        Vector2 vector2 = !this.autoSize ? vector : ((Vector2) (Vector2.one * 2.147484E+09f));
        float discreteValue = base.PixelsToUnits();
        Vector3 vector3 = (Vector3) ((base.pivot.TransformToUpperLeft(base.Size) + new Vector3((float) this.padding.left, (float) -this.padding.top)) * discreteValue);
        float num2 = this.TextScale * this.getTextScaleMultiplier();
        Color32 color = base.ApplyOpacity(this.getTextColorForState());
        dfFontRendererBase textRenderer = this.Font.ObtainRenderer();
        textRenderer.WordWrap = this.WordWrap;
        textRenderer.MultiLine = this.WordWrap;
        textRenderer.MaxSize = vector2;
        textRenderer.PixelRatio = discreteValue;
        textRenderer.TextScale = num2;
        textRenderer.CharacterSpacing = 0;
        textRenderer.VectorOffset = vector3.Quantize(discreteValue);
        textRenderer.TabSize = 0;
        textRenderer.TextAlign = !this.autoSize ? this.TextAlignment : UnityEngine.TextAlignment.Left;
        textRenderer.ProcessMarkup = true;
        textRenderer.DefaultColor = color;
        textRenderer.OverrideMarkupColors = false;
        textRenderer.Opacity = base.CalculateOpacity();
        textRenderer.Shadow = this.Shadow;
        textRenderer.ShadowColor = this.ShadowColor;
        textRenderer.ShadowOffset = this.ShadowOffset;
        dfDynamicFont.DynamicFontRenderer renderer = textRenderer as dfDynamicFont.DynamicFontRenderer;
        if (renderer != null)
        {
            renderer.SpriteAtlas = base.Atlas;
            renderer.SpriteBuffer = base.renderData;
        }
        if (this.vertAlign != dfVerticalAlignment.Top)
        {
            textRenderer.VectorOffset = this.getVertAlignOffset(textRenderer);
        }
        return textRenderer;
    }

    protected virtual void OnButtonStateChanged(ButtonState value)
    {
        if (base.isEnabled || (value == ButtonState.Disabled))
        {
            this.state = value;
            object[] args = new object[] { value };
            base.Signal("OnButtonStateChanged", args);
            if (this.ButtonStateChanged != null)
            {
                this.ButtonStateChanged(this, value);
            }
            this.Invalidate();
        }
    }

    protected internal override void OnClick(dfMouseEventArgs args)
    {
        if (this.group != null)
        {
            foreach (dfButton button in base.transform.parent.GetComponentsInChildren<dfButton>())
            {
                if (((button != this) && (button.ButtonGroup == this.ButtonGroup)) && (button != this))
                {
                    button.State = ButtonState.Default;
                }
            }
            if (!base.transform.IsChildOf(this.group.transform))
            {
                object[] objArray1 = new object[] { args };
                base.Signal(this.group.gameObject, "OnClick", objArray1);
            }
        }
        base.OnClick(args);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        bool flag = (this.Font != null) && this.Font.IsValid;
        if (Application.isPlaying && !flag)
        {
            this.Font = base.GetManager().DefaultFont;
        }
    }

    protected internal override void OnEnterFocus(dfFocusEventArgs args)
    {
        if (this.State != ButtonState.Pressed)
        {
            this.State = ButtonState.Focus;
        }
        base.OnEnterFocus(args);
    }

    protected internal override void OnIsEnabledChanged()
    {
        if (!base.IsEnabled)
        {
            this.State = ButtonState.Disabled;
        }
        else
        {
            this.State = ButtonState.Default;
        }
        base.OnIsEnabledChanged();
    }

    protected internal override void OnKeyPress(dfKeyEventArgs args)
    {
        if (this.IsInteractive && (args.KeyCode == KeyCode.Space))
        {
            this.OnClick(new dfMouseEventArgs(this, dfMouseButtons.Left, 1, new Ray(), Vector2.zero, 0f));
        }
        else
        {
            base.OnKeyPress(args);
        }
    }

    protected internal override void OnLeaveFocus(dfFocusEventArgs args)
    {
        this.State = ButtonState.Default;
        base.OnLeaveFocus(args);
    }

    protected internal override void OnLocalize()
    {
        base.OnLocalize();
        this.Text = base.getLocalizedValue(this.text);
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        if (!(base.parent is dfTabstrip) || (this.State != ButtonState.Focus))
        {
            this.State = ButtonState.Pressed;
        }
        base.OnMouseDown(args);
    }

    protected internal override void OnMouseEnter(dfMouseEventArgs args)
    {
        if (!(base.parent is dfTabstrip) || (this.State != ButtonState.Focus))
        {
            this.State = ButtonState.Hover;
        }
        base.OnMouseEnter(args);
    }

    protected internal override void OnMouseLeave(dfMouseEventArgs args)
    {
        if (this.ContainsFocus)
        {
            this.State = ButtonState.Focus;
        }
        else
        {
            this.State = ButtonState.Default;
        }
        base.OnMouseLeave(args);
    }

    protected internal override void OnMouseUp(dfMouseEventArgs args)
    {
        if (base.isMouseHovering)
        {
            if ((base.parent is dfTabstrip) && this.ContainsFocus)
            {
                this.State = ButtonState.Focus;
            }
            else
            {
                this.State = ButtonState.Hover;
            }
        }
        else if (this.HasFocus)
        {
            this.State = ButtonState.Focus;
        }
        else
        {
            this.State = ButtonState.Default;
        }
        base.OnMouseUp(args);
    }

    public dfList<dfRenderData> RenderMultiple()
    {
        if (!base.isVisible)
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
        base.isControlInvalidated = false;
        this.buffers.Clear();
        base.renderData.Clear();
        if (base.Atlas != null)
        {
            base.renderData.Material = base.Atlas.Material;
            base.renderData.Transform = base.transform.localToWorldMatrix;
            this.renderBackground();
            this.buffers.Add(base.renderData);
        }
        dfRenderData item = this.renderText();
        if ((item != null) && (item != base.renderData))
        {
            item.Transform = base.transform.localToWorldMatrix;
            this.buffers.Add(item);
        }
        this.updateCollider();
        return this.buffers;
    }

    private dfRenderData renderText()
    {
        if (((this.Font == null) || !this.Font.IsValid) || string.IsNullOrEmpty(this.Text))
        {
            return null;
        }
        dfRenderData renderData = base.renderData;
        if (this.font is dfDynamicFont)
        {
            dfDynamicFont font = (dfDynamicFont) this.font;
            renderData = this.textRenderData;
            renderData.Clear();
            renderData.Material = font.Material;
        }
        using (dfFontRendererBase base2 = this.obtainTextRenderer())
        {
            base2.Render(this.text, renderData);
        }
        return renderData;
    }

    public override void Update()
    {
        base.Update();
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
                this.autoSize = value;
                if (value)
                {
                    this.textAlign = UnityEngine.TextAlignment.Left;
                }
                this.Invalidate();
            }
        }
    }

    public dfControl ButtonGroup
    {
        get
        {
            return this.group;
        }
        set
        {
            if (value != this.group)
            {
                this.group = value;
                this.Invalidate();
            }
        }
    }

    public Color32 DisabledTextColor
    {
        get
        {
            return this.disabledText;
        }
        set
        {
            this.disabledText = value;
            this.Invalidate();
        }
    }

    public Color32 FocusBackgroundColor
    {
        get
        {
            return this.focusColor;
        }
        set
        {
            this.focusColor = value;
            this.Invalidate();
        }
    }

    public Color32 FocusTextColor
    {
        get
        {
            return this.focusText;
        }
        set
        {
            this.focusText = value;
            this.Invalidate();
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
            }
            this.Invalidate();
        }
    }

    public Color32 HoverBackgroundColor
    {
        get
        {
            return this.hoverColor;
        }
        set
        {
            this.hoverColor = value;
            this.Invalidate();
        }
    }

    public Color32 HoverTextColor
    {
        get
        {
            return this.hoverText;
        }
        set
        {
            this.hoverText = value;
            this.Invalidate();
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

    public Color32 PressedBackgroundColor
    {
        get
        {
            return this.pressedColor;
        }
        set
        {
            this.pressedColor = value;
            this.Invalidate();
        }
    }

    public string PressedSprite
    {
        get
        {
            return this.pressedSprite;
        }
        set
        {
            if (value != this.pressedSprite)
            {
                this.pressedSprite = value;
                this.Invalidate();
            }
        }
    }

    public Color32 PressedTextColor
    {
        get
        {
            return this.pressedText;
        }
        set
        {
            this.pressedText = value;
            this.Invalidate();
        }
    }

    public bool Shadow
    {
        get
        {
            return this.textShadow;
        }
        set
        {
            if (value != this.textShadow)
            {
                this.textShadow = value;
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

    public ButtonState State
    {
        get
        {
            return this.state;
        }
        set
        {
            if (value != this.state)
            {
                this.OnButtonStateChanged(value);
                this.Invalidate();
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
            if (value != this.text)
            {
                this.text = base.getLocalizedValue(value);
                this.Invalidate();
            }
        }
    }

    public UnityEngine.TextAlignment TextAlignment
    {
        get
        {
            if (this.autoSize)
            {
                return UnityEngine.TextAlignment.Left;
            }
            return this.textAlign;
        }
        set
        {
            if (value != this.textAlign)
            {
                this.textAlign = value;
                this.Invalidate();
            }
        }
    }

    public Color32 TextColor
    {
        get
        {
            return this.textColor;
        }
        set
        {
            this.textColor = value;
            this.Invalidate();
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

    public enum ButtonState
    {
        Default,
        Focus,
        Hover,
        Pressed,
        Disabled
    }
}

