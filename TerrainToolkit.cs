using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Terrain/Terrain Toolkit"), ExecuteInEditMode]
public class TerrainToolkit : MonoBehaviour
{
    public float brushOpacity = 1f;
    public Vector3 brushPosition;
    public float brushSize = 50f;
    public float brushSoftness = 0.5f;
    public Texture2D createIcon;
    public Texture2D defaultTexture;
    public float diamondSquareBlend = 1f;
    public float diamondSquareDelta = 0.5f;
    public Texture2D erodeIcon;
    private ErosionMode erosionMode;
    private ErosionType erosionType;
    public int erosionTypeInt;
    [NonSerialized]
    public int fastHydraulicErosionPresetId;
    public ArrayList fastHydraulicErosionPresets = new ArrayList();
    [NonSerialized]
    public int fractalPresetId;
    public ArrayList fractalPresets = new ArrayList();
    [NonSerialized]
    public int fullHydraulicErosionPresetId;
    public ArrayList fullHydraulicErosionPresets = new ArrayList();
    private GeneratorType generatorType;
    public int generatorTypeInt;
    public string[] gradientStyles;
    public GUISkin guiSkin;
    public List<float> heightBlendPoints;
    public Texture2D hillsIcon;
    public float hydraulicDowncutting = 0.1f;
    public float hydraulicEntropy;
    public float hydraulicEvaporation = 0.5f;
    public float hydraulicFalloff = 0.5f;
    public int hydraulicIterations = 0x19;
    public float hydraulicMaxSlope = 60f;
    public float hydraulicMomentum = 1f;
    public float hydraulicRainfall = 0.01f;
    public float hydraulicSedimentSaturation = 0.1f;
    public float hydraulicSedimentSolubility = 0.01f;
    public HydraulicType hydraulicType;
    public int hydraulicTypeInt;
    public float hydraulicVelocity = 20f;
    public float hydraulicVelocityEvaporation = 0.5f;
    public float hydraulicVelocityRainfall = 0.01f;
    public float hydraulicVelocitySedimentSaturation = 0.1f;
    public float hydraulicVelocitySedimentSolubility = 0.01f;
    public bool isBrushHidden;
    public bool isBrushOn;
    public bool isBrushPainting;
    public Texture2D mooreIcon;
    public Texture2D mountainsIcon;
    private Neighbourhood neighbourhood;
    public int neighbourhoodInt;
    public float normaliseBlend = 1f;
    public float normaliseMax = 1f;
    public float normaliseMin;
    public float perlinAmplitude = 1f;
    public float perlinBlend = 1f;
    public int perlinFrequency = 4;
    public int perlinOctaves = 8;
    [NonSerialized]
    public int perlinPresetId;
    public ArrayList perlinPresets = new ArrayList();
    public Texture2D plateausIcon;
    [NonSerialized]
    public bool presetsInitialised;
    public float slopeBlendMaxAngle = 75f;
    public float slopeBlendMinAngle = 60f;
    public float smoothBlend = 1f;
    public int smoothIterations;
    public SplatPrototype[] splatPrototypes;
    public Texture2D tempTexture;
    public Texture2D textureIcon;
    [NonSerialized]
    public int thermalErosionPresetId;
    public ArrayList thermalErosionPresets = new ArrayList();
    public float thermalFalloff = 0.5f;
    public int thermalIterations = 0x19;
    public float thermalMinSlope = 1f;
    public float tidalCliffLimit = 60f;
    [NonSerialized]
    public int tidalErosionPresetId;
    public ArrayList tidalErosionPresets = new ArrayList();
    public int tidalIterations = 0x19;
    public float tidalRangeAmount = 5f;
    public float tidalSeaLevel = 50f;
    public int toolModeInt;
    public bool useDifferenceMaps = true;
    [NonSerialized]
    public int velocityHydraulicErosionPresetId;
    public ArrayList velocityHydraulicErosionPresets = new ArrayList();
    public Texture2D vonNeumannIcon;
    public float voronoiBlend = 1f;
    public int voronoiCells = 0x10;
    public float voronoiFeatures = 1f;
    [NonSerialized]
    public int voronoiPresetId;
    public ArrayList voronoiPresets = new ArrayList();
    public float voronoiScale = 1f;
    public VoronoiType voronoiType;
    public int voronoiTypeInt;
    public float windCapacity = 0.01f;
    public float windDirection;
    public float windEntropy = 0.1f;
    [NonSerialized]
    public int windErosionPresetId;
    public ArrayList windErosionPresets = new ArrayList();
    public float windForce = 0.5f;
    public float windGravity = 0.5f;
    public int windIterations = 0x19;
    public float windLift = 0.01f;
    public float windSmoothing = 0.25f;

    public void addBlendPoints()
    {
        float num = 0f;
        if (this.heightBlendPoints.Count > 0)
        {
            num = this.heightBlendPoints[this.heightBlendPoints.Count - 1];
        }
        float item = num + ((1f - num) * 0.33f);
        this.heightBlendPoints.Add(item);
        item = num + ((1f - num) * 0.66f);
        this.heightBlendPoints.Add(item);
    }

    public void addPresets()
    {
        this.presetsInitialised = true;
        this.voronoiPresets = new ArrayList();
        this.fractalPresets = new ArrayList();
        this.perlinPresets = new ArrayList();
        this.thermalErosionPresets = new ArrayList();
        this.fastHydraulicErosionPresets = new ArrayList();
        this.fullHydraulicErosionPresets = new ArrayList();
        this.velocityHydraulicErosionPresets = new ArrayList();
        this.tidalErosionPresets = new ArrayList();
        this.windErosionPresets = new ArrayList();
        this.voronoiPresets.Add(new voronoiPresetData("Scattered Peaks", VoronoiType.Linear, 0x10, 8f, 0.5f, 1f));
        this.voronoiPresets.Add(new voronoiPresetData("Rolling Hills", VoronoiType.Sine, 8, 8f, 0f, 1f));
        this.voronoiPresets.Add(new voronoiPresetData("Jagged Mountains", VoronoiType.Linear, 0x20, 32f, 0.5f, 1f));
        this.fractalPresets.Add(new fractalPresetData("Rolling Plains", 0.4f, 1f));
        this.fractalPresets.Add(new fractalPresetData("Rough Mountains", 0.5f, 1f));
        this.fractalPresets.Add(new fractalPresetData("Add Noise", 0.75f, 0.05f));
        this.perlinPresets.Add(new perlinPresetData("Rough Plains", 2, 0.5f, 9, 1f));
        this.perlinPresets.Add(new perlinPresetData("Rolling Hills", 5, 0.75f, 3, 1f));
        this.perlinPresets.Add(new perlinPresetData("Rocky Mountains", 4, 1f, 8, 1f));
        this.perlinPresets.Add(new perlinPresetData("Hellish Landscape", 11, 1f, 7, 1f));
        this.perlinPresets.Add(new perlinPresetData("Add Noise", 10, 1f, 8, 0.2f));
        this.thermalErosionPresets.Add(new thermalErosionPresetData("Gradual, Weak Erosion", 0x19, 7.5f, 0.5f));
        this.thermalErosionPresets.Add(new thermalErosionPresetData("Fast, Harsh Erosion", 0x19, 2.5f, 0.1f));
        this.thermalErosionPresets.Add(new thermalErosionPresetData("Thermal Erosion Brush", 0x19, 0.1f, 0f));
        this.fastHydraulicErosionPresets.Add(new fastHydraulicErosionPresetData("Rainswept Earth", 0x19, 70f, 1f));
        this.fastHydraulicErosionPresets.Add(new fastHydraulicErosionPresetData("Terraced Slopes", 0x19, 30f, 0.4f));
        this.fastHydraulicErosionPresets.Add(new fastHydraulicErosionPresetData("Hydraulic Erosion Brush", 0x19, 85f, 1f));
        this.fullHydraulicErosionPresets.Add(new fullHydraulicErosionPresetData("Low Rainfall, Hard Rock", 0x19, 0.01f, 0.5f, 0.01f, 0.1f));
        this.fullHydraulicErosionPresets.Add(new fullHydraulicErosionPresetData("Low Rainfall, Soft Earth", 0x19, 0.01f, 0.5f, 0.06f, 0.15f));
        this.fullHydraulicErosionPresets.Add(new fullHydraulicErosionPresetData("Heavy Rainfall, Hard Rock", 0x19, 0.02f, 0.5f, 0.01f, 0.1f));
        this.fullHydraulicErosionPresets.Add(new fullHydraulicErosionPresetData("Heavy Rainfall, Soft Earth", 0x19, 0.02f, 0.5f, 0.06f, 0.15f));
        this.velocityHydraulicErosionPresets.Add(new velocityHydraulicErosionPresetData("Low Rainfall, Hard Rock", 0x19, 0.01f, 0.5f, 0.01f, 0.1f, 1f, 1f, 0.05f, 0.12f));
        this.velocityHydraulicErosionPresets.Add(new velocityHydraulicErosionPresetData("Low Rainfall, Soft Earth", 0x19, 0.01f, 0.5f, 0.06f, 0.15f, 1.2f, 2.8f, 0.05f, 0.12f));
        this.velocityHydraulicErosionPresets.Add(new velocityHydraulicErosionPresetData("Heavy Rainfall, Hard Rock", 0x19, 0.02f, 0.5f, 0.01f, 0.1f, 1.1f, 2.2f, 0.05f, 0.12f));
        this.velocityHydraulicErosionPresets.Add(new velocityHydraulicErosionPresetData("Heavy Rainfall, Soft Earth", 0x19, 0.02f, 0.5f, 0.06f, 0.15f, 1.2f, 2.4f, 0.05f, 0.12f));
        this.velocityHydraulicErosionPresets.Add(new velocityHydraulicErosionPresetData("Carved Stone", 0x19, 0.01f, 0.5f, 0.01f, 0.1f, 2f, 1.25f, 0.05f, 0.35f));
        this.tidalErosionPresets.Add(new tidalErosionPresetData("Low Tidal Range, Calm Waves", 0x19, 5f, 65f));
        this.tidalErosionPresets.Add(new tidalErosionPresetData("Low Tidal Range, Strong Waves", 0x19, 5f, 35f));
        this.tidalErosionPresets.Add(new tidalErosionPresetData("High Tidal Range, Calm Water", 0x19, 15f, 55f));
        this.tidalErosionPresets.Add(new tidalErosionPresetData("High Tidal Range, Strong Waves", 0x19, 15f, 25f));
        this.windErosionPresets.Add(new windErosionPresetData("Default (Northerly)", 0x19, 180f, 0.5f, 0.01f, 0.5f, 0.01f, 0.1f, 0.25f));
        this.windErosionPresets.Add(new windErosionPresetData("Default (Southerly)", 0x19, 0f, 0.5f, 0.01f, 0.5f, 0.01f, 0.1f, 0.25f));
        this.windErosionPresets.Add(new windErosionPresetData("Default (Easterly)", 0x19, 270f, 0.5f, 0.01f, 0.5f, 0.01f, 0.1f, 0.25f));
        this.windErosionPresets.Add(new windErosionPresetData("Default (Westerly)", 0x19, 90f, 0.5f, 0.01f, 0.5f, 0.01f, 0.1f, 0.25f));
    }

    public void addSplatPrototype(Texture2D tex, int index)
    {
        SplatPrototype[] prototypeArray = new SplatPrototype[index + 1];
        for (int i = 0; i <= index; i++)
        {
            prototypeArray[i] = new SplatPrototype();
            if (i == index)
            {
                prototypeArray[i].texture = tex;
                prototypeArray[i].tileSize = new Vector2(15f, 15f);
            }
            else
            {
                prototypeArray[i].texture = this.splatPrototypes[i].texture;
                prototypeArray[i].tileSize = this.splatPrototypes[i].tileSize;
            }
        }
        this.splatPrototypes = prototypeArray;
        if ((index + 1) > 2)
        {
            this.addBlendPoints();
        }
    }

