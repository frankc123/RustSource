using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

[StructLayout(LayoutKind.Sequential)]
public struct dfMarkupBorders
{
    public int left;
    public int top;
    public int right;
    public int bottom;
    public dfMarkupBorders(int left, int right, int top, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    public int horizontal
    {
        get
        {
            return (this.left + this.right);
        }
    }
    public int vertical
    {
        get
        {
            return (this.top + this.bottom);
        }
    }
    public static dfMarkupBorders Parse(string value)
    {
        dfMarkupBorders borders = new dfMarkupBorders();
        value = Regex.Replace(value, @"\s+", " ");
        char[] separator = new char[] { ' ' };
        string[] strArray = value.Split(separator);
        if (strArray.Length == 1)
        {
            int num = dfMarkupStyle.ParseSize(value, 0);
            borders.left = borders.right = num;
            borders.top = borders.bottom = num;
            return borders;
        }
        if (strArray.Length == 2)
        {
            int num2 = dfMarkupStyle.ParseSize(strArray[0], 0);
            borders.top = borders.bottom = num2;
            int num3 = dfMarkupStyle.ParseSize(strArray[1], 0);
            borders.left = borders.right = num3;
            return borders;
        }
        if (strArray.Length == 3)
        {
            int num4 = dfMarkupStyle.ParseSize(strArray[0], 0);
            borders.top = num4;
            int num5 = dfMarkupStyle.ParseSize(strArray[1], 0);
            borders.left = borders.right = num5;
            int num6 = dfMarkupStyle.ParseSize(strArray[2], 0);
            borders.bottom = num6;
            return borders;
        }
        if (strArray.Length == 4)
        {
            int num7 = dfMarkupStyle.ParseSize(strArray[0], 0);
            borders.top = num7;
            int num8 = dfMarkupStyle.ParseSize(strArray[1], 0);
            borders.right = num8;
            int num9 = dfMarkupStyle.ParseSize(strArray[2], 0);
            borders.bottom = num9;
            int num10 = dfMarkupStyle.ParseSize(strArray[3], 0);
            borders.left = num10;
        }
        return borders;
    }

    public override string ToString()
    {
        object[] args = new object[] { this.top, this.right, this.left, this.bottom };
        return string.Format("[T:{0},R:{1},L:{2},B:{3}]", args);
    }
}

