namespace NGUI.Structures
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public const int size = 0x10;
        public const int halfSize = 8;
        public Vector2 a;
        public Vector2 d;
        public Vector2 b
        {
            get
            {
                Vector2 vector;
                vector.x = this.d.x;
                vector.y = this.a.y;
                return vector;
            }
            set
            {
                this.d.x = value.x;
                this.a.y = value.y;
            }
        }
        public Vector2 c
        {
            get
            {
                Vector2 vector;
                vector.x = this.a.x;
                vector.y = this.d.y;
                return vector;
            }
            set
            {
                this.a.x = value.x;
                this.d.y = value.y;
            }
        }
        public Vector2 dim
        {
            get
            {
                Vector2 vector;
                vector.x = this.d.x - this.a.x;
                vector.y = this.d.y - this.a.y;
                return vector;
            }
        }
        public Vector2 center
        {
            get
            {
                Vector2 vector;
                vector.x = this.a.x + ((this.d.x - this.a.x) * 0.5f);
                vector.y = this.a.y + ((this.d.y - this.a.y) * 0.5f);
                return vector;
            }
        }
        public float height
        {
            get
            {
                return (this.d.y - this.a.y);
            }
        }
        public float width
        {
            get
            {
                return (this.d.x - this.a.x);
            }
        }
        public Vector2 this[int i]
        {
            get
            {
                Vector2 vector;
                vector.x = ((i & 1) != 1) ? this.a.x : this.d.x;
                vector.y = ((i & 2) != 2) ? this.a.y : this.d.y;
                return vector;
            }
            set
            {
                if ((i & 1) == 1)
                {
                    this.d.x = value.x;
                }
                else
                {
                    this.a.x = value.x;
                }
                if ((i & 2) == 2)
                {
                    this.d.y = value.y;
                }
                else
                {
                    this.a.y = value.y;
                }
            }
        }
    }
}

