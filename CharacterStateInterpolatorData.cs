using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct CharacterStateInterpolatorData
{
    public Vector3 origin;
    public Angle2 eyesAngles;
    public CharacterStateFlags state;
    public static void Lerp(ref CharacterStateInterpolatorData a, ref CharacterStateInterpolatorData b, float t, out CharacterStateInterpolatorData result)
    {
        if (t == 0f)
        {
            result = a;
        }
        else if (t == 1f)
        {
            result = b;
        }
        else
        {
            float num = 1f - t;
            result.origin.x = (a.origin.x * num) + (b.origin.x * t);
            result.origin.y = (a.origin.y * num) + (b.origin.y * t);
            result.origin.z = (a.origin.z * num) + (b.origin.z * t);
            result.eyesAngles = new Angle2();
            result.eyesAngles.yaw = a.eyesAngles.yaw + (Mathf.DeltaAngle(a.eyesAngles.yaw, b.eyesAngles.yaw) * t);
            result.eyesAngles.pitch = Mathf.DeltaAngle(0f, a.eyesAngles.pitch + (Mathf.DeltaAngle(a.eyesAngles.pitch, b.eyesAngles.pitch) * t));
            if (t > 1f)
            {
                result.state = b.state;
            }
            else if (t < 0f)
            {
                result.state = a.state;
            }
            else
            {
                result.state = a.state;
                result.state.flags = (ushort) (result.state.flags | ((byte) (b.state.flags & 0x43)));
                if (result.state.grounded != b.state.grounded)
                {
                    result.state.grounded = false;
                }
            }
        }
    }
}

