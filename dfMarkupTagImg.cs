using System;
using UnityEngine;

[dfMarkupTagInfo("img")]
public class dfMarkupTagImg : dfMarkupTag
{
    public dfMarkupTagImg() : base("img")
    {
        this.IsClosedTag = true;
    }

    public dfMarkupTagImg(dfMarkupTag original) : base(original)
    {
        this.IsClosedTag = true;
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        if (base.Owner == null)
        {
            Debug.LogError("Tag has no parent: " + this);
        }
        else
        {
            style = this.applyStyleAttributes(style);
            string[] names = new string[] { "src" };
            dfMarkupAttribute attribute = base.findAttribute(names);
            if (attribute != null)
            {
                string source = attribute.Value;
                dfMarkupBox box = this.createImageBox(base.Owner.Atlas, source, style);
                if (box != null)
                {
                    Vector2 zero = Vector2.zero;
                    string[] textArray2 = new string[] { "height" };
                    dfMarkupAttribute attribute2 = base.findAttribute(textArray2);
                    if (attribute2 != null)
                    {
                        zero.y = dfMarkupStyle.ParseSize(attribute2.Value, (int) box.Size.y);
                    }
                    string[] textArray3 = new string[] { "width" };
                    dfMarkupAttribute attribute3 = base.findAttribute(textArray3);
                    if (attribute3 != null)
                    {
                        zero.x = dfMarkupStyle.ParseSize(attribute3.Value, (int) box.Size.x);
                    }
                    if (zero.sqrMagnitude <= float.Epsilon)
                    {
                        zero = box.Size;
                    }
                    else if (zero.x <= float.Epsilon)
                    {
                        zero.x = zero.y * (box.Size.x / box.Size.y);
                    }
                    else if (zero.y <= float.Epsilon)
                    {
                        zero.y = zero.x * (box.Size.y / box.Size.x);
                    }
                    box.Size = zero;
                    box.Baseline = (int) zero.y;
                    container.AddChild(box);
                }
            }
        }
    }

    private dfMarkupStyle applyStyleAttributes(dfMarkupStyle style)
    {
        string[] names = new string[] { "valign" };
        dfMarkupAttribute attribute = base.findAttribute(names);
        if (attribute != null)
        {
            style.VerticalAlign = dfMarkupStyle.ParseVerticalAlignment(attribute.Value);
        }
        string[] textArray2 = new string[] { "color" };
        dfMarkupAttribute attribute2 = base.findAttribute(textArray2);
        if (attribute2 != null)
        {
            Color color = dfMarkupStyle.ParseColor(attribute2.Value, (Color) base.Owner.Color);
            color.a = style.Opacity;
            style.Color = color;
        }
        return style;
    }

    private dfMarkupBox createImageBox(dfAtlas atlas, string source, dfMarkupStyle style)
    {
        if (!source.ToLowerInvariant().StartsWith("http://"))
        {
            if ((atlas != null) && (atlas[source] != null))
            {
                dfMarkupBoxSprite sprite = new dfMarkupBoxSprite(this, dfMarkupDisplayType.inline, style);
                sprite.LoadImage(atlas, source);
                return sprite;
            }
            Texture texture = dfMarkupImageCache.Load(source);
            if (texture != null)
            {
                dfMarkupBoxTexture texture2 = new dfMarkupBoxTexture(this, dfMarkupDisplayType.inline, style);
                texture2.LoadTexture(texture);
                return texture2;
            }
        }
        return null;
    }
}

