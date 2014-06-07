using System;
using System.Runtime.InteropServices;
using uLink;

[StructLayout(LayoutKind.Sequential)]
public struct NetClockTester
{
    public Stamping Send;
    public Stamping Receive;
    [NonSerialized]
    public ulong Count;
    public Validity Results;
    public ValidityFlags LastTestFlags;
    public bool Any
    {
        get
        {
            return (this.Count > 0L);
        }
    }
    public bool Empty
    {
        get
        {
            return (this.Count == 0L);
        }
    }
    public static ValidityFlags TestValidity(ref NetClockTester test, ref NetworkMessageInfo info, double intervalSec, ValidityFlags testFor)
    {
        return TestValidity(ref test, ref info, (long) Math.Floor((double) (intervalSec * 1000.0)), testFor);
    }

    public static ValidityFlags TestValidity(ref NetClockTester test, ref NetworkMessageInfo info, long intervalMS, ValidityFlags testFor)
    {
        ValidityFlags flags = TestValidity(ref test, info.timestampInMillis, intervalMS);
        test.Results.Add(flags & testFor);
        return flags;
    }

    private static ValidityFlags TestValidity(ref NetClockTester test, ulong timeStamp, long minimalSendRateMS)
    {
        ulong timeInMillis = NetCull.timeInMillis;
        ValidityFlags flags = (timeInMillis >= timeStamp) ? ((ValidityFlags) 0) : ValidityFlags.AheadOfServerTime;
        if (test.Count > 0L)
        {
            long b = Subtract(timeStamp, test.Send.Last);
            long num3 = Subtract(timeInMillis, test.Receive.Last);
            test.Send.Sum = Add(test.Send.Sum, b);
            test.Receive.Sum = Add(test.Receive.Sum, num3);
            test.Count += (ulong) 1L;
            test.Send.Last = timeStamp;
            test.Receive.Last = timeInMillis;
            if (b < minimalSendRateMS)
            {
                flags |= ValidityFlags.TooFrequent;
            }
            long num4 = Subtract(test.Send.Last, test.Send.First);
            long num5 = Subtract(test.Receive.Last, test.Receive.First);
            if (test.Count >= 5L)
            {
                if (num4 > (num5 * 2L))
                {
                    flags |= ValidityFlags.OverTimed;
                }
            }
            else if ((test.Count >= 3L) && (num4 > (num5 * 4L)))
            {
                flags |= ValidityFlags.OverTimed;
            }
            ValidityFlags lastTestFlags = test.LastTestFlags;
            test.LastTestFlags = flags;
            if (((flags & ValidityFlags.TooFrequent) == ValidityFlags.TooFrequent) && ((lastTestFlags & ValidityFlags.TooFrequent) != ValidityFlags.TooFrequent))
            {
                flags &= ~ValidityFlags.TooFrequent;
                test.Count = 1L;
                test.Send.First = test.Send.Last;
                test.Send.Sum = 0L;
                if (num3 > 0L)
                {
                    test.Receive.First = (ulong) Subtract(test.Receive.Last, (ulong) num3);
                    test.Receive.Sum = (ulong) num3;
                }
                else
                {
                    test.Receive.First = test.Receive.Last;
                    test.Receive.Sum = 0L;
                }
            }
            return ((flags != 0) ? flags : ValidityFlags.Valid);
        }
        test.Send.Sum = (ulong) (test.Receive.Sum = 0L);
        test.Send.Last = test.Send.First = timeStamp;
        test.Receive.Last = test.Receive.First = timeInMillis;
        test.Count = 1L;
        return flags;
    }

    public static NetClockTester Reset
    {
        get
        {
            return new NetClockTester();
        }
    }
    private static long Subtract(ulong a, ulong b)
    {
        if (a > b)
        {
            return (long) (a - b);
        }
        if (a < b)
        {
            return (long) -(b - a);
        }
        return 0L;
    }

