using System;
using UnityEngine;

[Serializable, ExecuteInEditMode, RequireComponent(typeof(BoxCollider)), AddComponentMenu("Daikon Forge/User Interface/Sprite/Sliced")]
public class dfSlicedSprite : dfSprite
{
    private static int[][] fillIndices;
    private static int[][] horzFill;
    private static int[] triangleIndices = new int[] { 
        0, 1, 2, 2, 3, 0, 4, 5, 6, 6, 7, 4, 8, 9, 10, 10, 
        11, 8, 12, 13, 14, 14, 15, 12, 1, 4, 7, 7, 2, 1, 9, 12, 
        15, 15, 10, 9, 3, 2, 9, 9, 8, 3, 7, 6, 13, 13, 12, 7, 
        2, 7, 12, 12, 9, 2
     };
    private static Vector2[] uv;
    private static int[][] vertFill;
    private static Vector3[] verts;

    static dfSlicedSprite()
    {
        int[][] numArrayArray1 = new int[4][];
        int[] numArray1 = new int[4];
        numArray1[1] = 1;
        numArray1[2] = 4;
        numArray1[3] = 5;
        numArrayArray1[0] = numArray1;
        numArrayArray1[1] = new int[] { 3, 2, 7, 6 };
        numArrayArray1[2] = new int[] { 8, 9, 12, 13 };
        numArrayArray1[3] = new int[] { 11, 10, 15, 14 };
        horzFill = numArrayArray1;
        int[][] numArrayArray2 = new int[4][];
        int[] numArray2 = new int[4];
        numArray2[0] = 11;
        numArray2[1] = 8;
        numArray2[2] = 3;
        numArrayArray2[0] = numArray2;
        numArrayArray2[1] = new int[] { 10, 9, 2, 1 };
        numArrayArray2[2] = new int[] { 15, 12, 7, 4 };
        numArrayArray2[3] = new int[] { 14, 13, 6, 5 };
        vertFill = numArrayArray2;
        fillIndices = new int[][] { new int[4], new int[4], new int[4], new int[4] };
        verts = new Vector3[0x10];
        uv = new Vector2[0x10];
    }

    private static void doFill(dfRenderData renderData, dfSprite.RenderOptions options)
    {
        int baseIndex = options.baseIndex;
        dfList<Vector3> vertices = renderData.Vertices;
        dfList<Vector2> uV = renderData.UV;
        int[][] numArray = getFillIndices(options.fillDirection, baseIndex);
        bool invertFill = options.invertFill;
        if (options.fillDirection == dfFillDirection.Vertical)
        {
            invertFill = !invertFill;
        }
        if (invertFill)
        {
            for (int j = 0; j < numArray.Length; j++)
            {
                Array.Reverse(numArray[j]);
            }
        }
        int num3 = (options.fillDirection != dfFillDirection.Horizontal) ? 1 : 0;
        Vector3 vector5 = vertices[numArray[0][invertFill ? 3 : 0]];
        float num4 = vector5[num3];
        Vector3 vector6 = vertices[numArray[0][invertFill ? 0 : 3]];
        float num5 = vector6[num3];
        float num6 = Mathf.Abs((float) (num5 - num4));
        float num7 = invertFill ? (num5 - (options.fillAmount * num6)) : (num4 + (options.fillAmount * num6));
        for (int i = 0; i < numArray.Length; i++)
        {
            if (!invertFill)
            {
                for (int k = 3; k > 0; k--)
                {
                    Vector3 vector7 = vertices[numArray[i][k]];
                    float num10 = vector7[num3];
                    if (num10 >= num7)
                    {
                        Vector3 vector = vertices[numArray[i][k]];
                        vector[num3] = num7;
                        vertices[numArray[i][k]] = vector;
                        Vector3 vector8 = vertices[numArray[i][k - 1]];
                        float num11 = vector8[num3];
                        if (num11 <= num7)
                        {
                            float num12 = num10 - num11;
                            float t = (num7 - num11) / num12;
                            Vector2 vector9 = uV[numArray[i][k]];
                            float to = vector9[num3];
                            Vector2 vector10 = uV[numArray[i][k - 1]];
                            float from = vector10[num3];
                            Vector2 vector2 = uV[numArray[i][k]];
                            vector2[num3] = Mathf.Lerp(from, to, t);
                            uV[numArray[i][k]] = vector2;
                        }
                    }
                }
            }
            else
            {
                for (int m = 1; m < 4; m++)
                {
                    Vector3 vector11 = vertices[numArray[i][m]];
                    float num17 = vector11[num3];
                    if (num17 <= num7)
                    {
                        Vector3 vector3 = vertices[numArray[i][m]];
                        vector3[num3] = num7;
                        vertices[numArray[i][m]] = vector3;
                        Vector3 vector12 = vertices[numArray[i][m - 1]];
                        float num18 = vector12[num3];
                        if (num18 >= num7)
                        {
                            float num19 = num17 - num18;
                            float num20 = (num7 - num18) / num19;
                            Vector2 vector13 = uV[numArray[i][m]];
                            float num21 = vector13[num3];
                            Vector2 vector14 = uV[numArray[i][m - 1]];
                            float num22 = vector14[num3];
                            Vector2 vector4 = uV[numArray[i][m]];
                            vector4[num3] = Mathf.Lerp(num22, num21, num20);
                            uV[numArray[i][m]] = vector4;
                        }
                    }
                }
            }
        }
    }

