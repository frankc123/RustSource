using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Containers/Tab Control/Tab Strip"), ExecuteInEditMode]
public class dfTabstrip : dfControl
{
    [CompilerGenerated]
    private static Func<dfControl, bool> <>f__am$cache8;
    [SerializeField]
    protected bool allowKeyboardNavigation = true;
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected RectOffset layoutPadding = new RectOffset();
    [SerializeField]
    protected dfTabContainer pageContainer;
    [SerializeField]
    protected Vector2 scrollPosition = Vector2.zero;
    [SerializeField]
    protected int selectedIndex;

    public event PropertyChangedEventHandler<int> SelectedIndexChanged;

    public dfControl AddTab(string Text = "")
    {
        if (<>f__am$cache8 == null)
        {
            <>f__am$cache8 = i => i is dfButton;
        }
        dfButton button = base.controls.Where(<>f__am$cache8).FirstOrDefault() as dfButton;
        string str = "Tab " + (base.controls.Count + 1);
        if (string.IsNullOrEmpty(Text))
        {
            Text = str;
        }
        dfButton button2 = base.AddControl<dfButton>();
        button2.name = str;
        button2.Atlas = this.Atlas;
        button2.Text = Text;
        button2.ButtonGroup = this;
        if (button != null)
        {
            button2.Atlas = button.Atlas;
            button2.Font = button.Font;
            button2.AutoSize = button.AutoSize;
            button2.Size = button.Size;
            button2.BackgroundSprite = button.BackgroundSprite;
            button2.DisabledSprite = button.DisabledSprite;
            button2.FocusSprite = button.FocusSprite;
            button2.HoverSprite = button.HoverSprite;
            button2.PressedSprite = button.PressedSprite;
            button2.Shadow = button.Shadow;
            button2.ShadowColor = button.ShadowColor;
            button2.ShadowOffset = button.ShadowOffset;
            button2.TextColor = button.TextColor;
            button2.TextAlignment = button.TextAlignment;
            RectOffset padding = button.Padding;
            button2.Padding = new RectOffset(padding.left, padding.right, padding.top, padding.bottom);
        }
        if (this.pageContainer != null)
        {
            this.pageContainer.AddTabPage();
        }
        this.arrangeTabs();
        this.Invalidate();
        return button2;
    }

