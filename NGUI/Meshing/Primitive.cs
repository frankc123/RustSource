namespace NGUI.Meshing
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct Primitive
    {
        public readonly PrimitiveKind kind;
        public readonly ushort start;
        public Primitive(PrimitiveKind kind, ushort start)
        {
            this.kind = kind;
            this.start = start;
        }

        public static int VertexCount(PrimitiveKind kind)
        {
            switch (kind)
            {
                case PrimitiveKind.Triangle:
                    return 3;

                case PrimitiveKind.Quad:
                    return 4;

                case PrimitiveKind.Grid2x1:
                case PrimitiveKind.Grid1x2:
                    return 6;

                case PrimitiveKind.Grid2x2:
                    return 9;

                case PrimitiveKind.Grid1x3:
                case PrimitiveKind.Grid3x1:
                    return 8;

                case PrimitiveKind.Grid3x2:
                case PrimitiveKind.Grid2x3:
                    return 12;

                case PrimitiveKind.Grid3x3:
                case PrimitiveKind.Hole3x3:
                    return 0x10;
            }
            throw new NotImplementedException();
        }

        public static int IndexCount(PrimitiveKind kind)
        {
            switch (kind)
            {
                case PrimitiveKind.Triangle:
                    return 3;

                case PrimitiveKind.Quad:
                    return 6;

                case PrimitiveKind.Grid2x1:
                case PrimitiveKind.Grid1x2:
                    return 12;

                case PrimitiveKind.Grid2x2:
                    return 0x18;

                case PrimitiveKind.Grid1x3:
                case PrimitiveKind.Grid3x1:
                    return 0x12;

                case PrimitiveKind.Grid3x2:
                case PrimitiveKind.Grid2x3:
                    return 0x24;

                case PrimitiveKind.Grid3x3:
                    return 0x36;

                case PrimitiveKind.Hole3x3:
                    return 0x30;
            }
            throw new NotImplementedException();
        }

        public static bool JoinsInList(PrimitiveKind kind)
        {
            return true;
        }

        public void Copy(ref int start, Vertex[] v, int end, MeshBuffer p)
        {
            int num = (end - start) / VertexCount(this.kind);
            while (num-- > 0)
            {
                int num2;
                int num3 = p.Alloc(this.kind, out num2);
                while (num3 < num2)
                {
                    p.v[num3++] = v[start++];
                }
            }
        }

        public void Copy(ref int start, Vertex[] v, Vector3[] transformed, int end, MeshBuffer p)
        {
            int num = (end - start) / VertexCount(this.kind);
            while (num-- > 0)
            {
                int num2;
                int index = p.Alloc(this.kind, out num2);
                while (index < num2)
                {
                    p.v[index].x = transformed[start].x;
                    p.v[index].y = transformed[start].y;
                    p.v[index].z = transformed[start].z;
                    p.v[index].u = v[start].u;
                    p.v[index].v = v[start].v;
                    p.v[index].r = v[start].r;
                    p.v[index].g = v[start].g;
                    p.v[index].b = v[start].b;
                    p.v[index].a = v[start].a;
                    index++;
                    start++;
                }
            }
        }

        public void Put(int[] t, ref int v, ref int i, int end)
        {
            int num = (end - this.start) / VertexCount(this.kind);
            switch (this.kind)
            {
                case PrimitiveKind.Triangle:
                    while (num-- > 0)
                    {
                        t[i++] = v;
                        t[i++] = v + 1;
                        t[i++] = v + 2;
                        v += 3;
                    }
                    break;

                case PrimitiveKind.Quad:
                    while (num-- > 0)
                    {
                        t[i++] = v;
                        t[i++] = v + 1;
                        t[i++] = v + 3;
                        t[i++] = v + 2;
                        t[i++] = v;
                        t[i++] = v + 3;
                        v += 4;
                    }
                    break;

                case PrimitiveKind.Grid2x1:
                    while (num-- > 0)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            for (int k = 0; k < 1; k++)
                            {
                                t[i++] = v + (j + (k * 3));
                                t[i++] = v + ((j + 1) + (k * 3));
                                t[i++] = v + (j + ((k + 1) * 3));
                                t[i++] = v + ((j + 1) + (k * 3));
                                t[i++] = v + ((j + 1) + ((k + 1) * 3));
                                t[i++] = v + (j + ((k + 1) * 3));
                            }
                        }
                        v += 6;
                    }
                    break;

                case PrimitiveKind.Grid1x2:
                    while (num-- > 0)
                    {
                        for (int m = 0; m < 1; m++)
                        {
                            for (int n = 0; n < 2; n++)
                            {
                                t[i++] = v + (m + (n * 2));
                                t[i++] = v + ((m + 1) + (n * 2));
                                t[i++] = v + (m + ((n + 1) * 2));
                                t[i++] = v + ((m + 1) + (n * 2));
                                t[i++] = v + ((m + 1) + ((n + 1) * 2));
                                t[i++] = v + (m + ((n + 1) * 2));
                            }
                        }
                        v += 6;
                    }
                    break;

                case PrimitiveKind.Grid2x2:
                    while (num-- > 0)
                    {
                        for (int num14 = 0; num14 < 2; num14++)
                        {
                            for (int num15 = 0; num15 < 2; num15++)
                            {
                                t[i++] = v + (num14 + (num15 * 3));
                                t[i++] = v + ((num14 + 1) + (num15 * 3));
                                t[i++] = v + (num14 + ((num15 + 1) * 3));
                                t[i++] = v + ((num14 + 1) + (num15 * 3));
                                t[i++] = v + ((num14 + 1) + ((num15 + 1) * 3));
                                t[i++] = v + (num14 + ((num15 + 1) * 3));
                            }
                        }
                        v += 9;
                    }
                    break;

                case PrimitiveKind.Grid1x3:
                    while (num-- > 0)
                    {
                        for (int num12 = 0; num12 < 1; num12++)
                        {
                            for (int num13 = 0; num13 < 3; num13++)
                            {
                                t[i++] = v + (num12 + (num13 * 2));
                                t[i++] = v + ((num12 + 1) + (num13 * 2));
                                t[i++] = v + (num12 + ((num13 + 1) * 2));
                                t[i++] = v + ((num12 + 1) + (num13 * 2));
                                t[i++] = v + ((num12 + 1) + ((num13 + 1) * 2));
                                t[i++] = v + (num12 + ((num13 + 1) * 2));
                            }
                        }
                        v += 8;
                    }
                    break;

                case PrimitiveKind.Grid3x1:
                    while (num-- > 0)
                    {
                        for (int num10 = 0; num10 < 3; num10++)
                        {
                            for (int num11 = 0; num11 < 1; num11++)
                            {
                                t[i++] = v + (num10 + (num11 * 4));
                                t[i++] = v + ((num10 + 1) + (num11 * 4));
                                t[i++] = v + (num10 + ((num11 + 1) * 4));
                                t[i++] = v + ((num10 + 1) + (num11 * 4));
                                t[i++] = v + ((num10 + 1) + ((num11 + 1) * 4));
                                t[i++] = v + (num10 + ((num11 + 1) * 4));
                            }
                        }
                        v += 8;
                    }
                    break;

                case PrimitiveKind.Grid3x2:
                    while (num-- > 0)
                    {
                        for (int num8 = 0; num8 < 3; num8++)
                        {
                            for (int num9 = 0; num9 < 2; num9++)
                            {
                                t[i++] = v + (num8 + (num9 * 4));
                                t[i++] = v + ((num8 + 1) + (num9 * 4));
                                t[i++] = v + (num8 + ((num9 + 1) * 4));
                                t[i++] = v + ((num8 + 1) + (num9 * 4));
                                t[i++] = v + ((num8 + 1) + ((num9 + 1) * 4));
                                t[i++] = v + (num8 + ((num9 + 1) * 4));
                            }
                        }
                        v += 12;
                    }
                    break;

                case PrimitiveKind.Grid2x3:
                    while (num-- > 0)
                    {
                        for (int num6 = 0; num6 < 2; num6++)
                        {
                            for (int num7 = 0; num7 < 3; num7++)
                            {
                                t[i++] = v + (num6 + (num7 * 3));
                                t[i++] = v + ((num6 + 1) + (num7 * 3));
                                t[i++] = v + (num6 + ((num7 + 1) * 3));
                                t[i++] = v + ((num6 + 1) + (num7 * 3));
                                t[i++] = v + ((num6 + 1) + ((num7 + 1) * 3));
                                t[i++] = v + (num6 + ((num7 + 1) * 3));
                            }
                        }
                        v += 12;
                    }
                    break;

                case PrimitiveKind.Grid3x3:
                    while (num-- > 0)
                    {
                        for (int num2 = 0; num2 < 3; num2++)
                        {
                            for (int num3 = 0; num3 < 3; num3++)
                            {
                                t[i++] = v + (num2 + (num3 * 4));
                                t[i++] = v + ((num2 + 1) + (num3 * 4));
                                t[i++] = v + (num2 + ((num3 + 1) * 4));
                                t[i++] = v + ((num2 + 1) + (num3 * 4));
                                t[i++] = v + ((num2 + 1) + ((num3 + 1) * 4));
                                t[i++] = v + (num2 + ((num3 + 1) * 4));
                            }
                        }
                        v += 0x10;
                    }
                    break;

                case PrimitiveKind.Hole3x3:
                    while (num-- > 0)
                    {
                        for (int num4 = 0; num4 < 3; num4++)
                        {
                            for (int num5 = 0; num5 < 3; num5++)
                            {
                                if ((num4 != 1) || (num5 != 1))
                                {
                                    t[i++] = v + (num4 + (num5 * 4));
                                    t[i++] = v + ((num4 + 1) + (num5 * 4));
                                    t[i++] = v + (num4 + ((num5 + 1) * 4));
                                    t[i++] = v + ((num4 + 1) + (num5 * 4));
                                    t[i++] = v + ((num4 + 1) + ((num5 + 1) * 4));
                                    t[i++] = v + (num4 + ((num5 + 1) * 4));
                                }
                            }
                        }
                        v += 0x10;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}

