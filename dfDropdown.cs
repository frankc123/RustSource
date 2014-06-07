using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Dropdown List")]
public class dfDropdown : dfInteractiveBase, IDFMultiRender
{
    private dfList<dfRenderData> buffers = dfList<dfRenderData>.Obtain();
    private bool eventsAttached;
    [SerializeField]
    protected dfFontBase font;
    [SerializeField]
    protected int itemHeight = 0x19;
    [SerializeField]
    protected string itemHighlight = string.Empty;
    [SerializeField]
    protected string itemHover = string.Empty;
    [SerializeField]
    protected string[] items = new string[0];
    [SerializeField]
    protected string listBackground = string.Empty;
    [SerializeField]
    protected int listHeight = 200;
    [SerializeField]
    protected Vector2 listOffset = Vector2.zero;
    [SerializeField]
    protected RectOffset listPadding = new RectOffset();
    [SerializeField]
    protected PopupListPosition listPosition;
    [SerializeField]
    protected dfScrollbar listScrollbar;
    [SerializeField]
    protected int listWidth;
    [SerializeField]
    protected bool openOnMouseDown;
    private dfListbox popup;
    [SerializeField]
    protected int selectedIndex = -1;
    [SerializeField]
    protected bool shadow;
    [SerializeField]
    protected Color32 shadowColor = Color.black;
    [SerializeField]
    protected Vector2 shadowOffset = new Vector2(1f, -1f);
    [SerializeField]
    protected Color32 textColor = Color.white;
    [SerializeField]
    protected RectOffset textFieldPadding = new RectOffset();
    private dfRenderData textRenderData;
    [SerializeField]
    protected float textScale = 1f;
    [SerializeField]
    protected dfControl triggerButton;

    public event PopupEventHandler DropdownClose;

    public event PopupEventHandler DropdownOpen;

    public event PropertyChangedEventHandler<int> SelectedIndexChanged;

    public void AddItem(string item)
    {
        string[] destinationArray = new string[this.items.Length + 1];
        Array.Copy(this.items, destinationArray, this.items.Length);
        destinationArray[this.items.Length] = item;
        this.items = destinationArray;
    }

    private void attachChildEvents()
    {
        if ((this.triggerButton != null) && !this.eventsAttached)
        {
            this.eventsAttached = true;
            this.triggerButton.Click += new MouseEventHandler(this.trigger_Click);
        }
    }

    private Vector3 calculatePopupPosition(int height)
    {
        float num = base.PixelsToUnits();
        Vector3 vector = base.pivot.TransformToUpperLeft(base.Size);
        Vector3 vector2 = base.transform.position + ((Vector3) (vector * num));
        Vector3 vector3 = base.getScaledDirection(Vector3.down);
        Vector3 vector4 = (Vector3) (base.transformOffset((Vector3) this.listOffset) * num);
        Vector3 vector5 = (vector2 + vector4) + ((Vector3) ((vector3 * base.Size.y) * num));
        Vector3 vector6 = (vector2 + vector4) - ((Vector3) ((vector3 * this.popup.Size.y) * num));
        if (this.listPosition == PopupListPosition.Above)
        {
            return vector6;
        }
        if (this.listPosition != PopupListPosition.Below)
        {
            Vector3 vector7 = ((Vector3) (this.popup.transform.parent.position / num)) + this.popup.Parent.Pivot.TransformToUpperLeft(base.Size);
            Vector3 vector8 = vector7 + ((Vector3) (vector3 * base.parent.Size.y));
            Vector3 vector9 = (Vector3) ((vector5 / num) + (vector3 * this.popup.Size.y));
            if (vector9.y < vector8.y)
            {
                return vector6;
            }
            if (base.GetCamera().WorldToScreenPoint((Vector3) (vector9 * num)).y <= 0f)
            {
                return vector6;
            }
        }
        return vector5;
    }

    private Vector2 calculatePopupSize()
    {
        float x = (this.MaxListWidth <= 0) ? this.size.x : ((float) this.MaxListWidth);
        int b = (this.items.Length * this.itemHeight) + this.listPadding.vertical;
        if (this.items.Length == 0)
        {
            b = (this.itemHeight / 2) + this.listPadding.vertical;
        }
        return new Vector2(x, (float) Mathf.Min(this.MaxListHeight, b));
    }

