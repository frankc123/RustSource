using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

[DebuggerDisplay("Count = {Count}")]
public sealed class LockedList<T> : IEnumerable, IList, ICollection, ICollection<T>, IList<T>, IEnumerable<T>, IEquatable<List<T>>
{
    private readonly List<T> list;

    private LockedList()
    {
        this.list = new List<T>(0);
    }

    public LockedList(List<T> list)
    {
        if (object.ReferenceEquals(list, null))
        {
            throw new ArgumentNullException("list");
        }
        this.list = list;
    }

    public int BinarySearch(T item)
    {
        return this.list.BinarySearch(item);
    }

    public int BinarySearch(T item, IComparer<T> comparer)
    {
        return this.list.BinarySearch(item, comparer);
    }

    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
        return this.list.BinarySearch(index, count, item, comparer);
    }

    public bool Contains(T item)
    {
        return this.list.Contains(item);
    }

    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
        return this.list.ConvertAll<TOutput>(converter);
    }

    public void CopyTo(T[] array)
    {
        this.list.CopyTo(array);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        this.list.CopyTo(array, arrayIndex);
    }

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        this.list.CopyTo(index, array, arrayIndex, count);
    }

    public bool Equals(List<T> list)
    {
        return this.list.Equals(list);
    }

    public override bool Equals(object obj)
    {
        return (!(obj is LockedList<T>) ? ((obj is List<T>) && this.list.Equals(obj)) : this.list.Equals(((LockedList<T>) obj).list));
    }

    public T Find(Predicate<T> match)
    {
        return this.list.Find(match);
    }

    public List<T> FindAll(Predicate<T> match)
    {
        return this.list.FindAll(match);
    }

    public int FindIndex(Predicate<T> match)
    {
        return this.list.FindIndex(match);
    }

    public T FindLast(Predicate<T> match)
    {
        return this.list.FindLast(match);
    }

    public int FindLastIndex(Predicate<T> match)
    {
        return this.list.FindLastIndex(match);
    }

    public void ForEach(Action<T> action)
    {
        this.list.ForEach(action);
    }

    public List<T>.Enumerator GetEnumerator()
    {
        return this.list.GetEnumerator();
    }

    public override int GetHashCode()
    {
        return this.list.GetHashCode();
    }

    public List<T> GetRange(int index, int count)
    {
        return this.list.GetRange(index, count);
    }

    public int IndexOf(T item)
    {
        return this.list.IndexOf(item);
    }

    public int IndexOf(T item, int index)
    {
        return this.list.IndexOf(item, index);
    }

    public int IndexOf(T item, int index, int count)
    {
        return this.list.IndexOf(item, index, count);
    }

    public int LastIndexOf(T item)
    {
        return this.list.LastIndexOf(item);
    }

    public int LastIndexOf(T item, int index)
    {
        return this.list.LastIndexOf(item, index);
    }

    public int LastIndexOf(T item, int index, int count)
    {
        return this.list.LastIndexOf(item, index, count);
    }

    void ICollection<T>.Add(T item)
    {
        throw new NotSupportedException();
    }

    void ICollection<T>.Clear()
    {
        throw new NotSupportedException();
    }

    bool ICollection<T>.Contains(T item)
    {
        return this.ilist.Contains(item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        this.ilist.CopyTo(array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new NotSupportedException();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return this.ilist.GetEnumerator();
    }

    int IList<T>.IndexOf(T item)
    {
        return this.list.IndexOf(item);
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new NotSupportedException();
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
        this.olist.CopyTo(array, index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.olist.GetEnumerator();
    }

    int IList.Add(object value)
    {
        throw new NotSupportedException();
    }

    void IList.Clear()
    {
        throw new NotSupportedException();
    }

    bool IList.Contains(object value)
    {
        return this.olist.Contains(value);
    }

    int IList.IndexOf(object value)
    {
        return this.olist.IndexOf(value);
    }

    void IList.Insert(int index, object value)
    {
        throw new NotSupportedException();
    }

    void IList.Remove(object value)
    {
        throw new NotSupportedException();
    }

    void IList.RemoveAt(int index)
    {
        throw new NotSupportedException();
    }

    public T[] ToArray()
    {
        return this.list.ToArray();
    }

    public List<T> ToList()
    {
        return this.list.GetRange(0, this.list.Count);
    }

    public override string ToString()
    {
        return this.list.ToString();
    }

    public bool TrueForAll(Predicate<T> match)
    {
        return this.list.TrueForAll(match);
    }

    public int Capacity
    {
        get
        {
            return this.list.Capacity;
        }
    }

    public int Count
    {
        get
        {
            return this.list.Count;
        }
    }

    public static LockedList<T> Empty
    {
        get
        {
            return EmptyInstance<T>.List;
        }
    }

    private IList<T> ilist
    {
        get
        {
            return this.list;
        }
    }

    public T this[int index]
    {
        get
        {
            return this.list[index];
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    private IList olist
    {
        get
        {
            return this.list;
        }
    }

    int ICollection<T>.Count
    {
        get
        {
            return this.ilist.Count;
        }
    }

    bool ICollection<T>.IsReadOnly
    {
        get
        {
            return true;
        }
    }

    T IList<T>.this[int index]
    {
        get
        {
            return this.ilist[index];
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    int ICollection.Count
    {
        get
        {
            return this.olist.Count;
        }
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            return this.olist.IsSynchronized;
        }
    }

    object ICollection.SyncRoot
    {
        get
        {
            return this.olist.SyncRoot;
        }
    }

    bool IList.IsFixedSize
    {
        get
        {
            return false;
        }
    }

    bool IList.IsReadOnly
    {
        get
        {
            return true;
        }
    }

    object IList.this[int index]
    {
        get
        {
            return this.olist[index];
        }
        set
        {
            throw new NotSupportedException();
        }
    }

    private static class EmptyInstance
    {
        public static readonly LockedList<T> List;

        static EmptyInstance()
        {
            LockedList<T>.EmptyInstance.List = new LockedList<T>();
        }
    }
}

