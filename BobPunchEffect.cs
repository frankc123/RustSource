using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class BobPunchEffect : BobEffect
{
    [SerializeField]
    private AnimationCurve _pitch;
    [SerializeField]
    private AnimationCurve _roll;
    [SerializeField]
    private AnimationCurve _x;
    [SerializeField]
    private AnimationCurve _y;
    [SerializeField]
    private AnimationCurve _yaw;
    [SerializeField]
    private AnimationCurve _z;
    private CurveInfo glob;
    private CurveInfo pitch;
    private CurveInfo roll;
    private CurveInfo x;
    private CurveInfo y;
    private CurveInfo yaw;
    private CurveInfo z;

    protected override void CloseData(BobEffect.Data data)
    {
    }

    protected override void InitializeNonSerializedData()
    {
        this.x = new CurveInfo(this._x);
        this.y = new CurveInfo(this._y);
        this.z = new CurveInfo(this._z);
        this.yaw = new CurveInfo(this._yaw);
        this.pitch = new CurveInfo(this._pitch);
        this.roll = new CurveInfo(this._roll);
        this.glob.valid = (((this.x.valid || this.y.valid) || (this.z.valid || this.yaw.valid)) || this.pitch.valid) || this.roll.valid;
        this.glob.constant = this.glob.valid && (((((!this.x.valid || this.x.constant) && (!this.y.valid || this.y.constant)) && ((!this.z.valid || this.z.constant) && (!this.yaw.valid || this.yaw.constant))) && (!this.pitch.valid || this.pitch.constant)) && (!this.roll.valid || this.roll.constant));
        if (this.glob.constant)
        {
            this.glob.valid = false;
            this.glob.startTime = 0f;
            this.glob.endTime = 0f;
            this.glob.duration = 0f;
        }
        else
        {
            this.glob.startTime = float.PositiveInfinity;
            this.glob.endTime = float.NegativeInfinity;
            if (this.x.valid && !this.x.constant)
            {
                if (this.x.startTime < this.glob.startTime)
                {
                    this.glob.startTime = this.x.startTime;
                }
                if (this.x.endTime > this.glob.endTime)
                {
                    this.glob.endTime = this.x.endTime;
                }
            }
            if (this.z.valid && !this.z.constant)
            {
                if (this.z.startTime < this.glob.startTime)
                {
                    this.glob.startTime = this.z.startTime;
                }
                if (this.z.endTime > this.glob.endTime)
                {
                    this.glob.endTime = this.z.endTime;
                }
            }
            if (this.yaw.valid && !this.yaw.constant)
            {
                if (this.yaw.startTime < this.glob.startTime)
                {
                    this.glob.startTime = this.yaw.startTime;
                }
                if (this.yaw.endTime > this.glob.endTime)
                {
                    this.glob.endTime = this.yaw.endTime;
                }
            }
            if (this.pitch.valid && !this.pitch.constant)
            {
                if (this.pitch.startTime < this.glob.startTime)
                {
                    this.glob.startTime = this.pitch.startTime;
                }
                if (this.pitch.endTime > this.glob.endTime)
                {
                    this.glob.endTime = this.pitch.endTime;
                }
            }
            if (this.roll.valid && !this.roll.constant)
            {
                if (this.roll.startTime < this.glob.startTime)
                {
                    this.glob.startTime = this.roll.startTime;
                }
                if (this.roll.endTime > this.glob.endTime)
                {
                    this.glob.endTime = this.roll.endTime;
                }
            }
            if (this.roll.valid && !this.roll.constant)
            {
                if (this.roll.startTime < this.glob.startTime)
                {
                    this.glob.startTime = this.roll.startTime;
                }
                if (this.roll.endTime > this.glob.endTime)
                {
                    this.glob.endTime = this.roll.endTime;
                }
            }
            if (this.glob.startTime == float.PositiveInfinity)
            {
                this.glob.startTime = 0f;
                this.glob.endTime = 0f;
                this.glob.duration = 0f;
                this.glob.valid = false;
            }
            else
            {
                this.glob.duration = this.glob.endTime - this.glob.startTime;
            }
        }
    }

    protected override bool OpenData(out BobEffect.Data data)
    {
        if (!this.glob.valid)
        {
            data = null;
            return false;
        }
        data = new PunchData();
        data.effect = this;
        return true;
    }

    protected override BOBRES SimulateData(ref BobEffect.Context ctx)
    {
        if (ctx.dt != 0.0)
        {
            PunchData data = (PunchData) ctx.data;
            if (data.time >= this.glob.endTime)
            {
                return BOBRES.EXIT;
            }
            if (data.time >= this.glob.endTime)
            {
                return BOBRES.EXIT;
            }
            if (this.x.valid)
            {
                if (this.x.constant || (data.time <= this.x.startTime))
                {
                    data.force.x = this.x.startValue;
                }
                else if (data.time >= this.x.endValue)
                {
                    data.force.x = this.x.endValue;
                }
                else
                {
                    data.force.x = this.x.curve.Evaluate(data.time);
                }
            }
            if (this.y.valid)
            {
                if (this.y.constant || (data.time <= this.y.startTime))
                {
                    data.force.y = this.y.startValue;
                }
                else if (data.time >= this.y.endValue)
                {
                    data.force.y = this.y.endValue;
                }
                else
                {
                    data.force.y = this.y.curve.Evaluate(data.time);
                }
            }
            if (this.z.valid)
            {
                if (this.z.constant || (data.time <= this.z.startTime))
                {
                    data.force.z = this.z.startValue;
                }
                else if (data.time >= this.z.endValue)
                {
                    data.force.z = this.z.endValue;
                }
                else
                {
                    data.force.z = this.z.curve.Evaluate(data.time);
                }
            }
            if (this.pitch.valid)
            {
                if (this.pitch.constant || (data.time <= this.pitch.startTime))
                {
                    data.torque.x = this.pitch.startValue;
                }
                else if (data.time >= this.pitch.endValue)
                {
                    data.torque.x = this.pitch.endValue;
                }
                else
                {
                    data.torque.x = this.pitch.curve.Evaluate(data.time);
                }
            }
            if (this.yaw.valid)
            {
                if (this.yaw.constant || (data.time <= this.yaw.startTime))
                {
                    data.torque.y = this.yaw.startValue;
                }
                else if (data.time >= this.yaw.endValue)
                {
                    data.torque.y = this.yaw.endValue;
                }
                else
                {
                    data.torque.y = this.yaw.curve.Evaluate(data.time);
                }
            }
            if (this.roll.valid)
            {
                if (this.roll.constant || (data.time <= this.roll.startTime))
                {
                    data.torque.z = this.roll.startValue;
                }
                else if (data.time >= this.roll.endValue)
                {
                    data.torque.z = this.roll.endValue;
                }
                else
                {
                    data.torque.z = this.roll.curve.Evaluate(data.time);
                }
            }
            data.time += (float) ctx.dt;
        }
        return BOBRES.CONTINUE;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CurveInfo
    {
        public AnimationCurve curve;
        public float endTime;
        public float startTime;
        public float startValue;
        public float endValue;
        public float duration;
        public float min;
        public float max;
        public float range;
        public int length;
        public bool valid;
        public bool constant;
        public CurveInfo(AnimationCurve curve)
        {
            if (curve == null)
            {
                this = new BobPunchEffect.CurveInfo();
            }
            else
            {
                this.curve = curve;
            }
            this.length = curve.length;
            switch (this.length)
            {
                case 0:
                    this.endTime = 0f;
                    this.startTime = 0f;
                    this.duration = 0f;
                    this.min = 0f;
                    this.max = 0f;
                    this.range = 0f;
                    this.startValue = 0f;
                    this.endValue = 0f;
                    this.valid = false;
                    this.constant = false;
                    return;

                case 1:
                {
                    Keyframe keyframe = curve[0];
                    this.startTime = keyframe.time;
                    this.endTime = this.startTime;
                    this.duration = 0f;
                    Keyframe keyframe2 = curve[0];
                    this.min = keyframe2.value;
                    this.max = this.min;
                    this.startValue = this.min;
                    this.endValue = this.min;
                    this.range = 0f;
                    this.valid = true;
                    this.constant = true;
                    return;
                }
                case 2:
                {
                    Keyframe keyframe3 = curve[0];
                    this.startTime = keyframe3.time;
                    Keyframe keyframe4 = curve[1];
                    this.endTime = keyframe4.time;
                    this.duration = this.endTime - this.startTime;
                    Keyframe keyframe5 = curve[0];
                    this.startValue = keyframe5.value;
                    Keyframe keyframe6 = curve[1];
                    this.endValue = keyframe6.value;
                    if (this.endValue >= this.startValue)
                    {
                        this.range = this.endValue - this.startValue;
                        this.min = this.startValue;
                        this.max = this.endValue;
                        break;
                    }
                    this.range = this.startValue - this.endValue;
                    this.min = this.endValue;
                    this.max = this.startValue;
                    break;
                }
                default:
                {
                    Keyframe keyframe7 = curve[0];
                    this.startTime = keyframe7.time;
                    Keyframe keyframe8 = curve[this.length - 1];
                    this.endTime = keyframe8.time;
                    this.duration = this.endTime - this.startTime;
                    Keyframe keyframe9 = curve[0];
                    this.min = this.startValue = keyframe9.value;
                    this.max = this.min;
                    this.endValue = this.startValue;
                    for (int i = 1; i < this.length; i++)
                    {
                        Keyframe keyframe10 = curve[i];
                        this.endValue = keyframe10.value;
                        if (this.endValue > this.max)
                        {
                            this.max = this.endValue;
                        }
                        if (this.endValue < this.min)
                        {
                            this.min = this.endValue;
                        }
                    }
                    this.range = this.max - this.min;
                    this.valid = true;
                    this.constant = this.range == 0f;
                    return;
                }
            }
            this.valid = true;
            this.constant = this.range == 0f;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.startTime, this.duration, this.min, this.max, this.length, this.valid, this.constant };
            return string.Format("[CurveInfo startTime={0}, duration={1}, min={2}, max={3}, length={4}, valid={5}, constant={6}]", args);
        }
    }

    private class PunchData : BobEffect.Data
    {
        public float time;

        public override void CopyDataTo(BobEffect.Data data)
        {
            base.CopyDataTo(data);
            ((BobPunchEffect.PunchData) data).time = this.time;
        }
    }
}

