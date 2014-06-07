using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct Matrix4x4Decomp
{
    public Vector3 r;
    public Vector3 u;
    public Vector3 f;
    public Vector3 t;
    public Vector4 s;
    public Matrix4x4Decomp(Matrix4x4 v)
    {
        this.r.x = v.m00;
        this.r.y = v.m01;
        this.r.z = v.m02;
        this.s.x = v.m03;
        this.u.x = v.m10;
        this.u.y = v.m11;
        this.u.z = v.m12;
        this.s.y = v.m13;
        this.f.x = v.m20;
        this.f.y = v.m21;
        this.f.z = v.m22;
        this.s.z = v.m23;
        this.t.x = v.m30;
        this.t.y = v.m31;
        this.t.z = v.m32;
        this.s.w = v.m33;
    }

    public Matrix4x4 m
    {
        get
        {
            Matrix4x4 matrixx;
            matrixx.m00 = this.r.x;
            matrixx.m01 = this.r.y;
            matrixx.m02 = this.r.z;
            matrixx.m03 = this.s.x;
            matrixx.m10 = this.u.x;
            matrixx.m11 = this.u.y;
            matrixx.m12 = this.u.z;
            matrixx.m13 = this.s.y;
            matrixx.m20 = this.f.x;
            matrixx.m21 = this.f.y;
            matrixx.m22 = this.f.z;
            matrixx.m23 = this.s.z;
            matrixx.m30 = this.t.x;
            matrixx.m31 = this.t.y;
            matrixx.m32 = this.t.z;
            matrixx.m33 = this.s.w;
            return matrixx;
        }
        set
        {
            this.r.x = value.m00;
            this.r.y = value.m01;
            this.r.z = value.m02;
            this.s.x = value.m03;
            this.u.x = value.m10;
            this.u.y = value.m11;
            this.u.z = value.m12;
            this.s.y = value.m13;
            this.f.x = value.m20;
            this.f.y = value.m21;
            this.f.z = value.m22;
            this.s.z = value.m23;
            this.t.x = value.m30;
            this.t.y = value.m31;
            this.t.z = value.m32;
            this.s.w = value.m33;
        }
    }
    public Quaternion q
    {
        get
        {
            return Quaternion.LookRotation(this.f, this.u);
        }
        set
        {
            Quaternion quaternion = value * Quaternion.Inverse(this.q);
            this.r = (Vector3) (quaternion * this.r);
            this.u = (Vector3) (quaternion * this.u);
            this.f = (Vector3) (quaternion * this.f);
        }
    }
    public Vector3 S
    {
        get
        {
            Vector3 vector;
            vector.x = this.s.x;
            vector.y = this.s.y;
            vector.z = this.s.z;
            return vector;
        }
        set
        {
            this.s.x = value.x;
            this.s.y = value.y;
            this.s.z = value.z;
        }
    }
    public float w
    {
        get
        {
            return this.s.w;
        }
        set
        {
            this.s.w = value;
        }
    }
}

