using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct UITextMarkup
{
    public int index;
    public char value;
    public UITextMod mod;
}

