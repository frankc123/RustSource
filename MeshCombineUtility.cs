using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MeshCombineUtility
{
    public static Mesh Combine(MeshInstance[] combines, bool generateStrips)
    {
        int num = 0;
        int num2 = 0;
        int num3 = 0;
        foreach (MeshInstance instance in combines)
        {
            if (instance.mesh != null)
            {
                num += instance.mesh.vertexCount;
                if (generateStrips)
                {
                    int length = instance.mesh.GetTriangleStrip(instance.subMeshIndex).Length;
                    if (length != 0)
                    {
                        if (num3 != 0)
                        {
                            if ((num3 & 1) == 1)
                            {
                                num3 += 3;
                            }
                            else
                            {
                                num3 += 2;
                            }
                        }
                        num3 += length;
                    }
                    else
                    {
                        generateStrips = false;
                    }
                }
            }
        }
        if (!generateStrips)
        {
            foreach (MeshInstance instance2 in combines)
            {
                if (instance2.mesh != null)
                {
                    num2 += instance2.mesh.GetTriangles(instance2.subMeshIndex).Length;
                }
            }
        }
        Vector3[] dst = new Vector3[num];
        Vector3[] vectorArray2 = new Vector3[num];
        Vector4[] vectorArray3 = new Vector4[num];
        Vector2[] vectorArray4 = new Vector2[num];
        Vector2[] vectorArray5 = new Vector2[num];
        Color[] colorArray = new Color[num];
        int[] numArray = new int[num2];
        int[] triangles = new int[num3];
        int offset = 0;
        foreach (MeshInstance instance3 in combines)
        {
            if (instance3.mesh != null)
            {
                Copy(instance3.mesh.vertexCount, instance3.mesh.vertices, dst, ref offset, instance3.transform);
            }
        }
        offset = 0;
        foreach (MeshInstance instance4 in combines)
        {
            if (instance4.mesh != null)
            {
                Matrix4x4 transpose = instance4.transform.inverse.transpose;
                CopyNormal(instance4.mesh.vertexCount, instance4.mesh.normals, vectorArray2, ref offset, transpose);
            }
        }
        offset = 0;
        foreach (MeshInstance instance5 in combines)
        {
            if (instance5.mesh != null)
            {
                Matrix4x4 transform = instance5.transform.inverse.transpose;
                CopyTangents(instance5.mesh.vertexCount, instance5.mesh.tangents, vectorArray3, ref offset, transform);
            }
        }
        offset = 0;
        foreach (MeshInstance instance6 in combines)
        {
            if (instance6.mesh != null)
            {
                Copy(instance6.mesh.vertexCount, instance6.mesh.uv, vectorArray4, ref offset);
            }
        }
        offset = 0;
        foreach (MeshInstance instance7 in combines)
        {
            if (instance7.mesh != null)
            {
                Copy(instance7.mesh.vertexCount, instance7.mesh.uv1, vectorArray5, ref offset);
            }
        }
        offset = 0;
        foreach (MeshInstance instance8 in combines)
        {
            if (instance8.mesh != null)
            {
                CopyColors(instance8.mesh.vertexCount, instance8.mesh.colors, colorArray, ref offset);
            }
        }
        int num14 = 0;
        int index = 0;
        int num16 = 0;
        foreach (MeshInstance instance9 in combines)
        {
            if (instance9.mesh != null)
            {
                if (generateStrips)
                {
                    int[] triangleStrip = instance9.mesh.GetTriangleStrip(instance9.subMeshIndex);
                    if (index != 0)
                    {
                        if ((index & 1) == 1)
                        {
                            triangles[index] = triangles[index - 1];
                            triangles[index + 1] = triangleStrip[0] + num16;
                            triangles[index + 2] = triangleStrip[0] + num16;
                            index += 3;
                        }
                        else
                        {
                            triangles[index] = triangles[index - 1];
                            triangles[index + 1] = triangleStrip[0] + num16;
                            index += 2;
                        }
                    }
                    for (int i = 0; i < triangleStrip.Length; i++)
                    {
                        triangles[i + index] = triangleStrip[i] + num16;
                    }
                    index += triangleStrip.Length;
                }
                else
                {
                    int[] numArray4 = instance9.mesh.GetTriangles(instance9.subMeshIndex);
                    for (int j = 0; j < numArray4.Length; j++)
                    {
                        numArray[j + num14] = numArray4[j] + num16;
                    }
                    num14 += numArray4.Length;
                }
                num16 += instance9.mesh.vertexCount;
            }
        }
        Mesh mesh = new Mesh {
            name = "Combined Mesh",
            vertices = dst,
            normals = vectorArray2,
            colors = colorArray,
            uv = vectorArray4,
            uv1 = vectorArray5,
            tangents = vectorArray3
        };
        if (generateStrips)
        {
            mesh.SetTriangleStrip(triangles, 0);
            return mesh;
        }
        mesh.triangles = numArray;
        return mesh;
    }

    private static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
    {
        for (int i = 0; i < src.Length; i++)
        {
            dst[i + offset] = src[i];
        }
        offset += vertexcount;
    }

    private static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
        {
            dst[i + offset] = transform.MultiplyPoint(src[i]);
        }
        offset += vertexcount;
    }

    private static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
    {
        for (int i = 0; i < src.Length; i++)
        {
            dst[i + offset] = src[i];
        }
        offset += vertexcount;
    }

    private static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
        {
            dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
        }
        offset += vertexcount;
    }

    private static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
        {
            Vector4 vector = src[i];
            Vector3 v = new Vector3(vector.x, vector.y, vector.z);
            v = transform.MultiplyVector(v).normalized;
            dst[i + offset] = new Vector4(v.x, v.y, v.z, vector.w);
        }
        offset += vertexcount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MeshInstance
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Matrix4x4 transform;
    }
}

