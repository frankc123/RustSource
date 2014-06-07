using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ODBForwardEnumerator<T> : IDisposable, IEnumerator, ODBEnumerator<T>, IEnumerator<T> where T: Object
{
    private ODBSibling<T> sib;
    public T Current;
    public ODBForwardEnumerator(ODBNode<T> node)
    {
        this.sib.has = true;
        this.sib.item = node;
        this.Current = null;
    }

    public ODBForwardEnumerator(ODBList<T> list) : this(list.first)
    {
    }

    public ODBForwardEnumerator(ODBSibling<T> sibling)
    {
        this.sib = sibling;
        this.Current = null;
    }

    T ODBEnumerator<T>.ExplicitCurrent
    {
        get
        {
            return this.Current;
        }
    }
    T IEnumerator<T>.Current
    {
        get
        {
            return this.Current;
        }
    }
    object IEnumerator.Current
    {
        get
        {
            return this.Current;
        }
    }
    void IEnumerator.Reset()
    {
        throw new NotSupportedException();
    }

    public bool MoveNext()
    {
        if (this.sib.has)
        {
            ODBNode<T> item = this.sib.item;
            this.Current = item.self;
            this.sib = item.n;
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        this.sib = new ODBSibling<T>();
        this.Current = null;
    }

    public unsafe IEnumerator<T> ToGeneric()
    {
        ODBForwardEnumerator<T> enumerator = *((ODBForwardEnumerator<T>*) this);
        return ODBCachedEnumerator<T, ODBForwardEnumerator<T>>.Cache(ref enumerator);
    }
}

