using System;
using System.Collections;
using System.Collections.Generic;

public sealed class ODBGenericEnumerable<T, TEnumerator, TEnumerable> : IDisposable, IEnumerable, IEnumerable<T> where T: Object where TEnumerator: struct, ODBEnumerator<T> where TEnumerable: struct, ODBEnumerable<T, TEnumerator>
{
    private bool disposed;
    private TEnumerable enumerable;
    private ODBGenericEnumerable<T, TEnumerator, TEnumerable> next;
    private static ODBGenericEnumerable<T, TEnumerator, TEnumerable> recycle;

    private ODBGenericEnumerable(ref TEnumerable enumerable)
    {
        this.enumerable = enumerable;
    }

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.enumerable = default(TEnumerable);
            this.disposed = true;
            this.next = ODBGenericEnumerable<T, TEnumerator, TEnumerable>.recycle;
            ODBGenericEnumerable<T, TEnumerator, TEnumerable>.recycle = (ODBGenericEnumerable<T, TEnumerator, TEnumerable>) this;
        }
    }

    public TEnumerator GetEnumerator()
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException("enumerable");
        }
        return this.enumerable.GetEnumerator();
    }

    public static ODBGenericEnumerable<T, TEnumerator, TEnumerable> Open(ref TEnumerable enumerable)
    {
        if (ODBGenericEnumerable<T, TEnumerator, TEnumerable>.recycle == null)
        {
            return new ODBGenericEnumerable<T, TEnumerator, TEnumerable>(ref enumerable);
        }
        ODBGenericEnumerable<T, TEnumerator, TEnumerable> recycle = ODBGenericEnumerable<T, TEnumerator, TEnumerable>.recycle;
        recycle.disposed = false;
        ODBGenericEnumerable<T, TEnumerator, TEnumerable>.recycle = recycle.next;
        recycle.enumerable = enumerable;
        return recycle;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ODBCachedEnumerator<T, TEnumerator>.Cache(ref this.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotSupportedException("Cannot use non generic IEnumerable interface with given object");
    }
}

