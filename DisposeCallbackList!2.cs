using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct DisposeCallbackList<TOwner, TCallback> : IDisposable where TOwner: Object where TCallback: class
{
    private readonly Function<TOwner, TCallback> function;
    private TOwner owner;
    private List<TCallback> list;
    private int destroyIndex;
    private int count;
    public DisposeCallbackList(TOwner owner, Function<TOwner, TCallback> invoke)
    {
        if (invoke == null)
        {
            throw new ArgumentNullException("invoke");
        }
        this.function = invoke;
        this.list = null;
        this.destroyIndex = -1;
        this.count = 0;
        this.owner = owner;
    }

    private void Invoke(TCallback value)
    {
        try
        {
            this.function(this.owner, this.list[this.destroyIndex]);
        }
        catch (Exception exception)
        {
            object[] args = new object[] { value, this.function, this.owner, exception };
            Debug.LogError(string.Format("There was a exception thrown while attempting to invoke '{0}' thru '{1}' via owner '{2}'. exception is below\r\n{3}", args), this.owner);
        }
    }

    public bool Add(TCallback value)
    {
        if (this.list == null)
        {
            this.list = new List<TCallback>();
        }
        else
        {
            int index = this.list.IndexOf(value);
            if (index != -1)
            {
                if ((this.destroyIndex < index) && ((this.count - 1) != index))
                {
                    this.list.RemoveAt(index);
                    this.list.Add(value);
                }
                return false;
            }
        }
        this.list.Add(value);
        if (this.destroyIndex == this.count++)
        {
            this.Invoke(value);
            this.destroyIndex++;
        }
        return true;
    }

    public bool Remove(TCallback value)
    {
        return ((this.destroyIndex == -1) && ((this.list != null) && this.list.Remove(value)));
    }

    public void Dispose()
    {
        if (this.destroyIndex == -1)
        {
            while (++this.destroyIndex < this.count)
            {
                this.Invoke(this.list[this.destroyIndex]);
            }
        }
    }

    public static DisposeCallbackList<TOwner, TCallback> invalid
    {
        get
        {
            return new DisposeCallbackList<TOwner, TCallback>();
        }
    }
    public delegate void Function(TOwner owner, TCallback callback);
}

