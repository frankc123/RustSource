using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class CharacterTransformInterpolator : CharacterInterpolatorBase<CharacterTransformInterpolatorData>, IStateInterpolatorSampler<CharacterTransformInterpolatorData>, IStateInterpolator<CharacterTransformInterpolatorData>
{
    private bool once;

    void IStateInterpolator<CharacterTransformInterpolatorData>.SetGoals(ref TimeStamped<CharacterTransformInterpolatorData> sample)
    {
        base.SetGoals(ref sample);
    }

    void IStateInterpolator<CharacterTransformInterpolatorData>.SetGoals(ref CharacterTransformInterpolatorData sample, ref double timeStamp)
    {
        base.SetGoals(ref sample, ref timeStamp);
    }

    public bool Sample(ref double time, out CharacterTransformInterpolatorData result)
    {
        int index;
        double timeStamp;
        int num3;
        int num4;
        switch (base.len)
        {
            case 0:
                result = new CharacterTransformInterpolatorData();
                return false;

            case 1:
                index = base.tbuffer[0].index;
                timeStamp = base.tbuffer[index].timeStamp;
                result = base.tbuffer[index].value;
                return true;

            default:
                num3 = 0;
                num4 = -1;
                break;
        }
    Label_006F:
        index = base.tbuffer[num3].index;
        timeStamp = base.tbuffer[index].timeStamp;
        if (timeStamp > time)
        {
            num4 = index;
            goto Label_0613;
        }
        if (timeStamp == time)
        {
            result = base.tbuffer[index].value;
            return true;
        }
        if (timeStamp >= time)
        {
            goto Label_0613;
        }
        if (num4 == -1)
        {
            if (base.extrapolate && (num3 < (base.len - 1)))
            {
                num4 = index;
                index = base.tbuffer[num3 + 1].index;
                double num5 = (timeStamp - base.tbuffer[index].timeStamp) / (timeStamp - base.tbuffer[index].timeStamp);
                switch (num5)
                {
                    case 0.0:
                        result = base.tbuffer[index].value;
                        goto Label_0611;

                    case 1.0:
                        result = base.tbuffer[num4].value;
                        goto Label_0611;
                }
                double num6 = 1.0 - num5;
                result.origin.x = (float) ((base.tbuffer[index].value.origin.x * num6) + (base.tbuffer[num4].value.origin.x * num5));
                result.origin.y = (float) ((base.tbuffer[index].value.origin.y * num6) + (base.tbuffer[num4].value.origin.y * num5));
                result.origin.z = (float) ((base.tbuffer[index].value.origin.z * num6) + (base.tbuffer[num4].value.origin.z * num5));
                result.eyesAngles = new Angle2();
                result.eyesAngles.yaw = base.tbuffer[index].value.eyesAngles.yaw + ((float) (Mathf.DeltaAngle(base.tbuffer[index].value.eyesAngles.yaw, base.tbuffer[num4].value.eyesAngles.yaw) * num5));
                result.eyesAngles.pitch = Mathf.DeltaAngle(0f, base.tbuffer[index].value.eyesAngles.pitch + ((float) (Mathf.DeltaAngle(base.tbuffer[index].value.eyesAngles.pitch, base.tbuffer[num4].value.eyesAngles.pitch) * num5)));
            }
            else
            {
                result = base.tbuffer[index].value;
            }
        }
        else
        {
            double num7 = base.tbuffer[num4].timeStamp;
            double num8 = base.allowableTimeSpan + NetCull.sendInterval;
            double num9 = num7 - timeStamp;
            if (num9 > num8)
            {
                timeStamp = num7 - (num9 = num8);
                if (timeStamp >= time)
                {
                    result = base.tbuffer[index].value;
                    return true;
                }
            }
            double num10 = (time - timeStamp) / num9;
            switch (num10)
            {
                case 0.0:
                    result = base.tbuffer[index].value;
                    goto Label_0611;

                case 1.0:
                    result = base.tbuffer[num4].value;
                    goto Label_0611;
            }
            double num11 = 1.0 - num10;
            result.origin.x = (float) ((base.tbuffer[index].value.origin.x * num11) + (base.tbuffer[num4].value.origin.x * num10));
            result.origin.y = (float) ((base.tbuffer[index].value.origin.y * num11) + (base.tbuffer[num4].value.origin.y * num10));
            result.origin.z = (float) ((base.tbuffer[index].value.origin.z * num11) + (base.tbuffer[num4].value.origin.z * num10));
            result.eyesAngles = new Angle2();
            result.eyesAngles.yaw = base.tbuffer[index].value.eyesAngles.yaw + ((float) (Mathf.DeltaAngle(base.tbuffer[index].value.eyesAngles.yaw, base.tbuffer[num4].value.eyesAngles.yaw) * num10));
            result.eyesAngles.pitch = Mathf.DeltaAngle(0f, base.tbuffer[index].value.eyesAngles.pitch + ((float) (Mathf.DeltaAngle(base.tbuffer[index].value.eyesAngles.pitch, base.tbuffer[num4].value.eyesAngles.pitch) * num10)));
        }
    Label_0611:
        return true;
    Label_0613:
        if (++num3 < base.len)
        {
            goto Label_006F;
        }
        result = base.tbuffer[base.tbuffer[base.len - 1].index].value;
        return true;
    }

    public bool SampleWorldVelocity(out Angle2 worldAngularVelocity)
    {
        return this.SampleWorldVelocity(Interpolation.time, out worldAngularVelocity);
    }

    public bool SampleWorldVelocity(out Vector3 worldLinearVelocity)
    {
        return this.SampleWorldVelocity(Interpolation.time, out worldLinearVelocity);
    }

    public bool SampleWorldVelocity(out Vector3 worldLinearVelocity, out Angle2 worldAngularVelocity)
    {
        return this.SampleWorldVelocity(Interpolation.time, out worldLinearVelocity, out worldAngularVelocity);
    }

    public bool SampleWorldVelocity(double time, out Angle2 worldAngularVelocity)
    {
        switch (base.len)
        {
            case 0:
            case 1:
                worldAngularVelocity = new Angle2();
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
                    worldAngularVelocity = new Angle2();
                    return false;
                }
                double num5 = base.tbuffer[num4].timeStamp;
                double num6 = base.allowableTimeSpan + NetCull.sendInterval;
                double num7 = num5 - timeStamp;
                if (num7 >= num6)
                {
                    num7 = num6;
                    timeStamp = num5 - num7;
                    if (time <= timeStamp)
                    {
                        worldAngularVelocity = new Angle2();
                        return false;
                    }
                }
                worldAngularVelocity = Angle2.Delta(base.tbuffer[num].value.eyesAngles, base.tbuffer[num4].value.eyesAngles);
                worldAngularVelocity.x = (float) (((double) worldAngularVelocity.x) / num7);
                worldAngularVelocity.y = (float) (((double) worldAngularVelocity.y) / num7);
                return true;
            }
        }
        while (++index < base.len);
        worldAngularVelocity = new Angle2();
        return false;
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
                double num6 = base.allowableTimeSpan + NetCull.sendInterval;
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
                worldLinearVelocity = base.tbuffer[num4].value.origin - base.tbuffer[num].value.origin;
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

    public bool SampleWorldVelocity(double time, out Vector3 worldLinearVelocity, out Angle2 worldAngularVelocity)
    {
        switch (base.len)
        {
            case 0:
            case 1:
                worldLinearVelocity = new Vector3();
                worldAngularVelocity = new Angle2();
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
                    worldAngularVelocity = new Angle2();
                    return false;
                }
                double num5 = base.tbuffer[num4].timeStamp;
                double num6 = base.allowableTimeSpan + NetCull.sendInterval;
                double num7 = num5 - timeStamp;
                if (num7 >= num6)
                {
                    num7 = num6;
                    timeStamp = num5 - num7;
                    if (time <= timeStamp)
                    {
                        worldLinearVelocity = new Vector3();
                        worldAngularVelocity = new Angle2();
                        return false;
                    }
                }
                worldLinearVelocity = base.tbuffer[num4].value.origin - base.tbuffer[num].value.origin;
                worldAngularVelocity = Angle2.Delta(base.tbuffer[num].value.eyesAngles, base.tbuffer[num4].value.eyesAngles);
                worldLinearVelocity.x = (float) (((double) worldLinearVelocity.x) / num7);
                worldLinearVelocity.y = (float) (((double) worldLinearVelocity.y) / num7);
                worldLinearVelocity.z = (float) (((double) worldLinearVelocity.z) / num7);
                worldAngularVelocity.x = (float) (((double) worldAngularVelocity.x) / num7);
                worldAngularVelocity.y = (float) (((double) worldAngularVelocity.y) / num7);
                return true;
            }
        }
        while (++index < base.len);
        worldLinearVelocity = new Vector3();
        worldAngularVelocity = new Angle2();
        return false;
    }

    public void SetGoals(Vector3 pos, Angle2 rot, double timestamp)
    {
        CharacterTransformInterpolatorData data;
        data.origin = pos;
        data.eyesAngles = rot;
        base.SetGoals(ref data, ref timestamp);
    }

    public sealed override void SetGoals(Vector3 pos, Quaternion rot, double timestamp)
    {
        this.SetGoals(pos, (Angle2) rot, timestamp);
    }

    private void Update()
    {
        CharacterTransformInterpolatorData data;
        double time = Interpolation.time;
        if (this.Sample(ref time, out data))
        {
            Character idMain = base.idMain;
            if (idMain != null)
            {
                idMain.origin = data.origin;
                idMain.eyesAngles = data.eyesAngles;
            }
        }
    }
}

