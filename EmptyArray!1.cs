using System;
using System.Collections;
using System.Collections.Generic;

public static class EmptyArray<T>
{
    public static readonly T[] array;
    public static readonly bool isByRef;

    static EmptyArray()
    {
        EmptyArray<T>.array = new T[0];
        EmptyArray<T>.isByRef = typeof(T).IsByRef;
    }

    public static object defaultBoxedValue
    {
        get
        {
            return (!EmptyArray<T>.isByRef ? DefaultBoxedValue<T>.value : null);
        }
    }

    public static IEnumerable<T> emptyEnumerable
    {
        get
        {
            return EmptyEnumerable<T>.singleton;
        }
    }

    public static IEnumerator<T> emptyEnumerator
    {
        get
        {
            return EmptyEnumerator<T>.singleton;
        }
    }

    private static class DefaultBoxedValue
    {
        public static object value;

        static DefaultBoxedValue()
        {
            EmptyArray<T>.DefaultBoxedValue.value = default(T);
        }
    }

    private class EmptyEnumerable : IEnumerable, IEnumerable<T>
    {
        public static IEnumerable<T> singleton;

        static EmptyEnumerable()
        {
            EmptyArray<T>.EmptyEnumerable.singleton = new EmptyArray<T>.EmptyEnumerable();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return EmptyArray<T>.EmptyEnumerator.singleton;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EmptyArray<T>.EmptyEnumerator.singleton;
        }
    }

    private class EmptyEnumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        public static IEnumerator<T> singleton;

        static EmptyEnumerator()
        {
            EmptyArray<T>.EmptyEnumerator.singleton = new EmptyArray<T>.EmptyEnumerator();
        }

        private EmptyEnumerator()
        {
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public T Current
        {
            get
            {
                return default(T);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return EmptyArray<T>.defaultBoxedValue;
            }
        }
    }
}