    private static int[][] getFillIndices(dfFillDirection fillDirection, int baseIndex)
    {
        int[][] numArray = (fillDirection != dfFillDirection.Horizontal) ? vertFill : horzFill;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                fillIndices[i][j] = baseIndex + numArray[i][j];
            }
        }
        return fillIndices;
    }

    protected override void OnRebuildRenderData()
    {
        if (base.Atlas != null)
        {
            dfAtlas.ItemInfo spriteInfo = base.SpriteInfo;
            if (spriteInfo != null)
            {
                base.renderData.Material = base.Atlas.Material;
                if ((spriteInfo.border.horizontal == 0) && (spriteInfo.border.vertical == 0))
                {
                    base.OnRebuildRenderData();
                }
                else
                {
                    Color32 color = base.ApplyOpacity(!base.IsEnabled ? base.disabledColor : base.color);
                    dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                        atlas = base.atlas,
                        color = color,
                        fillAmount = base.fillAmount,
                        fillDirection = base.fillDirection,
                        flip = base.flip,
                        invertFill = base.invertFill,
                        offset = base.pivot.TransformToUpperLeft(base.Size),
                        pixelsToUnits = base.PixelsToUnits(),
                        size = base.Size,
                        spriteInfo = base.SpriteInfo
                    };
                    renderSprite(base.renderData, options);
                }
            }
        }
    }

    private static void rebuildColors(dfRenderData renderData, dfSprite.RenderOptions options)
    {
        for (int i = 0; i < 0x10; i++)
        {
            renderData.Colors.Add(options.color);
        }
    }

    private static void rebuildTriangles(dfRenderData renderData, dfSprite.RenderOptions options)
    {
        int baseIndex = options.baseIndex;
        dfList<int> triangles = renderData.Triangles;
        for (int i = 0; i < triangleIndices.Length; i++)
        {
            triangles.Add(baseIndex + triangleIndices[i]);
        }
    }

    private static void rebuildUV(dfRenderData renderData, dfSprite.RenderOptions options)
    {
        dfAtlas atlas = options.atlas;
        Vector2 vector = new Vector2((float) atlas.Texture.width, (float) atlas.Texture.height);
        dfAtlas.ItemInfo spriteInfo = options.spriteInfo;
        float num = ((float) spriteInfo.border.top) / vector.y;
        float num2 = ((float) spriteInfo.border.bottom) / vector.y;
        float num3 = ((float) spriteInfo.border.left) / vector.x;
        float num4 = ((float) spriteInfo.border.right) / vector.x;
        Rect region = spriteInfo.region;
        uv[0] = new Vector2(region.x, region.yMax);
        uv[1] = new Vector2(region.x + num3, region.yMax);
        uv[2] = new Vector2(region.x + num3, region.yMax - num);
        uv[3] = new Vector2(region.x, region.yMax - num);
        uv[4] = new Vector2(region.xMax - num4, region.yMax);
        uv[5] = new Vector2(region.xMax, region.yMax);
        uv[6] = new Vector2(region.xMax, region.yMax - num);
        uv[7] = new Vector2(region.xMax - num4, region.yMax - num);
        uv[8] = new Vector2(region.x, region.y + num2);
        uv[9] = new Vector2(region.x + num3, region.y + num2);
        uv[10] = new Vector2(region.x + num3, region.y);
        uv[11] = new Vector2(region.x, region.y);
        uv[12] = new Vector2(region.xMax - num4, region.y + num2);
        uv[13] = new Vector2(region.xMax, region.y + num2);
        uv[14] = new Vector2(region.xMax, region.y);
        uv[15] = new Vector2(region.xMax - num4, region.y);
        if (options.flip != dfSpriteFlip.None)
        {
            for (int j = 0; j < uv.Length; j += 4)
            {
                Vector2 zero = Vector2.zero;
                if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
                {
                    zero = uv[j];
                    uv[j] = uv[j + 1];
                    uv[j + 1] = zero;
                    zero = uv[j + 2];
                    uv[j + 2] = uv[j + 3];
                    uv[j + 3] = zero;
                }
                if (options.flip.IsSet(dfSpriteFlip.FlipVertical))
                {
                    zero = uv[j];
                    uv[j] = uv[j + 3];
                    uv[j + 3] = zero;
                    zero = uv[j + 1];
                    uv[j + 1] = uv[j + 2];
                    uv[j + 2] = zero;
                }
            }
            if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
            {
                Vector2[] destinationArray = new Vector2[uv.Length];
                Array.Copy(uv, destinationArray, uv.Length);
                Array.Copy(uv, 0, uv, 4, 4);
                Array.Copy(destinationArray, 4, uv, 0, 4);
                Array.Copy(uv, 8, uv, 12, 4);
                Array.Copy(destinationArray, 12, uv, 8, 4);
            }
            if (options.flip.IsSet(dfSpriteFlip.FlipVertical))
            {
                Vector2[] vectorArray2 = new Vector2[uv.Length];
                Array.Copy(uv, vectorArray2, uv.Length);
                Array.Copy(uv, 0, uv, 8, 4);
                Array.Copy(vectorArray2, 8, uv, 0, 4);
                Array.Copy(uv, 4, uv, 12, 4);
                Array.Copy(vectorArray2, 12, uv, 4, 4);
            }
        }
        for (int i = 0; i < uv.Length; i++)
        {
            renderData.UV.Add(uv[i]);
        }
    }

    private static void rebuildVertices(dfRenderData renderData, dfSprite.RenderOptions options)
    {
        float x = 0f;
        float y = 0f;
        float num3 = Mathf.Ceil(options.size.x);
        float num4 = Mathf.Ceil(-options.size.y);
        dfAtlas.ItemInfo spriteInfo = options.spriteInfo;
        float left = spriteInfo.border.left;
        float top = spriteInfo.border.top;
        float right = spriteInfo.border.right;
        float bottom = spriteInfo.border.bottom;
        if (options.flip.IsSet(dfSpriteFlip.FlipHorizontal))
        {
            float num9 = right;
            right = left;
            left = num9;
        }
        if (options.flip.IsSet(dfSpriteFlip.FlipVertical))
        {
            float num10 = bottom;
            bottom = top;
            top = num10;
        }
        verts[0] = new Vector3(x, y, 0f) + options.offset;
        verts[1] = verts[0] + new Vector3(left, 0f, 0f);
        verts[2] = verts[0] + new Vector3(left, -top, 0f);
        verts[3] = verts[0] + new Vector3(0f, -top, 0f);
        verts[4] = new Vector3(num3 - right, y, 0f) + options.offset;
        verts[5] = verts[4] + new Vector3(right, 0f, 0f);
        verts[6] = verts[4] + new Vector3(right, -top, 0f);
        verts[7] = verts[4] + new Vector3(0f, -top, 0f);
        verts[8] = new Vector3(x, num4 + bottom, 0f) + options.offset;
        verts[9] = verts[8] + new Vector3(left, 0f, 0f);
        verts[10] = verts[8] + new Vector3(left, -bottom, 0f);
        verts[11] = verts[8] + new Vector3(0f, -bottom, 0f);
        verts[12] = new Vector3(num3 - right, num4 + bottom, 0f) + options.offset;
        verts[13] = verts[12] + new Vector3(right, 0f, 0f);
        verts[14] = verts[12] + new Vector3(right, -bottom, 0f);
        verts[15] = verts[12] + new Vector3(0f, -bottom, 0f);
        for (int i = 0; i < verts.Length; i++)
        {
            renderData.Vertices.Add(((Vector3) (verts[i] * options.pixelsToUnits)).Quantize(options.pixelsToUnits));
        }
    }

    internal static void renderSprite(dfRenderData renderData, dfSprite.RenderOptions options)
    {
        options.baseIndex = renderData.Vertices.Count;
        rebuildTriangles(renderData, options);
        rebuildVertices(renderData, options);
        rebuildUV(renderData, options);
        rebuildColors(renderData, options);
        if (options.fillAmount < 1f)
        {
            doFill(renderData, options);
        }
    }
}

