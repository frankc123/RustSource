using Facepunch.Precision;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public class BobForceCurve
{
    private bool calc;
    private float duration;
    public AnimationCurve forceX;
    public AnimationCurve forceY;
    public AnimationCurve forceZ;
    private CurveInfo infoX;
    private CurveInfo infoY;
    private CurveInfo infoZ;
    private bool mask;
    private float offset;
    private bool once;
    public Vector3 outputScale = Vector3.one;
    public Vector3 positionScale = Vector3.one;
    private bool scale;
    private bool scaleFixed;
    public BobForceCurveSource source;
    public AnimationCurve sourceMask;
    public AnimationCurve sourceScale;
    public BobForceCurveTarget target;

    public void Calculate(ref Vector3G v, ref double pow, ref double dt, ref Vector3G sum)
    {
        if (!this.once)
        {
            this.Gasp();
        }
        if (this.calc)
        {
            Vector3G vectorg;
            float num = !this.mask ? 1f : this.sourceMask.Evaluate((float) pow);
            bool flag = (num == 0f) || (num == 0f);
            float num2 = !this.scaleFixed ? (!this.scale ? 1f : this.sourceScale.Evaluate((float) pow)) : 0f;
            bool flag2 = !this.scaleFixed && ((num2 != 0f) && !(num2 == 0f));
            if (this.infoX.calc)
            {
                if (flag2 && !this.infoX.constant)
                {
                    v.x += ((pow * dt) * num2) * this.positionScale.x;
                    if (v.x > this.infoX.duration)
                    {
                        v.x -= this.infoX.duration;
                    }
                    else if (v.x < -this.infoX.duration)
                    {
                        v.x += this.infoX.duration;
                    }
                }
                vectorg.x = !flag ? ((double) (this.forceX.Evaluate((float) v.x) * this.outputScale.x)) : ((double) 0f);
            }
            else
            {
                vectorg.x = 0.0;
            }
            if (this.infoY.calc)
            {
                if (flag2 && !this.infoY.constant)
                {
                    v.y += ((pow * dt) * num2) * this.positionScale.y;
                    if (v.y > this.infoY.duration)
                    {
                        v.y -= this.infoY.duration;
                    }
                    else if (v.y < -this.infoY.duration)
                    {
                        v.y += this.infoY.duration;
                    }
                }
                vectorg.y = !flag ? ((double) (this.forceY.Evaluate((float) v.y) * this.outputScale.y)) : ((double) 0f);
            }
            else
            {
                vectorg.y = 0.0;
            }
            if (this.infoZ.calc)
            {
                if (flag2 && !this.infoZ.constant)
                {
                    v.z += ((pow * dt) * num2) * this.positionScale.z;
                    if (v.z > this.infoZ.duration)
                    {
                        v.z -= this.infoZ.duration;
                    }
                    else if (v.z < -this.infoZ.duration)
                    {
                        v.z += this.infoZ.duration;
                    }
                }
                vectorg.z = !flag ? ((double) (this.forceZ.Evaluate((float) v.z) * this.outputScale.z)) : ((double) 0f);
            }
            else
            {
                vectorg.z = 0.0;
            }
            if (!flag)
            {
                sum.x += vectorg.x * num;
                sum.y += vectorg.y * num;
                sum.z += vectorg.z * num;
            }
        }
    }

    private void Gasp()
    {
        bool flag;
        bool flag2;
        bool flag3;
        this.infoX = new CurveInfo(this.forceX);
        this.infoY = new CurveInfo(this.forceY);
        this.infoZ = new CurveInfo(this.forceZ);
        this.calc = (this.infoX.calc || this.infoY.calc) || this.infoZ.calc;
        int length = this.sourceMask.length;
        if (length == 1)
        {
            Keyframe keyframe = this.sourceMask[0];
            if (keyframe.value == 1f)
            {
                flag = false;
            }
            else
            {
                Keyframe keyframe2 = this.sourceMask[0];
                if (keyframe2.value == 0f)
                {
                    this.calc = false;
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
        }
        else if (length == 0)
        {
            flag = false;
        }
        else
        {
            flag = true;
        }
        length = this.sourceScale.length;
        if (length == 1)
        {
            Keyframe keyframe3 = this.sourceScale[0];
            if (keyframe3.value == 1f)
            {
                flag2 = false;
            }
            else
            {
                flag2 = true;
            }
            Keyframe keyframe4 = this.sourceScale[0];
            if (keyframe4.value == 0f)
            {
                flag3 = true;
            }
            else
            {
                flag3 = false;
            }
        }
        else if (length == 0)
        {
            flag2 = false;
            flag3 = false;
        }
        else
        {
            flag2 = true;
            flag3 = false;
        }
        this.mask = flag;
        this.scale = flag2;
        this.scaleFixed = flag3;
        this.once = true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CurveInfo
    {
        public float duration;
        public float offset;
        public bool calc;
        public bool constant;
        public CurveInfo(AnimationCurve curve)
        {
            int num = (curve != null) ? curve.length : 0;
            switch (num)
            {
                case 0:
                    this.calc = false;
                    this.constant = true;
                    this.duration = 0f;
                    this.offset = 0f;
                    break;

                case 1:
                {
                    Keyframe keyframe3 = curve[0];
                    this.calc = !(keyframe3.value == 0f);
                    this.constant = true;
                    this.duration = 0f;
                    this.offset = 0f;
                    break;
                }
                default:
                {
                    Keyframe keyframe = curve[0];
                    Keyframe keyframe2 = curve[num - 1];
                    this.calc = true;
                    this.constant = false;
                    this.duration = keyframe2.time - keyframe.time;
                    Keyframe keyframe4 = curve[0];
                    this.offset = keyframe4.time;
                    this.duration *= 8f;
                    break;
                }
            }
        }
    }
}

