using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable, AddComponentMenu("Daikon Forge/User Interface/Containers/Tab Control/Tab Page Container"), ExecuteInEditMode]
public class dfTabContainer : dfControl
{
    [CompilerGenerated]
    private static Func<dfControl, bool> <>f__am$cache5;
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected RectOffset padding = new RectOffset();
    [SerializeField]
    protected int selectedIndex;

    public event PropertyChangedEventHandler<int> SelectedIndexChanged;

    public dfControl AddTabPage()
    {
        if (<>f__am$cache5 == null)
        {
            <>f__am$cache5 = i => i is dfPanel;
        }
        dfPanel panel = base.controls.Where(<>f__am$cache5).FirstOrDefault() as dfPanel;
        string str = "Tab Page " + (base.controls.Count + 1);
        dfPanel panel2 = base.AddControl<dfPanel>();
        panel2.name = str;
        panel2.Atlas = this.Atlas;
        panel2.Anchor = dfAnchorStyle.All;
        panel2.ClipChildren = true;
        if (panel != null)
        {
            panel2.Atlas = panel.Atlas;
            panel2.BackgroundSprite = panel.BackgroundSprite;
        }
        this.arrangeTabPages();
        this.Invalidate();
        return panel2;
    }

    private void arrangeTabPages()
    {
        if (this.padding == null)
        {
            this.padding = new RectOffset(0, 0, 0, 0);
        }
        Vector3 vector = new Vector3((float) this.padding.left, (float) this.padding.top);
        Vector2 vector2 = new Vector2(this.size.x - this.padding.horizontal, this.size.y - this.padding.vertical);
        for (int i = 0; i < base.controls.Count; i++)
        {
            dfPanel panel = base.controls[i] as dfPanel;
            if (panel != null)
            {
                panel.Size = vector2;
                panel.RelativePosition = vector;
            }
        }
    }

    private void attachEvents(dfControl control)
    {
        control.IsVisibleChanged += new PropertyChangedEventHandler<bool>(this.control_IsVisibleChanged);
        control.PositionChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
        control.SizeChanged += new PropertyChangedEventHandler<Vector2>(this.childControlInvalidated);
    }

    private void childControlInvalidated(dfControl control, Vector2 value)
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

    private void onChildControlInvalidatedLayout()
    {
        if (!base.IsLayoutSuspended)
        {
            this.arrangeTabPages();
            this.Invalidate();
        }
    }

    protected internal override void OnControlAdded(dfControl child)
    {
        base.OnControlAdded(child);
        this.attachEvents(child);
        this.arrangeTabPages();
    }

    protected internal override void OnControlRemoved(dfControl child)
    {
        base.OnControlRemoved(child);
        this.detachEvents(child);
        this.arrangeTabPages();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (this.size.sqrMagnitude < float.Epsilon)
        {
            base.Size = new Vector2(256f, 256f);
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

    protected internal virtual void OnSelectedIndexChanged(int Index)
    {
        object[] args = new object[] { Index };
        base.SignalHierarchy("OnSelectedIndexChanged", args);
        if (this.SelectedIndexChanged != null)
        {
            this.SelectedIndexChanged(this, Index);
        }
    }

    private void selectPageByIndex(int value)
    {
        value = Mathf.Max(Mathf.Min(value, base.controls.Count - 1), -1);
        if (value != this.selectedIndex)
        {
            this.selectedIndex = value;
            for (int i = 0; i < base.controls.Count; i++)
            {
                dfControl control = base.controls[i];
                if (control != null)
                {
                    control.IsVisible = i == value;
                }
            }
            this.arrangeTabPages();
            this.Invalidate();
            this.OnSelectedIndexChanged(value);
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
                this.arrangeTabPages();
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
                this.selectPageByIndex(value);
            }
        }
    }
}

