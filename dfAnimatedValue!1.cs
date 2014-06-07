using System;
using UnityEngine;

public abstract class dfAnimatedValue<T> where T: struct
{
    private float animLength;
    private dfEasingFunctions.EasingFunction easingFunction;
    private dfEasingType easingType;
    private T endValue;
    private float startTime;
    private T startValue;

    protected internal dfAnimatedValue()
    {
        this.animLength = 1f;
        this.startTime = Time.realtimeSinceStartup;
        this.easingFunction = dfEasingFunctions.GetFunction(this.easingType);
    }

    protected internal dfAnimatedValue(T StartValue, T EndValue, float Time) : this()
    {
        this.startValue = StartValue;
        this.endValue = EndValue;
        this.animLength = Time;
    }

    protected abstract T Lerp(T startValue, T endValue, float time);
    public static implicit operator T(dfAnimatedValue<T> animated)
    {
        return animated.Value;
    }

    public dfEasingType EasingType
    {
        get
        {
            return this.easingType;
        }
        set
        {
            this.easingType = value;
            this.easingFunction = dfEasingFunctions.GetFunction(this.easingType);
        }
    }

    public T EndValue
    {
        get
        {
            return this.endValue;
        }
        set
        {
            this.endValue = value;
            this.startTime = Time.realtimeSinceStartup;
        }
    }

    public bool IsDone
    {
        get
        {
            return ((Time.realtimeSinceStartup - this.startTime) >= this.Length);
        }
    }

    public float Length
    {
        get
        {
            return this.animLength;
        }
        set
        {
            this.animLength = value;
            this.startTime = Time.realtimeSinceStartup;
        }
    }

    public T StartValue
    {
        get
        {
            return this.startValue;
        }
        set
        {
            this.startValue = value;
            this.startTime = Time.realtimeSinceStartup;
        }
    }

    public T Value
    {
        get
        {
            float num = Time.realtimeSinceStartup - this.startTime;
            if (num >= this.animLength)
            {
                return this.endValue;
            }
            float time = Mathf.Clamp01(num / this.animLength);
            time = this.easingFunction(0f, 1f, time);
            return this.Lerp(this.startValue, this.endValue, time);
        }
    }
}

