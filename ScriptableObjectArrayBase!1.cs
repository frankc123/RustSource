using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class ScriptableObjectArrayBase<T> : ScriptableObject, IEnumerable, ICollection<T>, IList<T>, IEnumerable<T>
{
    [SerializeField]
    private T[] _array;

    public Enumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this._array);
    }

    void ICollection<T>.Add(T item)
    {
        this.array.Add(item);
    }

    void ICollection<T>.Clear()
    {
        this.array.Clear();
    }

    bool ICollection<T>.Contains(T item)
    {
        return (Array.IndexOf<T>(this.array, item) != -1);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        this.array.CopyTo(array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item)
    {
        return this.array.Remove(item);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return this.array.GetEnumerator();
    }

    int IList<T>.IndexOf(T item)
    {
        return this.array.IndexOf(item);
    }

    void IList<T>.Insert(int index, T item)
    {
        this.array.Insert(index, item);
    }

    void IList<T>.RemoveAt(int index)
    {
        this.array.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.array.GetEnumerator();
    }

    public T[] array
    {
        get
        {
            if (this._array == null)
            {
            }
            return konst<T>.empty;
        }
    }

    public T this[int i]
    {
        get
        {
            return this.array[i];
        }
    }

    public int Length
    {
        get
        {
            return ((this._array != null) ? this._array.Length : 0);
        }
    }

    int ICollection<T>.Count
    {
        get
        {
            return this.array.Count;
        }
    }

    bool ICollection<T>.IsReadOnly
    {
        get
        {
            return this.array.IsReadOnly;
        }
    }

    T IList<T>.this[int index]
    {
        get
        {
            return this.array[index];
        }
        set
        {
            this.array[index] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        private T[] array;
        private int i;
        public Enumerator(T[] array)
        {
            if (array == null)
            {
            }
            this.array = ScriptableObjectArrayBase<T>.konst.empty;
            this.i = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return this.array[this.i];
            }
        }
        public T Current
        {
            get
            {
                return this.array[this.i];
            }
        }
        public void Reset()
        {
            this.i = -1;
        }

        public bool MoveNext()
        {
            if (this.array == null)
            {
            }
            return (++this.i < ScriptableObjectArrayBase<T>.konst.empty.Length);
        }

        public void Dispose()
        {
        }
    }

    private static class konst
    {
        public static readonly T[] empty;

        static konst()
        {
            ScriptableObjectArrayBase<T>.konst.empty = new T[0];
        }
    }
}

