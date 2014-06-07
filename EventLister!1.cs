using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct EventLister<T>
{
    public static readonly Type InvokeCallType;
    public static readonly Type CalleeType;
    public static readonly MethodInfo CalleeMethod;
    private Node<T> node;
    static EventLister()
    {
        if (!typeof(T).IsSubclassOf(typeof(Delegate)))
        {
            throw new InvalidOperationException("T is not a delegate");
        }
        EventListerInvokeAttribute attribute = (EventListerInvokeAttribute) Attribute.GetCustomAttribute(typeof(T), typeof(EventListerInvokeAttribute), false);
        EventLister<T>.InvokeCallType = attribute.InvokeCall;
        EventLister<T>.CalleeType = attribute.InvokeClass;
        MethodInfo method = EventLister<T>.InvokeCallType.GetMethod("Invoke");
        ParameterInfo[] parameters = method.GetParameters();
        Type[] types = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            types[i] = parameters[i].ParameterType;
        }
        EventLister<T>.CalleeMethod = EventLister<T>.CalleeType.GetMethod(attribute.InvokeMember, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, method.CallingConvention, types, null);
        ParameterInfo[] infoArray2 = EventLister<T>.CalleeMethod.GetParameters();
        for (int j = 0; j < infoArray2.Length; j++)
        {
            if ((infoArray2[j].Attributes & parameters[j].Attributes) != parameters[j].Attributes)
            {
                throw new InvalidOperationException("Parameter does not match the InvokeCall " + infoArray2[j]);
            }
        }
    }

    public bool empty
    {
        get
        {
            return object.ReferenceEquals(this.node, null);
        }
    }
    public bool Add(T callback)
    {
        if (object.ReferenceEquals(this.node, null))
        {
            this.node = new Node<T>(callback);
            return true;
        }
        if (this.node.hashSet.Add(callback))
        {
            this.node.list.Add(callback);
            this.node.count++;
            return true;
        }
        return false;
    }

    public bool Remove(T callback)
    {
        if (object.ReferenceEquals(this.node, null) || !this.node.hashSet.Remove(callback))
        {
            return false;
        }
        if ((--this.node.count == 0) && !this.node.invoking)
        {
            this.node = null;
        }
        else
        {
            int index = this.node.list.IndexOf(callback);
            this.node.list.RemoveAt(index);
            if (this.node.iter > index)
            {
                this.node.iter--;
            }
        }
        return true;
    }

    public void Clear()
    {
        this.node = null;
    }

    public bool Invoke<C>(C caller)
    {
        if (object.ReferenceEquals(this.node, null))
        {
            return false;
        }
        if (this.node.invoking)
        {
            throw new InvalidOperationException("This lister is invoking already");
        }
        ExecCall<T, C> invoke = Invocation<T, C>.Invoke;
        try
        {
            this.node.invoking = true;
            this.node.iter = 0;
            while (this.node.iter < this.node.count)
            {
                T callback = this.node.list[this.node.iter++];
                try
                {
                    invoke(caller, callback);
                    continue;
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                    continue;
                }
            }
        }
        finally
        {
            if (this.node.count == 0)
            {
                this.node = null;
            }
            else
            {
                this.node.invoking = false;
            }
        }
        return true;
    }

    public void InvokeManual<C>(T callback, C caller)
    {
        try
        {
            Invocation<T, C>.Invoke(caller, callback);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    public bool Invoke<C, D>(C caller, ref D data)
    {
        if (object.ReferenceEquals(this.node, null))
        {
            return false;
        }
        if (this.node.invoking)
        {
            throw new InvalidOperationException("This lister is invoking already");
        }
        ExecCall<T, C, D> invoke = Invocation<T, C, D>.Invoke;
        try
        {
            this.node.invoking = true;
            this.node.iter = 0;
            while (this.node.iter < this.node.count)
            {
                T callback = this.node.list[this.node.iter++];
                try
                {
                    invoke(caller, ref data, callback);
                    continue;
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                    continue;
                }
            }
        }
        finally
        {
            if (this.node.count == 0)
            {
                this.node = null;
            }
            else
            {
                this.node.invoking = false;
            }
        }
        return true;
    }

    public void InvokeManual<C, D>(T callback, C caller, ref D data)
    {
        try
        {
            Invocation<T, C, D>.Invoke(caller, ref data, callback);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }
    public delegate void ExecCall<C>(C caller, T callback);

    public delegate void ExecCall<C, D>(C caller, ref D data, T callback);

    internal static class Invocation<C>
    {
        public static readonly EventLister<T>.ExecCall<C> Invoke;

        static Invocation()
        {
            if (EventLister<T>.InvokeCallType != typeof(EventLister<T>.ExecCall<C>))
            {
                throw new InvalidOperationException(EventLister<T>.InvokeCallType.Name + " should have been used.");
            }
            EventLister<T>.Invocation<C>.Invoke = (EventLister<T>.ExecCall<C>) Delegate.CreateDelegate(typeof(EventLister<T>.ExecCall<C>), EventLister<T>.CalleeMethod);
        }
    }

    internal static class Invocation<C, D>
    {
        public static readonly EventLister<T>.ExecCall<C, D> Invoke;

        static Invocation()
        {
            if (EventLister<T>.InvokeCallType != typeof(EventLister<T>.ExecCall<C, D>))
            {
                throw new InvalidOperationException(EventLister<T>.InvokeCallType.Name + " should have been used.");
            }
            EventLister<T>.Invocation<C, D>.Invoke = (EventLister<T>.ExecCall<C, D>) Delegate.CreateDelegate(typeof(EventLister<T>.ExecCall<C, D>), EventLister<T>.CalleeMethod);
        }
    }

    private sealed class Node
    {
        internal int count;
        internal readonly HashSet<T> hashSet;
        internal bool invoking;
        internal int iter;
        internal readonly List<T> list;

        internal Node(T callback)
        {
            this.hashSet = new HashSet<T>();
            this.list = new List<T>();
            this.hashSet.Add(callback);
            this.list.Add(callback);
            this.count = 1;
        }
    }
}

