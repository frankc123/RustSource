using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class dfMarkupBoxSprite : dfMarkupBox
{
    private dfRenderData renderData;
    private static int[] TRIANGLE_INDICES = new int[] { 0, 1, 2, 0, 2, 3 };

    public dfMarkupBoxSprite(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style) : base(element, display, style)
    {
        this.renderData = new dfRenderData(0x20);
    }

    private static void addTriangleIndices(dfList<Vector3> verts, dfList<int> triangles)
    {
        int count = verts.Count;
        int[] numArray = TRIANGLE_INDICES;
        for (int i = 0; i < numArray.Length; i++)
        {
            triangles.Add(count + numArray[i]);
        }
    }

    internal void LoadImage(dfAtlas atlas, string source)
    {
        dfAtlas.ItemInfo info = atlas[source];
        if (info == null)
        {
            throw new InvalidOperationException("Sprite does not exist in atlas: " + source);
        }
        this.Atlas = atlas;
        this.Source = source;
        base.Size = info.sizeInPixels;
        base.Baseline = (int) this.Size.y;
    }

    protected override dfRenderData OnRebuildRenderData()
    {
        this.renderData.Clear();
        if ((this.Atlas != null) && (this.Atlas[this.Source] != null))
        {
            dfSprite.RenderOptions options = new dfSprite.RenderOptions {
                atlas = this.Atlas,
                spriteInfo = this.Atlas[this.Source],
                pixelsToUnits = 1f,
                size = base.Size,
                color = this.Style.Color,
                fillAmount = 1f
            };
            dfSlicedSprite.renderSprite(this.renderData, options);
            this.renderData.Material = this.Atlas.Material;
            this.renderData.Transform = Matrix4x4.identity;
        }
        return this.renderData;
    }

    public dfAtlas Atlas { get; set; }

    public string Source { get; set; }
}

