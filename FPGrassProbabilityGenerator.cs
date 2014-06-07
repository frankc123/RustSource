using Facepunch;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class FPGrassProbabilityGenerator : ScriptableObject, IFPGrassAsset
{
    [SerializeField]
    public float gridScale;
    [SerializeField]
    public int gridSize;
    [SerializeField]
    private Material material;
    [NonSerialized]
    public RenderTexture probabilityTexture;

    private void CreateRenderTexture()
    {
        this.DestroyProbabilityTexture();
        int width = Mathf.NextPowerOfTwo(this.gridSize);
        RenderTexture texture = new RenderTexture(width, width, 0, FPGrass.Support.ProbabilityRenderTextureFormat4Channel) {
            hideFlags = HideFlags.DontSave
        };
        this.probabilityTexture = texture;
        this.probabilityTexture.filterMode = FilterMode.Point;
        this.probabilityTexture.useMipMap = false;
        this.probabilityTexture.anisoLevel = 0;
        this.probabilityTexture.Create();
    }

    public void DestroyObjects()
    {
        this.DestroyProbabilityTexture();
    }

    private void DestroyProbabilityTexture()
    {
        if (this.probabilityTexture != null)
        {
            Object.DestroyImmediate(this.probabilityTexture, true);
            this.probabilityTexture = null;
        }
    }

    public void Initialize()
    {
        if ((this.probabilityTexture == null) && (this.gridSize > 0))
        {
            this.CreateRenderTexture();
        }
        if (this.material == null)
        {
            this.material = (Material) Object.Instantiate(Bundling.Load("rust/fpgrass/RenderSplatMaterial", typeof(Material)));
            this.material.SetTexture("_Noise", (Texture2D) Bundling.Load("rust/fpgrass/noise", typeof(Texture2D)));
        }
    }

    private void OnDestroy()
    {
        this.DestroyProbabilityTexture();
    }

    private void OnDisable()
    {
        this.DestroyProbabilityTexture();
    }

    private void OnEnable()
    {
        this.Initialize();
    }

    public void SetDetailProbabilities(Texture2D texture)
    {
        this.material.SetTexture("_DetailProbabilities", texture);
    }

    public void SetGridScale(float newScale)
    {
        this.gridScale = newScale;
        this.material.SetFloat("_GridScale", this.gridScale);
    }

    public void SetGridSize(int newSize)
    {
        this.gridSize = newSize;
        this.material.SetFloat("_GridSize", (float) this.gridSize);
        this.CreateRenderTexture();
    }

    public void SetSplatTexture(Texture2D texture)
    {
        this.material.SetTexture("_Splat1", texture);
    }

    public void UpdateMap(Vector3 newPosition)
    {
        if (this.material == null)
        {
            Debug.Log("No Material to render splat!");
        }
        else
        {
            if (this.probabilityTexture == null)
            {
                this.CreateRenderTexture();
            }
            float y = 1f - ((((float) this.gridSize) / ((float) this.probabilityTexture.height)) * 2f);
            float x = -1f;
            float num3 = x + ((((float) this.gridSize) / ((float) this.probabilityTexture.width)) * 2f);
            float num4 = 1f;
            if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL", StringComparison.InvariantCultureIgnoreCase))
            {
                num4 = -1f;
                y = -1f + ((((float) this.gridSize) / ((float) this.probabilityTexture.height)) * 2f);
            }
            float num5 = newPosition.z - Mathf.FloorToInt((this.gridSize * 0.5f) * this.gridScale);
            float num6 = newPosition.x - Mathf.FloorToInt((this.gridSize * 0.5f) * this.gridScale);
            float num7 = num5 + (this.gridSize * this.gridScale);
            float num8 = num6 + (this.gridSize * this.gridScale);
            this.material.SetFloat("_TerrainSize", Terrain.activeTerrain.terrainData.size.x);
            this.material.SetVector("_Position", new Vector4(newPosition.x, newPosition.y, newPosition.z, 1f));
            int pass = (FPGrass.Support.DetailProbabilityFilterMode != FilterMode.Point) ? 1 : 0;
            RenderTexture active = RenderTexture.active;
            try
            {
                GL.PushMatrix();
                RenderTexture.active = this.probabilityTexture;
                GL.LoadPixelMatrix(0f, (float) this.probabilityTexture.width, 0f, (float) this.probabilityTexture.height);
                this.material.SetPass(pass);
                GL.Begin(5);
                GL.TexCoord(new Vector3(num8, num5, 0f));
                GL.Vertex3(num3, num4, 0f);
                GL.TexCoord(new Vector3(num6, num5, 0f));
                GL.Vertex3(x, num4, 0f);
                GL.TexCoord(new Vector3(num8, num7, 0f));
                GL.Vertex3(num3, y, 0f);
                GL.TexCoord(new Vector3(num6, num7, 0f));
                GL.Vertex3(x, y, 0f);
                GL.End();
                GL.PopMatrix();
            }
            finally
            {
                RenderTexture.active = active;
            }
        }
    }

    public string name
    {
        get
        {
            return base.name;
        }
        set
        {
            base.name = value;
            this.material.name = value + "(" + this.material.name.Replace("(Clone)", string.Empty) + ")";
        }
    }
}

