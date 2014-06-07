using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct StructureComponentKey : IEquatable<StructureComponentKey>
{
    private const float kStepX = 2.5f;
    private const float kStepY = 4f;
    private const float kStepZ = 2.5f;
    private const float kInverseStepX = 0.4f;
    private const float kInverseStepY = 0.25f;
    private const float kInverseStepZ = 0.4f;
    public readonly int iX;
    public readonly int iY;
    public readonly int iZ;
    public readonly int hashCode;
    private StructureComponentKey(int iX, int iY, int iZ)
    {
        this.hashCode = ((((iX << 8) | ((iX >> 8) & 0xffffff)) ^ ((iY << 0x10) | ((iY >> 0x10) & 0xffff))) ^ ((iZ << 0x18) | ((iZ >> 0x18) & 0xff))) ^ ((iX * iY) * iZ);
        this.iX = iX;
        this.iY = iY;
        this.iZ = iZ;
    }

    public StructureComponentKey(float x, float y, float z) : this(ROUND(x, 0.4f), ROUND(y, 0.25f), ROUND(z, 0.4f))
    {
    }

    public StructureComponentKey(Vector3 v) : this(v.x, v.y, v.z)
    {
    }

    public float x
    {
        get
        {
            return (this.iX * 2.5f);
        }
    }
    public float y
    {
        get
        {
            return (this.iY * 4f);
        }
    }
    public float z
    {
        get
        {
            return (this.iZ * 2.5f);
        }
    }
    public Vector3 vector
    {
        get
        {
            Vector3 vector;
            vector.x = this.iX * 2.5f;
            vector.y = this.iY * 4f;
            vector.z = this.iZ * 2.5f;
            return vector;
        }
    }
    public static int ROUND(float v, float inverseStepSize)
    {
        if (v < 0f)
        {
            return -Mathf.RoundToInt(v * -inverseStepSize);
        }
        if (v > 0f)
        {
            return Mathf.RoundToInt(v * inverseStepSize);
        }
        return 0;
    }

    public override int GetHashCode()
    {
        return this.hashCode;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is StructureComponentKey))
        {
            return false;
        }
        StructureComponentKey key = (StructureComponentKey) obj;
        return (((key.iX == this.iX) && (key.iZ == this.iZ)) && (key.iY == this.iY));
    }

    public bool Equals(StructureComponentKey other)
    {
        return (((this.iX == other.iX) && (other.iZ == this.iZ)) && (other.iY == this.iY));
    }

    public override string ToString()
    {
        return string.Format("[{0},{1},{2}]", this.iX, this.iY, this.iZ);
    }

    public static bool operator ==(StructureComponentKey l, StructureComponentKey r)
    {
        return ((((l.hashCode == r.hashCode) && (l.iX == r.iX)) && (l.iY == r.iY)) && (l.iZ == r.iZ));
    }

    public static bool operator !=(StructureComponentKey l, StructureComponentKey r)
    {
        return ((((l.hashCode != r.hashCode) || (l.iX != r.iX)) || (l.iY != r.iY)) || (l.iZ != r.iZ));
    }

    public static explicit operator StructureComponentKey(Vector3 v)
    {
        return new StructureComponentKey(ROUND(v.x, 0.4f), ROUND(v.y, 0.25f), ROUND(v.z, 0.4f));
    }

    public static implicit operator Vector3(StructureComponentKey key)
    {
        Vector3 vector;
        vector.x = key.iX * 2.5f;
        vector.y = key.iY * 4f;
        vector.z = key.iZ * 2.5f;
        return vector;
    }
}

