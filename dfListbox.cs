using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Listbox"), RequireComponent(typeof(BoxCollider)), ExecuteInEditMode]
public class dfListbox : dfInteractiveBase, IDFMultiRender
{
    [SerializeField]
    protected bool animateHover;
    private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();
    private bool eventsAttached;
    [SerializeField]
    protected dfFontBase font;
    private int hoverIndex = -1;
    private float hoverTweenLocation;
    [SerializeField]
    protected TextAlignment itemAlignment;
    [SerializeField]
    protected int itemHeight = 0x19;
    [SerializeField]
    protected string itemHighlight = string.Empty;
    [SerializeField]
    protected string itemHover = string.Empty;
    [SerializeField]
    protected RectOffset itemPadding = new RectOffset();
    [SerializeField]
    protected string[] items = new string[0];
    [SerializeField]
    protected Color32 itemTextColor = Color.white;
    [SerializeField]
    protected float itemTextScale = 1f;
    [SerializeField]
    protected RectOffset listPadding = new RectOffset();
    [SerializeField]
    protected dfScrollbar scrollbar;
    private float scrollPosition;
    [SerializeField]
    protected int selectedIndex = -1;
    [SerializeField]
    protected bool shadow;
    [SerializeField]
    protected Color32 shadowColor = Color.black;
    [SerializeField]
    protected Vector2 shadowOffset = new Vector2(1f, -1f);
    private Vector2 startSize = Vector2.zero;
    private dfRenderData textRenderData;
    [SerializeField]
    protected dfTextScaleMode textScaleMode;
    private Vector2 touchStartPosition = Vector2.zero;

    public event PropertyChangedEventHandler<int> ItemClicked;

    public event PropertyChangedEventHandler<int> SelectedIndexChanged;

    private void attachScrollbarEvents()
    {
        if ((this.scrollbar != null) && !this.eventsAttached)
        {
            this.eventsAttached = true;
            this.scrollbar.ValueChanged += new PropertyChangedEventHandler<float>(this.scrollbar_ValueChanged);
            this.scrollbar.GotFocus += new FocusEventHandler(this.scrollbar_GotFocus);
        }
    }

    public override void Awake()
    {
        base.Awake();
        this.startSize = base.Size;
    }

    private void clipQuads(dfRenderData buffer, int startIndex)
    {
        dfList<Vector3> vertices = buffer.Vertices;
        dfList<Vector2> uV = buffer.UV;
        float num = base.PixelsToUnits();
        float a = (base.Pivot.TransformToUpperLeft(base.Size).y - this.listPadding.top) * num;
        float b = a - ((this.size.y - this.listPadding.vertical) * num);
        for (int i = startIndex; i < vertices.Count; i += 4)
        {
            Vector3 vector = vertices[i];
            Vector3 vector2 = vertices[i + 1];
            Vector3 vector3 = vertices[i + 2];
            Vector3 vector4 = vertices[i + 3];
            float num5 = vector.y - vector4.y;
            if (vector4.y < b)
            {
                float t = 1f - (Mathf.Abs((float) (-b + vector.y)) / num5);
                vector = new Vector3(vector.x, Mathf.Max(vector.y, b), vector2.z);
                vertices[i] = vector;
                float y = Mathf.Max(vector2.y, b);
                vector2 = new Vector3(vector2.x, y, vector2.z);
                vertices[i + 1] = vector2;
                float introduced25 = Mathf.Max(vector3.y, b);
                vector3 = new Vector3(vector3.x, introduced25, vector3.z);
                vertices[i + 2] = vector3;
                float introduced26 = Mathf.Max(vector4.y, b);
                vector4 = new Vector3(vector4.x, introduced26, vector4.z);
                vertices[i + 3] = vector4;
                Vector2 vector6 = uV[i + 3];
                Vector2 vector7 = uV[i];
                float num7 = Mathf.Lerp(vector6.y, vector7.y, t);
                Vector2 vector8 = uV[i + 3];
                uV[i + 3] = new Vector2(vector8.x, num7);
                Vector2 vector9 = uV[i + 2];
                uV[i + 2] = new Vector2(vector9.x, num7);
                num5 = Mathf.Abs((float) (vector4.y - vector.y));
            }
            if (vector.y > a)
            {
                float num8 = Mathf.Abs((float) (a - vector.y)) / num5;
                float introduced27 = Mathf.Min(a, vector.y);
                vertices[i] = new Vector3(vector.x, introduced27, vector.z);
                float introduced28 = Mathf.Min(a, vector2.y);
                vertices[i + 1] = new Vector3(vector2.x, introduced28, vector2.z);
                float introduced29 = Mathf.Min(a, vector3.y);
                vertices[i + 2] = new Vector3(vector3.x, introduced29, vector3.z);
                float introduced30 = Mathf.Min(a, vector4.y);
                vertices[i + 3] = new Vector3(vector4.x, introduced30, vector4.z);
                Vector2 vector10 = uV[i];
                Vector2 vector11 = uV[i + 3];
                float num9 = Mathf.Lerp(vector10.y, vector11.y, num8);
                Vector2 vector12 = uV[i];
                uV[i] = new Vector2(vector12.x, num9);
                Vector2 vector13 = uV[i + 1];
                uV[i + 1] = new Vector2(vector13.x, num9);
            }
        }
    }

