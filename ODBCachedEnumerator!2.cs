using System;
using System.Collections;
using System.Collections.Generic;

public class ODBCachedEnumerator<T, TEnumerator> : IDisposable, IEnumerator, ODBEnumerator<T>, IEnumerator<T> where T: Object where TEnumerator: struct, ODBEnumerator<T>
{
    private bool disposed;
    private TEnumerator enumerator;
    private ODBCachedEnumerator<T, TEnumerator> next;
    private static ODBCachedEnumerator<T, TEnumerator> recycle;

    private ODBCachedEnumerator(ref TEnumerator enumerator)
    {
        this.enumerator = enumerator;
    }

    public static IEnumerator<T> Cache(ref TEnumerator enumerator)
    {
        if (ODBCachedEnumerator<T, TEnumerator>.recycle == null)
        {
            return new ODBCachedEnumerator<T, TEnumerator>(ref enumerator);
        }
        ODBCachedEnumerator<T, TEnumerator> recycle = ODBCachedEnumerator<T, TEnumerator>.recycle;
        ODBCachedEnumerator<T, TEnumerator>.recycle = recycle.next;
        recycle.disposed = false;
        recycle.enumerator = enumerator;
        recycle.next = null;
        return recycle;
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;
            this.next = ODBCachedEnumerator<T, TEnumerator>.recycle;
            ODBCachedEnumerator<T, TEnumerator>.recycle = (ODBCachedEnumerator<T, TEnumerator>) this;
            this.enumerator.Dispose();
            this.enumerator = default(TEnumerator);
        }
    }

    public bool MoveNext()
    {
        return this.enumerator.MoveNext();
    }

    IEnumerator<T> ODBEnumerator<T>.ToGeneric()
    {
        return this;
    }

    public void Reset()
    {
        this.enumerator.Reset();
    }

    public T Current
    {
        get
        {
            return this.enumerator.ExplicitCurrent;
        }
    }

    T ODBEnumerator<T>.ExplicitCurrent
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
            throw new NotSupportedException("You must use the IEnumerator<> interface. as dispose is entirely neccisary");
        }
    }
}

