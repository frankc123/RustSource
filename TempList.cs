using System;
using System.Collections.Generic;

public static class TempList
{
    public static TempList<T> New<T>()
    {
        return TempList<T>.New();
    }

    public static TempList<T> New<T>(IEnumerable<T> enumerable)
    {
        return TempList<T>.New(enumerable);
    }
}

