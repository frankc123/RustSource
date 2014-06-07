using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct HSetIter<T> : IDisposable, IEnumerator, IEnumerator<T>
{
    private HashSet<T>.Enumerator enumerator;
    public HSetIter(HashSet<T>.Enumerator enumerator)
    {
        this.enumerator = enumerator;
    }

    object IEnumerator.Current
    {
        get
        {
            return this.enumerator.Current;
        }
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
    public void Reset()
    {
        throw new NotSupportedException();
    }

    public void Dispose()
    {
        this.enumerator.Dispose();
    }
}

