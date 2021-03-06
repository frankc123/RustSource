﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class dfRichTextTokenizer
{
    private int index;
    private static dfRichTextTokenizer singleton;
    private string source;
    private List<dfMarkupToken> tokens = new List<dfMarkupToken>();

    private char Advance(int amount = 1)
    {
        this.index += amount;
        return this.Peek(0);
    }

    private bool AtTagPosition()
    {
        if (this.Peek(0) != '<')
        {
            return false;
        }
        char c = this.Peek(1);
        if (c == '/')
        {
            return char.IsLetter(this.Peek(2));
        }
        return char.IsLetter(c);
    }

    private dfMarkupToken parseAttributeValue()
    {
        int index = this.index;
        int endIndex = this.index;
        while (this.index < this.source.Length)
        {
            char c = this.Advance(1);
            if ((c == '>') || char.IsWhiteSpace(c))
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
        if (this.Peek(0) == '>')
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
        if (this.Peek(0) != '<')
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
        while ((this.index < this.source.Length) && (this.Peek(0) != '>'))
        {
            if (char.IsWhiteSpace(this.Peek(0)))
            {
                this.parseWhitespace();
                continue;
            }
            dfMarkupToken key = this.parseWord();
            if (key == null)
            {
                this.Advance(1);
                continue;
            }
            if (this.Peek(0) != '=')
            {
                token.AddAttribute(key, key);
                continue;
            }
            char ch = this.Advance(1);
            dfMarkupToken token3 = null;
            switch (ch)
            {
                case '"':
                case '\'':
                    token3 = this.parseQuotedString();
                    break;

                default:
                    token3 = this.parseAttributeValue();
                    break;
            }
            if (token3 == null)
            {
            }
            token.AddAttribute(key, key);
        }
        if (this.Peek(0) == '>')
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

    private List<dfMarkupToken> tokenize(string source)
    {
        dfMarkupToken.Reset();
        dfMarkupTokenAttribute.Reset();
        this.tokens.Clear();
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
            singleton = new dfRichTextTokenizer();
        }
        return singleton.tokenize(source);
    }
}