    private void convertIntVarsToEnums()
    {
        switch (this.erosionTypeInt)
        {
            case 0:
                this.erosionType = ErosionType.Thermal;
                break;

            case 1:
                this.erosionType = ErosionType.Hydraulic;
                break;

            case 2:
                this.erosionType = ErosionType.Tidal;
                break;

            case 3:
                this.erosionType = ErosionType.Wind;
                break;

            case 4:
                this.erosionType = ErosionType.Glacial;
                break;
        }
        switch (this.hydraulicTypeInt)
        {
            case 0:
                this.hydraulicType = HydraulicType.Fast;
                break;

            case 1:
                this.hydraulicType = HydraulicType.Full;
                break;

            case 2:
                this.hydraulicType = HydraulicType.Velocity;
                break;
        }
        switch (this.generatorTypeInt)
        {
            case 0:
                this.generatorType = GeneratorType.Voronoi;
                break;

            case 1:
                this.generatorType = GeneratorType.DiamondSquare;
                break;

            case 2:
                this.generatorType = GeneratorType.Perlin;
                break;

            case 3:
                this.generatorType = GeneratorType.Smooth;
                break;

            case 4:
                this.generatorType = GeneratorType.Normalise;
                break;
        }
        switch (this.voronoiTypeInt)
        {
            case 0:
                this.voronoiType = VoronoiType.Linear;
                break;

            case 1:
                this.voronoiType = VoronoiType.Sine;
                break;

            case 2:
                this.voronoiType = VoronoiType.Tangent;
                break;
        }
        switch (this.neighbourhoodInt)
        {
            case 0:
                this.neighbourhood = Neighbourhood.Moore;
                break;

            case 1:
                this.neighbourhood = Neighbourhood.VonNeumann;
                break;
        }
    }

    public void deleteAllBlendPoints()
    {
        this.heightBlendPoints = new List<float>();
    }

    public void deleteAllSplatPrototypes()
    {
        this.splatPrototypes = new SplatPrototype[0];
    }

    public void deleteBlendPoints()
    {
        if (this.heightBlendPoints.Count > 0)
        {
            this.heightBlendPoints.RemoveAt(this.heightBlendPoints.Count - 1);
        }
        if (this.heightBlendPoints.Count > 0)
        {
            this.heightBlendPoints.RemoveAt(this.heightBlendPoints.Count - 1);
        }
    }

    public void deleteSplatPrototype(Texture2D tex, int index)
    {
        int length = 0;
        length = this.splatPrototypes.Length;
        SplatPrototype[] prototypeArray = new SplatPrototype[length - 1];
        int num2 = 0;
        for (int i = 0; i < length; i++)
        {
            if (i != index)
            {
                prototypeArray[num2] = new SplatPrototype();
                prototypeArray[num2].texture = this.splatPrototypes[i].texture;
                prototypeArray[num2].tileSize = this.splatPrototypes[i].tileSize;
                num2++;
            }
        }
        this.splatPrototypes = prototypeArray;
        if ((length - 1) > 1)
        {
            this.deleteBlendPoints();
        }
    }

    private void dsCalculateHeight(float[,] heightMap, Vector2 arraySize, int Tx, int Ty, Vector2[] points, float heightRange)
    {
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        float num3 = 0f;
        for (int i = 0; i < 4; i++)
        {
            if (points[i].x < 0f)
            {
                points[i].x += x - 1;
            }
            else if (points[i].x > x)
            {
                points[i].x -= x - 1;
            }
            else if (points[i].y < 0f)
            {
                points[i].y += y - 1;
            }
            else if (points[i].y > y)
            {
                points[i].y -= y - 1;
            }
            num3 += heightMap[(int) points[i].x, (int) points[i].y] / 4f;
        }
        num3 += (Random.value * heightRange) - (heightRange / 2f);
        if (num3 < 0f)
        {
            num3 = 0f;
        }
        else if (num3 > 1f)
        {
            num3 = 1f;
        }
        heightMap[Tx, Ty] = num3;
        if (Tx == 0)
        {
            heightMap[x - 1, Ty] = num3;
        }
        else if (Tx == (x - 1))
        {
            heightMap[0, Ty] = num3;
        }
        else if (Ty == 0)
        {
            heightMap[Tx, y - 1] = num3;
        }
        else if (Ty == (y - 1))
        {
            heightMap[Tx, 0] = num3;
        }
    }

    public void dummyErosionProgress(string titleString, string displayString, int iteration, int nIterations, float percentComplete)
    {
    }

    public void dummyGeneratorProgress(string titleString, string displayString, float percentComplete)
    {
    }

    public void dummyTextureProgress(string titleString, string displayString, float percentComplete)
    {
    }

    public void erodeAllTerrain(ErosionProgressDelegate erosionProgressDelegate)
    {
        this.erosionMode = ErosionMode.Filter;
        this.convertIntVarsToEnums();
        Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
        if (component != null)
        {
            try
            {
                int thermalIterations;
                TerrainData terrainData = component.terrainData;
                int heightmapWidth = terrainData.heightmapWidth;
                int heightmapHeight = terrainData.heightmapHeight;
                float[,] heightMap = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
                switch (this.erosionType)
                {
                    case ErosionType.Thermal:
                        thermalIterations = this.thermalIterations;
                        heightMap = this.fastErosion(heightMap, new Vector2((float) heightmapWidth, (float) heightmapHeight), thermalIterations, erosionProgressDelegate);
                        goto Label_01BE;

                    case ErosionType.Hydraulic:
                        thermalIterations = this.hydraulicIterations;
                        switch (this.hydraulicType)
                        {
                            case HydraulicType.Full:
                                goto Label_00DB;

                            case HydraulicType.Velocity:
                                goto Label_00F6;
                        }
                        goto Label_01BE;

                    case ErosionType.Tidal:
                    {
                        Vector3 size = terrainData.size;
                        if ((this.tidalSeaLevel < base.transform.position.y) || (this.tidalSeaLevel > (base.transform.position.y + size.y)))
                        {
                            goto Label_0187;
                        }
                        thermalIterations = this.tidalIterations;
                        heightMap = this.fastErosion(heightMap, new Vector2((float) heightmapWidth, (float) heightmapHeight), thermalIterations, erosionProgressDelegate);
                        goto Label_01BE;
                    }
                    case ErosionType.Wind:
                        thermalIterations = this.windIterations;
                        heightMap = this.windErosion(heightMap, new Vector2((float) heightmapWidth, (float) heightmapHeight), thermalIterations, erosionProgressDelegate);
                        goto Label_01BE;

                    default:
                        return;
                }
                heightMap = this.fastErosion(heightMap, new Vector2((float) heightmapWidth, (float) heightmapHeight), thermalIterations, erosionProgressDelegate);
                goto Label_01BE;
            Label_00DB:
                heightMap = this.fullHydraulicErosion(heightMap, new Vector2((float) heightmapWidth, (float) heightmapHeight), thermalIterations, erosionProgressDelegate);
                goto Label_01BE;
            Label_00F6:
                heightMap = this.velocityHydraulicErosion(heightMap, new Vector2((float) heightmapWidth, (float) heightmapHeight), thermalIterations, erosionProgressDelegate);
                goto Label_01BE;
            Label_0187:
                Debug.LogError("Sea level does not intersect terrain object. Erosion operation failed.");
            Label_01BE:
                terrainData.SetHeights(0, 0, heightMap);
            }
            catch (Exception exception)
            {
                Debug.LogError("An error occurred: " + exception);
            }
        }
    }

