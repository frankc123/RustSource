using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, RequireComponent(typeof(BoxCollider)), ExecuteInEditMode, AddComponentMenu("Daikon Forge/User Interface/Sprite/Radial")]
public class dfRadialSprite : dfSprite
{
    private static Vector3[] baseVerts = new Vector3[] { new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0f), new Vector3(0.5f, 0f, 0f), new Vector3(0.5f, -0.5f, 0f), new Vector3(0f, -0.5f, 0f), new Vector3(-0.5f, -0.5f, 0f), new Vector3(-0.5f, 0f, 0f), new Vector3(-0.5f, 0.5f, 0f) };
    [SerializeField]
    protected dfPivotPoint fillOrigin = dfPivotPoint.MiddleCenter;

    private Color32[] buildColors(int vertCount)
    {
        Color32 color = base.ApplyOpacity(!base.IsEnabled ? base.disabledColor : base.color);
        Color32[] colorArray = new Color32[vertCount];
        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = color;
        }
        return colorArray;
    }

    private void buildMeshData(ref List<Vector3> verts, ref List<int> indices, ref List<Vector2> uv)
    {
        List<Vector3> list = verts = new List<Vector3>();
        verts.AddRange(baseVerts);
        int num = 8;
        int index = -1;
        switch (this.fillOrigin)
        {
            case dfPivotPoint.TopLeft:
                num = 4;
                index = 5;
                list.RemoveAt(6);
                list.RemoveAt(0);
                break;

            case dfPivotPoint.TopCenter:
                num = 6;
                index = 0;
                break;

            case dfPivotPoint.TopRight:
                num = 4;
                index = 0;
                list.RemoveAt(2);
                list.RemoveAt(0);
                break;

            case dfPivotPoint.MiddleLeft:
                num = 6;
                index = 6;
                break;

            case dfPivotPoint.MiddleCenter:
                num = 8;
                list.Add(list[0]);
                list.Insert(0, Vector3.zero);
                index = 0;
                break;

            case dfPivotPoint.MiddleRight:
                num = 6;
                index = 2;
                break;

            case dfPivotPoint.BottomLeft:
                num = 4;
                index = 4;
                list.RemoveAt(6);
                list.RemoveAt(4);
                break;

            case dfPivotPoint.BottomCenter:
                num = 6;
                index = 4;
                break;

            case dfPivotPoint.BottomRight:
                num = 4;
                index = 2;
                list.RemoveAt(4);
                list.RemoveAt(2);
                break;

            default:
                throw new NotImplementedException();
        }
        this.makeFirst(list, index);
        List<int> list2 = indices = this.buildTriangles(list);
        float stepSize = 1f / ((float) num);
        float num4 = base.fillAmount.Quantize(stepSize);
        int num5 = Mathf.CeilToInt(num4 / stepSize) + 1;
        for (int i = num5; i < num; i++)
        {
            if (base.invertFill)
            {
                list2.RemoveRange(0, 3);
            }
            else
            {
                list.RemoveAt(list.Count - 1);
                list2.RemoveRange(list2.Count - 3, 3);
            }
        }
        if (base.fillAmount < 1f)
        {
            int num7 = list2[!base.invertFill ? (list2.Count - 2) : 2];
            int num8 = list2[!base.invertFill ? (list2.Count - 1) : 1];
            float t = (base.FillAmount - num4) / stepSize;
            list[num8] = Vector3.Lerp(list[num7], list[num8], t);
        }
        uv = this.buildUV(list);
        float num10 = base.PixelsToUnits();
        Vector2 vector = (Vector2) (num10 * base.size);
        Vector3 vector2 = (Vector3) (base.pivot.TransformToCenter(base.size) * num10);
        for (int j = 0; j < list.Count; j++)
        {
            list[j] = Vector3.Scale(list[j], (Vector3) vector) + vector2;
        }
    }

    private List<int> buildTriangles(List<Vector3> verts)
    {
        List<int> list = new List<int>();
        int count = verts.Count;
        for (int i = 1; i < (count - 1); i++)
        {
            list.Add(0);
            list.Add(i);
            list.Add(i + 1);
        }
        return list;
    }

    private List<Vector2> buildUV(List<Vector3> verts)
    {
        dfAtlas.ItemInfo spriteInfo = base.SpriteInfo;
        if (spriteInfo == null)
        {
            return null;
        }
        Rect region = spriteInfo.region;
        if (base.flip.IsSet(dfSpriteFlip.FlipHorizontal))
        {
            region = new Rect(region.xMax, region.y, -region.width, region.height);
        }
        if (base.flip.IsSet(dfSpriteFlip.FlipVertical))
        {
            region = new Rect(region.x, region.yMax, region.width, -region.height);
        }
        Vector2 vector = new Vector2(region.x, region.y);
        Vector2 vector2 = new Vector2(0.5f, 0.5f);
        Vector2 b = new Vector2(region.width, region.height);
        List<Vector2> list = new List<Vector2>(verts.Count);
        for (int i = 0; i < verts.Count; i++)
        {
            Vector2 a = verts[i] + vector2;
            list.Add(Vector2.Scale(a, b) + vector);
        }
        return list;
    }

    private void makeFirst(List<Vector3> list, int index)
    {
        if (index != 0)
        {
            List<Vector3> range = list.GetRange(index, list.Count - index);
            list.RemoveRange(index, list.Count - index);
            list.InsertRange(0, range);
        }
    }

    protected override void OnRebuildRenderData()
    {
        if ((base.Atlas != null) && (base.SpriteInfo != null))
        {
            base.renderData.Material = base.Atlas.Material;
            List<Vector3> verts = null;
            List<int> indices = null;
            List<Vector2> uv = null;
            this.buildMeshData(ref verts, ref indices, ref uv);
            Color32[] list = this.buildColors(verts.Count);
            base.renderData.Vertices.AddRange(verts);
            base.renderData.Triangles.AddRange(indices);
            base.renderData.UV.AddRange(uv);
            base.renderData.Colors.AddRange(list);
        }
    }

    public dfPivotPoint FillOrigin
    {
        get
        {
            return this.fillOrigin;
        }
        set
        {
            if (value != this.fillOrigin)
            {
                this.fillOrigin = value;
                this.Invalidate();
            }
        }
    }
}

