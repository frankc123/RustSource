using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[dfMarkupTagInfo("ol"), dfMarkupTagInfo("ul")]
public class dfMarkupTagList : dfMarkupTag
{
    public dfMarkupTagList() : base("ul")
    {
    }

    public dfMarkupTagList(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        if (base.ChildNodes.Count != 0)
        {
            style.Align = dfMarkupTextAlign.Left;
            dfMarkupBox box = new dfMarkupBox(this, dfMarkupDisplayType.block, style);
            container.AddChild(box);
            this.calculateBulletWidth(style);
            for (int i = 0; i < base.ChildNodes.Count; i++)
            {
                dfMarkupTag tag = base.ChildNodes[i] as dfMarkupTag;
                if ((tag != null) && (tag.TagName == "li"))
                {
                    tag.PerformLayout(box, style);
                }
            }
            box.FitToContents(false);
        }
    }

    private void calculateBulletWidth(dfMarkupStyle style)
    {
        if (base.TagName == "ul")
        {
            Vector2 vector = style.Font.MeasureText("•", style.FontSize, style.FontStyle);
            this.BulletWidth = Mathf.CeilToInt(vector.x);
        }
        else
        {
            int num = 0;
            for (int i = 0; i < base.ChildNodes.Count; i++)
            {
                dfMarkupTag tag = base.ChildNodes[i] as dfMarkupTag;
                if ((tag != null) && (tag.TagName == "li"))
                {
                    num++;
                }
            }
            string text = new string('X', num.ToString().Length) + ".";
            Vector2 vector2 = style.Font.MeasureText(text, style.FontSize, style.FontStyle);
            this.BulletWidth = Mathf.CeilToInt(vector2.x);
        }
    }

    internal int BulletWidth { get; private set; }
}

