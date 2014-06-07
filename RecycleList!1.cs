using System;
using System.Collections.Generic;

public class RecycleList<T> : List<T>, IDisposable
{
    private static LinkedList<RecycleList<T>> bin;
    private static int binCount;
    private bool bound;

    static RecycleList()
    {
        RecycleList<T>.binCount = 0;
        RecycleList<T>.bin = new LinkedList<RecycleList<T>>();
    }

    internal RecycleList()
    {
    }

    public static void Bin(ref RecycleList<T> list)
    {
        if (list != null)
        {
            if (list.bound)
            {
                RecycleList<T>.bin.AddLast(list);
                list.bound = false;
            }
            list = null;
        }
    }

    public RecycleList<T> Clone()
    {
        return RecycleList<T>.Make<RecycleList<T>>((RecycleList<T>) this);
    }

    public void Dispose()
    {
        RecycleList<T>.Bin(ref list);
    }

    public static RecycleList<T> Make()
    {
        RecycleList<T> list;
        if (RecycleList<T>.binCount > 0)
        {
            list = RecycleList<T>.bin.First.Value;
            RecycleList<T>.bin.RemoveFirst();
            RecycleList<T>.binCount--;
        }
        else
        {
            list = new RecycleList<T>();
        }
        list.bound = true;
        return list;
    }

    public static RecycleList<T> Make<TClassEnumerable>(TClassEnumerable enumerable) where TClassEnumerable: class, IEnumerable<T>
    {
        RecycleList<T> list = RecycleList<T>.Make();
        list.AddRange(enumerable);
        return list;
    }

    public static RecycleList<T> MakeFromValuedEnumerator<TEnumerator>(ref TEnumerator enumerator) where TEnumerator: struct, IEnumerator<T>
    {
        RecycleList<T> list = RecycleList<T>.Make();
        while (enumerator.MoveNext())
        {
            list.Add((T) enumerator.Current);
        }
        enumerator.Dispose();
        return list;
    }

    public RecycleListIter<T> MakeIter()
    {
        return new RecycleListIter<T>(base.GetEnumerator());
    }

    public static RecycleList<T> MakeValueEnumerable<TStructEnumerable>(ref TStructEnumerable enumerable) where TStructEnumerable: struct, IEnumerable<T>
    {
        RecycleList<T> list = RecycleList<T>.Make();
        list.AddRange((TStructEnumerable) enumerable);
        return list;
    }
}

