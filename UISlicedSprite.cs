using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite (Sliced)"), ExecuteInEditMode]
public class UISlicedSprite : UIGeometricSprite
{
    public UISlicedSprite() : this(UIWidget.WidgetFlags.CustomBorder)
    {
    }

    protected UISlicedSprite(UIWidget.WidgetFlags additionalFlags) : base(additionalFlags)
    {
    }

    public Vector4 border
    {
        get
        {
            UIAtlas.Sprite sprite = base.sprite;
            if (sprite == null)
            {
                return Vector4.zero;
            }
            Rect outer = sprite.outer;
            Rect inner = sprite.inner;
            Texture mainTexture = base.mainTexture;
            if ((base.atlas.coordinates == UIAtlas.Coordinates.TexCoords) && (mainTexture != null))
            {
                outer = NGUIMath.ConvertToPixels(outer, mainTexture.width, mainTexture.height, true);
                inner = NGUIMath.ConvertToPixels(inner, mainTexture.width, mainTexture.height, true);
            }
            return (Vector4) (new Vector4(inner.xMin - outer.xMin, inner.yMin - outer.yMin, outer.xMax - inner.xMax, outer.yMax - inner.yMax) * base.atlas.pixelSize);
        }
    }

    protected override Vector4 customBorder
    {
        get
        {
            return this.border;
        }
    }
}

