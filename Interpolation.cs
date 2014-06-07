using System;
using System.Runtime.InteropServices;

public static class Interpolation
{
    private static ulong _delayFromSendRateMillis;
    private static double _delayFromSendRateSeconds;
    private static ulong _delayMillis;
    private static double _delaySeconds;
    private static double _deltaSeconds;
    private static double _ratio;
    private static float _sendRate;
    private static ulong _totalDelayMillis;
    private static double _totalDelaySeconds;
    private const int kDefaultDelayMillis = 20;
    private const float kDefaultSendRate = 5f;
    private const float kDefaultSendRateRatio = 1.5f;
    public static TimingData @struct;

    static Interpolation()
    {
        BindTiming((ulong) 20L, 1.5, 5f);
    }

    public static ulong AddDelayToTimeStampMillis(ulong timestamp)
    {
        return (timestamp + _totalDelayMillis);
    }

    public static double AddDelayToTimeStampSeconds(double timeStamp)
    {
        return (timeStamp + _totalDelaySeconds);
    }

    public static void BindTiming()
    {
        BindTiming(_delayMillis, _ratio, _sendRate);
    }

    public static void BindTiming(ulong delayMillis, double sendRateRatio, float sendRate)
    {
        _sendRate = sendRate;
        _ratio = sendRateRatio;
        if (((sendRate == 0f) || (sendRateRatio == 0.0)) || ((sendRate < 0f) != (sendRateRatio < 0.0)))
        {
            _delayFromSendRateMillis = 0L;
        }
        else
        {
            _delayFromSendRateMillis = (ulong) Math.Ceiling((double) ((1000.0 * sendRateRatio) / ((double) sendRate)));
        }
        _delayMillis = delayMillis;
        _totalDelayMillis = _delayFromSendRateMillis + _delayMillis;
        _delaySeconds = _delayMillis * 0.001;
        _delayFromSendRateSeconds = _delayFromSendRateMillis * 0.001;
        _totalDelaySeconds = _totalDelayMillis * 0.001;
        _deltaSeconds = -_totalDelaySeconds;
        @struct = Capture();
    }

    public static void BindTiming(ulong? delayMillis, double? sendRateRatio, float? sendRate)
    {
        BindTiming(!delayMillis.HasValue ? _delayMillis : delayMillis.Value, !sendRateRatio.HasValue ? _ratio : sendRateRatio.Value, !sendRate.HasValue ? _sendRate : sendRate.Value);
    }

    public static void BindTimingNetCull()
    {
        BindTiming(_delayMillis, _ratio, NetCull.sendRate);
    }

    public static void BindTimingNetCull(ulong? delayMillis, double? sendRateRatio)
    {
        BindTiming(!delayMillis.HasValue ? _delayMillis : delayMillis.Value, !sendRateRatio.HasValue ? _ratio : sendRateRatio.Value, NetCull.sendRate);
    }

    public static void BindTimingNetCull(ulong delayMillis, double sendRateRatio)
    {
        BindTiming(delayMillis, sendRateRatio, NetCull.sendRate);
    }

    public static TimingData Capture()
    {
        return new TimingData(_ratio, _deltaSeconds, _totalDelaySeconds, _delaySeconds, _delayFromSendRateSeconds, _totalDelayMillis, _delayFromSendRateMillis, _delayMillis, _sendRate);
    }

    public static ulong GetInterpolationTimeMillis(ulong timestamp)
    {
        if (timestamp < _totalDelayMillis)
        {
            return 0L;
        }
        return (timestamp - _totalDelayMillis);
    }

    public static double GetInterpolationTimeSeconds(double timeStamp)
    {
        return (timeStamp + _deltaSeconds);
    }

    public static ulong delayFromSendRateMillis
    {
        get
        {
            return _delayFromSendRateMillis;
        }
    }

    public static double delayFromSendRateSeconds
    {
        get
        {
            return _delayFromSendRateSeconds;
        }
    }

    public static float delayFromSendRateSecondsf
    {
        get
        {
            return (float) _delayFromSendRateSeconds;
        }
    }

