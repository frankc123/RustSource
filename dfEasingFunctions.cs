using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class dfEasingFunctions
{
    private static float bounce(float start, float end, float time)
    {
        time /= 1f;
        end -= start;
        if (time < 0.3636364f)
        {
            return ((end * ((7.5625f * time) * time)) + start);
        }
        if (time < 0.7272727f)
        {
            time -= 0.5454546f;
            return ((end * (((7.5625f * time) * time) + 0.75f)) + start);
        }
        if (time < 0.90909090909090906)
        {
            time -= 0.8181818f;
            return ((end * (((7.5625f * time) * time) + 0.9375f)) + start);
        }
        time -= 0.9545454f;
        return ((end * (((7.5625f * time) * time) + 0.984375f)) + start);
    }

    private static float clerp(float start, float end, float time)
    {
        float num = 0f;
        float num2 = 360f;
        float num3 = Mathf.Abs((float) ((num2 - num) / 2f));
        float num5 = 0f;
        if ((end - start) < -num3)
        {
            num5 = ((num2 - start) + end) * time;
            return (start + num5);
        }
        if ((end - start) > num3)
        {
            num5 = -((num2 - end) + start) * time;
            return (start + num5);
        }
        return (start + ((end - start) * time));
    }

    private static float easeInBack(float start, float end, float time)
    {
        end -= start;
        time /= 1f;
        float num = 1.70158f;
        return ((((end * time) * time) * (((num + 1f) * time) - num)) + start);
    }

    private static float easeInCirc(float start, float end, float time)
    {
        end -= start;
        return ((-end * (Mathf.Sqrt(1f - (time * time)) - 1f)) + start);
    }

    private static float easeInCubic(float start, float end, float time)
    {
        end -= start;
        return ((((end * time) * time) * time) + start);
    }

    private static float easeInExpo(float start, float end, float time)
    {
        end -= start;
        return ((end * Mathf.Pow(2f, 10f * ((time / 1f) - 1f))) + start);
    }

    private static float easeInOutBack(float start, float end, float time)
    {
        float num = 1.70158f;
        end -= start;
        time /= 0.5f;
        if (time < 1f)
        {
            num *= 1.525f;
            return (((end / 2f) * ((time * time) * (((num + 1f) * time) - num))) + start);
        }
        time -= 2f;
        num *= 1.525f;
        return (((end / 2f) * (((time * time) * (((num + 1f) * time) + num)) + 2f)) + start);
    }

    private static float easeInOutCirc(float start, float end, float time)
    {
        time /= 0.5f;
        end -= start;
        if (time < 1f)
        {
            return (((-end / 2f) * (Mathf.Sqrt(1f - (time * time)) - 1f)) + start);
        }
        time -= 2f;
        return (((end / 2f) * (Mathf.Sqrt(1f - (time * time)) + 1f)) + start);
    }

    private static float easeInOutCubic(float start, float end, float time)
    {
        time /= 0.5f;
        end -= start;
        if (time < 1f)
        {
            return (((((end / 2f) * time) * time) * time) + start);
        }
        time -= 2f;
        return (((end / 2f) * (((time * time) * time) + 2f)) + start);
    }

    private static float easeInOutExpo(float start, float end, float time)
    {
        time /= 0.5f;
        end -= start;
        if (time < 1f)
        {
            return (((end / 2f) * Mathf.Pow(2f, 10f * (time - 1f))) + start);
        }
        time--;
        return (((end / 2f) * (-Mathf.Pow(2f, -10f * time) + 2f)) + start);
    }

    private static float easeInOutQuad(float start, float end, float time)
    {
        time /= 0.5f;
        end -= start;
        if (time < 1f)
        {
            return ((((end / 2f) * time) * time) + start);
        }
        time--;
        return (((-end / 2f) * ((time * (time - 2f)) - 1f)) + start);
    }

    private static float easeInOutQuart(float start, float end, float time)
    {
        time /= 0.5f;
        end -= start;
        if (time < 1f)
        {
            return ((((((end / 2f) * time) * time) * time) * time) + start);
        }
        time -= 2f;
        return (((-end / 2f) * ((((time * time) * time) * time) - 2f)) + start);
    }

    private static float easeInOutQuint(float start, float end, float time)
    {
        time /= 0.5f;
        end -= start;
        if (time < 1f)
        {
            return (((((((end / 2f) * time) * time) * time) * time) * time) + start);
        }
        time -= 2f;
        return (((end / 2f) * (((((time * time) * time) * time) * time) + 2f)) + start);
    }

    private static float easeInOutSine(float start, float end, float time)
    {
        end -= start;
        return (((-end / 2f) * (Mathf.Cos((3.141593f * time) / 1f) - 1f)) + start);
    }

    private static float easeInQuad(float start, float end, float time)
    {
        end -= start;
        return (((end * time) * time) + start);
    }

    private static float easeInQuart(float start, float end, float time)
    {
        end -= start;
        return (((((end * time) * time) * time) * time) + start);
    }

    private static float easeInQuint(float start, float end, float time)
    {
        end -= start;
        return ((((((end * time) * time) * time) * time) * time) + start);
    }

    private static float easeInSine(float start, float end, float time)
    {
        end -= start;
        return (((-end * Mathf.Cos((time / 1f) * 1.570796f)) + end) + start);
    }

    private static float easeOutBack(float start, float end, float time)
    {
        float num = 1.70158f;
        end -= start;
        time = (time / 1f) - 1f;
        return ((end * (((time * time) * (((num + 1f) * time) + num)) + 1f)) + start);
    }

    private static float easeOutCirc(float start, float end, float time)
    {
        time--;
        end -= start;
        return ((end * Mathf.Sqrt(1f - (time * time))) + start);
    }

    private static float easeOutCubic(float start, float end, float time)
    {
        time--;
        end -= start;
        return ((end * (((time * time) * time) + 1f)) + start);
    }

    private static float easeOutExpo(float start, float end, float time)
    {
        end -= start;
        return ((end * (-Mathf.Pow(2f, (-10f * time) / 1f) + 1f)) + start);
    }

    private static float easeOutQuad(float start, float end, float time)
    {
        end -= start;
        return (((-end * time) * (time - 2f)) + start);
    }

    private static float easeOutQuart(float start, float end, float time)
    {
        time--;
        end -= start;
        return ((-end * ((((time * time) * time) * time) - 1f)) + start);
    }

    private static float easeOutQuint(float start, float end, float time)
    {
        time--;
        end -= start;
        return ((end * (((((time * time) * time) * time) * time) + 1f)) + start);
    }

    private static float easeOutSine(float start, float end, float time)
    {
        end -= start;
        return ((end * Mathf.Sin((time / 1f) * 1.570796f)) + start);
    }

    public static EasingFunction GetFunction(dfEasingType easeType)
    {
        switch (easeType)
        {
            case dfEasingType.Linear:
                return new EasingFunction(dfEasingFunctions.linear);

            case dfEasingType.Bounce:
                return new EasingFunction(dfEasingFunctions.bounce);

            case dfEasingType.BackEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInBack);

            case dfEasingType.BackEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutBack);

            case dfEasingType.BackEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutBack);

            case dfEasingType.CircEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInCirc);

            case dfEasingType.CircEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutCirc);

            case dfEasingType.CircEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutCirc);

            case dfEasingType.CubicEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInCubic);

            case dfEasingType.CubicEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutCubic);

            case dfEasingType.CubicEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutCubic);

            case dfEasingType.ExpoEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInExpo);

            case dfEasingType.ExpoEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutExpo);

            case dfEasingType.ExpoEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutExpo);

            case dfEasingType.QuadEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInQuad);

            case dfEasingType.QuadEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutQuad);

            case dfEasingType.QuadEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutQuad);

            case dfEasingType.QuartEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInQuart);

            case dfEasingType.QuartEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutQuart);

            case dfEasingType.QuartEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutQuart);

            case dfEasingType.QuintEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInQuint);

            case dfEasingType.QuintEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutQuint);

            case dfEasingType.QuintEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutQuint);

            case dfEasingType.SineEaseIn:
                return new EasingFunction(dfEasingFunctions.easeInSine);

            case dfEasingType.SineEaseOut:
                return new EasingFunction(dfEasingFunctions.easeOutSine);

            case dfEasingType.SineEaseInOut:
                return new EasingFunction(dfEasingFunctions.easeInOutSine);

            case dfEasingType.Spring:
                return new EasingFunction(dfEasingFunctions.spring);
        }
        throw new NotImplementedException();
    }

    private static float linear(float start, float end, float time)
    {
        return Mathf.Lerp(start, end, time);
    }

    private static float punch(float amplitude, float time)
    {
        float num = 9f;
        if (time == 0f)
        {
            return 0f;
        }
        if (time == 1f)
        {
            return 0f;
        }
        float num2 = 0.3f;
        num = (num2 / 6.283185f) * Mathf.Asin(0f);
        return ((amplitude * Mathf.Pow(2f, -10f * time)) * Mathf.Sin((((time * 1f) - num) * 6.283185f) / num2));
    }

    private static float spring(float start, float end, float time)
    {
        time = Mathf.Clamp01(time);
        time = ((Mathf.Sin((time * 3.141593f) * (0.2f + (((2.5f * time) * time) * time))) * Mathf.Pow(1f - time, 2.2f)) + time) * (1f + (1.2f * (1f - time)));
        return (start + ((end - start) * time));
    }

    public delegate float EasingFunction(float start, float end, float time);
}

