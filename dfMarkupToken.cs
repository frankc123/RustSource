using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

public class dfMarkupToken
{
    private static List<dfMarkupToken> pool = new List<dfMarkupToken>();
    private static int poolIndex = 0;
    private int startAttributeIndex;
    private string value;

    protected dfMarkupToken()
    {
    }

    internal void AddAttribute(dfMarkupToken key, dfMarkupToken value)
    {
        dfMarkupTokenAttribute attribute = dfMarkupTokenAttribute.Obtain(key, value);
        if (this.AttributeCount == 0)
        {
            this.startAttributeIndex = attribute.Index;
        }
        this.AttributeCount++;
    }

    public dfMarkupTokenAttribute GetAttribute(int index)
    {
        if (index < this.AttributeCount)
        {
            return dfMarkupTokenAttribute.GetAttribute(this.startAttributeIndex + index);
        }
        return null;
    }

    public bool Matches(string text)
    {
        if (this.Length != text.Length)
        {
            return false;
        }
        int length = text.Length;
        for (int i = 0; i < length; i++)
        {
            if (char.ToLowerInvariant(text[i]) != char.ToLowerInvariant(this[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static dfMarkupToken Obtain(string source, dfMarkupTokenType type, int startIndex, int endIndex)
    {
        if (poolIndex >= (pool.Count - 1))
        {
            pool.Add(new dfMarkupToken());
        }
        dfMarkupToken token = pool[poolIndex++];
        token.Source = source;
        token.TokenType = type;
        token.value = null;
        token.StartOffset = startIndex;
        token.EndOffset = endIndex;
        token.AttributeCount = 0;
        token.startAttributeIndex = 0;
        token.Width = 0;
        token.Height = 0;
        return token;
    }

    public static void Reset()
    {
        poolIndex = 0;
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public int AttributeCount { get; private set; }

    public int EndOffset { get; private set; }

    public int Height { get; set; }

    public char this[int index]
    {
        get
        {
            if ((index >= 0) && ((this.StartOffset + index) <= (this.Source.Length - 1)))
            {
                return this.Source[this.StartOffset + index];
            }
            return '\0';
        }
    }

    public int Length
    {
        get
        {
            return ((this.EndOffset - this.StartOffset) + 1);
        }
    }

    public string Source { get; private set; }

    public int StartOffset { get; private set; }

    public dfMarkupTokenType TokenType { get; private set; }

    public string Value
    {
        get
        {
            if (this.value == null)
            {
                int length = Math.Min((int) ((this.EndOffset - this.StartOffset) + 1), (int) (this.Source.Length - this.StartOffset));
                this.value = this.Source.Substring(this.StartOffset, length);
            }
            return this.value;
        }
    }

    public int Width { get; internal set; }
}

