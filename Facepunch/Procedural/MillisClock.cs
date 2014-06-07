namespace Facepunch.Procedural
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct MillisClock
    {
        [NonSerialized]
        public ulong remain;
        [NonSerialized]
        public ulong duration;
        [NonSerialized]
        public bool once;
        public ClockStatus clockStatus
        {
            get
            {
                return (!this.once ? ClockStatus.Unset : ((this.remain != 0) ? ((this.remain >= this.duration) ? ClockStatus.Negative : (ClockStatus.Unset | ClockStatus.WillElapse)) : ((this.duration != 0) ? ClockStatus.Elapsed : ClockStatus.Unset)));
            }
        }
        public ClockStatus ResetRandomDurationSeconds(double secondsMin, double secondsMax)
        {
            return this.ResetDurationSeconds(secondsMin + (Random.value * (secondsMax - secondsMin)));
        }

        public ClockStatus ResetDurationSeconds(double seconds)
        {
            return this.ResetDurationMillis((ulong) Math.Ceiling((double) (seconds * 1000.0)));
        }

        public ClockStatus ResetDurationMillis(ulong millis)
        {
            if (millis <= 1L)
            {
                this.SetImmediate();
                return ClockStatus.DidElapse;
            }
            this.once = true;
            this.remain = this.duration = millis;
            return (ClockStatus.Unset | ClockStatus.WillElapse);
        }

        public float percentf
        {
            get
            {
                return ((this.remain != 0) ? ((this.remain < this.duration) ? ((float) (1.0 - (((double) this.remain) / ((double) this.duration)))) : 0f) : 1f);
            }
        }
        public double percent
        {
            get
            {
                return ((this.remain != 0) ? ((this.remain < this.duration) ? (1.0 - (((double) this.remain) / ((double) this.duration))) : 0.0) : 1.0);
            }
        }
        public void SetImmediate()
        {
            this.once = true;
            this.remain = 1L;
            this.duration = 2L;
        }

        public bool IntegrateTime_Reached(ulong millis)
        {
            return (((byte) (this.IntegrateTime(millis) & Integration.Stationary)) == 1);
        }

        public Integration IntegrateTime(ulong millis)
        {
            if ((!this.once || (this.remain == 0)) || ((this.duration == 0) || (millis == 0)))
            {
                return Integration.Stationary;
            }
            if (this.remain <= millis)
            {
                this.remain = 0L;
                return Integration.Stationary;
            }
            this.remain -= millis;
            if (this.remain < this.duration)
            {
                return Integration.Moved;
            }
            return (Integration.Moved | Integration.Stationary);
        }
    }
}

