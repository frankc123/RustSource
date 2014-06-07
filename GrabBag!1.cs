using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public sealed class GrabBag<T> : IEnumerable, IList<T>, ICollection<T>, IEnumerable<T>
{
    private T[] _array;
    private int _length;

    public GrabBag()
    {
        this._array = EmptyArray<T>.array;
        this._length = 0;
    }

    public GrabBag(IEnumerable<T> collection)
    {
        this._array = collection.ToArray<T>();
        this._length = this._array.Length;
    }

    public GrabBag(int capacity)
    {
        this._array = new T[capacity];
        this._length = 0;
    }

    public GrabBag(GrabBag<T> copy)
    {
        if ((copy == null) || (copy._length == 0))
        {
            this._length = 0;
            this._array = EmptyArray<T>.array;
        }
        else
        {
            this._length = copy._length;
            this._array = new T[this._length];
            Array.Copy(copy._array, this._array, this._length);
        }
    }

    public GrabBag(T[] copy)
    {
        if ((copy == null) || ((this._length = copy.Length) == 0))
        {
            this._length = 0;
            this._array = EmptyArray<T>.array;
        }
        else
        {
            this._length = copy.Length;
            this._array = new T[this._length];
            Array.Copy(copy, this._array, this._length);
        }
    }

    public GrabBag(ICollection<T> collection)
    {
        this._array = collection.ToArray<T>();
        this._length = this._array.Length;
    }

    public int Add(T item)
    {
        int index = this.Grow(1);
        this._array[index] = item;
        return index;
    }

    public void Clear()
    {
        while (this._length > 0)
        {
            T local = default(T);
            this._array[--this._length] = local;
        }
    }

    public bool Contains(T item)
    {
        return (Array.IndexOf<T>(this._array, item) != -1);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < this._length; i++)
        {
            array[arrayIndex++] = this._array[i];
        }
    }

    public Enumerator<T> GetEnumerator()
    {
        Enumerator<T> enumerator;
        enumerator.array = (GrabBag<T>) this;
        enumerator.nonNull = true;
        enumerator.index = -1;
        return enumerator;
    }

    public int Grow(int count)
    {
        int num = this._length;
        int num2 = (this._length + count) - this._array.Length;
        if (num2 > 0)
        {
            Array.Resize<T>(ref this._array, (((num2 / 2) * 4) + 1) + this._length);
        }
        this._length += count;
        return num;
    }

    public int IndexOf(T item)
    {
        return ((this._length != 0) ? Array.IndexOf<T>(this._array, item, 0, this._length) : -1);
    }

    public int IndexOf(T item, int start)
    {
        return ((this._length != 0) ? Array.IndexOf<T>(this._array, item, start, this._length - start) : -1);
    }

    public int IndexOf(T item, int start, int count)
    {
        return ((this._length != 0) ? Array.IndexOf<T>(this._array, item, start, count) : -1);
    }

    public void Insert(int index, T item)
    {
        int num = this.Grow(1);
        this._array[num] = this._array[index];
        this._array[index] = item;
    }

    public int LastIndexOf(T item)
    {
        return ((this._length != 0) ? Array.LastIndexOf<T>(this._array, item, 0, this._length) : -1);
    }

    public int LastIndexOf(T item, int start)
    {
        return ((this._length != 0) ? Array.LastIndexOf<T>(this._array, item, start, this._length - start) : -1);
    }

    public int LastIndexOf(T item, int start, int count)
    {
        return ((this._length != 0) ? Array.LastIndexOf<T>(this._array, item, start, count) : -1);
    }

    public bool Remove(T item)
    {
        int index = Array.IndexOf<T>(this._array, item, 0, this._length);
        if (index != -1)
        {
            this._array[index] = this._array[--this._length];
            this._array[this._length] = default(T);
            return true;
        }
        return false;
    }

    public int RemoveAll(T item)
    {
        int num = 0;
        while (this.Remove(item))
        {
            num++;
        }
        return num;
    }

    public void RemoveAt(int index)
    {
        this._array[index] = this._array[--this._length];
        this._array[this._length] = default(T);
    }

    public void Reverse()
    {
        if (this._length > 0)
        {
            Array.Reverse(this._array, 0, this._length);
        }
    }

    public void Reverse(int start, int count)
    {
        if (this._length > 0)
        {
            Array.Reverse(this._array, start, count);
        }
    }

    public void Shrink()
    {
        if (this._length < this._array.Length)
        {
            Array.Resize<T>(ref this._array, this._length);
        }
    }

    public void Sort()
    {
        if (this._length != 0)
        {
            Array.Sort<T>(this._array, 0, this._length);
        }
    }

    public void Sort(IComparer<T> comparer)
    {
        if (this._length != 0)
        {
            Array.Sort<T>(this._array, 0, this._length, comparer);
        }
    }

    public void Sort(int start, int count)
    {
        if (this._length != 0)
        {
            Array.Sort<T>(this._array, start, count);
        }
    }

    public void Sort(IComparer<T> comparer, int start, int count)
    {
        if (this._length != 0)
        {
            Array.Sort<T>(this._array, start, count, comparer);
        }
    }

    public void SortAsKey<V>(V[] values)
    {
        Array.Sort<T, V>(this._array, values, 0, this._length);
    }

    public void SortAsKey<V>(V[] values, IComparer<T> comparer)
    {
        Array.Sort<T, V>(this._array, values, 0, this._length, comparer);
    }

    public void SortAsKey<V>(V[] values, int start, int count)
    {
        Array.Sort<T, V>(this._array, values, start, count);
    }

    public void SortAsKey<V>(V[] values, int start, int count, IComparer<T> comparer)
    {
        Array.Sort<T, V>(this._array, values, start, count, comparer);
    }

    public void SortAsValue<K>(K[] keys)
    {
        Array.Sort<K, T>(keys, this._array, 0, this._length);
    }

    public void SortAsValue<K>(K[] keys, IComparer<K> comparer)
    {
        Array.Sort<K, T>(keys, this._array, 0, this._length, comparer);
    }

    public void SortAsValue<K>(K[] keys, int start, int count)
    {
        Array.Sort<K, T>(keys, this._array, start, count);
    }

    public void SortAsValue<K>(K[] keys, int start, int count, IComparer<K> comparer)
    {
        Array.Sort<K, T>(keys, this._array, start, count, comparer);
    }

    void ICollection<T>.Add(T item)
    {
        int index = this.Grow(1);
        this._array[index] = item;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ((this._length != 0) ? new KlassEnumerator<T>((GrabBag<T>) this) : EmptyArray<T>.emptyEnumerator);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((this._length != 0) ? new KlassEnumerator<T>((GrabBag<T>) this) : EmptyArray<T>.emptyEnumerator);
    }

    public T[] ToArray()
    {
        if (this._length == 0)
        {
            return EmptyArray<T>.array;
        }
        T[] destinationArray = new T[this._length];
        Array.Copy(this._array, destinationArray, this._length);
        return destinationArray;
    }

    public override string ToString()
    {
        return string.Format(StringGetter<T>.Format, this.Count, this.Capacity);
    }

    public ArraySegment<T> ArraySegment
    {
        get
        {
            return new ArraySegment<T>(this._array, 0, this._length);
        }
    }

    public T[] Buffer
    {
        get
        {
            return this._array;
        }
    }

    public int Capacity
    {
        get
        {
            return this._array.Length;
        }
    }

    public int Count
    {
        get
        {
            return this._length;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public T this[int i]
    {
        get
        {
            return this._array[i];
        }
        set
        {
            this._array[i] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        public GrabBag<T> array;
        public int index;
        public bool nonNull;
        object IEnumerator.Current
        {
            get
            {
                return this.array._array[this.index];
            }
        }
        public T Current
        {
            get
            {
                return this.array._array[this.index];
            }
        }
        public bool MoveNext()
        {
            return (this.nonNull && (++this.index < this.array._length));
        }

        public void Reset()
        {
            this.index = -1;
        }

        public void Dispose()
        {
            this = (GrabBag<>.Enumerator) new GrabBag<T>.Enumerator();
        }
    }

    private class KlassEnumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        public GrabBag<T> array;
        public int index;

        public KlassEnumerator(GrabBag<T> array)
        {
            this.array = array;
            this.index = -1;
        }

        public void Dispose()
        {
            this.array = null;
        }

        public bool MoveNext()
        {
            return (++this.index < this.array._length);
        }

        public void Reset()
        {
            this.index = -1;
        }

        public T Current
        {
            get
            {
                return this.array._array[this.index];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.array._array[this.index];
            }
        }
    }

    private static class StringGetter
    {
        public static readonly string Format;

        static StringGetter()
        {
            GrabBag<T>.StringGetter.Format = "[DynArray<" + typeof(T).Name + ">: Count={0}, Capacity={1}]";
        }
    }
}

