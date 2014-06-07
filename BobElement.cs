using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct BobElement
{
    public Vector3 value;
    public Vector3 time;
}

