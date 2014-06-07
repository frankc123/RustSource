using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct RecycleListIter<T>
{
    private List<T>.Enumerator enumerator;
    internal RecycleListIter(List<T>.Enumerator enumerator)
    {
        this.enumerator = enumerator;
    }

    public bool MoveNext()
    {
        return this.enumerator.MoveNext();
    }

    public T Current
    {
        get
        {
            return this.enumerator.Current;
        }
    }
    public void Dispose()
    {
        this.enumerator.Dispose();
    }
}