    private void arrangeTabs()
    {
        this.SuspendLayout();
        try
        {
            this.layoutPadding = this.layoutPadding.ConstrainPadding();
            float x = this.layoutPadding.left - this.scrollPosition.x;
            float y = this.layoutPadding.top - this.scrollPosition.y;
            float b = 0f;
            float num4 = 0f;
            for (int i = 0; i < base.Controls.Count; i++)
            {
                dfControl control = base.controls[i];
                if ((control.IsVisible && control.enabled) && control.gameObject.activeSelf)
                {
                    Vector2 vector = new Vector2(x, y);
                    control.RelativePosition = (Vector3) vector;
                    float a = control.Width + this.layoutPadding.horizontal;
                    float num7 = control.Height + this.layoutPadding.vertical;
                    b = Mathf.Max(a, b);
                    num4 = Mathf.Max(num7, num4);
                    x += a;
                }
            }
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    private void attachEvents(dfControl control)
    {
        control.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.control_IsVisibleChanged);
        control.PositionChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.ZOrderChanged += new PropertyChangedEventHandler<int>(this.childControlZOrderChanged);
    }

    private void childControlInvalidated(dfControl control, Vector2 value)
    {
        this.onChildControlInvalidatedLayout();
    }

    private void childControlZOrderChanged(dfControl control, int value)
    {
        this.onChildControlInvalidatedLayout();
    }

    private void control_IsVisibleChanged(dfControl control, bool value)
    {
        this.onChildControlInvalidatedLayout();
    }

    private void detachEvents(dfControl control)
    {
        control.IsVisibleChanged -= new PropertyChangedEventHandler<bool>(this.control_IsVisibleChanged);
        control.PositionChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.SizeChanged -= new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
    }

    public void DisableTab(int index)
    {
        if ((this.selectedIndex >= 0) && (this.selectedIndex <= (base.controls.Count - 1)))
        {
            base.controls[index].Disable();
        }
    }

    public void EnableTab(int index)
    {
        if ((this.selectedIndex >= 0) && (this.selectedIndex <= (base.controls.Count - 1)))
        {
            base.controls[index].Enable();
        }
    }

    private void onChildControlInvalidatedLayout()
    {
        if (!base.IsLayoutSuspended)
        {
            this.arrangeTabs();
            this.Invalidate();
        }
    }

    protected internal override void OnClick(dfMouseEventArgs args)
    {
        if (base.controls.Contains(args.Source))
        {
            this.SelectedIndex = args.Source.ZOrder;
        }
        base.OnClick(args);
    }

    private void OnClick(dfControl sender, dfMouseEventArgs args)
    {
        if (base.controls.Contains(args.Source))
        {
            this.SelectedIndex = args.Source.ZOrder;
        }
    }

    protected internal override void OnControlAdded(dfControl child)
    {
        base.OnControlAdded(child);
        this.attachEvents(child);
        this.arrangeTabs();
    }

    protected internal override void OnControlRemoved(dfControl child)
    {
        base.OnControlRemoved(child);
        this.detachEvents(child);
        this.arrangeTabs();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (this.size.sqrMagnitude < float.Epsilon)
        {
            base.Size = new Vector2(256f, 26f);
        }
        if (Application.isPlaying)
        {
            this.selectTabByIndex(Mathf.Max(this.selectedIndex, 0));
        }
    }

    protected internal override void OnGotFocus(dfFocusEventArgs args)
    {
        if (base.controls.Contains(args.GotFocus))
        {
            this.SelectedIndex = args.GotFocus.ZOrder;
        }
        base.OnGotFocus(args);
    }

    protected internal override void OnKeyDown(dfKeyEventArgs args)
    {
        if (!args.Used)
        {
            if (this.allowKeyboardNavigation)
            {
                if ((args.KeyCode == KeyCode.LeftArrow) || ((args.KeyCode == KeyCode.Tab) && args.Shift))
                {
                    this.SelectedIndex = Mathf.Max(0, this.SelectedIndex - 1);
                    args.Use();
                    return;
                }
                if ((args.KeyCode == KeyCode.RightArrow) || (args.KeyCode == KeyCode.Tab))
                {
                    this.SelectedIndex++;
                    args.Use();
                    return;
                }
            }
            base.OnKeyDown(args);
        }
    }

    protected internal override void OnLostFocus(dfFocusEventArgs args)
    {
        base.OnLostFocus(args);
        if (base.controls.Contains(args.LostFocus))
        {
            this.showSelectedTab();
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

    protected internal virtual void OnSelectedIndexChanged()
    {
        object[] args = new object[] { this.SelectedIndex };
        base.SignalHierarchy("OnSelectedIndexChanged", args);
        if (this.SelectedIndexChanged != null)
        {
            this.SelectedIndexChanged(this, this.SelectedIndex);
        }
    }

    private void selectTabByIndex(int value)
    {
        value = Mathf.Max(Mathf.Min(value, base.controls.Count - 1), -1);
        if (value != this.selectedIndex)
        {
            this.selectedIndex = value;
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfButton button = base.controls[i] as dfButton;
                if (button != null)
                {
                    if (i == value)
                    {
                        button.State = dfButton.ButtonState.Focus;
                    }
                    else
                    {
                        button.State = dfButton.ButtonState.Default;
                    }
                }
            }
            this.Invalidate();
            this.OnSelectedIndexChanged();
            if (this.pageContainer != null)
            {
                this.pageContainer.SelectedIndex = value;
            }
        }
    }

    private void showSelectedTab()
    {
        if ((this.selectedIndex >= 0) && (this.selectedIndex <= (base.controls.Count - 1)))
        {
            dfButton button = base.controls[this.selectedIndex] as dfButton;
            if ((button != null) && !button.ContainsMouse)
            {
                button.State = dfButton.ButtonState.Focus;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (base.isControlInvalidated)
        {
            this.arrangeTabs();
        }
        this.showSelectedTab();
    }

    public bool AllowKeyboardNavigation
    {
        get
        {
            return this.allowKeyboardNavigation;
        }
        set
        {
            this.allowKeyboardNavigation = value;
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

    public RectOffset LayoutPadding
    {
        get
        {
            if (this.layoutPadding == null)
            {
                this.layoutPadding = new RectOffset();
            }
            return this.layoutPadding;
        }
        set
        {
            value = value.ConstrainPadding();
            if (!object.Equals(value, this.layoutPadding))
            {
                this.layoutPadding = value;
                this.arrangeTabs();
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
            if (value != this.selectedIndex)
            {
                this.selectTabByIndex(value);
            }
        }
    }

    public dfTabContainer TabPages
    {
        get
        {
            return this.pageContainer;
        }
        set
        {
            if (this.pageContainer != value)
            {
                this.pageContainer = value;
                if (value != null)
                {
                    while (value.Controls.Count < base.controls.Count)
                    {
                        value.AddTabPage();
                    }
                }
                this.pageContainer.SelectedIndex = this.SelectedIndex;
                this.Invalidate();
            }
        }
    }
}