    private static ulong Add(ulong a, long b)
    {
        if (b >= 0L)
        {
            return (a + ((ulong) b));
        }
        if (a > -b)
        {
            return (a - ((ulong) -b));
        }
        return 0L;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Stamping
    {
        public ulong Last;
        public ulong First;
        public ulong Sum;
        public long Duration
        {
            get
            {
                return NetClockTester.Subtract(this.Last, this.First);
            }
        }
        public long Variance
        {
            get
            {
                return (((long) this.Sum) - NetClockTester.Subtract(this.Last, this.First));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Validity
    {
        public uint TooFrequent;
        public uint OverTimed;
        public uint AheadOfServerTime;
        public uint Valid;
        public NetClockTester.ValidityFlags Flags
        {
            get
            {
                if (this.TooFrequent > 0)
                {
                    if (this.OverTimed > 0)
                    {
                        if (this.AheadOfServerTime > 0)
                        {
                            return (NetClockTester.ValidityFlags.AheadOfServerTime | NetClockTester.ValidityFlags.OverTimed | NetClockTester.ValidityFlags.TooFrequent);
                        }
                        return (NetClockTester.ValidityFlags.OverTimed | NetClockTester.ValidityFlags.TooFrequent);
                    }
                    if (this.AheadOfServerTime > 0)
                    {
                        return (NetClockTester.ValidityFlags.AheadOfServerTime | NetClockTester.ValidityFlags.TooFrequent);
                    }
                    return NetClockTester.ValidityFlags.TooFrequent;
                }
                if (this.OverTimed > 0)
                {
                    if (this.AheadOfServerTime > 0)
                    {
                        return (NetClockTester.ValidityFlags.AheadOfServerTime | NetClockTester.ValidityFlags.OverTimed);
                    }
                    return NetClockTester.ValidityFlags.OverTimed;
                }
                if (this.AheadOfServerTime > 0)
                {
                    return NetClockTester.ValidityFlags.AheadOfServerTime;
                }
                if (this.Valid > 0)
                {
                    return NetClockTester.ValidityFlags.Valid;
                }
                return 0;
            }
        }
        public void Add(NetClockTester.ValidityFlags vf)
        {
            switch ((vf & (NetClockTester.ValidityFlags.AheadOfServerTime | NetClockTester.ValidityFlags.OverTimed | NetClockTester.ValidityFlags.TooFrequent)))
            {
                case 0:
                    if ((vf & NetClockTester.ValidityFlags.Valid) == NetClockTester.ValidityFlags.Valid)
                    {
                        this.Valid++;
                    }
                    break;

                case NetClockTester.ValidityFlags.TooFrequent:
                    this.TooFrequent++;
                    break;

                case NetClockTester.ValidityFlags.OverTimed:
                    this.OverTimed++;
                    break;

                case (NetClockTester.ValidityFlags.OverTimed | NetClockTester.ValidityFlags.TooFrequent):
                    this.OverTimed++;
                    this.TooFrequent++;
                    break;

                case NetClockTester.ValidityFlags.AheadOfServerTime:
                    this.AheadOfServerTime++;
                    break;

                case (NetClockTester.ValidityFlags.AheadOfServerTime | NetClockTester.ValidityFlags.TooFrequent):
                    this.AheadOfServerTime++;
                    this.TooFrequent++;
                    break;

                case (NetClockTester.ValidityFlags.AheadOfServerTime | NetClockTester.ValidityFlags.OverTimed):
                    this.AheadOfServerTime++;
                    this.OverTimed++;
                    break;

                case (NetClockTester.ValidityFlags.AheadOfServerTime | NetClockTester.ValidityFlags.OverTimed | NetClockTester.ValidityFlags.TooFrequent):
                    this.AheadOfServerTime++;
                    this.OverTimed++;
                    this.TooFrequent++;
                    break;
            }
        }
    }

    [Flags]
    public enum ValidityFlags
    {
        AheadOfServerTime = 8,
        OverTimed = 4,
        TooFrequent = 2,
        Valid = 1
    }
}

