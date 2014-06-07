using System;
using System.Collections.Generic;

public static class ODBGenericEnumerable<T, TEnumerator> where T: Object where TEnumerator: struct, ODBEnumerator<T>
{
    public static IEnumerable<T> Open<TEnumerable>(ref TEnumerable enumerable) where TEnumerable: struct, ODBEnumerable<T, TEnumerator>
    {
        return ODBGenericEnumerable<T, TEnumerator, TEnumerable>.Open(ref enumerable);
    }
}

