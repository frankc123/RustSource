using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct CharacterTransformInterpolatorData
{
    public Vector3 origin;
    public Angle2 eyesAngles;
}