    private float constrainScrollPosition(float value)
    {
        value = Mathf.Max(0f, value);
        int num = this.items.Length * this.itemHeight;
        float num2 = this.size.y - this.listPadding.vertical;
        if (num < num2)
        {
            return 0f;
        }
        return Mathf.Min(value, num - num2);
    }

    private void detachScrollbarEvents()
    {
        if ((this.scrollbar != null) && this.eventsAttached)
        {
            this.eventsAttached = false;
            this.scrollbar.ValueChanged -= new PropertyChangedEventHandler<float>(this.scrollbar_ValueChanged);
            this.scrollbar.GotFocus -= new FocusEventHandler(this.scrollbar_GotFocus);
        }
    }

    public void EnsureVisible(int index)
    {
        int num = index * this.ItemHeight;
        if (this.scrollPosition > num)
        {
            this.ScrollPosition = num;
        }
        float num2 = this.size.y - this.listPadding.vertical;
        if ((this.scrollPosition + num2) < (num + this.itemHeight))
        {
            this.ScrollPosition = (num - num2) + this.itemHeight;
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

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (Application.isPlaying)
        {
            this.attachScrollbarEvents();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        this.detachScrollbarEvents();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.detachScrollbarEvents();
    }

    protected internal virtual void OnItemClicked()
    {
        object[] args = new object[] { this.selectedIndex };
        base.Signal("OnItemClicked", args);
        if (this.ItemClicked != null)
        {
            this.ItemClicked(this, this.selectedIndex);
        }
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        switch (args.KeyCode)
        {
            case KeyCode.UpArrow:
                this.SelectedIndex = Mathf.Max(0, this.selectedIndex - 1);
                break;

            case KeyCode.DownArrow:
                this.SelectedIndex++;
                break;

            case KeyCode.Home:
                this.SelectedIndex = 0;
                break;

            case KeyCode.End:
                this.SelectedIndex = this.items.Length;
                break;

            case KeyCode.PageUp:
            {
                int b = this.SelectedIndex - Mathf.FloorToInt((this.size.y - this.listPadding.vertical) / ((float) this.itemHeight));
                this.SelectedIndex = Mathf.Max(0, b);
                break;
            }
            case KeyCode.PageDown:
                this.SelectedIndex += Mathf.FloorToInt((this.size.y - this.listPadding.vertical) / ((float) this.itemHeight));
                break;
        }
        base.OnKeyDown(args);
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        base.OnMouseDown(args);
        if (args is dfTouchEventArgs)
        {
            this.touchStartPosition = args.Position;
        }
        else
        {
            this.selectItemUnderMouse(args);
        }
    }

    protected internal override void OnMouseEnter(dfMouseEventArgs args)
    {
        base.OnMouseEnter(args);
        this.touchStartPosition = args.Position;
    }

    protected internal override void OnMouseLeave(dfMouseEventArgs args)
    {
        base.OnMouseLeave(args);
        this.hoverIndex = -1;
    }

    protected internal override void OnMouseMove(dfMouseEventArgs args)
    {
        base.OnMouseMove(args);
        if (args is dfTouchEventArgs)
        {
            if (Mathf.Abs((float) (args.Position.y - this.touchStartPosition.y)) >= (this.itemHeight / 2))
            {
                this.ScrollPosition = Mathf.Max((float) 0f, (float) (this.ScrollPosition + args.MoveDelta.y));
                this.synchronizeScrollbar();
                this.hoverIndex = -1;
            }
        }
        else
        {
            this.updateItemHover(args);
        }
    }

    protected internal override void OnMouseUp(dfMouseEventArgs args)
    {
        this.hoverIndex = -1;
        base.OnMouseUp(args);
        if ((args is dfTouchEventArgs) && (Mathf.Abs((float) (args.Position.y - this.touchStartPosition.y)) < this.itemHeight))
        {
            this.selectItemUnderMouse(args);
        }
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
        base.OnMouseWheel(args);
        this.ScrollPosition = Mathf.Max((float) 0f, (float) (this.ScrollPosition - (((int) args.WheelDelta) * this.ItemHeight)));
        this.synchronizeScrollbar();
        this.updateItemHover(args);
    }

    protected internal virtual void OnSelectedIndexChanged()
    {
        object[] args = new object[] { this.selectedIndex };
        base.SignalHierarchy("OnSelectedIndexChanged", args);
        if (this.SelectedIndexChanged != null)
        {
            this.SelectedIndexChanged(this, this.selectedIndex);
        }
    }

    private void renderHover()
    {
        if (Application.isPlaying && ((((base.Atlas != null) && base.IsEnabled) && ((this.hoverIndex >= 0) && (this.hoverIndex <= (this.items.Length - 1)))) && !string.IsNullOrEmpty(this.ItemHover)))
        {
            dfAtlas.ItemInfo info = base.Atlas[this.ItemHover];
            if (info != null)
            {
                Vector3 vector = base.pivot.TransformToUpperLeft(base.Size);
                Vector3 vector2 = new Vector3(vector.x + this.listPadding.left, (vector.y - this.listPadding.top) + this.scrollPosition, 0f);
                float stepSize = base.PixelsToUnits();
                int num2 = this.hoverIndex * this.itemHeight;
                if (this.animateHover)
                {
                    float num3 = Mathf.Abs((float) (this.hoverTweenLocation - num2));
                    float num4 = (this.size.y - this.listPadding.vertical) * 0.5f;
                    if (num3 > num4)
                    {
                        this.hoverTweenLocation = num2 + (Mathf.Sign(this.hoverTweenLocation - num2) * num4);
                        num3 = num4;
                    }
                    float maxDelta = (Time.deltaTime / stepSize) * 2f;
                    this.hoverTweenLocation = Mathf.MoveTowards(this.hoverTweenLocation, (float) num2, maxDelta);
                }
                else
                {
                    this.hoverTweenLocation = num2;
                }
                vector2.y -= this.hoverTweenLocation.Quantize(stepSize);
                Color32 color = base.ApplyOpacity(base.color);
                dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                    atlas = base.atlas,
                    color = color,
                    fillAmount = 1f,
                    pixelsToUnits = base.PixelsToUnits(),
                    size = new Vector3(this.size.x - this.listPadding.horizontal, (float) this.itemHeight),
                    spriteInfo = info,
                    offset = vector2
                };
                if ((info.border.horizontal > 0) || (info.border.vertical > 0))
                {
                    dfSlicedSprite.renderSprite(base.renderData, options);
                }
                else
                {
                    dfSprite.renderSprite(base.renderData, options);
                }
                if (num2 != this.hoverTweenLocation)
                {
                    this.Invalidate();
                }
            }
        }
    }

    private void renderItems(dfRenderData buffer)
    {
        if (((this.font != null) && (this.items != null)) && (this.items.Length != 0))
        {
            float num = base.PixelsToUnits();
            Vector2 vector = new Vector2((this.size.x - this.itemPadding.horizontal) - this.listPadding.horizontal, (float) (this.itemHeight - this.itemPadding.vertical));
            Vector3 vector2 = base.pivot.TransformToUpperLeft(base.Size);
            Vector3 vector3 = (Vector3) (new Vector3((vector2.x + this.itemPadding.left) + this.listPadding.left, (vector2.y - this.itemPadding.top) - this.listPadding.top, 0f) * num);
            vector3.y += this.scrollPosition * num;
            Color32 color = !base.IsEnabled ? base.DisabledColor : this.ItemTextColor;
            float num2 = vector2.y * num;
            float num3 = num2 - (this.size.y * num);
            for (int i = 0; i < this.items.Length; i++)
            {
                using (dfFontRendererBase base2 = this.font.ObtainRenderer())
                {
                    base2.WordWrap = false;
                    base2.MaxSize = vector;
                    base2.PixelRatio = num;
                    base2.TextScale = this.ItemTextScale * this.getTextScaleMultiplier();
                    base2.VectorOffset = vector3;
                    base2.MultiLine = false;
                    base2.TextAlign = this.ItemAlignment;
                    base2.ProcessMarkup = true;
                    base2.DefaultColor = color;
                    base2.OverrideMarkupColors = false;
                    base2.Opacity = base.CalculateOpacity();
                    base2.Shadow = this.Shadow;
                    base2.ShadowColor = this.ShadowColor;
                    base2.ShadowOffset = this.ShadowOffset;
                    dfDynamicFont.DynamicFontRenderer renderer = base2 as dfDynamicFont.DynamicFontRenderer;
                    if (renderer != null)
                    {
                        renderer.SpriteAtlas = base.Atlas;
                        renderer.SpriteBuffer = base.renderData;
                    }
                    if ((vector3.y - (this.itemHeight * num)) <= num2)
                    {
                        base2.Render(this.items[i], buffer);
                    }
                    vector3.y -= this.itemHeight * num;
                    base2.VectorOffset = vector3;
                    if (vector3.y < num3)
                    {
                        break;
                    }
                }
            }
        }
    }

    public dfList<dfRenderData> RenderMultiple()
    {
        if ((base.Atlas == null) || (this.Font == null))
        {
            return null;
        }
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
        this.buffers.Clear();
        base.renderData.Clear();
        base.renderData.Material = base.Atlas.Material;
        base.renderData.Transform = base.transform.localToWorldMatrix;
        this.buffers.Add(base.renderData);
        this.textRenderData.Clear();
        this.textRenderData.Material = base.Atlas.Material;
        this.textRenderData.Transform = base.transform.localToWorldMatrix;
        this.buffers.Add(this.textRenderData);
        this.renderBackground();
        int count = base.renderData.Vertices.Count;
        this.renderHover();
        this.renderSelection();
        this.renderItems(this.textRenderData);
        this.clipQuads(base.renderData, count);
        this.clipQuads(this.textRenderData, 0);
        base.isControlInvalidated = false;
        this.updateCollider();
        return this.buffers;
    }

    private void renderSelection()
    {
        if ((base.Atlas != null) && (this.selectedIndex >= 0))
        {
            dfAtlas.ItemInfo info = base.Atlas[this.ItemHighlight];
            if (info != null)
            {
                Vector3 vector2;
                float num = base.PixelsToUnits();
                Vector3 vector = base.pivot.TransformToUpperLeft(base.Size);
                vector2 = new Vector3(vector.x + this.listPadding.left, (vector.y - this.listPadding.top) + this.scrollPosition, 0f) {
                    y = vector2.y - (this.selectedIndex * this.itemHeight)
                };
                Color32 color = base.ApplyOpacity(base.color);
                dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                    atlas = base.atlas,
                    color = color,
                    fillAmount = 1f,
                    pixelsToUnits = num,
                    size = new Vector3(this.size.x - this.listPadding.horizontal, (float) this.itemHeight),
                    spriteInfo = info,
                    offset = vector2
                };
                if ((info.border.horizontal > 0) || (info.border.vertical > 0))
                {
                    dfSlicedSprite.renderSprite(base.renderData, options);
                }
                else
                {
                    dfSprite.renderSprite(base.renderData, options);
                }
            }
        }
    }

