using System;
using UnityEngine;

public class TOD_Weather : MonoBehaviour
{
    private float atmosphereFog;
    private float atmosphereFogDefault;
    private float cloudBrightness;
    private float cloudBrightnessDefault;
    private float cloudDensity;
    private float cloudDensityDefault;
    public CloudType Clouds;
    private float cloudSharpness;
    public float FadeTime = 10f;
    private TOD_Sky sky;
    public WeatherType Weather;

    protected void Start()
    {
        this.sky = base.GetComponent<TOD_Sky>();
        this.cloudBrightness = this.cloudBrightnessDefault = this.sky.Clouds.Brightness;
        this.cloudDensity = this.cloudDensityDefault = this.sky.Clouds.Density;
        this.atmosphereFog = this.atmosphereFogDefault = this.sky.Atmosphere.Fogginess;
        this.cloudSharpness = this.sky.Clouds.Sharpness;
    }

    protected void Update()
    {
        if ((this.Clouds != CloudType.Custom) || (this.Weather != WeatherType.Custom))
        {
            switch (this.Clouds)
            {
                case CloudType.Custom:
                    this.cloudDensity = this.sky.Clouds.Density;
                    this.cloudSharpness = this.sky.Clouds.Sharpness;
                    break;

                case CloudType.None:
                    this.cloudDensity = 0f;
                    this.cloudSharpness = 1f;
                    break;

                case CloudType.Few:
                    this.cloudDensity = this.cloudDensityDefault;
                    this.cloudSharpness = 6f;
                    break;

                case CloudType.Scattered:
                    this.cloudDensity = this.cloudDensityDefault;
                    this.cloudSharpness = 3f;
                    break;

                case CloudType.Broken:
                    this.cloudDensity = this.cloudDensityDefault;
                    this.cloudSharpness = 1f;
                    break;

                case CloudType.Overcast:
                    this.cloudDensity = this.cloudDensityDefault;
                    this.cloudSharpness = 0.1f;
                    break;
            }
            switch (this.Weather)
            {
                case WeatherType.Custom:
                    this.cloudBrightness = this.sky.Clouds.Brightness;
                    this.atmosphereFog = this.sky.Atmosphere.Fogginess;
                    break;

                case WeatherType.Clear:
                    this.cloudBrightness = this.cloudBrightnessDefault;
                    this.atmosphereFog = this.atmosphereFogDefault;
                    break;

                case WeatherType.Storm:
                    this.cloudBrightness = 0.3f;
                    this.atmosphereFog = 1f;
                    break;

                case WeatherType.Dust:
                    this.cloudBrightness = this.cloudBrightnessDefault;
                    this.atmosphereFog = 0.5f;
                    break;

                case WeatherType.Fog:
                    this.cloudBrightness = this.cloudBrightnessDefault;
                    this.atmosphereFog = 1f;
                    break;
            }
            float t = Time.deltaTime / this.FadeTime;
            this.sky.Clouds.Brightness = Mathf.Lerp(this.sky.Clouds.Brightness, this.cloudBrightness, t);
            this.sky.Clouds.Density = Mathf.Lerp(this.sky.Clouds.Density, this.cloudDensity, t);
            this.sky.Clouds.Sharpness = Mathf.Lerp(this.sky.Clouds.Sharpness, this.cloudSharpness, t);
            this.sky.Atmosphere.Fogginess = Mathf.Lerp(this.sky.Atmosphere.Fogginess, this.atmosphereFog, t);
        }
    }

    public enum CloudType
    {
        Custom,
        None,
        Few,
        Scattered,
        Broken,
        Overcast
    }

    public enum WeatherType
    {
        Custom,
        Clear,
        Storm,
        Dust,
        Fog
    }
}

