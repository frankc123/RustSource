using System;
using System.Collections.Generic;

public static class ODBGenericEnumerable<T> where T: Object
{
    public static IEnumerable<T> Open<TEnumerator, TEnumerable>(ref TEnumerable enumerable) where TEnumerator: struct, ODBEnumerator<T> where TEnumerable: struct, ODBEnumerable<T, TEnumerator>
    {
        return ODBGenericEnumerable<T, TEnumerator, TEnumerable>.Open(ref enumerable);
    }
}

