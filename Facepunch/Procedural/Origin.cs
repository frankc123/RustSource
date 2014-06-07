namespace Facepunch.Procedural
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct Origin
    {
        [NonSerialized]
        public Integrated<Vector3> value;
        [NonSerialized]
        public Vector3 delta;
        public void Target(ref Vector3 target, float moveSpeed)
        {
            if (!this.value.clock.once)
            {
                this.delta.x = this.delta.y = this.delta.z = 0f;
                this.value.SetImmediate(ref target);
            }
            else
            {
                this.delta.x = target.x - this.value.current.x;
                this.delta.y = target.y - this.value.current.y;
                this.delta.z = target.z - this.value.current.z;
                float f = ((this.delta.x * this.delta.x) + (this.delta.y * this.delta.y)) + (this.delta.z * this.delta.z);
                float num2 = moveSpeed * float.Epsilon;
                float num3 = num2 * num2;
                if ((f <= num3) || (moveSpeed == 0f))
                {
                    this.delta.x = this.delta.y = this.delta.z = 0f;
                    this.value.SetImmediate(ref target);
                }
                else
                {
                    float num4 = Mathf.Sqrt(f);
                    this.value.begin = this.value.current;
                    this.value.end = target;
                    if (moveSpeed < 0f)
                    {
                        moveSpeed = -moveSpeed;
                    }
                    this.value.clock.remain = this.value.clock.duration = (ulong) Math.Ceiling((double) ((num4 * 1000.0) / ((double) moveSpeed)));
                    if (this.value.clock.remain <= 1L)
                    {
                        this.delta.x = this.delta.y = this.delta.z = 0f;
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
                double percent = this.value.clock.percent;
                this.value.current.x = this.value.begin.x + ((float) (this.delta.x * percent));
                this.value.current.y = this.value.begin.y + ((float) (this.delta.y * percent));
                this.value.current.z = this.value.begin.z + ((float) (this.delta.z * percent));
                return integration;
            }
            this.value.current = this.value.end;
            return integration;
        }
    }
}

