namespace Facepunch.Procedural
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct Direction
    {
        [NonSerialized]
        public Integrated<Vector3> value;
        public void Target(ref Vector3 target, float degreeSpeed)
        {
            if (!this.value.clock.once)
            {
                this.value.SetImmediate(ref target);
            }
            else
            {
                float num = Mathf.Abs(Vector3.Angle(this.value.current, target));
                if ((num < (degreeSpeed * float.Epsilon)) || (degreeSpeed == 0f))
                {
                    this.value.SetImmediate(ref target);
                }
                else
                {
                    this.value.begin = this.value.current;
                    this.value.end = target;
                    if (degreeSpeed < 0f)
                    {
                        degreeSpeed = -degreeSpeed;
                    }
                    this.value.clock.duration = this.value.clock.remain = (ulong) Math.Ceiling((double) ((num * 1000.0) / ((double) degreeSpeed)));
                    if (this.value.clock.remain <= 1L)
                    {
                        this.value.SetImmediate(ref target);
                    }
                }
            }
        }

        public Integration Advance(ulong millis)
        {
            Integration integration = this.value.clock.IntegrateTime(millis);
            Integration integration2 = integration;
            if (integration2 != Integration.Moved)
            {
                if (integration2 != (Integration.Moved | Integration.Stationary))
                {
                    return integration;
                }
            }
            else
            {
                this.value.current = Vector3.Slerp(this.value.begin, this.value.end, this.value.clock.percentf);
                return integration;
            }
            this.value.current = this.value.end;
            return integration;
        }
    }
}

