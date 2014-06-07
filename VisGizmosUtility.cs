using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class VisGizmosUtility
{
    private static Vector3[] circleVerts = new Vector3[0x1f];
    private const float degreePerCircleVert = 11.25f;
    private const int halveCircleIndex = 0x10;
    private const int lengthCircleVerts = 0x1f;
    private static Matrix4x4[] matStack = new Matrix4x4[8];
    private static readonly Matrix4x4 ninetyX = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90f, 0f, 0f), Vector3.one);
    private static readonly Matrix4x4 ninetyY = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 90f, 0f), Vector3.one);
    private static readonly Matrix4x4 ninetyZ = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
    private const int numCircleVerts = 0x20;
    private const float radPerCircleVert = 0.1963495f;
    private static int stackPos = 0;

    static VisGizmosUtility()
    {
        for (int i = 0; i < 0x1f; i++)
        {
            float f = 0.1963495f * i;
            circleVerts[i].x = Mathf.Cos(f);
            circleVerts[i].y = Mathf.Sin(f);
        }
    }

    public static void DrawAngle(Vector3 origin, Vector3 heading, Vector3 axis, float angle, float radius)
    {
        PushMatrix();
        if (angle < 0f)
        {
            axis = -axis;
            angle = -angle;
        }
        Vector3 upwards = Vector3.Cross(axis, heading);
        Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.LookRotation(axis, upwards), new Vector3(radius, radius, 1f)) * Gizmos.matrix;
        Vector3 zero = Vector3.zero;
        if (angle == 0f)
        {
            Gizmos.DrawLine(Vector3.zero, new Vector3(0f, 1f, 0f));
        }
        else if (angle < 360f)
        {
            Vector3 vector3;
            int num = 0;
            float num2 = 0f;
            do
            {
                vector3 = circleVerts[num++];
                Gizmos.DrawLine(zero, vector3);
                zero = vector3;
                num2 += 11.25f;
            }
            while (num2 < angle);
            if (num2 != angle)
            {
                vector3 = new Vector3(Mathf.Cos(angle * 0.01745329f), Mathf.Sin(angle * 0.01745329f));
                Gizmos.DrawLine(zero, vector3);
                zero = vector3;
            }
            Gizmos.DrawLine(zero, Vector3.zero);
        }
        PopMatrix();
    }

    public static void DrawCapsule(Vector3 capA, Vector3 capB, float radius)
    {
        if (radius == 0f)
        {
            Gizmos.DrawLine(capA, capB);
        }
        else
        {
            float num = Vector3.Distance(capA, capB);
            if (num == 0f)
            {
                DrawSphere(capA, radius);
            }
            else
            {
                PushMatrix();
                Matrix4x4 matrixx = Matrix4x4.TRS(capA, MagicFlat(capA, capB), new Vector3(radius, radius, radius)) * Gizmos.matrix;
                Gizmos.matrix = matrixx;
                float lengthOverRadius = num / radius;
                DrawFlatCapsule(lengthOverRadius);
                Gizmos.matrix = ninetyZ * matrixx;
                DrawFlatCapsule(lengthOverRadius);
                Gizmos.matrix = ninetyY * Gizmos.matrix;
                DrawFlatCircle();
                Gizmos.matrix = ninetyY * matrixx;
                DrawFlatCircle();
                PopMatrix();
            }
        }
    }

    public static void DrawCapsule(Vector3 center, float length, float radius, Vector3 heading)
    {
        length = Mathf.Max((float) (length - (radius * 2f)), (float) 0f);
        if (length == 0f)
        {
            DrawSphere(center, radius, heading);
        }
        heading.Normalize();
        length /= 2f;
        DrawCapsule(center - ((Vector3) (heading * length)), center + ((Vector3) (heading * length)), radius);
    }

    public static void DrawCircle(Vector3 origin, Vector3 axis, float radius)
    {
        PushMatrix();
        Gizmos.matrix = Matrix4x4.TRS(origin, Quaternion.LookRotation(axis), new Vector3(radius, radius, 1f)) * Gizmos.matrix;
        DrawFlatCircle();
        PopMatrix();
    }

    public static void DrawDotArc(Vector3 position, Transform transform, float length, float arc, float back)
    {
        Vector3 forward = transform.forward;
        DrawDotCone(position, forward, arc * length, arc, back);
        float angle = Mathf.Acos(arc) * 57.29578f;
        Vector3 up = transform.up;
        Vector3 right = transform.right;
        DrawAngle(position, forward, up, angle, length);
        DrawAngle(position, forward, up, -angle, length);
        DrawAngle(position, forward, right, angle, length);
        DrawAngle(position, forward, right, -angle, length);
    }

    public static void DrawDotCone(Vector3 position, Vector3 forward, float length, float arc)
    {
        DrawDotCone(position, forward, length, arc, 0f);
    }

    public static void DrawDotCone(Vector3 position, Vector3 forward, float length, float arc, float back)
    {
        if (arc == 1f)
        {
            Gizmos.DrawLine(position, position + ((Vector3) (forward * length)));
        }
        else
        {
            float z = Mathf.Ceil(length);
            if (z != 0f)
            {
                int num5;
                float num7;
                float num8;
                Matrix4x4 matrixx;
                float num2 = Mathf.Acos(arc);
                int num3 = Mathf.Abs((int) z);
                float num4 = length / z;
                float num6 = num4 * num2;
                if (back == 0f)
                {
                    z = num4;
                    num5 = 1;
                    num7 = num6;
                    num8 = 0f;
                }
                else
                {
                    z = 0f;
                    num5 = 0;
                    num7 = num2 * back;
                    num8 = num7;
                }
                PushMatrixMul(Matrix4x4.TRS(position, Quaternion.LookRotation(forward), Vector3.one), out matrixx);
                Vector3 from = new Vector3(num8, 0f, 0f);
                Vector3 to = new Vector3(num8 + (num2 * length), 0f, length);
                Gizmos.DrawLine(from, to);
                from.x = -from.x;
                to.x = -to.x;
                Gizmos.DrawLine(from, to);
                from.y = from.x;
                from.x = 0f;
                to.y = to.x;
                to.x = 0f;
                Gizmos.DrawLine(from, to);
                from.y = -from.y;
                to.y = -to.y;
                Gizmos.DrawLine(from, to);
                while (num5 <= num3)
                {
                    Gizmos.matrix = matrixx * Matrix4x4.TRS(new Vector3(0f, 0f, z), Quaternion.identity, new Vector3(num7, num7, 1f));
                    DrawFlatCircle();
                    num5++;
                    z += num4;
                    num7 += num6;
                }
                PopMatrix();
            }
        }
    }

    public static void DrawFlatCapEnd()
    {
        int index = 30;
        int num2 = 0;
        do
        {
            Gizmos.DrawLine(circleVerts[index], circleVerts[num2]);
            index = num2++;
        }
        while (num2 < 0x10);
    }

    public static void DrawFlatCapStart()
    {
        int index = 0;
        int num2 = 30;
        do
        {
            Gizmos.DrawLine(circleVerts[index], circleVerts[num2]);
            index = num2--;
        }
        while (num2 >= 0x10);
    }

    public static void DrawFlatCapsule(float lengthOverRadius)
    {
        DrawFlatCapStart();
        Gizmos.DrawLine(circleVerts[0x10], circleVerts[0x10] + new Vector3(lengthOverRadius, 0f));
        PushMatrix();
        Gizmos.matrix *= Matrix4x4.TRS(new Vector3(lengthOverRadius, 0f, 0f), Quaternion.identity, Vector3.one);
        DrawFlatCapEnd();
        Gizmos.DrawLine(circleVerts[0], circleVerts[0] - new Vector3(lengthOverRadius, 0f));
        PopMatrix();
    }

    public static void DrawFlatCircle()
    {
        int index = 30;
        int num2 = 0;
        do
        {
            Gizmos.DrawLine(circleVerts[index], circleVerts[num2]);
            index = num2++;
        }
        while (num2 < circleVerts.Length);
    }

    public static void DrawFlatCircle(float radius)
    {
        PushMatrixMul(Matrix4x4.Scale((Vector3) (Vector3.one * radius)));
        DrawFlatCircle();
        PopMatrix();
    }

    public static void DrawSphere(Vector3 center, float radius)
    {
        DrawSphere(center, radius, Quaternion.identity);
    }

    public static void DrawSphere(Vector3 center, float radius, Quaternion rotation)
    {
        PushMatrix();
        Matrix4x4 matrixx = Matrix4x4.TRS(center, rotation, new Vector3(radius, radius, radius)) * Gizmos.matrix;
        Gizmos.matrix = matrixx;
        DrawFlatCircle();
        Gizmos.matrix = ninetyX * matrixx;
        DrawFlatCircle();
        Gizmos.matrix = ninetyY * matrixx;
        DrawFlatCircle();
        PopMatrix();
    }

    public static void DrawSphere(Vector3 center, float radius, Vector3 forward)
    {
        DrawSphere(center, radius, Quaternion.LookRotation(forward));
    }

    private static Quaternion MagicFlat(Vector3 a, Vector3 b)
    {
        Vector3 vector;
        Vector3 vector2;
        MagicForward(a, b, out vector2, out vector);
        return Quaternion.LookRotation(vector, vector2);
    }

    private static void MagicForward(Vector3 a, Vector3 b, out Vector3 up, out Vector3 forward)
    {
        Vector3 lhs = a - b;
        lhs.Normalize();
        if ((lhs.y * lhs.y) > 0.8f)
        {
            up = Vector3.Cross(lhs, Vector3.forward);
            forward = Vector3.Cross(lhs, up);
        }
        else
        {
            forward = Vector3.Cross(lhs, Vector3.up);
            up = Vector3.Cross(lhs, forward);
        }
        up.Normalize();
        forward.Normalize();
    }

    public static void PopMatrix()
    {
        Gizmos.matrix = matStack[--stackPos];
    }

    public static void PopMatrix(out Matrix4x4 mat)
    {
        mat = matStack[--stackPos];
        Gizmos.matrix = mat;
    }

    public static void PushMatrix()
    {
        if (stackPos == matStack.Length)
        {
            Array.Resize<Matrix4x4>(ref matStack, stackPos + 8);
        }
        matStack[stackPos++] = Gizmos.matrix;
    }

    public static void PushMatrix(Matrix4x4 mat)
    {
        PushMatrix();
        Gizmos.matrix = mat;
    }

    public static void PushMatrixMul(Matrix4x4 mat)
    {
        PushMatrix();
        Gizmos.matrix = mat * matStack[stackPos - 1];
    }

    public static void PushMatrixMul(Matrix4x4 mat, out Matrix4x4 res)
    {
        PushMatrix();
        Gizmos.matrix = res = mat * matStack[stackPos - 1];
    }

    public static void ResetMatrixStack()
    {
        stackPos = 0;
    }
}

