using System;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
    public string hoverSprite;
    public string normalSprite;
    public string pressedSprite;
    public UISprite target;

    private void OnHover(bool isOver)
    {
        if (base.enabled && (this.target != null))
        {
            this.target.spriteName = !isOver ? this.normalSprite : this.hoverSprite;
            this.target.MakePixelPerfect();
        }
    }

    private void OnPress(bool pressed)
    {
        if (base.enabled && (this.target != null))
        {
            this.target.spriteName = !pressed ? this.normalSprite : this.pressedSprite;
            this.target.MakePixelPerfect();
        }
    }

    private void Start()
    {
        if (this.target == null)
        {
            this.target = base.GetComponentInChildren<UISprite>();
        }
    }
}

