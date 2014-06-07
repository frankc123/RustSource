using System;
using UnityEngine;

public class SurfaceInfoTerrain : SurfaceInfo
{
    public SurfaceInfoObject[] surfaces;

    public override SurfaceInfoObject SurfaceObj()
    {
        return this.surfaces[0];
    }

    public override SurfaceInfoObject SurfaceObj(Vector3 worldPos)
    {
        int textureIndex = TerrainTextureHelper.GetTextureIndex(worldPos);
        if (textureIndex >= this.surfaces.Length)
        {
            Debug.Log("Missing surface info for splat index " + textureIndex);
            return this.surfaces[0];
        }
        return this.surfaces[textureIndex];
    }
}

