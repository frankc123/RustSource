using System;
using UnityEngine;

public class SpawnInArea : MonoBehaviour
{
    private float AboveGround = 1f;
    private float Offset = 10f;
    public Texture2D SpawnMap;
    private bool TerrainOnly = true;

    private void RandomPositionOnTerrain(GameObject obj)
    {
        Vector3 size = Terrain.activeTerrain.terrainData.size;
        Vector3 origin = new Vector3();
        bool flag = false;
        while (!flag)
        {
            origin = Terrain.activeTerrain.transform.position;
            float num = Random.Range(0f, size.x);
            float num2 = Random.Range(0f, size.z);
            origin.x += num;
            origin.y += size.y + this.Offset;
            origin.z += num2;
            if (this.SpawnMap != null)
            {
                int x = Mathf.RoundToInt((this.SpawnMap.width * num) / size.x);
                int y = Mathf.RoundToInt((this.SpawnMap.height * num2) / size.z);
                float grayscale = this.SpawnMap.GetPixel(x, y).grayscale;
                if ((grayscale > 0f) && (Random.Range((float) 0f, (float) 1f) < grayscale))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                RaycastHit hit;
                if (Physics.Raycast(origin, -Vector3.up, out hit))
                {
                    float distance = hit.distance;
                    if ((hit.transform.name != "Terrain") && this.TerrainOnly)
                    {
                        flag = false;
                    }
                    origin.y -= distance - this.AboveGround;
                }
                else
                {
                    flag = false;
                }
            }
        }
        obj.transform.position = origin;
        base.transform.Rotate(Vector3.up * Random.Range(0, 360), Space.World);
    }
}

