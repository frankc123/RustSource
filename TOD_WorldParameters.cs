using System;
using UnityEngine;

[Serializable]
public class TOD_WorldParameters
{
    public float FogColorBias;
    public float HorizonOffset;
    public bool SetAmbientLight;
    public bool SetFogColor;
    public float ViewerHeight;

    public void CheckRange()
    {
        this.FogColorBias = Mathf.Clamp01(this.FogColorBias);
        this.ViewerHeight = Mathf.Clamp01(this.ViewerHeight);
        this.HorizonOffset = Mathf.Clamp01(this.HorizonOffset);
    }
}

