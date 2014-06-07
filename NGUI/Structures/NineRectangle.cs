namespace NGUI.Structures
{
    using NGUI.Meshing;
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Explicit)]
    public struct NineRectangle
    {
        private const PrimitiveKind GRID_1ROWS_1COLUMNS = PrimitiveKind.Quad;
        private const PrimitiveKind GRID_1ROWS_2COLUMNS = PrimitiveKind.Grid2x1;
        private const PrimitiveKind GRID_1ROWS_3COLUMNS = PrimitiveKind.Grid3x1;
        private const PrimitiveKind GRID_2ROWS_1COLUMNS = PrimitiveKind.Grid1x2;
        private const PrimitiveKind GRID_2ROWS_2COLUMNS = PrimitiveKind.Grid2x2;
        private const PrimitiveKind GRID_2ROWS_3COLUMNS = PrimitiveKind.Grid3x2;
        private const PrimitiveKind GRID_3ROWS_1COLUMNS = PrimitiveKind.Grid1x3;
        private const PrimitiveKind GRID_3ROWS_2COLUMNS = PrimitiveKind.Grid2x3;
        private const PrimitiveKind GRID_3ROWS_3COLUMNS = PrimitiveKind.Grid3x3;
        [FieldOffset(0x18)]
        public Vector2 ww;
        [FieldOffset(0)]
        public Vector2 xx;
        [FieldOffset(8)]
        public Vector2 yy;
        [FieldOffset(0x10)]
        public Vector2 zz;

        public static void Calculate(UIWidget.Pivot pivot, float pixelSize, Texture tex, ref Vector4 minMaxX, ref Vector4 minMaxY, ref Vector2 scale, out NineRectangle nqV, out NineRectangle nqT)
        {
            if ((tex == null) || (pixelSize == 0f))
            {
                nqV.xx.x = nqV.yy.x = nqV.xx.y = nqV.yy.y = 0f;
                nqV.zz.x = nqV.ww.x = 1f;
                nqV.zz.y = nqV.ww.y = -1f;
                nqT = new NineRectangle();
            }
            else
            {
                Vector2 vector;
                Vector2 vector2;
                float num7;
                float num = (minMaxX.y - minMaxX.x) * pixelSize;
                float num2 = (minMaxX.w - minMaxX.z) * pixelSize;
                float num3 = (minMaxY.z - minMaxY.w) * pixelSize;
                float num4 = (minMaxY.x - minMaxY.y) * pixelSize;
                if (scale.x < 0f)
                {
                    scale.x = 0f;
                    vector.x = num / 0f;
                    vector2.x = num2 / 0f;
                }
                else
                {
                    float num5 = (float) (1.0 / (((double) scale.x) / ((double) tex.width)));
                    vector.x = num * num5;
                    vector2.x = num2 * num5;
                }
                if (scale.y < 0f)
                {
                    scale.y = 0f;
                    vector.y = num3 / 0f;
                    vector2.y = num4 / 0f;
                }
                else
                {
                    float num6 = (float) (1.0 / (((double) scale.y) / ((double) tex.height)));
                    vector.y = num3 * num6;
                    vector2.y = num4 * num6;
                }
                switch (pivot)
                {
                    case UIWidget.Pivot.TopRight:
                    case UIWidget.Pivot.Right:
                    case UIWidget.Pivot.BottomRight:
                        num7 = vector2.x + vector.x;
                        if (num7 <= 1f)
                        {
                            nqV.xx.x = 0f;
                            nqV.ww.x = 1f;
                            nqV.yy.x = vector.x;
                            num7 = 1f - vector2.x;
                            nqV.zz.x = (num7 <= vector.x) ? vector.x : num7;
                        }
                        else
                        {
                            nqV.xx.x = 1f - num7;
                            nqV.yy.x = nqV.xx.x + vector.x;
                            nqV.ww.x = nqV.xx.x + num7;
                            num7 = 1f - vector2.x;
                            nqV.zz.x = nqV.xx.x + ((num7 <= vector.x) ? vector.x : num7);
                        }
                        break;

                    default:
                        nqV.xx.x = 0f;
                        nqV.yy.x = vector.x;
                        num7 = 1f - vector2.x;
                        nqV.zz.x = (num7 <= vector.x) ? vector.x : num7;
                        num7 = vector.x + vector2.x;
                        nqV.ww.x = (num7 <= 1f) ? 1f : num7;
                        break;
                }
                switch (pivot)
                {
                    case UIWidget.Pivot.BottomLeft:
                    case UIWidget.Pivot.Bottom:
                    case UIWidget.Pivot.BottomRight:
                        num7 = (-1f - vector2.y) + vector.y;
                        if (num7 > 0f)
                        {
                            nqV.xx.y = num7;
                            nqV.yy.y = nqV.xx.y + vector.x;
                            num7 = -1f - vector2.y;
                            nqV.zz.y = nqV.xx.y + ((num7 >= vector.y) ? vector.y : num7);
                            num7 = vector.y + vector2.y;
                            nqV.ww.y = nqV.xx.y + ((num7 >= -1f) ? -1f : num7);
                            break;
                        }
                        nqV.xx.y = 0f;
                        nqV.yy.y = vector.y;
                        num7 = -1f - vector2.y;
                        nqV.zz.y = (num7 >= vector.y) ? vector.y : num7;
                        num7 = vector.y + vector2.y;
                        nqV.ww.y = (num7 >= -1f) ? -1f : num7;
                        break;

                    default:
                        nqV.xx.y = 0f;
                        nqV.yy.y = vector.y;
                        num7 = -1f - vector2.y;
                        nqV.zz.y = (num7 >= vector.y) ? vector.y : num7;
                        num7 = vector2.y + vector.y;
                        nqV.ww.y = (num7 >= -1f) ? -1f : num7;
                        break;
                }
                nqT.xx.x = minMaxX.x;
                nqT.yy.x = minMaxX.y;
                nqT.zz.x = minMaxX.z;
                nqT.ww.x = minMaxX.w;
                nqT.xx.y = minMaxY.w;
                nqT.yy.y = minMaxY.z;
                nqT.zz.y = minMaxY.y;
                nqT.ww.y = minMaxY.x;
            }
        }

        private static void Commit3x3(int start, ref NineRectangle nqV, ref NineRectangle nqT, ref Color color, MeshBuffer m)
        {
            m.v[start].x = nqV.xx.x;
            m.v[start].y = nqV.xx.y;
            m.v[start].u = nqT.xx.x;
            m.v[start].v = nqT.xx.y;
            m.v[start + 1].x = nqV.yx.x;
            m.v[start + 1].y = nqV.yx.y;
            m.v[start + 1].u = nqT.yx.x;
            m.v[start + 1].v = nqT.yx.y;
            m.v[start + 2].x = nqV.zx.x;
            m.v[start + 2].y = nqV.zx.y;
            m.v[start + 2].u = nqT.zx.x;
            m.v[start + 2].v = nqT.zx.y;
            m.v[start + 3].x = nqV.wx.x;
            m.v[start + 3].y = nqV.wx.y;
            m.v[start + 3].u = nqT.wx.x;
            m.v[start + 3].v = nqT.wx.y;
            m.v[start + 4].x = nqV.xy.x;
            m.v[start + 4].y = nqV.xy.y;
            m.v[start + 4].u = nqT.xy.x;
            m.v[start + 4].v = nqT.xy.y;
            m.v[(start + 1) + 4].x = nqV.yy.x;
            m.v[(start + 1) + 4].y = nqV.yy.y;
            m.v[(start + 1) + 4].u = nqT.yy.x;
            m.v[(start + 1) + 4].v = nqT.yy.y;
            m.v[(start + 2) + 4].x = nqV.zy.x;
            m.v[(start + 2) + 4].y = nqV.zy.y;
            m.v[(start + 2) + 4].u = nqT.zy.x;
            m.v[(start + 2) + 4].v = nqT.zy.y;
            m.v[(start + 3) + 4].x = nqV.wy.x;
            m.v[(start + 3) + 4].y = nqV.wy.y;
            m.v[(start + 3) + 4].u = nqT.wy.x;
            m.v[(start + 3) + 4].v = nqT.wy.y;
            m.v[start + 8].x = nqV.xz.x;
            m.v[start + 8].y = nqV.xz.y;
            m.v[start + 8].u = nqT.xz.x;
            m.v[start + 8].v = nqT.xz.y;
            m.v[(start + 1) + 8].x = nqV.yz.x;
            m.v[(start + 1) + 8].y = nqV.yz.y;
            m.v[(start + 1) + 8].u = nqT.yz.x;
            m.v[(start + 1) + 8].v = nqT.yz.y;
            m.v[(start + 2) + 8].x = nqV.zz.x;
            m.v[(start + 2) + 8].y = nqV.zz.y;
            m.v[(start + 2) + 8].u = nqT.zz.x;
            m.v[(start + 2) + 8].v = nqT.zz.y;
            m.v[(start + 3) + 8].x = nqV.wz.x;
            m.v[(start + 3) + 8].y = nqV.wz.y;
            m.v[(start + 3) + 8].u = nqT.wz.x;
            m.v[(start + 3) + 8].v = nqT.wz.y;
            m.v[start + 12].x = nqV.xw.x;
            m.v[start + 12].y = nqV.xw.y;
            m.v[start + 12].u = nqT.xw.x;
            m.v[start + 12].v = nqT.xw.y;
            m.v[(start + 1) + 12].x = nqV.yw.x;
            m.v[(start + 1) + 12].y = nqV.yw.y;
            m.v[(start + 1) + 12].u = nqT.yw.x;
            m.v[(start + 1) + 12].v = nqT.yw.y;
            m.v[(start + 2) + 12].x = nqV.zw.x;
            m.v[(start + 2) + 12].y = nqV.zw.y;
            m.v[(start + 2) + 12].u = nqT.zw.x;
            m.v[(start + 2) + 12].v = nqT.zw.y;
            m.v[(start + 3) + 12].x = nqV.ww.x;
            m.v[(start + 3) + 12].y = nqV.ww.y;
            m.v[(start + 3) + 12].u = nqT.ww.x;
            m.v[(start + 3) + 12].v = nqT.ww.y;
            for (int i = 0; i < 0x10; i++)
            {
                m.v[start + i].z = 0f;
                m.v[start + i].r = color.r;
                m.v[start + i].g = color.g;
                m.v[start + i].b = color.b;
                m.v[start + i].a = color.a;
            }
        }

        public static void Fill8(ref NineRectangle nqV, ref NineRectangle nqT, ref Color color, MeshBuffer m)
        {
            Commit3x3(m.Alloc(PrimitiveKind.Hole3x3), ref nqV, ref nqT, ref color, m);
        }

        public static void Fill9(ref NineRectangle nqV, ref NineRectangle nqT, ref Color color, MeshBuffer m)
        {
            if (nqV.xx.x == nqV.yy.x)
            {
                if (nqV.yy.x == nqV.zz.x)
                {
                    if (nqV.zz.x != nqT.ww.x)
                    {
                        FillColumn1(ref nqV, ref nqT, 2, ref color, m);
                    }
                }
                else if (nqV.zz.x == nqT.ww.x)
                {
                    FillColumn1(ref nqV, ref nqT, 1, ref color, m);
                }
                else
                {
                    FillColumn2(ref nqV, ref nqT, 1, ref color, m);
                }
            }
            else if (nqV.yy.x == nqV.zz.x)
            {
                if (nqV.zz.x == nqV.ww.x)
                {
                    FillColumn1(ref nqV, ref nqT, 1, ref color, m);
                }
                else
                {
                    FillColumn2(ref nqV, ref nqT, 2, ref color, m);
                }
            }
            else if (nqV.zz.x == nqV.ww.x)
            {
                FillColumn2(ref nqV, ref nqT, 0, ref color, m);
            }
            else
            {
                FillColumn3(ref nqV, ref nqT, ref color, m);
            }
        }

        private static void FillColumn1(ref NineRectangle nqV, ref NineRectangle nqT, int columnStart, ref Color color, MeshBuffer m)
        {
            if (nqV.xx.y == nqV.yy.y)
            {
                if (nqV.yy.y == nqV.zz.y)
                {
                    if (nqV.zz.y != nqV.ww.y)
                    {
                        switch (columnStart)
                        {
                            case 0:
                                m.FastCell(nqV.xz, nqV.yw, nqT.xz, nqT.yw, ref color);
                                break;

                            case 1:
                                m.FastCell(nqV.yz, nqV.zw, nqT.yz, nqT.zw, ref color);
                                break;

                            case 2:
                                m.FastCell(nqV.zz, nqV.ww, nqT.zz, nqT.ww, ref color);
                                break;
                        }
                    }
                }
                else if (nqV.zz.y == nqV.ww.y)
                {
                    switch (columnStart)
                    {
                        case 0:
                            m.FastCell(nqV.xy, nqV.yz, nqT.xy, nqT.yz, ref color);
                            break;

                        case 1:
                            m.FastCell(nqV.yy, nqV.zz, nqT.yy, nqT.zz, ref color);
                            break;

                        case 2:
                            m.FastCell(nqV.zy, nqV.wz, nqT.zy, nqT.wz, ref color);
                            break;
                    }
                }
                else
                {
                    int index = m.Alloc(PrimitiveKind.Grid1x2, 0f, (Color) color);
                    switch (columnStart)
                    {
                        case 0:
                            m.v[index].x = nqV.xx.x;
                            m.v[index].y = nqV.yy.y;
                            m.v[index].u = nqT.xx.x;
                            m.v[index].v = nqT.yy.y;
                            m.v[index + 1].x = nqV.yy.x;
                            m.v[index + 1].y = nqV.yy.y;
                            m.v[index + 1].u = nqT.yy.x;
                            m.v[index + 1].v = nqT.yy.y;
                            m.v[index + 2].x = nqV.xx.x;
                            m.v[index + 2].y = nqV.zz.y;
                            m.v[index + 2].u = nqT.xx.x;
                            m.v[index + 2].v = nqT.zz.y;
                            m.v[index + 3].x = nqV.yy.x;
                            m.v[index + 3].y = nqV.zz.y;
                            m.v[index + 3].u = nqT.yy.x;
                            m.v[index + 3].v = nqT.zz.y;
                            m.v[index + 4].x = nqV.xx.x;
                            m.v[index + 4].y = nqV.ww.y;
                            m.v[index + 4].u = nqT.xx.x;
                            m.v[index + 4].v = nqT.ww.y;
                            m.v[index + 5].x = nqV.yy.x;
                            m.v[index + 5].y = nqV.ww.y;
                            m.v[index + 5].u = nqT.yy.x;
                            m.v[index + 5].v = nqT.ww.y;
                            break;

                        case 1:
                            m.v[index].x = nqV.yy.x;
                            m.v[index].y = nqV.yy.y;
                            m.v[index].u = nqT.yy.x;
                            m.v[index].v = nqT.yy.y;
                            m.v[index + 1].x = nqV.zz.x;
                            m.v[index + 1].y = nqV.yy.y;
                            m.v[index + 1].u = nqT.zz.x;
                            m.v[index + 1].v = nqT.yy.y;
                            m.v[index + 2].x = nqV.yy.x;
                            m.v[index + 2].y = nqV.zz.y;
                            m.v[index + 2].u = nqT.yy.x;
                            m.v[index + 2].v = nqT.zz.y;
                            m.v[index + 3].x = nqV.zz.x;
                            m.v[index + 3].y = nqV.zz.y;
                            m.v[index + 3].u = nqT.zz.x;
                            m.v[index + 3].v = nqT.zz.y;
                            m.v[index + 4].x = nqV.yy.x;
                            m.v[index + 4].y = nqV.ww.y;
                            m.v[index + 4].u = nqT.yy.x;
                            m.v[index + 4].v = nqT.ww.y;
                            m.v[index + 5].x = nqV.zz.x;
                            m.v[index + 5].y = nqV.ww.y;
                            m.v[index + 5].u = nqT.zz.x;
                            m.v[index + 5].v = nqT.ww.y;
                            break;

                        case 2:
                            m.v[index].x = nqV.zz.x;
                            m.v[index].y = nqV.yy.y;
                            m.v[index].u = nqT.zz.x;
                            m.v[index].v = nqT.yy.y;
                            m.v[index + 1].x = nqV.ww.x;
                            m.v[index + 1].y = nqV.yy.y;
                            m.v[index + 1].u = nqT.ww.x;
                            m.v[index + 1].v = nqT.yy.y;
                            m.v[index + 2].x = nqV.zz.x;
                            m.v[index + 2].y = nqV.zz.y;
                            m.v[index + 2].u = nqT.zz.x;
                            m.v[index + 2].v = nqT.zz.y;
                            m.v[index + 3].x = nqV.ww.x;
                            m.v[index + 3].y = nqV.zz.y;
                            m.v[index + 3].u = nqT.ww.x;
                            m.v[index + 3].v = nqT.zz.y;
                            m.v[index + 4].x = nqV.zz.x;
                            m.v[index + 4].y = nqV.ww.y;
                            m.v[index + 4].u = nqT.zz.x;
                            m.v[index + 4].v = nqT.ww.y;
                            m.v[index + 5].x = nqV.ww.x;
                            m.v[index + 5].y = nqV.ww.y;
                            m.v[index + 5].u = nqT.ww.x;
                            m.v[index + 5].v = nqT.ww.y;
                            break;
                    }
                }
            }
            else if (nqV.yy.y == nqV.zz.y)
            {
                if (nqV.zz.y == nqV.ww.y)
                {
                    switch (columnStart)
                    {
                        case 0:
                            m.FastCell(nqV.xx, nqV.yy, nqT.xx, nqT.yy, ref color);
                            break;

                        case 1:
                            m.FastCell(nqV.yx, nqV.zy, nqT.yx, nqT.zy, ref color);
                            break;

                        case 2:
                            m.FastCell(nqV.zx, nqV.wy, nqT.zx, nqT.wy, ref color);
                            break;
                    }
                }
                else
                {
                    switch (columnStart)
                    {
                        case 0:
                            m.FastCell(nqV.xx, nqV.yy, nqT.xx, nqT.yy, ref color);
                            m.FastCell(nqV.xz, nqV.yw, nqT.xz, nqT.yw, ref color);
                            break;

                        case 1:
                            m.FastCell(nqV.yx, nqV.zy, nqT.yx, nqT.zy, ref color);
                            m.FastCell(nqV.yz, nqV.zw, nqT.yz, nqT.zw, ref color);
                            break;

                        case 2:
                            m.FastCell(nqV.zx, nqV.wy, nqT.zx, nqT.wy, ref color);
                            m.FastCell(nqV.zz, nqV.ww, nqT.zz, nqT.ww, ref color);
                            break;
                    }
                }
            }
            else if (nqV.zz.y == nqV.ww.y)
            {
                int num2 = m.Alloc(PrimitiveKind.Grid1x2, 0f, (Color) color);
                switch (columnStart)
                {
                    case 0:
                        m.v[num2].x = nqV.xx.x;
                        m.v[num2].y = nqV.xx.y;
                        m.v[num2].u = nqT.xx.x;
                        m.v[num2].v = nqT.xx.y;
                        m.v[num2 + 1].x = nqV.yy.x;
                        m.v[num2 + 1].y = nqV.xx.y;
                        m.v[num2 + 1].u = nqT.yy.x;
                        m.v[num2 + 1].v = nqT.xx.y;
                        m.v[num2 + 2].x = nqV.xx.x;
                        m.v[num2 + 2].y = nqV.yy.y;
                        m.v[num2 + 2].u = nqT.xx.x;
                        m.v[num2 + 2].v = nqT.yy.y;
                        m.v[num2 + 3].x = nqV.yy.x;
                        m.v[num2 + 3].y = nqV.yy.y;
                        m.v[num2 + 3].u = nqT.yy.x;
                        m.v[num2 + 3].v = nqT.yy.y;
                        m.v[num2 + 4].x = nqV.xx.x;
                        m.v[num2 + 4].y = nqV.zz.y;
                        m.v[num2 + 4].u = nqT.xx.x;
                        m.v[num2 + 4].v = nqT.zz.y;
                        m.v[num2 + 5].x = nqV.yy.x;
                        m.v[num2 + 5].y = nqV.zz.y;
                        m.v[num2 + 5].u = nqT.yy.x;
                        m.v[num2 + 5].v = nqT.zz.y;
                        break;

                    case 1:
                        m.v[num2].x = nqV.yy.x;
                        m.v[num2].y = nqV.xx.y;
                        m.v[num2].u = nqT.yy.x;
                        m.v[num2].v = nqT.xx.y;
                        m.v[num2 + 1].x = nqV.zz.x;
                        m.v[num2 + 1].y = nqV.xx.y;
                        m.v[num2 + 1].u = nqT.zz.x;
                        m.v[num2 + 1].v = nqT.xx.y;
                        m.v[num2 + 2].x = nqV.yy.x;
                        m.v[num2 + 2].y = nqV.yy.y;
                        m.v[num2 + 2].u = nqT.yy.x;
                        m.v[num2 + 2].v = nqT.yy.y;
                        m.v[num2 + 3].x = nqV.zz.x;
                        m.v[num2 + 3].y = nqV.yy.y;
                        m.v[num2 + 3].u = nqT.zz.x;
                        m.v[num2 + 3].v = nqT.yy.y;
                        m.v[num2 + 4].x = nqV.yy.x;
                        m.v[num2 + 4].y = nqV.zz.y;
                        m.v[num2 + 4].u = nqT.yy.x;
                        m.v[num2 + 4].v = nqT.zz.y;
                        m.v[num2 + 5].x = nqV.zz.x;
                        m.v[num2 + 5].y = nqV.zz.y;
                        m.v[num2 + 5].u = nqT.zz.x;
                        m.v[num2 + 5].v = nqT.zz.y;
                        break;

                    case 2:
                        m.v[num2].x = nqV.zz.x;
                        m.v[num2].y = nqV.xx.y;
                        m.v[num2].u = nqT.zz.x;
                        m.v[num2].v = nqT.xx.y;
                        m.v[num2 + 1].x = nqV.ww.x;
                        m.v[num2 + 1].y = nqV.xx.y;
                        m.v[num2 + 1].u = nqT.ww.x;
                        m.v[num2 + 1].v = nqT.xx.y;
                        m.v[num2 + 2].x = nqV.zz.x;
                        m.v[num2 + 2].y = nqV.yy.y;
                        m.v[num2 + 2].u = nqT.zz.x;
                        m.v[num2 + 2].v = nqT.yy.y;
                        m.v[num2 + 3].x = nqV.ww.x;
                        m.v[num2 + 3].y = nqV.yy.y;
                        m.v[num2 + 3].u = nqT.ww.x;
                        m.v[num2 + 3].v = nqT.yy.y;
                        m.v[num2 + 4].x = nqV.zz.x;
                        m.v[num2 + 4].y = nqV.zz.y;
                        m.v[num2 + 4].u = nqT.zz.x;
                        m.v[num2 + 4].v = nqT.zz.y;
                        m.v[num2 + 5].x = nqV.ww.x;
                        m.v[num2 + 5].y = nqV.zz.y;
                        m.v[num2 + 5].u = nqT.ww.x;
                        m.v[num2 + 5].v = nqT.zz.y;
                        break;
                }
            }
            else
            {
                int num3 = m.Alloc(PrimitiveKind.Grid1x2, 0f, (Color) color);
                switch (columnStart)
                {
                    case 0:
                        m.v[num3].x = nqV.xx.x;
                        m.v[num3].y = nqV.xx.y;
                        m.v[num3].u = nqT.xx.x;
                        m.v[num3].v = nqT.xx.y;
                        m.v[num3 + 1].x = nqV.yy.x;
                        m.v[num3 + 1].y = nqV.xx.y;
                        m.v[num3 + 1].u = nqT.yy.x;
                        m.v[num3 + 1].v = nqT.xx.y;
                        m.v[num3 + 2].x = nqV.xx.x;
                        m.v[num3 + 2].y = nqV.yy.y;
                        m.v[num3 + 2].u = nqT.xx.x;
                        m.v[num3 + 2].v = nqT.yy.y;
                        m.v[num3 + 3].x = nqV.yy.x;
                        m.v[num3 + 3].y = nqV.yy.y;
                        m.v[num3 + 3].u = nqT.yy.x;
                        m.v[num3 + 3].v = nqT.yy.y;
                        m.v[num3 + 4].x = nqV.xx.x;
                        m.v[num3 + 4].y = nqV.zz.y;
                        m.v[num3 + 4].u = nqT.xx.x;
                        m.v[num3 + 4].v = nqT.zz.y;
                        m.v[num3 + 5].x = nqV.yy.x;
                        m.v[num3 + 5].y = nqV.zz.y;
                        m.v[num3 + 5].u = nqT.yy.x;
                        m.v[num3 + 5].v = nqT.zz.y;
                        m.v[num3 + 6].x = nqV.xx.x;
                        m.v[num3 + 6].y = nqV.ww.y;
                        m.v[num3 + 6].u = nqT.xx.x;
                        m.v[num3 + 6].v = nqT.ww.y;
                        m.v[num3 + 7].x = nqV.yy.x;
                        m.v[num3 + 7].y = nqV.ww.y;
                        m.v[num3 + 7].u = nqT.yy.x;
                        m.v[num3 + 7].v = nqT.ww.y;
                        break;

                    case 1:
                        m.v[num3].x = nqV.yy.x;
                        m.v[num3].y = nqV.xx.y;
                        m.v[num3].u = nqT.yy.x;
                        m.v[num3].v = nqT.xx.y;
                        m.v[num3 + 1].x = nqV.zz.x;
                        m.v[num3 + 1].y = nqV.xx.y;
                        m.v[num3 + 1].u = nqT.zz.x;
                        m.v[num3 + 1].v = nqT.xx.y;
                        m.v[num3 + 2].x = nqV.yy.x;
                        m.v[num3 + 2].y = nqV.yy.y;
                        m.v[num3 + 2].u = nqT.yy.x;
                        m.v[num3 + 2].v = nqT.yy.y;
                        m.v[num3 + 3].x = nqV.zz.x;
                        m.v[num3 + 3].y = nqV.yy.y;
                        m.v[num3 + 3].u = nqT.zz.x;
                        m.v[num3 + 3].v = nqT.yy.y;
                        m.v[num3 + 4].x = nqV.yy.x;
                        m.v[num3 + 4].y = nqV.zz.y;
                        m.v[num3 + 4].u = nqT.yy.x;
                        m.v[num3 + 4].v = nqT.zz.y;
                        m.v[num3 + 5].x = nqV.zz.x;
                        m.v[num3 + 5].y = nqV.zz.y;
                        m.v[num3 + 5].u = nqT.zz.x;
                        m.v[num3 + 5].v = nqT.zz.y;
                        m.v[num3 + 6].x = nqV.yy.x;
                        m.v[num3 + 6].y = nqV.ww.y;
                        m.v[num3 + 6].u = nqT.yy.x;
                        m.v[num3 + 6].v = nqT.ww.y;
                        m.v[num3 + 7].x = nqV.zz.x;
                        m.v[num3 + 7].y = nqV.ww.y;
                        m.v[num3 + 7].u = nqT.zz.x;
                        m.v[num3 + 7].v = nqT.ww.y;
                        break;

                    case 2:
                        m.v[num3].x = nqV.zz.x;
                        m.v[num3].y = nqV.xx.y;
                        m.v[num3].u = nqT.zz.x;
                        m.v[num3].v = nqT.xx.y;
                        m.v[num3 + 1].x = nqV.ww.x;
                        m.v[num3 + 1].y = nqV.xx.y;
                        m.v[num3 + 1].u = nqT.ww.x;
                        m.v[num3 + 1].v = nqT.xx.y;
                        m.v[num3 + 2].x = nqV.zz.x;
                        m.v[num3 + 2].y = nqV.yy.y;
                        m.v[num3 + 2].u = nqT.zz.x;
                        m.v[num3 + 2].v = nqT.yy.y;
                        m.v[num3 + 3].x = nqV.ww.x;
                        m.v[num3 + 3].y = nqV.yy.y;
                        m.v[num3 + 3].u = nqT.ww.x;
                        m.v[num3 + 3].v = nqT.yy.y;
                        m.v[num3 + 4].x = nqV.zz.x;
                        m.v[num3 + 4].y = nqV.zz.y;
                        m.v[num3 + 4].u = nqT.zz.x;
                        m.v[num3 + 4].v = nqT.zz.y;
                        m.v[num3 + 5].x = nqV.ww.x;
                        m.v[num3 + 5].y = nqV.zz.y;
                        m.v[num3 + 5].u = nqT.ww.x;
                        m.v[num3 + 5].v = nqT.zz.y;
                        m.v[num3 + 6].x = nqV.zz.x;
                        m.v[num3 + 6].y = nqV.ww.y;
                        m.v[num3 + 6].u = nqT.zz.x;
                        m.v[num3 + 6].v = nqT.ww.y;
                        m.v[num3 + 7].x = nqV.ww.x;
                        m.v[num3 + 7].y = nqV.ww.y;
                        m.v[num3 + 7].u = nqT.ww.x;
                        m.v[num3 + 7].v = nqT.ww.y;
                        break;
                }
            }
        }

        private static void FillColumn2(ref NineRectangle nqV, ref NineRectangle nqT, int columnStart, ref Color color, MeshBuffer m)
        {
            if (nqV.xx.y == nqV.yy.y)
            {
                if (nqV.yy.y == nqV.zz.y)
                {
                    if (nqV.zz.y != nqV.ww.y)
                    {
                        int num;
                        switch (columnStart)
                        {
                            case 0:
                                num = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                                m.v[num].x = nqV.xx.x;
                                m.v[num].y = nqV.zz.y;
                                m.v[num].u = nqT.xx.x;
                                m.v[num].v = nqT.zz.y;
                                m.v[num + 1].x = nqV.yy.x;
                                m.v[num + 1].y = nqV.zz.y;
                                m.v[num + 1].u = nqT.yy.x;
                                m.v[num + 1].v = nqT.zz.y;
                                m.v[num + 2].x = nqV.zz.x;
                                m.v[num + 2].y = nqV.zz.y;
                                m.v[num + 2].u = nqT.zz.x;
                                m.v[num + 2].v = nqT.zz.y;
                                m.v[num + 3].x = nqV.xx.x;
                                m.v[num + 3].y = nqV.ww.y;
                                m.v[num + 3].u = nqT.xx.x;
                                m.v[num + 3].v = nqT.ww.y;
                                m.v[num + 4].x = nqV.yy.x;
                                m.v[num + 4].y = nqV.ww.y;
                                m.v[num + 4].u = nqT.yy.x;
                                m.v[num + 4].v = nqT.ww.y;
                                m.v[num + 5].x = nqV.zz.x;
                                m.v[num + 5].y = nqV.ww.y;
                                m.v[num + 5].u = nqT.zz.x;
                                m.v[num + 5].v = nqT.ww.y;
                                break;

                            case 1:
                                num = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                                m.v[num].x = nqV.yy.x;
                                m.v[num].y = nqV.zz.y;
                                m.v[num].u = nqT.yy.x;
                                m.v[num].v = nqT.zz.y;
                                m.v[num + 1].x = nqV.zz.x;
                                m.v[num + 1].y = nqV.zz.y;
                                m.v[num + 1].u = nqT.zz.x;
                                m.v[num + 1].v = nqT.zz.y;
                                m.v[num + 2].x = nqV.ww.x;
                                m.v[num + 2].y = nqV.zz.y;
                                m.v[num + 2].u = nqT.ww.x;
                                m.v[num + 2].v = nqT.zz.y;
                                m.v[num + 3].x = nqV.yy.x;
                                m.v[num + 3].y = nqV.ww.y;
                                m.v[num + 3].u = nqT.yy.x;
                                m.v[num + 3].v = nqT.ww.y;
                                m.v[num + 4].x = nqV.zz.x;
                                m.v[num + 4].y = nqV.ww.y;
                                m.v[num + 4].u = nqT.zz.x;
                                m.v[num + 4].v = nqT.ww.y;
                                m.v[num + 5].x = nqV.ww.x;
                                m.v[num + 5].y = nqV.ww.y;
                                m.v[num + 5].u = nqT.ww.x;
                                m.v[num + 5].v = nqT.ww.y;
                                break;

                            case 2:
                                m.FastCell(nqV.xz, nqV.yw, nqT.xz, nqT.yw, ref color);
                                m.FastCell(nqV.zz, nqV.ww, nqT.zz, nqT.ww, ref color);
                                break;
                        }
                    }
                }
                else if (nqV.zz.y == nqV.ww.y)
                {
                    int num2;
                    switch (columnStart)
                    {
                        case 0:
                            num2 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num2].x = nqV.xx.x;
                            m.v[num2].y = nqV.yy.y;
                            m.v[num2].u = nqT.xx.x;
                            m.v[num2].v = nqT.yy.y;
                            m.v[num2 + 1].x = nqV.yy.x;
                            m.v[num2 + 1].y = nqV.yy.y;
                            m.v[num2 + 1].u = nqT.yy.x;
                            m.v[num2 + 1].v = nqT.yy.y;
                            m.v[num2 + 2].x = nqV.zz.x;
                            m.v[num2 + 2].y = nqV.yy.y;
                            m.v[num2 + 2].u = nqT.zz.x;
                            m.v[num2 + 2].v = nqT.yy.y;
                            m.v[num2 + 3].x = nqV.xx.x;
                            m.v[num2 + 3].y = nqV.zz.y;
                            m.v[num2 + 3].u = nqT.xx.x;
                            m.v[num2 + 3].v = nqT.zz.y;
                            m.v[num2 + 4].x = nqV.yy.x;
                            m.v[num2 + 4].y = nqV.zz.y;
                            m.v[num2 + 4].u = nqT.yy.x;
                            m.v[num2 + 4].v = nqT.zz.y;
                            m.v[num2 + 5].x = nqV.zz.x;
                            m.v[num2 + 5].y = nqV.zz.y;
                            m.v[num2 + 5].u = nqT.zz.x;
                            m.v[num2 + 5].v = nqT.zz.y;
                            break;

                        case 1:
                            num2 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num2].x = nqV.yy.x;
                            m.v[num2].y = nqV.yy.y;
                            m.v[num2].u = nqT.yy.x;
                            m.v[num2].v = nqT.yy.y;
                            m.v[num2 + 1].x = nqV.zz.x;
                            m.v[num2 + 1].y = nqV.yy.y;
                            m.v[num2 + 1].u = nqT.zz.x;
                            m.v[num2 + 1].v = nqT.yy.y;
                            m.v[num2 + 2].x = nqV.ww.x;
                            m.v[num2 + 2].y = nqV.yy.y;
                            m.v[num2 + 2].u = nqT.ww.x;
                            m.v[num2 + 2].v = nqT.yy.y;
                            m.v[num2 + 3].x = nqV.yy.x;
                            m.v[num2 + 3].y = nqV.zz.y;
                            m.v[num2 + 3].u = nqT.yy.x;
                            m.v[num2 + 3].v = nqT.zz.y;
                            m.v[num2 + 4].x = nqV.zz.x;
                            m.v[num2 + 4].y = nqV.zz.y;
                            m.v[num2 + 4].u = nqT.zz.x;
                            m.v[num2 + 4].v = nqT.zz.y;
                            m.v[num2 + 5].x = nqV.ww.x;
                            m.v[num2 + 5].y = nqV.zz.y;
                            m.v[num2 + 5].u = nqT.ww.x;
                            m.v[num2 + 5].v = nqT.zz.y;
                            break;

                        case 2:
                            m.FastCell(nqV.xy, nqV.yz, nqT.xy, nqT.yz, ref color);
                            m.FastCell(nqV.zy, nqV.wz, nqT.zy, nqT.wz, ref color);
                            break;
                    }
                }
                else
                {
                    int num3;
                    switch (columnStart)
                    {
                        case 0:
                            num3 = m.Alloc(PrimitiveKind.Grid2x2, 0f, (Color) color);
                            m.v[num3].x = nqV.xx.x;
                            m.v[num3].y = nqV.yy.y;
                            m.v[num3].u = nqT.xx.x;
                            m.v[num3].v = nqT.yy.y;
                            m.v[num3 + 1].x = nqV.yy.x;
                            m.v[num3 + 1].y = nqV.yy.y;
                            m.v[num3 + 1].u = nqT.yy.x;
                            m.v[num3 + 1].v = nqT.yy.y;
                            m.v[num3 + 2].x = nqV.zz.x;
                            m.v[num3 + 2].y = nqV.yy.y;
                            m.v[num3 + 2].u = nqT.zz.x;
                            m.v[num3 + 2].v = nqT.yy.y;
                            m.v[num3 + 3].x = nqV.xx.x;
                            m.v[num3 + 3].y = nqV.zz.y;
                            m.v[num3 + 3].u = nqT.xx.x;
                            m.v[num3 + 3].v = nqT.zz.y;
                            m.v[num3 + 4].x = nqV.yy.x;
                            m.v[num3 + 4].y = nqV.zz.y;
                            m.v[num3 + 4].u = nqT.yy.x;
                            m.v[num3 + 4].v = nqT.zz.y;
                            m.v[num3 + 5].x = nqV.zz.x;
                            m.v[num3 + 5].y = nqV.zz.y;
                            m.v[num3 + 5].u = nqT.zz.x;
                            m.v[num3 + 5].v = nqT.zz.y;
                            m.v[num3 + 6].x = nqV.xx.x;
                            m.v[num3 + 6].y = nqV.ww.y;
                            m.v[num3 + 6].u = nqT.xx.x;
                            m.v[num3 + 6].v = nqT.ww.y;
                            m.v[num3 + 7].x = nqV.yy.x;
                            m.v[num3 + 7].y = nqV.ww.y;
                            m.v[num3 + 7].u = nqT.yy.x;
                            m.v[num3 + 7].v = nqT.ww.y;
                            m.v[num3 + 8].x = nqV.zz.x;
                            m.v[num3 + 8].y = nqV.ww.y;
                            m.v[num3 + 8].u = nqT.zz.x;
                            m.v[num3 + 8].v = nqT.ww.y;
                            break;

                        case 1:
                            num3 = m.Alloc(PrimitiveKind.Grid2x2, 0f, (Color) color);
                            m.v[num3].x = nqV.yy.x;
                            m.v[num3].y = nqV.yy.y;
                            m.v[num3].u = nqT.yy.x;
                            m.v[num3].v = nqT.yy.y;
                            m.v[num3 + 1].x = nqV.zz.x;
                            m.v[num3 + 1].y = nqV.yy.y;
                            m.v[num3 + 1].u = nqT.zz.x;
                            m.v[num3 + 1].v = nqT.yy.y;
                            m.v[num3 + 2].x = nqV.ww.x;
                            m.v[num3 + 2].y = nqV.yy.y;
                            m.v[num3 + 2].u = nqT.ww.x;
                            m.v[num3 + 2].v = nqT.yy.y;
                            m.v[num3 + 3].x = nqV.yy.x;
                            m.v[num3 + 3].y = nqV.zz.y;
                            m.v[num3 + 3].u = nqT.yy.x;
                            m.v[num3 + 3].v = nqT.zz.y;
                            m.v[num3 + 4].x = nqV.zz.x;
                            m.v[num3 + 4].y = nqV.zz.y;
                            m.v[num3 + 4].u = nqT.zz.x;
                            m.v[num3 + 4].v = nqT.zz.y;
                            m.v[num3 + 5].x = nqV.ww.x;
                            m.v[num3 + 5].y = nqV.zz.y;
                            m.v[num3 + 5].u = nqT.ww.x;
                            m.v[num3 + 5].v = nqT.zz.y;
                            m.v[num3 + 6].x = nqV.yy.x;
                            m.v[num3 + 6].y = nqV.ww.y;
                            m.v[num3 + 6].u = nqT.yy.x;
                            m.v[num3 + 6].v = nqT.ww.y;
                            m.v[num3 + 7].x = nqV.zz.x;
                            m.v[num3 + 7].y = nqV.ww.y;
                            m.v[num3 + 7].u = nqT.zz.x;
                            m.v[num3 + 7].v = nqT.ww.y;
                            m.v[num3 + 8].x = nqV.ww.x;
                            m.v[num3 + 8].y = nqV.ww.y;
                            m.v[num3 + 8].u = nqT.ww.x;
                            m.v[num3 + 8].v = nqT.ww.y;
                            break;

                        case 2:
                            num3 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num3].x = nqV.xx.x;
                            m.v[num3].y = nqV.yy.y;
                            m.v[num3].u = nqT.xx.x;
                            m.v[num3].v = nqT.yy.y;
                            m.v[num3 + 1].x = nqV.yy.x;
                            m.v[num3 + 1].y = nqV.yy.y;
                            m.v[num3 + 1].u = nqT.yy.x;
                            m.v[num3 + 1].v = nqT.yy.y;
                            m.v[num3 + 2].x = nqV.xx.x;
                            m.v[num3 + 2].y = nqV.zz.y;
                            m.v[num3 + 2].u = nqT.xx.x;
                            m.v[num3 + 2].v = nqT.zz.y;
                            m.v[num3 + 3].x = nqV.yy.x;
                            m.v[num3 + 3].y = nqV.zz.y;
                            m.v[num3 + 3].u = nqT.yy.x;
                            m.v[num3 + 3].v = nqT.zz.y;
                            m.v[num3 + 4].x = nqV.yy.x;
                            m.v[num3 + 4].y = nqV.ww.y;
                            m.v[num3 + 4].u = nqT.yy.x;
                            m.v[num3 + 4].v = nqT.ww.y;
                            m.v[num3 + 5].x = nqV.zz.x;
                            m.v[num3 + 5].y = nqV.ww.y;
                            m.v[num3 + 5].u = nqT.zz.x;
                            m.v[num3 + 5].v = nqT.ww.y;
                            num3 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num3].x = nqV.zz.x;
                            m.v[num3].y = nqV.yy.y;
                            m.v[num3].u = nqT.zz.x;
                            m.v[num3].v = nqT.yy.y;
                            m.v[num3 + 1].x = nqV.ww.x;
                            m.v[num3 + 1].y = nqV.yy.y;
                            m.v[num3 + 1].u = nqT.ww.x;
                            m.v[num3 + 1].v = nqT.yy.y;
                            m.v[num3 + 2].x = nqV.zz.x;
                            m.v[num3 + 2].y = nqV.zz.y;
                            m.v[num3 + 2].u = nqT.zz.x;
                            m.v[num3 + 2].v = nqT.zz.y;
                            m.v[num3 + 3].x = nqV.ww.x;
                            m.v[num3 + 3].y = nqV.zz.y;
                            m.v[num3 + 3].u = nqT.ww.x;
                            m.v[num3 + 3].v = nqT.zz.y;
                            m.v[num3 + 4].x = nqV.zz.x;
                            m.v[num3 + 4].y = nqV.ww.y;
                            m.v[num3 + 4].u = nqT.zz.x;
                            m.v[num3 + 4].v = nqT.ww.y;
                            m.v[num3 + 5].x = nqV.ww.x;
                            m.v[num3 + 5].y = nqV.ww.y;
                            m.v[num3 + 5].u = nqT.ww.x;
                            m.v[num3 + 5].v = nqT.ww.y;
                            break;
                    }
                }
            }
            else if (nqV.yy.y == nqV.zz.y)
            {
                if (nqV.zz.y == nqV.ww.y)
                {
                    int num4;
                    switch (columnStart)
                    {
                        case 0:
                            num4 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num4].x = nqV.xx.x;
                            m.v[num4].y = nqV.xx.y;
                            m.v[num4].u = nqT.xx.x;
                            m.v[num4].v = nqT.xx.y;
                            m.v[num4 + 1].x = nqV.yy.x;
                            m.v[num4 + 1].y = nqV.xx.y;
                            m.v[num4 + 1].u = nqT.yy.x;
                            m.v[num4 + 1].v = nqT.xx.y;
                            m.v[num4 + 2].x = nqV.zz.x;
                            m.v[num4 + 2].y = nqV.xx.y;
                            m.v[num4 + 2].u = nqT.zz.x;
                            m.v[num4 + 2].v = nqT.xx.y;
                            m.v[num4 + 3].x = nqV.xx.x;
                            m.v[num4 + 3].y = nqV.yy.y;
                            m.v[num4 + 3].u = nqT.xx.x;
                            m.v[num4 + 3].v = nqT.yy.y;
                            m.v[num4 + 4].x = nqV.yy.x;
                            m.v[num4 + 4].y = nqV.yy.y;
                            m.v[num4 + 4].u = nqT.yy.x;
                            m.v[num4 + 4].v = nqT.yy.y;
                            m.v[num4 + 5].x = nqV.zz.x;
                            m.v[num4 + 5].y = nqV.yy.y;
                            m.v[num4 + 5].u = nqT.zz.x;
                            m.v[num4 + 5].v = nqT.yy.y;
                            break;

                        case 1:
                            num4 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num4].x = nqV.yy.x;
                            m.v[num4].y = nqV.xx.y;
                            m.v[num4].u = nqT.yy.x;
                            m.v[num4].v = nqT.xx.y;
                            m.v[num4 + 1].x = nqV.zz.x;
                            m.v[num4 + 1].y = nqV.xx.y;
                            m.v[num4 + 1].u = nqT.zz.x;
                            m.v[num4 + 1].v = nqT.xx.y;
                            m.v[num4 + 2].x = nqV.ww.x;
                            m.v[num4 + 2].y = nqV.xx.y;
                            m.v[num4 + 2].u = nqT.ww.x;
                            m.v[num4 + 2].v = nqT.xx.y;
                            m.v[num4 + 3].x = nqV.yy.x;
                            m.v[num4 + 3].y = nqV.yy.y;
                            m.v[num4 + 3].u = nqT.yy.x;
                            m.v[num4 + 3].v = nqT.yy.y;
                            m.v[num4 + 4].x = nqV.zz.x;
                            m.v[num4 + 4].y = nqV.yy.y;
                            m.v[num4 + 4].u = nqT.zz.x;
                            m.v[num4 + 4].v = nqT.yy.y;
                            m.v[num4 + 5].x = nqV.ww.x;
                            m.v[num4 + 5].y = nqV.yy.y;
                            m.v[num4 + 5].u = nqT.ww.x;
                            m.v[num4 + 5].v = nqT.yy.y;
                            break;

                        case 2:
                            m.FastCell(nqV.xx, nqV.yy, nqT.xx, nqT.yy, ref color);
                            m.FastCell(nqV.zx, nqV.wy, nqT.zx, nqT.wy, ref color);
                            break;
                    }
                }
                else
                {
                    int num5;
                    switch (columnStart)
                    {
                        case 0:
                            num5 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num5].x = nqV.xx.x;
                            m.v[num5].y = nqV.xx.y;
                            m.v[num5].u = nqT.xx.x;
                            m.v[num5].v = nqT.xx.y;
                            m.v[num5 + 1].x = nqV.yy.x;
                            m.v[num5 + 1].y = nqV.xx.y;
                            m.v[num5 + 1].u = nqT.yy.x;
                            m.v[num5 + 1].v = nqT.xx.y;
                            m.v[num5 + 2].x = nqV.zz.x;
                            m.v[num5 + 2].y = nqV.xx.y;
                            m.v[num5 + 2].u = nqT.zz.x;
                            m.v[num5 + 2].v = nqT.xx.y;
                            m.v[num5 + 3].x = nqV.xx.x;
                            m.v[num5 + 3].y = nqV.yy.y;
                            m.v[num5 + 3].u = nqT.xx.x;
                            m.v[num5 + 3].v = nqT.yy.y;
                            m.v[num5 + 4].x = nqV.yy.x;
                            m.v[num5 + 4].y = nqV.yy.y;
                            m.v[num5 + 4].u = nqT.yy.x;
                            m.v[num5 + 4].v = nqT.yy.y;
                            m.v[num5 + 5].x = nqV.zz.x;
                            m.v[num5 + 5].y = nqV.yy.y;
                            m.v[num5 + 5].u = nqT.zz.x;
                            m.v[num5 + 5].v = nqT.yy.y;
                            num5 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num5].x = nqV.xx.x;
                            m.v[num5].y = nqV.zz.y;
                            m.v[num5].u = nqT.xx.x;
                            m.v[num5].v = nqT.zz.y;
                            m.v[num5 + 1].x = nqV.yy.x;
                            m.v[num5 + 1].y = nqV.zz.y;
                            m.v[num5 + 1].u = nqT.yy.x;
                            m.v[num5 + 1].v = nqT.zz.y;
                            m.v[num5 + 2].x = nqV.zz.x;
                            m.v[num5 + 2].y = nqV.zz.y;
                            m.v[num5 + 2].u = nqT.zz.x;
                            m.v[num5 + 2].v = nqT.zz.y;
                            m.v[num5 + 3].x = nqV.xx.x;
                            m.v[num5 + 3].y = nqV.ww.y;
                            m.v[num5 + 3].u = nqT.xx.x;
                            m.v[num5 + 3].v = nqT.ww.y;
                            m.v[num5 + 4].x = nqV.yy.x;
                            m.v[num5 + 4].y = nqV.ww.y;
                            m.v[num5 + 4].u = nqT.yy.x;
                            m.v[num5 + 4].v = nqT.ww.y;
                            m.v[num5 + 5].x = nqV.zz.x;
                            m.v[num5 + 5].y = nqV.ww.y;
                            m.v[num5 + 5].u = nqT.zz.x;
                            m.v[num5 + 5].v = nqT.ww.y;
                            break;

                        case 1:
                            num5 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num5].x = nqV.yy.x;
                            m.v[num5].y = nqV.xx.y;
                            m.v[num5].u = nqT.yy.x;
                            m.v[num5].v = nqT.xx.y;
                            m.v[num5 + 1].x = nqV.zz.x;
                            m.v[num5 + 1].y = nqV.xx.y;
                            m.v[num5 + 1].u = nqT.zz.x;
                            m.v[num5 + 1].v = nqT.xx.y;
                            m.v[num5 + 2].x = nqV.ww.x;
                            m.v[num5 + 2].y = nqV.xx.y;
                            m.v[num5 + 2].u = nqT.ww.x;
                            m.v[num5 + 2].v = nqT.xx.y;
                            m.v[num5 + 3].x = nqV.yy.x;
                            m.v[num5 + 3].y = nqV.yy.y;
                            m.v[num5 + 3].u = nqT.yy.x;
                            m.v[num5 + 3].v = nqT.yy.y;
                            m.v[num5 + 4].x = nqV.zz.x;
                            m.v[num5 + 4].y = nqV.yy.y;
                            m.v[num5 + 4].u = nqT.zz.x;
                            m.v[num5 + 4].v = nqT.yy.y;
                            m.v[num5 + 5].x = nqV.ww.x;
                            m.v[num5 + 5].y = nqV.yy.y;
                            m.v[num5 + 5].u = nqT.ww.x;
                            m.v[num5 + 5].v = nqT.yy.y;
                            num5 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                            m.v[num5].x = nqV.yy.x;
                            m.v[num5].y = nqV.zz.y;
                            m.v[num5].u = nqT.yy.x;
                            m.v[num5].v = nqT.zz.y;
                            m.v[num5 + 1].x = nqV.zz.x;
                            m.v[num5 + 1].y = nqV.zz.y;
                            m.v[num5 + 1].u = nqT.zz.x;
                            m.v[num5 + 1].v = nqT.zz.y;
                            m.v[num5 + 2].x = nqV.ww.x;
                            m.v[num5 + 2].y = nqV.zz.y;
                            m.v[num5 + 2].u = nqT.ww.x;
                            m.v[num5 + 2].v = nqT.zz.y;
                            m.v[num5 + 3].x = nqV.yy.x;
                            m.v[num5 + 3].y = nqV.ww.y;
                            m.v[num5 + 3].u = nqT.yy.x;
                            m.v[num5 + 3].v = nqT.ww.y;
                            m.v[num5 + 4].x = nqV.zz.x;
                            m.v[num5 + 4].y = nqV.ww.y;
                            m.v[num5 + 4].u = nqT.zz.x;
                            m.v[num5 + 4].v = nqT.ww.y;
                            m.v[num5 + 5].x = nqV.ww.x;
                            m.v[num5 + 5].y = nqV.ww.y;
                            m.v[num5 + 5].u = nqT.ww.x;
                            m.v[num5 + 5].v = nqT.ww.y;
                            break;

                        case 2:
                            m.FastCell(nqV.xx, nqV.yy, nqT.xx, nqT.yy, ref color);
                            m.FastCell(nqV.zx, nqV.wy, nqT.zx, nqT.wy, ref color);
                            m.FastCell(nqV.xz, nqV.yw, nqT.xz, nqT.yw, ref color);
                            m.FastCell(nqV.zz, nqV.ww, nqT.zz, nqT.ww, ref color);
                            break;
                    }
                }
            }
            else if (nqV.zz.y == nqV.ww.y)
            {
                int num6;
                switch (columnStart)
                {
                    case 0:
                        num6 = m.Alloc(PrimitiveKind.Grid2x2, 0f, (Color) color);
                        m.v[num6].x = nqV.xx.x;
                        m.v[num6].y = nqV.xx.y;
                        m.v[num6].u = nqT.xx.x;
                        m.v[num6].v = nqT.xx.y;
                        m.v[num6 + 1].x = nqV.yy.x;
                        m.v[num6 + 1].y = nqV.xx.y;
                        m.v[num6 + 1].u = nqT.yy.x;
                        m.v[num6 + 1].v = nqT.xx.y;
                        m.v[num6 + 2].x = nqV.zz.x;
                        m.v[num6 + 2].y = nqV.xx.y;
                        m.v[num6 + 2].u = nqT.zz.x;
                        m.v[num6 + 2].v = nqT.xx.y;
                        m.v[num6 + 3].x = nqV.xx.x;
                        m.v[num6 + 3].y = nqV.yy.y;
                        m.v[num6 + 3].u = nqT.xx.x;
                        m.v[num6 + 3].v = nqT.zz.y;
                        m.v[num6 + 4].x = nqV.yy.x;
                        m.v[num6 + 4].y = nqV.yy.y;
                        m.v[num6 + 4].u = nqT.yy.x;
                        m.v[num6 + 4].v = nqT.yy.y;
                        m.v[num6 + 5].x = nqV.zz.x;
                        m.v[num6 + 5].y = nqV.yy.y;
                        m.v[num6 + 5].u = nqT.zz.x;
                        m.v[num6 + 5].v = nqT.yy.y;
                        m.v[num6 + 6].x = nqV.xx.x;
                        m.v[num6 + 6].y = nqV.zz.y;
                        m.v[num6 + 6].u = nqT.xx.x;
                        m.v[num6 + 6].v = nqT.zz.y;
                        m.v[num6 + 7].x = nqV.yy.x;
                        m.v[num6 + 7].y = nqV.zz.y;
                        m.v[num6 + 7].u = nqT.yy.x;
                        m.v[num6 + 7].v = nqT.zz.y;
                        m.v[num6 + 8].x = nqV.zz.x;
                        m.v[num6 + 8].y = nqV.zz.y;
                        m.v[num6 + 8].u = nqT.zz.x;
                        m.v[num6 + 8].v = nqT.zz.y;
                        break;

                    case 1:
                        num6 = m.Alloc(PrimitiveKind.Grid2x2, 0f, (Color) color);
                        m.v[num6].x = nqV.yy.x;
                        m.v[num6].y = nqV.xx.y;
                        m.v[num6].u = nqT.yy.x;
                        m.v[num6].v = nqT.xx.y;
                        m.v[num6 + 1].x = nqV.zz.x;
                        m.v[num6 + 1].y = nqV.xx.y;
                        m.v[num6 + 1].u = nqT.zz.x;
                        m.v[num6 + 1].v = nqT.xx.y;
                        m.v[num6 + 2].x = nqV.ww.x;
                        m.v[num6 + 2].y = nqV.xx.y;
                        m.v[num6 + 2].u = nqT.ww.x;
                        m.v[num6 + 2].v = nqT.xx.y;
                        m.v[num6 + 3].x = nqV.yy.x;
                        m.v[num6 + 3].y = nqV.yy.y;
                        m.v[num6 + 3].u = nqT.yy.x;
                        m.v[num6 + 3].v = nqT.yy.y;
                        m.v[num6 + 4].x = nqV.zz.x;
                        m.v[num6 + 4].y = nqV.yy.y;
                        m.v[num6 + 4].u = nqT.zz.x;
                        m.v[num6 + 4].v = nqT.yy.y;
                        m.v[num6 + 5].x = nqV.ww.x;
                        m.v[num6 + 5].y = nqV.yy.y;
                        m.v[num6 + 5].u = nqT.ww.x;
                        m.v[num6 + 5].v = nqT.yy.y;
                        m.v[num6 + 6].x = nqV.yy.x;
                        m.v[num6 + 6].y = nqV.zz.y;
                        m.v[num6 + 6].u = nqT.yy.x;
                        m.v[num6 + 6].v = nqT.zz.y;
                        m.v[num6 + 7].x = nqV.zz.x;
                        m.v[num6 + 7].y = nqV.zz.y;
                        m.v[num6 + 7].u = nqT.zz.x;
                        m.v[num6 + 7].v = nqT.zz.y;
                        m.v[num6 + 8].x = nqV.ww.x;
                        m.v[num6 + 8].y = nqV.zz.y;
                        m.v[num6 + 8].u = nqT.ww.x;
                        m.v[num6 + 8].v = nqT.zz.y;
                        break;

                    case 2:
                        num6 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                        m.v[num6].x = nqV.xx.x;
                        m.v[num6].y = nqV.xx.y;
                        m.v[num6].u = nqT.xx.x;
                        m.v[num6].v = nqT.xx.y;
                        m.v[num6 + 1].x = nqV.yy.x;
                        m.v[num6 + 1].y = nqV.xx.y;
                        m.v[num6 + 1].u = nqT.yy.x;
                        m.v[num6 + 1].v = nqT.xx.y;
                        m.v[num6 + 2].x = nqV.xx.x;
                        m.v[num6 + 2].y = nqV.yy.y;
                        m.v[num6 + 2].u = nqT.xx.x;
                        m.v[num6 + 2].v = nqT.yy.y;
                        m.v[num6 + 3].x = nqV.yy.x;
                        m.v[num6 + 3].y = nqV.yy.y;
                        m.v[num6 + 3].u = nqT.yy.x;
                        m.v[num6 + 3].v = nqT.yy.y;
                        m.v[num6 + 4].x = nqV.yy.x;
                        m.v[num6 + 4].y = nqV.zz.y;
                        m.v[num6 + 4].u = nqT.yy.x;
                        m.v[num6 + 4].v = nqT.zz.y;
                        m.v[num6 + 5].x = nqV.zz.x;
                        m.v[num6 + 5].y = nqV.zz.y;
                        m.v[num6 + 5].u = nqT.zz.x;
                        m.v[num6 + 5].v = nqT.zz.y;
                        num6 = m.Alloc(PrimitiveKind.Grid2x1, 0f, (Color) color);
                        m.v[num6].x = nqV.zz.x;
                        m.v[num6].y = nqV.xx.y;
                        m.v[num6].u = nqT.zz.x;
                        m.v[num6].v = nqT.xx.y;
                        m.v[num6 + 1].x = nqV.ww.x;
                        m.v[num6 + 1].y = nqV.xx.y;
                        m.v[num6 + 1].u = nqT.ww.x;
                        m.v[num6 + 1].v = nqT.xx.y;
                        m.v[num6 + 2].x = nqV.zz.x;
                        m.v[num6 + 2].y = nqV.yy.y;
                        m.v[num6 + 2].u = nqT.zz.x;
                        m.v[num6 + 2].v = nqT.yy.y;
                        m.v[num6 + 3].x = nqV.ww.x;
                        m.v[num6 + 3].y = nqV.yy.y;
                        m.v[num6 + 3].u = nqT.ww.x;
                        m.v[num6 + 3].v = nqT.yy.y;
                        m.v[num6 + 4].x = nqV.zz.x;
                        m.v[num6 + 4].y = nqV.zz.y;
                        m.v[num6 + 4].u = nqT.zz.x;
                        m.v[num6 + 4].v = nqT.zz.y;
                        m.v[num6 + 5].x = nqV.ww.x;
                        m.v[num6 + 5].y = nqV.zz.y;
                        m.v[num6 + 5].u = nqT.ww.x;
                        m.v[num6 + 5].v = nqT.zz.y;
                        break;
                }
            }
            else
            {
                int num7;
                switch (columnStart)
                {
                    case 0:
                        num7 = m.Alloc(PrimitiveKind.Grid2x3, 0f, (Color) color);
                        m.v[num7].x = nqV.xx.x;
                        m.v[num7].y = nqV.xx.y;
                        m.v[num7].u = nqT.xx.x;
                        m.v[num7].v = nqT.xx.y;
                        m.v[num7 + 1].x = nqV.yy.x;
                        m.v[num7 + 1].y = nqV.xx.y;
                        m.v[num7 + 1].u = nqT.yy.x;
                        m.v[num7 + 1].v = nqT.xx.y;
                        m.v[num7 + 2].x = nqV.zz.x;
                        m.v[num7 + 2].y = nqV.xx.y;
                        m.v[num7 + 2].u = nqT.zz.x;
                        m.v[num7 + 2].v = nqT.xx.y;
                        m.v[num7 + 3].x = nqV.xx.x;
                        m.v[num7 + 3].y = nqV.yy.y;
                        m.v[num7 + 3].u = nqT.xx.x;
                        m.v[num7 + 3].v = nqT.yy.y;
                        m.v[num7 + 4].x = nqV.yy.x;
                        m.v[num7 + 4].y = nqV.yy.y;
                        m.v[num7 + 4].u = nqT.yy.x;
                        m.v[num7 + 4].v = nqT.yy.y;
                        m.v[num7 + 5].x = nqV.zz.x;
                        m.v[num7 + 5].y = nqV.yy.y;
                        m.v[num7 + 5].u = nqT.zz.x;
                        m.v[num7 + 5].v = nqT.yy.y;
                        m.v[num7 + 6].x = nqV.xx.x;
                        m.v[num7 + 6].y = nqV.zz.y;
                        m.v[num7 + 6].u = nqT.xx.x;
                        m.v[num7 + 6].v = nqT.zz.y;
                        m.v[num7 + 7].x = nqV.yy.x;
                        m.v[num7 + 7].y = nqV.zz.y;
                        m.v[num7 + 7].u = nqT.yy.x;
                        m.v[num7 + 7].v = nqT.zz.y;
                        m.v[num7 + 8].x = nqV.zz.x;
                        m.v[num7 + 8].y = nqV.zz.y;
                        m.v[num7 + 8].u = nqT.zz.x;
                        m.v[num7 + 8].v = nqT.zz.y;
                        m.v[num7 + 9].x = nqV.xx.x;
                        m.v[num7 + 9].y = nqV.ww.y;
                        m.v[num7 + 9].u = nqT.xx.x;
                        m.v[num7 + 9].v = nqT.ww.y;
                        m.v[num7 + 10].x = nqV.yy.x;
                        m.v[num7 + 10].y = nqV.ww.y;
                        m.v[num7 + 10].u = nqT.yy.x;
                        m.v[num7 + 10].v = nqT.ww.y;
                        m.v[num7 + 11].x = nqV.zz.x;
                        m.v[num7 + 11].y = nqV.ww.y;
                        m.v[num7 + 11].u = nqT.zz.x;
                        m.v[num7 + 11].v = nqT.ww.y;
                        break;

                    case 1:
                        num7 = m.Alloc(PrimitiveKind.Grid2x3, 0f, (Color) color);
                        m.v[num7].x = nqV.yy.x;
                        m.v[num7].y = nqV.xx.y;
                        m.v[num7].u = nqT.yy.x;
                        m.v[num7].v = nqT.xx.y;
                        m.v[num7 + 1].x = nqV.zz.x;
                        m.v[num7 + 1].y = nqV.xx.y;
                        m.v[num7 + 1].u = nqT.zz.x;
                        m.v[num7 + 1].v = nqT.xx.y;
                        m.v[num7 + 2].x = nqV.ww.x;
                        m.v[num7 + 2].y = nqV.xx.y;
                        m.v[num7 + 2].u = nqT.ww.x;
                        m.v[num7 + 2].v = nqT.xx.y;
                        m.v[num7 + 3].x = nqV.yy.x;
                        m.v[num7 + 3].y = nqV.yy.y;
                        m.v[num7 + 3].u = nqT.yy.x;
                        m.v[num7 + 3].v = nqT.yy.y;
                        m.v[num7 + 4].x = nqV.zz.x;
                        m.v[num7 + 4].y = nqV.yy.y;
                        m.v[num7 + 4].u = nqT.zz.x;
                        m.v[num7 + 4].v = nqT.yy.y;
                        m.v[num7 + 5].x = nqV.ww.x;
                        m.v[num7 + 5].y = nqV.yy.y;
                        m.v[num7 + 5].u = nqT.ww.x;
                        m.v[num7 + 5].v = nqT.yy.y;
                        m.v[num7 + 6].x = nqV.yy.x;
                        m.v[num7 + 6].y = nqV.zz.y;
                        m.v[num7 + 6].u = nqT.yy.x;
                        m.v[num7 + 6].v = nqT.zz.y;
                        m.v[num7 + 7].x = nqV.zz.x;
                        m.v[num7 + 7].y = nqV.zz.y;
                        m.v[num7 + 7].u = nqT.zz.x;
                        m.v[num7 + 7].v = nqT.zz.y;
                        m.v[num7 + 8].x = nqV.ww.x;
                        m.v[num7 + 8].y = nqV.zz.y;
                        m.v[num7 + 8].u = nqT.ww.x;
                        m.v[num7 + 8].v = nqT.zz.y;
                        m.v[num7 + 9].x = nqV.yy.x;
                        m.v[num7 + 9].y = nqV.ww.y;
                        m.v[num7 + 9].u = nqT.yy.x;
                        m.v[num7 + 9].v = nqT.ww.y;
                        m.v[num7 + 10].x = nqV.zz.x;
                        m.v[num7 + 10].y = nqV.ww.y;
                        m.v[num7 + 10].u = nqT.zz.x;
                        m.v[num7 + 10].v = nqT.ww.y;
                        m.v[num7 + 11].x = nqV.ww.x;
                        m.v[num7 + 11].y = nqV.ww.y;
                        m.v[num7 + 11].u = nqT.ww.x;
                        m.v[num7 + 11].v = nqT.ww.y;
                        break;

                    case 2:
                        num7 = m.Alloc(PrimitiveKind.Grid1x3, 0f, (Color) color);
                        m.v[num7].x = nqV.xx.x;
                        m.v[num7].y = nqV.xx.y;
                        m.v[num7].u = nqT.xx.x;
                        m.v[num7].v = nqT.xx.y;
                        m.v[num7 + 1].x = nqV.yy.x;
                        m.v[num7 + 1].y = nqV.xx.y;
                        m.v[num7 + 1].u = nqT.yy.x;
                        m.v[num7 + 1].v = nqT.xx.y;
                        m.v[num7 + 2].x = nqV.xx.x;
                        m.v[num7 + 2].y = nqV.yy.y;
                        m.v[num7 + 2].u = nqT.xx.x;
                        m.v[num7 + 2].v = nqT.yy.y;
                        m.v[num7 + 3].x = nqV.yy.x;
                        m.v[num7 + 3].y = nqV.yy.y;
                        m.v[num7 + 3].u = nqT.yy.x;
                        m.v[num7 + 3].v = nqT.yy.y;
                        m.v[num7 + 4].x = nqV.xx.x;
                        m.v[num7 + 4].y = nqV.zz.y;
                        m.v[num7 + 4].u = nqT.xx.x;
                        m.v[num7 + 4].v = nqT.zz.y;
                        m.v[num7 + 5].x = nqV.yy.x;
                        m.v[num7 + 5].y = nqV.zz.y;
                        m.v[num7 + 5].u = nqT.yy.x;
                        m.v[num7 + 5].v = nqT.zz.y;
                        m.v[num7 + 6].x = nqV.xx.x;
                        m.v[num7 + 6].y = nqV.ww.y;
                        m.v[num7 + 6].u = nqT.xx.x;
                        m.v[num7 + 6].v = nqT.ww.y;
                        m.v[num7 + 7].x = nqV.yy.x;
                        m.v[num7 + 7].y = nqV.ww.y;
                        m.v[num7 + 7].u = nqT.yy.x;
                        m.v[num7 + 7].v = nqT.ww.y;
                        num7 = m.Alloc(PrimitiveKind.Grid1x3, 0f, (Color) color);
                        m.v[num7].x = nqV.zz.x;
                        m.v[num7].y = nqV.xx.y;
                        m.v[num7].u = nqT.zz.x;
                        m.v[num7].v = nqT.xx.y;
                        m.v[num7 + 1].x = nqV.ww.x;
                        m.v[num7 + 1].y = nqV.xx.y;
                        m.v[num7 + 1].u = nqT.ww.x;
                        m.v[num7 + 1].v = nqT.xx.y;
                        m.v[num7 + 2].x = nqV.zz.x;
                        m.v[num7 + 2].y = nqV.yy.y;
                        m.v[num7 + 2].u = nqT.zz.x;
                        m.v[num7 + 2].v = nqT.yy.y;
                        m.v[num7 + 3].x = nqV.ww.x;
                        m.v[num7 + 3].y = nqV.yy.y;
                        m.v[num7 + 3].u = nqT.ww.x;
                        m.v[num7 + 3].v = nqT.yy.y;
                        m.v[num7 + 4].x = nqV.zz.x;
                        m.v[num7 + 4].y = nqV.zz.y;
                        m.v[num7 + 4].u = nqT.zz.x;
                        m.v[num7 + 4].v = nqT.zz.y;
                        m.v[num7 + 5].x = nqV.ww.x;
                        m.v[num7 + 5].y = nqV.zz.y;
                        m.v[num7 + 5].u = nqT.ww.x;
                        m.v[num7 + 5].v = nqT.zz.y;
                        m.v[num7 + 6].x = nqV.zz.x;
                        m.v[num7 + 6].y = nqV.ww.y;
                        m.v[num7 + 6].u = nqT.zz.x;
                        m.v[num7 + 6].v = nqT.ww.y;
                        m.v[num7 + 7].x = nqV.ww.x;
                        m.v[num7 + 7].y = nqV.ww.y;
                        m.v[num7 + 7].u = nqT.ww.x;
                        m.v[num7 + 7].v = nqT.ww.y;
                        break;
                }
            }
        }

        private static void FillColumn3(ref NineRectangle nqV, ref NineRectangle nqT, ref Color color, MeshBuffer m)
        {
            if (nqV.xx.y == nqV.yy.y)
            {
                if (nqV.yy.y == nqV.zz.y)
                {
                    if (nqV.zz.y != nqV.ww.y)
                    {
                        int index = m.Alloc(PrimitiveKind.Grid3x1, 0f, (Color) color);
                        m.v[index].x = nqV.xx.x;
                        m.v[index].y = nqV.zz.y;
                        m.v[index].u = nqT.xx.x;
                        m.v[index].v = nqT.zz.y;
                        m.v[index + 1].x = nqV.yy.x;
                        m.v[index + 1].y = nqV.zz.y;
                        m.v[index + 1].u = nqT.yy.x;
                        m.v[index + 1].v = nqT.zz.y;
                        m.v[index + 2].x = nqV.zz.x;
                        m.v[index + 2].y = nqV.zz.y;
                        m.v[index + 2].u = nqT.zz.x;
                        m.v[index + 2].v = nqT.zz.y;
                        m.v[index + 3].x = nqV.ww.x;
                        m.v[index + 3].y = nqV.zz.y;
                        m.v[index + 3].u = nqT.ww.x;
                        m.v[index + 3].v = nqT.zz.y;
                        m.v[index + 4].x = nqV.xx.x;
                        m.v[index + 4].y = nqV.ww.y;
                        m.v[index + 4].u = nqT.xx.x;
                        m.v[index + 4].v = nqT.ww.y;
                        m.v[index + 5].x = nqV.yy.x;
                        m.v[index + 5].y = nqV.ww.y;
                        m.v[index + 5].u = nqT.yy.x;
                        m.v[index + 5].v = nqT.ww.y;
                        m.v[index + 6].x = nqV.zz.x;
                        m.v[index + 6].y = nqV.ww.y;
                        m.v[index + 6].u = nqT.zz.x;
                        m.v[index + 6].v = nqT.ww.y;
                        m.v[index + 7].x = nqV.ww.x;
                        m.v[index + 7].y = nqV.ww.y;
                        m.v[index + 7].u = nqT.ww.x;
                        m.v[index + 7].v = nqT.ww.y;
                    }
                }
                else if (nqV.zz.y == nqV.ww.y)
                {
                    int num2 = m.Alloc(PrimitiveKind.Grid3x1, 0f, (Color) color);
                    m.v[num2].x = nqV.xx.x;
                    m.v[num2].y = nqV.yy.y;
                    m.v[num2].u = nqT.xx.x;
                    m.v[num2].v = nqT.yy.y;
                    m.v[num2 + 1].x = nqV.yy.x;
                    m.v[num2 + 1].y = nqV.yy.y;
                    m.v[num2 + 1].u = nqT.yy.x;
                    m.v[num2 + 1].v = nqT.yy.y;
                    m.v[num2 + 2].x = nqV.zz.x;
                    m.v[num2 + 2].y = nqV.yy.y;
                    m.v[num2 + 2].u = nqT.zz.x;
                    m.v[num2 + 2].v = nqT.yy.y;
                    m.v[num2 + 3].x = nqV.ww.x;
                    m.v[num2 + 3].y = nqV.yy.y;
                    m.v[num2 + 3].u = nqT.ww.x;
                    m.v[num2 + 3].v = nqT.yy.y;
                    m.v[num2 + 4].x = nqV.xx.x;
                    m.v[num2 + 4].y = nqV.zz.y;
                    m.v[num2 + 4].u = nqT.xx.x;
                    m.v[num2 + 4].v = nqT.zz.y;
                    m.v[num2 + 5].x = nqV.yy.x;
                    m.v[num2 + 5].y = nqV.zz.y;
                    m.v[num2 + 5].u = nqT.yy.x;
                    m.v[num2 + 5].v = nqT.zz.y;
                    m.v[num2 + 6].x = nqV.zz.x;
                    m.v[num2 + 6].y = nqV.zz.y;
                    m.v[num2 + 6].u = nqT.zz.x;
                    m.v[num2 + 6].v = nqT.zz.y;
                    m.v[num2 + 7].x = nqV.ww.x;
                    m.v[num2 + 7].y = nqV.zz.y;
                    m.v[num2 + 7].u = nqT.ww.x;
                    m.v[num2 + 7].v = nqT.zz.y;
                }
                else
                {
                    int num3 = m.Alloc(PrimitiveKind.Grid3x2, 0f, (Color) color);
                    m.v[num3].x = nqV.xx.x;
                    m.v[num3].y = nqV.yy.y;
                    m.v[num3].u = nqT.xx.x;
                    m.v[num3].v = nqT.yy.y;
                    m.v[num3 + 1].x = nqV.yy.x;
                    m.v[num3 + 1].y = nqV.yy.y;
                    m.v[num3 + 1].u = nqT.yy.x;
                    m.v[num3 + 1].v = nqT.yy.y;
                    m.v[num3 + 2].x = nqV.zz.x;
                    m.v[num3 + 2].y = nqV.yy.y;
                    m.v[num3 + 2].u = nqT.zz.x;
                    m.v[num3 + 2].v = nqT.yy.y;
                    m.v[num3 + 3].x = nqV.ww.x;
                    m.v[num3 + 3].y = nqV.yy.y;
                    m.v[num3 + 3].u = nqT.ww.x;
                    m.v[num3 + 3].v = nqT.yy.y;
                    m.v[num3 + 4].x = nqV.xx.x;
                    m.v[num3 + 4].y = nqV.zz.y;
                    m.v[num3 + 4].u = nqT.xx.x;
                    m.v[num3 + 4].v = nqT.zz.y;
                    m.v[num3 + 5].x = nqV.yy.x;
                    m.v[num3 + 5].y = nqV.zz.y;
                    m.v[num3 + 5].u = nqT.yy.x;
                    m.v[num3 + 5].v = nqT.zz.y;
                    m.v[num3 + 6].x = nqV.zz.x;
                    m.v[num3 + 6].y = nqV.zz.y;
                    m.v[num3 + 6].u = nqT.zz.x;
                    m.v[num3 + 6].v = nqT.zz.y;
                    m.v[num3 + 7].x = nqV.ww.x;
                    m.v[num3 + 7].y = nqV.zz.y;
                    m.v[num3 + 7].u = nqT.ww.x;
                    m.v[num3 + 7].v = nqT.zz.y;
                    m.v[num3 + 8].x = nqV.xx.x;
                    m.v[num3 + 8].y = nqV.ww.y;
                    m.v[num3 + 8].u = nqT.xx.x;
                    m.v[num3 + 8].v = nqT.ww.y;
                    m.v[num3 + 9].x = nqV.yy.x;
                    m.v[num3 + 9].y = nqV.ww.y;
                    m.v[num3 + 9].u = nqT.yy.x;
                    m.v[num3 + 9].v = nqT.ww.y;
                    m.v[num3 + 10].x = nqV.zz.x;
                    m.v[num3 + 10].y = nqV.ww.y;
                    m.v[num3 + 10].u = nqT.zz.x;
                    m.v[num3 + 10].v = nqT.ww.y;
                    m.v[num3 + 11].x = nqV.ww.x;
                    m.v[num3 + 11].y = nqV.ww.y;
                    m.v[num3 + 11].u = nqT.ww.x;
                    m.v[num3 + 11].v = nqT.ww.y;
                }
            }
            else if (nqV.yy.y == nqV.zz.y)
            {
                if (nqV.zz.y == nqV.ww.y)
                {
                    int num4 = m.Alloc(PrimitiveKind.Grid3x1, 0f, (Color) color);
                    m.v[num4].x = nqV.xx.x;
                    m.v[num4].y = nqV.xx.y;
                    m.v[num4].u = nqT.xx.x;
                    m.v[num4].v = nqT.xx.y;
                    m.v[num4 + 1].x = nqV.yy.x;
                    m.v[num4 + 1].y = nqV.xx.y;
                    m.v[num4 + 1].u = nqT.yy.x;
                    m.v[num4 + 1].v = nqT.xx.y;
                    m.v[num4 + 2].x = nqV.zz.x;
                    m.v[num4 + 2].y = nqV.xx.y;
                    m.v[num4 + 2].u = nqT.zz.x;
                    m.v[num4 + 2].v = nqT.xx.y;
                    m.v[num4 + 3].x = nqV.ww.x;
                    m.v[num4 + 3].y = nqV.xx.y;
                    m.v[num4 + 3].u = nqT.ww.x;
                    m.v[num4 + 3].v = nqT.xx.y;
                    m.v[num4 + 4].x = nqV.xx.x;
                    m.v[num4 + 4].y = nqV.yy.y;
                    m.v[num4 + 4].u = nqT.xx.x;
                    m.v[num4 + 4].v = nqT.yy.y;
                    m.v[num4 + 5].x = nqV.yy.x;
                    m.v[num4 + 5].y = nqV.yy.y;
                    m.v[num4 + 5].u = nqT.yy.x;
                    m.v[num4 + 5].v = nqT.yy.y;
                    m.v[num4 + 6].x = nqV.zz.x;
                    m.v[num4 + 6].y = nqV.yy.y;
                    m.v[num4 + 6].u = nqT.zz.x;
                    m.v[num4 + 6].v = nqT.yy.y;
                    m.v[num4 + 7].x = nqV.ww.x;
                    m.v[num4 + 7].y = nqV.yy.y;
                    m.v[num4 + 7].u = nqT.ww.x;
                    m.v[num4 + 7].v = nqT.yy.y;
                }
                else
                {
                    int num5 = m.Alloc(PrimitiveKind.Grid3x1, 0f, (Color) color);
                    m.v[num5].x = nqV.xx.x;
                    m.v[num5].y = nqV.xx.y;
                    m.v[num5].u = nqT.xx.x;
                    m.v[num5].v = nqT.xx.y;
                    m.v[num5 + 1].x = nqV.yy.x;
                    m.v[num5 + 1].y = nqV.xx.y;
                    m.v[num5 + 1].u = nqT.yy.x;
                    m.v[num5 + 1].v = nqT.xx.y;
                    m.v[num5 + 2].x = nqV.zz.x;
                    m.v[num5 + 2].y = nqV.xx.y;
                    m.v[num5 + 2].u = nqT.zz.x;
                    m.v[num5 + 2].v = nqT.xx.y;
                    m.v[num5 + 3].x = nqV.ww.x;
                    m.v[num5 + 3].y = nqV.xx.y;
                    m.v[num5 + 3].u = nqT.ww.x;
                    m.v[num5 + 3].v = nqT.xx.y;
                    m.v[num5 + 4].x = nqV.xx.x;
                    m.v[num5 + 4].y = nqV.yy.y;
                    m.v[num5 + 4].u = nqT.xx.x;
                    m.v[num5 + 4].v = nqT.yy.y;
                    m.v[num5 + 5].x = nqV.yy.x;
                    m.v[num5 + 5].y = nqV.yy.y;
                    m.v[num5 + 5].u = nqT.yy.x;
                    m.v[num5 + 5].v = nqT.yy.y;
                    m.v[num5 + 6].x = nqV.zz.x;
                    m.v[num5 + 6].y = nqV.yy.y;
                    m.v[num5 + 6].u = nqT.zz.x;
                    m.v[num5 + 6].v = nqT.yy.y;
                    m.v[num5 + 7].x = nqV.ww.x;
                    m.v[num5 + 7].y = nqV.yy.y;
                    m.v[num5 + 7].u = nqT.ww.x;
                    m.v[num5 + 7].v = nqT.yy.y;
                    num5 = m.Alloc(PrimitiveKind.Grid3x1, 0f, (Color) color);
                    m.v[num5].x = nqV.xx.x;
                    m.v[num5].y = nqV.zz.y;
                    m.v[num5].u = nqT.xx.x;
                    m.v[num5].v = nqT.zz.y;
                    m.v[num5 + 1].x = nqV.yy.x;
                    m.v[num5 + 1].y = nqV.zz.y;
                    m.v[num5 + 1].u = nqT.yy.x;
                    m.v[num5 + 1].v = nqT.zz.y;
                    m.v[num5 + 2].x = nqV.zz.x;
                    m.v[num5 + 2].y = nqV.zz.y;
                    m.v[num5 + 2].u = nqT.zz.x;
                    m.v[num5 + 2].v = nqT.zz.y;
                    m.v[num5 + 3].x = nqV.ww.x;
                    m.v[num5 + 3].y = nqV.zz.y;
                    m.v[num5 + 3].u = nqT.ww.x;
                    m.v[num5 + 3].v = nqT.zz.y;
                    m.v[num5 + 4].x = nqV.xx.x;
                    m.v[num5 + 4].y = nqV.ww.y;
                    m.v[num5 + 4].u = nqT.xx.x;
                    m.v[num5 + 4].v = nqT.ww.y;
                    m.v[num5 + 5].x = nqV.yy.x;
                    m.v[num5 + 5].y = nqV.ww.y;
                    m.v[num5 + 5].u = nqT.yy.x;
                    m.v[num5 + 5].v = nqT.ww.y;
                    m.v[num5 + 6].x = nqV.zz.x;
                    m.v[num5 + 6].y = nqV.ww.y;
                    m.v[num5 + 6].u = nqT.zz.x;
                    m.v[num5 + 6].v = nqT.ww.y;
                    m.v[num5 + 7].x = nqV.ww.x;
                    m.v[num5 + 7].y = nqV.ww.y;
                    m.v[num5 + 7].u = nqT.ww.x;
                    m.v[num5 + 7].v = nqT.ww.y;
                }
            }
            else if (nqV.zz.y == nqV.ww.y)
            {
                int num6 = m.Alloc(PrimitiveKind.Grid3x2, 0f, (Color) color);
                m.v[num6].x = nqV.xx.x;
                m.v[num6].y = nqV.xx.y;
                m.v[num6].u = nqT.xx.x;
                m.v[num6].v = nqT.xx.y;
                m.v[num6 + 1].x = nqV.yy.x;
                m.v[num6 + 1].y = nqV.xx.y;
                m.v[num6 + 1].u = nqT.yy.x;
                m.v[num6 + 1].v = nqT.xx.y;
                m.v[num6 + 2].x = nqV.zz.x;
                m.v[num6 + 2].y = nqV.xx.y;
                m.v[num6 + 2].u = nqT.zz.x;
                m.v[num6 + 2].v = nqT.xx.y;
                m.v[num6 + 3].x = nqV.ww.x;
                m.v[num6 + 3].y = nqV.xx.y;
                m.v[num6 + 3].u = nqT.ww.x;
                m.v[num6 + 3].v = nqT.xx.y;
                m.v[num6 + 4].x = nqV.xx.x;
                m.v[num6 + 4].y = nqV.yy.y;
                m.v[num6 + 4].u = nqT.xx.x;
                m.v[num6 + 4].v = nqT.yy.y;
                m.v[num6 + 5].x = nqV.yy.x;
                m.v[num6 + 5].y = nqV.yy.y;
                m.v[num6 + 5].u = nqT.yy.x;
                m.v[num6 + 5].v = nqT.yy.y;
                m.v[num6 + 6].x = nqV.zz.x;
                m.v[num6 + 6].y = nqV.yy.y;
                m.v[num6 + 6].u = nqT.zz.x;
                m.v[num6 + 6].v = nqT.yy.y;
                m.v[num6 + 7].x = nqV.ww.x;
                m.v[num6 + 7].y = nqV.yy.y;
                m.v[num6 + 7].u = nqT.ww.x;
                m.v[num6 + 7].v = nqT.yy.y;
                m.v[num6 + 8].x = nqV.xx.x;
                m.v[num6 + 8].y = nqV.zz.y;
                m.v[num6 + 8].u = nqT.xx.x;
                m.v[num6 + 8].v = nqT.zz.y;
                m.v[num6 + 9].x = nqV.yy.x;
                m.v[num6 + 9].y = nqV.zz.y;
                m.v[num6 + 9].u = nqT.yy.x;
                m.v[num6 + 9].v = nqT.zz.y;
                m.v[num6 + 10].x = nqV.zz.x;
                m.v[num6 + 10].y = nqV.zz.y;
                m.v[num6 + 10].u = nqT.zz.x;
                m.v[num6 + 10].v = nqT.zz.y;
                m.v[num6 + 11].x = nqV.ww.x;
                m.v[num6 + 11].y = nqV.zz.y;
                m.v[num6 + 11].u = nqT.ww.x;
                m.v[num6 + 11].v = nqT.zz.y;
            }
            else
            {
                Commit3x3(m.Alloc(PrimitiveKind.Grid3x3), ref nqV, ref nqT, ref color, m);
            }
        }

        public float this[int i, int a]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return this.xx[a];

                    case 1:
                        return this.yy[a];

                    case 2:
                        return this.zz[a];

                    case 3:
                        return this.ww[a];
                }
                throw new IndexOutOfRangeException();
            }
        }

        public Vector2 this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return this.xx;

                    case 1:
                        return this.yy;

                    case 2:
                        return this.zz;

                    case 3:
                        return this.ww;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public Vector2 wx
        {
            get
            {
                Vector2 vector;
                vector.x = this.ww.x;
                vector.y = this.xx.y;
                return vector;
            }
            set
            {
                this.ww.x = value.x;
                this.xx.y = value.y;
            }
        }

        public Vector2 wy
        {
            get
            {
                Vector2 vector;
                vector.x = this.ww.x;
                vector.y = this.yy.y;
                return vector;
            }
            set
            {
                this.ww.x = value.x;
                this.zz.y = value.y;
            }
        }

        public Vector2 wz
        {
            get
            {
                Vector2 vector;
                vector.x = this.ww.x;
                vector.y = this.zz.y;
                return vector;
            }
            set
            {
                this.ww.x = value.x;
                this.zz.y = value.y;
            }
        }

        public Vector2 xw
        {
            get
            {
                Vector2 vector;
                vector.x = this.xx.x;
                vector.y = this.ww.y;
                return vector;
            }
            set
            {
                this.xx.x = value.x;
                this.ww.y = value.y;
            }
        }

        public Vector2 xy
        {
            get
            {
                Vector2 vector;
                vector.x = this.xx.x;
                vector.y = this.yy.y;
                return vector;
            }
            set
            {
                this.xx.x = value.x;
                this.yy.y = value.y;
            }
        }

        public Vector2 xz
        {
            get
            {
                Vector2 vector;
                vector.x = this.xx.x;
                vector.y = this.zz.y;
                return vector;
            }
            set
            {
                this.xx.x = value.x;
                this.zz.y = value.y;
            }
        }

        public Vector2 yw
        {
            get
            {
                Vector2 vector;
                vector.x = this.yy.x;
                vector.y = this.ww.y;
                return vector;
            }
            set
            {
                this.yy.x = value.x;
                this.ww.y = value.y;
            }
        }

        public Vector2 yx
        {
            get
            {
                Vector2 vector;
                vector.x = this.yy.x;
                vector.y = this.xx.y;
                return vector;
            }
            set
            {
                this.yy.x = value.x;
                this.xx.y = value.y;
            }
        }

        public Vector2 yz
        {
            get
            {
                Vector2 vector;
                vector.x = this.yy.x;
                vector.y = this.zz.y;
                return vector;
            }
            set
            {
                this.yy.x = value.x;
                this.zz.y = value.y;
            }
        }

        public Vector2 zw
        {
            get
            {
                Vector2 vector;
                vector.x = this.zz.x;
                vector.y = this.ww.y;
                return vector;
            }
            set
            {
                this.zz.x = value.x;
                this.ww.y = value.y;
            }
        }

        public Vector2 zx
        {
            get
            {
                Vector2 vector;
                vector.x = this.zz.x;
                vector.y = this.xx.y;
                return vector;
            }
            set
            {
                this.zz.x = value.x;
                this.xx.y = value.y;
            }
        }

        public Vector2 zy
        {
            get
            {
                Vector2 vector;
                vector.x = this.zz.x;
                vector.y = this.yy.y;
                return vector;
            }
            set
            {
                this.zz.x = value.x;
                this.zz.y = value.y;
            }
        }
    }
}

