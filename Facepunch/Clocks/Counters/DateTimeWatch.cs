namespace Facepunch.Clocks.Counters
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DateTimeWatch
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
        public static DateTimeWatch Restart
        {
            get
            {
                DateTimeWatch watch;
                watch.deductSeconds = 0.0;
                watch.endTime = double.PositiveInfinity;
                watch.startTime = TIME_SOURCE.NOW;
                return watch;
            }
        }
        public static DateTimeWatch Reset
        {
            get
            {
                DateTimeWatch watch;
                watch.deductSeconds = 0.0;
                watch.endTime = double.PositiveInfinity;
                watch.startTime = double.NegativeInfinity;
                return watch;
            }
        }
        private static class TIME_SOURCE
        {
            [DecimalConstant(0x1c, 0, (uint) 0x36, (uint) 0x35c9adc5, (uint) 0xdea00000)]
            private static readonly decimal kTickToSecond = 0.0000001000000000000000000000M;
            public static readonly DateTime Then = DateTime.Now;
            public static readonly long ThenTicks = Then.Ticks;

            public static double NOW
            {
                get
                {
                    return (double) ((DateTime.Now.Ticks - ThenTicks) * 0.0000001000000000000000000000M);
                }
            }
        }
    }
}