    private void scrollbar_GotFocus(dfControl control, dfFocusEventArgs args)
    {
        base.Focus();
    }

    private void scrollbar_ValueChanged(dfControl control, float value)
    {
        this.ScrollPosition = value;
    }

    private void selectItemUnderMouse(dfMouseEventArgs args)
    {
        float num = base.pivot.TransformToUpperLeft(base.Size).y + ((-this.itemHeight * (this.selectedIndex - this.scrollPosition)) - this.listPadding.top);
        float num2 = (((this.selectedIndex - this.scrollPosition) + 1f) * this.itemHeight) + this.listPadding.vertical;
        float num3 = num2 - this.size.y;
        if (num3 > 0f)
        {
            num += num3;
        }
        float num4 = base.GetHitPosition(args).y - this.listPadding.top;
        if ((num4 >= 0f) && (num4 <= (this.size.y - this.listPadding.bottom)))
        {
            this.SelectedIndex = (int) ((this.scrollPosition + num4) / ((float) this.itemHeight));
            this.OnItemClicked();
        }
    }

    private void synchronizeScrollbar()
    {
        if (this.scrollbar != null)
        {
            int num = this.items.Length * this.itemHeight;
            float num2 = this.size.y - this.listPadding.vertical;
            this.scrollbar.IncrementAmount = this.itemHeight;
            this.scrollbar.MinValue = 0f;
            this.scrollbar.MaxValue = num;
            this.scrollbar.ScrollSize = num2;
            this.scrollbar.Value = this.scrollPosition;
        }
    }

