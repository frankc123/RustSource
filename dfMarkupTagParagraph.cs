using System;

[dfMarkupTagInfo("p")]
public class dfMarkupTagParagraph : dfMarkupTag
{
    public dfMarkupTagParagraph() : base("p")
    {
    }

    public dfMarkupTagParagraph(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        if (base.ChildNodes.Count != 0)
        {
            style = base.applyTextStyleAttributes(style);
            int top = (container.Children.Count != 0) ? style.LineHeight : 0;
            dfMarkupBox box = null;
            if (style.BackgroundColor.a > 0.005f)
            {
                dfMarkupBoxSprite sprite = new dfMarkupBoxSprite(this, dfMarkupDisplayType.block, style) {
                    Atlas = base.Owner.Atlas,
                    Source = base.Owner.BlankTextureSprite
                };
                sprite.Style.Color = style.BackgroundColor;
                box = sprite;
            }
            else
            {
                box = new dfMarkupBox(this, dfMarkupDisplayType.block, style);
            }
            box.Margins = new dfMarkupBorders(0, 0, top, style.LineHeight);
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
            if (box.Children.Count > 0)
            {
                box.Children[box.Children.Count - 1].IsNewline = true;
            }
            box.FitToContents(true);
        }
    }
}

