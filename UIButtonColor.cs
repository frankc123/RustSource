using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : MonoBehaviour
{
    public float duration = 0.2f;
    public Color hover = new Color(0.6f, 1f, 0.2f, 1f);
    protected Color mColor;
    protected bool mHighlighted;
    protected bool mInitDone;
    protected bool mStarted;
    public Color pressed = Color.grey;
    public GameObject tweenTarget;

    protected void Init()
    {
        this.mInitDone = true;
        if (this.tweenTarget == null)
        {
            this.tweenTarget = base.gameObject;
        }
        UIWidget component = this.tweenTarget.GetComponent<UIWidget>();
        if (component != null)
        {
            this.mColor = component.color;
        }
        else
        {
            Renderer renderer = this.tweenTarget.renderer;
            if (renderer != null)
            {
                this.mColor = renderer.material.color;
            }
            else
            {
                Light light = this.tweenTarget.light;
                if (light != null)
                {
                    this.mColor = light.color;
                }
                else
                {
                    Debug.LogWarning(NGUITools.GetHierarchy(base.gameObject) + " has nothing for UIButtonColor to color", this);
                    base.enabled = false;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (this.tweenTarget != null)
        {
            TweenColor component = this.tweenTarget.GetComponent<TweenColor>();
            if (component != null)
            {
                component.color = this.mColor;
                component.enabled = false;
            }
        }
    }

    protected virtual void OnEnable()
    {
        if (this.mStarted && this.mHighlighted)
        {
            this.OnHover(UICamera.IsHighlighted(base.gameObject));
        }
    }

    protected virtual void OnHover(bool isOver)
    {
        if (base.enabled)
        {
            if (!this.mInitDone)
            {
                this.Init();
            }
            TweenColor.Begin(this.tweenTarget, this.duration, !isOver ? this.mColor : this.hover);
            this.mHighlighted = isOver;
        }
    }

    protected virtual void OnPress(bool isPressed)
    {
        if (!this.mInitDone)
        {
            this.Init();
        }
        if (base.enabled)
        {
            TweenColor.Begin(this.tweenTarget, this.duration, !isPressed ? (!UICamera.IsHighlighted(base.gameObject) ? this.mColor : this.hover) : this.pressed);
        }
    }

    private void Start()
    {
        this.mStarted = true;
        if (!this.mInitDone)
        {
            this.Init();
        }
    }

    public Color defaultColor
    {
        get
        {
            return this.mColor;
        }
        set
        {
            this.mColor = value;
        }
    }
}