    private void checkForPopupClose()
    {
        if ((this.popup != null) && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Camera camera = base.GetCamera();
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (!this.popup.collider.Raycast(ray, out hit, camera.farClipPlane) && ((this.popup.Scrollbar == null) || !this.popup.Scrollbar.collider.Raycast(ray, out hit, camera.farClipPlane)))
            {
                this.closePopup(true);
            }
        }
    }

    private void closePopup(bool allowOverride = true)
    {
        if (this.popup != null)
        {
            this.popup.LostFocus -= new FocusEventHandler(this.popup_LostFocus);
            this.popup.SelectedIndexChanged -= new PropertyChangedEventHandler<int>(this.popup_SelectedIndexChanged);
            this.popup.ItemClicked -= new PropertyChangedEventHandler<int>(this.popup_ItemClicked);
            this.popup.KeyDown -= new KeyPressHandler(this.popup_KeyDown);
            if (!allowOverride)
            {
                Object.Destroy(this.popup.gameObject);
                this.popup = null;
            }
            else
            {
                bool overridden = false;
                if (this.DropdownClose != null)
                {
                    this.DropdownClose(this, this.popup, ref overridden);
                }
                if (!overridden)
                {
                    object[] args = new object[] { this, this.popup };
                    overridden = base.Signal("OnDropdownClose", args);
                }
                if (!overridden)
                {
                    Object.Destroy(this.popup.gameObject);
                }
                this.popup = null;
            }
        }
    }

