using System;
using System.Collections.Generic;

public class dfPlainTextTokenizer
{
    private static dfPlainTextTokenizer singleton;
    private List<dfMarkupToken> tokens = new List<dfMarkupToken>();

    private List<dfMarkupToken> tokenize(string source)
    {
        dfMarkupToken.Reset();
        dfMarkupTokenAttribute.Reset();
        this.tokens.Clear();
        int startIndex = 0;
        int num2 = 0;
        int length = source.Length;
        while (startIndex < length)
        {
            if (source[startIndex] == '\r')
            {
                startIndex++;
                num2 = startIndex;
            }
            else
            {
                while ((startIndex < length) && !char.IsWhiteSpace(source[startIndex]))
                {
                    startIndex++;
                }
                if (startIndex > num2)
                {
                    this.tokens.Add(dfMarkupToken.Obtain(source, dfMarkupTokenType.Text, num2, startIndex - 1));
                    num2 = startIndex;
                }
                if ((startIndex < length) && (source[startIndex] == '\n'))
                {
                    this.tokens.Add(dfMarkupToken.Obtain(source, dfMarkupTokenType.Newline, startIndex, startIndex));
                    startIndex++;
                    num2 = startIndex;
                }
                while (((startIndex < length) && (source[startIndex] != '\n')) && ((source[startIndex] != '\r') && char.IsWhiteSpace(source[startIndex])))
                {
                    startIndex++;
                }
                if (startIndex > num2)
                {
                    this.tokens.Add(dfMarkupToken.Obtain(source, dfMarkupTokenType.Whitespace, num2, startIndex - 1));
                    num2 = startIndex;
                }
            }
        }
        return this.tokens;
    }

    public static List<dfMarkupToken> Tokenize(string source)
    {
        if (singleton == null)
        {
            singleton = new dfPlainTextTokenizer();
        }
        return singleton.tokenize(source);
    }
}

