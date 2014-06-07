using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
    public Color disabledColor = Color.grey;

    protected override void OnEnable()
    {
        if (this.isEnabled)
        {
            base.OnEnable();
        }
        else
        {
            this.UpdateColor(false, true);
        }
    }

    protected override void OnHover(bool isOver)
    {
        if (this.isEnabled)
        {
            base.OnHover(isOver);
        }
    }

    protected override void OnPress(bool isPressed)
    {
        if (this.isEnabled)
        {
            base.OnPress(isPressed);
        }
    }

    public void UpdateColor(bool shouldBeEnabled, bool immediate)
    {
        if (base.tweenTarget != null)
        {
            if (!base.mInitDone)
            {
                base.Init();
            }
            Color color = !shouldBeEnabled ? this.disabledColor : base.defaultColor;
            TweenColor color2 = TweenColor.Begin(base.tweenTarget, 0.15f, color);
            if (immediate)
            {
                color2.color = color;
                color2.enabled = false;
            }
        }
    }

    public bool isEnabled
    {
        get
        {
            return NGUITools.GetAllowClick(this);
        }
        set
        {
            bool flag;
            bool allowClick = NGUITools.GetAllowClick(this, out flag);
            if (flag && (allowClick != value))
            {
                NGUITools.SetAllowClick(this, value);
                this.UpdateColor(value, false);
            }
        }
    }
}

