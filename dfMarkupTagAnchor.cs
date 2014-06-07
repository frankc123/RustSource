using System;

[dfMarkupTagInfo("a")]
public class dfMarkupTagAnchor : dfMarkupTag
{
    public dfMarkupTagAnchor() : base("a")
    {
    }

    public dfMarkupTagAnchor(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        style.TextDecoration = dfMarkupTextDecoration.Underline;
        style = base.applyTextStyleAttributes(style);
        for (int i = 0; i < base.ChildNodes.Count; i++)
        {
            dfMarkupElement element = base.ChildNodes[i];
            if (element is dfMarkupString)
            {
                dfMarkupString str = element as dfMarkupString;
                if (str.Text == "\n")
                {
                    if (style.PreserveWhitespace)
                    {
                        container.AddLineBreak();
                    }
                    continue;
                }
            }
            element.PerformLayout(container, style);
        }
    }

    public string HRef
    {
        get
        {
            string[] names = new string[] { "href" };
            dfMarkupAttribute attribute = base.findAttribute(names);
            return ((attribute == null) ? string.Empty : attribute.Value);
        }
    }
}

