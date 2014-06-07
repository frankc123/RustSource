using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Color")]
public class TweenColor : UITweener
{
    public Color from = Color.white;
    [NonSerialized]
    public bool isFullscreen;
    private Light mLight;
    private Material mMat;
    private Transform mTrans;
    private UIWidget mWidget;
    public Color to = Color.white;

    private void Awake()
    {
        this.mWidget = base.GetComponentInChildren<UIWidget>();
        Renderer renderer = base.renderer;
        if (renderer != null)
        {
            this.mMat = renderer.material;
        }
        this.mLight = base.light;
    }

    public static TweenColor Begin(GameObject go, float duration, Color color)
    {
        TweenColor color2 = UITweener.Begin<TweenColor>(go, duration);
        color2.from = color2.color;
        color2.to = color;
        return color2;
    }

    protected override void OnUpdate(float factor)
    {
        Color color = (Color) ((this.from * (1f - factor)) + (this.to * factor));
        if (this.isFullscreen)
        {
            GameFullscreen instance = ImageEffectManager.GetInstance<GameFullscreen>();
            if (instance != null)
            {
                instance.autoFadeColor = color;
            }
            color.a = 0f;
        }
        this.color = color;
    }

    public Color color
    {
        get
        {
            if (this.mWidget != null)
            {
                return this.mWidget.color;
            }
            if (this.mLight != null)
            {
                return this.mLight.color;
            }
            if (this.mMat != null)
            {
                return this.mMat.color;
            }
            return Color.black;
        }
        set
        {
            if (this.mWidget != null)
            {
                this.mWidget.color = value;
            }
            if (this.mMat != null)
            {
                this.mMat.color = value;
            }
            if (this.mLight != null)
            {
                this.mLight.color = value;
                this.mLight.enabled = ((value.r + value.g) + value.b) > 0.01f;
            }
        }
    }
}

