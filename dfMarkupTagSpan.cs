using System;
using System.Collections.Generic;

[dfMarkupTagInfo("span")]
public class dfMarkupTagSpan : dfMarkupTag
{
    private static Queue<dfMarkupTagSpan> objectPool = new Queue<dfMarkupTagSpan>();

    public dfMarkupTagSpan() : base("span")
    {
    }

    public dfMarkupTagSpan(dfMarkupTag original) : base(original)
    {
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
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

    internal static dfMarkupTagSpan Obtain()
    {
        if (objectPool.Count > 0)
        {
            return objectPool.Dequeue();
        }
        return new dfMarkupTagSpan();
    }

    internal override void Release()
    {
        base.Release();
        objectPool.Enqueue(this);
    }
}

