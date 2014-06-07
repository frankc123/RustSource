using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class FPGrass : MonoBehaviour, IFPGrassAsset
{
    private static List<FPGrass> AllEnabledFPGrass = new List<FPGrass>();
    private static List<FPGrass> AllEnabledFPGrassInstancesSwap = new List<FPGrass>();
    public float baseLevelSize = 20f;
    public static bool castShadows = false;
    [SerializeField]
    private List<FPGrassLevel> children = new List<FPGrassLevel>();
    public bool followSceneCamera;
    public FPGrassAtlas grassAtlas;
    public FPGrassProbabilities grassProbabilities;
    [SerializeField]
    private float gridSizeAtFinestLevel;
    public int gridSizePerLevel = 0x1c;
    [SerializeField, HideInInspector]
    private Texture2D heightMap;
    [NonSerialized]
    private bool inList;
    public Material material;
    [SerializeField]
    private float normalBias = 0.7f;
    [HideInInspector, SerializeField]
    private Texture2D normalMap;
    public int numberOfLevels = 4;
    public Camera parentCamera;
    public static bool receiveShadows = true;
    [SerializeField]
    private float scatterAmount = 1f;
    [NonSerialized]
    private bool settingsDirty;
    [SerializeField, HideInInspector]
    private Texture2D splatMap;
    public bool toggleWireframe;
    [SerializeField]
    private float windBending = 1f;
    [SerializeField]
    private float windSize = 1f;
    [SerializeField]
    private float windSpeed = 0.1f;
    [SerializeField]
    private Color windTint = Color.white;

    private void Awake()
    {
        if (this.material == null)
        {
            this.material = (Material) Object.Instantiate(Bundling.Load("rust/fpgrass/grassmaterial", typeof(Material)));
        }
    }

    internal static void DrawAllGrass(ref RenderArguments renderArgs)
    {
        List<FPGrass> allEnabledFPGrass = AllEnabledFPGrass;
        AllEnabledFPGrassInstancesSwap.AddRange(allEnabledFPGrass);
        AllEnabledFPGrass = AllEnabledFPGrassInstancesSwap;
        AllEnabledFPGrassInstancesSwap = allEnabledFPGrass;
        try
        {
            foreach (FPGrass grass in AllEnabledFPGrassInstancesSwap)
            {
                grass.Render(ref renderArgs);
            }
        }
        finally
        {
            AllEnabledFPGrassInstancesSwap.Clear();
        }
    }

    private bool EnterList()
    {
        if (!this.inList)
        {
            AllEnabledFPGrass.Add(this);
            this.inList = true;
            return true;
        }
        return false;
    }

    private bool ExitList()
    {
        if (this.inList)
        {
            bool flag = AllEnabledFPGrass.Remove(this);
            this.inList = false;
            return flag;
        }
        return false;
    }

    private void Initialize()
    {
        if (Support.Supported)
        {
            if (this.grassProbabilities == null)
            {
                this.grassProbabilities = ScriptableObject.CreateInstance<FPGrassProbabilities>();
                this.grassProbabilities.name = "FPGrassProbabilities";
            }
            if (this.grassAtlas == null)
            {
                this.grassAtlas = ScriptableObject.CreateInstance<FPGrassAtlas>();
                this.grassAtlas.name = "FPGrassAtlas";
            }
            this.settingsDirty = true;
            this.UpdateProbabilities();
            this.UpdateGrassProperties();
        }
    }

    private void OnDisable()
    {
        this.ExitList();
        if (Terrain.activeTerrain != null)
        {
            Terrain.activeTerrain.detailObjectDistance = 134.6f;
            Terrain.activeTerrain.detailObjectDensity = 1f;
        }
    }

    private void OnEnable()
    {
        if (Support.Supported)
        {
            this.EnterList();
            if (Terrain.activeTerrain != null)
            {
                Terrain.activeTerrain.detailObjectDistance = 0f;
                Terrain.activeTerrain.detailObjectDensity = 0f;
            }
        }
    }

    private void OnValidate()
    {
        this.Initialize();
    }

    private void Render(ref RenderArguments renderArgs)
    {
        if (base.enabled)
        {
            foreach (FPGrassLevel level in this.children)
            {
                if (level.enabled)
                {
                    if (!renderArgs.immediate)
                    {
                        level.UpdateLevel(renderArgs.center, renderArgs.terrain);
                    }
                    if (level.enabled)
                    {
                        level.Render(ref renderArgs);
                    }
                }
            }
        }
    }

    private void Reset()
    {
        for (int i = base.transform.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
        }
        if (this.grassAtlas != null)
        {
            Object.DestroyImmediate(this.grassAtlas);
        }
        if (this.grassProbabilities != null)
        {
            Object.DestroyImmediate(this.grassProbabilities);
        }
        this.Initialize();
    }

    private void Start()
    {
        this.Initialize();
    }

    private void Update()
    {
        if (Support.Supported)
        {
            if (!grass.on)
            {
                this.ExitList();
            }
            else
            {
                if (this.EnterList())
                {
                    this.Initialize();
                }
                else
                {
                    this.settingsDirty = true;
                    if (this.settingsDirty)
                    {
                        this.UpdateProbabilities();
                        this.UpdateGrassProperties();
                        this.settingsDirty = false;
                    }
                }
                if (Application.isPlaying && (this.parentCamera != null))
                {
                    this.UpdateLevels(this.parentCamera.transform.position);
                }
            }
        }
    }

    public void UpdateGrassProperties()
    {
        if ((this.grassAtlas != null) && (this.material != null))
        {
            for (int i = 0; i < 0x10; i++)
            {
                FPGrassProperty property = this.grassAtlas.properties[i];
                this.material.SetColor("_GrassColorsOne" + i, property.Color1);
                this.material.SetColor("_GrassColorsTwo" + i, property.Color2);
                this.material.SetVector("_GrassSizes" + i, new Vector4(property.MinWidth, property.MaxWidth, property.MinHeight, property.MaxHeight));
            }
            for (int j = 0; j < this.children.Count; j++)
            {
                for (int k = 0; k < 0x10; k++)
                {
                    FPGrassProperty property2 = this.grassAtlas.properties[k];
                    this.children[j].levelMaterial.SetColor("_GrassColorsOne" + k, property2.Color1);
                    this.children[j].levelMaterial.SetColor("_GrassColorsTwo" + k, property2.Color2);
                    this.children[j].levelMaterial.SetVector("_GrassSizes" + k, new Vector4(property2.MinWidth, property2.MaxWidth, property2.MinHeight, property2.MaxHeight));
                }
            }
        }
    }

    public void UpdateLevels(Vector3 position)
    {
        base.transform.position = Vector3.zero;
        Terrain activeTerrain = Terrain.activeTerrain;
        if (activeTerrain != null)
        {
            foreach (FPGrassLevel level in this.children)
            {
                level.UpdateLevel(position, activeTerrain);
            }
        }
    }

    public void UpdateProbabilities()
    {
        foreach (FPGrassLevel level in this.children)
        {
            level.probabilityGenerator.SetDetailProbabilities(this.grassProbabilities.GetTexture());
        }
    }

    public void UpdateWindSettings()
    {
        for (int i = 0; i < this.children.Count; i++)
        {
            this.children[i].levelMaterial.SetVector("_WaveAndDistance", new Vector4(this.windSpeed, this.windSize, this.windBending, 0f));
            this.children[i].levelMaterial.SetColor("_WavingTint", this.windTint);
        }
    }

    public static bool anyEnabled
    {
        get
        {
            return (AllEnabledFPGrass.Count > 0);
        }
    }

    public float NormalBias
    {
        get
        {
            return this.normalBias;
        }
        set
        {
            this.normalBias = value;
            this.material.SetFloat("_GroundNormalBias", this.normalBias);
            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].levelMaterial.SetFloat("_GroundNormalBias", this.normalBias);
            }
        }
    }

    public float ScatterAmount
    {
        get
        {
            return this.scatterAmount;
        }
        set
        {
            this.scatterAmount = value;
            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].levelMaterial.SetFloat("_ScatterAmount", this.scatterAmount);
            }
        }
    }

    public float WindBending
    {
        get
        {
            return this.windBending;
        }
        set
        {
            this.windBending = value;
            this.UpdateWindSettings();
        }
    }

    public float WindSize
    {
        get
        {
            return this.windSize;
        }
        set
        {
            this.windSize = value;
        }
    }

    public float WindSpeed
    {
        get
        {
            return this.windSpeed;
        }
        set
        {
            this.windSpeed = value;
            this.UpdateWindSettings();
        }
    }

    public Color WindTint
    {
        get
        {
            return this.windTint;
        }
        set
        {
            this.windTint = value;
            this.UpdateWindSettings();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RenderArguments
    {
        public Plane[] frustum;
        public Camera camera;
        public Terrain terrain;
        public Vector3 center;
        public bool immediate;
    }

    public static class Support
    {
        public static FilterMode DetailProbabilityFilterMode;
        public static readonly bool DisplacementExpensive;
        public static readonly RenderTextureFormat ProbabilityRenderTextureFormat1Channel;
        public static readonly RenderTextureFormat ProbabilityRenderTextureFormat4Channel;
        public static readonly bool Supported;

        static Support()
        {
            bool flag;
            bool flag2;
            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
            {
                ProbabilityRenderTextureFormat4Channel = RenderTextureFormat.ARGB32;
                flag = true;
            }
            else
            {
                ProbabilityRenderTextureFormat4Channel = RenderTextureFormat.Default;
                flag = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default);
            }
            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8))
            {
                ProbabilityRenderTextureFormat1Channel = RenderTextureFormat.R8;
                flag2 = true;
            }
            else
            {
                flag2 = false;
                ProbabilityRenderTextureFormat1Channel = RenderTextureFormat.Default;
            }
            if (flag && !flag2)
            {
                DisplacementExpensive = true;
                ProbabilityRenderTextureFormat1Channel = ProbabilityRenderTextureFormat4Channel;
            }
            else
            {
                DisplacementExpensive = false;
            }
            Supported = flag2 || flag;
            if (!SystemInfo.supportsComputeShaders)
            {
                DetailProbabilityFilterMode = FilterMode.Bilinear;
            }
            else
            {
                DetailProbabilityFilterMode = FilterMode.Point;
            }
        }
    }
}

