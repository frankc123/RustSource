using System;
using System.Collections.Generic;

public static class ODBCachedEnumerator<T> where T: Object
{
    public static IEnumerator<T> Cache<TEnumerator>(ref TEnumerator enumerator) where TEnumerator: struct, ODBEnumerator<T>
    {
        return ODBCachedEnumerator<T, TEnumerator>.Cache(ref enumerator);
    }
}

