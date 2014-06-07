using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GeometryMeshing
{
    public static Mesh Capsule(CapsuleInfo capsule)
    {
        if (capsule.height <= (capsule.radius * 2f))
        {
            SphereInfo info;
            info.offset = capsule.offset;
            info.radius = capsule.radius;
            info.capSplit = capsule.capSplit;
            info.sides = capsule.sides;
            return Sphere(info);
        }
        bool flag = capsule.capSplit == 0;
        int num = !flag ? (capsule.capSplit - 1) : 0;
        int num2 = !flag ? ((num * capsule.sides) + 1) : 0;
        Vector3[] vertices = new Vector3[(capsule.sides * 2) + (!flag ? (2 + ((num * capsule.sides) * 2)) : 0)];
        float y = capsule.offset.y - (capsule.height / 2f);
        float num4 = y + capsule.radius;
        float num5 = capsule.offset.y + (capsule.height / 2f);
        float num6 = num5 - capsule.radius;
        for (int i = 0; i < capsule.sides; i++)
        {
            float f = (((float) i) / (((float) capsule.sides) / 2f)) * 3.141593f;
            int index = i + num2;
            int num10 = index + capsule.sides;
            vertices[index].x = vertices[num10].x = capsule.offset.x + (Mathf.Cos(f) * capsule.radius);
            vertices[index].z = vertices[num10].z = capsule.offset.z + (Mathf.Sin(f) * capsule.radius);
            vertices[index].y = num4;
            vertices[num10].y = num6;
        }
        if (!flag)
        {
            vertices[0] = new Vector3(capsule.offset.x, y, capsule.offset.z);
            vertices[vertices.Length - 1] = new Vector3(capsule.offset.x, num5, capsule.offset.z);
        }
        int[] indices = new int[3 * (((!flag ? capsule.sides : (capsule.sides - 1)) * 2) + (capsule.sides * 2))];
        int num11 = 0;
        if (flag)
        {
            for (int k = 1; k < capsule.sides; k++)
            {
                indices[num11++] = k + num2;
                indices[num11++] = ((k + 1) % capsule.sides) + num2;
                indices[num11++] = 0;
            }
            for (int m = 0; m < (capsule.sides - 1); m++)
            {
                indices[num11++] = m + (num2 + capsule.sides);
                indices[num11++] = (((m + 1) % capsule.sides) + num2) + capsule.sides;
                indices[num11++] = vertices.Length - 1;
            }
        }
        else
        {
            for (int n = 0; n < capsule.sides; n++)
            {
                indices[num11++] = n + num2;
                indices[num11++] = ((n + 1) % capsule.sides) + num2;
                indices[num11++] = 0;
            }
            for (int num15 = 0; num15 < capsule.sides; num15++)
            {
                indices[num11++] = num15 + (num2 + capsule.sides);
                indices[num11++] = vertices.Length - 1;
                indices[num11++] = ((num15 + 1) % capsule.sides) + (num2 + capsule.sides);
            }
        }
        for (int j = 0; j < capsule.sides; j++)
        {
            indices[num11++] = j + num2;
            indices[num11++] = (j + num2) + capsule.sides;
            indices[num11++] = ((j + 1) % capsule.sides) + num2;
            indices[num11++] = (j + num2) + capsule.sides;
            indices[num11++] = (((j + 1) % capsule.sides) + num2) + capsule.sides;
            indices[num11++] = ((j + 1) % capsule.sides) + num2;
        }
        return new Mesh(vertices, indices, IndexKind.Triangles);
    }

    public static Mesh Sphere(SphereInfo sphere)
    {
        Debug.Log("TODO");
        return new Mesh();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CapsuleInfo
    {
        public Vector3 offset;
        public float height;
        public float radius;
        public int sides;
        public int capSplit;
    }

    public enum IndexKind : sbyte
    {
        Invalid = 0,
        Triangles = 1,
        TriangleStrip = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Mesh
    {
        public readonly Vector3[] vertices;
        public readonly int[] indices;
        public readonly uint indexCount;
        public readonly ushort vertexCount;
        public readonly GeometryMeshing.IndexKind indexKind;
        internal Mesh(Vector3[] vertices, int[] indices, GeometryMeshing.IndexKind kind)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.vertexCount = (ushort) this.vertices.Length;
            this.indexCount = (uint) this.indices.Length;
            this.indexKind = kind;
        }

        public bool valid
        {
            get
            {
                return (((int) this.indexKind) != 0);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SphereInfo
    {
        public Vector3 offset;
        public float radius;
        public int sides;
        public int capSplit;
    }
}

