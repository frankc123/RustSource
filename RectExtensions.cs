using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class RectExtensions
{
    public static RectOffset ConstrainPadding(this RectOffset borders)
    {
        if (borders == null)
        {
            return new RectOffset();
        }
        borders.left = Mathf.Max(0, borders.left);
        borders.right = Mathf.Max(0, borders.right);
        borders.top = Mathf.Max(0, borders.top);
        borders.bottom = Mathf.Max(0, borders.bottom);
        return borders;
    }

    public static bool Contains(this Rect rect, Rect other)
    {
        bool flag = rect.x <= other.x;
        bool flag2 = (rect.x + rect.width) >= (other.x + other.width);
        bool flag3 = rect.yMin <= other.yMin;
        bool flag4 = (rect.y + rect.height) >= (other.y + other.height);
        return (((flag && flag2) && flag3) && flag4);
    }

    public static string Debug(this Rect rect)
    {
        object[] args = new object[] { rect.xMin, rect.yMin, rect.xMax, rect.yMax };
        return string.Format("[{0},{1},{2},{3}]", args);
    }

    public static Rect Intersection(this Rect a, Rect b)
    {
        if (!a.Intersects(b))
        {
            return new Rect();
        }
        float left = Mathf.Max(a.xMin, b.xMin);
        float right = Mathf.Min(a.xMax, b.xMax);
        float bottom = Mathf.Max(a.yMin, b.yMin);
        float top = Mathf.Min(a.yMax, b.yMax);
        return Rect.MinMaxRect(left, top, right, bottom);
    }

    public static bool Intersects(this Rect rect, Rect other)
    {
        bool flag = (((rect.xMax < other.xMin) || (rect.yMax < other.xMin)) || (rect.xMin > other.xMax)) || (rect.yMin > other.yMax);
        return !flag;
    }

    public static bool IsEmpty(this Rect rect)
    {
        return ((rect.xMin == rect.xMax) || (rect.yMin == rect.yMax));
    }

    public static Rect RoundToInt(this Rect rect)
    {
        return new Rect((float) Mathf.RoundToInt(rect.x), (float) Mathf.RoundToInt(rect.y), (float) Mathf.RoundToInt(rect.width), (float) Mathf.RoundToInt(rect.height));
    }

    public static Rect Union(this Rect a, Rect b)
    {
        float left = Mathf.Min(a.xMin, b.xMin);
        float right = Mathf.Max(a.xMax, b.xMax);
        float top = Mathf.Min(a.yMin, b.yMin);
        float bottom = Mathf.Max(a.yMax, b.yMax);
        return Rect.MinMaxRect(left, top, right, bottom);
    }
}

