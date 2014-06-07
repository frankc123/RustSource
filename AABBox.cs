using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct AABBox : IEquatable<AABBox>
{
    public const int kX = 2;
    public const int kY = 4;
    public const int kZ = 1;
    public const int kA = 0;
    public const int kB = 1;
    public const int kC = 2;
    public const int kD = 3;
    public const int kE = 4;
    public const int kF = 5;
    public const int kG = 6;
    public const int kH = 7;
    public Vector3 m;
    public Vector3 M;
    public AABBox(Vector3 min, Vector3 max)
    {
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public AABBox(ref Vector3 min, ref Vector3 max)
    {
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public AABBox(ref Vector3 center)
    {
        this.m.x = this.M.x = center.x;
        this.m.y = this.M.y = center.y;
        this.m.z = this.M.z = center.z;
    }

    public AABBox(Vector3 center)
    {
        this.m.x = this.M.x = center.x;
        this.m.y = this.M.y = center.y;
        this.m.z = this.M.z = center.z;
    }

    public AABBox(Bounds bounds) : this(bounds.min, bounds.max)
    {
    }

    public AABBox(ref Bounds bounds) : this(bounds.min, bounds.max)
    {
    }

    public Vector3 a
    {
        get
        {
            Vector3 vector;
            vector.x = this.m.x;
            vector.y = this.m.y;
            vector.z = this.m.z;
            return vector;
        }
    }
    public Vector3 b
    {
        get
        {
            Vector3 vector;
            vector.x = this.m.x;
            vector.y = this.m.y;
            vector.z = this.M.z;
            return vector;
        }
    }
    public Vector3 c
    {
        get
        {
            Vector3 vector;
            vector.x = this.M.x;
            vector.y = this.m.y;
            vector.z = this.m.z;
            return vector;
        }
    }
    public Vector3 d
    {
        get
        {
            Vector3 vector;
            vector.x = this.M.x;
            vector.y = this.m.y;
            vector.z = this.M.z;
            return vector;
        }
    }
    public Vector3 e
    {
        get
        {
            Vector3 vector;
            vector.x = this.m.x;
            vector.y = this.M.y;
            vector.z = this.m.z;
            return vector;
        }
    }
    public Vector3 f
    {
        get
        {
            Vector3 vector;
            vector.x = this.m.x;
            vector.y = this.M.y;
            vector.z = this.M.z;
            return vector;
        }
    }
    public Vector3 g
    {
        get
        {
            Vector3 vector;
            vector.x = this.M.x;
            vector.y = this.M.y;
            vector.z = this.m.z;
            return vector;
        }
    }
    public Vector3 h
    {
        get
        {
            Vector3 vector;
            vector.x = this.M.x;
            vector.y = this.M.y;
            vector.z = this.M.z;
            return vector;
        }
    }
    public Vector3 line00
    {
        get
        {
            return this.a;
        }
    }
    public Vector3 line01
    {
        get
        {
            return this.b;
        }
    }
    public Vector3 line10
    {
        get
        {
            return this.a;
        }
    }
    public Vector3 line11
    {
        get
        {
            return this.c;
        }
    }
    public Vector3 line20
    {
        get
        {
            return this.a;
        }
    }
    public Vector3 line21
    {
        get
        {
            return this.e;
        }
    }
    public Vector3 line30
    {
        get
        {
            return this.b;
        }
    }
    public Vector3 line31
    {
        get
        {
            return this.d;
        }
    }
    public Vector3 line40
    {
        get
        {
            return this.b;
        }
    }
    public Vector3 line41
    {
        get
        {
            return this.f;
        }
    }
    public Vector3 line50
    {
        get
        {
            return this.c;
        }
    }
    public Vector3 line51
    {
        get
        {
            return this.d;
        }
    }
    public Vector3 line60
    {
        get
        {
            return this.c;
        }
    }
    public Vector3 line61
    {
        get
        {
            return this.g;
        }
    }
    public Vector3 line70
    {
        get
        {
            return this.d;
        }
    }
    public Vector3 line71
    {
        get
        {
            return this.h;
        }
    }
    public Vector3 line80
    {
        get
        {
            return this.e;
        }
    }
    public Vector3 line81
    {
        get
        {
            return this.f;
        }
    }
    public Vector3 line90
    {
        get
        {
            return this.e;
        }
    }
    public Vector3 line91
    {
        get
        {
            return this.g;
        }
    }
    public Vector3 lineA0
    {
        get
        {
            return this.f;
        }
    }
    public Vector3 lineA1
    {
        get
        {
            return this.h;
        }
    }
    public Vector3 lineB0
    {
        get
        {
            return this.g;
        }
    }
    public Vector3 lineB1
    {
        get
        {
            return this.h;
        }
    }
    public void SetMinMax(ref Vector3 min, ref Vector3 max)
    {
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public void SetMinMax(ref Vector3 min, Vector3 max)
    {
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public void SetMinMax(Vector3 min, ref Vector3 max)
    {
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public void SetMinMax(Vector3 min, Vector3 max)
    {
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public void SetMinMax(Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public void SetMinMax(ref Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        if (min.x > max.x)
        {
            this.m.x = max.x;
            this.M.x = min.x;
        }
        else
        {
            this.m.x = min.x;
            this.M.x = max.x;
        }
        if (min.y > max.y)
        {
            this.m.y = max.y;
            this.M.y = min.y;
        }
        else
        {
            this.m.y = min.y;
            this.M.y = max.y;
        }
        if (min.z > max.z)
        {
            this.m.z = max.z;
            this.M.z = min.z;
        }
        else
        {
            this.m.z = min.z;
            this.M.z = max.z;
        }
    }

    public void EnsureMinMax()
    {
        if (this.m.x > this.M.x)
        {
            float x = this.m.x;
            this.m.x = this.M.x;
            this.M.x = x;
        }
        if (this.m.y > this.M.y)
        {
            float y = this.m.y;
            this.m.y = this.M.y;
            this.M.y = y;
        }
        if (this.m.z > this.M.z)
        {
            float z = this.m.z;
            this.m.z = this.M.z;
            this.M.z = z;
        }
    }

    public Vector3 min
    {
        get
        {
            Vector3 vector;
            vector.x = (this.M.x >= this.m.x) ? this.m.x : this.M.x;
            vector.y = (this.M.y >= this.m.y) ? this.m.y : this.M.y;
            vector.z = (this.M.z >= this.m.z) ? this.m.z : this.M.z;
            return vector;
        }
    }
    public Vector3 max
    {
        get
        {
            Vector3 vector;
            vector.x = (this.m.x <= this.M.x) ? this.M.x : this.m.x;
            vector.y = (this.m.y <= this.M.y) ? this.M.y : this.m.y;
            vector.z = (this.m.z <= this.M.z) ? this.M.z : this.m.z;
            return vector;
        }
    }
    public Vector3 size
    {
        get
        {
            Vector3 vector;
            vector.x = (this.M.x >= this.m.x) ? (this.M.x - this.m.x) : (this.m.x - this.M.x);
            vector.y = (this.M.y >= this.m.y) ? (this.M.y - this.m.y) : (this.m.y - this.M.y);
            vector.z = (this.M.z >= this.m.z) ? (this.M.z - this.m.z) : (this.m.z - this.M.z);
            return vector;
        }
        set
        {
            Vector3 vector;
            vector.x = this.m.x + ((this.M.x - this.m.x) * 0.5f);
            vector.y = this.m.y + ((this.M.y - this.m.y) * 0.5f);
            vector.z = this.m.z + ((this.M.z - this.m.z) * 0.5f);
            if (value.x < 0f)
            {
                value.x *= -0.5f;
            }
            else
            {
                value.x *= 0.5f;
            }
            this.m.x = vector.x - value.x;
            this.M.x = vector.x + value.x;
            if (value.y < 0f)
            {
                value.y *= -0.5f;
            }
            else
            {
                value.y *= 0.5f;
            }
            this.m.y = vector.y - value.y;
            this.M.y = vector.y + value.y;
            if (value.z < 0f)
            {
                value.z *= -0.5f;
            }
            else
            {
                value.z *= 0.5f;
            }
            this.m.z = vector.z - value.z;
            this.M.z = vector.z + value.z;
        }
    }
    public Vector3 center
    {
        get
        {
            Vector3 vector;
            vector.x = this.m.x + ((this.M.x - this.m.x) * 0.5f);
            vector.y = this.m.y + ((this.M.y - this.m.y) * 0.5f);
            vector.z = this.m.z + ((this.M.z - this.m.z) * 0.5f);
            return vector;
        }
        set
        {
            float num = value.x - (this.m.x + ((this.M.x - this.m.x) * 0.5f));
            this.m.x += num;
            this.M.x += num;
            num = value.y - (this.m.y + ((this.M.y - this.m.y) * 0.5f));
            this.m.y += num;
            this.M.y += num;
            num = value.z - (this.m.z + ((this.M.z - this.m.z) * 0.5f));
            this.m.z += num;
            this.M.z += num;
        }
    }
    public bool empty
    {
        get
        {
            return (((this.m.x == this.M.x) && (this.m.y == this.M.y)) && (this.m.z == this.M.z));
        }
    }
    public float volume
    {
        get
        {
            if (((this.M.x == this.m.x) || (this.M.y == this.m.y)) || (this.M.z == this.m.z))
            {
                return 0f;
            }
            if (this.M.x < this.m.x)
            {
                if (this.M.y < this.m.y)
                {
                    if (this.M.z < this.m.z)
                    {
                        return (((this.m.x - this.M.x) * (this.m.y - this.M.y)) * (this.m.z - this.M.z));
                    }
                    return (((this.m.x - this.M.x) * (this.m.y - this.M.y)) * (this.M.z - this.m.z));
                }
                if (this.M.z < this.m.z)
                {
                    return (((this.m.x - this.M.x) * (this.M.y - this.m.y)) * (this.m.z - this.M.z));
                }
                return (((this.m.x - this.M.x) * (this.M.y - this.m.y)) * (this.M.z - this.m.z));
            }
            if (this.M.y < this.m.y)
            {
                if (this.M.z < this.m.z)
                {
                    return (((this.M.x - this.m.x) * (this.m.y - this.M.y)) * (this.m.z - this.M.z));
                }
                return (((this.M.x - this.m.x) * (this.m.y - this.M.y)) * (this.M.z - this.m.z));
            }
            if (this.M.z < this.m.z)
            {
                return (((this.M.x - this.m.x) * (this.M.y - this.m.y)) * (this.m.z - this.M.z));
            }
            return (((this.M.x - this.m.x) * (this.M.y - this.m.y)) * (this.M.z - this.m.z));
        }
    }
    public float surfaceArea
    {
        get
        {
            Vector3 vector;
            vector.x = (this.M.x >= this.m.x) ? (this.M.x - this.m.x) : (this.m.x - this.M.x);
            vector.y = (this.M.y >= this.m.y) ? (this.M.y - this.m.y) : (this.m.y - this.M.y);
            vector.z = (this.M.z >= this.m.z) ? (this.M.z - this.m.z) : (this.m.z - this.M.z);
            return ((((2f * vector.x) * vector.y) + ((2f * vector.y) * vector.z)) + ((2f * vector.x) * vector.z));
        }
    }
    public void Encapsulate(ref Vector3 v)
    {
        if (v.x < this.m.x)
        {
            this.m.x = v.x;
        }
        if (v.x > this.M.x)
        {
            this.M.x = v.x;
        }
        if (v.y < this.m.y)
        {
            this.m.y = v.y;
        }
        if (v.y > this.M.y)
        {
            this.M.y = v.y;
        }
        if (v.z < this.m.z)
        {
            this.m.z = v.z;
        }
        if (v.z > this.M.z)
        {
            this.M.z = v.z;
        }
    }

    public void Encapsulate(Vector3 v)
    {
        if (v.x < this.m.x)
        {
            this.m.x = v.x;
        }
        if (v.x > this.M.x)
        {
            this.M.x = v.x;
        }
        if (v.y < this.m.y)
        {
            this.m.y = v.y;
        }
        if (v.y > this.M.y)
        {
            this.M.y = v.y;
        }
        if (v.z < this.m.z)
        {
            this.m.z = v.z;
        }
        if (v.z > this.M.z)
        {
            this.M.z = v.z;
        }
    }

    public void Encapsulate(ref AABBox v)
    {
        if (v.M.x < v.m.x)
        {
            if (v.M.x < this.m.x)
            {
                this.m.x = v.M.x;
            }
            if (v.m.x > this.M.x)
            {
                this.M.x = v.m.x;
            }
        }
        else
        {
            if (v.m.x < this.m.x)
            {
                this.m.x = v.m.x;
            }
            if (v.M.x > this.M.x)
            {
                this.M.x = v.M.x;
            }
        }
        if (v.M.y < v.m.y)
        {
            if (v.M.y < this.m.y)
            {
                this.m.y = v.M.y;
            }
            if (v.m.y > this.M.y)
            {
                this.M.y = v.m.y;
            }
        }
        else
        {
            if (v.m.y < this.m.y)
            {
                this.m.y = v.m.y;
            }
            if (v.M.y > this.M.y)
            {
                this.M.y = v.M.y;
            }
        }
        if (v.M.z < v.m.z)
        {
            if (v.M.z < this.m.z)
            {
                this.m.z = v.M.z;
            }
            if (v.m.z > this.M.z)
            {
                this.M.z = v.m.z;
            }
        }
        else
        {
            if (v.m.z < this.m.z)
            {
                this.m.z = v.m.z;
            }
            if (v.M.z > this.M.z)
            {
                this.M.z = v.M.z;
            }
        }
    }

    public void Encapsulate(AABBox v)
    {
        if (v.M.x < v.m.x)
        {
            if (v.M.x < this.m.x)
            {
                this.m.x = v.M.x;
            }
            if (v.m.x > this.M.x)
            {
                this.M.x = v.m.x;
            }
        }
        else
        {
            if (v.m.x < this.m.x)
            {
                this.m.x = v.m.x;
            }
            if (v.M.x > this.M.x)
            {
                this.M.x = v.M.x;
            }
        }
        if (v.M.y < v.m.y)
        {
            if (v.M.y < this.m.y)
            {
                this.m.y = v.M.y;
            }
            if (v.m.y > this.M.y)
            {
                this.M.y = v.m.y;
            }
        }
        else
        {
            if (v.m.y < this.m.y)
            {
                this.m.y = v.m.y;
            }
            if (v.M.y > this.M.y)
            {
                this.M.y = v.M.y;
            }
        }
        if (v.M.z < v.m.z)
        {
            if (v.M.z < this.m.z)
            {
                this.m.z = v.M.z;
            }
            if (v.m.z > this.M.z)
            {
                this.M.z = v.m.z;
            }
        }
        else
        {
            if (v.m.z < this.m.z)
            {
                this.m.z = v.m.z;
            }
            if (v.M.z > this.M.z)
            {
                this.M.z = v.M.z;
            }
        }
    }

    public void Encapsulate(ref Vector3 min, ref Vector3 max)
    {
        if (max.x < min.x)
        {
            if (max.x < this.m.x)
            {
                this.m.x = max.x;
            }
            if (min.x > this.M.x)
            {
                this.M.x = min.x;
            }
        }
        else
        {
            if (min.x < this.m.x)
            {
                this.m.x = min.x;
            }
            if (max.x > this.M.x)
            {
                this.M.x = max.x;
            }
        }
        if (max.y < min.y)
        {
            if (max.y < this.m.y)
            {
                this.m.y = max.y;
            }
            if (min.y > this.M.y)
            {
                this.M.y = min.y;
            }
        }
        else
        {
            if (min.y < this.m.y)
            {
                this.m.y = min.y;
            }
            if (max.y > this.M.y)
            {
                this.M.y = max.y;
            }
        }
        if (max.z < min.z)
        {
            if (max.z < this.m.z)
            {
                this.m.z = max.z;
            }
            if (min.z > this.M.z)
            {
                this.M.z = min.z;
            }
        }
        else
        {
            if (min.z < this.m.z)
            {
                this.m.z = min.z;
            }
            if (max.z > this.M.z)
            {
                this.M.z = max.z;
            }
        }
    }

    public void Encapsulate(Vector3 min, ref Vector3 max)
    {
        if (max.x < min.x)
        {
            if (max.x < this.m.x)
            {
                this.m.x = max.x;
            }
            if (min.x > this.M.x)
            {
                this.M.x = min.x;
            }
        }
        else
        {
            if (min.x < this.m.x)
            {
                this.m.x = min.x;
            }
            if (max.x > this.M.x)
            {
                this.M.x = max.x;
            }
        }
        if (max.y < min.y)
        {
            if (max.y < this.m.y)
            {
                this.m.y = max.y;
            }
            if (min.y > this.M.y)
            {
                this.M.y = min.y;
            }
        }
        else
        {
            if (min.y < this.m.y)
            {
                this.m.y = min.y;
            }
            if (max.y > this.M.y)
            {
                this.M.y = max.y;
            }
        }
        if (max.z < min.z)
        {
            if (max.z < this.m.z)
            {
                this.m.z = max.z;
            }
            if (min.z > this.M.z)
            {
                this.M.z = min.z;
            }
        }
        else
        {
            if (min.z < this.m.z)
            {
                this.m.z = min.z;
            }
            if (max.z > this.M.z)
            {
                this.M.z = max.z;
            }
        }
    }

    public void Encapsulate(ref Vector3 min, Vector3 max)
    {
        if (max.x < min.x)
        {
            if (max.x < this.m.x)
            {
                this.m.x = max.x;
            }
            if (min.x > this.M.x)
            {
                this.M.x = min.x;
            }
        }
        else
        {
            if (min.x < this.m.x)
            {
                this.m.x = min.x;
            }
            if (max.x > this.M.x)
            {
                this.M.x = max.x;
            }
        }
        if (max.y < min.y)
        {
            if (max.y < this.m.y)
            {
                this.m.y = max.y;
            }
            if (min.y > this.M.y)
            {
                this.M.y = min.y;
            }
        }
        else
        {
            if (min.y < this.m.y)
            {
                this.m.y = min.y;
            }
            if (max.y > this.M.y)
            {
                this.M.y = max.y;
            }
        }
        if (max.z < min.z)
        {
            if (max.z < this.m.z)
            {
                this.m.z = max.z;
            }
            if (min.z > this.M.z)
            {
                this.M.z = min.z;
            }
        }
        else
        {
            if (min.z < this.m.z)
            {
                this.m.z = min.z;
            }
            if (max.z > this.M.z)
            {
                this.M.z = max.z;
            }
        }
    }

    public void Encapsulate(Vector3 min, Vector3 max)
    {
        if (max.x < min.x)
        {
            if (max.x < this.m.x)
            {
                this.m.x = max.x;
            }
            if (min.x > this.M.x)
            {
                this.M.x = min.x;
            }
        }
        else
        {
            if (min.x < this.m.x)
            {
                this.m.x = min.x;
            }
            if (max.x > this.M.x)
            {
                this.M.x = max.x;
            }
        }
        if (max.y < min.y)
        {
            if (max.y < this.m.y)
            {
                this.m.y = max.y;
            }
            if (min.y > this.M.y)
            {
                this.M.y = min.y;
            }
        }
        else
        {
            if (min.y < this.m.y)
            {
                this.m.y = min.y;
            }
            if (max.y > this.M.y)
            {
                this.M.y = max.y;
            }
        }
        if (max.z < min.z)
        {
            if (max.z < this.m.z)
            {
                this.m.z = max.z;
            }
            if (min.z > this.M.z)
            {
                this.M.z = min.z;
            }
        }
        else
        {
            if (min.z < this.m.z)
            {
                this.m.z = min.z;
            }
            if (max.z > this.M.z)
            {
                this.M.z = max.z;
            }
        }
    }

    public void Encapsulate(ref Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        if (max.x < min.x)
        {
            if (max.x < this.m.x)
            {
                this.m.x = max.x;
            }
            if (min.x > this.M.x)
            {
                this.M.x = min.x;
            }
        }
        else
        {
            if (min.x < this.m.x)
            {
                this.m.x = min.x;
            }
            if (max.x > this.M.x)
            {
                this.M.x = max.x;
            }
        }
        if (max.y < min.y)
        {
            if (max.y < this.m.y)
            {
                this.m.y = max.y;
            }
            if (min.y > this.M.y)
            {
                this.M.y = min.y;
            }
        }
        else
        {
            if (min.y < this.m.y)
            {
                this.m.y = min.y;
            }
            if (max.y > this.M.y)
            {
                this.M.y = max.y;
            }
        }
        if (max.z < min.z)
        {
            if (max.z < this.m.z)
            {
                this.m.z = max.z;
            }
            if (min.z > this.M.z)
            {
                this.M.z = min.z;
            }
        }
        else
        {
            if (min.z < this.m.z)
            {
                this.m.z = min.z;
            }
            if (max.z > this.M.z)
            {
                this.M.z = max.z;
            }
        }
    }

    public void Encapsulate(Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        if (max.x < min.x)
        {
            if (max.x < this.m.x)
            {
                this.m.x = max.x;
            }
            if (min.x > this.M.x)
            {
                this.M.x = min.x;
            }
        }
        else
        {
            if (min.x < this.m.x)
            {
                this.m.x = min.x;
            }
            if (max.x > this.M.x)
            {
                this.M.x = max.x;
            }
        }
        if (max.y < min.y)
        {
            if (max.y < this.m.y)
            {
                this.m.y = max.y;
            }
            if (min.y > this.M.y)
            {
                this.M.y = min.y;
            }
        }
        else
        {
            if (min.y < this.m.y)
            {
                this.m.y = min.y;
            }
            if (max.y > this.M.y)
            {
                this.M.y = max.y;
            }
        }
        if (max.z < min.z)
        {
            if (max.z < this.m.z)
            {
                this.m.z = max.z;
            }
            if (min.z > this.M.z)
            {
                this.M.z = min.z;
            }
        }
        else
        {
            if (min.z < this.m.z)
            {
                this.m.z = min.z;
            }
            if (max.z > this.M.z)
            {
                this.M.z = max.z;
            }
        }
    }

    public bool Contains(ref Vector3 v)
    {
        return (((((this.m.x <= this.M.x) && (this.m.y <= this.M.y)) && ((this.m.z <= this.M.z) && (v.x >= this.m.x))) && (((v.y >= this.m.y) && (v.z >= this.m.z)) && ((v.x <= this.M.x) && (v.y <= this.M.y)))) && (v.z <= this.M.z));
    }

    public Vector3 this[int corner]
    {
        get
        {
            Vector3 vector;
            vector.x = ((corner & 2) != 2) ? this.m.x : this.M.x;
            vector.y = ((corner & 4) != 4) ? this.m.y : this.M.y;
            vector.z = ((corner & 1) != 1) ? this.m.z : this.M.z;
            return vector;
        }
    }
    public float this[int corner, int axis]
    {
        get
        {
            switch (axis)
            {
                case 0:
                    return (((corner & 2) != 2) ? this.m.x : this.M.x);

                case 1:
                    return (((corner & 4) != 4) ? this.m.y : this.M.y);

                case 2:
                    return (((corner & 1) != 1) ? this.m.z : this.M.z);
            }
            throw new ArgumentOutOfRangeException("axis", axis, "axis<0||axis>2");
        }
    }
    public BBox ToBBox()
    {
        BBox box;
        box.a.x = this.m.x;
        box.a.y = this.m.y;
        box.a.z = this.m.z;
        box.b.x = this.m.x;
        box.b.y = this.m.y;
        box.b.z = this.M.z;
        box.c.x = this.M.x;
        box.c.y = this.m.y;
        box.c.z = this.m.z;
        box.d.x = this.M.x;
        box.d.y = this.m.y;
        box.d.z = this.M.z;
        box.e.x = this.m.x;
        box.e.y = this.M.y;
        box.e.z = this.m.z;
        box.f.x = this.m.x;
        box.f.y = this.M.y;
        box.f.z = this.M.z;
        box.g.x = this.M.x;
        box.g.y = this.M.y;
        box.g.z = this.m.z;
        box.h.x = this.M.x;
        box.h.y = this.M.y;
        box.h.z = this.M.z;
        return box;
    }

    public void ToBBox(out BBox box)
    {
        box.a.x = this.m.x;
        box.a.y = this.m.y;
        box.a.z = this.m.z;
        box.b.x = this.m.x;
        box.b.y = this.m.y;
        box.b.z = this.M.z;
        box.c.x = this.M.x;
        box.c.y = this.m.y;
        box.c.z = this.m.z;
        box.d.x = this.M.x;
        box.d.y = this.m.y;
        box.d.z = this.M.z;
        box.e.x = this.m.x;
        box.e.y = this.M.y;
        box.e.z = this.m.z;
        box.f.x = this.m.x;
        box.f.y = this.M.y;
        box.f.z = this.M.z;
        box.g.x = this.M.x;
        box.g.y = this.M.y;
        box.g.z = this.m.z;
        box.h.x = this.M.x;
        box.h.y = this.M.y;
        box.h.z = this.M.z;
    }

    public void TransformedAABB3x4(ref Matrix4x4 t, out AABBox mM)
    {
        Vector3 vector;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        float num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        float num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        float num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        mM.m.x = mM.M.x = num;
        mM.m.y = mM.M.y = num2;
        mM.m.z = mM.M.z = num3;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        num = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        num2 = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        num3 = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
    }

    public void TransformedAABB3x4(ref Matrix4x4 t, out Bounds bounds)
    {
        AABBox box;
        Vector3 vector;
        Vector3 vector2;
        this.TransformedAABB3x4(ref t, out box);
        vector.x = box.M.x - box.m.x;
        vector2.x = box.m.x + (vector.x * 0.5f);
        vector.y = box.M.y - box.m.y;
        vector2.y = box.m.y + (vector.y * 0.5f);
        vector.z = box.M.z - box.m.z;
        vector2.z = box.m.z + (vector.z * 0.5f);
        bounds = new Bounds(vector2, vector);
    }

    public void ToBoxCorners3x4(ref Matrix4x4 t, out BBox box, out AABBox mM)
    {
        Vector3 vector;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        box.a.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.a.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.a.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        mM.m = box.a;
        mM.M = box.a;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        box.b.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.b.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.b.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (box.b.x < mM.m.x)
        {
            mM.m.x = box.b.x;
        }
        if (box.b.x > mM.M.x)
        {
            mM.M.x = box.b.x;
        }
        if (box.b.y < mM.m.y)
        {
            mM.m.y = box.b.y;
        }
        if (box.b.y > mM.M.y)
        {
            mM.M.y = box.b.y;
        }
        if (box.b.z < mM.m.z)
        {
            mM.m.z = box.b.z;
        }
        if (box.b.z > mM.M.z)
        {
            mM.M.z = box.b.z;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        box.c.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.c.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.c.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (box.c.x < mM.m.x)
        {
            mM.m.x = box.c.x;
        }
        if (box.c.x > mM.M.x)
        {
            mM.M.x = box.c.x;
        }
        if (box.c.y < mM.m.y)
        {
            mM.m.y = box.c.y;
        }
        if (box.c.y > mM.M.y)
        {
            mM.M.y = box.c.y;
        }
        if (box.c.z < mM.m.z)
        {
            mM.m.z = box.c.z;
        }
        if (box.c.z > mM.M.z)
        {
            mM.M.z = box.c.z;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        box.d.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.d.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.d.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (box.d.x < mM.m.x)
        {
            mM.m.x = box.d.x;
        }
        if (box.d.x > mM.M.x)
        {
            mM.M.x = box.d.x;
        }
        if (box.d.y < mM.m.y)
        {
            mM.m.y = box.d.y;
        }
        if (box.d.y > mM.M.y)
        {
            mM.M.y = box.d.y;
        }
        if (box.d.z < mM.m.z)
        {
            mM.m.z = box.d.z;
        }
        if (box.d.z > mM.M.z)
        {
            mM.M.z = box.d.z;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        box.e.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.e.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.e.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (box.e.x < mM.m.x)
        {
            mM.m.x = box.e.x;
        }
        if (box.e.x > mM.M.x)
        {
            mM.M.x = box.e.x;
        }
        if (box.e.y < mM.m.y)
        {
            mM.m.y = box.e.y;
        }
        if (box.e.y > mM.M.y)
        {
            mM.M.y = box.e.y;
        }
        if (box.e.z < mM.m.z)
        {
            mM.m.z = box.e.z;
        }
        if (box.e.z > mM.M.z)
        {
            mM.M.z = box.e.z;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        box.f.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.f.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.f.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (box.f.x < mM.m.x)
        {
            mM.m.x = box.f.x;
        }
        if (box.f.x > mM.M.x)
        {
            mM.M.x = box.f.x;
        }
        if (box.f.y < mM.m.y)
        {
            mM.m.y = box.f.y;
        }
        if (box.f.y > mM.M.y)
        {
            mM.M.y = box.f.y;
        }
        if (box.f.z < mM.m.z)
        {
            mM.m.z = box.f.z;
        }
        if (box.f.z > mM.M.z)
        {
            mM.M.z = box.f.z;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        box.g.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.g.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.g.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (box.g.x < mM.m.x)
        {
            mM.m.x = box.g.x;
        }
        if (box.g.x > mM.M.x)
        {
            mM.M.x = box.g.x;
        }
        if (box.g.y < mM.m.y)
        {
            mM.m.y = box.g.y;
        }
        if (box.g.y > mM.M.y)
        {
            mM.M.y = box.g.y;
        }
        if (box.g.z < mM.m.z)
        {
            mM.m.z = box.g.z;
        }
        if (box.g.z > mM.M.z)
        {
            mM.M.z = box.g.z;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        box.h.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.h.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.h.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        if (box.h.x < mM.m.x)
        {
            mM.m.x = box.h.x;
        }
        if (box.h.x > mM.M.x)
        {
            mM.M.x = box.h.x;
        }
        if (box.h.y < mM.m.y)
        {
            mM.m.y = box.h.y;
        }
        if (box.h.y > mM.M.y)
        {
            mM.M.y = box.h.y;
        }
        if (box.h.z < mM.m.z)
        {
            mM.m.z = box.h.z;
        }
        if (box.h.z > mM.M.z)
        {
            mM.M.z = box.h.z;
        }
    }

    public void TransformedAABB4x4(ref Matrix4x4 t, out AABBox mM)
    {
        Vector4 vector;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        float num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        float num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        float num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        mM.m.x = mM.M.x = num;
        mM.m.y = mM.M.y = num2;
        mM.m.z = mM.M.z = num3;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        num = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        num2 = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        num3 = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (num < mM.m.x)
        {
            mM.m.x = num;
        }
        if (num > mM.M.x)
        {
            mM.M.x = num;
        }
        if (num2 < mM.m.y)
        {
            mM.m.y = num2;
        }
        if (num2 > mM.M.y)
        {
            mM.M.y = num2;
        }
        if (num3 < mM.m.z)
        {
            mM.m.z = num3;
        }
        if (num3 > mM.M.z)
        {
            mM.M.z = num3;
        }
    }

    public void TransformedAABB4x4(ref Matrix4x4 t, out Bounds bounds)
    {
        AABBox box;
        Vector3 vector;
        Vector3 vector2;
        this.TransformedAABB4x4(ref t, out box);
        vector.x = box.M.x - box.m.x;
        vector2.x = box.m.x + (vector.x * 0.5f);
        vector.y = box.M.y - box.m.y;
        vector2.y = box.m.y + (vector.y * 0.5f);
        vector.z = box.M.z - box.m.z;
        vector2.z = box.m.z + (vector.z * 0.5f);
        bounds = new Bounds(vector2, vector);
    }

    public void ToBoxCorners4x4(ref Matrix4x4 t, out BBox box, out AABBox mM)
    {
        Vector4 vector;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.a.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.a.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.a.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        mM.m = box.a;
        mM.M = box.a;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.b.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.b.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.b.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (box.b.x < mM.m.x)
        {
            mM.m.x = box.b.x;
        }
        if (box.b.x > mM.M.x)
        {
            mM.M.x = box.b.x;
        }
        if (box.b.y < mM.m.y)
        {
            mM.m.y = box.b.y;
        }
        if (box.b.y > mM.M.y)
        {
            mM.M.y = box.b.y;
        }
        if (box.b.z < mM.m.z)
        {
            mM.m.z = box.b.z;
        }
        if (box.b.z > mM.M.z)
        {
            mM.M.z = box.b.z;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.c.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.c.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.c.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (box.c.x < mM.m.x)
        {
            mM.m.x = box.c.x;
        }
        if (box.c.x > mM.M.x)
        {
            mM.M.x = box.c.x;
        }
        if (box.c.y < mM.m.y)
        {
            mM.m.y = box.c.y;
        }
        if (box.c.y > mM.M.y)
        {
            mM.M.y = box.c.y;
        }
        if (box.c.z < mM.m.z)
        {
            mM.m.z = box.c.z;
        }
        if (box.c.z > mM.M.z)
        {
            mM.M.z = box.c.z;
        }
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.d.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.d.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.d.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (box.d.x < mM.m.x)
        {
            mM.m.x = box.d.x;
        }
        if (box.d.x > mM.M.x)
        {
            mM.M.x = box.d.x;
        }
        if (box.d.y < mM.m.y)
        {
            mM.m.y = box.d.y;
        }
        if (box.d.y > mM.M.y)
        {
            mM.M.y = box.d.y;
        }
        if (box.d.z < mM.m.z)
        {
            mM.m.z = box.d.z;
        }
        if (box.d.z > mM.M.z)
        {
            mM.M.z = box.d.z;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.e.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.e.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.e.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (box.e.x < mM.m.x)
        {
            mM.m.x = box.e.x;
        }
        if (box.e.x > mM.M.x)
        {
            mM.M.x = box.e.x;
        }
        if (box.e.y < mM.m.y)
        {
            mM.m.y = box.e.y;
        }
        if (box.e.y > mM.M.y)
        {
            mM.M.y = box.e.y;
        }
        if (box.e.z < mM.m.z)
        {
            mM.m.z = box.e.z;
        }
        if (box.e.z > mM.M.z)
        {
            mM.M.z = box.e.z;
        }
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.f.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.f.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.f.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (box.f.x < mM.m.x)
        {
            mM.m.x = box.f.x;
        }
        if (box.f.x > mM.M.x)
        {
            mM.M.x = box.f.x;
        }
        if (box.f.y < mM.m.y)
        {
            mM.m.y = box.f.y;
        }
        if (box.f.y > mM.M.y)
        {
            mM.M.y = box.f.y;
        }
        if (box.f.z < mM.m.z)
        {
            mM.m.z = box.f.z;
        }
        if (box.f.z > mM.M.z)
        {
            mM.M.z = box.f.z;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.g.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.g.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.g.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (box.g.x < mM.m.x)
        {
            mM.m.x = box.g.x;
        }
        if (box.g.x > mM.M.x)
        {
            mM.M.x = box.g.x;
        }
        if (box.g.y < mM.m.y)
        {
            mM.m.y = box.g.y;
        }
        if (box.g.y > mM.M.y)
        {
            mM.M.y = box.g.y;
        }
        if (box.g.z < mM.m.z)
        {
            mM.m.z = box.g.z;
        }
        if (box.g.z > mM.M.z)
        {
            mM.M.z = box.g.z;
        }
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.h.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.h.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.h.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        if (box.h.x < mM.m.x)
        {
            mM.m.x = box.h.x;
        }
        if (box.h.x > mM.M.x)
        {
            mM.M.x = box.h.x;
        }
        if (box.h.y < mM.m.y)
        {
            mM.m.y = box.h.y;
        }
        if (box.h.y > mM.M.y)
        {
            mM.M.y = box.h.y;
        }
        if (box.h.z < mM.m.z)
        {
            mM.m.z = box.h.z;
        }
        if (box.h.z > mM.M.z)
        {
            mM.M.z = box.h.z;
        }
    }

    public void ToBoxCorners3x4(ref Matrix4x4 t, out BBox box)
    {
        Vector3 vector;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        box.a.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.a.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.a.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        box.b.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.b.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.b.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        box.c.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.c.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.c.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        box.d.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.d.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.d.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        box.e.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.e.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.e.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        box.f.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.f.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.f.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        box.g.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.g.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.g.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        box.h.x = (((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03;
        box.h.y = (((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13;
        box.h.z = (((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23;
    }

    public void ToBoxCorners4x4(ref Matrix4x4 t, out BBox box)
    {
        Vector4 vector;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.a.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.a.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.a.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        vector.x = this.m.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.b.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.b.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.b.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.c.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.c.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.c.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        vector.x = this.M.x;
        vector.y = this.m.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.d.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.d.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.d.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.e.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.e.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.e.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        vector.x = this.m.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.f.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.f.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.f.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.m.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.g.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.g.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.g.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
        vector.x = this.M.x;
        vector.y = this.M.y;
        vector.z = this.M.z;
        vector.w = 1f / ((((t.m30 * vector.x) + (t.m31 * vector.y)) + (t.m32 * vector.z)) + t.m33);
        box.h.x = ((((t.m00 * vector.x) + (t.m01 * vector.y)) + (t.m02 * vector.z)) + t.m03) * vector.w;
        box.h.y = ((((t.m10 * vector.x) + (t.m11 * vector.y)) + (t.m12 * vector.z)) + t.m13) * vector.w;
        box.h.z = ((((t.m20 * vector.x) + (t.m21 * vector.y)) + (t.m22 * vector.z)) + t.m23) * vector.w;
    }

    public static void Transform3x4(ref AABBox src, ref Matrix4x4 transform, out AABBox dst)
    {
        src.TransformedAABB3x4(ref transform, out dst);
    }

    public static void Transform4x4(ref AABBox src, ref Matrix4x4 transform, out AABBox dst)
    {
        src.TransformedAABB4x4(ref transform, out dst);
    }

    public static void Transform3x4(ref AABBox src, ref Matrix4x4 transform, out Bounds dst)
    {
        src.TransformedAABB3x4(ref transform, out dst);
    }

    public static void Transform4x4(ref AABBox src, ref Matrix4x4 transform, out Bounds dst)
    {
        src.TransformedAABB4x4(ref transform, out dst);
    }

    public static void Transform3x4(ref Bounds boundsSrc, ref Matrix4x4 transform, out AABBox dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform3x4(ref src, ref transform, out dst);
    }

    public static void Transform4x4(ref Bounds boundsSrc, ref Matrix4x4 transform, out AABBox dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform4x4(ref src, ref transform, out dst);
    }

    public static void Transform3x4(ref Bounds boundsSrc, ref Matrix4x4 transform, out Bounds dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform3x4(ref src, ref transform, out dst);
    }

    public static void Transform4x4(ref Bounds boundsSrc, ref Matrix4x4 transform, out Bounds dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform4x4(ref src, ref transform, out dst);
    }

    public static void Transform3x4(AABBox src, ref Matrix4x4 transform, out AABBox dst)
    {
        src.TransformedAABB3x4(ref transform, out dst);
    }

    public static void Transform4x4(AABBox src, ref Matrix4x4 transform, out AABBox dst)
    {
        src.TransformedAABB4x4(ref transform, out dst);
    }

    public static void Transform3x4(AABBox src, ref Matrix4x4 transform, out Bounds dst)
    {
        src.TransformedAABB3x4(ref transform, out dst);
    }

    public static void Transform4x4(AABBox src, ref Matrix4x4 transform, out Bounds dst)
    {
        src.TransformedAABB4x4(ref transform, out dst);
    }

    public static void Transform3x4(Bounds boundsSrc, ref Matrix4x4 transform, out AABBox dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform3x4(ref src, ref transform, out dst);
    }

    public static void Transform4x4(Bounds boundsSrc, ref Matrix4x4 transform, out AABBox dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform4x4(ref src, ref transform, out dst);
    }

    public static void Transform3x4(Bounds boundsSrc, ref Matrix4x4 transform, out Bounds dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform3x4(ref src, ref transform, out dst);
    }

    public static void Transform4x4(Bounds boundsSrc, ref Matrix4x4 transform, out Bounds dst)
    {
        AABBox src = new AABBox(boundsSrc.min, boundsSrc.max);
        Transform4x4(ref src, ref transform, out dst);
    }

    public static AABBox CenterAndSize(Vector3 center, Vector3 size)
    {
        center.x -= size.x * 0.5f;
        center.y -= size.y * 0.5f;
        center.z -= size.z * 0.5f;
        size.x = center.x + size.x;
        size.y = center.y + size.y;
        size.z = center.z + size.z;
        return new AABBox(ref center, ref size);
    }

    public override bool Equals(object obj)
    {
        return ((obj is AABBox) && this.Equals((AABBox) obj));
    }

    public override int GetHashCode()
    {
        float num3 = (this.m.x + this.M.x) * 0.5f;
        float num4 = (this.m.y + this.M.y) * 0.5f;
        int num = num3.GetHashCode() ^ num4.GetHashCode();
        float num5 = (this.m.x + this.M.x) - (this.m.y + this.M.y);
        int num2 = (num5.GetHashCode() & 0x7fffffff) % 0x20;
        return ((num << num2) ^ (num >> num2));
    }

    public bool Equals(AABBox other)
    {
        return ((((this.m.x.Equals(other.m.x) && this.m.y.Equals(other.m.y)) && (this.m.z.Equals(other.m.z) && this.M.x.Equals(other.M.x))) && this.M.y.Equals(other.M.y)) && this.M.z.Equals(other.M.z));
    }

    public bool Equals(ref AABBox other)
    {
        return ((((this.m.x.Equals(other.m.x) && this.m.y.Equals(other.m.y)) && (this.m.z.Equals(other.m.z) && this.M.x.Equals(other.M.x))) && this.M.y.Equals(other.M.y)) && this.M.z.Equals(other.M.z));
    }

    public static explicit operator Bounds(AABBox mM)
    {
        Vector3 vector;
        Vector3 vector2;
        vector.x = mM.M.x - mM.m.x;
        vector2.x = mM.m.x + (vector.x * 0.5f);
        if (vector.x < 0f)
        {
            vector.x = -vector.x;
        }
        vector.y = mM.M.y - mM.m.y;
        vector2.y = mM.m.y + (vector.y * 0.5f);
        if (vector.y < 0f)
        {
            vector.y = -vector.y;
        }
        vector.z = mM.M.z - mM.m.z;
        vector2.z = mM.m.z + (vector.z * 0.5f);
        if (vector.z < 0f)
        {
            vector.z = -vector.z;
        }
        return new Bounds(vector2, vector);
    }

    public static explicit operator AABBox(Bounds bounds)
    {
        AABBox box;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        if (min.x > max.x)
        {
            box.M.x = min.x;
            box.m.x = max.x;
        }
        else
        {
            box.M.x = max.x;
            box.m.x = min.x;
        }
        if (min.y > max.y)
        {
            box.M.y = min.y;
            box.m.y = max.y;
        }
        else
        {
            box.M.y = max.y;
            box.m.y = min.y;
        }
        if (min.z > max.z)
        {
            box.M.z = min.z;
            box.m.z = max.z;
            return box;
        }
        box.M.z = max.z;
        box.m.z = min.z;
        return box;
    }

    public static explicit operator BBox(AABBox mM)
    {
        BBox box;
        box.a.x = mM.m.x;
        box.a.y = mM.m.y;
        box.a.z = mM.m.z;
        box.b.x = mM.m.x;
        box.b.y = mM.m.y;
        box.b.z = mM.M.z;
        box.c.x = mM.M.x;
        box.c.y = mM.m.y;
        box.c.z = mM.m.z;
        box.d.x = mM.M.x;
        box.d.y = mM.m.y;
        box.d.z = mM.M.z;
        box.e.x = mM.m.x;
        box.e.y = mM.M.y;
        box.e.z = mM.m.z;
        box.f.x = mM.m.x;
        box.f.y = mM.M.y;
        box.f.z = mM.M.z;
        box.g.x = mM.M.x;
        box.g.y = mM.M.y;
        box.g.z = mM.m.z;
        box.h.x = mM.M.x;
        box.h.y = mM.M.y;
        box.h.z = mM.M.z;
        return box;
    }

    public static explicit operator AABBox(BBox box)
    {
        AABBox box2;
        box2.m.x = box2.M.x = box.a.x;
        box2.m.y = box2.M.y = box.a.y;
        box2.m.z = box2.M.z = box.a.z;
        if (box.b.x < box2.m.x)
        {
            box2.m.x = box.b.x;
        }
        if (box.b.x > box2.M.x)
        {
            box2.M.x = box.b.x;
        }
        if (box.b.y < box2.m.y)
        {
            box2.m.y = box.b.y;
        }
        if (box.b.y > box2.M.y)
        {
            box2.M.y = box.b.y;
        }
        if (box.b.z < box2.m.z)
        {
            box2.m.z = box.b.z;
        }
        if (box.b.z > box2.M.z)
        {
            box2.M.z = box.b.z;
        }
        if (box.c.x < box2.m.x)
        {
            box2.m.x = box.c.x;
        }
        if (box.c.x > box2.M.x)
        {
            box2.M.x = box.c.x;
        }
        if (box.c.y < box2.m.y)
        {
            box2.m.y = box.c.y;
        }
        if (box.c.y > box2.M.y)
        {
            box2.M.y = box.c.y;
        }
        if (box.c.z < box2.m.z)
        {
            box2.m.z = box.c.z;
        }
        if (box.c.z > box2.M.z)
        {
            box2.M.z = box.c.z;
        }
        if (box.d.x < box2.m.x)
        {
            box2.m.x = box.d.x;
        }
        if (box.d.x > box2.M.x)
        {
            box2.M.x = box.d.x;
        }
        if (box.d.y < box2.m.y)
        {
            box2.m.y = box.d.y;
        }
        if (box.d.y > box2.M.y)
        {
            box2.M.y = box.d.y;
        }
        if (box.d.z < box2.m.z)
        {
            box2.m.z = box.d.z;
        }
        if (box.d.z > box2.M.z)
        {
            box2.M.z = box.d.z;
        }
        if (box.e.x < box2.m.x)
        {
            box2.m.x = box.e.x;
        }
        if (box.e.x > box2.M.x)
        {
            box2.M.x = box.e.x;
        }
        if (box.e.y < box2.m.y)
        {
            box2.m.y = box.e.y;
        }
        if (box.e.y > box2.M.y)
        {
            box2.M.y = box.e.y;
        }
        if (box.e.z < box2.m.z)
        {
            box2.m.z = box.e.z;
        }
        if (box.e.z > box2.M.z)
        {
            box2.M.z = box.e.z;
        }
        if (box.f.x < box2.m.x)
        {
            box2.m.x = box.f.x;
        }
        if (box.f.x > box2.M.x)
        {
            box2.M.x = box.f.x;
        }
        if (box.f.y < box2.m.y)
        {
            box2.m.y = box.f.y;
        }
        if (box.f.y > box2.M.y)
        {
            box2.M.y = box.f.y;
        }
        if (box.f.z < box2.m.z)
        {
            box2.m.z = box.f.z;
        }
        if (box.f.z > box2.M.z)
        {
            box2.M.z = box.f.z;
        }
        if (box.g.x < box2.m.x)
        {
            box2.m.x = box.g.x;
        }
        if (box.g.x > box2.M.x)
        {
            box2.M.x = box.g.x;
        }
        if (box.g.y < box2.m.y)
        {
            box2.m.y = box.g.y;
        }
        if (box.g.y > box2.M.y)
        {
            box2.M.y = box.g.y;
        }
        if (box.g.z < box2.m.z)
        {
            box2.m.z = box.g.z;
        }
        if (box.g.z > box2.M.z)
        {
            box2.M.z = box.g.z;
        }
        if (box.h.x < box2.m.x)
        {
            box2.m.x = box.h.x;
        }
        if (box.h.x > box2.M.x)
        {
            box2.M.x = box.h.x;
        }
        if (box.h.y < box2.m.y)
        {
            box2.m.y = box.h.y;
        }
        if (box.h.y > box2.M.y)
        {
            box2.M.y = box.h.y;
        }
        if (box.h.z < box2.m.z)
        {
            box2.m.z = box.h.z;
        }
        if (box.h.z > box2.M.z)
        {
            box2.M.z = box.h.z;
        }
        return box2;
    }
}

