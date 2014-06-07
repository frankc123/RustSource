using System;
using UnityEngine;

public static class Gizmos2
{
    public static void DrawCube(Vector3 center, Vector3 size)
    {
        Gizmos.DrawCube(center, size);
    }

    public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect)
    {
        Gizmos.DrawFrustum(center, fov, maxRange, minRange, aspect);
    }

    public static void DrawGUITexture(Rect screenRect, Texture texture)
    {
        Gizmos.DrawGUITexture(screenRect, texture);
    }

    public static void DrawGUITexture(Rect screenRect, Texture texture, Material mat)
    {
        Gizmos.DrawGUITexture(screenRect, texture, mat);
    }

    public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
    {
        DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder);
    }

    public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
    {
        DrawGUITexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
    }

    public static void DrawIcon(Vector3 center, string name)
    {
        Gizmos.DrawIcon(center, name);
    }

    public static void DrawIcon(Vector3 center, string name, bool allowScaling)
    {
        Gizmos.DrawIcon(center, name, allowScaling);
    }

    public static void DrawLine(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(from, to);
    }

    public static void DrawRay(Ray r)
    {
        Gizmos.DrawRay(r);
    }

    public static void DrawRay(Vector3 from, Vector3 direction)
    {
        Gizmos.DrawRay(from, direction);
    }

    public static void DrawSphere(Vector3 center, float radius)
    {
        Gizmos.DrawSphere(center, radius);
    }

    public static void DrawWireCapsule(Vector3 center, float radius, float height, int axis)
    {
        Vector3 up;
        Vector3 forward;
        Vector3 right;
        int num8 = axis % 3;
        switch ((num8 + 2))
        {
            case 0:
            case 3:
                up = Vector3.up;
                forward = Vector3.forward;
                right = Vector3.right;
                break;

            case 1:
            case 4:
                up = Vector3.forward;
                forward = Vector3.right;
                right = Vector3.up;
                break;

            case 2:
                up = Vector3.right;
                forward = Vector3.up;
                right = Vector3.forward;
                break;

            default:
                return;
        }
        Vector3 b = Vector3.one - ((Vector3) (forward * 2f));
        Vector3 vector5 = Vector3.one - ((Vector3) (right * 2f));
        if ((radius * 2f) >= height)
        {
            Gizmos.DrawWireSphere(center, radius);
        }
        else
        {
            Vector3 vector6 = center + ((Vector3) (up * ((height - (radius * 2f)) / 2f)));
            Vector3 vector7 = center - ((Vector3) (up * ((height - (radius * 2f)) / 2f)));
            Gizmos.DrawLine(vector6 + ((Vector3) (forward * radius)), vector7 + ((Vector3) (forward * radius)));
            Gizmos.DrawLine(vector6 + ((Vector3) (right * radius)), vector7 + ((Vector3) (right * radius)));
            Gizmos.DrawLine(vector6 - ((Vector3) (forward * radius)), vector7 - ((Vector3) (forward * radius)));
            Gizmos.DrawLine(vector6 - ((Vector3) (right * radius)), vector7 - ((Vector3) (right * radius)));
            for (int i = 0; i < 6; i++)
            {
                float f = (((float) i) / 12f) * 3.141593f;
                float num3 = ((i + 1f) / 12f) * 3.141593f;
                float num4 = Mathf.Cos(f) * radius;
                float num5 = Mathf.Sin(f) * radius;
                float num6 = Mathf.Cos(num3) * radius;
                float num7 = Mathf.Sin(num3) * radius;
                Vector3 a = (Vector3) ((up * num5) + (forward * num4));
                Vector3 vector9 = (Vector3) ((up * num7) + (forward * num6));
                Vector3 vector10 = (Vector3) ((up * num5) + (right * num4));
                Vector3 vector11 = (Vector3) ((up * num7) + (right * num6));
                Vector3 vector12 = (Vector3) ((forward * num5) + (right * num4));
                Vector3 vector13 = (Vector3) ((forward * num7) + (right * num6));
                Gizmos.DrawLine(vector6 + a, vector6 + vector9);
                Gizmos.DrawLine(vector6 + vector10, vector6 + vector11);
                Gizmos.DrawLine(vector7 - a, vector7 - vector9);
                Gizmos.DrawLine(vector7 - vector10, vector7 - vector11);
                Gizmos.DrawLine(vector6 + vector12, vector6 + vector13);
                Gizmos.DrawLine(vector6 - vector12, vector6 - vector13);
                Gizmos.DrawLine(vector7 + vector12, vector7 + vector13);
                Gizmos.DrawLine(vector7 - vector12, vector7 - vector13);
                a = Vector3.Scale(a, b);
                vector9 = Vector3.Scale(vector9, b);
                vector10 = Vector3.Scale(vector10, vector5);
                vector11 = Vector3.Scale(vector11, vector5);
                vector12 = Vector3.Scale(vector12, b);
                vector13 = Vector3.Scale(vector13, b);
                Gizmos.DrawLine(vector6 + a, vector6 + vector9);
                Gizmos.DrawLine(vector6 + vector10, vector6 + vector11);
                Gizmos.DrawLine(vector7 - a, vector7 - vector9);
                Gizmos.DrawLine(vector7 - vector10, vector7 - vector11);
                Gizmos.DrawLine(vector6 + vector12, vector6 + vector13);
                Gizmos.DrawLine(vector6 - vector12, vector6 - vector13);
                Gizmos.DrawLine(vector7 + vector12, vector7 + vector13);
                Gizmos.DrawLine(vector7 - vector12, vector7 - vector13);
            }
        }
    }

    public static void DrawWireCube(Vector3 center, Vector3 size)
    {
        Gizmos.DrawWireCube(center, size);
    }

    public static void DrawWireSphere(Vector3 center, float radius)
    {
        Gizmos.DrawWireSphere(center, radius);
    }

    public static Color color
    {
        get
        {
            return Gizmos.color;
        }
        set
        {
            Gizmos.color = value;
        }
    }

    public static Matrix4x4 matrix
    {
        get
        {
            return Gizmos.matrix;
        }
        set
        {
            Gizmos.matrix = value;
        }
    }
}

