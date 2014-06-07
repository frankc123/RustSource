using System.Collections;
using System.Collections.Generic;

public interface ODBEnumerable<T, TEnumerator> : IEnumerable, IEnumerable<T> where T: Object where TEnumerator: struct, ODBEnumerator<T>
{
    TEnumerator GetEnumerator();
    IEnumerable<T> ToGeneric();
}

