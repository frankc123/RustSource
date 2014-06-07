using System;
using UnityEngine;

[Serializable]
public class TOD_DayParameters
{
    public Color AdditiveColor = Color.black;
    public float AmbientIntensity = 0.75f;
    public float CloudMultiplier = 1f;
    public float ShadowStrength = 1f;
    public float SkyMultiplier = 1f;
    public Color SunLightColor = ((Color) new Color32(0xff, 0xf3, 0xea, 0xff));
    public float SunLightIntensity = 0.75f;
    public Color SunMeshColor = ((Color) new Color32(0xff, 0xe9, 180, 0xff));
    public float SunMeshSize = 1f;
    public Color SunShaftColor = ((Color) new Color32(0xff, 0xf3, 0xea, 0xff));

    public void CheckRange()
    {
        this.SunLightIntensity = Mathf.Max(0f, this.SunLightIntensity);
        this.SunMeshSize = Mathf.Max(0f, this.SunMeshSize);
        this.AmbientIntensity = Mathf.Clamp01(this.AmbientIntensity);
        this.ShadowStrength = Mathf.Clamp01(this.ShadowStrength);
        this.SkyMultiplier = Mathf.Clamp01(this.SkyMultiplier);
        this.CloudMultiplier = Mathf.Clamp01(this.CloudMultiplier);
    }
}

