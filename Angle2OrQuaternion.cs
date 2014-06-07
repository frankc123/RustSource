using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct Angle2OrQuaternion
{
    internal Quaternion quat;
    public static implicit operator Angle2OrQuaternion(Angle2 v)
    {
        Angle2OrQuaternion quaternion;
        quaternion.quat = v.quat;
        return quaternion;
    }

    public static implicit operator Angle2OrQuaternion(Quaternion v)
    {
        Angle2OrQuaternion quaternion;
        quaternion.quat = v;
        return quaternion;
    }
}

