using System;
using UnityEngine;

[ExecuteInEditMode]
public class WaterColor : MonoBehaviour
{
    public float colorLerp = 0.5f;
    public Color colorMain = Color.green;
    private TOD_Sky sky;
    private WaterBase water;

    private void Start()
    {
        this.sky = (TOD_Sky) Object.FindObjectOfType(typeof(TOD_Sky));
        this.water = base.GetComponent<WaterBase>();
    }

    private void Update()
    {
        if ((this.sky != null) && (this.water != null))
        {
            Color a = Color.Lerp(this.sky.FogColor, this.sky.AmbientColor, 0.5f);
            a.a = 1f;
            Color color = (Color) (Color.Lerp(a, this.colorMain, this.colorLerp) * 0.8f);
            color.a = 0.1f;
            Color color3 = (Color) (color * 0.8f);
            color3.a = 0.75f;
            this.water.sharedMaterial.SetColor("_ReflectionColor", color);
            this.water.sharedMaterial.SetColor("_BaseColor", color3);
        }
    }
}

