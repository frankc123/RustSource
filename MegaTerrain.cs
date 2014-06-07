using System;
using UnityEngine;

public class MegaTerrain : MonoBehaviour
{
    public TerrainData _rootTerrainData;
    public Terrain[] _terrains;
    public string name_base = "rust_terrain";

    public Terrain FindTerrain(int x, int y)
    {
        object[] objArray1 = new object[] { this.name_base, "_x", x, "_y", y };
        string name = string.Concat(objArray1);
        return ((GameObject.Find(name) == null) ? null : GameObject.Find(name).GetComponent<Terrain>());
    }

    [ContextMenu("Generate")]
    private void Generate()
    {
        for (int i = 0; i < 0x10; i++)
        {
            for (int j = 0; j < 0x10; j++)
            {
            }
        }
    }

    private void Start()
    {
    }

    [ContextMenu("Stitch")]
    private void Stitch()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Terrain terrain = this.FindTerrain(i, j);
                if (terrain != null)
                {
                    Debug.Log("found terrain");
                    Terrain left = this.FindTerrain(i - 1, j);
                    Terrain right = this.FindTerrain(i + 1, j);
                    Terrain top = this.FindTerrain(i, j + 1);
                    Terrain bottom = this.FindTerrain(i, j - 1);
                    terrain.SetNeighbors(left, top, right, bottom);
                    if (left == null)
                    {
                    }
                }
                else
                {
                    Debug.Log(string.Concat(new object[] { "couldnt find terrain :", this.name_base, "_x", i, "_y", j }));
                }
            }
        }
    }

    private void Update()
    {
    }
}

