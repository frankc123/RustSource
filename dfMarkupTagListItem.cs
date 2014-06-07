using System;
using UnityEngine;

[dfMarkupTagInfo("li")]
public class dfMarkupTagListItem : dfMarkupTag
{
    public dfMarkupTagListItem() : base("li")
    {
    }

    public dfMarkupTagListItem(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        if (base.ChildNodes.Count != 0)
        {
            float x = container.Size.x;
            dfMarkupBox box = new dfMarkupBox(this, dfMarkupDisplayType.listItem, style);
            box.Margins.top = 10;
            container.AddChild(box);
            dfMarkupTagList parent = base.Parent as dfMarkupTagList;
            if (parent == null)
            {
                base._PerformLayoutImpl(container, style);
            }
            else
            {
                style.VerticalAlign = dfMarkupVerticalAlign.Baseline;
                string str = "•";
                if (parent.TagName == "ol")
                {
                    str = container.Children.Count + ".";
                }
                dfMarkupStyle style2 = style;
                style2.VerticalAlign = dfMarkupVerticalAlign.Baseline;
                style2.Align = dfMarkupTextAlign.Right;
                dfMarkupBoxText text = dfMarkupBoxText.Obtain(this, dfMarkupDisplayType.inlineBlock, style2);
                text.SetText(str);
                text.Width = parent.BulletWidth;
                text.Margins.left = style.FontSize * 2;
                box.AddChild(text);
                dfMarkupBox box2 = new dfMarkupBox(this, dfMarkupDisplayType.inlineBlock, style);
                int fontSize = style.FontSize;
                float num3 = ((x - text.Size.x) - text.Margins.left) - fontSize;
                box2.Size = new Vector2(num3, (float) fontSize);
                box2.Margins.left = (int) (style.FontSize * 0.5f);
                box.AddChild(box2);
                for (int i = 0; i < base.ChildNodes.Count; i++)
                {
                    base.ChildNodes[i].PerformLayout(box2, style);
                }
                box2.FitToContents(false);
                box2.Parent.FitToContents(false);
                box.FitToContents(false);
            }
        }
    }
}

