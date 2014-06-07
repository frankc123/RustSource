namespace NGUI.Meshing
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MeshBuffer
    {
        public int iCount;
        private PrimitiveKind lastPrimitiveKind = PrimitiveKind.Invalid;
        private int primCapacity;
        private Primitive[] primitives;
        private int primSize;
        public Vertex[] v;
        private int vertCapacity;
        public int vSize;

        public int Alloc(PrimitiveKind kind)
        {
            int num;
            return this.Alloc(kind, out num);
        }

        public int Alloc(PrimitiveKind kind, out int end)
        {
            int count = Primitive.VertexCount(kind);
            if (this.lastPrimitiveKind != kind)
            {
                int index = Gen_Alloc<Primitive>(1, ref this.primSize, ref this.primCapacity, ref this.primitives, 4, 0x20, 0x20);
                if (Primitive.JoinsInList(kind))
                {
                    this.lastPrimitiveKind = kind;
                }
                else
                {
                    this.lastPrimitiveKind = PrimitiveKind.Invalid;
                }
                this.primitives[index] = new Primitive(kind, (ushort) this.vSize);
            }
            this.iCount += Primitive.IndexCount(kind);
            int num3 = Gen_Alloc<Vertex>(count, ref this.vSize, ref this.vertCapacity, ref this.v, 0x20, 0x200, 0x200);
            end = num3 + count;
            return num3;
        }

        public int Alloc(PrimitiveKind kind, Vertex v)
        {
            int num;
            return this.Alloc(kind, v, out num);
        }

        public int Alloc(PrimitiveKind kind, float z)
        {
            int num;
            return this.Alloc(kind, z, out num);
        }

        public int Alloc(PrimitiveKind kind, Color color)
        {
            int num;
            return this.Alloc(kind, color, out num);
        }

        public int Alloc(PrimitiveKind primitive, Vertex V, out int end)
        {
            int num = this.Alloc(primitive, out end);
            for (int i = num; i < end; i++)
            {
                this.v[i].x = V.x;
                this.v[i].y = V.y;
                this.v[i].r = V.r;
                this.v[i].u = V.u;
                this.v[i].v = V.v;
                this.v[i].g = V.g;
                this.v[i].b = V.b;
                this.v[i].a = V.a;
            }
            return num;
        }

        public int Alloc(PrimitiveKind primitive, float z, out int end)
        {
            int num = this.Alloc(primitive, out end);
            for (int i = num; i < end; i++)
            {
                this.v[i].z = z;
                this.v[i].r = this.v[i].g = this.v[i].b = this.v[i].a = 1f;
            }
            return num;
        }

        public int Alloc(PrimitiveKind kind, float z, Color color)
        {
            int num;
            return this.Alloc(kind, z, color, out num);
        }

        public int Alloc(PrimitiveKind kind, float z, ref Color color)
        {
            int num;
            return this.Alloc(kind, z, ref color, out num);
        }

        public int Alloc(PrimitiveKind primitive, Color color, out int end)
        {
            int num = this.Alloc(primitive, out end);
            float r = color.r;
            float g = color.g;
            float b = color.b;
            float a = color.a;
            for (int i = num; i < end; i++)
            {
                this.v[i].r = r;
                this.v[i].g = g;
                this.v[i].b = b;
                this.v[i].a = a;
            }
            return num;
        }

        public int Alloc(PrimitiveKind primitive, float z, Color color, out int end)
        {
            int num = this.Alloc(primitive, out end);
            float r = color.r;
            float g = color.g;
            float b = color.b;
            float a = color.a;
            for (int i = num; i < end; i++)
            {
                this.v[i].r = r;
                this.v[i].g = g;
                this.v[i].b = b;
                this.v[i].a = a;
                this.v[i].z = z;
            }
            return num;
        }

        public int Alloc(PrimitiveKind primitive, float z, ref Color color, out int end)
        {
            int num = this.Alloc(primitive, out end);
            float r = color.r;
            float g = color.g;
            float b = color.b;
            float a = color.a;
            for (int i = num; i < end; i++)
            {
                this.v[i].r = r;
                this.v[i].g = g;
                this.v[i].b = b;
                this.v[i].a = a;
                this.v[i].z = z;
            }
            return num;
        }

        public void ApplyEffect(Transform transform, int vertexStart, UILabel.Effect effect, Color effectColor, float size)
        {
            this.ApplyEffect(transform, vertexStart, this.vSize, effect, effectColor, size);
        }

        public void ApplyEffect(Transform transform, int vertexStart, int vertexEnd, UILabel.Effect effect, Color effectColor, float size)
        {
            int num;
            if ((((effect != UILabel.Effect.None) && (vertexStart != vertexEnd)) && (!NGUITools.ZeroAlpha(effectColor.a) && (size != 0f))) && (!ZeroedXYScale(transform) && this.SeekPrimitiveIndex(vertexStart, out num)))
            {
                float pixel = 1f / size;
                switch (effect)
                {
                    case UILabel.Effect.Shadow:
                        this.ApplyShadow(vertexStart, vertexEnd, num, pixel, effectColor.r, effectColor.g, effectColor.b, effectColor.a);
                        break;

                    case UILabel.Effect.Outline:
                        this.ApplyOutline(vertexStart, vertexEnd, num, pixel, effectColor.r, effectColor.g, effectColor.b, effectColor.a);
                        break;
                }
            }
        }

        private void ApplyOutline(int start, int end, int primitiveIndex, float pixel, float r, float g, float b, float a)
        {
            while (start < end)
            {
                int num;
                int num2;
                int num3;
                int num4;
                if ((primitiveIndex != (this.primSize - 1)) && (this.primitives[primitiveIndex + 1].start <= start))
                {
                    primitiveIndex++;
                }
                int index = this.Alloc(this.primitives[primitiveIndex].kind, out num);
                int num6 = this.Alloc(this.primitives[primitiveIndex].kind, out num2);
                int num7 = this.Alloc(this.primitives[primitiveIndex].kind, out num3);
                int num8 = this.Alloc(this.primitives[primitiveIndex].kind, out num4);
                if (num8 == num4)
                {
                    throw new InvalidOperationException();
                }
                while (num8 < num4)
                {
                    Vertex vertex;
                    this.v[num8] = this.v[start];
                    this.v[start].r = r;
                    this.v[start].g = g;
                    this.v[start].b = b;
                    this.v[start].a *= a;
                    this.v[num7] = vertex = this.v[start];
                    this.v[index] = this.v[num6] = vertex;
                    this.v[start].x += pixel;
                    this.v[start].y -= pixel;
                    this.v[index].x -= pixel;
                    this.v[index].y += pixel;
                    this.v[num6].x += pixel;
                    this.v[num6].y += pixel;
                    this.v[num7].x -= pixel;
                    this.v[num7].y -= pixel;
                    index++;
                    num6++;
                    num7++;
                    num8++;
                    start++;
                }
            }
        }

        private void ApplyShadow(int start, int end, int primitiveIndex, float pixel, float r, float g, float b, float a)
        {
            while (start < end)
            {
                int num;
                if ((primitiveIndex != (this.primSize - 1)) && (this.primitives[primitiveIndex + 1].start <= start))
                {
                    primitiveIndex++;
                }
                int num2 = this.Alloc(this.primitives[primitiveIndex].kind, out num);
                if (num2 == num)
                {
                    throw new InvalidOperationException();
                }
                while (num2 < num)
                {
                    this.v[num2++] = this.v[start];
                    this.v[start].r = r;
                    this.v[start].g = g;
                    this.v[start].b = b;
                    this.v[start].a *= a;
                    this.v[start].x += pixel;
                    this.v[start].y -= pixel;
                    start++;
                }
            }
        }

        public void BuildTransformedVertices4x4(ref Vector3[] tV, float m00, float m10, float m20, float m30, float m01, float m11, float m21, float m31, float m02, float m12, float m22, float m32, float m03, float m13, float m23, float m33)
        {
            Array.Resize<Vector3>(ref tV, this.vSize);
            for (int i = 0; i < this.vSize; i++)
            {
                float num = 1f / ((((m30 * this.v[i].x) + (m31 * this.v[i].y)) + (m32 * this.v[i].z)) + m33);
                tV[i].x = ((((m00 * this.v[i].x) + (m01 * this.v[i].y)) + (m02 * this.v[i].z)) + m03) * num;
                tV[i].y = ((((m10 * this.v[i].x) + (m11 * this.v[i].y)) + (m12 * this.v[i].z)) + m13) * num;
                tV[i].z = ((((m20 * this.v[i].x) + (m21 * this.v[i].y)) + (m22 * this.v[i].z)) + m23) * num;
            }
        }

        public void Clear()
        {
            this.vSize = 0;
            this.iCount = 0;
            this.primSize = 0;
            this.lastPrimitiveKind = PrimitiveKind.Invalid;
        }

        private void Extract(FillBuffer<Vector3> vertices, FillBuffer<Vector2> uvs, FillBuffer<Color> colors, FillBuffer<int> triangles)
        {
            Vector3[] buf = vertices.buf;
            Vector2[] vectorArray2 = uvs.buf;
            Color[] colorArray = colors.buf;
            int[] t = triangles.buf;
            int offset = vertices.offset;
            int index = uvs.offset;
            int num3 = colors.offset;
            for (int i = 0; i < this.vSize; i++)
            {
                buf[offset].x = this.v[i].x;
                buf[offset].y = this.v[i].y;
                buf[offset].z = this.v[i].z;
                vectorArray2[index].x = this.v[i].u;
                vectorArray2[index].y = this.v[i].v;
                colorArray[num3].r = this.v[i].r;
                colorArray[num3].g = this.v[i].g;
                colorArray[num3].b = this.v[i].b;
                colorArray[num3].a = this.v[i].a;
                offset++;
                index++;
                num3++;
            }
            int num5 = triangles.offset;
            int v = vertices.offset;
            if (this.primSize > 0)
            {
                for (int j = 0; j < (this.primSize - 1); j++)
                {
                    this.primitives[j].Put(t, ref v, ref num5, this.primitives[j + 1].start);
                }
                this.primitives[this.primSize - 1].Put(t, ref v, ref num5, this.vSize);
            }
        }

        public bool ExtractMeshBuffers(ref Vector3[] vertices, ref Vector2[] uvs, ref Color[] colors, ref int[] triangles)
        {
            bool flag = ((ResizeChecked<Vector3>(ref vertices, this.vSize) | ResizeChecked<Vector2>(ref uvs, this.vSize)) | ResizeChecked<Color>(ref colors, this.vSize)) | ResizeChecked<int>(ref triangles, this.iCount);
            FillBuffer<Vector3> buffer = new FillBuffer<Vector3> {
                buf = vertices
            };
            FillBuffer<Vector2> buffer2 = new FillBuffer<Vector2> {
                buf = uvs
            };
            FillBuffer<Color> buffer3 = new FillBuffer<Color> {
                buf = colors
            };
            FillBuffer<int> buffer4 = new FillBuffer<int> {
                buf = triangles
            };
            this.Extract(buffer, buffer2, buffer3, buffer4);
            return flag;
        }

        public void FastCell(Vector2 xy0, Vector2 xy1, Vector2 uv0, Vector2 uv1, ref Color color)
        {
            Vertex vertex;
            Vertex vertex2;
            Vertex vertex3;
            Vertex vertex4;
            vertex.x = vertex2.x = xy1.x;
            vertex.y = vertex4.y = xy1.y;
            vertex3.x = vertex4.x = xy0.x;
            vertex2.y = vertex3.y = xy0.y;
            vertex.z = vertex2.z = vertex3.z = vertex4.z = 0f;
            vertex.u = vertex2.u = uv1.x;
            vertex.v = vertex4.v = uv1.y;
            vertex3.u = vertex4.u = uv0.x;
            vertex2.v = vertex3.v = uv0.y;
            vertex.r = vertex2.r = vertex3.r = vertex4.r = color.r;
            vertex.g = vertex2.g = vertex3.g = vertex4.g = color.g;
            vertex.b = vertex2.b = vertex3.b = vertex4.b = color.b;
            vertex.a = vertex2.a = vertex3.a = vertex4.a = color.a;
            this.Quad(vertex, vertex2, vertex3, vertex4);
        }

        public int FastQuad(Rect uv, Color color)
        {
            return this.FastQuad(new Vector2(uv.xMin, uv.yMin), new Vector2(uv.xMax, uv.yMax), color);
        }

        public int FastQuad(Vector2 uv0, Vector2 uv1, Color color)
        {
            Vertex vertex;
            Vertex vertex2;
            Vertex vertex3;
            Vertex vertex4;
            vertex.x = vertex2.x = 1f;
            vertex2.y = vertex3.y = -1f;
            vertex.y = vertex.z = vertex2.z = vertex3.x = vertex3.z = vertex4.x = vertex4.y = vertex4.z = 0f;
            vertex.u = vertex2.u = uv1.x;
            vertex.v = vertex4.v = uv1.y;
            vertex3.u = vertex4.u = uv0.x;
            vertex2.v = vertex3.v = uv0.y;
            vertex.r = vertex2.r = vertex3.r = vertex4.r = color.r;
            vertex.g = vertex2.g = vertex3.g = vertex4.g = color.g;
            vertex.b = vertex2.b = vertex3.b = vertex4.b = color.b;
            vertex.a = vertex2.a = vertex3.a = vertex4.a = color.a;
            return this.Quad(vertex, vertex2, vertex3, vertex4);
        }

        public int FastQuad(Vector2 xy0, Vector2 xy1, Vector2 uv0, Vector2 uv1, Color color)
        {
            Vertex vertex;
            Vertex vertex2;
            Vertex vertex3;
            Vertex vertex4;
            vertex.x = vertex2.x = xy1.x;
            vertex.y = vertex4.y = xy1.y;
            vertex3.x = vertex4.x = xy0.x;
            vertex2.y = vertex3.y = xy0.y;
            vertex.z = vertex2.z = vertex3.z = vertex4.z = 0f;
            vertex.u = vertex2.u = uv1.x;
            vertex.v = vertex4.v = uv1.y;
            vertex3.u = vertex4.u = uv0.x;
            vertex2.v = vertex3.v = uv0.y;
            vertex.r = vertex2.r = vertex3.r = vertex4.r = color.r;
            vertex.g = vertex2.g = vertex3.g = vertex4.g = color.g;
            vertex.b = vertex2.b = vertex3.b = vertex4.b = color.b;
            vertex.a = vertex2.a = vertex3.a = vertex4.a = color.a;
            return this.Quad(vertex, vertex2, vertex3, vertex4);
        }

        public int FastQuad(Vector2 xy0, Vector2 xy1, float z, Vector2 uv0, Vector2 uv1, Color color)
        {
            Vertex vertex;
            Vertex vertex2;
            Vertex vertex3;
            Vertex vertex4;
            vertex.x = vertex2.x = xy1.x;
            vertex.y = vertex4.y = xy1.y;
            vertex3.x = vertex4.x = xy0.x;
            vertex2.y = vertex3.y = xy0.y;
            vertex.z = vertex2.z = vertex3.z = vertex4.z = z;
            vertex.u = vertex2.u = uv1.x;
            vertex.v = vertex4.v = uv1.y;
            vertex3.u = vertex4.u = uv0.x;
            vertex2.v = vertex3.v = uv0.y;
            vertex.r = vertex2.r = vertex3.r = vertex4.r = color.r;
            vertex.g = vertex2.g = vertex3.g = vertex4.g = color.g;
            vertex.b = vertex2.b = vertex3.b = vertex4.b = color.b;
            vertex.a = vertex2.a = vertex3.a = vertex4.a = color.a;
            return this.Quad(vertex, vertex2, vertex3, vertex4);
        }

        private static int Gen_Alloc<T>(int count, ref int size, ref int cap, ref T[] array, int initAllocSize, int maxAllocSize, int maxAllocSizeIncrement)
        {
            if (count <= 0)
            {
                return -1;
            }
            int num = size;
            size += count;
            if (size > cap)
            {
                if (cap == 0)
                {
                    cap = initAllocSize;
                }
                while (cap < size)
                {
                    if (cap < maxAllocSize)
                    {
                        cap *= 2;
                    }
                    else
                    {
                        cap += maxAllocSizeIncrement;
                    }
                }
                Array.Resize<T>(ref array, cap);
            }
            return num;
        }

        public void Offset(float x, float y, float z)
        {
            if (x == 0f)
            {
                if (y == 0f)
                {
                    if (z != 0f)
                    {
                        for (int i = 0; i < this.vSize; i++)
                        {
                            this.v[i].z += z;
                        }
                    }
                }
                else if (z == 0f)
                {
                    for (int j = 0; j < this.vSize; j++)
                    {
                        this.v[j].y += y;
                    }
                }
                else
                {
                    for (int k = 0; k < this.vSize; k++)
                    {
                        this.v[k].y += y;
                        this.v[k].z += z;
                    }
                }
            }
            else if (y == 0f)
            {
                if (z == 0f)
                {
                    for (int m = 0; m < this.vSize; m++)
                    {
                        this.v[m].x += x;
                    }
                }
                else
                {
                    for (int n = 0; n < this.vSize; n++)
                    {
                        this.v[n].x += x;
                        this.v[n].z += z;
                    }
                }
            }
            else if (z == 0f)
            {
                for (int num6 = 0; num6 < this.vSize; num6++)
                {
                    this.v[num6].x += x;
                    this.v[num6].y += y;
                }
            }
            else
            {
                for (int num7 = 0; num7 < this.vSize; num7++)
                {
                    this.v[num7].x += x;
                    this.v[num7].y += y;
                    this.v[num7].z += z;
                }
            }
        }

        public void OffsetThenTransformVertices(float x, float y, float z, float m00, float m10, float m20, float m01, float m11, float m21, float m02, float m12, float m22, float m03, float m13, float m23)
        {
            float num;
            float num2;
            float num3;
            if (x == 0f)
            {
                if (y == 0f)
                {
                    if (z == 0f)
                    {
                        for (int i = 0; i < this.vSize; i++)
                        {
                            num = this.v[i].x;
                            num2 = this.v[i].y;
                            num3 = this.v[i].z;
                            this.v[i].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                            this.v[i].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                            this.v[i].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < this.vSize; j++)
                        {
                            num = this.v[j].x;
                            num2 = this.v[j].y;
                            num3 = this.v[j].z + z;
                            this.v[j].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                            this.v[j].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                            this.v[j].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                        }
                    }
                }
                else if (z == 0f)
                {
                    for (int k = 0; k < this.vSize; k++)
                    {
                        num = this.v[k].x;
                        num2 = this.v[k].y + y;
                        num3 = this.v[k].z;
                        this.v[k].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                        this.v[k].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                        this.v[k].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                    }
                }
                else
                {
                    for (int m = 0; m < this.vSize; m++)
                    {
                        num = this.v[m].x;
                        num2 = this.v[m].y + y;
                        num3 = this.v[m].z + z;
                        this.v[m].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                        this.v[m].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                        this.v[m].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                    }
                }
            }
            else if (y == 0f)
            {
                if (z == 0f)
                {
                    for (int n = 0; n < this.vSize; n++)
                    {
                        num = this.v[n].x + x;
                        num2 = this.v[n].y;
                        num3 = this.v[n].z;
                        this.v[n].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                        this.v[n].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                        this.v[n].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                    }
                }
                else
                {
                    for (int num9 = 0; num9 < this.vSize; num9++)
                    {
                        num = this.v[num9].x + x;
                        num2 = this.v[num9].y;
                        num3 = this.v[num9].z + z;
                        this.v[num9].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                        this.v[num9].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                        this.v[num9].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                    }
                }
            }
            else if (z == 0f)
            {
                for (int num10 = 0; num10 < this.vSize; num10++)
                {
                    num = this.v[num10].x + x;
                    num2 = this.v[num10].y + y;
                    num3 = this.v[num10].z;
                    this.v[num10].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                    this.v[num10].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                    this.v[num10].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                }
            }
            else
            {
                for (int num11 = 0; num11 < this.vSize; num11++)
                {
                    num = this.v[num11].x + x;
                    num2 = this.v[num11].y + y;
                    num3 = this.v[num11].z + z;
                    this.v[num11].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                    this.v[num11].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                    this.v[num11].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                }
            }
        }

        public int Quad(Vertex A, Vertex B, Vertex C, Vertex D)
        {
            int num;
            int num2 = this.Alloc(PrimitiveKind.Quad, out num);
            int num3 = num2;
            this.v[num3++] = B;
            this.v[num3++] = A;
            this.v[num3++] = C;
            this.v[num3++] = D;
            return num2;
        }

        public int QuadAlt(Vertex A, Vertex B, Vertex C, Vertex D)
        {
            return this.Quad(D, A, B, C);
        }

        private static bool ResizeChecked<T>(ref T[] array, int size)
        {
            if (size == 0)
            {
                if ((array != null) && (array.Length != 0))
                {
                    array = null;
                    return true;
                }
                return false;
            }
            if ((array != null) && (array.Length == size))
            {
                return false;
            }
            Array.Resize<T>(ref array, size);
            return true;
        }

        private bool SeekPrimitiveIndex(int start, out int i)
        {
            i = this.primSize - 1;
            while (i >= 0)
            {
                if (this.primitives[i].start <= start)
                {
                    return true;
                }
                i--;
            }
            i = -1;
            return false;
        }

        public int TextureQuad(Vertex A, Vertex B, Vertex C, Vertex D)
        {
            int num;
            int num2 = this.Alloc(PrimitiveKind.Quad, out num);
            int num3 = num2;
            this.v[num3++] = D;
            this.v[num3++] = A;
            this.v[num3++] = C;
            this.v[num3++] = B;
            return num2;
        }

        public void TransformThenOffsetVertices(float m00, float m10, float m20, float m01, float m11, float m21, float m02, float m12, float m22, float m03, float m13, float m23, float x, float y, float z)
        {
            float num;
            float num2;
            float num3;
            if (x == 0f)
            {
                if (y == 0f)
                {
                    if (z == 0f)
                    {
                        for (int i = 0; i < this.vSize; i++)
                        {
                            num = this.v[i].x;
                            num2 = this.v[i].y;
                            num3 = this.v[i].z;
                            this.v[i].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                            this.v[i].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                            this.v[i].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < this.vSize; j++)
                        {
                            num = this.v[j].x;
                            num2 = this.v[j].y;
                            num3 = this.v[j].z;
                            this.v[j].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                            this.v[j].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                            this.v[j].z = ((((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23) + z;
                        }
                    }
                }
                else if (z == 0f)
                {
                    for (int k = 0; k < this.vSize; k++)
                    {
                        num = this.v[k].x;
                        num2 = this.v[k].y;
                        num3 = this.v[k].z;
                        this.v[k].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                        this.v[k].y = ((((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13) + y;
                        this.v[k].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                    }
                }
                else
                {
                    for (int m = 0; m < this.vSize; m++)
                    {
                        num = this.v[m].x;
                        num2 = this.v[m].y;
                        num3 = this.v[m].z;
                        this.v[m].x = (((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03;
                        this.v[m].y = ((((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13) + y;
                        this.v[m].z = ((((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23) + z;
                    }
                }
            }
            else if (y == 0f)
            {
                if (z == 0f)
                {
                    for (int n = 0; n < this.vSize; n++)
                    {
                        num = this.v[n].x;
                        num2 = this.v[n].y;
                        num3 = this.v[n].z;
                        this.v[n].x = ((((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03) + x;
                        this.v[n].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                        this.v[n].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                    }
                }
                else
                {
                    for (int num9 = 0; num9 < this.vSize; num9++)
                    {
                        num = this.v[num9].x;
                        num2 = this.v[num9].y;
                        num3 = this.v[num9].z;
                        this.v[num9].x = ((((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03) + x;
                        this.v[num9].y = (((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13;
                        this.v[num9].z = ((((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23) + z;
                    }
                }
            }
            else if (z == 0f)
            {
                for (int num10 = 0; num10 < this.vSize; num10++)
                {
                    num = this.v[num10].x;
                    num2 = this.v[num10].y;
                    num3 = this.v[num10].z;
                    this.v[num10].x = ((((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03) + x;
                    this.v[num10].y = ((((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13) + y;
                    this.v[num10].z = (((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23;
                }
            }
            else
            {
                for (int num11 = 0; num11 < this.vSize; num11++)
                {
                    num = this.v[num11].x;
                    num2 = this.v[num11].y;
                    num3 = this.v[num11].z;
                    this.v[num11].x = ((((m00 * num) + (m01 * num2)) + (m02 * num3)) + m03) + x;
                    this.v[num11].y = ((((m10 * num) + (m11 * num2)) + (m12 * num3)) + m13) + y;
                    this.v[num11].z = ((((m20 * num) + (m21 * num2)) + (m22 * num3)) + m23) + z;
                }
            }
        }

        public void TransformVertices(float m00, float m10, float m20, float m01, float m11, float m21, float m02, float m12, float m22, float m03, float m13, float m23)
        {
            for (int i = 0; i < this.vSize; i++)
            {
                float x = this.v[i].x;
                float y = this.v[i].y;
                float z = this.v[i].z;
                this.v[i].x = (((m00 * x) + (m01 * y)) + (m02 * z)) + m03;
                this.v[i].y = (((m10 * x) + (m11 * y)) + (m12 * z)) + m13;
                this.v[i].z = (((m20 * x) + (m21 * y)) + (m22 * z)) + m23;
            }
        }

        public int Triangle(Vertex A, Vertex B, Vertex C)
        {
            int num;
            int num2 = this.Alloc(PrimitiveKind.Triangle, out num);
            int num3 = num2;
            this.v[num3++] = A;
            this.v[num3++] = B;
            this.v[num3++] = C;
            return num2;
        }

        public void WriteBuffers(MeshBuffer target)
        {
            if (this.primSize > 0)
            {
                int start = 0;
                int index = 0;
                while (index < (this.primSize - 1))
                {
                    this.primitives[index].Copy(ref start, this.v, this.primitives[index + 1].start, target);
                    index++;
                }
                this.primitives[index].Copy(ref start, this.v, this.vSize, target);
            }
        }

        public void WriteBuffers(Vector3[] transformedVertexes, MeshBuffer target)
        {
            if (transformedVertexes == null)
            {
                this.WriteBuffers(target);
            }
            else if (this.primSize > 0)
            {
                int start = 0;
                int index = 0;
                while (index < (this.primSize - 1))
                {
                    this.primitives[index].Copy(ref start, this.v, transformedVertexes, this.primitives[index + 1].start, target);
                    index++;
                }
                this.primitives[index].Copy(ref start, this.v, transformedVertexes, this.vSize, target);
            }
        }

        private static bool ZeroedXYScale(Transform transform)
        {
            if (transform == null)
            {
                return false;
            }
            Vector3 localScale = transform.localScale;
            return ((localScale.x == 0f) || (localScale.y == 0f));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FillBuffer<T>
        {
            public T[] buf;
            public int offset;
        }
    }
}