    public override void Update()
    {
        base.Update();
        if (this.size.magnitude == 0f)
        {
            base.size = new Vector2(200f, 150f);
        }
        if (this.animateHover && (this.hoverIndex != -1))
        {
            float num = (this.hoverIndex * this.itemHeight) * base.PixelsToUnits();
            if (Mathf.Abs((float) (this.hoverTweenLocation - num)) < 1f)
            {
                this.Invalidate();
            }
        }
        if (base.isControlInvalidated)
        {
            this.synchronizeScrollbar();
        }
    }

    private void updateItemHover(dfMouseEventArgs args)
    {
        if (Application.isPlaying)
        {
            RaycastHit hit;
            Ray ray = args.Ray;
            if (!base.collider.Raycast(ray, out hit, 1000f))
            {
                this.hoverIndex = -1;
                this.hoverTweenLocation = 0f;
            }
            else
            {
                Vector2 vector;
                base.GetHitPosition(ray, out vector);
                float num = base.Pivot.TransformToUpperLeft(base.Size).y + ((-this.itemHeight * (this.selectedIndex - this.scrollPosition)) - this.listPadding.top);
                float num2 = (((this.selectedIndex - this.scrollPosition) + 1f) * this.itemHeight) + this.listPadding.vertical;
                float num3 = num2 - this.size.y;
                if (num3 > 0f)
                {
                    num += num3;
                }
                float num4 = vector.y - this.listPadding.top;
                int num5 = ((int) (this.scrollPosition + num4)) / this.itemHeight;
                if (num5 != this.hoverIndex)
                {
                    this.hoverIndex = num5;
                    this.Invalidate();
                }
            }
        }
    }

