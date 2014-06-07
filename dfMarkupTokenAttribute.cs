using System;
using System.Collections.Generic;

public class dfMarkupTokenAttribute
{
    public int Index;
    public dfMarkupToken Key;
    private static List<dfMarkupTokenAttribute> pool = new List<dfMarkupTokenAttribute>();
    private static int poolIndex = 0;
    public dfMarkupToken Value;

    private dfMarkupTokenAttribute()
    {
    }

    internal static dfMarkupTokenAttribute GetAttribute(int index)
    {
        return pool[index];
    }

    public static dfMarkupTokenAttribute Obtain(dfMarkupToken key, dfMarkupToken value)
    {
        if (poolIndex >= (pool.Count - 1))
        {
            pool.Add(new dfMarkupTokenAttribute());
        }
        dfMarkupTokenAttribute attribute = pool[poolIndex];
        attribute.Index = poolIndex;
        attribute.Key = key;
        attribute.Value = value;
        poolIndex++;
        return attribute;
    }

    public static void Reset()
    {
        poolIndex = 0;
    }
}

