namespace Facepunch.Clocks.Counters.uLink
{
    using System;
    using System.Runtime.InteropServices;
    using uLink;

    [StructLayout(LayoutKind.Sequential)]
    public struct LocalTime
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
        public static LocalTime Restart
        {
            get
            {
                LocalTime time;
                time.deductSeconds = 0.0;
                time.endTime = double.PositiveInfinity;
                time.startTime = TIME_SOURCE.NOW;
                return time;
            }
        }
        public static LocalTime Reset
        {
            get
            {
                LocalTime time;
                time.deductSeconds = 0.0;
                time.endTime = double.PositiveInfinity;
                time.startTime = double.NegativeInfinity;
                return time;
            }
        }
        private static class TIME_SOURCE
        {
            public static double NOW
            {
                get
                {
                    return Network.localTime;
                }
            }
        }
    }
}

