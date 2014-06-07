using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ODBSibling<T> where T: Object
{
    public ODBNode<T> item;
    public bool has;
}

