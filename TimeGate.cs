using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct TimeGate
{
    [NonSerialized]
    private bool initialized;
    [NonSerialized]
    private long startTime;
    public bool started
    {
        get
        {
            return this.initialized;
        }
    }
    public long elapsedMillis
    {
        get
        {
            return (!this.initialized ? 0x7fffffffL : (timeSource - this.startTime));
        }
        set
        {
            if (value == 0x7fffffffL)
            {
                this.initialized = false;
            }
            else
            {
                this.startTime = timeSource - value;
                this.initialized = true;
            }
        }
    }
    public double elapsedSeconds
    {
        get
        {
            return (!this.initialized ? double.PositiveInfinity : (((double) (timeSource - this.startTime)) / 1000.0));
        }
        set
        {
            if (double.IsPositiveInfinity(value))
            {
                this.initialized = false;
            }
            else
            {
                this.startTime = timeSource - SecondsToMS(value);
                this.initialized = true;
            }
        }
    }
    public long timeInMillis
    {
        get
        {
            return (!this.initialized ? 0L : this.startTime);
        }
        set
        {
            this.startTime = value;
            this.initialized = true;
        }
    }
    public double timeInSeconds
    {
        get
        {
            return (!this.initialized ? 0.0 : (((double) this.startTime) / 1000.0));
        }
        set
        {
            this.startTime = SecondsToMS(value);
            this.initialized = true;
        }
    }
    public bool passedOrAtTime
    {
        get
        {
            return (!this.initialized || (this.startTime <= timeSource));
        }
    }
    public bool behindOrAtTime
    {
        get
        {
            return (!this.initialized || (this.startTime >= timeSource));
        }
    }
    public bool passedTime
    {
        get
        {
            return (!this.initialized || (this.startTime < timeSource));
        }
    }
    public bool behindTime
    {
        get
        {
            return (!this.initialized || (this.startTime > timeSource));
        }
    }
    private static long timeSource
    {
        get
        {
            return (long) NetCull.timeInMillis;
        }
    }
    private static long SecondsToMS(double seconds)
    {
        return (long) Math.Floor((double) (seconds * 1000.0));
    }

    public bool ElapsedMillis(long span)
    {
        return (((span <= 0L) || !this.initialized) || ((timeSource - this.startTime) >= span));
    }

    public bool ElapsedSeconds(double seconds)
    {
        return (((seconds <= 0.0) || !this.initialized) || ((timeSource - this.startTime) >= SecondsToMS(seconds)));
    }

    public bool FireMillis(long minimumElapsedTime)
    {
        return ((minimumElapsedTime <= 0L) || this.RefireMillis(-minimumElapsedTime));
    }

    public bool RefireMillis(long intervalMS)
    {
        long timeSource = TimeGate.timeSource;
        if (!this.initialized)
        {
            this.initialized = true;
            this.startTime = timeSource;
            return true;
        }
        if (intervalMS == 0)
        {
            bool flag = timeSource != this.startTime;
            this.startTime = timeSource;
            return flag;
        }
        if (intervalMS < 0L)
        {
            long num2 = this.startTime - timeSource;
            if (num2 <= intervalMS)
            {
                this.startTime = timeSource;
                return true;
            }
            return false;
        }
        long num3 = timeSource - this.startTime;
        if (num3 >= intervalMS)
        {
            this.startTime += intervalMS;
            return true;
        }
        return false;
    }

    public bool RefireSeconds(double intervalSeconds)
    {
        return this.RefireMillis(SecondsToMS(intervalSeconds));
    }

    public static implicit operator TimeGate(double timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = SecondsToMS((((double) timeSource) / 1000.0) - timeRemaining);
        return gate;
    }

    public static implicit operator TimeGate(float timeRemaining)
    {
        return (double) timeRemaining;
    }

    public static implicit operator TimeGate(long timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - timeRemaining;
        return gate;
    }

    public static implicit operator TimeGate(ulong timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - ((long) timeRemaining);
        return gate;
    }

    public static implicit operator TimeGate(int timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - timeRemaining;
        return gate;
    }

    public static implicit operator TimeGate(uint timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - timeRemaining;
        return gate;
    }

    public static implicit operator TimeGate(short timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - timeRemaining;
        return gate;
    }

    public static implicit operator TimeGate(ushort timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - timeRemaining;
        return gate;
    }

    public static implicit operator TimeGate(byte timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - timeRemaining;
        return gate;
    }

    public static implicit operator TimeGate(sbyte timeRemaining)
    {
        TimeGate gate;
        gate.initialized = true;
        gate.startTime = timeSource - timeRemaining;
        return gate;
    }

    public static bool operator true(TimeGate gate)
    {
        return gate.passedOrAtTime;
    }

    public static bool operator false(TimeGate gate)
    {
        return gate.behindTime;
    }
}

