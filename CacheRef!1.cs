using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct CacheRef<T> where T: Object
{
    [NonSerialized]
    public T value;
    [NonSerialized]
    public readonly bool cached;
    [NonSerialized]
    private bool existed;
    private CacheRef(T value)
    {
        this.value = value;
        this.existed = value;
        this.cached = true;
    }

    public bool alive
    {
        get
        {
            return (this.existed && ((bool) (this.existed = this.value)));
        }
    }
    public bool Get(out T value)
    {
        value = this.value;
        return ((this.cached && this.existed) && ((bool) (this.existed = (T) value)));
    }

    public static implicit operator CacheRef<T>(T value)
    {
        return new CacheRef<T>(value);
    }
}

