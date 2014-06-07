using System;
using UnityEngine;

[Serializable, ExecuteInEditMode, RequireComponent(typeof(BoxCollider))]
public class dfInteractiveBase : dfControl
{
    [SerializeField]
    protected dfAtlas atlas;
    [SerializeField]
    protected string backgroundSprite;
    [SerializeField]
    protected string disabledSprite;
    [SerializeField]
    protected string focusSprite;
    [SerializeField]
    protected string hoverSprite;

    public override Vector2 CalculateMinimumSize()
    {
        dfAtlas.ItemInfo info = this.getBackgroundSprite();
        if (info == null)
        {
            return base.CalculateMinimumSize();
        }
        RectOffset border = info.border;
        if ((border.horizontal <= 0) && (border.vertical <= 0))
        {
            return base.CalculateMinimumSize();
        }
        return Vector2.Max(base.CalculateMinimumSize(), new Vector2((float) border.horizontal, (float) border.vertical));
    }

    protected virtual Color32 getActiveColor()
    {
        if (base.IsEnabled)
        {
            return base.color;
        }
        if ((!string.IsNullOrEmpty(this.disabledSprite) && (this.Atlas != null)) && (this.Atlas[this.DisabledSprite] != null))
        {
            return base.color;
        }
        return base.disabledColor;
    }

    protected internal virtual dfAtlas.ItemInfo getBackgroundSprite()
    {
        if (this.Atlas == null)
        {
            return null;
        }
        if (!base.IsEnabled)
        {
            dfAtlas.ItemInfo info = this.atlas[this.DisabledSprite];
            if (info != null)
            {
                return info;
            }
            return this.atlas[this.BackgroundSprite];
        }
        if (this.HasFocus)
        {
            dfAtlas.ItemInfo info2 = this.atlas[this.FocusSprite];
            if (info2 != null)
            {
                return info2;
            }
            return this.atlas[this.BackgroundSprite];
        }
        if (base.isMouseHovering)
        {
            dfAtlas.ItemInfo info3 = this.atlas[this.HoverSprite];
            if (info3 != null)
            {
                return info3;
            }
        }
        return this.Atlas[this.BackgroundSprite];
    }

    protected internal override void OnGotFocus(dfFocusEventArgs args)
    {
        base.OnGotFocus(args);
        this.Invalidate();
    }

    protected internal override void OnLostFocus(dfFocusEventArgs args)
    {
        base.OnLostFocus(args);
        this.Invalidate();
    }

    protected internal override void OnMouseEnter(dfMouseEventArgs args)
    {
        base.OnMouseEnter(args);
        this.Invalidate();
    }

    protected internal override void OnMouseLeave(dfMouseEventArgs args)
    {
        base.OnMouseLeave(args);
        this.Invalidate();
    }

    protected internal virtual void renderBackground()
    {
        if (this.Atlas != null)
        {
            dfAtlas.ItemInfo info = this.getBackgroundSprite();
            if (info != null)
            {
                Color32 color = base.ApplyOpacity(this.getActiveColor());
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

    public override bool CanFocus
    {
        get
        {
            return ((base.IsEnabled && base.IsVisible) || base.CanFocus);
        }
    }

    public string DisabledSprite
    {
        get
        {
            return this.disabledSprite;
        }
        set
        {
            if (value != this.disabledSprite)
            {
                this.disabledSprite = value;
                this.Invalidate();
            }
        }
    }

    public string FocusSprite
    {
        get
        {
            return this.focusSprite;
        }
        set
        {
            if (value != this.focusSprite)
            {
                this.focusSprite = value;
                this.Invalidate();
            }
        }
    }

    public string HoverSprite
    {
        get
        {
            return this.hoverSprite;
        }
        set
        {
            if (value != this.hoverSprite)
            {
                this.hoverSprite = value;
                this.Invalidate();
            }
        }
    }
}