    private void detachChildEvents()
    {
        if ((this.triggerButton != null) && this.eventsAttached)
        {
            this.triggerButton.Click -= new MouseEventHandler(this.trigger_Click);
            this.eventsAttached = false;
        }
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (Application.isPlaying)
        {
            if (!this.eventsAttached)
            {
                this.attachChildEvents();
            }
            if ((this.popup != null) && !this.popup.ContainsFocus)
            {
                this.closePopup(true);
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        this.closePopup(false);
        this.detachChildEvents();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        this.closePopup(false);
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

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        switch (args.KeyCode)
        {
            case KeyCode.UpArrow:
                this.SelectedIndex = Mathf.Max(0, this.selectedIndex - 1);
                break;

            case KeyCode.DownArrow:
                this.SelectedIndex = Mathf.Min((int) (this.items.Length - 1), (int) (this.selectedIndex + 1));
                break;

            case KeyCode.Home:
                this.SelectedIndex = 0;
                break;

            case KeyCode.End:
                this.SelectedIndex = this.items.Length - 1;
                break;

            case KeyCode.Return:
            case KeyCode.Space:
                this.openPopup();
                break;
        }
        base.OnKeyDown(args);
    }

    protected internal override void OnMouseDown(dfMouseEventArgs args)
    {
        if ((this.openOnMouseDown && !args.Used) && ((args.Buttons == dfMouseButtons.Left) && (args.Source == this)))
        {
            args.Use();
            base.OnMouseDown(args);
            if (this.popup != null)
            {
                this.closePopup(true);
            }
            else
            {
                this.openPopup();
            }
        }
        else
        {
            base.OnMouseDown(args);
        }
    }

    protected internal override void OnMouseWheel(dfMouseEventArgs args)
    {
        this.SelectedIndex = Mathf.Max(0, this.SelectedIndex - Mathf.RoundToInt(args.WheelDelta));
        args.Use();
        base.OnMouseWheel(args);
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

    private void openPopup()
    {
        if ((this.popup == null) && (this.items.Length != 0))
        {
            Vector2 vector = this.calculatePopupSize();
            this.popup = base.GetManager().AddControl<dfListbox>();
            this.popup.name = base.name + " - Dropdown List";
            this.popup.gameObject.hideFlags = HideFlags.DontSave;
            this.popup.Atlas = base.Atlas;
            this.popup.Anchor = dfAnchorStyle.Left | dfAnchorStyle.Top;
            this.popup.Font = this.Font;
            this.popup.Pivot = dfPivotPoint.TopLeft;
            this.popup.Size = vector;
            this.popup.Font = this.Font;
            this.popup.ItemHeight = this.ItemHeight;
            this.popup.ItemHighlight = this.ItemHighlight;
            this.popup.ItemHover = this.ItemHover;
            this.popup.ItemPadding = this.TextFieldPadding;
            this.popup.ItemTextColor = this.TextColor;
            this.popup.ItemTextScale = this.TextScale;
            this.popup.Items = this.Items;
            this.popup.ListPadding = this.ListPadding;
            this.popup.BackgroundSprite = this.ListBackground;
            this.popup.Shadow = this.Shadow;
            this.popup.ShadowColor = this.ShadowColor;
            this.popup.ShadowOffset = this.ShadowOffset;
            this.popup.ZOrder = 0x7fffffff;
            if ((vector.y >= this.MaxListHeight) && (this.listScrollbar != null))
            {
                <openPopup>c__AnonStorey54 storey = new <openPopup>c__AnonStorey54();
                storey.activeScrollbar = (Object.Instantiate(this.listScrollbar.gameObject) as GameObject).GetComponent<dfScrollbar>();
                float num = base.PixelsToUnits();
                Vector3 vector2 = this.popup.transform.TransformDirection(Vector3.right);
                Vector3 vector3 = this.popup.transform.position + ((Vector3) ((vector2 * (vector.x - storey.activeScrollbar.Width)) * num));
                storey.activeScrollbar.transform.parent = this.popup.transform;
                storey.activeScrollbar.transform.position = vector3;
                storey.activeScrollbar.Anchor = dfAnchorStyle.Bottom | dfAnchorStyle.Top;
                storey.activeScrollbar.Height = this.popup.Height;
                this.popup.Width -= storey.activeScrollbar.Width;
                this.popup.Scrollbar = storey.activeScrollbar;
                this.popup.SizeChanged += new PropertyChangedEventHandler<Vector2>(storey.<>m__1E);
            }
            Vector3 vector4 = this.calculatePopupPosition((int) this.popup.Size.y);
            this.popup.transform.position = vector4;
            this.popup.transform.rotation = base.transform.rotation;
            this.popup.SelectedIndexChanged += new PropertyChangedEventHandler<int>(this.popup_SelectedIndexChanged);
            this.popup.LostFocus += new FocusEventHandler(this.popup_LostFocus);
            this.popup.ItemClicked += new PropertyChangedEventHandler<int>(this.popup_ItemClicked);
            this.popup.KeyDown += new KeyPressHandler(this.popup_KeyDown);
            this.popup.SelectedIndex = Mathf.Max(0, this.SelectedIndex);
            this.popup.EnsureVisible(this.popup.SelectedIndex);
            this.popup.Focus();
            if (this.DropdownOpen != null)
            {
                bool overridden = false;
                this.DropdownOpen(this, this.popup, ref overridden);
            }
            object[] args = new object[] { this, this.popup };
            base.Signal("OnDropdownOpen", args);
        }
    }

    private void popup_ItemClicked(dfControl control, int selectedIndex)
    {
        this.closePopup(true);
        base.Focus();
    }

    private void popup_KeyDown(dfControl control, dfKeyEventArgs args)
    {
        if ((args.KeyCode == KeyCode.Escape) || (args.KeyCode == KeyCode.Return))
        {
            this.closePopup(true);
            base.Focus();
        }
    }

    private void popup_LostFocus(dfControl control, dfFocusEventArgs args)
    {
        if ((this.popup != null) && !this.popup.ContainsFocus)
        {
            this.closePopup(true);
        }
    }

    private void popup_SelectedIndexChanged(dfControl control, int selectedIndex)
    {
        this.SelectedIndex = selectedIndex;
        this.Invalidate();
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
        this.renderText(this.textRenderData);
        base.isControlInvalidated = false;
        this.updateCollider();
        return this.buffers;
    }

    private void renderText(dfRenderData buffer)
    {
        if ((this.selectedIndex >= 0) && (this.selectedIndex < this.items.Length))
        {
            string text = this.items[this.selectedIndex];
            float num = base.PixelsToUnits();
            Vector2 vector = new Vector2(this.size.x - this.textFieldPadding.horizontal, this.size.y - this.textFieldPadding.vertical);
            Vector3 vector2 = base.pivot.TransformToUpperLeft(base.Size);
            Vector3 vector3 = (Vector3) (new Vector3(vector2.x + this.textFieldPadding.left, vector2.y - this.textFieldPadding.top, 0f) * num);
            Color32 color = !base.IsEnabled ? base.DisabledColor : this.TextColor;
            using (dfFontRendererBase base2 = this.font.ObtainRenderer())
            {
                base2.WordWrap = false;
                base2.MaxSize = vector;
                base2.PixelRatio = num;
                base2.TextScale = this.TextScale;
                base2.VectorOffset = vector3;
                base2.MultiLine = false;
                base2.TextAlign = TextAlignment.Left;
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
                    renderer.SpriteBuffer = buffer;
                }
                base2.Render(text, buffer);
            }
        }
    }

    private void trigger_Click(dfControl control, dfMouseEventArgs mouseEvent)
    {
        if ((mouseEvent.Source == this.triggerButton) && !mouseEvent.Used)
        {
            mouseEvent.Use();
            if (this.popup == null)
            {
                this.openPopup();
            }
            else
            {
                Debug.Log("Close popup");
                this.closePopup(true);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        this.checkForPopupClose();
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
                this.closePopup(true);
                this.font = value;
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
            value = Mathf.Max(1, value);
            if (value != this.itemHeight)
            {
                this.closePopup(true);
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
                this.closePopup(true);
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
            this.closePopup(true);
            if (value == null)
            {
                value = new string[0];
            }
            this.items = value;
            this.Invalidate();
        }
    }

    public string ListBackground
    {
        get
        {
            return this.listBackground;
        }
        set
        {
            if (value != this.listBackground)
            {
                this.closePopup(true);
                this.listBackground = value;
                this.Invalidate();
            }
        }
    }

    public Vector2 ListOffset
    {
        get
        {
            return this.listOffset;
        }
        set
        {
            if (Vector2.Distance(this.listOffset, value) > 1f)
            {
                this.listOffset = value;
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

    public PopupListPosition ListPosition
    {
        get
        {
            return this.listPosition;
        }
        set
        {
            if (value != this.ListPosition)
            {
                this.closePopup(true);
                this.listPosition = value;
                this.Invalidate();
            }
        }
    }

    public dfScrollbar ListScrollbar
    {
        get
        {
            return this.listScrollbar;
        }
        set
        {
            if (value != this.listScrollbar)
            {
                this.listScrollbar = value;
                this.Invalidate();
            }
        }
    }

    public int MaxListHeight
    {
        get
        {
            return this.listHeight;
        }
        set
        {
            this.listHeight = value;
            this.Invalidate();
        }
    }

    public int MaxListWidth
    {
        get
        {
            return this.listWidth;
        }
        set
        {
            this.listWidth = value;
        }
    }

    public bool OpenOnMouseDown
    {
        get
        {
            return this.openOnMouseDown;
        }
        set
        {
            this.openOnMouseDown = value;
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
                if (this.popup != null)
                {
                    this.popup.SelectedIndex = value;
                }
                this.selectedIndex = value;
                this.OnSelectedIndexChanged();
                this.Invalidate();
            }
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

    public Color32 TextColor
    {
        get
        {
            return this.textColor;
        }
        set
        {
            this.closePopup(true);
            this.textColor = value;
            this.Invalidate();
        }
    }

    public RectOffset TextFieldPadding
    {
        get
        {
            if (this.textFieldPadding == null)
            {
                this.textFieldPadding = new RectOffset();
            }
            return this.textFieldPadding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.textFieldPadding))
            {
                this.textFieldPadding = value;
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
                this.closePopup(true);
                this.textScale = value;
                this.Invalidate();
            }
        }
    }

    public dfControl TriggerButton
    {
        get
        {
            return this.triggerButton;
        }
        set
        {
            if (value != this.triggerButton)
            {
                this.detachChildEvents();
                this.triggerButton = value;
                this.attachChildEvents();
                this.Invalidate();
            }
        }
    }

    [CompilerGenerated]
    private sealed class <openPopup>c__AnonStorey54
    {
        internal dfScrollbar activeScrollbar;

        internal void <>m__1E(dfControl control, Vector2 size)
        {
            this.activeScrollbar.Height = control.Height;
        }
    }

    [dfEventCategory("Popup")]
    public delegate void PopupEventHandler(dfDropdown dropdown, dfListbox popup, ref bool overridden);

    public enum PopupListPosition
    {
        Below,
        Above,
        Automatic
    }
}

