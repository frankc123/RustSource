using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class CurveUtility
{
    public static float EvaluateClampedTime(this AnimationCurve curve, ref float time, float advance)
    {
        int length = curve.length;
        if (curve.length == 0)
        {
            return 1f;
        }
        if (curve.length == 1)
        {
            return curve.Evaluate(0f);
        }
        if (advance > 0f)
        {
            Keyframe keyframe = curve[length - 1];
            float num2 = keyframe.time;
            if (time < num2)
            {
                time += advance;
                if (time > num2)
                {
                    time = num2;
                }
            }
        }
        else if (advance < 0f)
        {
            Keyframe keyframe2 = curve[0];
            float num3 = keyframe2.time;
            if (time > num3)
            {
                time += advance;
                if (time < num3)
                {
                    time = num3;
                }
            }
        }
        return curve.Evaluate(time);
    }

    public static float GetEndTime(this AnimationCurve curve)
    {
        if (curve.length == 0)
        {
            return 0f;
        }
        Keyframe keyframe = curve[curve.length - 1];
        return keyframe.time;
    }

    public static float GetStartTime(this AnimationCurve curve)
    {
        if (curve.length == 0)
        {
            return 0f;
        }
        Keyframe keyframe = curve[0];
        return keyframe.time;
    }
}

