using System;
using UnityEngine;

[Serializable, ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Sprite/Tiled")]
public class dfTiledSprite : dfSprite
{
    private static int[] quadTriangles = new int[] { 0, 1, 3, 3, 1, 2 };
    private static Vector2[] quadUV = new Vector2[4];
    [SerializeField]
    protected Vector2 tileScale = Vector2.one;
    [SerializeField]
    protected Vector2 tileScroll = Vector2.zero;

    private void addQuadColors(dfList<Color32> colors)
    {
        colors.EnsureCapacity(colors.Count + 4);
        Color32 item = base.ApplyOpacity(!base.IsEnabled ? base.disabledColor : base.color);
        for (int i = 0; i < 4; i++)
        {
            colors.Add(item);
        }
    }

    private void addQuadTriangles(dfList<int> triangles, int baseIndex)
    {
        for (int i = 0; i < quadTriangles.Length; i++)
        {
            triangles.Add(quadTriangles[i] + baseIndex);
        }
    }

    private void addQuadUV(dfList<Vector2> uv, Vector2[] spriteUV)
    {
        uv.AddRange(spriteUV);
    }

    private Vector2[] buildQuadUV()
    {
        Rect region = base.SpriteInfo.region;
        quadUV[0] = new Vector2(region.x, region.yMax);
        quadUV[1] = new Vector2(region.xMax, region.yMax);
        quadUV[2] = new Vector2(region.xMax, region.y);
        quadUV[3] = new Vector2(region.x, region.y);
        Vector2 zero = Vector2.zero;
        if (base.flip.IsSet(dfSpriteFlip.FlipHorizontal))
        {
            zero = quadUV[1];
            quadUV[1] = quadUV[0];
            quadUV[0] = zero;
            zero = quadUV[3];
            quadUV[3] = quadUV[2];
            quadUV[2] = zero;
        }
        if (base.flip.IsSet(dfSpriteFlip.FlipVertical))
        {
            zero = quadUV[0];
            quadUV[0] = quadUV[3];
            quadUV[3] = zero;
            zero = quadUV[1];
            quadUV[1] = quadUV[2];
            quadUV[2] = zero;
        }
        return quadUV;
    }

    private void clipQuads(dfList<Vector3> verts, dfList<Vector2> uv)
    {
        float a = 0f;
        float x = this.size.x;
        float b = -this.size.y;
        float num4 = 0f;
        if (base.fillAmount < 1f)
        {
            if (base.fillDirection == dfFillDirection.Horizontal)
            {
                if (!base.invertFill)
                {
                    x = this.size.x * base.fillAmount;
                }
                else
                {
                    a = this.size.x - (this.size.x * base.fillAmount);
                }
            }
            else if (!base.invertFill)
            {
                b = -this.size.y * base.fillAmount;
            }
            else
            {
                num4 = -this.size.y * (1f - base.fillAmount);
            }
        }
        for (int i = 0; i < verts.Count; i += 4)
        {
            Vector3 vector = verts[i];
            Vector3 vector2 = verts[i + 1];
            Vector3 vector3 = verts[i + 2];
            Vector3 vector4 = verts[i + 3];
            float num6 = vector2.x - vector.x;
            float num7 = vector.y - vector4.y;
            if (vector.x < a)
            {
                float t = (a - vector.x) / num6;
                float introduced35 = Mathf.Max(a, vector.x);
                vector = new Vector3(introduced35, vector.y, vector.z);
                verts[i] = vector;
                float introduced36 = Mathf.Max(a, vector2.x);
                vector2 = new Vector3(introduced36, vector2.y, vector2.z);
                verts[i + 1] = vector2;
                float introduced37 = Mathf.Max(a, vector3.x);
                vector3 = new Vector3(introduced37, vector3.y, vector3.z);
                verts[i + 2] = vector3;
                float introduced38 = Mathf.Max(a, vector4.x);
                vector4 = new Vector3(introduced38, vector4.y, vector4.z);
                verts[i + 3] = vector4;
                Vector2 vector5 = uv[i];
                Vector2 vector6 = uv[i + 1];
                float num9 = Mathf.Lerp(vector5.x, vector6.x, t);
                Vector2 vector7 = uv[i];
                uv[i] = new Vector2(num9, vector7.y);
                Vector2 vector8 = uv[i + 3];
                uv[i + 3] = new Vector2(num9, vector8.y);
                num6 = vector2.x - vector.x;
            }
            if (vector2.x > x)
            {
                float num10 = 1f - (((x - vector2.x) + num6) / num6);
                float introduced39 = Mathf.Min(vector.x, x);
                vector = new Vector3(introduced39, vector.y, vector.z);
                verts[i] = vector;
                float introduced40 = Mathf.Min(vector2.x, x);
                vector2 = new Vector3(introduced40, vector2.y, vector2.z);
                verts[i + 1] = vector2;
                float introduced41 = Mathf.Min(vector3.x, x);
                vector3 = new Vector3(introduced41, vector3.y, vector3.z);
                verts[i + 2] = vector3;
                float introduced42 = Mathf.Min(vector4.x, x);
                vector4 = new Vector3(introduced42, vector4.y, vector4.z);
                verts[i + 3] = vector4;
                Vector2 vector9 = uv[i + 1];
                Vector2 vector10 = uv[i];
                float num11 = Mathf.Lerp(vector9.x, vector10.x, num10);
                Vector2 vector11 = uv[i + 1];
                uv[i + 1] = new Vector2(num11, vector11.y);
                Vector2 vector12 = uv[i + 2];
                uv[i + 2] = new Vector2(num11, vector12.y);
                num6 = vector2.x - vector.x;
            }
            if (vector4.y < b)
            {
                float num12 = 1f - (Mathf.Abs((float) (-b + vector.y)) / num7);
                vector = new Vector3(vector.x, Mathf.Max(vector.y, b), vector2.z);
                verts[i] = vector;
                float y = Mathf.Max(vector2.y, b);
                vector2 = new Vector3(vector2.x, y, vector2.z);
                verts[i + 1] = vector2;
                float introduced44 = Mathf.Max(vector3.y, b);
                vector3 = new Vector3(vector3.x, introduced44, vector3.z);
                verts[i + 2] = vector3;
                float introduced45 = Mathf.Max(vector4.y, b);
                vector4 = new Vector3(vector4.x, introduced45, vector4.z);
                verts[i + 3] = vector4;
                Vector2 vector13 = uv[i + 3];
                Vector2 vector14 = uv[i];
                float num13 = Mathf.Lerp(vector13.y, vector14.y, num12);
                Vector2 vector15 = uv[i + 3];
                uv[i + 3] = new Vector2(vector15.x, num13);
                Vector2 vector16 = uv[i + 2];
                uv[i + 2] = new Vector2(vector16.x, num13);
                num7 = Mathf.Abs((float) (vector4.y - vector.y));
            }
            if (vector.y > num4)
            {
                float num14 = Mathf.Abs((float) (num4 - vector.y)) / num7;
                float introduced46 = Mathf.Min(num4, vector.y);
                vector = new Vector3(vector.x, introduced46, vector.z);
                verts[i] = vector;
                float introduced47 = Mathf.Min(num4, vector2.y);
                vector2 = new Vector3(vector2.x, introduced47, vector2.z);
                verts[i + 1] = vector2;
                float introduced48 = Mathf.Min(num4, vector3.y);
                vector3 = new Vector3(vector3.x, introduced48, vector3.z);
                verts[i + 2] = vector3;
                float introduced49 = Mathf.Min(num4, vector4.y);
                vector4 = new Vector3(vector4.x, introduced49, vector4.z);
                verts[i + 3] = vector4;
                Vector2 vector17 = uv[i];
                Vector2 vector18 = uv[i + 3];
                float num15 = Mathf.Lerp(vector17.y, vector18.y, num14);
                Vector2 vector19 = uv[i];
                uv[i] = new Vector2(vector19.x, num15);
                Vector2 vector20 = uv[i + 1];
                uv[i + 1] = new Vector2(vector20.x, num15);
            }
        }
    }

    protected override void OnRebuildRenderData()
    {
        if (base.Atlas != null)
        {
            dfAtlas.ItemInfo spriteInfo = base.SpriteInfo;
            if (spriteInfo != null)
            {
                base.renderData.Material = base.Atlas.Material;
                dfList<Vector3> vertices = base.renderData.Vertices;
                dfList<Vector2> uV = base.renderData.UV;
                dfList<Color32> colors = base.renderData.Colors;
                dfList<int> triangles = base.renderData.Triangles;
                Vector2[] spriteUV = this.buildQuadUV();
                Vector2 vector = Vector2.Scale(spriteInfo.sizeInPixels, this.tileScale);
                Vector2 vector2 = new Vector2(this.tileScroll.x % 1f, this.tileScroll.y % 1f);
                for (float i = -Mathf.Abs((float) (vector2.y * vector.y)); i < this.size.y; i += vector.y)
                {
                    for (float k = -Mathf.Abs((float) (vector2.x * vector.x)); k < this.size.x; k += vector.x)
                    {
                        int count = vertices.Count;
                        vertices.Add(new Vector3(k, -i));
                        vertices.Add(new Vector3(k + vector.x, -i));
                        vertices.Add(new Vector3(k + vector.x, -i + -vector.y));
                        vertices.Add(new Vector3(k, -i + -vector.y));
                        this.addQuadTriangles(triangles, count);
                        this.addQuadUV(uV, spriteUV);
                        this.addQuadColors(colors);
                    }
                }
                this.clipQuads(vertices, uV);
                float num4 = base.PixelsToUnits();
                Vector3 vector3 = base.pivot.TransformToUpperLeft(base.size);
                for (int j = 0; j < vertices.Count; j++)
                {
                    vertices[j] = (Vector3) ((vertices[j] + vector3) * num4);
                }
            }
        }
    }

    public Vector2 TileScale
    {
        get
        {
            return this.tileScale;
        }
        set
        {
            if (Vector2.Distance(value, this.tileScale) > float.Epsilon)
            {
                this.tileScale = Vector2.Max((Vector2) (Vector2.one * 0.1f), value);
                this.Invalidate();
            }
        }
    }

    public Vector2 TileScroll
    {
        get
        {
            return this.tileScroll;
        }
        set
        {
            if (Vector2.Distance(value, this.tileScroll) > float.Epsilon)
            {
                this.tileScroll = value;
                this.Invalidate();
            }
        }
    }
}

