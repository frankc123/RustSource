using Facepunch.Precision;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MatrixHelper : MonoBehaviour
{
    public static bool InvertMatrix(ref Matrix4x4 m, out Matrix4x4 o)
    {
        Vector4 vector;
        Vector4 vector2;
        Vector4 vector3;
        Vector4 vector4;
        Vector4 vector5;
        Vector4 vector6;
        Vector4 vector7;
        Vector4 vector8;
        Vector4 vector9;
        vector.x = m.m00;
        vector.y = m.m01;
        vector.z = m.m02;
        vector.w = m.m03;
        vector2.x = m.m10;
        vector2.y = m.m11;
        vector2.z = m.m12;
        vector2.w = m.m13;
        vector3.x = m.m20;
        vector3.y = m.m21;
        vector3.z = m.m22;
        vector3.w = m.m23;
        vector4.x = m.m30;
        vector4.y = m.m31;
        vector4.z = m.m32;
        vector4.w = m.m33;
        vector6.x = vector7.y = vector8.z = vector9.w = 1f;
        vector6.y = vector6.z = vector6.w = vector7.x = vector7.z = vector7.w = vector8.x = vector8.y = vector8.w = vector9.x = vector9.y = vector9.z = 0f;
        if ((vector4.x * vector4.x) > (vector3.x * vector3.x))
        {
            vector5 = vector4;
            vector4 = vector3;
            vector3 = vector5;
            vector5 = vector9;
            vector9 = vector8;
            vector8 = vector5;
        }
        if ((vector3.x * vector3.x) > (vector2.x * vector2.x))
        {
            vector5 = vector2;
            vector2 = vector3;
            vector3 = vector5;
            vector5 = vector7;
            vector7 = vector8;
            vector8 = vector5;
        }
        if ((vector2.x * vector3.x) > (vector.x * vector.x))
        {
            vector5 = vector2;
            vector2 = vector;
            vector = vector5;
            vector5 = vector6;
            vector6 = vector7;
            vector7 = vector5;
        }
        if (vector.x == 0.0)
        {
            o = new Matrix4x4();
            return false;
        }
        float z = vector2.x / vector.x;
        float w = vector3.x / vector.x;
        float num3 = vector4.x / vector.x;
        vector2.y -= z * vector.y;
        vector3.y -= w * vector.y;
        vector4.y -= num3 * vector.y;
        vector2.z -= z * vector.z;
        vector3.z -= w * vector.z;
        vector4.z -= num3 * vector.z;
        vector2.w -= z * vector.w;
        vector3.w -= w * vector.w;
        vector4.w -= num3 * vector.w;
        if (vector6.x != 0.0)
        {
            vector7.x -= z * vector6.x;
            vector8.x -= w * vector6.x;
            vector9.x -= num3 * vector6.x;
        }
        if (vector6.y != 0.0)
        {
            vector7.y -= z * vector6.y;
            vector8.y -= w * vector6.y;
            vector9.y -= num3 * vector6.y;
        }
        if (vector6.z != 0.0)
        {
            vector7.z -= z * vector6.z;
            vector8.z -= w * vector6.z;
            vector9.z -= num3 * vector6.z;
        }
        if (vector6.w != 0.0)
        {
            vector7.w -= z * vector6.w;
            vector8.w -= w * vector6.w;
            vector9.w -= num3 * vector6.w;
        }
        if ((vector4.y * vector4.y) > (vector3.y * vector3.y))
        {
            vector5 = vector4;
            vector4 = vector3;
            vector3 = vector5;
            vector5 = vector9;
            vector9 = vector8;
            vector8 = vector5;
        }
        if ((vector3.y * vector3.y) > (vector2.y * vector2.y))
        {
            vector5 = vector2;
            vector2 = vector3;
            vector3 = vector5;
            vector5 = vector7;
            vector7 = vector8;
            vector8 = vector5;
        }
        if (vector2.y == 0.0)
        {
            o = new Matrix4x4();
            return false;
        }
        w = vector3.y / vector2.y;
        num3 = vector4.y / vector2.y;
        vector3.z -= w * vector2.z;
        vector4.z -= num3 * vector2.z;
        vector3.w -= w * vector2.w;
        vector4.w -= num3 * vector2.w;
        if (vector7.x != 0.0)
        {
            vector8.x -= w * vector7.x;
            vector9.x -= num3 * vector7.x;
        }
        if (vector7.y != 0.0)
        {
            vector8.y -= w * vector7.y;
            vector9.y -= num3 * vector7.y;
        }
        if (vector7.z != 0.0)
        {
            vector8.z -= w * vector7.z;
            vector9.z -= num3 * vector7.z;
        }
        if (vector7.w != 0.0)
        {
            vector8.w -= w * vector7.w;
            vector9.w -= num3 * vector7.w;
        }
        if ((vector4.y * vector4.y) > (vector3.y * vector3.y))
        {
            vector5 = vector4;
            vector4 = vector3;
            vector3 = vector5;
            vector5 = vector9;
            vector9 = vector8;
            vector8 = vector5;
        }
        if (vector3.z == 0.0)
        {
            o = new Matrix4x4();
            return false;
        }
        num3 = vector4.z / vector3.z;
        vector4.w -= num3 * vector3.w;
        vector9.x -= num3 * vector8.x;
        vector9.y -= num3 * vector8.y;
        vector9.z -= num3 * vector8.z;
        vector9.w -= num3 * vector8.w;
        if (vector4.w == 0.0)
        {
            o = new Matrix4x4();
            return false;
        }
        float num4 = 1f / vector4.w;
        vector9.x *= num4;
        vector9.y *= num4;
        vector9.z *= num4;
        vector9.w *= num4;
        w = vector3.w;
        num4 = 1f / vector3.z;
        vector8.x = num4 * (vector8.x - (vector9.x * w));
        vector8.y = num4 * (vector8.y - (vector9.y * w));
        vector8.z = num4 * (vector8.z - (vector9.z * w));
        vector8.w = num4 * (vector8.w - (vector9.w * w));
        z = vector2.w;
        vector7.x -= vector9.x * z;
        vector7.y -= vector9.y * z;
        vector7.z -= vector9.z * z;
        vector7.w -= vector9.w * z;
        float y = vector.w;
        vector6.x -= vector9.x * y;
        vector6.y -= vector9.y * y;
        vector6.z -= vector9.z * y;
        vector6.w -= vector9.w * y;
        z = vector2.z;
        num4 = 1f / vector2.y;
        vector7.x = num4 * (vector7.x - (vector8.x * z));
        vector7.y = num4 * (vector7.y - (vector8.y * z));
        vector7.z = num4 * (vector7.z - (vector8.z * z));
        vector7.w = num4 * (vector7.w - (vector8.w * z));
        y = vector.z;
        vector6.x -= vector8.x * y;
        vector6.y -= vector8.y * y;
        vector6.z -= vector8.z * y;
        vector6.w -= vector8.w * y;
        y = vector.y;
        num4 = 1f / vector.x;
        vector6.x = num4 * (vector6.x - (vector7.x * y));
        vector6.y = num4 * (vector6.y - (vector7.y * y));
        vector6.z = num4 * (vector6.z - (vector7.z * y));
        vector6.w = num4 * (vector6.w - (vector7.w * y));
        o.m00 = vector6.x;
        o.m01 = vector6.y;
        o.m02 = vector6.z;
        o.m03 = vector6.w;
        o.m10 = vector7.x;
        o.m11 = vector7.y;
        o.m12 = vector7.z;
        o.m13 = vector7.w;
        o.m20 = vector8.x;
        o.m21 = vector8.y;
        o.m22 = vector8.z;
        o.m23 = vector8.w;
        o.m30 = vector9.x;
        o.m31 = vector9.y;
        o.m32 = vector9.z;
        o.m33 = vector9.w;
        return true;
    }

    public static void MultiplyVector4(out Vector4 resultvector, ref Matrix4x4 matrix, ref Vector4 pvector)
    {
        resultvector.x = (((matrix[0] * pvector[0]) + (matrix[4] * pvector[1])) + (matrix[8] * pvector[2])) + (matrix[12] * pvector[3]);
        resultvector.y = (((matrix[1] * pvector[0]) + (matrix[5] * pvector[1])) + (matrix[9] * pvector[2])) + (matrix[13] * pvector[3]);
        resultvector.z = (((matrix[2] * pvector[0]) + (matrix[6] * pvector[1])) + (matrix[10] * pvector[2])) + (matrix[14] * pvector[3]);
        resultvector.w = (((matrix[3] * pvector[0]) + (matrix[7] * pvector[1])) + (matrix[11] * pvector[2])) + (matrix[15] * pvector[3]);
    }

    public static bool Project(ref Vector3 obj, ref Matrix4x4 modelview, ref Matrix4x4 projection, ref Vector4 viewport, out Vector3 windowCoordinate)
    {
        Vector4 vector;
        Vector4 vector2;
        vector.x = (((modelview.m00 * obj.x) + (modelview.m10 * obj.y)) + (modelview.m20 * obj.z)) + modelview.m30;
        vector.y = (((modelview.m01 * obj.x) + (modelview.m11 * obj.y)) + (modelview.m21 * obj.z)) + modelview.m31;
        vector.z = (((modelview.m02 * obj.x) + (modelview.m12 * obj.y)) + (modelview.m22 * obj.z)) + modelview.m32;
        vector.w = (((modelview.m03 * obj.x) + (modelview.m13 * obj.y)) + (modelview.m23 * obj.z)) + modelview.m33;
        vector2.x = (((projection.m00 * vector.x) + (projection.m10 * vector.y)) + (projection.m20 * vector.z)) + (projection.m30 * vector.w);
        vector2.y = (((projection.m01 * vector.x) + (projection.m11 * vector.y)) + (projection.m21 * vector.z)) + (projection.m31 * vector.w);
        vector2.z = (((projection.m02 * vector.x) + (projection.m12 * vector.y)) + (projection.m22 * vector.z)) + (projection.m32 * vector.w);
        vector2.w = -vector.z;
        if (vector2.w == 0.0)
        {
            windowCoordinate = new Vector3();
            return false;
        }
        vector2.w = 1f / vector2.w;
        vector2.x *= vector2.w;
        vector2.y *= vector2.w;
        vector2.z *= vector2.w;
        windowCoordinate.x = (((vector2.x * 0.5f) + 0.5f) * viewport.z) + viewport.x;
        windowCoordinate.y = (((vector2.y * 0.5f) + 0.5f) * viewport.w) + viewport.y;
        windowCoordinate.z = 1f - vector2.z;
        return true;
    }

    public static bool UnProject(ref Vector3 win, ref Matrix4x4 modelview, ref Matrix4x4 projection, ref Vector4 viewport, out Vector3 objectCoordinate)
    {
        Matrix4x4 matrixx2;
        Vector4 vector;
        Vector4 vector2;
        Matrix4x4 m = projection * modelview;
        if (!InvertMatrix(ref m, out matrixx2))
        {
            objectCoordinate = new Vector3();
            return false;
        }
        vector.x = (((win.x - viewport.x) / viewport.z) * 2f) - 1f;
        vector.y = (((win.y - viewport.y) / viewport.w) * 2f) - 1f;
        vector.z = 1f - win.z;
        vector.w = 1f;
        MultiplyVector4(out vector2, ref matrixx2, ref vector);
        if (vector2.w == 0.0)
        {
            objectCoordinate = new Vector3();
            return false;
        }
        vector2.w = 1f / vector2.w;
        objectCoordinate.x = vector2.x * vector2.w;
        objectCoordinate.y = vector2.y * vector2.w;
        objectCoordinate.z = vector2.z * vector2.w;
        return true;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProjectHelper
    {
        public Matrix4x4 modelview;
        public Matrix4x4 projection;
        public Vector2 offset;
        public Vector2 size;
        public bool Project(ref Vector3 obj, out Vector3 windowCoordinate)
        {
            Vector4 vector;
            Vector4 vector2;
            vector.x = (((this.modelview.m00 * obj.x) + (this.modelview.m01 * obj.y)) + (this.modelview.m02 * obj.z)) + this.modelview.m03;
            vector.y = (((this.modelview.m10 * obj.x) + (this.modelview.m11 * obj.y)) + (this.modelview.m12 * obj.z)) + this.modelview.m13;
            vector.z = (((this.modelview.m20 * obj.x) + (this.modelview.m21 * obj.y)) + (this.modelview.m22 * obj.z)) + this.modelview.m23;
            vector.w = (((this.modelview.m30 * obj.x) + (this.modelview.m31 * obj.y)) + (this.modelview.m32 * obj.z)) + this.modelview.m33;
            vector2.x = (((this.projection.m00 * vector.x) + (this.projection.m01 * vector.y)) + (this.projection.m02 * vector.z)) + (this.projection.m03 * vector.w);
            vector2.y = (((this.projection.m10 * vector.x) + (this.projection.m11 * vector.y)) + (this.projection.m12 * vector.z)) + (this.projection.m13 * vector.w);
            vector2.z = (((this.projection.m20 * vector.x) + (this.projection.m21 * vector.y)) + (this.projection.m22 * vector.z)) + (this.projection.m23 * vector.w);
            vector2.w = -vector.z;
            if (vector2.w == 0.0)
            {
                windowCoordinate = new Vector3();
                return false;
            }
            vector2.w = 1f / vector2.w;
            vector2.x *= vector2.w;
            vector2.y *= vector2.w;
            windowCoordinate.x = (((vector2.x * 0.5f) + 0.5f) * this.size.x) + this.offset.x;
            windowCoordinate.y = (((vector2.y * 0.5f) + 0.5f) * this.size.y) + this.offset.y;
            windowCoordinate.z = vector2.z;
            return true;
        }

        public bool UnProject(ref Vector3 win, out Vector3 objectCoordinate)
        {
            Matrix4x4 matrixx2;
            Vector4 vector;
            Vector4 vector2;
            Matrix4x4 m = this.projection * this.modelview;
            if (!MatrixHelper.InvertMatrix(ref m, out matrixx2))
            {
                objectCoordinate = new Vector3();
                return false;
            }
            vector.x = (((win.x - this.offset.x) / this.size.x) * 2f) - 1f;
            vector.y = (((win.y - this.offset.y) / this.size.y) * 2f) - 1f;
            vector.z = -win.z;
            vector.w = 1f;
            MatrixHelper.MultiplyVector4(out vector2, ref matrixx2, ref vector);
            if (vector2.w == 0.0)
            {
                objectCoordinate = new Vector3();
                return false;
            }
            vector2.w = 1f / vector2.w;
            objectCoordinate.x = vector2.x * vector2.w;
            objectCoordinate.y = vector2.y * vector2.w;
            objectCoordinate.z = vector2.z * vector2.w;
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProjectHelperG
    {
        public Matrix4x4G modelview;
        public Matrix4x4G projection;
        public Vector2G offset;
        public Vector2G size;
        public bool Project(ref Vector3G obj, out Vector3G windowCoordinate)
        {
            Vector4G vectorg;
            Vector4G vectorg2;
            vectorg.x = (((this.modelview.m00 * obj.x) + (this.modelview.m01 * obj.y)) + (this.modelview.m02 * obj.z)) + this.modelview.m03;
            vectorg.y = (((this.modelview.m10 * obj.x) + (this.modelview.m11 * obj.y)) + (this.modelview.m12 * obj.z)) + this.modelview.m13;
            vectorg.z = (((this.modelview.m20 * obj.x) + (this.modelview.m21 * obj.y)) + (this.modelview.m22 * obj.z)) + this.modelview.m23;
            vectorg.w = (((this.modelview.m30 * obj.x) + (this.modelview.m31 * obj.y)) + (this.modelview.m32 * obj.z)) + this.modelview.m33;
            vectorg2.x = (((this.projection.m00 * vectorg.x) + (this.projection.m01 * vectorg.y)) + (this.projection.m02 * vectorg.z)) + (this.projection.m03 * vectorg.w);
            vectorg2.y = (((this.projection.m10 * vectorg.x) + (this.projection.m11 * vectorg.y)) + (this.projection.m12 * vectorg.z)) + (this.projection.m13 * vectorg.w);
            vectorg2.z = (((this.projection.m20 * vectorg.x) + (this.projection.m21 * vectorg.y)) + (this.projection.m22 * vectorg.z)) + (this.projection.m23 * vectorg.w);
            vectorg2.w = -vectorg.z;
            if (vectorg2.w == 0.0)
            {
                windowCoordinate = new Vector3G();
                return false;
            }
            vectorg2.w = 1.0 / vectorg2.w;
            vectorg2.x *= vectorg2.w;
            vectorg2.y *= vectorg2.w;
            windowCoordinate.x = (((vectorg2.x * 0.5) + 0.5) * this.size.x) + this.offset.x;
            windowCoordinate.y = (((vectorg2.y * 0.5) + 0.5) * this.size.y) + this.offset.y;
            windowCoordinate.z = vectorg2.z;
            return true;
        }

        public bool UnProject(ref Vector3G win, out Vector3G objectCoordinate)
        {
            Matrix4x4G matrixxg;
            Matrix4x4G matrixxg2;
            Vector4G vectorg;
            Vector4G vectorg2;
            Matrix4x4G.Mult(ref this.projection, ref this.modelview, out matrixxg);
            if (!Matrix4x4G.Inverse(ref matrixxg, out matrixxg2))
            {
                objectCoordinate = new Vector3G();
                return false;
            }
            vectorg.x = (((win.x - this.offset.x) / this.size.x) * 2.0) - 1.0;
            vectorg.y = (((win.y - this.offset.y) / this.size.y) * 2.0) - 1.0;
            vectorg.z = -win.z;
            vectorg.w = 1.0;
            Matrix4x4G.Mult(ref vectorg, ref matrixxg2, out vectorg2);
            if (vectorg2.w == 0.0)
            {
                objectCoordinate = new Vector3G();
                return false;
            }
            vectorg2.w = 1.0 / vectorg2.w;
            objectCoordinate.x = vectorg2.x * vectorg2.w;
            objectCoordinate.y = vectorg2.y * vectorg2.w;
            objectCoordinate.z = vectorg2.z * vectorg2.w;
            return true;
        }
    }
}

