using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class dfMarkupTokenizer
{
    private int index;
    private static dfMarkupTokenizer singleton;
    private string source;
    private List<dfMarkupToken> tokens = new List<dfMarkupToken>();
    private static List<string> validTags = new List<string> { "color", "sprite" };

    private char Advance(int amount = 1)
    {
        this.index += amount;
        return this.Peek(0);
    }

    private bool AtTagPosition()
    {
        if (this.Peek(0) != '[')
        {
            return false;
        }
        char c = this.Peek(1);
        if (c == '/')
        {
            return (char.IsLetter(this.Peek(2)) && this.isValidTag(this.index + 2, true));
        }
        return (char.IsLetter(c) && this.isValidTag(this.index + 1, false));
    }

    private bool isValidTag(int index, bool endTag)
    {
        for (int i = 0; i < validTags.Count; i++)
        {
            string str = validTags[i];
            bool flag = true;
            for (int j = 0; (j < (str.Length - 1)) && ((j + index) < (this.source.Length - 1)); j++)
            {
                if ((!endTag && (this.source[j + index] == ' ')) || (this.source[j + index] == ']'))
                {
                    break;
                }
                if (char.ToLowerInvariant(str[j]) != char.ToLowerInvariant(this.source[j + index]))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                return true;
            }
        }
        return false;
    }

    private dfMarkupToken parseAttributeValue()
    {
        int index = this.index;
        int endIndex = this.index;
        while (this.index < this.source.Length)
        {
            char c = this.Advance(1);
            if ((c == ']') || char.IsWhiteSpace(c))
            {
                break;
            }
            endIndex++;
        }
        return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index, endIndex);
    }

    private dfMarkupToken parseEndTag()
    {
        this.Advance(2);
        int index = this.index;
        int endIndex = this.index;
        while ((this.index < this.source.Length) && char.IsLetterOrDigit(this.Advance(1)))
        {
            endIndex++;
        }
        if (this.Peek(0) == ']')
        {
            this.Advance(1);
        }
        return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.EndTag, index, endIndex);
    }

    private dfMarkupToken parseNonWhitespace()
    {
        int index = this.index;
        int endIndex = this.index;
        while (this.index < this.source.Length)
        {
            if (char.IsWhiteSpace(this.Advance(1)) || this.AtTagPosition())
            {
                break;
            }
            endIndex++;
        }
        return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index, endIndex);
    }

    private dfMarkupToken parseQuotedString()
    {
        char ch = this.Peek(0);
        if ((ch != '"') && (ch != '\''))
        {
            return null;
        }
        this.Advance(1);
        int index = this.index;
        int endIndex = this.index;
        while ((this.index < this.source.Length) && (this.Advance(1) != ch))
        {
            endIndex++;
        }
        if (this.Peek(0) == ch)
        {
            this.Advance(1);
        }
        return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index, endIndex);
    }

    private dfMarkupToken parseTag()
    {
        if (this.Peek(0) != '[')
        {
            return null;
        }
        if (this.Peek(1) == '/')
        {
            return this.parseEndTag();
        }
        this.Advance(1);
        if (!char.IsLetterOrDigit(this.Peek(0)))
        {
            return null;
        }
        int index = this.index;
        int endIndex = this.index;
        while ((this.index < this.source.Length) && char.IsLetterOrDigit(this.Advance(1)))
        {
            endIndex++;
        }
        dfMarkupToken token = dfMarkupToken.Obtain(this.source, dfMarkupTokenType.StartTag, index, endIndex);
        if ((this.index < this.source.Length) && (this.Peek(0) != ']'))
        {
            if (char.IsWhiteSpace(this.Peek(0)))
            {
                this.parseWhitespace();
            }
            int startIndex = this.index;
            int num4 = this.index;
            if (this.Peek(0) == '"')
            {
                dfMarkupToken key = this.parseQuotedString();
                token.AddAttribute(key, key);
            }
            else
            {
                while ((this.index < this.source.Length) && (this.Advance(1) != ']'))
                {
                    num4++;
                }
                dfMarkupToken token3 = dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, startIndex, num4);
                token.AddAttribute(token3, token3);
            }
        }
        if (this.Peek(0) == ']')
        {
            this.Advance(1);
        }
        return token;
    }

    private dfMarkupToken parseWhitespace()
    {
        int index = this.index;
        int endIndex = this.index;
        if (this.Peek(0) == '\n')
        {
            this.Advance(1);
            return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Newline, index, index);
        }
        while (this.index < this.source.Length)
        {
            char c = this.Advance(1);
            if (((c == '\n') || (c == '\r')) || !char.IsWhiteSpace(c))
            {
                break;
            }
            endIndex++;
        }
        return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Whitespace, index, endIndex);
    }

    private dfMarkupToken parseWord()
    {
        if (!char.IsLetter(this.Peek(0)))
        {
            return null;
        }
        int index = this.index;
        int endIndex = this.index;
        while ((this.index < this.source.Length) && char.IsLetter(this.Advance(1)))
        {
            endIndex++;
        }
        return dfMarkupToken.Obtain(this.source, dfMarkupTokenType.Text, index, endIndex);
    }

    private char Peek(int offset = 0)
    {
        if ((this.index + offset) > (this.source.Length - 1))
        {
            return '\0';
        }
        return this.source[this.index + offset];
    }

    private void reset()
    {
        dfMarkupToken.Reset();
        dfMarkupTokenAttribute.Reset();
        this.tokens.Clear();
    }

    private List<dfMarkupToken> tokenize(string source)
    {
        this.reset();
        this.source = source;
        this.index = 0;
        while (this.index < source.Length)
        {
            char c = this.Peek(0);
            if (this.AtTagPosition())
            {
                dfMarkupToken item = this.parseTag();
                if (item != null)
                {
                    this.tokens.Add(item);
                }
            }
            else
            {
                dfMarkupToken token2 = null;
                if (char.IsWhiteSpace(c))
                {
                    if (c != '\r')
                    {
                        token2 = this.parseWhitespace();
                    }
                }
                else
                {
                    token2 = this.parseNonWhitespace();
                }
                if (token2 == null)
                {
                    this.Advance(1);
                }
                else
                {
                    this.tokens.Add(token2);
                }
            }
        }
        return this.tokens;
    }

    public static List<dfMarkupToken> Tokenize(string source)
    {
        if (singleton == null)
        {
            singleton = new dfMarkupTokenizer();
        }
        return singleton.tokenize(source);
    }
}

