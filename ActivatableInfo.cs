using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ActivatableInfo
{
    public bool requiresInstigator;
    public bool alwaysToggleActTrigger;
}

