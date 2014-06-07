using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ODBForwardEnumerable<T> : IEnumerable, ODBEnumerable<T, ODBForwardEnumerator<T>>, IEnumerable<T> where T: Object
{
    private ODBSibling<T> sibling;
    public ODBForwardEnumerable(ODBNode<T> node)
    {
        this.sibling.has = true;
        this.sibling.item = node;
    }

    public ODBForwardEnumerable(ODBList<T> list) : this(list.last)
    {
    }

    public ODBForwardEnumerable(ODBSibling<T> sibling)
    {
        this.sibling = sibling;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ODBCachedEnumerator<T, ODBForwardEnumerator<T>>.Cache(ref this.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public ODBForwardEnumerator<T> GetEnumerator()
    {
        return new ODBForwardEnumerator<T>(this.sibling);
    }

    public unsafe IEnumerable<T> ToGeneric()
    {
        ODBForwardEnumerable<T> enumerable = *((ODBForwardEnumerable<T>*) this);
        return ODBGenericEnumerable<T, ODBForwardEnumerator<T>, ODBForwardEnumerable<T>>.Open(ref enumerable);
    }
}

