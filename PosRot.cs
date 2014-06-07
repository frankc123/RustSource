using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct PosRot
{
    public Vector3 position;
    public Quaternion rotation;
    public static void Lerp(ref PosRot a, ref PosRot b, float t, out PosRot v)
    {
        v.position = Vector3.Lerp(a.position, b.position, t);
        v.rotation = Quaternion.Slerp(a.rotation, b.rotation, t);
    }

    public static void Lerp(ref PosRot a, ref PosRot b, double t, out PosRot v)
    {
        Lerp(ref a, ref b, (float) t, out v);
    }
}

