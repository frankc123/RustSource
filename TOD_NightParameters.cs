using System;
using UnityEngine;

[Serializable]
public class TOD_NightParameters
{
    public Color AdditiveColor = Color.black;
    public float AmbientIntensity = 0.2f;
    public float CloudMultiplier = 0.2f;
    public Color MoonHaloColor = ((Color) new Color32(0x51, 0x68, 0x9b, 0xff));
    public Color MoonLightColor = ((Color) new Color32(0xb5, 0xcc, 0xff, 0xff));
    public float MoonLightIntensity = 0.1f;
    public Color MoonMeshColor = ((Color) new Color32(0xff, 0xe9, 200, 0xff));
    public float MoonMeshSize = 1f;
    public float ShadowStrength = 1f;
    public float SkyMultiplier = 0.1f;

    public void CheckRange()
    {
        this.MoonLightIntensity = Mathf.Max(0f, this.MoonLightIntensity);
        this.MoonMeshSize = Mathf.Max(0f, this.MoonMeshSize);
        this.AmbientIntensity = Mathf.Clamp01(this.AmbientIntensity);
        this.ShadowStrength = Mathf.Clamp01(this.ShadowStrength);
        this.SkyMultiplier = Mathf.Clamp01(this.SkyMultiplier);
        this.CloudMultiplier = Mathf.Clamp01(this.CloudMultiplier);
    }
}

