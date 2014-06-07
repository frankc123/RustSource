using System;
using System.Collections;
using System.Collections.Generic;

public interface ODBEnumerator<T> : IDisposable, IEnumerator, IEnumerator<T> where T: Object
{
    IEnumerator<T> ToGeneric();

    T ExplicitCurrent { get; }
}

