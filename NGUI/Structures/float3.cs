namespace NGUI.Structures
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Explicit)]
    public struct float3
    {
        [FieldOffset(0)]
        public float x;
        [FieldOffset(0)]
        public Vector2 xy;
        [FieldOffset(0)]
        public Vector3 xyz;
        [FieldOffset(4)]
        public float y;
        [FieldOffset(4)]
        public Vector2 yz;
        [FieldOffset(8)]
        public float z;
    }
}

