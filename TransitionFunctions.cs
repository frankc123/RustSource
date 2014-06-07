using Facepunch.Precision;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TransitionFunctions
{
    public static double Acos(double v)
    {
        return Math.Acos(v);
    }

    public static float Acos(float v)
    {
        return Mathf.Acos(v);
    }

    public static double AngleDegrees(Vector3G a, Vector3G b)
    {
        double v = DotNormal(a, b);
        if (v >= 1.0)
        {
            return 0.0;
        }
        if (v <= -1.0)
        {
            return 180.0;
        }
        if (v == 0.0)
        {
            return 90.0;
        }
        return RadiansToDegrees(Acos(v));
    }

    public static float AngleDegrees(Vector3 a, Vector3 b)
    {
        float v = DotNormal(a, b);
        if (v >= 1f)
        {
            return 0f;
        }
        if (v <= -1f)
        {
            return 180f;
        }
        if (v == 0f)
        {
            return 90f;
        }
        return RadiansToDegrees(Acos(v));
    }

    public static double AngleRadians(Vector3G a, Vector3G b)
    {
        double v = DotNormal(a, b);
        if (v >= 1.0)
        {
            return 0.0;
        }
        if (v <= -1.0)
        {
            return 3.1415926535897931;
        }
        if (v == 0.0)
        {
            return 1.5707963267948966;
        }
        return Acos(v);
    }

    public static float AngleRadians(Vector3 a, Vector3 b)
    {
        float v = DotNormal(a, b);
        if (v >= 1f)
        {
            return 0f;
        }
        if (v <= -1f)
        {
            return 3.141593f;
        }
        if (v == 0f)
        {
            return 1.570796f;
        }
        return Acos(v);
    }

    public static double Atan2(double y, double x)
    {
        return Math.Atan2(y, x);
    }

    public static float Atan2(float y, float x)
    {
        return Mathf.Atan2(y, x);
    }

    public static Matrix4x4G Ceil(double t, Matrix4x4G a, Matrix4x4G b)
    {
        return ((t <= 0.0) ? a : b);
    }

    public static QuaternionG Ceil(double t, QuaternionG a, QuaternionG b)
    {
        return ((t <= 0.0) ? a : b);
    }

    public static Vector3G Ceil(double t, Vector3G a, Vector3G b)
    {
        return ((t <= 0.0) ? a : b);
    }

    public static double Ceil(double t, double a, double b)
    {
        return ((t <= 0.0) ? a : b);
    }

    public static double Ceil(float t, double a, double b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static float Ceil(float t, float a, float b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static Color Ceil(float t, Color a, Color b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static Matrix4x4 Ceil(float t, Matrix4x4 a, Matrix4x4 b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static Quaternion Ceil(float t, Quaternion a, Quaternion b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static Vector2 Ceil(float t, Vector2 a, Vector2 b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static Vector3 Ceil(float t, Vector3 a, Vector3 b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static Vector4 Ceil(float t, Vector4 a, Vector4 b)
    {
        return ((t <= 0f) ? a : b);
    }

    public static double Cos(double v)
    {
        return Math.Cos(v);
    }

    public static float Cos(float v)
    {
        return Mathf.Cos(v);
    }

    public static Vector3G Cross(Vector3G a, Vector3G b)
    {
        Vector3G vectorg;
        vectorg.x = (a.y * b.z) - (a.z * b.y);
        vectorg.y = (a.z * b.x) - (a.x * b.z);
        vectorg.z = (a.x * b.y) - (a.y * b.x);
        return vectorg;
    }

    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        Vector3 vector;
        vector.x = (a.y * b.z) - (a.z * b.y);
        vector.y = (a.z * b.x) - (a.x * b.z);
        vector.z = (a.x * b.y) - (a.y * b.x);
        return vector;
    }

    public static double CrossDot(Vector3G a, Vector3G b, Vector3G dotB)
    {
        return (((((a.y * b.z) - (a.z * b.y)) * dotB.x) + (((a.z * b.x) - (a.x * b.z)) * dotB.y)) + (((a.x * b.y) - (a.y * b.x)) * dotB.z));
    }

    public static float CrossDot(Vector3 a, Vector3 b, Vector3 dotB)
    {
        return (((((a.y * b.z) - (a.z * b.y)) * dotB.x) + (((a.z * b.x) - (a.x * b.z)) * dotB.y)) + (((a.x * b.y) - (a.y * b.x)) * dotB.z));
    }

    public static double DegreesToRadians(double rads)
    {
        return (0.017453292519943295 * rads);
    }

    public static float DegreesToRadians(float rads)
    {
        return (0.01745329f * rads);
    }

    private static Vector3G DIR_X(Matrix4x4G a)
    {
        return GET_X0(a);
    }

    private static Vector3 DIR_X(Matrix4x4 a)
    {
        return GET_X0(a);
    }

    private static void DIR_X(ref Matrix4x4G a, Vector3G v)
    {
        SET_X0(ref a, v);
    }

    private static void DIR_X(ref Matrix4x4 a, Vector3 v)
    {
        SET_X0(ref a, v);
    }

    private static Vector3G DIR_Y(Matrix4x4G a)
    {
        return GET_X1(a);
    }

    private static Vector3 DIR_Y(Matrix4x4 a)
    {
        return GET_X1(a);
    }

    private static void DIR_Y(ref Matrix4x4G a, Vector3G v)
    {
        SET_X1(ref a, v);
    }

    private static void DIR_Y(ref Matrix4x4 a, Vector3 v)
    {
        SET_X1(ref a, v);
    }

    private static Vector3G DIR_Z(Matrix4x4G a)
    {
        return GET_X2(a);
    }

    private static Vector3 DIR_Z(Matrix4x4 a)
    {
        return GET_X2(a);
    }

    private static void DIR_Z(ref Matrix4x4G a, Vector3G v)
    {
        SET_X2(ref a, v);
    }

    private static void DIR_Z(ref Matrix4x4 a, Vector3 v)
    {
        SET_X2(ref a, v);
    }

    public static double Distance(double a, double b)
    {
        return ((b <= a) ? (a - b) : (b - a));
    }

    public static float Distance(float a, float b)
    {
        return ((b <= a) ? (a - b) : (b - a));
    }

    public static double Dot(Vector3G a, Vector3G b)
    {
        return (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z));
    }

    public static float Dot(Vector3 a, Vector3 b)
    {
        return (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z));
    }

    public static double DotNormal(Vector3G a, Vector3G b)
    {
        return Dot(Normalize(a), Normalize(b));
    }

    public static float DotNormal(Vector3 a, Vector3 b)
    {
        return Dot(Normalize(a), Normalize(b));
    }

    public static double Evaluate(this TransitionFunction f, double t)
    {
        return f.Evaluate(t, ((double) 0.0), ((double) 1.0));
    }

    public static float Evaluate(this TransitionFunction f, float t)
    {
        return f.Evaluate(t, ((float) 0f), ((float) 1f));
    }

    public static Matrix4x4G Evaluate(this TransitionFunction<Matrix4x4G> v, double t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static QuaternionG Evaluate(this TransitionFunction<QuaternionG> v, double t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Vector3G Evaluate(this TransitionFunction<Vector3G> v, double t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static double Evaluate(this TransitionFunction<double> v, double t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static double Evaluate(this TransitionFunction<double> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static float Evaluate(this TransitionFunction<float> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Color Evaluate(this TransitionFunction<Color> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Matrix4x4 Evaluate(this TransitionFunction<Matrix4x4> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Quaternion Evaluate(this TransitionFunction<Quaternion> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Vector2 Evaluate(this TransitionFunction<Vector2> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Vector3 Evaluate(this TransitionFunction<Vector3> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Vector4 Evaluate(this TransitionFunction<Vector4> v, float t)
    {
        return v.f.Evaluate(t, v.a, v.b);
    }

    public static Matrix4x4G Evaluate(this TransitionFunction f, double t, Matrix4x4G a, Matrix4x4G b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static QuaternionG Evaluate(this TransitionFunction f, double t, QuaternionG a, QuaternionG b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Vector3G Evaluate(this TransitionFunction f, double t, Vector3G a, Vector3G b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static double Evaluate(this TransitionFunction f, double t, double a, double b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static double Evaluate(this TransitionFunction f, float t, double a, double b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static float Evaluate(this TransitionFunction f, float t, float a, float b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Color Evaluate(this TransitionFunction f, float t, Color a, Color b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Matrix4x4 Evaluate(this TransitionFunction f, float t, Matrix4x4 a, Matrix4x4 b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Quaternion Evaluate(this TransitionFunction f, float t, Quaternion a, Quaternion b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Vector2 Evaluate(this TransitionFunction f, float t, Vector2 a, Vector2 b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Vector3 Evaluate(this TransitionFunction f, float t, Vector3 a, Vector3 b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Vector4 Evaluate(this TransitionFunction f, float t, Vector4 a, Vector4 b)
    {
        switch (f)
        {
            case TransitionFunction.Linear:
                return Linear(t, a, b);

            case TransitionFunction.Round:
                return Round(t, a, b);

            case TransitionFunction.Floor:
                return Floor(t, a, b);

            case TransitionFunction.Ceil:
                return Ceil(t, a, b);

            case TransitionFunction.Spline:
                return Spline(t, a, b);
        }
        throw new ArgumentOutOfRangeException("v", "Attempted use of unrecognized TransitionFunction enum value");
    }

    public static Matrix4x4G Floor(double t, Matrix4x4G a, Matrix4x4G b)
    {
        return ((t >= 1.0) ? b : a);
    }

    public static QuaternionG Floor(double t, QuaternionG a, QuaternionG b)
    {
        return ((t >= 1.0) ? b : a);
    }

    public static Vector3G Floor(double t, Vector3G a, Vector3G b)
    {
        return ((t >= 1.0) ? b : a);
    }

    public static double Floor(double t, double a, double b)
    {
        return ((t >= 1.0) ? b : a);
    }

    public static double Floor(float t, double a, double b)
    {
        return ((t >= 1f) ? b : a);
    }

    public static float Floor(float t, float a, float b)
    {
        return ((t >= 1f) ? b : a);
    }

    public static Color Floor(float t, Color a, Color b)
    {
        return ((t >= 1f) ? b : a);
    }

    public static Matrix4x4 Floor(float t, Matrix4x4 a, Matrix4x4 b)
    {
        return ((t >= 1f) ? b : a);
    }

    public static Quaternion Floor(float t, Quaternion a, Quaternion b)
    {
        return ((t >= 1f) ? b : a);
    }

    public static Vector2 Floor(float t, Vector2 a, Vector2 b)
    {
        return ((t >= 1f) ? b : a);
    }

    public static Vector3 Floor(float t, Vector3 a, Vector3 b)
    {
        return ((t >= 1f) ? b : a);
    }

    public static Vector4 Floor(float t, Vector4 a, Vector4 b)
    {
        return ((t >= 1f) ? b : a);
    }

    private static Vector3G GET_0X(Matrix4x4G a)
    {
        return VECT3F(a.m00, a.m01, a.m02);
    }

    private static Vector3 GET_0X(Matrix4x4 a)
    {
        return Vect(a.m00, a.m01, a.m02);
    }

    private static Vector3G GET_1X(Matrix4x4G a)
    {
        return VECT3F(a.m10, a.m11, a.m12);
    }

    private static Vector3 GET_1X(Matrix4x4 a)
    {
        return Vect(a.m10, a.m11, a.m12);
    }

    private static Vector3G GET_2X(Matrix4x4G a)
    {
        return VECT3F(a.m20, a.m21, a.m22);
    }

    private static Vector3 GET_2X(Matrix4x4 a)
    {
        return Vect(a.m20, a.m21, a.m22);
    }

    private static Vector3G GET_3X(Matrix4x4G a)
    {
        return VECT3F(a.m30, a.m31, a.m32);
    }

    private static Vector3 GET_3X(Matrix4x4 a)
    {
        return Vect(a.m30, a.m31, a.m32);
    }

    private static Vector3G GET_X0(Matrix4x4G a)
    {
        return VECT3F(a.m00, a.m10, a.m20);
    }

    private static Vector3 GET_X0(Matrix4x4 a)
    {
        return Vect(a.m00, a.m10, a.m20);
    }

    private static Vector3G GET_X1(Matrix4x4G a)
    {
        return VECT3F(a.m01, a.m11, a.m21);
    }

    private static Vector3 GET_X1(Matrix4x4 a)
    {
        return Vect(a.m01, a.m11, a.m21);
    }

    private static Vector3G GET_X2(Matrix4x4G a)
    {
        return VECT3F(a.m02, a.m12, a.m22);
    }

    private static Vector3 GET_X2(Matrix4x4 a)
    {
        return Vect(a.m02, a.m12, a.m22);
    }

    private static Vector3G GET_X3(Matrix4x4G a)
    {
        return VECT3F(a.m03, a.m13, a.m23);
    }

    private static Vector3 GET_X3(Matrix4x4 a)
    {
        return Vect(a.m03, a.m13, a.m23);
    }

    public static Matrix4x4G Inverse(Matrix4x4G v)
    {
        Matrix4x4G matrixxg;
        Matrix4x4G.Inverse(ref v, out matrixxg);
        return matrixxg;
    }

    public static Matrix4x4 Inverse(Matrix4x4 v)
    {
        return Matrix4x4.Inverse(v);
    }

    public static double Length(Vector3G a)
    {
        return Sqrt((double) (((a.x * a.x) + (a.y * a.y)) + (a.z * a.z)));
    }

    public static float Length(Vector3 a)
    {
        return Sqrt((float) (((a.x * a.x) + (a.y * a.y)) + (a.z * a.z)));
    }

    public static Matrix4x4G Linear(double t, Matrix4x4G a, Matrix4x4G b)
    {
        return Sum(Mul(a, 1.0 - t), Mul(b, t));
    }

    public static QuaternionG Linear(double t, QuaternionG a, QuaternionG b)
    {
        return Slerp(t, a, b);
    }

    public static Vector3G Linear(double t, Vector3G a, Vector3G b)
    {
        return Sum(Mul(a, 1.0 - t), Mul(b, t));
    }

    public static double Linear(double t, double a, double b)
    {
        return Sum(Mul(a, 1.0 - t), Mul(b, t));
    }

    public static double Linear(float t, double a, double b)
    {
        return Sum(Mul(a, (double) (1f - t)), Mul(b, (double) t));
    }

    public static float Linear(float t, float a, float b)
    {
        return Sum(Mul(a, 1f - t), Mul(b, t));
    }

    public static Color Linear(float t, Color a, Color b)
    {
        return Sum(Mul(a, 1f - t), Mul(b, t));
    }

    public static Matrix4x4 Linear(float t, Matrix4x4 a, Matrix4x4 b)
    {
        return Sum(Mul(a, 1f - t), Mul(b, t));
    }

    public static Quaternion Linear(float t, Quaternion a, Quaternion b)
    {
        return Slerp(t, a, b);
    }

    public static Vector2 Linear(float t, Vector2 a, Vector2 b)
    {
        return Sum(Mul(a, 1f - t), Mul(b, t));
    }

    public static Vector3 Linear(float t, Vector3 a, Vector3 b)
    {
        return Sum(Mul(a, 1f - t), Mul(b, t));
    }

    public static Vector4 Linear(float t, Vector4 a, Vector4 b)
    {
        return Sum(Mul(a, 1f - t), Mul(b, t));
    }

    private static Vector3G LLERP(double t, Vector3G a, Vector3G b)
    {
        return Linear(t, a, b);
    }

    private static Vector3 LLERP(float t, Vector3 a, Vector3 b)
    {
        return Linear(t, a, b);
    }

    public static QuaternionG LookRotation(Vector3G forward, Vector3G up)
    {
        QuaternionG ng;
        QuaternionG.LookRotation(ref forward, ref up, out ng);
        return ng;
    }

    public static Quaternion LookRotation(Vector3 forward, Vector3 up)
    {
        return Quaternion.LookRotation(forward, up);
    }

    public static double Max(double a, double b)
    {
        return ((b <= a) ? a : b);
    }

    public static float Max(float a, float b)
    {
        return ((b <= a) ? a : b);
    }

    public static double Min(double a, double b)
    {
        return ((b >= a) ? a : b);
    }

    public static float Min(float a, float b)
    {
        return ((b >= a) ? a : b);
    }

    public static Matrix4x4G Mul(Matrix4x4G a, double b)
    {
        Matrix4x4G matrixxg;
        matrixxg.m00 = a.m00 * b;
        matrixxg.m10 = a.m10 * b;
        matrixxg.m20 = a.m20 * b;
        matrixxg.m30 = a.m30 * b;
        matrixxg.m01 = a.m01 * b;
        matrixxg.m11 = a.m11 * b;
        matrixxg.m21 = a.m21 * b;
        matrixxg.m31 = a.m31 * b;
        matrixxg.m02 = a.m02 * b;
        matrixxg.m12 = a.m12 * b;
        matrixxg.m22 = a.m22 * b;
        matrixxg.m32 = a.m32 * b;
        matrixxg.m03 = a.m03 * b;
        matrixxg.m13 = a.m13 * b;
        matrixxg.m23 = a.m23 * b;
        matrixxg.m33 = a.m33 * b;
        return matrixxg;
    }

    public static QuaternionG Mul(QuaternionG a, double b)
    {
        QuaternionG ng;
        ng.x = a.x * b;
        ng.y = a.y * b;
        ng.z = a.z * b;
        ng.w = a.w * b;
        return ng;
    }

    public static Vector3G Mul(Vector3G a, double b)
    {
        Vector3G vectorg;
        vectorg.x = a.x * b;
        vectorg.y = a.y * b;
        vectorg.z = a.z * b;
        return vectorg;
    }

    public static double Mul(double a, double b)
    {
        return (a * b);
    }

    public static float Mul(float a, float b)
    {
        return (a * b);
    }

    public static Color Mul(Color a, float b)
    {
        Color color;
        color.r = a.r * b;
        color.g = a.g * b;
        color.b = a.b * b;
        color.a = a.a * b;
        return color;
    }

    public static Matrix4x4 Mul(Matrix4x4 a, float b)
    {
        Matrix4x4 matrixx;
        matrixx.m00 = a.m00 * b;
        matrixx.m10 = a.m10 * b;
        matrixx.m20 = a.m20 * b;
        matrixx.m30 = a.m30 * b;
        matrixx.m01 = a.m01 * b;
        matrixx.m11 = a.m11 * b;
        matrixx.m21 = a.m21 * b;
        matrixx.m31 = a.m31 * b;
        matrixx.m02 = a.m02 * b;
        matrixx.m12 = a.m12 * b;
        matrixx.m22 = a.m22 * b;
        matrixx.m32 = a.m32 * b;
        matrixx.m03 = a.m03 * b;
        matrixx.m13 = a.m13 * b;
        matrixx.m23 = a.m23 * b;
        matrixx.m33 = a.m33 * b;
        return matrixx;
    }

    public static Quaternion Mul(Quaternion a, float b)
    {
        Quaternion quaternion;
        quaternion.x = a.x * b;
        quaternion.y = a.y * b;
        quaternion.z = a.z * b;
        quaternion.w = a.w * b;
        return quaternion;
    }

    public static Vector2 Mul(Vector2 a, float b)
    {
        Vector2 vector;
        vector.x = a.x * b;
        vector.y = a.y * b;
        return vector;
    }

    public static Vector3 Mul(Vector3 a, float b)
    {
        Vector3 vector;
        vector.x = a.x * b;
        vector.y = a.y * b;
        vector.z = a.z * b;
        return vector;
    }

    public static Vector4 Mul(Vector4 a, float b)
    {
        Vector4 vector;
        vector.x = a.x * b;
        vector.y = a.y * b;
        vector.z = a.z * b;
        vector.w = a.w * b;
        return vector;
    }

    public static Vector3G Normalize(Vector3G v)
    {
        Vector3G vectorg;
        double num = ((v.x * v.x) + (v.y * v.y)) + (v.z * v.z);
        switch (num)
        {
            case 0.0:
            case 1.0:
                return v;
        }
        num = Sqrt(num);
        vectorg.x = v.x / num;
        vectorg.y = v.y / num;
        vectorg.z = v.z / num;
        return vectorg;
    }

    public static Vector3 Normalize(Vector3 v)
    {
        Vector3 vector;
        float num = ((v.x * v.x) + (v.y * v.y)) + (v.z * v.z);
        switch (num)
        {
            case 0f:
            case 1f:
                return v;
        }
        num = Sqrt(num);
        vector.x = v.x / num;
        vector.y = v.y / num;
        vector.z = v.z / num;
        return vector;
    }

    public static double RadiansToDegrees(double degs)
    {
        return (57.295779513082323 * degs);
    }

    public static float RadiansToDegrees(float degs)
    {
        return (57.29578f * degs);
    }

    public static Vector3G Rotate(QuaternionG rotation, Vector3G vector)
    {
        Vector3G vectorg;
        QuaternionG.Mult(ref rotation, ref vector, out vectorg);
        return vectorg;
    }

    public static Vector3 Rotate(Quaternion rotation, Vector3 vector)
    {
        return (Vector3) (rotation * vector);
    }

    public static Matrix4x4G Round(double t, Matrix4x4G a, Matrix4x4G b)
    {
        return ((t >= 0.5) ? b : a);
    }

    public static QuaternionG Round(double t, QuaternionG a, QuaternionG b)
    {
        return ((t >= 0.5) ? b : a);
    }

    public static Vector3G Round(double t, Vector3G a, Vector3G b)
    {
        return ((t >= 0.5) ? b : a);
    }

    public static double Round(double t, double a, double b)
    {
        return ((t >= 0.5) ? b : a);
    }

    public static double Round(float t, double a, double b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    public static float Round(float t, float a, float b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    public static Color Round(float t, Color a, Color b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    public static Matrix4x4 Round(float t, Matrix4x4 a, Matrix4x4 b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    public static Quaternion Round(float t, Quaternion a, Quaternion b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    public static Vector2 Round(float t, Vector2 a, Vector2 b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    public static Vector3 Round(float t, Vector3 a, Vector3 b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    public static Vector4 Round(float t, Vector4 a, Vector4 b)
    {
        return ((t >= 0.5f) ? b : a);
    }

    private static Vector3G SCALE(Matrix4x4G a)
    {
        return GET_3X(a);
    }

    private static Vector3 SCALE(Matrix4x4 a)
    {
        return GET_3X(a);
    }

    private static void SCALE(ref Matrix4x4G a, Vector3G v)
    {
        SET_3X(ref a, v);
    }

    private static void SCALE(ref Matrix4x4 a, Vector3 v)
    {
        SET_3X(ref a, v);
    }

    public static Vector3G Scale3(double xyz)
    {
        Vector3G vectorg;
        vectorg.x = vectorg.y = vectorg.z = xyz;
        return vectorg;
    }

    public static Vector3 Scale3(float xyz)
    {
        Vector3 vector;
        vector.x = vector.y = vector.z = xyz;
        return vector;
    }

    private static void SET_0X(ref Matrix4x4G m, Vector3G v)
    {
        m.m00 = v.x;
        m.m01 = v.y;
        m.m02 = v.z;
    }

    private static void SET_0X(ref Matrix4x4 m, Vector3 v)
    {
        m.m00 = v.x;
        m.m01 = v.y;
        m.m02 = v.z;
    }

    private static void SET_1X(ref Matrix4x4G m, Vector3G v)
    {
        m.m10 = v.x;
        m.m11 = v.y;
        m.m12 = v.z;
    }

    private static void SET_1X(ref Matrix4x4 m, Vector3 v)
    {
        m.m10 = v.x;
        m.m11 = v.y;
        m.m12 = v.z;
    }

    private static void SET_2X(ref Matrix4x4G m, Vector3G v)
    {
        m.m20 = v.x;
        m.m21 = v.y;
        m.m22 = v.z;
    }

    private static void SET_2X(ref Matrix4x4 m, Vector3 v)
    {
        m.m20 = v.x;
        m.m21 = v.y;
        m.m22 = v.z;
    }

    private static void SET_3X(ref Matrix4x4G m, Vector3G v)
    {
        m.m30 = v.x;
        m.m31 = v.y;
        m.m32 = v.z;
    }

    private static void SET_3X(ref Matrix4x4 m, Vector3 v)
    {
        m.m30 = v.x;
        m.m31 = v.y;
        m.m32 = v.z;
    }

    private static void SET_X0(ref Matrix4x4G m, Vector3G v)
    {
        m.m00 = v.x;
        m.m10 = v.y;
        m.m20 = v.z;
    }

    private static void SET_X0(ref Matrix4x4 m, Vector3 v)
    {
        m.m00 = v.x;
        m.m10 = v.y;
        m.m20 = v.z;
    }

    private static void SET_X1(ref Matrix4x4G m, Vector3G v)
    {
        m.m01 = v.x;
        m.m11 = v.y;
        m.m21 = v.z;
    }

    private static void SET_X1(ref Matrix4x4 m, Vector3 v)
    {
        m.m01 = v.x;
        m.m11 = v.y;
        m.m21 = v.z;
    }

    private static void SET_X2(ref Matrix4x4G m, Vector3G v)
    {
        m.m02 = v.x;
        m.m12 = v.y;
        m.m22 = v.z;
    }

    private static void SET_X2(ref Matrix4x4 m, Vector3 v)
    {
        m.m02 = v.x;
        m.m12 = v.y;
        m.m22 = v.z;
    }

    private static void SET_X3(ref Matrix4x4G m, Vector3G v)
    {
        m.m03 = v.x;
        m.m13 = v.y;
        m.m23 = v.z;
    }

    private static void SET_X3(ref Matrix4x4 m, Vector3 v)
    {
        m.m03 = v.x;
        m.m13 = v.y;
        m.m23 = v.z;
    }

    private static double SimpleSpline(double v01)
    {
        return ((3.0 * (v01 * v01)) - ((2.0 * (v01 * v01)) * v01));
    }

    private static float SimpleSpline(float v01)
    {
        return ((3f * (v01 * v01)) - ((2f * (v01 * v01)) * v01));
    }

    public static double Sin(double v)
    {
        return Math.Sin(v);
    }

    public static float Sin(float v)
    {
        return Mathf.Sin(v);
    }

    public static Matrix4x4G Slerp(double t, Matrix4x4G a, Matrix4x4G b)
    {
        Matrix4x4G identity = Matrix4x4G.identity;
        Vector3G dotB = Slerp(t, DIR_X(a), DIR_X(b));
        Vector3G up = Slerp(t, DIR_Y(a), DIR_Y(b));
        Vector3G forward = Slerp(t, DIR_Z(a), DIR_Z(b));
        QuaternionG rotation = LookRotation(forward, up);
        up = Rotate(rotation, Y3(Length(up)));
        if (CrossDot(forward, up, dotB) > 0.0)
        {
            dotB = Rotate(rotation, X3(-Length(dotB)));
        }
        else
        {
            dotB = Rotate(rotation, X3(Length(dotB)));
        }
        DIR_X(ref identity, dotB);
        DIR_Y(ref identity, up);
        DIR_Z(ref identity, forward);
        SCALE(ref identity, Linear(t, SCALE(a), SCALE(b)));
        TRANS(ref identity, Linear(t, TRANS(a), TRANS(b)));
        identity.m33 = Linear(t, a.m33, b.m33);
        return identity;
    }

    public static QuaternionG Slerp(double t, QuaternionG a, QuaternionG b)
    {
        QuaternionG ng;
        double num2;
        double num3;
        double num4;
        if (t == 0.0)
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        double v = (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z)) + (a.w * b.w);
        if (v == 1.0)
        {
            return a;
        }
        if (v < 0.0)
        {
            v = Acos(-v);
            num4 = Sin(v);
            if (num4 == 0.0)
            {
                num2 = 1.0 - t;
                ng.x = (a.x * num2) + (b.x * t);
                ng.y = (a.y * num2) + (b.y * t);
                ng.z = (a.z * num2) + (b.z * t);
                ng.w = (a.w * num2) + (b.w * t);
                return ng;
            }
            num3 = Sin((double) (v * t));
            num2 = Sin((double) (v * (1.0 - t)));
            ng.x = ((a.x * num2) - (b.x * num3)) / num4;
            ng.y = ((a.y * num2) - (b.y * num3)) / num4;
            ng.z = ((a.z * num2) - (b.z * num3)) / num4;
            ng.w = ((a.w * num2) - (b.w * num3)) / num4;
            return ng;
        }
        v = Acos(v);
        num4 = Sin(v);
        if (num4 == 0.0)
        {
            num2 = 1.0 - t;
            ng.x = (a.x * num2) + (b.x * t);
            ng.y = (a.y * num2) + (b.y * t);
            ng.z = (a.z * num2) + (b.z * t);
            ng.w = (a.w * num2) + (b.w * t);
            return ng;
        }
        num3 = Sin((double) (v * t));
        num2 = Sin((double) (v * (1.0 - t)));
        ng.x = ((a.x * num2) + (b.x * num3)) / num4;
        ng.y = ((a.y * num2) + (b.y * num3)) / num4;
        ng.z = ((a.z * num2) + (b.z * num3)) / num4;
        ng.w = ((a.w * num2) + (b.w * num3)) / num4;
        return ng;
    }

    public static Vector3G Slerp(double t, Vector3G a, Vector3G b)
    {
        double num2;
        double v = AngleRadians(a, b);
        if ((v == 0.0) || ((num2 = Sin(v)) == 0.0))
        {
            return Linear(t, a, b);
        }
        double num3 = Sin((double) ((1.0 - t) * v)) / num2;
        double num4 = Sin((double) (t * v)) / num2;
        return Sum(Mul(a, num3), Mul(b, num4));
    }

    public static Matrix4x4 Slerp(float t, Matrix4x4 a, Matrix4x4 b)
    {
        Matrix4x4 identity = Matrix4x4.identity;
        Vector3 dotB = Slerp(t, DIR_X(a), DIR_X(b));
        Vector3 up = Slerp(t, DIR_Y(a), DIR_Y(b));
        Vector3 forward = Slerp(t, DIR_Z(a), DIR_Z(b));
        Quaternion rotation = LookRotation(forward, up);
        up = Rotate(rotation, Y3(Length(up)));
        if (CrossDot(forward, up, dotB) > 0f)
        {
            dotB = Rotate(rotation, X3(-Length(dotB)));
        }
        else
        {
            dotB = Rotate(rotation, X3(Length(dotB)));
        }
        DIR_X(ref identity, dotB);
        DIR_Y(ref identity, up);
        DIR_Z(ref identity, forward);
        SCALE(ref identity, Linear(t, SCALE(a), SCALE(b)));
        TRANS(ref identity, Linear(t, TRANS(a), TRANS(b)));
        identity.m33 = Linear(t, a.m33, b.m33);
        return identity;
    }

    public static Quaternion Slerp(float t, Quaternion a, Quaternion b)
    {
        Quaternion quaternion;
        float num2;
        float num3;
        float num4;
        if (t == 0f)
        {
            return a;
        }
        if (t == 1f)
        {
            return b;
        }
        float v = (((a.x * b.x) + (a.y * b.y)) + (a.z * b.z)) + (a.w * b.w);
        if (v == 1f)
        {
            return a;
        }
        if (v < 0f)
        {
            v = Acos(-v);
            num4 = Sin(v);
            if (num4 == 0f)
            {
                num2 = 1f - t;
                quaternion.x = (a.x * num2) + (b.x * t);
                quaternion.y = (a.y * num2) + (b.y * t);
                quaternion.z = (a.z * num2) + (b.z * t);
                quaternion.w = (a.w * num2) + (b.w * t);
                return quaternion;
            }
            num3 = Sin((float) (v * t));
            num2 = Sin((float) (v * (1f - t)));
            quaternion.x = ((a.x * num2) - (b.x * num3)) / num4;
            quaternion.y = ((a.y * num2) - (b.y * num3)) / num4;
            quaternion.z = ((a.z * num2) - (b.z * num3)) / num4;
            quaternion.w = ((a.w * num2) - (b.w * num3)) / num4;
            return quaternion;
        }
        v = Acos(v);
        num4 = Sin(v);
        if (num4 == 0f)
        {
            num2 = 1f - t;
            quaternion.x = (a.x * num2) + (b.x * t);
            quaternion.y = (a.y * num2) + (b.y * t);
            quaternion.z = (a.z * num2) + (b.z * t);
            quaternion.w = (a.w * num2) + (b.w * t);
            return quaternion;
        }
        num3 = Sin((float) (v * t));
        num2 = Sin((float) (v * (1f - t)));
        quaternion.x = ((a.x * num2) + (b.x * num3)) / num4;
        quaternion.y = ((a.y * num2) + (b.y * num3)) / num4;
        quaternion.z = ((a.z * num2) + (b.z * num3)) / num4;
        quaternion.w = ((a.w * num2) + (b.w * num3)) / num4;
        return quaternion;
    }

    public static Vector2 Slerp(float t, Vector2 a, Vector2 b)
    {
        float num2;
        float v = DegreesToRadians(Vector2.Angle(a, b));
        if ((v == 0f) || ((num2 = Sin(v)) == 0f))
        {
            return Linear(t, a, b);
        }
        float num3 = Sin((float) ((1f - t) * v)) / num2;
        float num4 = Sin((float) (t * v)) / num2;
        return Sum(Mul(a, num3), Mul(b, num4));
    }

    public static Vector3 Slerp(float t, Vector3 a, Vector3 b)
    {
        float num2;
        float v = AngleRadians(a, b);
        if ((v == 0f) || ((num2 = Sin(v)) == 0f))
        {
            return Linear(t, a, b);
        }
        float num3 = Sin((float) ((1f - t) * v)) / num2;
        float num4 = Sin((float) (t * v)) / num2;
        return Sum(Mul(a, num3), Mul(b, num4));
    }

    private static Vector3G SLERP(double t, Vector3G a, Vector3G b)
    {
        return Slerp(t, a, b);
    }

    private static Vector3 SLERP(float t, Vector3 a, Vector3 b)
    {
        return Slerp(t, a, b);
    }

    public static Matrix4x4G SlerpWorldToCamera(double t, Matrix4x4G a, Matrix4x4G b)
    {
        return Inverse(Slerp(t, Inverse(a), Inverse(b)));
    }

    public static Matrix4x4 SlerpWorldToCamera(float t, Matrix4x4 a, Matrix4x4 b)
    {
        return Slerp(t, a.inverse, b.inverse).inverse;
    }

    public static Matrix4x4G Spline(double t, Matrix4x4G a, Matrix4x4G b)
    {
        return ((t > 0.0) ? ((t < 1.0) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static QuaternionG Spline(double t, QuaternionG a, QuaternionG b)
    {
        return ((t > 0.0) ? ((t < 1.0) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static Vector3G Spline(double t, Vector3G a, Vector3G b)
    {
        return ((t > 0.0) ? ((t < 1.0) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static double Spline(double t, double a, double b)
    {
        return ((t > 0.0) ? ((t < 1.0) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static double Spline(float t, double a, double b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static float Spline(float t, float a, float b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static Color Spline(float t, Color a, Color b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static Matrix4x4 Spline(float t, Matrix4x4 a, Matrix4x4 b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static Quaternion Spline(float t, Quaternion a, Quaternion b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static Vector2 Spline(float t, Vector2 a, Vector2 b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static Vector3 Spline(float t, Vector3 a, Vector3 b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static Vector4 Spline(float t, Vector4 a, Vector4 b)
    {
        return ((t > 0f) ? ((t < 1f) ? Linear(SimpleSpline(t), a, b) : b) : a);
    }

    public static double Sqrt(double v)
    {
        return Math.Sqrt(v);
    }

    public static float Sqrt(float v)
    {
        return Mathf.Sqrt(v);
    }

    public static Matrix4x4G Sum(Matrix4x4G a, Matrix4x4G b)
    {
        Matrix4x4G matrixxg;
        matrixxg.m00 = a.m00 + b.m00;
        matrixxg.m10 = a.m10 + b.m10;
        matrixxg.m20 = a.m20 + b.m20;
        matrixxg.m30 = a.m30 + b.m30;
        matrixxg.m01 = a.m01 + b.m01;
        matrixxg.m11 = a.m11 + b.m11;
        matrixxg.m21 = a.m21 + b.m21;
        matrixxg.m31 = a.m31 + b.m31;
        matrixxg.m02 = a.m02 + b.m02;
        matrixxg.m12 = a.m12 + b.m12;
        matrixxg.m22 = a.m22 + b.m22;
        matrixxg.m32 = a.m32 + b.m32;
        matrixxg.m03 = a.m03 + b.m03;
        matrixxg.m13 = a.m13 + b.m13;
        matrixxg.m23 = a.m23 + b.m23;
        matrixxg.m33 = a.m33 + b.m33;
        return matrixxg;
    }

    public static QuaternionG Sum(QuaternionG a, QuaternionG b)
    {
        QuaternionG ng;
        ng.x = a.x + b.x;
        ng.y = a.y + b.y;
        ng.z = a.z + b.z;
        ng.w = a.w * b.w;
        return ng;
    }

    public static Vector3G Sum(Vector3G a, Vector3G b)
    {
        Vector3G vectorg;
        vectorg.x = a.x + b.x;
        vectorg.y = a.y + b.y;
        vectorg.z = a.z + b.z;
        return vectorg;
    }

    public static double Sum(double a, double b)
    {
        return (a + b);
    }

    public static float Sum(float a, float b)
    {
        return (a + b);
    }

    public static Color Sum(Color a, Color b)
    {
        Color color;
        color.r = a.r + b.r;
        color.g = a.g + b.g;
        color.b = a.b + b.b;
        color.a = a.a * b.a;
        return color;
    }

    public static Matrix4x4 Sum(Matrix4x4 a, Matrix4x4 b)
    {
        Matrix4x4 matrixx;
        matrixx.m00 = a.m00 + b.m00;
        matrixx.m10 = a.m10 + b.m10;
        matrixx.m20 = a.m20 + b.m20;
        matrixx.m30 = a.m30 + b.m30;
        matrixx.m01 = a.m01 + b.m01;
        matrixx.m11 = a.m11 + b.m11;
        matrixx.m21 = a.m21 + b.m21;
        matrixx.m31 = a.m31 + b.m31;
        matrixx.m02 = a.m02 + b.m02;
        matrixx.m12 = a.m12 + b.m12;
        matrixx.m22 = a.m22 + b.m22;
        matrixx.m32 = a.m32 + b.m32;
        matrixx.m03 = a.m03 + b.m03;
        matrixx.m13 = a.m13 + b.m13;
        matrixx.m23 = a.m23 + b.m23;
        matrixx.m33 = a.m33 + b.m33;
        return matrixx;
    }

    public static Quaternion Sum(Quaternion a, Quaternion b)
    {
        Quaternion quaternion;
        quaternion.x = a.x + b.x;
        quaternion.y = a.y + b.y;
        quaternion.z = a.z + b.z;
        quaternion.w = a.w * b.w;
        return quaternion;
    }

    public static Vector2 Sum(Vector2 a, Vector2 b)
    {
        Vector2 vector;
        vector.x = a.x + b.x;
        vector.y = a.y + b.y;
        return vector;
    }

    public static Vector3 Sum(Vector3 a, Vector3 b)
    {
        Vector3 vector;
        vector.x = a.x + b.x;
        vector.y = a.y + b.y;
        vector.z = a.z + b.z;
        return vector;
    }

    public static Vector4 Sum(Vector4 a, Vector4 b)
    {
        Vector4 vector;
        vector.x = a.x + b.x;
        vector.y = a.y + b.y;
        vector.z = a.z + b.z;
        vector.w = a.w * b.w;
        return vector;
    }

    private static Vector3G TRANS(Matrix4x4G a)
    {
        return GET_X3(a);
    }

    private static Vector3 TRANS(Matrix4x4 a)
    {
        return GET_X3(a);
    }

    private static void TRANS(ref Matrix4x4G a, Vector3G v)
    {
        SET_X3(ref a, v);
    }

    private static void TRANS(ref Matrix4x4 a, Vector3 v)
    {
        SET_X3(ref a, v);
    }

    public static Matrix4x4G Transpose(Matrix4x4G v)
    {
        Matrix4x4G matrixxg;
        Matrix4x4G.Transpose(ref v, out matrixxg);
        return matrixxg;
    }

    public static Matrix4x4 Transpose(Matrix4x4 v)
    {
        return Matrix4x4.Transpose(v);
    }

    public static Vector3G Vect(double x, double y, double z)
    {
        Vector3G vectorg;
        vectorg.x = x;
        vectorg.y = y;
        vectorg.z = z;
        return vectorg;
    }

    public static Vector3 Vect(float x, float y, float z)
    {
        Vector3 vector;
        vector.x = x;
        vector.y = y;
        vector.z = z;
        return vector;
    }

    private static Vector3G VECT3F(double x, double y, double z)
    {
        Vector3G vectorg;
        vectorg.x = x;
        vectorg.y = y;
        vectorg.z = z;
        return vectorg;
    }

    public static Vector3G X3(double x)
    {
        Vector3G vectorg;
        vectorg.y = vectorg.z = 0.0;
        vectorg.x = x;
        return vectorg;
    }

    public static Vector3 X3(float x)
    {
        Vector3 vector;
        vector.y = vector.z = 0f;
        vector.x = x;
        return vector;
    }

    public static Vector3G Y3(double y)
    {
        Vector3G vectorg;
        vectorg.x = vectorg.z = 0.0;
        vectorg.y = y;
        return vectorg;
    }

    public static Vector3 Y3(float y)
    {
        Vector3 vector;
        vector.x = vector.z = 0f;
        vector.y = y;
        return vector;
    }

    public static Vector3G Z3(double z)
    {
        Vector3G vectorg;
        vectorg.x = vectorg.y = 0.0;
        vectorg.z = z;
        return vectorg;
    }

    public static Vector3 Z3(float z)
    {
        Vector3 vector;
        vector.x = vector.y = 0f;
        vector.z = z;
        return vector;
    }
}

