using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct ODBItem<TItem> : IEquatable<TItem> where TItem: Object
{
    internal ODBNode<TItem> node;
    internal ODBItem(ODBNode<TItem> node)
    {
        this.node = node;
    }

    public override int GetHashCode()
    {
        return this.node.GetHashCode();
    }

    public override string ToString()
    {
        return this.node.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return ((this.node == null) || (this.node.self == 0));
        }
        if (obj is ODBItem<TItem>)
        {
            ODBItem<TItem> item = (ODBItem<TItem>) obj;
            return (item.node == this.node);
        }
        if (obj is Object)
        {
            return (((this.node != null) && (this.node.self != null)) && (this.node.self == ((Object) obj)));
        }
        return obj.Equals(this.node);
    }

    public bool Equals(TItem obj)
    {
        if (obj != null)
        {
            return obj.Equals((TItem) this);
        }
        return ((this.node == null) || (this.node.self == 0));
    }

    public static bool operator ==(ODBItem<TItem> L, ODBItem<TItem> R)
    {
        return (L.node == R.node);
    }

    public static bool operator !=(ODBItem<TItem> L, ODBItem<TItem> R)
    {
        return (L.node != R.node);
    }

    public static bool operator ==(ODBItem<TItem> L, TItem R)
    {
        return L.Equals(R);
    }

    public static bool operator !=(ODBItem<TItem> L, TItem R)
    {
        return !L.Equals(R);
    }

    public static bool operator ==(TItem L, ODBItem<TItem> R)
    {
        return R.Equals(L);
    }

    public static bool operator !=(TItem L, ODBItem<TItem> R)
    {
        return !R.Equals(L);
    }

    public static implicit operator TItem(ODBItem<TItem> item)
    {
        if (item.node != null)
        {
            return item.node.self;
        }
        return null;
    }
}