    private void erodeTerrainWithBrush()
    {
        this.erosionMode = ErosionMode.Brush;
        Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
        if (component != null)
        {
            int xBase = 0;
            int yBase = 0;
            try
            {
                TerrainData terrainData = component.terrainData;
                int heightmapWidth = terrainData.heightmapWidth;
                int heightmapHeight = terrainData.heightmapHeight;
                Vector3 size = terrainData.size;
                int width = (int) Mathf.Floor((((float) heightmapWidth) / size.x) * this.brushSize);
                int height = (int) Mathf.Floor((((float) heightmapHeight) / size.z) * this.brushSize);
                Vector3 vector2 = base.transform.InverseTransformPoint(this.brushPosition);
                xBase = (int) Mathf.Round(((vector2.x / size.x) * heightmapWidth) - (width / 2));
                yBase = (int) Mathf.Round(((vector2.z / size.z) * heightmapHeight) - (height / 2));
                if (xBase < 0)
                {
                    width += xBase;
                    xBase = 0;
                }
                if (yBase < 0)
                {
                    height += yBase;
                    yBase = 0;
                }
                if ((xBase + width) > heightmapWidth)
                {
                    width = heightmapWidth - xBase;
                }
                if ((yBase + height) > heightmapHeight)
                {
                    height = heightmapHeight - yBase;
                }
                float[,] heights = terrainData.GetHeights(xBase, yBase, width, height);
                width = heights.GetLength(1);
                height = heights.GetLength(0);
                float[,] heightMap = (float[,]) heights.Clone();
                ErosionProgressDelegate erosionProgressDelegate = new ErosionProgressDelegate(this.dummyErosionProgress);
                heightMap = this.fastErosion(heightMap, new Vector2((float) width, (float) height), 1, erosionProgressDelegate);
                float x = ((float) width) / 2f;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        float num10 = heights[j, i];
                        float num11 = heightMap[j, i];
                        float num12 = Vector2.Distance(new Vector2((float) j, (float) i), new Vector2(x, x));
                        float num13 = 1f - ((num12 - (x - (x * this.brushSoftness))) / (x * this.brushSoftness));
                        if (num13 < 0f)
                        {
                            num13 = 0f;
                        }
                        else if (num13 > 1f)
                        {
                            num13 = 1f;
                        }
                        num13 *= this.brushOpacity;
                        float num14 = (num11 * num13) + (num10 * (1f - num13));
                        heights[j, i] = num14;
                    }
                }
                terrainData.SetHeights(xBase, yBase, heights);
            }
            catch (Exception exception)
            {
                Debug.LogError("A brush error occurred: " + exception);
            }
        }
    }

    private float[,] fastErosion(float[,] heightMap, Vector2 arraySize, int iterations, ErosionProgressDelegate erosionProgressDelegate)
    {
        int y = (int) arraySize.y;
        int x = (int) arraySize.x;
        float[,] numArray = new float[y, x];
        Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
        Vector3 size = component.terrainData.size;
        float num3 = 0f;
        float num4 = 0f;
        float num5 = 0f;
        float num6 = 0f;
        float num7 = 0f;
        float num8 = 0f;
        float num9 = 0f;
        float num10 = 0f;
        float num11 = 0f;
        switch (this.erosionType)
        {
            case ErosionType.Thermal:
            {
                num3 = ((size.x / ((float) y)) * Mathf.Tan(this.thermalMinSlope * 0.01745329f)) / size.y;
                if (num3 > 1f)
                {
                    num3 = 1f;
                }
                if (this.thermalFalloff == 1f)
                {
                    this.thermalFalloff = 0.999f;
                }
                float num12 = this.thermalMinSlope + ((90f - this.thermalMinSlope) * this.thermalFalloff);
                num4 = ((size.x / ((float) y)) * Mathf.Tan(num12 * 0.01745329f)) / size.y;
                if (num4 > 1f)
                {
                    num4 = 1f;
                }
                break;
            }
            case ErosionType.Hydraulic:
            {
                num6 = ((size.x / ((float) y)) * Mathf.Tan(this.hydraulicMaxSlope * 0.01745329f)) / size.y;
                if (this.hydraulicFalloff == 0f)
                {
                    this.hydraulicFalloff = 0.001f;
                }
                float num13 = this.hydraulicMaxSlope * (1f - this.hydraulicFalloff);
                num5 = ((size.x / ((float) y)) * Mathf.Tan(num13 * 0.01745329f)) / size.y;
                break;
            }
            case ErosionType.Tidal:
                num7 = (this.tidalSeaLevel - base.transform.position.y) / (base.transform.position.y + size.y);
                num8 = ((this.tidalSeaLevel - base.transform.position.y) - this.tidalRangeAmount) / (base.transform.position.y + size.y);
                num9 = ((this.tidalSeaLevel - base.transform.position.y) + this.tidalRangeAmount) / (base.transform.position.y + size.y);
                num10 = num9 - num7;
                num11 = ((size.x / ((float) y)) * Mathf.Tan(this.tidalCliffLimit * 0.01745329f)) / size.y;
                break;

            default:
                return heightMap;
        }
        for (int i = 0; i < iterations; i++)
        {
            int num20;
            int num21 = 0;
            while (num21 < x)
            {
                int num15;
                int num17;
                int num19;
                if (num21 == 0)
                {
                    num15 = 2;
                    num17 = 0;
                    num19 = 0;
                }
                else if (num21 == (x - 1))
                {
                    num15 = 2;
                    num17 = -1;
                    num19 = 1;
                }
                else
                {
                    num15 = 3;
                    num17 = -1;
                    num19 = 1;
                }
                num20 = 0;
                while (num20 < y)
                {
                    int num14;
                    int num16;
                    int num18;
                    int num27;
                    float num28;
                    if (num20 == 0)
                    {
                        num14 = 2;
                        num16 = 0;
                        num18 = 0;
                    }
                    else if (num20 == (y - 1))
                    {
                        num14 = 2;
                        num16 = -1;
                        num18 = 1;
                    }
                    else
                    {
                        num14 = 3;
                        num16 = -1;
                        num18 = 1;
                    }
                    float num23 = 1f;
                    float num24 = 0f;
                    float num25 = 0f;
                    float num29 = heightMap[(num20 + num18) + num16, (num21 + num19) + num17];
                    float num30 = num29;
                    int num31 = 0;
                    int num26 = 0;
                    while (num26 < num15)
                    {
                        num27 = 0;
                        while (num27 < num14)
                        {
                            if (((num27 != num18) || (num26 != num19)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((num27 == num18) || (num26 == num19)))))
                            {
                                float num32 = heightMap[(num20 + num27) + num16, (num21 + num26) + num17];
                                num30 += num32;
                                num28 = num29 - num32;
                                if (num28 > 0f)
                                {
                                    num25 += num28;
                                    if (num28 < num23)
                                    {
                                        num23 = num28;
                                    }
                                    if (num28 > num24)
                                    {
                                        num24 = num28;
                                    }
                                }
                                num31++;
                            }
                            num27++;
                        }
                        num26++;
                    }
                    float num33 = num25 / ((float) num31);
                    bool flag = false;
                    switch (this.erosionType)
                    {
                        case ErosionType.Thermal:
                            if (num33 >= num3)
                            {
                                flag = true;
                            }
                            break;

                        case ErosionType.Hydraulic:
                            if ((num33 > 0f) && (num33 <= num6))
                            {
                                flag = true;
                            }
                            break;

                        case ErosionType.Tidal:
                            if (((num33 > 0f) && (num33 <= num11)) && ((num29 < num9) && (num29 > num8)))
                            {
                                flag = true;
                            }
                            break;

                        default:
                            return heightMap;
                    }
                    if (flag)
                    {
                        float num34;
                        if (this.erosionType == ErosionType.Tidal)
                        {
                            float num35 = num30 / ((float) (num31 + 1));
                            float f = Mathf.Abs((float) (num7 - num29));
                            num34 = f / num10;
                            float num37 = (num29 * num34) + (num35 * (1f - num34));
                            float num38 = Mathf.Pow(f, 3f);
                            heightMap[(num20 + num18) + num16, (num21 + num19) + num17] = (num7 * num38) + (num37 * (1f - num38));
                        }
                        else
                        {
                            float num39;
                            if (this.erosionType == ErosionType.Thermal)
                            {
                                if (num33 > num4)
                                {
                                    num34 = 1f;
                                }
                                else
                                {
                                    num39 = num4 - num3;
                                    num34 = (num33 - num3) / num39;
                                }
                            }
                            else if (num33 < num5)
                            {
                                num34 = 1f;
                            }
                            else
                            {
                                num39 = num6 - num5;
                                num34 = 1f - ((num33 - num5) / num39);
                            }
                            float num40 = (num23 / 2f) * num34;
                            float num41 = heightMap[(num20 + num18) + num16, (num21 + num19) + num17];
                            if ((this.erosionMode == ErosionMode.Filter) || ((this.erosionMode == ErosionMode.Brush) && this.useDifferenceMaps))
                            {
                                float num42 = numArray[(num20 + num18) + num16, (num21 + num19) + num17];
                                float num43 = num42 - num40;
                                numArray[(num20 + num18) + num16, (num21 + num19) + num17] = num43;
                            }
                            else
                            {
                                float num44 = num41 - num40;
                                if (num44 < 0f)
                                {
                                    num44 = 0f;
                                }
                                heightMap[(num20 + num18) + num16, (num21 + num19) + num17] = num44;
                            }
                            for (num26 = 0; num26 < num15; num26++)
                            {
                                for (num27 = 0; num27 < num14; num27++)
                                {
                                    if (((num27 != num18) || (num26 != num19)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((num27 == num18) || (num26 == num19)))))
                                    {
                                        float num45 = heightMap[(num20 + num27) + num16, (num21 + num26) + num17];
                                        num28 = num41 - num45;
                                        if (num28 > 0f)
                                        {
                                            float num46 = num40 * (num28 / num25);
                                            if ((this.erosionMode == ErosionMode.Filter) || ((this.erosionMode == ErosionMode.Brush) && this.useDifferenceMaps))
                                            {
                                                float num47 = numArray[(num20 + num27) + num16, (num21 + num26) + num17];
                                                float num48 = num47 + num46;
                                                numArray[(num20 + num27) + num16, (num21 + num26) + num17] = num48;
                                            }
                                            else
                                            {
                                                num45 += num46;
                                                if (num45 < 0f)
                                                {
                                                    num45 = 0f;
                                                }
                                                heightMap[(num20 + num27) + num16, (num21 + num26) + num17] = num45;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    num20++;
                }
                num21++;
            }
            if (((this.erosionMode == ErosionMode.Filter) || ((this.erosionMode == ErosionMode.Brush) && this.useDifferenceMaps)) && (this.erosionType != ErosionType.Tidal))
            {
                for (num21 = 0; num21 < x; num21++)
                {
                    for (num20 = 0; num20 < y; num20++)
                    {
                        float num49 = heightMap[num20, num21] + numArray[num20, num21];
                        if (num49 > 1f)
                        {
                            num49 = 1f;
                        }
                        else if (num49 < 0f)
                        {
                            num49 = 0f;
                        }
                        heightMap[num20, num21] = num49;
                        numArray[num20, num21] = 0f;
                    }
                }
            }
            if (this.erosionMode == ErosionMode.Filter)
            {
                string titleString = string.Empty;
                string displayString = string.Empty;
                switch (this.erosionType)
                {
                    case ErosionType.Thermal:
                        titleString = "Applying Thermal Erosion";
                        displayString = "Applying thermal erosion.";
                        break;

                    case ErosionType.Hydraulic:
                        titleString = "Applying Hydraulic Erosion";
                        displayString = "Applying hydraulic erosion.";
                        break;

                    case ErosionType.Tidal:
                        titleString = "Applying Tidal Erosion";
                        displayString = "Applying tidal erosion.";
                        break;

                    default:
                        return heightMap;
                }
                float percentComplete = ((float) i) / ((float) iterations);
                erosionProgressDelegate(titleString, displayString, i, iterations, percentComplete);
            }
        }
        return heightMap;
    }

    public void FastHydraulicErosion(int iterations, float maxSlope, float blendAmount)
    {
        this.erosionTypeInt = 1;
        this.erosionType = ErosionType.Hydraulic;
        this.hydraulicTypeInt = 0;
        this.hydraulicType = HydraulicType.Fast;
        this.hydraulicIterations = iterations;
        this.hydraulicMaxSlope = maxSlope;
        this.hydraulicFalloff = blendAmount;
        this.neighbourhood = Neighbourhood.Moore;
        ErosionProgressDelegate erosionProgressDelegate = new ErosionProgressDelegate(this.dummyErosionProgress);
        this.erodeAllTerrain(erosionProgressDelegate);
    }

    public void FastThermalErosion(int iterations, float minSlope, float blendAmount)
    {
        this.erosionTypeInt = 0;
        this.erosionType = ErosionType.Thermal;
        this.thermalIterations = iterations;
        this.thermalMinSlope = minSlope;
        this.thermalFalloff = blendAmount;
        this.neighbourhood = Neighbourhood.Moore;
        ErosionProgressDelegate erosionProgressDelegate = new ErosionProgressDelegate(this.dummyErosionProgress);
        this.erodeAllTerrain(erosionProgressDelegate);
    }

    public void FractalGenerator(float fractalDelta, float blend)
    {
        this.generatorTypeInt = 1;
        this.generatorType = GeneratorType.DiamondSquare;
        this.diamondSquareDelta = fractalDelta;
        this.diamondSquareBlend = blend;
        GeneratorProgressDelegate generatorProgressDelegate = new GeneratorProgressDelegate(this.dummyGeneratorProgress);
        this.generateTerrain(generatorProgressDelegate);
    }

    private float[,] fullHydraulicErosion(float[,] heightMap, Vector2 arraySize, int iterations, ErosionProgressDelegate erosionProgressDelegate)
    {
        int num3;
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        float[,] numArray = new float[x, y];
        float[,] numArray2 = new float[x, y];
        float[,] numArray3 = new float[x, y];
        float[,] numArray4 = new float[x, y];
        int num4 = 0;
        while (num4 < y)
        {
            num3 = 0;
            while (num3 < x)
            {
                numArray[num3, num4] = 0f;
                numArray2[num3, num4] = 0f;
                numArray3[num3, num4] = 0f;
                numArray4[num3, num4] = 0f;
                num3++;
            }
            num4++;
        }
        for (int i = 0; i < iterations; i++)
        {
            float num14;
            float num17;
            float num18;
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    float num21 = numArray[num3, num4] + this.hydraulicRainfall;
                    if (num21 > 1f)
                    {
                        num21 = 1f;
                    }
                    numArray[num3, num4] = num21;
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    float num22 = numArray3[num3, num4];
                    num17 = numArray[num3, num4] * this.hydraulicSedimentSaturation;
                    if (num22 < num17)
                    {
                        float num23 = numArray[num3, num4] * this.hydraulicSedimentSolubility;
                        if ((num22 + num23) > num17)
                        {
                            num23 = num17 - num22;
                        }
                        num14 = heightMap[num3, num4];
                        if (num23 > num14)
                        {
                            num23 = num14;
                        }
                        numArray3[num3, num4] = num22 + num23;
                        heightMap[num3, num4] = num14 - num23;
                    }
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                int num6;
                int num8;
                int num10;
                if (num4 == 0)
                {
                    num6 = 2;
                    num8 = 0;
                    num10 = 0;
                }
                else if (num4 == (y - 1))
                {
                    num6 = 2;
                    num8 = -1;
                    num10 = 1;
                }
                else
                {
                    num6 = 3;
                    num8 = -1;
                    num10 = 1;
                }
                num3 = 0;
                while (num3 < x)
                {
                    int num5;
                    int num7;
                    int num9;
                    int num11;
                    float num13;
                    float num15;
                    float num27;
                    if (num3 == 0)
                    {
                        num5 = 2;
                        num7 = 0;
                        num9 = 0;
                    }
                    else if (num3 == (x - 1))
                    {
                        num5 = 2;
                        num7 = -1;
                        num9 = 1;
                    }
                    else
                    {
                        num5 = 3;
                        num7 = -1;
                        num9 = 1;
                    }
                    float num24 = 0f;
                    float num25 = 0f;
                    num14 = heightMap[(num3 + num9) + num7, (num4 + num10) + num8];
                    float a = numArray[(num3 + num9) + num7, (num4 + num10) + num8];
                    float num28 = num14;
                    int num16 = 0;
                    int num12 = 0;
                    while (num12 < num6)
                    {
                        num11 = 0;
                        while (num11 < num5)
                        {
                            if (((num11 != num9) || (num12 != num10)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((num11 == num9) || (num12 == num10)))))
                            {
                                num15 = heightMap[(num3 + num11) + num7, (num4 + num12) + num8];
                                num27 = numArray[(num3 + num11) + num7, (num4 + num12) + num8];
                                num13 = (num14 + a) - (num15 + num27);
                                if (num13 > 0f)
                                {
                                    num24 += num13;
                                    num28 += num15 + num27;
                                    num16++;
                                    if (num13 > num25)
                                    {
                                        num13 = num25;
                                    }
                                }
                            }
                            num11++;
                        }
                        num12++;
                    }
                    float num29 = num28 / ((float) (num16 + 1));
                    float b = (num14 + a) - num29;
                    float num31 = Mathf.Min(a, b);
                    float num32 = numArray2[(num3 + num9) + num7, (num4 + num10) + num8];
                    float num33 = num32 - num31;
                    numArray2[(num3 + num9) + num7, (num4 + num10) + num8] = num33;
                    float num34 = numArray3[(num3 + num9) + num7, (num4 + num10) + num8];
                    float num35 = num34 * (num31 / a);
                    float num36 = numArray4[(num3 + num9) + num7, (num4 + num10) + num8];
                    float num37 = num36 - num35;
                    numArray4[(num3 + num9) + num7, (num4 + num10) + num8] = num37;
                    for (num12 = 0; num12 < num6; num12++)
                    {
                        for (num11 = 0; num11 < num5; num11++)
                        {
                            if (((num11 != num9) || (num12 != num10)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((num11 == num9) || (num12 == num10)))))
                            {
                                num15 = heightMap[(num3 + num11) + num7, (num4 + num12) + num8];
                                num27 = numArray[(num3 + num11) + num7, (num4 + num12) + num8];
                                num13 = (num14 + a) - (num15 + num27);
                                if (num13 > 0f)
                                {
                                    float num38 = numArray2[(num3 + num11) + num7, (num4 + num12) + num8];
                                    float num39 = num31 * (num13 / num24);
                                    float num40 = num38 + num39;
                                    numArray2[(num3 + num11) + num7, (num4 + num12) + num8] = num40;
                                    float num41 = numArray4[(num3 + num11) + num7, (num4 + num12) + num8];
                                    float num42 = num35 * (num13 / num24);
                                    float num43 = num41 + num42;
                                    numArray4[(num3 + num11) + num7, (num4 + num12) + num8] = num43;
                                }
                            }
                        }
                    }
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    float num44 = numArray[num3, num4] + numArray2[num3, num4];
                    float num45 = num44 * this.hydraulicEvaporation;
                    num44 -= num45;
                    if (num44 < 0f)
                    {
                        num44 = 0f;
                    }
                    numArray[num3, num4] = num44;
                    numArray2[num3, num4] = 0f;
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    num18 = numArray3[num3, num4] + numArray4[num3, num4];
                    if (num18 > 1f)
                    {
                        num18 = 1f;
                    }
                    else if (num18 < 0f)
                    {
                        num18 = 0f;
                    }
                    numArray3[num3, num4] = num18;
                    numArray4[num3, num4] = 0f;
                    num3++;
                }
                num4++;
            }
            for (num4 = 0; num4 < y; num4++)
            {
                for (num3 = 0; num3 < x; num3++)
                {
                    num17 = numArray[num3, num4] * this.hydraulicSedimentSaturation;
                    num18 = numArray3[num3, num4];
                    if (num18 > num17)
                    {
                        float num46 = num18 - num17;
                        numArray3[num3, num4] = num17;
                        float num19 = heightMap[num3, num4];
                        heightMap[num3, num4] = num19 + num46;
                    }
                }
            }
            float percentComplete = ((float) i) / ((float) iterations);
            erosionProgressDelegate("Applying Hydraulic Erosion", "Applying hydraulic erosion.", i, iterations, percentComplete);
        }
        return heightMap;
    }

    public void FullHydraulicErosion(int iterations, float rainfall, float evaporation, float solubility, float saturation)
    {
        this.erosionTypeInt = 1;
        this.erosionType = ErosionType.Hydraulic;
        this.hydraulicTypeInt = 1;
        this.hydraulicType = HydraulicType.Full;
        this.hydraulicIterations = iterations;
        this.hydraulicRainfall = rainfall;
        this.hydraulicEvaporation = evaporation;
        this.hydraulicSedimentSolubility = solubility;
        this.hydraulicSedimentSaturation = saturation;
        this.neighbourhood = Neighbourhood.Moore;
        ErosionProgressDelegate erosionProgressDelegate = new ErosionProgressDelegate(this.dummyErosionProgress);
        this.erodeAllTerrain(erosionProgressDelegate);
    }

    private float[,] generateDiamondSquare(float[,] heightMap, Vector2 arraySize, GeneratorProgressDelegate generatorProgressDelegate)
    {
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        float heightRange = 1f;
        int num4 = x - 1;
        heightMap[0, 0] = 0.5f;
        heightMap[x - 1, 0] = 0.5f;
        heightMap[0, y - 1] = 0.5f;
        heightMap[x - 1, y - 1] = 0.5f;
        generatorProgressDelegate("Fractal Generator", "Generating height map. Please wait.", 0f);
        while (num4 > 1)
        {
            for (int i = 0; i < (x - 1); i += num4)
            {
                for (int k = 0; k < (y - 1); k += num4)
                {
                    int tx = i + (num4 >> 1);
                    int ty = k + (num4 >> 1);
                    Vector2[] points = new Vector2[] { new Vector2((float) i, (float) k), new Vector2((float) (i + num4), (float) k), new Vector2((float) i, (float) (k + num4)), new Vector2((float) (i + num4), (float) (k + num4)) };
                    this.dsCalculateHeight(heightMap, arraySize, tx, ty, points, heightRange);
                }
            }
            for (int j = 0; j < (x - 1); j += num4)
            {
                for (int m = 0; m < (y - 1); m += num4)
                {
                    int num11 = num4 >> 1;
                    int num12 = j + num11;
                    int num13 = m;
                    int num14 = j;
                    int num15 = m + num11;
                    Vector2[] vectorArray2 = new Vector2[] { new Vector2((float) (num12 - num11), (float) num13), new Vector2((float) num12, (float) (num13 - num11)), new Vector2((float) (num12 + num11), (float) num13), new Vector2((float) num12, (float) (num13 + num11)) };
                    Vector2[] vectorArray3 = new Vector2[] { new Vector2((float) (num14 - num11), (float) num15), new Vector2((float) num14, (float) (num15 - num11)), new Vector2((float) (num14 + num11), (float) num15), new Vector2((float) num14, (float) (num15 + num11)) };
                    this.dsCalculateHeight(heightMap, arraySize, num12, num13, vectorArray2, heightRange);
                    this.dsCalculateHeight(heightMap, arraySize, num14, num15, vectorArray3, heightRange);
                }
            }
            heightRange *= this.diamondSquareDelta;
            num4 = num4 >> 1;
        }
        generatorProgressDelegate("Fractal Generator", "Generating height map. Please wait.", 1f);
        return heightMap;
    }

    private float[,] generatePerlin(float[,] heightMap, Vector2 arraySize, GeneratorProgressDelegate generatorProgressDelegate)
    {
        int num7;
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        for (int i = 0; i < y; i++)
        {
            for (int k = 0; k < x; k++)
            {
                heightMap[k, i] = 0f;
            }
        }
        PerlinNoise2D[] noisedArray = new PerlinNoise2D[this.perlinOctaves];
        int perlinFrequency = this.perlinFrequency;
        float num6 = 1f;
        for (num7 = 0; num7 < this.perlinOctaves; num7++)
        {
            noisedArray[num7] = new PerlinNoise2D(perlinFrequency, num6);
            perlinFrequency *= 2;
            num6 /= 2f;
        }
        for (num7 = 0; num7 < this.perlinOctaves; num7++)
        {
            double num8 = ((float) x) / ((float) noisedArray[num7].Frequency);
            double num9 = ((float) y) / ((float) noisedArray[num7].Frequency);
            for (int m = 0; m < x; m++)
            {
                for (int n = 0; n < y; n++)
                {
                    int num12 = (int) (((double) m) / num8);
                    int num13 = num12 + 1;
                    int num14 = (int) (((double) n) / num9);
                    int num15 = num14 + 1;
                    double num16 = noisedArray[num7].getInterpolatedPoint(num12, num13, num14, num15, (((double) m) / num8) - num12, (((double) n) / num9) - num14);
                    float single1 = heightMap[m, n];
                    single1[0] += (float) (num16 * noisedArray[num7].Amplitude);
                }
            }
            float percentComplete = (num7 + 1) / this.perlinOctaves;
            generatorProgressDelegate("Perlin Generator", "Generating height map. Please wait.", percentComplete);
        }
        GeneratorProgressDelegate delegate2 = new GeneratorProgressDelegate(this.dummyGeneratorProgress);
        float normaliseMin = this.normaliseMin;
        float normaliseMax = this.normaliseMax;
        float normaliseBlend = this.normaliseBlend;
        this.normaliseMin = 0f;
        this.normaliseMax = 1f;
        this.normaliseBlend = 1f;
        heightMap = this.normalise(heightMap, arraySize, delegate2);
        this.normaliseMin = normaliseMin;
        this.normaliseMax = normaliseMax;
        this.normaliseBlend = normaliseBlend;
        for (int j = 0; j < x; j++)
        {
            for (int num22 = 0; num22 < y; num22++)
            {
                heightMap[j, num22] *= this.perlinAmplitude;
            }
        }
        for (num7 = 0; num7 < this.perlinOctaves; num7++)
        {
            noisedArray[num7] = null;
        }
        noisedArray = null;
        return heightMap;
    }

    public void generateTerrain(GeneratorProgressDelegate generatorProgressDelegate)
    {
        TerrainData terrainData;
        int heightmapWidth;
        int heightmapHeight;
        float[,] numArray;
        float[,] numArray2;
        int num3;
        this.convertIntVarsToEnums();
        Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
        if (component != null)
        {
            terrainData = component.terrainData;
            heightmapWidth = terrainData.heightmapWidth;
            heightmapHeight = terrainData.heightmapHeight;
            numArray = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
            numArray2 = (float[,]) numArray.Clone();
            switch (this.generatorType)
            {
                case GeneratorType.Voronoi:
                    numArray2 = this.generateVoronoi(numArray2, new Vector2((float) heightmapWidth, (float) heightmapHeight), generatorProgressDelegate);
                    goto Label_00FE;

                case GeneratorType.DiamondSquare:
                    numArray2 = this.generateDiamondSquare(numArray2, new Vector2((float) heightmapWidth, (float) heightmapHeight), generatorProgressDelegate);
                    goto Label_00FE;

                case GeneratorType.Perlin:
                    numArray2 = this.generatePerlin(numArray2, new Vector2((float) heightmapWidth, (float) heightmapHeight), generatorProgressDelegate);
                    goto Label_00FE;

                case GeneratorType.Smooth:
                    numArray2 = this.smooth(numArray2, new Vector2((float) heightmapWidth, (float) heightmapHeight), generatorProgressDelegate);
                    goto Label_00FE;

                case GeneratorType.Normalise:
                    numArray2 = this.normalise(numArray2, new Vector2((float) heightmapWidth, (float) heightmapHeight), generatorProgressDelegate);
                    goto Label_00FE;
            }
        }
        return;
    Label_00FE:
        num3 = 0;
        while (num3 < heightmapHeight)
        {
            for (int i = 0; i < heightmapWidth; i++)
            {
                float num5 = numArray[i, num3];
                float num6 = numArray2[i, num3];
                float num7 = 0f;
                switch (this.generatorType)
                {
                    case GeneratorType.Voronoi:
                        num7 = (num6 * this.voronoiBlend) + (num5 * (1f - this.voronoiBlend));
                        break;

                    case GeneratorType.DiamondSquare:
                        num7 = (num6 * this.diamondSquareBlend) + (num5 * (1f - this.diamondSquareBlend));
                        break;

                    case GeneratorType.Perlin:
                        num7 = (num6 * this.perlinBlend) + (num5 * (1f - this.perlinBlend));
                        break;

                    case GeneratorType.Smooth:
                        num7 = (num6 * this.smoothBlend) + (num5 * (1f - this.smoothBlend));
                        break;

                    case GeneratorType.Normalise:
                        num7 = (num6 * this.normaliseBlend) + (num5 * (1f - this.normaliseBlend));
                        break;
                }
                numArray[i, num3] = num7;
            }
            num3++;
        }
        terrainData.SetHeights(0, 0, numArray);
    }

    private float[,] generateVoronoi(float[,] heightMap, Vector2 arraySize, GeneratorProgressDelegate generatorProgressDelegate)
    {
        int num7;
        int num8;
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        ArrayList list = new ArrayList();
        int num3 = 0;
        while (num3 < this.voronoiCells)
        {
            Peak peak = new Peak();
            int num4 = (int) Mathf.Floor(Random.value * x);
            int num5 = (int) Mathf.Floor(Random.value * y);
            float num6 = Random.value;
            if (Random.value > this.voronoiFeatures)
            {
                num6 = 0f;
            }
            peak.peakPoint = new Vector2((float) num4, (float) num5);
            peak.peakHeight = num6;
            list.Add(peak);
            num3++;
        }
        float num9 = 0f;
        for (num8 = 0; num8 < y; num8++)
        {
            num7 = 0;
            while (num7 < x)
            {
                float num17;
                ArrayList list2 = new ArrayList();
                for (num3 = 0; num3 < this.voronoiCells; num3++)
                {
                    Peak peak2 = (Peak) list[num3];
                    float num10 = Vector2.Distance(peak2.peakPoint, new Vector2((float) num7, (float) num8));
                    PeakDistance distance = new PeakDistance {
                        id = num3,
                        dist = num10
                    };
                    list2.Add(distance);
                }
                list2.Sort();
                PeakDistance distance2 = (PeakDistance) list2[0];
                PeakDistance distance3 = (PeakDistance) list2[1];
                int id = distance2.id;
                float dist = distance2.dist;
                float num13 = distance3.dist;
                float num14 = Mathf.Abs((float) (dist - num13)) / (((float) (x + y)) / Mathf.Sqrt((float) this.voronoiCells));
                Peak peak3 = (Peak) list[id];
                float peakHeight = peak3.peakHeight;
                float num16 = peakHeight - (Mathf.Abs((float) (dist / num13)) * peakHeight);
                switch (this.voronoiType)
                {
                    case VoronoiType.Sine:
                        num17 = (num16 * 3.141593f) - 1.570796f;
                        num16 = 0.5f + (Mathf.Sin(num17) / 2f);
                        break;

                    case VoronoiType.Tangent:
                        num17 = (num16 * 3.141593f) / 2f;
                        num16 = 0.5f + (Mathf.Tan(num17) / 2f);
                        break;
                }
                num16 = ((num16 * num14) * this.voronoiScale) + (num16 * (1f - this.voronoiScale));
                if (num16 < 0f)
                {
                    num16 = 0f;
                }
                else if (num16 > 1f)
                {
                    num16 = 1f;
                }
                heightMap[num7, num8] = num16;
                if (num16 > num9)
                {
                    num9 = num16;
                }
                num7++;
            }
            float num18 = num8 * y;
            float num19 = x * y;
            float percentComplete = num18 / num19;
            generatorProgressDelegate("Voronoi Generator", "Generating height map. Please wait.", percentComplete);
        }
        for (num8 = 0; num8 < y; num8++)
        {
            for (num7 = 0; num7 < x; num7++)
            {
                float num21 = heightMap[num7, num8] * (1f / num9);
                heightMap[num7, num8] = num21;
            }
        }
        return heightMap;
    }

    private float[,] normalise(float[,] heightMap, Vector2 arraySize, GeneratorProgressDelegate generatorProgressDelegate)
    {
        int num3;
        int num4;
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        float num5 = 0f;
        float num6 = 1f;
        generatorProgressDelegate("Normalise Filter", "Normalising height map. Please wait.", 0f);
        for (num4 = 0; num4 < y; num4++)
        {
            num3 = 0;
            while (num3 < x)
            {
                float num7 = heightMap[num3, num4];
                if (num7 < num6)
                {
                    num6 = num7;
                }
                else if (num7 > num5)
                {
                    num5 = num7;
                }
                num3++;
            }
        }
        generatorProgressDelegate("Normalise Filter", "Normalising height map. Please wait.", 0.5f);
        float num8 = num5 - num6;
        float num9 = this.normaliseMax - this.normaliseMin;
        for (num4 = 0; num4 < y; num4++)
        {
            for (num3 = 0; num3 < x; num3++)
            {
                float num10 = ((heightMap[num3, num4] - num6) / num8) * num9;
                heightMap[num3, num4] = this.normaliseMin + num10;
            }
        }
        generatorProgressDelegate("Normalise Filter", "Normalising height map. Please wait.", 1f);
        return heightMap;
    }

    public void NormaliseTerrain(float minHeight, float maxHeight, float blend)
    {
        this.generatorTypeInt = 4;
        this.generatorType = GeneratorType.Normalise;
        this.normaliseMin = minHeight;
        this.normaliseMax = maxHeight;
        this.normaliseBlend = blend;
        GeneratorProgressDelegate generatorProgressDelegate = new GeneratorProgressDelegate(this.dummyGeneratorProgress);
        this.generateTerrain(generatorProgressDelegate);
    }

    public void NormalizeTerrain(float minHeight, float maxHeight, float blend)
    {
        this.NormaliseTerrain(minHeight, maxHeight, blend);
    }

    public void OnDrawGizmos()
    {
        Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
        if (component != null)
        {
            if (this.isBrushOn && !this.isBrushHidden)
            {
                if (this.isBrushPainting)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                float x = this.brushSize / 4f;
                Gizmos.DrawLine(this.brushPosition + new Vector3(-x, 0f, 0f), this.brushPosition + new Vector3(x, 0f, 0f));
                Gizmos.DrawLine(this.brushPosition + new Vector3(0f, -x, 0f), this.brushPosition + new Vector3(0f, x, 0f));
                Gizmos.DrawLine(this.brushPosition + new Vector3(0f, 0f, -x), this.brushPosition + new Vector3(0f, 0f, x));
                Gizmos.DrawWireCube(this.brushPosition, new Vector3(this.brushSize, 0f, this.brushSize));
                Gizmos.DrawWireSphere(this.brushPosition, this.brushSize / 2f);
            }
            Vector3 size = component.terrainData.size;
            if ((this.toolModeInt == 1) && (this.erosionTypeInt == 2))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(new Vector3(base.transform.position.x + (size.x / 2f), this.tidalSeaLevel, base.transform.position.z + (size.z / 2f)), new Vector3(size.x, 0f, size.z));
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(base.transform.position.x + (size.x / 2f), this.tidalSeaLevel, base.transform.position.z + (size.z / 2f)), new Vector3(size.x, this.tidalRangeAmount * 2f, size.z));
            }
            if ((this.toolModeInt == 1) && (this.erosionTypeInt == 3))
            {
                Gizmos.color = Color.blue;
                Vector3 vector2 = (Vector3) (Quaternion.Euler(0f, this.windDirection, 0f) * Vector3.forward);
                Vector3 from = new Vector3(base.transform.position.x + (size.x / 2f), base.transform.position.y + size.y, base.transform.position.z + (size.z / 2f));
                Vector3 to = from + ((Vector3) (vector2 * (size.x / 4f)));
                Vector3 vector5 = from + ((Vector3) (vector2 * (size.x / 6f)));
                Gizmos.DrawLine(from, to);
                Gizmos.DrawLine(to, vector5 + new Vector3(0f, size.x / 16f, 0f));
                Gizmos.DrawLine(to, vector5 - new Vector3(0f, size.x / 16f, 0f));
            }
        }
    }

    public void paint()
    {
        this.convertIntVarsToEnums();
        this.erodeTerrainWithBrush();
    }

    public void PerlinGenerator(int frequency, float amplitude, int octaves, float blend)
    {
        this.generatorTypeInt = 2;
        this.generatorType = GeneratorType.Perlin;
        this.perlinFrequency = frequency;
        this.perlinAmplitude = amplitude;
        this.perlinOctaves = octaves;
        this.perlinBlend = blend;
        GeneratorProgressDelegate generatorProgressDelegate = new GeneratorProgressDelegate(this.dummyGeneratorProgress);
        this.generateTerrain(generatorProgressDelegate);
    }

    public void setFastHydraulicErosionPreset(fastHydraulicErosionPresetData preset)
    {
        this.erosionTypeInt = 1;
        this.erosionType = ErosionType.Hydraulic;
        this.hydraulicTypeInt = 0;
        this.hydraulicType = HydraulicType.Fast;
        this.hydraulicIterations = preset.hydraulicIterations;
        this.hydraulicMaxSlope = preset.hydraulicMaxSlope;
        this.hydraulicFalloff = preset.hydraulicFalloff;
    }

    public void setFractalPreset(fractalPresetData preset)
    {
        this.generatorTypeInt = 1;
        this.generatorType = GeneratorType.DiamondSquare;
        this.diamondSquareDelta = preset.diamondSquareDelta;
        this.diamondSquareBlend = preset.diamondSquareBlend;
    }

    public void setFullHydraulicErosionPreset(fullHydraulicErosionPresetData preset)
    {
        this.erosionTypeInt = 1;
        this.erosionType = ErosionType.Hydraulic;
        this.hydraulicTypeInt = 1;
        this.hydraulicType = HydraulicType.Full;
        this.hydraulicIterations = preset.hydraulicIterations;
        this.hydraulicRainfall = preset.hydraulicRainfall;
        this.hydraulicEvaporation = preset.hydraulicEvaporation;
        this.hydraulicSedimentSolubility = preset.hydraulicSedimentSolubility;
        this.hydraulicSedimentSaturation = preset.hydraulicSedimentSaturation;
    }

    public void setPerlinPreset(perlinPresetData preset)
    {
        this.generatorTypeInt = 2;
        this.generatorType = GeneratorType.Perlin;
        this.perlinFrequency = preset.perlinFrequency;
        this.perlinAmplitude = preset.perlinAmplitude;
        this.perlinOctaves = preset.perlinOctaves;
        this.perlinBlend = preset.perlinBlend;
    }

    public void setThermalErosionPreset(thermalErosionPresetData preset)
    {
        this.erosionTypeInt = 0;
        this.erosionType = ErosionType.Thermal;
        this.thermalIterations = preset.thermalIterations;
        this.thermalMinSlope = preset.thermalMinSlope;
        this.thermalFalloff = preset.thermalFalloff;
    }

    public void setTidalErosionPreset(tidalErosionPresetData preset)
    {
        this.erosionTypeInt = 2;
        this.erosionType = ErosionType.Tidal;
        this.tidalIterations = preset.tidalIterations;
        this.tidalRangeAmount = preset.tidalRangeAmount;
        this.tidalCliffLimit = preset.tidalCliffLimit;
    }

    public void setVelocityHydraulicErosionPreset(velocityHydraulicErosionPresetData preset)
    {
        this.erosionTypeInt = 1;
        this.erosionType = ErosionType.Hydraulic;
        this.hydraulicTypeInt = 2;
        this.hydraulicType = HydraulicType.Velocity;
        this.hydraulicIterations = preset.hydraulicIterations;
        this.hydraulicVelocityRainfall = preset.hydraulicVelocityRainfall;
        this.hydraulicVelocityEvaporation = preset.hydraulicVelocityEvaporation;
        this.hydraulicVelocitySedimentSolubility = preset.hydraulicVelocitySedimentSolubility;
        this.hydraulicVelocitySedimentSaturation = preset.hydraulicVelocitySedimentSaturation;
        this.hydraulicVelocity = preset.hydraulicVelocity;
        this.hydraulicMomentum = preset.hydraulicMomentum;
        this.hydraulicEntropy = preset.hydraulicEntropy;
        this.hydraulicDowncutting = preset.hydraulicDowncutting;
    }

    public void setVoronoiPreset(voronoiPresetData preset)
    {
        this.generatorTypeInt = 0;
        this.generatorType = GeneratorType.Voronoi;
        this.voronoiTypeInt = (int) preset.voronoiType;
        this.voronoiType = preset.voronoiType;
        this.voronoiCells = preset.voronoiCells;
        this.voronoiFeatures = preset.voronoiFeatures;
        this.voronoiScale = preset.voronoiScale;
        this.voronoiBlend = preset.voronoiBlend;
    }

    public void setWindErosionPreset(windErosionPresetData preset)
    {
        this.erosionTypeInt = 3;
        this.erosionType = ErosionType.Wind;
        this.windIterations = preset.windIterations;
        this.windDirection = preset.windDirection;
        this.windForce = preset.windForce;
        this.windLift = preset.windLift;
        this.windGravity = preset.windGravity;
        this.windCapacity = preset.windCapacity;
        this.windEntropy = preset.windEntropy;
        this.windSmoothing = preset.windSmoothing;
    }

    private float[,] smooth(float[,] heightMap, Vector2 arraySize, GeneratorProgressDelegate generatorProgressDelegate)
    {
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        for (int i = 0; i < this.smoothIterations; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int num4;
                int num6;
                int num8;
                if (j == 0)
                {
                    num4 = 2;
                    num6 = 0;
                    num8 = 0;
                }
                else if (j == (y - 1))
                {
                    num4 = 2;
                    num6 = -1;
                    num8 = 1;
                }
                else
                {
                    num4 = 3;
                    num6 = -1;
                    num8 = 1;
                }
                for (int k = 0; k < x; k++)
                {
                    int num3;
                    int num5;
                    int num7;
                    if (k == 0)
                    {
                        num3 = 2;
                        num5 = 0;
                        num7 = 0;
                    }
                    else if (k == (x - 1))
                    {
                        num3 = 2;
                        num5 = -1;
                        num7 = 1;
                    }
                    else
                    {
                        num3 = 3;
                        num5 = -1;
                        num7 = 1;
                    }
                    float num14 = 0f;
                    int num15 = 0;
                    for (int m = 0; m < num4; m++)
                    {
                        for (int n = 0; n < num3; n++)
                        {
                            if ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((n == num7) || (m == num8))))
                            {
                                float num16 = heightMap[(k + n) + num5, (j + m) + num6];
                                num14 += num16;
                                num15++;
                            }
                        }
                    }
                    float num17 = num14 / ((float) num15);
                    heightMap[(k + num7) + num5, (j + num8) + num6] = num17;
                }
            }
            float percentComplete = (i + 1) / this.smoothIterations;
            generatorProgressDelegate("Smoothing Filter", "Smoothing height map. Please wait.", percentComplete);
        }
        return heightMap;
    }

    public void SmoothTerrain(int iterations, float blend)
    {
        this.generatorTypeInt = 3;
        this.generatorType = GeneratorType.Smooth;
        this.smoothIterations = iterations;
        this.smoothBlend = blend;
        GeneratorProgressDelegate generatorProgressDelegate = new GeneratorProgressDelegate(this.dummyGeneratorProgress);
        this.generateTerrain(generatorProgressDelegate);
    }

    public void textureTerrain(TextureProgressDelegate textureProgressDelegate)
    {
        Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
        if (component != null)
        {
            TerrainData terrainData = component.terrainData;
            this.splatPrototypes = terrainData.splatPrototypes;
            int length = this.splatPrototypes.Length;
            if (length < 2)
            {
                Debug.LogError("Error: You must assign at least 2 textures.");
            }
            else
            {
                textureProgressDelegate("Procedural Terrain Texture", "Generating height and slope maps. Please wait.", 0.1f);
                int width = terrainData.heightmapWidth - 1;
                int height = terrainData.heightmapHeight - 1;
                float[,] numArray = new float[width, height];
                float[,] numArray2 = new float[width, height];
                terrainData.alphamapResolution = width;
                float[,,] map = terrainData.GetAlphamaps(0, 0, width, width);
                Vector3 size = terrainData.size;
                float num4 = ((size.x / ((float) width)) * Mathf.Tan(this.slopeBlendMinAngle * 0.01745329f)) / size.y;
                float num5 = ((size.x / ((float) width)) * Mathf.Tan(this.slopeBlendMaxAngle * 0.01745329f)) / size.y;
                try
                {
                    int num23;
                    float num6 = 0f;
                    float[,] numArray4 = terrainData.GetHeights(0, 0, width, height);
                    for (int i = 0; i < height; i++)
                    {
                        int num8;
                        int num10;
                        int num12;
                        if (i == 0)
                        {
                            num8 = 2;
                            num10 = 0;
                            num12 = 0;
                        }
                        else if (i == (height - 1))
                        {
                            num8 = 2;
                            num10 = -1;
                            num12 = 1;
                        }
                        else
                        {
                            num8 = 3;
                            num10 = -1;
                            num12 = 1;
                        }
                        for (int k = 0; k < width; k++)
                        {
                            int num7;
                            int num9;
                            int num11;
                            if (k == 0)
                            {
                                num7 = 2;
                                num9 = 0;
                                num11 = 0;
                            }
                            else if (k == (width - 1))
                            {
                                num7 = 2;
                                num9 = -1;
                                num11 = 1;
                            }
                            else
                            {
                                num7 = 3;
                                num9 = -1;
                                num11 = 1;
                            }
                            float num15 = numArray4[(k + num11) + num9, (i + num12) + num10];
                            if (num15 > num6)
                            {
                                num6 = num15;
                            }
                            numArray[k, i] = num15;
                            float num16 = 0f;
                            float num17 = (num7 * num8) - 1;
                            for (int m = 0; m < num8; m++)
                            {
                                for (int n = 0; n < num7; n++)
                                {
                                    if ((n != num11) || (m != num12))
                                    {
                                        float num20 = Mathf.Abs((float) (num15 - numArray4[(k + n) + num9, (i + m) + num10]));
                                        num16 += num20;
                                    }
                                }
                            }
                            float num21 = num16 / num17;
                            numArray2[k, i] = num21;
                        }
                    }
                    int num24 = 0;
                    while (num24 < height)
                    {
                        num23 = 0;
                        while (num23 < width)
                        {
                            float num22 = numArray2[num23, num24];
                            if (num22 < num4)
                            {
                                num22 = 0f;
                            }
                            else if (num22 < num5)
                            {
                                num22 = (num22 - num4) / (num5 - num4);
                            }
                            else if (num22 > num5)
                            {
                                num22 = 1f;
                            }
                            numArray2[num23, num24] = num22;
                            map[num23, num24, 0] = num22;
                            num23++;
                        }
                        num24++;
                    }
                    for (int j = 1; j < length; j++)
                    {
                        for (num24 = 0; num24 < height; num24++)
                        {
                            for (num23 = 0; num23 < width; num23++)
                            {
                                float num26 = 0f;
                                float num27 = 0f;
                                float num28 = 1f;
                                float num29 = 1f;
                                float num31 = 0f;
                                if (j > 1)
                                {
                                    num26 = this.heightBlendPoints[(j * 2) - 4];
                                    num27 = this.heightBlendPoints[(j * 2) - 3];
                                }
                                if (j < (length - 1))
                                {
                                    num28 = this.heightBlendPoints[(j * 2) - 2];
                                    num29 = this.heightBlendPoints[(j * 2) - 1];
                                }
                                float num30 = numArray[num23, num24];
                                if ((num30 >= num27) && (num30 <= num28))
                                {
                                    num31 = 1f;
                                }
                                else if ((num30 >= num26) && (num30 < num27))
                                {
                                    num31 = (num30 - num26) / (num27 - num26);
                                }
                                else if ((num30 > num28) && (num30 <= num29))
                                {
                                    num31 = 1f - ((num30 - num28) / (num29 - num28));
                                }
                                float num32 = numArray2[num23, num24];
                                num31 -= num32;
                                if (num31 < 0f)
                                {
                                    num31 = 0f;
                                }
                                map[num23, num24, j] = num31;
                            }
                        }
                    }
                    textureProgressDelegate("Procedural Terrain Texture", "Generating splat map. Please wait.", 0.9f);
                    terrainData.SetAlphamaps(0, 0, map);
                    numArray = null;
                    numArray2 = null;
                    map = null;
                }
                catch (Exception exception)
                {
                    numArray = null;
                    numArray2 = null;
                    map = null;
                    Debug.LogError("An error occurred: " + exception);
                }
            }
        }
    }

    public void TextureTerrain(float[] slopeStops, float[] heightStops, Texture2D[] textures)
    {
        if (slopeStops.Length != 2)
        {
            Debug.LogError("Error: slopeStops must have 2 values");
        }
        else if (heightStops.Length > 8)
        {
            Debug.LogError("Error: heightStops must have no more than 8 values");
        }
        else if ((heightStops.Length % 2) != 0)
        {
            Debug.LogError("Error: heightStops must have an even number of values");
        }
        else
        {
            int length = textures.Length;
            int num2 = (heightStops.Length / 2) + 2;
            if (length != num2)
            {
                Debug.LogError("Error: heightStops contains an incorrect number of values");
            }
            else
            {
                foreach (float num3 in slopeStops)
                {
                    if ((num3 < 0f) || (num3 > 90f))
                    {
                        Debug.LogError("Error: The value of all slopeStops must be in the range 0.0 to 90.0");
                        return;
                    }
                }
                foreach (float num5 in heightStops)
                {
                    if ((num5 < 0f) || (num5 > 1f))
                    {
                        Debug.LogError("Error: The value of all heightStops must be in the range 0.0 to 1.0");
                        return;
                    }
                }
                Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
                TerrainData terrainData = component.terrainData;
                this.splatPrototypes = terrainData.splatPrototypes;
                this.deleteAllSplatPrototypes();
                int index = 0;
                foreach (Texture2D textured in textures)
                {
                    this.addSplatPrototype(textured, index);
                    index++;
                }
                this.slopeBlendMinAngle = slopeStops[0];
                this.slopeBlendMaxAngle = slopeStops[1];
                index = 0;
                foreach (float num9 in heightStops)
                {
                    this.heightBlendPoints[index] = num9;
                    index++;
                }
                terrainData.splatPrototypes = this.splatPrototypes;
                TextureProgressDelegate textureProgressDelegate = new TextureProgressDelegate(this.dummyTextureProgress);
                this.textureTerrain(textureProgressDelegate);
            }
        }
    }

    public void TidalErosion(int iterations, float seaLevel, float tidalRange, float cliffLimit)
    {
        this.erosionTypeInt = 2;
        this.erosionType = ErosionType.Tidal;
        this.tidalIterations = iterations;
        this.tidalSeaLevel = seaLevel;
        this.tidalRangeAmount = tidalRange;
        this.tidalCliffLimit = cliffLimit;
        this.neighbourhood = Neighbourhood.Moore;
        ErosionProgressDelegate erosionProgressDelegate = new ErosionProgressDelegate(this.dummyErosionProgress);
        this.erodeAllTerrain(erosionProgressDelegate);
    }

    public void Update()
    {
        if (this.isBrushOn && (((this.toolModeInt != 1) || (this.erosionTypeInt > 2)) || ((this.erosionTypeInt == 1) && (this.hydraulicTypeInt > 0))))
        {
            this.isBrushOn = false;
        }
    }

    private float[,] velocityHydraulicErosion(float[,] heightMap, Vector2 arraySize, int iterations, ErosionProgressDelegate erosionProgressDelegate)
    {
        int num3;
        int num4;
        int num5;
        int num6;
        int num7;
        int num8;
        int num9;
        int num10;
        float num13;
        int num14;
        int num15;
        float num16;
        float num17;
        float num18;
        int num19;
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        float[,] numArray = new float[x, y];
        float[,] numArray2 = new float[x, y];
        float[,] numArray3 = new float[x, y];
        float[,] numArray4 = new float[x, y];
        float[,] numArray5 = new float[x, y];
        float[,] numArray6 = new float[x, y];
        float[,] numArray7 = new float[x, y];
        float[,] numArray8 = new float[x, y];
        for (num4 = 0; num4 < y; num4++)
        {
            num3 = 0;
            while (num3 < x)
            {
                numArray3[num3, num4] = 0f;
                numArray4[num3, num4] = 0f;
                numArray5[num3, num4] = 0f;
                numArray6[num3, num4] = 0f;
                numArray7[num3, num4] = 0f;
                numArray8[num3, num4] = 0f;
                num3++;
            }
        }
        for (num4 = 0; num4 < y; num4++)
        {
            num3 = 0;
            while (num3 < x)
            {
                float num12 = heightMap[num3, num4];
                numArray[num3, num4] = num12;
                num3++;
            }
        }
        num4 = 0;
        while (num4 < y)
        {
            if (num4 == 0)
            {
                num6 = 2;
                num8 = 0;
                num10 = 0;
            }
            else if (num4 == (y - 1))
            {
                num6 = 2;
                num8 = -1;
                num10 = 1;
            }
            else
            {
                num6 = 3;
                num8 = -1;
                num10 = 1;
            }
            num3 = 0;
            while (num3 < x)
            {
                if (num3 == 0)
                {
                    num5 = 2;
                    num7 = 0;
                    num9 = 0;
                }
                else if (num3 == (x - 1))
                {
                    num5 = 2;
                    num7 = -1;
                    num9 = 1;
                }
                else
                {
                    num5 = 3;
                    num7 = -1;
                    num9 = 1;
                }
                num13 = 0f;
                num17 = heightMap[(num3 + num9) + num7, (num4 + num10) + num8];
                num19 = 0;
                num15 = 0;
                while (num15 < num6)
                {
                    num14 = 0;
                    while (num14 < num5)
                    {
                        if (((num14 != num9) || (num15 != num10)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((num14 == num9) || (num15 == num10)))))
                        {
                            num18 = heightMap[(num3 + num14) + num7, (num4 + num15) + num8];
                            num16 = Mathf.Abs((float) (num17 - num18));
                            num13 += num16;
                            num19++;
                        }
                        num14++;
                    }
                    num15++;
                }
                float num20 = num13 / ((float) num19);
                numArray2[(num3 + num9) + num7, (num4 + num10) + num8] = num20;
                num3++;
            }
            num4++;
        }
        for (int i = 0; i < iterations; i++)
        {
            float num11;
            float num49;
            float num52;
            float num53;
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    float num22 = numArray3[num3, num4] + (numArray[num3, num4] * this.hydraulicVelocityRainfall);
                    if (num22 > 1f)
                    {
                        num22 = 1f;
                    }
                    numArray3[num3, num4] = num22;
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    float num23 = numArray7[num3, num4];
                    num11 = numArray3[num3, num4] * this.hydraulicVelocitySedimentSaturation;
                    if (num23 < num11)
                    {
                        float num24 = (numArray3[num3, num4] * numArray5[num3, num4]) * this.hydraulicVelocitySedimentSolubility;
                        if ((num23 + num24) > num11)
                        {
                            num24 = num11 - num23;
                        }
                        num17 = heightMap[num3, num4];
                        if (num24 > num17)
                        {
                            num24 = num17;
                        }
                        numArray7[num3, num4] = num23 + num24;
                        heightMap[num3, num4] = num17 - num24;
                    }
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                if (num4 == 0)
                {
                    num6 = 2;
                    num8 = 0;
                    num10 = 0;
                }
                else if (num4 == (y - 1))
                {
                    num6 = 2;
                    num8 = -1;
                    num10 = 1;
                }
                else
                {
                    num6 = 3;
                    num8 = -1;
                    num10 = 1;
                }
                num3 = 0;
                while (num3 < x)
                {
                    float num27;
                    if (num3 == 0)
                    {
                        num5 = 2;
                        num7 = 0;
                        num9 = 0;
                    }
                    else if (num3 == (x - 1))
                    {
                        num5 = 2;
                        num7 = -1;
                        num9 = 1;
                    }
                    else
                    {
                        num5 = 3;
                        num7 = -1;
                        num9 = 1;
                    }
                    num13 = 0f;
                    num17 = heightMap[num3, num4];
                    float num25 = num17;
                    float a = numArray3[num3, num4];
                    num19 = 0;
                    num15 = 0;
                    while (num15 < num6)
                    {
                        num14 = 0;
                        while (num14 < num5)
                        {
                            if (((num14 != num9) || (num15 != num10)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((num14 == num9) || (num15 == num10)))))
                            {
                                num18 = heightMap[(num3 + num14) + num7, (num4 + num15) + num8];
                                num27 = numArray3[(num3 + num14) + num7, (num4 + num15) + num8];
                                num16 = (num17 + a) - (num18 + num27);
                                if (num16 > 0f)
                                {
                                    num13 += num16;
                                    num25 += num17 + a;
                                    num19++;
                                }
                            }
                            num14++;
                        }
                        num15++;
                    }
                    float num28 = numArray5[num3, num4];
                    float num29 = numArray2[num3, num4];
                    float num30 = numArray7[num3, num4];
                    float num31 = num28 + (this.hydraulicVelocity * num29);
                    float num32 = num25 / ((float) (num19 + 1));
                    float num33 = (num17 + a) - num32;
                    float num34 = Mathf.Min(a, num33 * (1f + num28));
                    float num35 = numArray4[num3, num4];
                    float num36 = num35 - num34;
                    numArray4[num3, num4] = num36;
                    float num37 = num31 * (num34 / a);
                    float num38 = num30 * (num34 / a);
                    for (num15 = 0; num15 < num6; num15++)
                    {
                        for (num14 = 0; num14 < num5; num14++)
                        {
                            if (((num14 != num9) || (num15 != num10)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((num14 == num9) || (num15 == num10)))))
                            {
                                num18 = heightMap[(num3 + num14) + num7, (num4 + num15) + num8];
                                num27 = numArray3[(num3 + num14) + num7, (num4 + num15) + num8];
                                num16 = (num17 + a) - (num18 + num27);
                                if (num16 > 0f)
                                {
                                    float num39 = numArray4[(num3 + num14) + num7, (num4 + num15) + num8];
                                    float num40 = num34 * (num16 / num13);
                                    float num41 = num39 + num40;
                                    numArray4[(num3 + num14) + num7, (num4 + num15) + num8] = num41;
                                    float num42 = numArray6[(num3 + num14) + num7, (num4 + num15) + num8];
                                    float num43 = (num37 * this.hydraulicMomentum) * (num16 / num13);
                                    float num44 = num42 + num43;
                                    numArray6[(num3 + num14) + num7, (num4 + num15) + num8] = num44;
                                    float num45 = numArray8[(num3 + num14) + num7, (num4 + num15) + num8];
                                    float num46 = (num38 * this.hydraulicMomentum) * (num16 / num13);
                                    float num47 = num45 + num46;
                                    numArray8[(num3 + num14) + num7, (num4 + num15) + num8] = num47;
                                }
                            }
                        }
                    }
                    float num48 = numArray6[num3, num4];
                    numArray6[num3, num4] = num48 - num37;
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    num49 = numArray5[num3, num4] + numArray6[num3, num4];
                    num49 *= 1f - this.hydraulicEntropy;
                    if (num49 > 1f)
                    {
                        num49 = 1f;
                    }
                    else if (num49 < 0f)
                    {
                        num49 = 0f;
                    }
                    numArray5[num3, num4] = num49;
                    numArray6[num3, num4] = 0f;
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    float num50 = numArray3[num3, num4] + numArray4[num3, num4];
                    float num51 = num50 * this.hydraulicVelocityEvaporation;
                    num50 -= num51;
                    if (num50 > 1f)
                    {
                        num50 = 1f;
                    }
                    else if (num50 < 0f)
                    {
                        num50 = 0f;
                    }
                    numArray3[num3, num4] = num50;
                    numArray4[num3, num4] = 0f;
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    num52 = numArray7[num3, num4] + numArray8[num3, num4];
                    if (num52 > 1f)
                    {
                        num52 = 1f;
                    }
                    else if (num52 < 0f)
                    {
                        num52 = 0f;
                    }
                    numArray7[num3, num4] = num52;
                    numArray8[num3, num4] = 0f;
                    num3++;
                }
                num4++;
            }
            num4 = 0;
            while (num4 < y)
            {
                num3 = 0;
                while (num3 < x)
                {
                    num11 = numArray3[num3, num4] * this.hydraulicVelocitySedimentSaturation;
                    num52 = numArray7[num3, num4];
                    if (num52 > num11)
                    {
                        float num54 = num52 - num11;
                        numArray7[num3, num4] = num11;
                        num53 = heightMap[num3, num4];
                        heightMap[num3, num4] = num53 + num54;
                    }
                    num3++;
                }
                num4++;
            }
            for (num4 = 0; num4 < y; num4++)
            {
                for (num3 = 0; num3 < x; num3++)
                {
                    num49 = numArray3[num3, num4];
                    num53 = heightMap[num3, num4];
                    float num55 = 1f - (Mathf.Abs((float) (0.5f - num53)) * 2f);
                    float num56 = (this.hydraulicDowncutting * num49) * num55;
                    num53 -= num56;
                    heightMap[num3, num4] = num53;
                }
            }
            float percentComplete = ((float) i) / ((float) iterations);
            erosionProgressDelegate("Applying Hydraulic Erosion", "Applying hydraulic erosion.", i, iterations, percentComplete);
        }
        return heightMap;
    }

    public void VelocityHydraulicErosion(int iterations, float rainfall, float evaporation, float solubility, float saturation, float velocity, float momentum, float entropy, float downcutting)
    {
        this.erosionTypeInt = 1;
        this.erosionType = ErosionType.Hydraulic;
        this.hydraulicTypeInt = 2;
        this.hydraulicType = HydraulicType.Velocity;
        this.hydraulicIterations = iterations;
        this.hydraulicVelocityRainfall = rainfall;
        this.hydraulicVelocityEvaporation = evaporation;
        this.hydraulicVelocitySedimentSolubility = solubility;
        this.hydraulicVelocitySedimentSaturation = saturation;
        this.hydraulicVelocity = velocity;
        this.hydraulicMomentum = momentum;
        this.hydraulicEntropy = entropy;
        this.hydraulicDowncutting = downcutting;
        this.neighbourhood = Neighbourhood.Moore;
        ErosionProgressDelegate erosionProgressDelegate = new ErosionProgressDelegate(this.dummyErosionProgress);
        this.erodeAllTerrain(erosionProgressDelegate);
    }

    public void VoronoiGenerator(FeatureType featureType, int cells, float features, float scale, float blend)
    {
        this.generatorTypeInt = 0;
        this.generatorType = GeneratorType.Voronoi;
        switch (featureType)
        {
            case FeatureType.Mountains:
                this.voronoiTypeInt = 0;
                this.voronoiType = VoronoiType.Linear;
                break;

            case FeatureType.Hills:
                this.voronoiTypeInt = 1;
                this.voronoiType = VoronoiType.Sine;
                break;

            case FeatureType.Plateaus:
                this.voronoiTypeInt = 2;
                this.voronoiType = VoronoiType.Tangent;
                break;
        }
        this.voronoiCells = cells;
        this.voronoiFeatures = features;
        this.voronoiScale = scale;
        this.voronoiBlend = blend;
        GeneratorProgressDelegate generatorProgressDelegate = new GeneratorProgressDelegate(this.dummyGeneratorProgress);
        this.generateTerrain(generatorProgressDelegate);
    }

    private float[,] windErosion(float[,] heightMap, Vector2 arraySize, int iterations, ErosionProgressDelegate erosionProgressDelegate)
    {
        int num9;
        Terrain component = (Terrain) base.GetComponent(typeof(Terrain));
        TerrainData terrainData = component.terrainData;
        Vector3 to = (Vector3) (Quaternion.Euler(0f, this.windDirection + 180f, 0f) * Vector3.forward);
        int x = (int) arraySize.x;
        int y = (int) arraySize.y;
        float[,] numArray = new float[x, y];
        float[,] numArray2 = new float[x, y];
        float[,] numArray3 = new float[x, y];
        float[,] numArray4 = new float[x, y];
        float[,] numArray5 = new float[x, y];
        float[,] numArray6 = new float[x, y];
        float[,] numArray7 = new float[x, y];
        int num10 = 0;
        while (num10 < y)
        {
            num9 = 0;
            while (num9 < x)
            {
                numArray[num9, num10] = 0f;
                numArray2[num9, num10] = 0f;
                numArray3[num9, num10] = 0f;
                numArray4[num9, num10] = 0f;
                numArray5[num9, num10] = 0f;
                numArray6[num9, num10] = 0f;
                numArray7[num9, num10] = 0f;
                num9++;
            }
            num10++;
        }
        for (int i = 0; i < iterations; i++)
        {
            float num11;
            float num12;
            num10 = 0;
            while (num10 < y)
            {
                num9 = 0;
                while (num9 < x)
                {
                    num12 = numArray3[num9, num10];
                    float num14 = heightMap[num9, num10];
                    float num15 = numArray5[num9, num10];
                    float num16 = num15 * this.windGravity;
                    numArray5[num9, num10] = num15 - num16;
                    heightMap[num9, num10] = num14 + num16;
                    num9++;
                }
                num10++;
            }
            num10 = 0;
            while (num10 < y)
            {
                num9 = 0;
                while (num9 < x)
                {
                    float num17 = heightMap[num9, num10];
                    Vector3 interpolatedNormal = terrainData.GetInterpolatedNormal(((float) num9) / ((float) x), ((float) num10) / ((float) y));
                    float num18 = (Vector3.Angle(interpolatedNormal, to) - 90f) / 90f;
                    if (num18 < 0f)
                    {
                        num18 = 0f;
                    }
                    numArray[num9, num10] = num18 * num17;
                    float num19 = 1f - (Mathf.Abs((float) (Vector3.Angle(interpolatedNormal, to) - 90f)) / 90f);
                    numArray2[num9, num10] = num19 * num17;
                    float num20 = (num19 * num17) * this.windForce;
                    float num21 = numArray3[num9, num10];
                    float num22 = num21 + num20;
                    numArray3[num9, num10] = num22;
                    num11 = numArray5[num9, num10];
                    float num23 = this.windLift * num22;
                    if ((num11 + num23) > this.windCapacity)
                    {
                        num23 = this.windCapacity - num11;
                    }
                    numArray5[num9, num10] = num11 + num23;
                    heightMap[num9, num10] = num17 - num23;
                    num9++;
                }
                num10++;
            }
            num10 = 0;
            while (num10 < y)
            {
                int num4;
                int num6;
                int num8;
                if (num10 == 0)
                {
                    num4 = 2;
                    num6 = 0;
                    num8 = 0;
                }
                else if (num10 == (y - 1))
                {
                    num4 = 2;
                    num6 = -1;
                    num8 = 1;
                }
                else
                {
                    num4 = 3;
                    num6 = -1;
                    num8 = 1;
                }
                num9 = 0;
                while (num9 < x)
                {
                    int num3;
                    int num5;
                    int num7;
                    if (num9 == 0)
                    {
                        num3 = 2;
                        num5 = 0;
                        num7 = 0;
                    }
                    else if (num9 == (x - 1))
                    {
                        num3 = 2;
                        num5 = -1;
                        num7 = 1;
                    }
                    else
                    {
                        num3 = 3;
                        num5 = -1;
                        num7 = 1;
                    }
                    float num26 = numArray2[num9, num10];
                    float num27 = numArray[num9, num10];
                    num11 = numArray5[num9, num10];
                    for (int j = 0; j < num4; j++)
                    {
                        for (int k = 0; k < num3; k++)
                        {
                            if (((k != num7) || (j != num8)) && ((this.neighbourhood == Neighbourhood.Moore) || ((this.neighbourhood == Neighbourhood.VonNeumann) && ((k == num7) || (j == num8)))))
                            {
                                Vector3 from = new Vector3((float) (k + num5), 0f, (float) (-1 * (j + num6)));
                                float num28 = (90f - Vector3.Angle(from, to)) / 90f;
                                if (num28 < 0f)
                                {
                                    num28 = 0f;
                                }
                                float num29 = numArray4[(num9 + k) + num5, (num10 + j) + num6];
                                float num30 = (num28 * (num26 - num27)) * 0.1f;
                                if (num30 < 0f)
                                {
                                    num30 = 0f;
                                }
                                float num31 = num29 + num30;
                                numArray4[(num9 + k) + num5, (num10 + j) + num6] = num31;
                                float num32 = numArray4[num9, num10];
                                float num33 = num32 - num30;
                                numArray4[num9, num10] = num33;
                                float num34 = numArray6[(num9 + k) + num5, (num10 + j) + num6];
                                float num35 = num11 * num30;
                                float num36 = num34 + num35;
                                numArray6[(num9 + k) + num5, (num10 + j) + num6] = num36;
                                float num37 = numArray6[num9, num10];
                                float num38 = num37 - num35;
                                numArray6[num9, num10] = num38;
                            }
                        }
                    }
                    num9++;
                }
                num10++;
            }
            num10 = 0;
            while (num10 < y)
            {
                num9 = 0;
                while (num9 < x)
                {
                    float num39 = numArray5[num9, num10] + numArray6[num9, num10];
                    if (num39 > 1f)
                    {
                        num39 = 1f;
                    }
                    else if (num39 < 0f)
                    {
                        num39 = 0f;
                    }
                    numArray5[num9, num10] = num39;
                    numArray6[num9, num10] = 0f;
                    num9++;
                }
                num10++;
            }
            num10 = 0;
            while (num10 < y)
            {
                num9 = 0;
                while (num9 < x)
                {
                    num12 = numArray3[num9, num10] + numArray4[num9, num10];
                    num12 *= 1f - this.windEntropy;
                    if (num12 > 1f)
                    {
                        num12 = 1f;
                    }
                    else if (num12 < 0f)
                    {
                        num12 = 0f;
                    }
                    numArray3[num9, num10] = num12;
                    numArray4[num9, num10] = 0f;
                    num9++;
                }
                num10++;
            }
            this.smoothIterations = 1;
            this.smoothBlend = 0.25f;
            float[,] numArray8 = (float[,]) heightMap.Clone();
            GeneratorProgressDelegate generatorProgressDelegate = new GeneratorProgressDelegate(this.dummyGeneratorProgress);
            numArray8 = this.smooth(numArray8, arraySize, generatorProgressDelegate);
            for (num10 = 0; num10 < y; num10++)
            {
                for (num9 = 0; num9 < x; num9++)
                {
                    float num40 = heightMap[num9, num10];
                    float num41 = numArray8[num9, num10];
                    float num42 = numArray[num9, num10] * this.windSmoothing;
                    float num43 = (num41 * num42) + (num40 * (1f - num42));
                    heightMap[num9, num10] = num43;
                }
            }
            float percentComplete = ((float) i) / ((float) iterations);
            erosionProgressDelegate("Applying Wind Erosion", "Applying wind erosion.", i, iterations, percentComplete);
        }
        return heightMap;
    }

    public void WindErosion(int iterations, float direction, float force, float lift, float gravity, float capacity, float entropy, float smoothing)
    {
        this.erosionTypeInt = 3;
        this.erosionType = ErosionType.Wind;
        this.windIterations = iterations;
        this.windDirection = direction;
        this.windForce = force;
        this.windLift = lift;
        this.windGravity = gravity;
        this.windCapacity = capacity;
        this.windEntropy = entropy;
        this.windSmoothing = smoothing;
        this.neighbourhood = Neighbourhood.Moore;
        ErosionProgressDelegate erosionProgressDelegate = new ErosionProgressDelegate(this.dummyErosionProgress);
        this.erodeAllTerrain(erosionProgressDelegate);
    }

    public enum ErosionMode
    {
        Filter,
        Brush
    }

    public delegate void ErosionProgressDelegate(string titleString, string displayString, int iteration, int nIterations, float percentComplete);

    public enum ErosionType
    {
        Thermal,
        Hydraulic,
        Tidal,
        Wind,
        Glacial
    }

    public class fastHydraulicErosionPresetData
    {
        public float hydraulicFalloff;
        public int hydraulicIterations;
        public float hydraulicMaxSlope;
        public string presetName;

        public fastHydraulicErosionPresetData(string pn, int hi, float hms, float hba)
        {
            this.presetName = pn;
            this.hydraulicIterations = hi;
            this.hydraulicMaxSlope = hms;
            this.hydraulicFalloff = hba;
        }
    }

    public enum FeatureType
    {
        Mountains,
        Hills,
        Plateaus
    }

    public class fractalPresetData
    {
        public float diamondSquareBlend;
        public float diamondSquareDelta;
        public string presetName;

        public fractalPresetData(string pn, float dsd, float dsb)
        {
            this.presetName = pn;
            this.diamondSquareDelta = dsd;
            this.diamondSquareBlend = dsb;
        }
    }

    public class fullHydraulicErosionPresetData
    {
        public float hydraulicEvaporation;
        public int hydraulicIterations;
        public float hydraulicRainfall;
        public float hydraulicSedimentSaturation;
        public float hydraulicSedimentSolubility;
        public string presetName;

        public fullHydraulicErosionPresetData(string pn, int hi, float hr, float he, float hso, float hsa)
        {
            this.presetName = pn;
            this.hydraulicIterations = hi;
            this.hydraulicRainfall = hr;
            this.hydraulicEvaporation = he;
            this.hydraulicSedimentSolubility = hso;
            this.hydraulicSedimentSaturation = hsa;
        }
    }

    public delegate void GeneratorProgressDelegate(string titleString, string displayString, float percentComplete);

    public enum GeneratorType
    {
        Voronoi,
        DiamondSquare,
        Perlin,
        Smooth,
        Normalise
    }

    public enum HydraulicType
    {
        Fast,
        Full,
        Velocity
    }

    public enum Neighbourhood
    {
        Moore,
        VonNeumann
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Peak
    {
        public Vector2 peakPoint;
        public float peakHeight;
    }

    public class PeakDistance : IComparable
    {
        public float dist;
        public int id;

        public int CompareTo(object obj)
        {
            TerrainToolkit.PeakDistance distance = (TerrainToolkit.PeakDistance) obj;
            int num = this.dist.CompareTo(distance.dist);
            if (num == 0)
            {
                num = this.dist.CompareTo(distance.dist);
            }
            return num;
        }
    }

    public class PerlinNoise2D
    {
        private float amplitude = 1f;
        private int frequency = 1;
        private double[,] noiseValues;

        public PerlinNoise2D(int freq, float _amp)
        {
            Random random = new Random(Environment.TickCount);
            this.noiseValues = new double[freq, freq];
            this.amplitude = _amp;
            this.frequency = freq;
            for (int i = 0; i < freq; i++)
            {
                for (int j = 0; j < freq; j++)
                {
                    this.noiseValues[i, j] = random.NextDouble();
                }
            }
        }

        public double getInterpolatedPoint(int _xa, int _xb, int _ya, int _yb, double Px, double Py)
        {
            double pa = this.interpolate(this.noiseValues[_xa % this.Frequency, _ya % this.frequency], this.noiseValues[_xb % this.Frequency, _ya % this.frequency], Px);
            double pb = this.interpolate(this.noiseValues[_xa % this.Frequency, _yb % this.frequency], this.noiseValues[_xb % this.Frequency, _yb % this.frequency], Px);
            return this.interpolate(pa, pb, Py);
        }

        private double interpolate(double Pa, double Pb, double Px)
        {
            double num = Px * 3.1415927410125732;
            double num2 = (1f - Mathf.Cos((float) num)) * 0.5;
            return ((Pa * (1.0 - num2)) + (Pb * num2));
        }

        public float Amplitude
        {
            get
            {
                return this.amplitude;
            }
        }

        public int Frequency
        {
            get
            {
                return this.frequency;
            }
        }
    }

    public class perlinPresetData
    {
        public float perlinAmplitude;
        public float perlinBlend;
        public int perlinFrequency;
        public int perlinOctaves;
        public string presetName;

        public perlinPresetData(string pn, int pf, float pa, int po, float pb)
        {
            this.presetName = pn;
            this.perlinFrequency = pf;
            this.perlinAmplitude = pa;
            this.perlinOctaves = po;
            this.perlinBlend = pb;
        }
    }

    public delegate void TextureProgressDelegate(string titleString, string displayString, float percentComplete);

    public class thermalErosionPresetData
    {
        public string presetName;
        public float thermalFalloff;
        public int thermalIterations;
        public float thermalMinSlope;

        public thermalErosionPresetData(string pn, int ti, float tms, float tba)
        {
            this.presetName = pn;
            this.thermalIterations = ti;
            this.thermalMinSlope = tms;
            this.thermalFalloff = tba;
        }
    }

    public class tidalErosionPresetData
    {
        public string presetName;
        public float tidalCliffLimit;
        public int tidalIterations;
        public float tidalRangeAmount;

        public tidalErosionPresetData(string pn, int ti, float tra, float tcl)
        {
            this.presetName = pn;
            this.tidalIterations = ti;
            this.tidalRangeAmount = tra;
            this.tidalCliffLimit = tcl;
        }
    }

    public enum ToolMode
    {
        Create,
        Erode,
        Texture
    }

    public class velocityHydraulicErosionPresetData
    {
        public float hydraulicDowncutting;
        public float hydraulicEntropy;
        public int hydraulicIterations;
        public float hydraulicMomentum;
        public float hydraulicVelocity;
        public float hydraulicVelocityEvaporation;
        public float hydraulicVelocityRainfall;
        public float hydraulicVelocitySedimentSaturation;
        public float hydraulicVelocitySedimentSolubility;
        public string presetName;

        public velocityHydraulicErosionPresetData(string pn, int hi, float hvr, float hve, float hso, float hsa, float hv, float hm, float he, float hd)
        {
            this.presetName = pn;
            this.hydraulicIterations = hi;
            this.hydraulicVelocityRainfall = hvr;
            this.hydraulicVelocityEvaporation = hve;
            this.hydraulicVelocitySedimentSolubility = hso;
            this.hydraulicVelocitySedimentSaturation = hsa;
            this.hydraulicVelocity = hv;
            this.hydraulicMomentum = hm;
            this.hydraulicEntropy = he;
            this.hydraulicDowncutting = hd;
        }
    }

    public class voronoiPresetData
    {
        public string presetName;
        public float voronoiBlend;
        public int voronoiCells;
        public float voronoiFeatures;
        public float voronoiScale;
        public TerrainToolkit.VoronoiType voronoiType;

        public voronoiPresetData(string pn, TerrainToolkit.VoronoiType vt, int c, float vf, float vs, float vb)
        {
            this.presetName = pn;
            this.voronoiType = vt;
            this.voronoiCells = c;
            this.voronoiFeatures = vf;
            this.voronoiScale = vs;
            this.voronoiBlend = vb;
        }
    }

    public enum VoronoiType
    {
        Linear,
        Sine,
        Tangent
    }

    public class windErosionPresetData
    {
        public string presetName;
        public float windCapacity;
        public float windDirection;
        public float windEntropy;
        public float windForce;
        public float windGravity;
        public int windIterations;
        public float windLift;
        public float windSmoothing;

        public windErosionPresetData(string pn, int wi, float wd, float wf, float wl, float wg, float wc, float we, float ws)
        {
            this.presetName = pn;
            this.windIterations = wi;
            this.windDirection = wd;
            this.windForce = wf;
            this.windLift = wl;
            this.windGravity = wg;
            this.windCapacity = wc;
            this.windEntropy = we;
            this.windSmoothing = ws;
        }
    }
}

