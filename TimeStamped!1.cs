﻿using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct TimeStamped<T>
{
    public double timeStamp;
    public int index;
    public T value;
    public void Set(ref T value, ref double timeStamp)
    {
        this.timeStamp = timeStamp;
        this.value = value;
    }
}