    public bool AnimateHover
    {
        get
        {
            return this.animateHover;
        }
        set
        {
            this.animateHover = value;
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

    public TextAlignment ItemAlignment
    {
        get
        {
            return this.itemAlignment;
        }
        set
        {
            if (value != this.itemAlignment)
            {
                this.itemAlignment = value;
                this.Invalidate();
            }
        }
    }

    public int ItemHeight
    {
        get
        {
            return this.itemHeight;
        }
        set
        {
            this.scrollPosition = 0f;
            value = Mathf.Max(1, value);
            if (value != this.itemHeight)
            {
                this.itemHeight = value;
                this.Invalidate();
            }
        }
    }

    public string ItemHighlight
    {
        get
        {
            return this.itemHighlight;
        }
        set
        {
            if (value != this.itemHighlight)
            {
                this.itemHighlight = value;
                this.Invalidate();
            }
        }
    }

    public string ItemHover
    {
        get
        {
            return this.itemHover;
        }
        set
        {
            if (value != this.itemHover)
            {
                this.itemHover = value;
                this.Invalidate();
            }
        }
    }

    public RectOffset ItemPadding
    {
        get
        {
            if (this.itemPadding == null)
            {
                this.itemPadding = new RectOffset();
            }
            return this.itemPadding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!value.Equals(this.itemPadding))
            {
                this.itemPadding = value;
                this.Invalidate();
            }
        }
    }

    public string[] Items
    {
        get
        {
            if (this.items == null)
            {
                this.items = new string[0];
            }
            return this.items;
        }
        set
        {
            if (value != this.items)
            {
                this.scrollPosition = 0f;
                if (value == null)
                {
                    value = new string[0];
                }
                this.items = value;
                this.Invalidate();
            }
        }
    }

    public Color32 ItemTextColor
    {
        get
        {
            return this.itemTextColor;
        }
        set
        {
            if (!value.Equals(this.itemTextColor))
            {
                this.itemTextColor = value;
                this.Invalidate();
            }
        }
    }

    public float ItemTextScale
    {
        get
        {
            return this.itemTextScale;
        }
        set
        {
            value = Mathf.Max(0.1f, value);
            if (!Mathf.Approximately(this.itemTextScale, value))
            {
                this.itemTextScale = value;
                this.Invalidate();
            }
        }
    }

    public RectOffset ListPadding
    {
        get
        {
            if (this.listPadding == null)
            {
                this.listPadding = new RectOffset();
            }
            return this.listPadding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.listPadding))
            {
                this.listPadding = value;
                this.Invalidate();
            }
        }
    }

    public dfScrollbar Scrollbar
    {
        get
        {
            return this.scrollbar;
        }
        set
        {
            this.scrollPosition = 0f;
            if (value != this.scrollbar)
            {
                this.detachScrollbarEvents();
                this.scrollbar = value;
                this.attachScrollbarEvents();
                this.Invalidate();
            }
        }
    }

    public float ScrollPosition
    {
        get
        {
            return this.scrollPosition;
        }
        set
        {
            if (!Mathf.Approximately(value, this.scrollPosition))
            {
                this.scrollPosition = this.constrainScrollPosition(value);
                this.Invalidate();
            }
        }
    }

    public int SelectedIndex
    {
        get
        {
            return this.selectedIndex;
        }
        set
        {
            value = Mathf.Max(-1, value);
            value = Mathf.Min(this.items.Length - 1, value);
            if (value != this.selectedIndex)
            {
                this.selectedIndex = value;
                this.EnsureVisible(value);
                this.OnSelectedIndexChanged();
                this.Invalidate();
            }
        }
    }

    public string SelectedItem
    {
        get
        {
            if (this.selectedIndex == -1)
            {
                return null;
            }
            return this.items[this.selectedIndex];
        }
    }

    public string SelectedValue
    {
        get
        {
            return this.items[this.selectedIndex];
        }
        set
        {
            this.selectedIndex = -1;
            for (int i = 0; i < this.items.Length; i++)
            {
                if (this.items[i] == value)
                {
                    this.selectedIndex = i;
                    break;
                }
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
}

