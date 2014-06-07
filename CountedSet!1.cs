using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class CountedSet<TValue> : IEnumerable, IEnumerable<TValue>, ICollection<TValue>
{
    private static TValue[] empty;
    private Dictionary<TValue, Node<TValue>> index;
    private uint nodeCount;
    private uint totalRetains;

    static CountedSet()
    {
        CountedSet<TValue>.empty = new TValue[0];
    }

    public CountedSet(IEnumerable<TValue> values, IEqualityComparer<TValue> comparer)
    {
        this.index = new Dictionary<TValue, Node<TValue>>(comparer);
        IEnumerator<TValue> enumerator = values.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                TValue current = enumerator.Current;
                this.Retain(current);
            }
        }
        finally
        {
            if (enumerator == null)
            {
            }
            enumerator.Dispose();
        }
    }

    public bool Contains(TValue value)
    {
        return this.index.ContainsKey(value);
    }

    private static EqualityComparer<Node<TValue>> ConvertEqualityComparer(IEqualityComparer<TValue> comparer)
    {
        if ((comparer != null) && (comparer != DefaultComparer<TValue>.Singleton.Value.Comparer))
        {
            return new CustomComparer<TValue>(comparer);
        }
        return DefaultComparer<TValue>.Singleton.Value;
    }

    public Dictionary<TValue, Node<TValue>>.KeyCollection.Enumerator GetEnumerator()
    {
        return this.index.Keys.GetEnumerator();
    }

    public int Release(TValue value)
    {
        Node<TValue> node;
        if (!this.index.TryGetValue(value, out node))
        {
            return -1;
        }
        bool flag = node.Release();
        this.totalRetains--;
        if (flag)
        {
            this.index.Remove(value);
            this.nodeCount--;
        }
        return (int) node.count;
    }

    public TValue[] ReleaseAll()
    {
        using (ReleaseRecursor<TValue> recursor = new ReleaseRecursor<TValue>((CountedSet<TValue>) this))
        {
            recursor.Run();
            return recursor.array;
        }
    }

    public int Retain(TValue value)
    {
        Node<TValue> node;
        if (!this.index.TryGetValue(value, out node))
        {
            Node<TValue> node2 = new Node<TValue> {
                v = value
            };
            this.index[value] = node = node2;
            this.nodeCount++;
        }
        uint count = node.count;
        node.Retain();
        this.totalRetains++;
        return (int) count;
    }

    public void RetainAll()
    {
        foreach (Node<TValue> node in this.index.Values)
        {
            node.Retain();
            this.totalRetains++;
        }
    }

    void ICollection<TValue>.Add(TValue item)
    {
        this.index.Keys.Add(item);
    }

    void ICollection<TValue>.Clear()
    {
        this.index.Keys.Clear();
    }

    void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
    {
        this.index.Keys.CopyTo(array, arrayIndex);
    }

    bool ICollection<TValue>.Remove(TValue item)
    {
        throw new NotSupportedException();
    }

    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
        return this.index.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.index.Keys.GetEnumerator();
    }

    public int Count
    {
        get
        {
            return (int) this.nodeCount;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return true;
        }
    }

    public int this[TValue value]
    {
        get
        {
            Node<TValue> node;
            return (!this.index.TryGetValue(value, out node) ? -1 : (((int) node.count) - 1));
        }
    }

    private class CustomComparer : EqualityComparer<CountedSet<TValue>.Node>, IDisposable
    {
        private IEqualityComparer<TValue> comparer;

        public CustomComparer(IEqualityComparer<TValue> comparer)
        {
            this.comparer = comparer;
        }

        public void Dispose()
        {
            if (this.comparer is IDisposable)
            {
                ((IDisposable) this.comparer).Dispose();
            }
            this.comparer = null;
        }

        public override bool Equals(CountedSet<TValue>.Node x, CountedSet<TValue>.Node y)
        {
            return this.comparer.Equals(x.v, y.v);
        }

        public override int GetHashCode(CountedSet<TValue>.Node obj)
        {
            return this.comparer.GetHashCode(obj.v);
        }
    }

    private class DefaultComparer : EqualityComparer<CountedSet<TValue>.Node>
    {
        public readonly EqualityComparer<TValue> Comparer;

        private DefaultComparer()
        {
            this.Comparer = EqualityComparer<TValue>.Default;
        }

        public override bool Equals(CountedSet<TValue>.Node x, CountedSet<TValue>.Node y)
        {
            return this.Comparer.Equals(x.v, y.v);
        }

        public override int GetHashCode(CountedSet<TValue>.Node obj)
        {
            return this.Comparer.GetHashCode(obj.v);
        }

        public static class Singleton
        {
            public static readonly CountedSet<TValue>.DefaultComparer Value;

            static Singleton()
            {
                CountedSet<TValue>.DefaultComparer.Singleton.Value = new CountedSet<TValue>.DefaultComparer();
            }
        }
    }

    public class Node
    {
        public uint count;
        public bool done;
        public TValue v;

        public bool Release()
        {
            return (!this.done && (--this.count == 0));
        }

        public bool Retain()
        {
            if (this.done)
            {
                return false;
            }
            return (this.count++ == 0);
        }

        public uint ReferenceCount
        {
            get
            {
                return (this.count + 1);
            }
        }

        public bool Released
        {
            get
            {
                return this.done;
            }
        }

        public bool Retained
        {
            get
            {
                return !this.done;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ReleaseRecursor : IDisposable
    {
        private CountedSet<TValue> s;
        private Dictionary<TValue, CountedSet<TValue>.Node> dict;
        private Dictionary<TValue, CountedSet<TValue>.Node>.ValueCollection.Enumerator enumerator;
        public TValue[] array;
        private int count;
        private bool disposed;
        public ReleaseRecursor(CountedSet<TValue> v)
        {
            this.s = v;
            this.dict = this.s.index;
            this.enumerator = this.dict.Values.GetEnumerator();
            this.array = CountedSet<TValue>.empty;
            this.count = 0;
            this.disposed = false;
        }

        public void Run()
        {
            if (this.enumerator.MoveNext())
            {
                CountedSet<TValue>.Node current = this.enumerator.Current;
                if (current.Release())
                {
                    this.s.totalRetains--;
                    this.count++;
                    this.Run();
                    this.dict.Remove(current.v);
                    this.s.nodeCount--;
                    this.array[this.count--] = current.v;
                }
                else
                {
                    this.s.totalRetains--;
                }
            }
            else
            {
                this.Dispose();
                if (this.count > 0)
                {
                    this.array = new TValue[this.count];
                }
                this.count--;
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.enumerator.Dispose();
            }
        }
    }
}

