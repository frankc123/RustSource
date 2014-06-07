using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[dfMarkupTagInfo("h4"), dfMarkupTagInfo("h6"), dfMarkupTagInfo("h5"), dfMarkupTagInfo("h1"), dfMarkupTagInfo("h2"), dfMarkupTagInfo("h3")]
public class dfMarkupTagHeading : dfMarkupTag
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$mapE;

    public dfMarkupTagHeading() : base("h1")
    {
    }

    public dfMarkupTagHeading(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        dfMarkupBorders margins = new dfMarkupBorders();
        dfMarkupStyle style2 = this.applyDefaultStyles(style, ref margins);
        style2 = base.applyTextStyleAttributes(style2);
        string[] names = new string[] { "margin" };
        dfMarkupAttribute attribute = base.findAttribute(names);
        if (attribute != null)
        {
            margins = dfMarkupBorders.Parse(attribute.Value);
        }
        dfMarkupBox box = new dfMarkupBox(this, dfMarkupDisplayType.block, style2) {
            Margins = margins
        };
        container.AddChild(box);
        base._PerformLayoutImpl(box, style2);
        box.FitToContents(false);
    }

    private dfMarkupStyle applyDefaultStyles(dfMarkupStyle style, ref dfMarkupBorders margins)
    {
        float num = 1f;
        float num2 = 1f;
        string tagName = base.TagName;
        if (tagName != null)
        {
            int num3;
            if (<>f__switch$mapE == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
                dictionary.Add("h1", 0);
                dictionary.Add("h2", 1);
                dictionary.Add("h3", 2);
                dictionary.Add("h4", 3);
                dictionary.Add("h5", 4);
                dictionary.Add("h6", 5);
                <>f__switch$mapE = dictionary;
            }
            if (<>f__switch$mapE.TryGetValue(tagName, out num3))
            {
                switch (num3)
                {
                    case 0:
                        num2 = 2f;
                        num = 0.65f;
                        break;

                    case 1:
                        num2 = 1.5f;
                        num = 0.75f;
                        break;

                    case 2:
                        num2 = 1.35f;
                        num = 0.85f;
                        break;

                    case 3:
                        num2 = 1.15f;
                        num = 0f;
                        break;

                    case 4:
                        num2 = 0.85f;
                        num = 1.5f;
                        break;

                    case 5:
                        num2 = 0.75f;
                        num = 1.75f;
                        break;
                }
            }
        }
        style.FontSize = (int) (style.FontSize * num2);
        style.FontStyle = FontStyle.Bold;
        style.Align = dfMarkupTextAlign.Left;
        num *= style.FontSize;
        margins.top = margins.bottom = (int) num;
        return style;
    }
}

