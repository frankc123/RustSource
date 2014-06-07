using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ODBReverseEnumerable<T> : IEnumerable, ODBEnumerable<T, ODBReverseEnumerator<T>>, IEnumerable<T> where T: Object
{
    private ODBSibling<T> sibling;
    public ODBReverseEnumerable(ODBNode<T> node)
    {
        this.sibling.has = true;
        this.sibling.item = node;
    }

    public ODBReverseEnumerable(ODBList<T> list) : this(list.last)
    {
    }

    public ODBReverseEnumerable(ODBSibling<T> sibling)
    {
        this.sibling = sibling;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ODBCachedEnumerator<T, ODBReverseEnumerator<T>>.Cache(ref this.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public ODBReverseEnumerator<T> GetEnumerator()
    {
        return new ODBReverseEnumerator<T>(this.sibling);
    }

    public unsafe IEnumerable<T> ToGeneric()
    {
        ODBReverseEnumerable<T> enumerable = *((ODBReverseEnumerable<T>*) this);
        return ODBGenericEnumerable<T, ODBReverseEnumerator<T>, ODBReverseEnumerable<T>>.Open(ref enumerable);
    }
}

