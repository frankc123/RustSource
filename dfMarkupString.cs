using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

public class dfMarkupString : dfMarkupElement
{
    private static StringBuilder buffer = new StringBuilder();
    private bool isWhitespace;
    private static Queue<dfMarkupString> objectPool = new Queue<dfMarkupString>();
    private static Regex whitespacePattern = new Regex(@"\s+");

    public dfMarkupString(string text)
    {
        this.Text = this.processWhitespace(dfMarkupEntity.Replace(text));
        this.isWhitespace = whitespacePattern.IsMatch(this.Text);
    }

    protected override void _PerformLayoutImpl(dfMarkupBox container, dfMarkupStyle style)
    {
        if (style.Font != null)
        {
            string str = (!style.PreserveWhitespace && this.isWhitespace) ? " " : this.Text;
            dfMarkupBoxText box = dfMarkupBoxText.Obtain(this, dfMarkupDisplayType.inline, style);
            box.SetText(str);
            container.AddChild(box);
        }
    }

    internal static dfMarkupString Obtain(string text)
    {
        if (objectPool.Count > 0)
        {
            dfMarkupString str = objectPool.Dequeue();
            str.Text = dfMarkupEntity.Replace(text);
            str.isWhitespace = whitespacePattern.IsMatch(str.Text);
            return str;
        }
        return new dfMarkupString(text);
    }

    private string processWhitespace(string text)
    {
        buffer.Length = 0;
        buffer.Append(text);
        buffer.Replace("\r\n", "\n");
        buffer.Replace("\r", "\n");
        buffer.Replace("\t", "    ");
        return buffer.ToString();
    }

    internal override void Release()
    {
        base.Release();
        objectPool.Enqueue(this);
    }

    internal dfMarkupElement SplitWords()
    {
        dfMarkupTagSpan span = dfMarkupTagSpan.Obtain();
        int num = 0;
        int startIndex = 0;
        int length = this.Text.Length;
        while (num < length)
        {
            while ((num < length) && !char.IsWhiteSpace(this.Text[num]))
            {
                num++;
            }
            if (num > startIndex)
            {
                span.AddChildNode(Obtain(this.Text.Substring(startIndex, num - startIndex)));
                startIndex = num;
            }
            while (((num < length) && (this.Text[num] != '\n')) && char.IsWhiteSpace(this.Text[num]))
            {
                num++;
            }
            if (num > startIndex)
            {
                span.AddChildNode(Obtain(this.Text.Substring(startIndex, num - startIndex)));
                startIndex = num;
            }
            if ((num < length) && (this.Text[num] == '\n'))
            {
                span.AddChildNode(Obtain("\n"));
                startIndex = ++num;
            }
        }
        return span;
    }

    public override string ToString()
    {
        return this.Text;
    }

    public bool IsWhitespace
    {
        get
        {
            return this.isWhitespace;
        }
    }

    public string Text { get; private set; }
}

