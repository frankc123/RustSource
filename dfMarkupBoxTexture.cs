using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class dfMarkupBoxTexture : dfMarkupBox
{
    private Material material;
    private dfRenderData renderData;
    private static int[] TRIANGLE_INDICES = new int[] { 0, 1, 2, 0, 2, 3 };

    public dfMarkupBoxTexture(dfMarkupElement element, dfMarkupDisplayType display, dfMarkupStyle style) : base(element, display, style)
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

    private void ensureMaterial()
    {
        if ((this.material == null) && (this.Texture != null))
        {
            Shader shader = Shader.Find("Daikon Forge/Default UI Shader");
            if (shader == null)
            {
                Debug.LogError("Failed to find default shader");
            }
            else
            {
                Material material = new Material(shader) {
                    name = "Default Texture Shader",
                    hideFlags = HideFlags.DontSave,
                    mainTexture = this.Texture
                };
                this.material = material;
            }
        }
    }

    internal void LoadTexture(UnityEngine.Texture texture)
    {
        if (texture == null)
        {
            throw new InvalidOperationException();
        }
        this.Texture = texture;
        base.Size = new Vector2((float) texture.width, (float) texture.height);
        base.Baseline = (int) this.Size.y;
    }

    protected override dfRenderData OnRebuildRenderData()
    {
        this.renderData.Clear();
        this.ensureMaterial();
        this.renderData.Material = this.material;
        this.renderData.Material.mainTexture = this.Texture;
        Vector3 zero = Vector3.zero;
        Vector3 item = zero + ((Vector3) (Vector3.right * this.Size.x));
        Vector3 vector3 = item + ((Vector3) (Vector3.down * this.Size.y));
        Vector3 vector4 = zero + ((Vector3) (Vector3.down * this.Size.y));
        this.renderData.Vertices.Add(zero);
        this.renderData.Vertices.Add(item);
        this.renderData.Vertices.Add(vector3);
        this.renderData.Vertices.Add(vector4);
        this.renderData.Triangles.AddRange(TRIANGLE_INDICES);
        this.renderData.UV.Add(new Vector2(0f, 1f));
        this.renderData.UV.Add(new Vector2(1f, 1f));
        this.renderData.UV.Add(new Vector2(1f, 0f));
        this.renderData.UV.Add(new Vector2(0f, 0f));
        Color color = this.Style.Color;
        this.renderData.Colors.Add(color);
        this.renderData.Colors.Add(color);
        this.renderData.Colors.Add(color);
        this.renderData.Colors.Add(color);
        return this.renderData;
    }

    public UnityEngine.Texture Texture { get; set; }
}

