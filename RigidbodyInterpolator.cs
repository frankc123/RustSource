using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class RigidbodyInterpolator : StateInterpolator<PosRot>, IStateInterpolatorWithLinearVelocity, IStateInterpolator<PosRot>, IStateInterpolatorSampler<PosRot>
{
    public float allowDifference = 0.1f;
    public bool exterpolate;
    public Rigidbody target;

    void IStateInterpolator<PosRot>.SetGoals(ref TimeStamped<PosRot> sample)
    {
        base.SetGoals(ref sample);
    }

    public bool Sample(ref double time, out PosRot result)
    {
        int num;
        double timeStamp;
        switch (base.len)
        {
            case 0:
                result = new PosRot();
                return false;

            case 1:
                num = base.tbuffer[0].index;
                timeStamp = base.tbuffer[num].timeStamp;
                result = base.tbuffer[num].value;
                return true;
        }
        int index = 0;
        int num4 = -1;
        do
        {
            num = base.tbuffer[index].index;
            timeStamp = base.tbuffer[num].timeStamp;
            if (timeStamp > time)
            {
                num4 = num;
            }
            else
            {
                if (timeStamp == time)
                {
                    result = base.tbuffer[num].value;
                    return true;
                }
                if (timeStamp < time)
                {
                    if (num4 == -1)
                    {
                        if (this.exterpolate && (index < (base.len - 1)))
                        {
                            num4 = num;
                            num = base.tbuffer[index + 1].index;
                            double t = (time - base.tbuffer[num].timeStamp) / (base.tbuffer[num4].timeStamp - base.tbuffer[num].timeStamp);
                            PosRot.Lerp(ref base.tbuffer[num].value, ref base.tbuffer[num4].value, t, out result);
                        }
                        else
                        {
                            result = base.tbuffer[num].value;
                        }
                    }
                    else
                    {
                        double num6 = base.tbuffer[num4].timeStamp;
                        double num7 = this.allowDifference + NetCull.sendInterval;
                        double num8 = num6 - timeStamp;
                        if (num8 > num7)
                        {
                            timeStamp = num6 - (num8 = num7);
                            if (timeStamp >= time)
                            {
                                result = base.tbuffer[num].value;
                                return true;
                            }
                        }
                        double num9 = (time - timeStamp) / num8;
                        PosRot.Lerp(ref base.tbuffer[num].value, ref base.tbuffer[num4].value, num9, out result);
                    }
                    return true;
                }
            }
        }
        while (++index < base.len);
        result = base.tbuffer[base.tbuffer[base.len - 1].index].value;
        return true;
    }

    public bool SampleWorldVelocity(out Vector3 worldLinearVelocity)
    {
        return this.SampleWorldVelocity(Interpolation.time, out worldLinearVelocity);
    }

    public bool SampleWorldVelocity(double time, out Vector3 worldLinearVelocity)
    {
        switch (base.len)
        {
            case 0:
            case 1:
                worldLinearVelocity = new Vector3();
                return false;
        }
        int index = 0;
        int num4 = -1;
        do
        {
            int num = base.tbuffer[index].index;
            double timeStamp = base.tbuffer[num].timeStamp;
            if (timeStamp > time)
            {
                num4 = num;
            }
            else
            {
                if (num4 == -1)
                {
                    worldLinearVelocity = new Vector3();
                    return false;
                }
                double num5 = base.tbuffer[num4].timeStamp;
                double num6 = this.allowDifference + NetCull.sendInterval;
                double num7 = num5 - timeStamp;
                if (num7 >= num6)
                {
                    num7 = num6;
                    timeStamp = num5 - num7;
                    if (time <= timeStamp)
                    {
                        worldLinearVelocity = new Vector3();
                        return false;
                    }
                }
                worldLinearVelocity = base.tbuffer[num4].value.position - base.tbuffer[num].value.position;
                worldLinearVelocity.x = (float) (((double) worldLinearVelocity.x) / num7);
                worldLinearVelocity.y = (float) (((double) worldLinearVelocity.y) / num7);
                worldLinearVelocity.z = (float) (((double) worldLinearVelocity.z) / num7);
                return true;
            }
        }
        while (++index < base.len);
        worldLinearVelocity = new Vector3();
        return false;
    }

    public void SetGoals(PosRot frame, double timestamp)
    {
        this.SetGoals(ref frame, ref timestamp);
    }

    public sealed override void SetGoals(Vector3 pos, Quaternion rot, double timestamp)
    {
        PosRot rot2;
        rot2.position = pos;
        rot2.rotation = rot;
        this.SetGoals(ref rot2, ref timestamp);
    }

    protected override void Syncronize()
    {
        PosRot rot;
        double time = Interpolation.time;
        if (this.Sample(ref time, out rot))
        {
            this.target.MovePosition(rot.position);
            this.target.MoveRotation(rot.rotation);
        }
    }
}

