using System;

[dfMarkupTagInfo("pre")]
public class dfMarkupTagPre : dfMarkupTag
{
    public dfMarkupTagPre() : base("pre")
    {
    }

    public dfMarkupTagPre(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        style = base.applyTextStyleAttributes(style);
        style.PreserveWhitespace = true;
        style.Preformatted = true;
        if (style.Align == dfMarkupTextAlign.Justify)
        {
            style.Align = dfMarkupTextAlign.Left;
        }
        dfMarkupBox box = null;
        if (style.BackgroundColor.a > 0.1f)
        {
            dfMarkupBoxSprite sprite = new dfMarkupBoxSprite(this, dfMarkupDisplayType.block, style);
            sprite.LoadImage(base.Owner.Atlas, base.Owner.BlankTextureSprite);
            sprite.Style.Color = style.BackgroundColor;
            box = sprite;
        }
        else
        {
            box = new dfMarkupBox(this, dfMarkupDisplayType.block, style);
        }
        string[] names = new string[] { "margin" };
        dfMarkupAttribute attribute = base.findAttribute(names);
        if (attribute != null)
        {
            box.Margins = dfMarkupBorders.Parse(attribute.Value);
        }
        string[] textArray2 = new string[] { "padding" };
        dfMarkupAttribute attribute2 = base.findAttribute(textArray2);
        if (attribute2 != null)
        {
            box.Padding = dfMarkupBorders.Parse(attribute2.Value);
        }
        container.AddChild(box);
        base._PerformLayoutImpl(box, style);
        box.FitToContents(false);
    }
}

