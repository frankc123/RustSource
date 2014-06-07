using System;
using System.Runtime.InteropServices;

public class ODBNode<T> : IDisposable where T: Object
{
    private bool hasList;
    public ODBList<T> list;
    public ODBSibling<T> n;
    public ODBSibling<T> p;
    private static Recycler<T> recycle;
    public T self;

    private ODBNode()
    {
    }

    public void Dispose()
    {
        if (this.hasList)
        {
            if (this.n.has)
            {
                if (this.p.has)
                {
                    this.p.item.n = this.n;
                    this.n.item.p = this.p;
                    this.p = new ODBSibling<T>();
                    this.list.count--;
                }
                else
                {
                    this.n.item.p = new ODBSibling<T>();
                    this.list.first = this.n;
                    this.list.count--;
                }
            }
            else if (this.p.has)
            {
                this.p.item.n = new ODBSibling<T>();
                this.list.last = this.p;
                this.p = new ODBSibling<T>();
                this.list.count--;
            }
            else
            {
                this.list.count = 0;
                this.list.any = false;
                this.list.first = new ODBSibling<T>();
                this.list.last = new ODBSibling<T>();
            }
            this.hasList = false;
            this.list = null;
            ODBNode<T>.recycle.Push((ODBNode<T>) this);
        }
    }

    public static ODBNode<T> New(ODBList<T> list, T self)
    {
        ODBNode<T> node;
        if (!ODBNode<T>.recycle.Pop(out node))
        {
            node = new ODBNode<T>();
        }
        node.Setup(list, self);
        return node;
    }

    private void Setup(ODBList<T> list, T self)
    {
        this.self = self;
        this.list = list;
        this.hasList = true;
        this.n = new ODBSibling<T>();
        if (list.any)
        {
            this.p = list.last;
            this.p.item.n.item = (ODBNode<T>) this;
            this.p.item.n.has = true;
            list.last.item = (ODBNode<T>) this;
            list.count++;
        }
        else
        {
            ODBSibling<T> sibling;
            list.count = 1;
            list.any = true;
            sibling.has = true;
            sibling.item = (ODBNode<T>) this;
            list.first = sibling;
            list.last = sibling;
        }
    }

    public ODBForwardEnumerable<T> afterExclusive
    {
        get
        {
            return new ODBForwardEnumerable<T>(this.n);
        }
    }

    public ODBForwardEnumerable<T> afterInclusive
    {
        get
        {
            return new ODBForwardEnumerable<T>((ODBNode<T>) this);
        }
    }

    public ODBReverseEnumerable<T> beforeExclusive
    {
        get
        {
            return new ODBReverseEnumerable<T>(this.p);
        }
    }

    public ODBReverseEnumerable<T> beforeInclusive
    {
        get
        {
            return new ODBReverseEnumerable<T>((ODBNode<T>) this);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Recycler
    {
        public ODBNode<T> items;
        public int count;
        public bool any;
        public bool Pop(out ODBNode<T> o)
        {
            o = this.items;
            if (!this.any)
            {
                return false;
            }
            if (--this.count == 0)
            {
                this.any = false;
                this.items = null;
            }
            else
            {
                this.items = o.n.item;
            }
            return true;
        }

        public void Push(ODBNode<T> item)
        {
            item.list = null;
            item.self = null;
            if (this.any)
            {
                item.n.item = this.items;
                item.n.has = true;
                this.items = item;
                this.count++;
            }
            else
            {
                item.n = new ODBSibling<T>();
                this.items = item;
                this.count = 1;
                this.any = true;
            }
        }
    }
}

