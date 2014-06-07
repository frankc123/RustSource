using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ODBList<T> : IEnumerable, IEnumerable<T>, ICollection<T>, ODBEnumerable<T, ODBForwardEnumerator<T>> where T: Object
{
    public bool any;
    public int count;
    public ODBSibling<T> first;
    protected readonly HSet<T> hashSet;
    private readonly bool isReadOnly;
    public ODBSibling<T> last;

    protected ODBList()
    {
        this.hashSet = new HSet<T>();
    }

    protected ODBList(bool isReadOnly) : this()
    {
        this.isReadOnly = isReadOnly;
    }

    protected ODBList(IEnumerable<T> collection) : this()
    {
        IEnumerator<T> enumerator = collection.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                this.DoAdd(current);
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

    protected ODBList(bool isReadOnly, IEnumerable<T> collection) : this(collection)
    {
        this.isReadOnly = isReadOnly;
    }

    public bool Contains(T item)
    {
        return (this.any && this.hashSet.Contains(item));
    }

    public bool Contains(ODBNode<T> item)
    {
        return (this.any && (item.list == this));
    }

    public int CopyTo(T[] array)
    {
        return this.CopyTo(array, 0, this.count);
    }

    public int CopyTo(T[] array, int arrayIndex)
    {
        return this.CopyTo(array, arrayIndex, this.count);
    }

    public int CopyTo(T[] array, int arrayIndex, int count)
    {
        if (!this.any)
        {
            return 0;
        }
        ODBNode<T> item = this.first.item;
        int num = -1;
        if (count > this.count)
        {
            count = this.count;
        }
        while (++num < count)
        {
            array[arrayIndex++] = item.self;
            item = item.n.item;
        }
        return num;
    }

    protected bool DoAdd(T item)
    {
        if (item == null)
        {
            throw new MissingReferenceException("You cannot pass a missing or null item into the list");
        }
        if (this.hashSet.Add(item))
        {
            ODBNode<T>.New((ODBList<T>) this, item);
            return true;
        }
        return false;
    }

    protected bool DoAdd(T item, out ODBNode<T> node)
    {
        if (item == null)
        {
            throw new MissingReferenceException("You cannot pass a missing or null item into the list");
        }
        if (this.hashSet.Add(item))
        {
            node = ODBNode<T>.New((ODBList<T>) this, item);
            return true;
        }
        node = null;
        return false;
    }

    protected void DoClear()
    {
        if (this.any)
        {
            this.hashSet.Clear();
            do
            {
                this.first.item.Dispose();
            }
            while (this.any);
        }
    }

    protected void DoExceptWith(ODBList<T> list)
    {
        if (this.any && list.any)
        {
            if (list == this)
            {
                this.DoClear();
            }
            else
            {
                ODBSibling<T> first = list.first;
                do
                {
                    T self = first.item.self;
                    first = first.item.n;
                    if (this.hashSet.Remove(self))
                    {
                        this.KnownFind(self).Dispose();
                    }
                }
                while (first.has);
            }
        }
    }

    protected void DoIntersectWith(ODBList<T> list)
    {
        if (this.any)
        {
            if (list.any)
            {
                if (list != this)
                {
                    this.hashSet.IntersectWith(list.hashSet);
                    int count = this.hashSet.Count;
                    if (count == 0)
                    {
                        while (this.any)
                        {
                            this.first.item.Dispose();
                        }
                    }
                    else
                    {
                        ODBSibling<T> first = this.first;
                        do
                        {
                            ODBNode<T> item = first.item;
                            first = first.item.n;
                            if (!this.hashSet.Contains(item.self))
                            {
                                item.Dispose();
                                if (this.count == count)
                                {
                                    break;
                                }
                            }
                        }
                        while (first.has);
                    }
                }
            }
            else
            {
                this.DoClear();
            }
        }
    }

    protected bool DoRemove(ref ODBNode<T> node)
    {
        if (this.any && (node.list == this))
        {
            this.hashSet.Remove(node.self);
            node.Dispose();
            node = null;
            return true;
        }
        return false;
    }

    protected bool DoRemove(T item)
    {
        if (this.any && this.hashSet.Remove(item))
        {
            this.KnownFind(item).Dispose();
            return true;
        }
        return false;
    }

    protected void DoSymmetricExceptWith(ODBList<T> list)
    {
        if (this.any)
        {
            if (list.any)
            {
                if (list == this)
                {
                    this.DoClear();
                }
                else
                {
                    ODBSibling<T> first = list.first;
                    do
                    {
                        T self = first.item.self;
                        first = first.item.n;
                        if (this.hashSet.Remove(self))
                        {
                            this.KnownFind(self).Dispose();
                        }
                        else
                        {
                            this.hashSet.Add(self);
                            ODBNode<T>.New((ODBList<T>) this, self);
                        }
                    }
                    while (first.has);
                }
            }
        }
        else if (list.any)
        {
            ODBSibling<T> n = list.first;
            do
            {
                T item = n.item.self;
                n = n.item.n;
                this.hashSet.Add(item);
                ODBNode<T>.New((ODBList<T>) this, item);
            }
            while (n.has);
        }
    }

    protected void DoUnionWith(ODBList<T> list)
    {
        if (list.any && (list != this))
        {
            ODBSibling<T> first = list.first;
            do
            {
                T self = first.item.self;
                first = first.item.n;
                if (this.hashSet.Add(self))
                {
                    ODBNode<T>.New((ODBList<T>) this, self);
                }
            }
            while (first.has);
        }
    }

    public RecycleList<T> ExceptList(ODBList<T> list)
    {
        return this.hashSet.ExceptList(list.hashSet);
    }

    public RecycleList<T> ExceptList(IEnumerable<T> e)
    {
        return this.hashSet.ExceptList(e);
    }

    public ODBForwardEnumerator<T> GetEnumerator()
    {
        return new ODBForwardEnumerator<T>((ODBList<T>) this);
    }

    public RecycleList<T> IntersectList(ODBList<T> list)
    {
        return this.hashSet.IntersectList(list.hashSet);
    }

    public RecycleList<T> IntersectList(IEnumerable<T> e)
    {
        return this.hashSet.IntersectList(e);
    }

    protected ODBNode<T> KnownFind(T item)
    {
        ODBSibling<T> first = this.first;
        while (first.item.self != item)
        {
            first = first.item.n;
            if (!first.has)
            {
                throw new ArgumentException("item was not found", "item");
            }
        }
        return first.item;
    }

    public RecycleList<T> OperList(HSetOper oper, ODBList<T> list)
    {
        return this.hashSet.OperList(oper, list.hashSet);
    }

    public RecycleList<T> OperList(HSetOper oper, IEnumerable<T> collection)
    {
        return this.hashSet.OperList(oper, collection);
    }

    public RecycleList<T> SymmetricExceptList(ODBList<T> list)
    {
        return this.hashSet.SymmetricExceptList(list.hashSet);
    }

    public RecycleList<T> SymmetricExceptList(IEnumerable<T> e)
    {
        return this.hashSet.SymmetricExceptList(e);
    }

    void ICollection<T>.Add(T item)
    {
        if (this.isReadOnly)
        {
            throw new NotSupportedException("Read Only");
        }
        if (!this.DoAdd(item))
        {
            throw new ArgumentException("The list already contains the given item " + item, "item");
        }
    }

    void ICollection<T>.Clear()
    {
        if (this.isReadOnly)
        {
            throw new NotSupportedException("Read Only");
        }
        this.DoClear();
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        this.CopyTo(array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item)
    {
        if (this.isReadOnly)
        {
            throw new NotSupportedException("Read Only");
        }
        return this.DoRemove(item);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return ODBCachedEnumerator<T, ODBForwardEnumerator<T>>.Cache(ref this.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public T[] ToArray()
    {
        T[] array = new T[this.count];
        this.CopyTo(array, 0, this.count);
        return array;
    }

    public IEnumerable<T> ToGeneric()
    {
        return this;
    }

    public RecycleList<T> UnionList(ODBList<T> list)
    {
        return this.hashSet.UnionList(list.hashSet);
    }

    public RecycleList<T> UnionList(IEnumerable<T> e)
    {
        return this.hashSet.UnionList(e);
    }

    public ODBForwardEnumerable<T> forward
    {
        get
        {
            return new ODBForwardEnumerable<T>((ODBList<T>) this);
        }
    }

    public ODBReverseEnumerable<T> reverse
    {
        get
        {
            return new ODBReverseEnumerable<T>((ODBList<T>) this);
        }
    }

    int ICollection<T>.Count
    {
        get
        {
            return this.count;
        }
    }

    bool ICollection<T>.IsReadOnly
    {
        get
        {
            return this.isReadOnly;
        }
    }
}

