using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct DisposeCallbackList<TCallback> : IDisposable where TCallback: class
{
    private DisposeCallbackList<Object, TCallback> def;
    public DisposeCallbackList(DisposeCallbackList<Object, TCallback>.Function invoke)
    {
        this.def = new DisposeCallbackList<Object, TCallback>(null, invoke);
    }

    public bool Add(TCallback callback)
    {
        return this.def.Add(callback);
    }

    public bool Remove(TCallback callback)
    {
        return this.def.Remove(callback);
    }

    public void Dispose()
    {
        this.def.Dispose();
    }

    public static DisposeCallbackList<TCallback> invalid
    {
        get
        {
            return new DisposeCallbackList<TCallback>();
        }
    }
}

