namespace Facepunch.Clocks.Counters
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTimestamp
    {
        private const double ZeroDeductions = 0.0;
        private const double OneThousand = 1000.0;
        private const double ZeroElapsed = 0.0;
        private double startTime;
        private double endTime;
        private double deductSeconds;
        public void Start()
        {
            if (double.IsNegativeInfinity(this.startTime))
            {
                this.startTime = TIME_SOURCE.NOW;
                this.deductSeconds = 0.0;
                this.endTime = double.PositiveInfinity;
            }
            else if (!double.IsPositiveInfinity(this.endTime))
            {
                double endTime = this.endTime;
                this.endTime = double.PositiveInfinity;
                this.deductSeconds += TIME_SOURCE.NOW - endTime;
            }
        }

        public void Stop()
        {
            if (!double.IsNegativeInfinity(this.startTime) && double.IsPositiveInfinity(this.endTime))
            {
                this.endTime = TIME_SOURCE.NOW;
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                if (double.IsNegativeInfinity(this.startTime))
                {
                    return 0.0;
                }
                if (double.IsPositiveInfinity(this.endTime))
                {
                    return ((TIME_SOURCE.NOW - this.deductSeconds) - this.startTime);
                }
                return ((this.endTime - this.deductSeconds) - this.startTime);
            }
        }
        public long ElapsedMilliseconds
        {
            get
            {
                return (long) Math.Floor((double) (this.ElapsedSeconds * 1000.0));
            }
        }
        public TimeSpan Elapsed
        {
            get
            {
                if (double.IsNegativeInfinity(this.startTime))
                {
                    return TimeSpan.Zero;
                }
                return TimeSpan.FromSeconds(((!double.IsPositiveInfinity(this.endTime) ? this.endTime : TIME_SOURCE.NOW) - this.deductSeconds) - this.startTime);
            }
        }
        public bool IsRunning
        {
            get
            {
                return (double.IsPositiveInfinity(this.endTime) && !double.IsNegativeInfinity(this.startTime));
            }
        }
        public static SystemTimestamp Restart
        {
            get
            {
                SystemTimestamp timestamp;
                timestamp.deductSeconds = 0.0;
                timestamp.endTime = double.PositiveInfinity;
                timestamp.startTime = TIME_SOURCE.NOW;
                return timestamp;
            }
        }
        public static SystemTimestamp Reset
        {
            get
            {
                SystemTimestamp timestamp;
                timestamp.deductSeconds = 0.0;
                timestamp.endTime = double.PositiveInfinity;
                timestamp.startTime = double.NegativeInfinity;
                return timestamp;
            }
        }
        private static class TIME_SOURCE
        {
            private static readonly long Frequency = Stopwatch.Frequency;
            private static readonly bool IsHighResolution = Stopwatch.IsHighResolution;
            private static readonly long ThenTimestamp = Stopwatch.GetTimestamp();
            private static readonly double ToSeconds = ((double) (1M / Frequency));

            static TIME_SOURCE()
            {
                string message = string.Format("SystemTimestampWatch settings={{IsHighResolution={0},Frequency={1},ToSecond={2}}}", IsHighResolution, Frequency, ToSeconds);
                if (!IsHighResolution)
                {
                    Debug.LogWarning(message);
                }
            }

            public static double NOW
            {
                get
                {
                    return ((Stopwatch.GetTimestamp() - ThenTimestamp) * ToSeconds);
                }
            }
        }
    }
}

