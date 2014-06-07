namespace Facepunch.Clocks.Counters.Unity
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct RealtimeSinceStartup
    {
        private const double ZeroDeductions = 0.0;
        private const double OneThousand = 1000.0;
        private const double ZeroElapsed = 0.0;
        private float startTime;
        private float endTime;
        private double deductSeconds;
        public void Start()
        {
            if (float.IsNegativeInfinity(this.startTime))
            {
                this.startTime = TIME_SOURCE.NOW;
                this.deductSeconds = 0.0;
                this.endTime = float.PositiveInfinity;
            }
            else if (!float.IsPositiveInfinity(this.endTime))
            {
                float endTime = this.endTime;
                this.endTime = float.PositiveInfinity;
                this.deductSeconds += TIME_SOURCE.NOW - endTime;
            }
        }

        public void Stop()
        {
            if (!float.IsNegativeInfinity(this.startTime) && float.IsPositiveInfinity(this.endTime))
            {
                this.endTime = TIME_SOURCE.NOW;
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                if (float.IsNegativeInfinity(this.startTime))
                {
                    return 0.0;
                }
                if (float.IsPositiveInfinity(this.endTime))
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
                if (float.IsNegativeInfinity(this.startTime))
                {
                    return TimeSpan.Zero;
                }
                return TimeSpan.FromSeconds(((!float.IsPositiveInfinity(this.endTime) ? ((double) this.endTime) : ((double) TIME_SOURCE.NOW)) - this.deductSeconds) - this.startTime);
            }
        }
        public bool IsRunning
        {
            get
            {
                return (float.IsPositiveInfinity(this.endTime) && !float.IsNegativeInfinity(this.startTime));
            }
        }
        public static RealtimeSinceStartup Restart
        {
            get
            {
                RealtimeSinceStartup startup;
                startup.deductSeconds = 0.0;
                startup.endTime = float.PositiveInfinity;
                startup.startTime = TIME_SOURCE.NOW;
                return startup;
            }
        }
        public static RealtimeSinceStartup Reset
        {
            get
            {
                RealtimeSinceStartup startup;
                startup.deductSeconds = 0.0;
                startup.endTime = float.PositiveInfinity;
                startup.startTime = float.NegativeInfinity;
                return startup;
            }
        }
        private static class TIME_SOURCE
        {
            public static float NOW
            {
                get
                {
                    return Time.realtimeSinceStartup;
                }
            }
        }
    }
}

