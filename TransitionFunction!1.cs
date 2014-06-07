using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct TransitionFunction<T>
{
    public T a;
    public T b;
    public TransitionFunction f;
}

