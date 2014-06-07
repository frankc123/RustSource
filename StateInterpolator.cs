using System;
using UnityEngine;

public abstract class StateInterpolator : BaseStateInterpolator
{
    [SerializeField]
    protected int _bufferCapacity = 0x20;
    protected int len;

    protected StateInterpolator()
    {
    }

    protected abstract void __Clear();
    public void Clear()
    {
        this.__Clear();
    }

    protected abstract double __newestTimeStamp { get; }

    protected abstract double __oldestTimeStamp { get; }

    protected abstract double __storedDuration { get; }

    public double newestTimeStamp
    {
        get
        {
            return this.__newestTimeStamp;
        }
    }

    public double oldestTimeStamp
    {
        get
        {
            return this.__oldestTimeStamp;
        }
    }

    public double storedDuration
    {
        get
        {
            return this.__storedDuration;
        }
    }
}

