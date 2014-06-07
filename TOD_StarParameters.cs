using System;
using UnityEngine;

[Serializable]
public class TOD_StarParameters
{
    public float Density = 0.5f;
    public float Tiling = 2f;

    public void CheckRange()
    {
        this.Tiling = Mathf.Max(0f, this.Tiling);
        this.Density = Mathf.Clamp01(this.Density);
    }
}

