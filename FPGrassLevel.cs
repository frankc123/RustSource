using System;
using System.Collections.Generic;
using UnityEngine;

public class FPGrassLevel : MonoBehaviour, IFPGrassAsset
{
    [SerializeField]
    private List<FPGrassPatch> children = new List<FPGrassPatch>();
    [SerializeField]
    private int gridSize;
    [SerializeField]
    private int gridSizeAtLevel;
    [SerializeField]
    private float gridSpacingAtLevel;
    private Vector3 lastPosition;
    public Material levelMaterial;
    public int levelNumber;
    [SerializeField]
    private float levelSize;
    public FPGrass parent;
    public FPGrassProbabilityGenerator probabilityGenerator;
    [NonSerialized]
    private bool probabilityUpdateQueued;

    internal void Draw(FPGrassPatch patch, Mesh mesh, ref Vector3 renderPosition, ref FPGrass.RenderArguments renderArgs)
    {
        if (this.probabilityUpdateQueued || grass.forceredraw)
        {
            this.UpdateMapsNow(this.lastPosition);
            this.probabilityUpdateQueued = false;
        }
        if (grass.displacement)
        {
            Graphics.Blit(FPGrassDisplacementCamera.GetRT(), this.probabilityGenerator.probabilityTexture, FPGrassDisplacementCamera.GetBlitMat());
        }
        if (renderArgs.immediate)
        {
            GL.PushMatrix();
            this.levelMaterial.SetPass(0);
            Graphics.DrawMeshNow(mesh, renderPosition, Constant.rotation, 0);
            GL.PopMatrix();
        }
        else
        {
            Graphics.DrawMesh(mesh, renderPosition, Constant.rotation, this.levelMaterial, base.gameObject.layer, base.camera, 0, null, FPGrass.castShadows, FPGrass.receiveShadows);
        }
    }

    private void OnDestroy()
    {
        this.probabilityGenerator.DestroyObjects();
    }

    internal void Render(ref FPGrass.RenderArguments renderArgs)
    {
        FPGrass.RenderArguments arguments = renderArgs;
        arguments.center = this.lastPosition;
        foreach (FPGrassPatch patch in this.children)
        {
            if (patch.enabled)
            {
                patch.Render(ref arguments);
            }
        }
    }

    public void UpdateLevel(Vector3 position, Terrain terrain)
    {
        int num = Mathf.FloorToInt(position.x / this.gridSpacingAtLevel);
        int num2 = Mathf.FloorToInt(position.z / this.gridSpacingAtLevel);
        Vector3 zero = Vector3.zero;
        zero.x = num * this.gridSpacingAtLevel;
        zero.z = num2 * this.gridSpacingAtLevel;
        if ((zero != this.lastPosition) && !this.probabilityUpdateQueued)
        {
            if (Application.isPlaying)
            {
                this.probabilityUpdateQueued = true;
            }
            else
            {
                this.UpdateMapsNow(zero);
            }
        }
        this.lastPosition = zero;
    }

    private void UpdateMapsNow(Vector3 gridPosition)
    {
        Terrain activeTerrain = Terrain.activeTerrain;
        if (activeTerrain != null)
        {
            this.probabilityGenerator.UpdateMap(activeTerrain.transform.InverseTransformPoint(gridPosition));
            this.levelMaterial.SetTexture("_TextureIndexTex", this.probabilityGenerator.probabilityTexture);
            this.levelMaterial.SetVector("_TerrainPosition", activeTerrain.transform.position);
        }
    }

    private static class Constant
    {
        public static readonly Quaternion rotation = Quaternion.identity;
    }
}

