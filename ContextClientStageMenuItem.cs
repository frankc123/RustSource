using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct ContextClientStageMenuItem
{
    [NonSerialized]
    public int name;
    [NonSerialized]
    public string text;
}

