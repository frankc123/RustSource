namespace NGUI.Meshing
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Explicit, Size=0x24)]
    public struct Vertex
    {
        [FieldOffset(0x20)]
        public float a;
        [FieldOffset(0x1c)]
        public float b;
        [FieldOffset(0x18)]
        public float g;
        [FieldOffset(20)]
        public float r;
        [FieldOffset(12)]
        public float u;
        [FieldOffset(0x10)]
        public float v;
        [FieldOffset(0)]
        public float x;
        [FieldOffset(4)]
        public float y;
        [FieldOffset(8)]
        public float z;

        public Color color
        {
            get
            {
                Color color;
                color.r = this.r;
                color.g = this.g;
                color.b = this.b;
                color.a = this.a;
                return color;
            }
            set
            {
                this.r = value.r;
                this.g = value.g;
                this.b = value.b;
                this.a = value.a;
            }
        }

        public Vector3 position
        {
            get
            {
                Vector3 vector;
                vector.x = this.x;
                vector.y = this.y;
                vector.z = this.z;
                return vector;
            }
            set
            {
                this.x = value.x;
                this.y = value.y;
                this.z = value.z;
            }
        }

        public Vector2 texcoord
        {
            get
            {
                Vector2 vector;
                vector.x = this.u;
                vector.y = this.v;
                return vector;
            }
            set
            {
                this.u = value.x;
                this.v = value.y;
            }
        }
    }
}