    public static ulong delayMillis
    {
        get
        {
            return _delayMillis;
        }
        set
        {
            if (value != _delayMillis)
            {
                BindTiming(value, _ratio, _sendRate);
            }
        }
    }

    public static double delaySeconds
    {
        get
        {
            return _delaySeconds;
        }
        set
        {
            if (value < 0.0005)
            {
                delayMillis = 0L;
            }
            else
            {
                delayMillis = (ulong) Math.Round((double) (value * 1000.0));
            }
        }
    }

    public static float delaySecondsf
    {
        get
        {
            return (float) _delaySeconds;
        }
        set
        {
            delaySeconds = value;
        }
    }

    public static double deltaSeconds
    {
        get
        {
            return _deltaSeconds;
        }
    }

    public static float deltaSecondsf
    {
        get
        {
            return (float) _deltaSeconds;
        }
    }

    public static double localTime
    {
        get
        {
            return (NetCull.localTime + _deltaSeconds);
        }
    }

    public static ulong localTimeInMillis
    {
        get
        {
            ulong localTimeInMillis = NetCull.localTimeInMillis;
            if (localTimeInMillis < _totalDelayMillis)
            {
                return 0L;
            }
            return (localTimeInMillis - _totalDelayMillis);
        }
    }

    public static float sendRate
    {
        get
        {
            return _sendRate;
        }
        set
        {
            if (value != _sendRate)
            {
                BindTiming(_delayMillis, _ratio, value);
            }
        }
    }

    public static double sendRateRatio
    {
        get
        {
            return _ratio;
        }
        set
        {
            if (value != _ratio)
            {
                BindTiming(_delayMillis, value, _sendRate);
            }
        }
    }

    public static float sendRateRatiof
    {
        get
        {
            return (float) _ratio;
        }
        set
        {
            sendRateRatio = value;
        }
    }

    public static double time
    {
        get
        {
            return (NetCull.time + _deltaSeconds);
        }
    }

    public static ulong timeInMillis
    {
        get
        {
            ulong timeInMillis = NetCull.timeInMillis;
            if (timeInMillis < _totalDelayMillis)
            {
                return 0L;
            }
            return (timeInMillis - _totalDelayMillis);
        }
    }

    public static ulong totalDelayMillis
    {
        get
        {
            return _totalDelayMillis;
        }
    }

    public static double totalDelaySeconds
    {
        get
        {
            return _totalDelaySeconds;
        }
    }

    public static float totalDelaySecondsf
    {
        get
        {
            return (float) _totalDelaySeconds;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TimingData
    {
        public readonly double sendRateRatio;
        public readonly double deltaSeconds;
        public readonly double totalDelaySeconds;
        public readonly double delaySeconds;
        public readonly double delayFromSendRateSeconds;
        public readonly float sendRateRatioF;
        public readonly float deltaSecondsF;
        public readonly float totalDelaySecondsF;
        public readonly float delaySecondsF;
        public readonly float delayFromSendRateSecondsF;
        public readonly ulong totalDelayMillis;
        public readonly ulong delayFromSendRateMillis;
        public readonly ulong delayMillis;
        public readonly float sendRate;
        public TimingData(double sendRateRatio, double deltaSeconds, double totalDelaySeconds, double delaySeconds, double delayFromSendRateSeconds, ulong totalDelayMillis, ulong delayFromSendRateMillis, ulong delayMillis, float sendRate)
        {
            this.sendRateRatio = sendRateRatio;
            this.deltaSeconds = deltaSeconds;
            this.totalDelaySeconds = totalDelaySeconds;
            this.delaySeconds = delaySeconds;
            this.delayFromSendRateSeconds = delayFromSendRateSeconds;
            this.totalDelayMillis = totalDelayMillis;
            this.delayFromSendRateMillis = delayFromSendRateMillis;
            this.delayMillis = delayMillis;
            this.sendRate = sendRate;
            this.sendRateRatioF = (float) sendRateRatio;
            this.deltaSecondsF = (float) deltaSeconds;
            this.totalDelaySecondsF = (float) totalDelaySeconds;
            this.delaySecondsF = (float) delaySeconds;
            this.delayFromSendRateSecondsF = (float) delayFromSendRateSeconds;
        }
    }
}

