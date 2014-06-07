using System;
using UnityEngine;

[dfMarkupTagInfo("font")]
public class dfMarkupTagFont : dfMarkupTag
{
    public dfMarkupTagFont() : base("font")
    {
    }

    public dfMarkupTagFont(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        string[] names = new string[] { "name", "face" };
        dfMarkupAttribute attribute = base.findAttribute(names);
        if (attribute != null)
        {
            dfDynamicFont font1 = dfDynamicFont.FindByName(attribute.Value);
            if (font1 == null)
            {
            }
            style.Font = style.Font;
        }
        string[] textArray2 = new string[] { "size", "font-size" };
        dfMarkupAttribute attribute2 = base.findAttribute(textArray2);
        if (attribute2 != null)
        {
            style.FontSize = dfMarkupStyle.ParseSize(attribute2.Value, style.FontSize);
        }
        string[] textArray3 = new string[] { "color" };
        dfMarkupAttribute attribute3 = base.findAttribute(textArray3);
        if (attribute3 != null)
        {
            style.Color = dfMarkupStyle.ParseColor(attribute3.Value, Color.red);
            style.Color.a = style.Opacity;
        }
        string[] textArray4 = new string[] { "style" };
        dfMarkupAttribute attribute4 = base.findAttribute(textArray4);
        if (attribute4 != null)
        {
            style.FontStyle = dfMarkupStyle.ParseFontStyle(attribute4.Value, style.FontStyle);
        }
        base._PerformLayoutImpl(container, style);
    }
}

