using System;
using UnityEngine;

[Serializable]
public class TOD_AtmosphereParameters
{
    public float Brightness = 1f;
    public float Contrast = 1f;
    public float Directionality = 0.5f;
    public float Fogginess;
    public float Haziness = 0.5f;
    public float MieMultiplier = 1f;
    public float RayleighMultiplier = 1f;
    public Color ScatteringColor = Color.white;

    public void CheckRange()
    {
        this.MieMultiplier = Mathf.Max(0f, this.MieMultiplier);
        this.RayleighMultiplier = Mathf.Max(0f, this.RayleighMultiplier);
        this.Brightness = Mathf.Max(0f, this.Brightness);
        this.Contrast = Mathf.Max(0f, this.Contrast);
        this.Directionality = Mathf.Clamp01(this.Directionality);
        this.Haziness = Mathf.Clamp01(this.Haziness);
        this.Fogginess = Mathf.Clamp01(this.Fogginess);
    }
}

