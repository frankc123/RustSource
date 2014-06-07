using System;
using UnityEngine;

public class TerrainTextureHelper
{
    public static Terrain cachedTerrain;
    public static byte[,] textures;

    public static void CacheTextures()
    {
        Debug.Log("Caching Terrain splatmap lookups, please wait...");
        Terrain activeTerrain = Terrain.activeTerrain;
        TerrainData terrainData = activeTerrain.terrainData;
        Vector3 position = activeTerrain.transform.position;
        float[,,] numArray = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        textures = new byte[numArray.GetUpperBound(0) + 1, numArray.GetUpperBound(1) + 1];
        for (int i = 0; i < terrainData.alphamapWidth; i++)
        {
            for (int j = 0; j < terrainData.alphamapHeight; j++)
            {
                float num3 = 0f;
                int num4 = 0;
                for (int k = 0; k < (numArray.GetUpperBound(2) + 1); k++)
                {
                    if (numArray[i, j, k] >= num3)
                    {
                        num4 = k;
                        num3 = numArray[i, j, k];
                    }
                }
                textures[i, j] = (byte) num4;
            }
        }
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
    }

    public static void EnsureInit()
    {
        if (cachedTerrain != Terrain.activeTerrain)
        {
            CacheTextures();
            cachedTerrain = Terrain.activeTerrain;
        }
    }

    public static float[] GetTextureAmounts(Vector3 worldPos)
    {
        return OLD_GetTextureMix(worldPos);
    }

    public static int GetTextureIndex(Vector3 worldPos)
    {
        return OLD_GetMainTexture(worldPos);
    }

    public static int OLD_GetMainTexture(Vector3 worldPos)
    {
        float[] numArray = OLD_GetTextureMix(worldPos);
        float num = 0f;
        int num2 = 0;
        for (int i = 0; i < numArray.Length; i++)
        {
            if (numArray[i] > num)
            {
                num2 = i;
                num = numArray[i];
            }
        }
        return num2;
    }

    public static float[] OLD_GetTextureMix(Vector3 worldPos)
    {
        Terrain activeTerrain = Terrain.activeTerrain;
        TerrainData terrainData = activeTerrain.terrainData;
        Vector3 position = activeTerrain.transform.position;
        int x = (int) (((worldPos.x - position.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int y = (int) (((worldPos.z - position.z) / terrainData.size.z) * terrainData.alphamapHeight);
        float[,,] numArray = terrainData.GetAlphamaps(x, y, 1, 1);
        float[] numArray2 = new float[numArray.GetUpperBound(2) + 1];
        for (int i = 0; i < numArray2.Length; i++)
        {
            numArray2[i] = numArray[0, 0, i];
        }
        return numArray2;
    }
}

