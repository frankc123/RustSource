using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class dfList<T> : IDisposable, IEnumerable, ICollection<T>, IList<T>, IEnumerable<T>
{
    private int count;
    private const int DEFAULT_CAPACITY = 0x80;
    private T[] items;
    private static Queue<object> pool;

    static dfList()
    {
        dfList<T>.pool = new Queue<object>();
    }

    internal dfList()
    {
        this.items = new T[0x80];
    }

    internal dfList(int capacity)
    {
        this.items = new T[0x80];
        this.EnsureCapacity(capacity);
    }

    internal dfList(IList<T> listToClone)
    {
        this.items = new T[0x80];
        this.AddRange(listToClone);
    }

    public void Add(T item)
    {
        this.EnsureCapacity(this.count + 1);
        this.items[this.count++] = item;
    }

    public void AddRange(dfList<T> list)
    {
        this.EnsureCapacity(this.count + list.Count);
        Array.Copy(list.items, 0, this.items, this.count, list.Count);
        this.count += list.Count;
    }

    public void AddRange(IList<T> list)
    {
        this.EnsureCapacity(this.count + list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            this.items[this.count++] = list[i];
        }
    }

    public void AddRange(T[] list)
    {
        this.EnsureCapacity(this.count + list.Length);
        Array.Copy(list, 0, this.items, this.count, list.Length);
        this.count += list.Length;
    }

    public bool Any(Func<T, bool> predicate)
    {
        for (int i = 0; i < this.count; i++)
        {
            if (predicate(this.items[i]))
            {
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        Array.Clear(this.items, 0, this.items.Length);
        this.count = 0;
    }

    public dfList<T> Clone()
    {
        dfList<T> list = dfList<T>.Obtain(this.count);
        Array.Copy(this.items, list.items, this.count);
        list.count = this.count;
        return list;
    }

    public dfList<T> Concat(dfList<T> list)
    {
        dfList<T> list2 = dfList<T>.Obtain(this.count + list.count);
        list2.AddRange((dfList<T>) this);
        list2.AddRange(list);
        return list2;
    }

    public bool Contains(T item)
    {
        if (item == null)
        {
            for (int j = 0; j < this.count; j++)
            {
                if (this.items[j] == null)
                {
                    return true;
                }
            }
            return false;
        }
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < this.count; i++)
        {
            if (comparer.Equals(this.items[i], item))
            {
                return true;
            }
        }
        return false;
    }

    public dfList<TResult> Convert<TResult>()
    {
        dfList<TResult> list = dfList<TResult>.Obtain(this.count);
        for (int i = 0; i < this.count; i++)
        {
            list.Add((TResult) System.Convert.ChangeType(this.items[i], typeof(TResult)));
        }
        return list;
    }

    public void CopyTo(T[] array)
    {
        this.CopyTo(array, 0);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(this.items, 0, array, arrayIndex, this.count);
    }

    public void CopyTo(int sourceIndex, T[] dest, int destIndex, int length)
    {
        if ((sourceIndex + length) > this.count)
        {
            throw new IndexOutOfRangeException("sourceIndex");
        }
        if (dest == null)
        {
            throw new ArgumentNullException("dest");
        }
        if ((destIndex + length) > dest.Length)
        {
            throw new IndexOutOfRangeException("destIndex");
        }
        Array.Copy(this.items, sourceIndex, dest, destIndex, length);
    }

    public T Dequeue()
    {
        if (this.count == 0)
        {
            throw new IndexOutOfRangeException();
        }
        T local = this.items[0];
        this.RemoveAt(0);
        return local;
    }

    public void Dispose()
    {
        this.Release();
    }

    public void Enqueue(T item)
    {
        this.Add(item);
    }

    public void EnsureCapacity(int Size)
    {
        if (this.items.Length < Size)
        {
            int newSize = ((Size / 0x80) * 0x80) + 0x80;
            Array.Resize<T>(ref this.items, newSize);
        }
    }

    public T First()
    {
        if (this.count == 0)
        {
            throw new IndexOutOfRangeException();
        }
        return this.items[0];
    }

    public T FirstOrDefault()
    {
        if (this.count > 0)
        {
            return this.items[0];
        }
        return default(T);
    }

    public T FirstOrDefault(Func<T, bool> predicate)
    {
        for (int i = 0; i < this.count; i++)
        {
            if (predicate(this.items[i]))
            {
                return this.items[i];
            }
        }
        return default(T);
    }

    public void ForEach(Action<T> action)
    {
        int num = 0;
        while (num < this.Count)
        {
            action(this.items[num++]);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return PooledEnumerator<T>.Obtain((dfList<T>) this, null);
    }

    public dfList<T> GetRange(int index, int length)
    {
        dfList<T> list = dfList<T>.Obtain(length);
        this.CopyTo(0, list.items, index, length);
        return list;
    }

    public int IndexOf(T item)
    {
        return Array.IndexOf<T>(this.items, item, 0, this.count);
    }

    public void Insert(int index, T item)
    {
        this.EnsureCapacity(this.count + 1);
        if (index < this.count)
        {
            Array.Copy(this.items, index, this.items, index + 1, this.count - index);
        }
        this.items[index] = item;
        this.count++;
    }

    public void InsertRange(int index, T[] array)
    {
        if (array == null)
        {
            throw new ArgumentNullException("items");
        }
        if ((index < 0) || (index > this.count))
        {
            throw new ArgumentOutOfRangeException("index");
        }
        this.EnsureCapacity(this.count + array.Length);
        if (index < this.count)
        {
            Array.Copy(this.items, index, this.items, index + array.Length, this.count - index);
        }
        array.CopyTo(this.items, index);
        this.count += array.Length;
    }

    public void InsertRange(int index, dfList<T> list)
    {
        if (list == null)
        {
            throw new ArgumentNullException("items");
        }
        if ((index < 0) || (index > this.count))
        {
            throw new ArgumentOutOfRangeException("index");
        }
        this.EnsureCapacity(this.count + list.count);
        if (index < this.count)
        {
            Array.Copy(this.items, index, this.items, index + list.count, this.count - index);
        }
        Array.Copy(list.items, 0, this.items, index, list.count);
        this.count += list.count;
    }

    public T Last()
    {
        if (this.count == 0)
        {
            throw new IndexOutOfRangeException();
        }
        return this.items[this.count - 1];
    }

    public T LastOrDefault()
    {
        if (this.count == 0)
        {
            return default(T);
        }
        return this.items[this.count - 1];
    }

    public T LastOrDefault(Func<T, bool> predicate)
    {
        T local = default(T);
        for (int i = 0; i < this.count; i++)
        {
            if (predicate(this.items[i]))
            {
                local = this.items[i];
            }
        }
        return local;
    }

    public int Matching(Func<T, bool> predicate)
    {
        int num = 0;
        for (int i = 0; i < this.count; i++)
        {
            if (predicate(this.items[i]))
            {
                num++;
            }
        }
        return num;
    }

    public static dfList<T> Obtain()
    {
        return ((dfList<T>.pool.Count <= 0) ? new dfList<T>() : ((dfList<T>) dfList<T>.pool.Dequeue()));
    }

    internal static dfList<T> Obtain(int capacity)
    {
        dfList<T> list = dfList<T>.Obtain();
        list.EnsureCapacity(capacity);
        return list;
    }

    public void Release()
    {
        this.Clear();
        dfList<T>.pool.Enqueue(this);
    }

    public bool Remove(T item)
    {
        int index = this.IndexOf(item);
        if (index == -1)
        {
            return false;
        }
        this.RemoveAt(index);
        return true;
    }

    public void RemoveAll(Predicate<T> predicate)
    {
        int index = 0;
        while (index < this.count)
        {
            if (predicate(this.items[index]))
            {
                this.RemoveAt(index);
            }
            else
            {
                index++;
            }
        }
    }

    public void RemoveAt(int index)
    {
        if (index >= this.count)
        {
            throw new ArgumentOutOfRangeException();
        }
        this.count--;
        if (index < this.count)
        {
            Array.Copy(this.items, index + 1, this.items, index, this.count - index);
        }
        this.items[this.count] = default(T);
    }

    public void RemoveRange(int index, int length)
    {
        if (((index < 0) || (length < 0)) || ((this.count - index) < length))
        {
            throw new ArgumentOutOfRangeException();
        }
        if (this.count > 0)
        {
            this.count -= length;
            if (index < this.count)
            {
                Array.Copy(this.items, index + length, this.items, index, this.count - index);
            }
            Array.Clear(this.items, this.count, length);
        }
    }

    public void Reverse()
    {
        Array.Reverse(this.items, 0, this.count);
    }

    public dfList<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        dfList<TResult> list = dfList<TResult>.Obtain(this.count);
        for (int i = 0; i < this.count; i++)
        {
            list.Add(selector(this.items[i]));
        }
        return list;
    }

    public void Sort()
    {
        Array.Sort<T>(this.items, 0, this.count, null);
    }

    public void Sort(IComparer<T> comparer)
    {
        Array.Sort<T>(this.items, 0, this.count, comparer);
    }

    public void Sort(Comparison<T> comparison)
    {
        if (comparison == null)
        {
            throw new ArgumentNullException("comparison");
        }
        if (this.count > 0)
        {
            using (FunctorComparer<T> comparer = FunctorComparer<T>.Obtain(comparison))
            {
                Array.Sort<T>(this.items, 0, this.count, comparer);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return PooledEnumerator<T>.Obtain((dfList<T>) this, null);
    }

    public T[] ToArray()
    {
        T[] destinationArray = new T[this.count];
        Array.Copy(this.items, destinationArray, this.count);
        return destinationArray;
    }

    public T[] ToArray(int index, int length)
    {
        T[] dest = new T[this.count];
        if (this.count > 0)
        {
            this.CopyTo(index, dest, 0, length);
        }
        return dest;
    }

    public void TrimExcess()
    {
        Array.Resize<T>(ref this.items, this.count);
    }

    public dfList<T> Where(Func<T, bool> predicate)
    {
        dfList<T> list = dfList<T>.Obtain(this.count);
        for (int i = 0; i < this.count; i++)
        {
            if (predicate(this.items[i]))
            {
                list.Add(this.items[i]);
            }
        }
        return list;
    }

    internal int Capacity
    {
        get
        {
            return this.items.Length;
        }
    }

    public int Count
    {
        get
        {
            return this.count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public T this[int index]
    {
        get
        {
            if ((index < 0) || (index > (this.count - 1)))
            {
                throw new IndexOutOfRangeException();
            }
            return this.items[index];
        }
        set
        {
            if ((index < 0) || (index > (this.count - 1)))
            {
                throw new IndexOutOfRangeException();
            }
            this.items[index] = value;
        }
    }

    internal T[] Items
    {
        get
        {
            return this.items;
        }
    }

    private class FunctorComparer : IDisposable, IComparer<T>
    {
        private Comparison<T> comparison;
        private static Queue<dfList<T>.FunctorComparer> pool;

        static FunctorComparer()
        {
            dfList<T>.FunctorComparer.pool = new Queue<dfList<T>.FunctorComparer>();
        }

        public int Compare(T x, T y)
        {
            return this.comparison(x, y);
        }

        public void Dispose()
        {
            this.Release();
        }

        public static dfList<T>.FunctorComparer Obtain(Comparison<T> comparison)
        {
            dfList<T>.FunctorComparer comparer = (dfList<T>.FunctorComparer.pool.Count <= 0) ? new dfList<T>.FunctorComparer() : dfList<T>.FunctorComparer.pool.Dequeue();
            comparer.comparison = comparison;
            return comparer;
        }

        public void Release()
        {
            this.comparison = null;
            if (!dfList<T>.FunctorComparer.pool.Contains((dfList<T>.FunctorComparer) this))
            {
                dfList<T>.FunctorComparer.pool.Enqueue((dfList<T>.FunctorComparer) this);
            }
        }
    }

    private class PooledEnumerator : IDisposable, IEnumerator, IEnumerable, IEnumerable<T>, IEnumerator<T>
    {
        private int currentIndex;
        private T currentValue;
        private bool isValid;
        private dfList<T> list;
        private static Queue<dfList<T>.PooledEnumerator> pool;
        private Func<T, bool> predicate;

        static PooledEnumerator()
        {
            dfList<T>.PooledEnumerator.pool = new Queue<dfList<T>.PooledEnumerator>();
        }

        public void Dispose()
        {
            this.Release();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (!this.isValid)
            {
                throw new InvalidOperationException("The enumerator is no longer valid");
            }
            while (this.currentIndex < this.list.Count)
            {
                T local = this.list[this.currentIndex++];
                if ((this.predicate == null) || this.predicate(local))
                {
                    this.currentValue = local;
                    return true;
                }
            }
            this.Release();
            this.currentValue = default(T);
            return false;
        }

        public static dfList<T>.PooledEnumerator Obtain(dfList<T> list, Func<T, bool> predicate = null)
        {
            dfList<T>.PooledEnumerator enumerator = (dfList<T>.PooledEnumerator.pool.Count <= 0) ? new dfList<T>.PooledEnumerator() : dfList<T>.PooledEnumerator.pool.Dequeue();
            enumerator.ResetInternal(list, predicate);
            return enumerator;
        }

        public void Release()
        {
            if (this.isValid)
            {
                this.isValid = false;
                dfList<T>.PooledEnumerator.pool.Enqueue((dfList<T>.PooledEnumerator) this);
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private void ResetInternal(dfList<T> list, Func<T, bool> predicate = null)
        {
            this.isValid = true;
            this.list = list;
            this.predicate = predicate;
            this.currentIndex = 0;
            this.currentValue = default(T);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T Current
        {
            get
            {
                if (!this.isValid)
                {
                    throw new InvalidOperationException("The enumerator is no longer valid");
                }
                return this.currentValue;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
    }
}

