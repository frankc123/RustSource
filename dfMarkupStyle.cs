using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct dfMarkupStyle
{
    private static Dictionary<string, UnityEngine.Color> namedColors;
    public dfRichTextLabel Host;
    public dfAtlas Atlas;
    public dfDynamicFont Font;
    public int FontSize;
    public UnityEngine.FontStyle FontStyle;
    public dfMarkupTextDecoration TextDecoration;
    public dfMarkupTextAlign Align;
    public dfMarkupVerticalAlign VerticalAlign;
    public UnityEngine.Color Color;
    public UnityEngine.Color BackgroundColor;
    public float Opacity;
    public bool PreserveWhitespace;
    public bool Preformatted;
    public int WordSpacing;
    public int CharacterSpacing;
    private int lineHeight;
    public dfMarkupStyle(dfDynamicFont Font, int FontSize, UnityEngine.FontStyle FontStyle)
    {
        this.Host = null;
        this.Atlas = null;
        this.Font = Font;
        this.FontSize = FontSize;
        this.FontStyle = FontStyle;
        this.Align = dfMarkupTextAlign.Left;
        this.VerticalAlign = dfMarkupVerticalAlign.Baseline;
        this.Color = UnityEngine.Color.white;
        this.BackgroundColor = UnityEngine.Color.clear;
        this.TextDecoration = dfMarkupTextDecoration.None;
        this.PreserveWhitespace = false;
        this.Preformatted = false;
        this.WordSpacing = 0;
        this.CharacterSpacing = 0;
        this.lineHeight = 0;
        this.Opacity = 1f;
    }

    static dfMarkupStyle()
    {
        Dictionary<string, UnityEngine.Color> dictionary = new Dictionary<string, UnityEngine.Color>();
        dictionary.Add("aqua", (UnityEngine.Color) UIntToColor(0xffff));
        dictionary.Add("black", UnityEngine.Color.black);
        dictionary.Add("blue", UnityEngine.Color.blue);
        dictionary.Add("cyan", UnityEngine.Color.cyan);
        dictionary.Add("fuchsia", (UnityEngine.Color) UIntToColor(0xff00ff));
        dictionary.Add("gray", UnityEngine.Color.gray);
        dictionary.Add("green", UnityEngine.Color.green);
        dictionary.Add("lime", (UnityEngine.Color) UIntToColor(0xff00));
        dictionary.Add("magenta", UnityEngine.Color.magenta);
        dictionary.Add("maroon", (UnityEngine.Color) UIntToColor(0x800000));
        dictionary.Add("navy", (UnityEngine.Color) UIntToColor(0x80));
        dictionary.Add("olive", (UnityEngine.Color) UIntToColor(0x808000));
        dictionary.Add("orange", (UnityEngine.Color) UIntToColor(0xffa500));
        dictionary.Add("purple", (UnityEngine.Color) UIntToColor(0x800080));
        dictionary.Add("red", UnityEngine.Color.red);
        dictionary.Add("silver", (UnityEngine.Color) UIntToColor(0xc0c0c0));
        dictionary.Add("teal", (UnityEngine.Color) UIntToColor(0x8080));
        dictionary.Add("white", UnityEngine.Color.white);
        dictionary.Add("yellow", UnityEngine.Color.yellow);
        namedColors = dictionary;
    }

    public int LineHeight
    {
        get
        {
            if (this.lineHeight == 0)
            {
                return Mathf.CeilToInt((float) this.FontSize);
            }
            return Mathf.Max(this.FontSize, this.lineHeight);
        }
        set
        {
            this.lineHeight = value;
        }
    }
    public static dfMarkupTextDecoration ParseTextDecoration(string value)
    {
        if (value == "underline")
        {
            return dfMarkupTextDecoration.Underline;
        }
        if (value == "overline")
        {
            return dfMarkupTextDecoration.Overline;
        }
        if (value == "line-through")
        {
            return dfMarkupTextDecoration.LineThrough;
        }
        return dfMarkupTextDecoration.None;
    }

    public static dfMarkupVerticalAlign ParseVerticalAlignment(string value)
    {
        if (value == "top")
        {
            return dfMarkupVerticalAlign.Top;
        }
        if ((value == "center") || (value == "middle"))
        {
            return dfMarkupVerticalAlign.Middle;
        }
        if (value == "bottom")
        {
            return dfMarkupVerticalAlign.Bottom;
        }
        return dfMarkupVerticalAlign.Baseline;
    }

    public static dfMarkupTextAlign ParseTextAlignment(string value)
    {
        if (value == "right")
        {
            return dfMarkupTextAlign.Right;
        }
        if (value == "center")
        {
            return dfMarkupTextAlign.Center;
        }
        if (value == "justify")
        {
            return dfMarkupTextAlign.Justify;
        }
        return dfMarkupTextAlign.Left;
    }

    public static UnityEngine.FontStyle ParseFontStyle(string value, UnityEngine.FontStyle baseStyle)
    {
        if (value == "normal")
        {
            return UnityEngine.FontStyle.Normal;
        }
        if (value == "bold")
        {
            if (baseStyle == UnityEngine.FontStyle.Normal)
            {
                return UnityEngine.FontStyle.Bold;
            }
            if (baseStyle != UnityEngine.FontStyle.Italic)
            {
                return baseStyle;
            }
            return UnityEngine.FontStyle.BoldAndItalic;
        }
        if (value == "italic")
        {
            if (baseStyle == UnityEngine.FontStyle.Normal)
            {
                return UnityEngine.FontStyle.Italic;
            }
            if (baseStyle == UnityEngine.FontStyle.Bold)
            {
                return UnityEngine.FontStyle.BoldAndItalic;
            }
        }
        return baseStyle;
    }

    public static int ParseSize(string value, int baseValue)
    {
        int num2;
        if ((value.Length > 1) && value.EndsWith("%"))
        {
            int num;
            char[] trimChars = new char[] { '%' };
            if (int.TryParse(value.TrimEnd(trimChars), out num))
            {
                return (int) (baseValue * (((float) num) / 100f));
            }
        }
        if (value.EndsWith("px"))
        {
            value = value.Substring(0, value.Length - 2);
        }
        if (int.TryParse(value, out num2))
        {
            return num2;
        }
        return baseValue;
    }

    public static UnityEngine.Color ParseColor(string color, UnityEngine.Color defaultColor)
    {
        UnityEngine.Color color3;
        UnityEngine.Color color2 = defaultColor;
        if (color.StartsWith("#"))
        {
            uint result = 0;
            if (uint.TryParse(color.Substring(1), NumberStyles.HexNumber, null, out result))
            {
                return (UnityEngine.Color) UIntToColor(result);
            }
            return UnityEngine.Color.red;
        }
        if (namedColors.TryGetValue(color.ToLowerInvariant(), out color3))
        {
            color2 = color3;
        }
        return color2;
    }

    private static Color32 UIntToColor(uint color)
    {
        byte r = (byte) (color >> 0x10);
        byte g = (byte) (color >> 8);
        return new Color32(r, g, (byte) color, 0xff);
    }
}

