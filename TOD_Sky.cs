using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class TOD_Sky : MonoBehaviour
{
    public TOD_AtmosphereParameters Atmosphere;
    private Vector3 betaMie;
    private Vector2 betaMiePhase;
    private Vector3 betaMieTheta;
    private Vector3 betaNight;
    private Vector3 betaRayleigh;
    private Vector3 betaRayleighTheta;
    public CloudQualityType CloudQuality = CloudQualityType.Bumped;
    public TOD_CloudParameters Clouds;
    public TOD_CycleParameters Cycle;
    public TOD_DayParameters Day;
    public TOD_LightParameters Light;
    public MeshQualityType MeshQuality = MeshQualityType.High;
    public TOD_NightParameters Night;
    private Vector3 oneOverBeta;
    private Vector2 opticalDepth;
    private const float pi = 3.141593f;
    private const float pi2 = 9.869605f;
    private const float pi3 = 31.00628f;
    private const float pi4 = 97.4091f;
    public TOD_StarParameters Stars;
    public ColorSpaceDetection UnityColorSpace;
    public TOD_WorldParameters World;

    private Vector3 Inverse(Vector3 v)
    {
        return new Vector3(1f / v.x, 1f / v.y, 1f / v.z);
    }

    private float Max3(float a, float b, float c)
    {
        return (((a < b) || (a < c)) ? ((b < c) ? c : b) : a);
    }

    protected void OnEnable()
    {
        this.Components = base.GetComponent<TOD_Components>();
        if (this.Components == null)
        {
            Debug.LogError("TOD_Components not found. Disabling script.");
            base.enabled = false;
        }
    }

    internal Vector3 OrbitalToLocal(float theta, float phi)
    {
        Vector3 vector;
        float num = Mathf.Sin(theta);
        float num2 = Mathf.Cos(theta);
        float num3 = Mathf.Sin(phi);
        float num4 = Mathf.Cos(phi);
        vector.z = num * num4;
        vector.y = num2;
        vector.x = num * num3;
        return vector;
    }

    internal Vector3 OrbitalToUnity(float radius, float theta, float phi)
    {
        Vector3 vector;
        float num = Mathf.Sin(theta);
        float num2 = Mathf.Cos(theta);
        float num3 = Mathf.Sin(phi);
        float num4 = Mathf.Cos(phi);
        vector.z = (radius * num) * num4;
        vector.y = radius * num2;
        vector.x = (radius * num) * num3;
        return vector;
    }

    private Color PowRGB(Color c, float p)
    {
        return new Color(Mathf.Pow(c.r, p), Mathf.Pow(c.g, p), Mathf.Pow(c.b, p), c.a);
    }

    private Color PowRGBA(Color c, float p)
    {
        return new Color(Mathf.Pow(c.r, p), Mathf.Pow(c.g, p), Mathf.Pow(c.b, p), Mathf.Pow(c.a, p));
    }

    internal Color SampleAtmosphere(Vector3 direction, bool clampAlpha = true)
    {
        direction = this.Components.DomeTransform.InverseTransformDirection(direction);
        float horizonOffset = this.World.HorizonOffset;
        float p = this.Atmosphere.Contrast * 0.4545454f;
        float haziness = this.Atmosphere.Haziness;
        float fogginess = this.Atmosphere.Fogginess;
        Color sunColor = this.SunColor;
        Color moonColor = this.MoonColor;
        Color moonHaloColor = this.MoonHaloColor;
        Color cloudColor = this.CloudColor;
        Color additiveColor = this.AdditiveColor;
        Vector3 rhs = this.Components.DomeTransform.InverseTransformDirection(this.SunDirection);
        Vector3 lhs = this.Components.DomeTransform.InverseTransformDirection(this.MoonDirection);
        Vector3 opticalDepth = (Vector3) this.opticalDepth;
        Vector3 oneOverBeta = this.oneOverBeta;
        Vector3 betaRayleigh = this.betaRayleigh;
        Vector3 betaRayleighTheta = this.betaRayleighTheta;
        Vector3 betaMie = this.betaMie;
        Vector3 betaMieTheta = this.betaMieTheta;
        Vector3 betaMiePhase = (Vector3) this.betaMiePhase;
        Vector3 betaNight = this.betaNight;
        Color black = Color.black;
        float num7 = Mathf.Max(0f, Vector3.Dot(-direction, rhs));
        float num9 = Mathf.Pow(Mathf.Clamp((float) (direction.y + horizonOffset), (float) 0.001f, (float) 1f), haziness);
        float num10 = (1f - num9) * 190000f;
        float num11 = num10 + (num9 * (opticalDepth.x - num10));
        float num12 = num10 + (num9 * (opticalDepth.y - num10));
        float num13 = 1f + (num7 * num7);
        Vector3 vector11 = (Vector3) ((betaRayleigh * num11) + (betaMie * num12));
        Vector3 vector12 = betaRayleighTheta + ((Vector3) (betaMieTheta / Mathf.Pow(betaMiePhase.x - (betaMiePhase.y * num7), 1.5f)));
        float r = sunColor.r;
        float g = sunColor.g;
        float b = sunColor.b;
        float num17 = moonColor.r;
        float num18 = moonColor.g;
        float num19 = moonColor.b;
        float num20 = Mathf.Exp(-vector11.x);
        float num21 = Mathf.Exp(-vector11.y);
        float num22 = Mathf.Exp(-vector11.z);
        float num23 = (num13 * vector12.x) * oneOverBeta.x;
        float num24 = (num13 * vector12.y) * oneOverBeta.y;
        float num25 = (num13 * vector12.z) * oneOverBeta.z;
        float x = betaNight.x;
        float y = betaNight.y;
        float z = betaNight.z;
        black.r = (1f - num20) * ((r * num23) + (num17 * x));
        black.g = (1f - num21) * ((g * num24) + (num18 * y));
        black.b = (1f - num22) * ((b * num25) + (num19 * z));
        black.a = 10f * this.Max3(black.r, black.g, black.b);
        black += (Color) (moonHaloColor * Mathf.Pow(Mathf.Max(0f, Vector3.Dot(lhs, -direction)), 10f));
        black += additiveColor;
        black.r = Mathf.Lerp(black.r, cloudColor.r, fogginess);
        black.g = Mathf.Lerp(black.g, cloudColor.g, fogginess);
        black.b = Mathf.Lerp(black.b, cloudColor.b, fogginess);
        black.a += fogginess;
        if (clampAlpha)
        {
            black.a = Mathf.Clamp01(black.a);
        }
        return this.PowRGBA(black, p);
    }

    private Color SampleFogColor()
    {
        Vector3 vector = (this.Components.CameraTransform == null) ? Vector3.forward : this.Components.CameraTransform.forward;
        Vector3 direction = Vector3.Lerp(new Vector3(vector.x, 0f, vector.z), Vector3.up, this.World.FogColorBias);
        Color color = this.SampleAtmosphere(direction, true);
        return new Color(color.a * color.r, color.a * color.g, color.a * color.b, 1f);
    }

    private void SetupLightSource(float theta, float phi)
    {
        float num25;
        float shadowStrength;
        Vector3 vector;
        float num = Mathf.Cos((Mathf.Pow(theta / 6.283185f, 2f - this.Light.Falloff) * 2f) * 3.141593f);
        float num2 = Mathf.Sqrt((((501264f * num) * num) + 1416f) + 1f) - (708f * num);
        float r = this.Day.SunLightColor.r;
        float g = this.Day.SunLightColor.g;
        float b = this.Day.SunLightColor.b;
        float a = this.Components.LightSource.intensity / Mathf.Max(this.Day.SunLightIntensity, this.Night.MoonLightIntensity);
        r *= Mathf.Exp(-0.008735f * Mathf.Pow(0.68f, -4.08f * num2));
        g *= Mathf.Exp(-0.008735f * Mathf.Pow(0.55f, -4.08f * num2));
        b *= Mathf.Exp(-0.008735f * Mathf.Pow(0.44f, -4.08f * num2));
        this.LerpValue = Mathf.Clamp01(1.1f * this.Max3(r, g, b));
        float num12 = this.Night.MoonLightColor.r;
        float num13 = this.Night.MoonLightColor.g;
        float num14 = this.Night.MoonLightColor.b;
        float num15 = this.Day.SunLightColor.r * Mathf.Lerp(1f, r, this.Light.Coloring);
        float num16 = this.Day.SunLightColor.g * Mathf.Lerp(1f, g, this.Light.Coloring);
        float num17 = this.Day.SunLightColor.b * Mathf.Lerp(1f, b, this.Light.Coloring);
        float num18 = this.Day.SunShaftColor.r * Mathf.Lerp(1f, r, this.Light.ShaftColoring);
        float num19 = this.Day.SunShaftColor.g * Mathf.Lerp(1f, g, this.Light.ShaftColoring);
        float num20 = this.Day.SunShaftColor.b * Mathf.Lerp(1f, b, this.Light.ShaftColoring);
        Color color = new Color(num12, num13, num14, a);
        Color color2 = new Color(num15, num16, num17, a);
        Color color3 = Color.Lerp(color, color2, this.Max3(color2.r, color2.g, color2.b));
        this.SunShaftColor = new Color(num18, num19, num20, a);
        float num21 = Mathf.Lerp(this.Night.AmbientIntensity, this.Day.AmbientIntensity, this.LerpValue);
        this.AmbientColor = new Color(color3.r * num21, color3.g * num21, color3.b * num21, 1f);
        this.SunColor = (Color) (((this.Atmosphere.Brightness * this.Day.SkyMultiplier) * Mathf.Lerp(1f, 0.1f, Mathf.Sqrt(this.SunZenith / 90f) - 0.25f)) * Color.Lerp((Color) (this.Day.SunLightColor * this.LerpValue), new Color(r, g, b, a), this.Light.SkyColoring));
        this.SunColor = new Color(this.SunColor.r, this.SunColor.g, this.SunColor.b, this.LerpValue);
        this.MoonColor = (Color) (((((1f - this.LerpValue) * 0.5f) * this.Atmosphere.Brightness) * this.Night.SkyMultiplier) * this.Night.MoonLightColor);
        this.MoonColor = new Color(this.MoonColor.r, this.MoonColor.g, this.MoonColor.b, 1f - this.LerpValue);
        Color color4 = (Color) ((((1f - this.LerpValue) * (1f - Mathf.Abs(this.Cycle.MoonPhase))) * this.Atmosphere.Brightness) * this.Night.MoonHaloColor);
        color4.r *= color4.a;
        color4.g *= color4.a;
        color4.b *= color4.a;
        color4.a = this.Max3(color4.r, color4.g, color4.b);
        this.MoonHaloColor = color4;
        Color color5 = Color.Lerp(this.MoonColor, this.SunColor, this.LerpValue);
        float num22 = Mathf.Lerp(this.Night.CloudMultiplier, this.Day.CloudMultiplier, this.LerpValue);
        float num23 = ((color5.r + color5.g) + color5.b) / 3f;
        this.CloudColor = (Color) (((num22 * 1.25f) * this.Clouds.Brightness) * Color.Lerp(new Color(num23, num23, num23), color5, this.Light.CloudColoring));
        this.CloudColor = new Color(this.CloudColor.r, this.CloudColor.g, this.CloudColor.b, num22);
        Color color6 = Color.Lerp(this.Night.AdditiveColor, this.Day.AdditiveColor, this.LerpValue);
        color6.r *= color6.a;
        color6.g *= color6.a;
        color6.b *= color6.a;
        color6.a = this.Max3(color6.r, color6.g, color6.b);
        this.AdditiveColor = color6;
        if (this.LerpValue > 0.2f)
        {
            float t = (this.LerpValue - 0.2f) / 0.8f;
            num25 = Mathf.Lerp(0f, this.Day.SunLightIntensity, t);
            shadowStrength = this.Day.ShadowStrength;
            vector = this.OrbitalToLocal(Mathf.Min(theta, ((1f - this.Light.MinimumHeight) * 3.141593f) / 2f), phi);
            this.Components.LightSource.color = color2;
        }
        else
        {
            float num28 = (0.2f - this.LerpValue) / 0.2f;
            float num29 = 1f - Mathf.Abs(this.Cycle.MoonPhase);
            num25 = Mathf.Lerp(0f, this.Night.MoonLightIntensity * num29, num28);
            shadowStrength = this.Night.ShadowStrength;
            vector = this.OrbitalToLocal(Mathf.Max((float) (theta + 3.141593f), (float) ((((1f + this.Light.MinimumHeight) * 3.141593f) / 2f) + 3.141593f)), phi);
            this.Components.LightSource.color = color;
        }
        LightShadows shadows = (this.Components.LightSource.shadowStrength != 0f) ? LightShadows.Soft : LightShadows.None;
        this.Components.LightSource.intensity = num25;
        this.Components.LightSource.shadowStrength = shadowStrength;
        this.Components.LightTransform.localPosition = vector;
        this.Components.LightTransform.LookAt(this.Components.DomeTransform.position);
        this.Components.LightSource.shadows = shadows;
    }

    private void SetupQualitySettings()
    {
        Material spaceMaterial;
        TOD_Resources resources = this.Components.Resources;
        Material cloudMaterialFastest = null;
        Material shadowMaterialFastest = null;
        switch (this.CloudQuality)
        {
            case CloudQualityType.Fastest:
                cloudMaterialFastest = resources.CloudMaterialFastest;
                shadowMaterialFastest = resources.ShadowMaterialFastest;
                break;

            case CloudQualityType.Density:
                cloudMaterialFastest = resources.CloudMaterialDensity;
                shadowMaterialFastest = resources.ShadowMaterialDensity;
                break;

            case CloudQualityType.Bumped:
                cloudMaterialFastest = resources.CloudMaterialBumped;
                shadowMaterialFastest = resources.ShadowMaterialBumped;
                break;

            default:
                Debug.LogError("Unknown cloud quality.");
                break;
        }
        Mesh icosphereLow = null;
        Mesh icosphereMedium = null;
        Mesh mesh3 = null;
        Mesh halfIcosphereLow = null;
        Mesh quad = null;
        Mesh sphereLow = null;
        switch (this.MeshQuality)
        {
            case MeshQualityType.Low:
                icosphereLow = resources.IcosphereLow;
                icosphereMedium = resources.IcosphereLow;
                mesh3 = resources.IcosphereLow;
                halfIcosphereLow = resources.HalfIcosphereLow;
                quad = resources.Quad;
                sphereLow = resources.SphereLow;
                break;

            case MeshQualityType.Medium:
                icosphereLow = resources.IcosphereMedium;
                icosphereMedium = resources.IcosphereMedium;
                mesh3 = resources.IcosphereLow;
                halfIcosphereLow = resources.HalfIcosphereMedium;
                quad = resources.Quad;
                sphereLow = resources.SphereMedium;
                break;

            case MeshQualityType.High:
                icosphereLow = resources.IcosphereHigh;
                icosphereMedium = resources.IcosphereHigh;
                mesh3 = resources.IcosphereLow;
                halfIcosphereLow = resources.HalfIcosphereHigh;
                quad = resources.Quad;
                sphereLow = resources.SphereHigh;
                break;

            default:
                Debug.LogError("Unknown mesh quality.");
                break;
        }
        if ((this.Components.SpaceShader == null) || (this.Components.SpaceShader.name != resources.SpaceMaterial.name))
        {
            spaceMaterial = resources.SpaceMaterial;
            this.Components.SpaceRenderer.sharedMaterial = spaceMaterial;
            this.Components.SpaceShader = spaceMaterial;
        }
        if ((this.Components.AtmosphereShader == null) || (this.Components.AtmosphereShader.name != resources.AtmosphereMaterial.name))
        {
            spaceMaterial = resources.AtmosphereMaterial;
            this.Components.AtmosphereRenderer.sharedMaterial = spaceMaterial;
            this.Components.AtmosphereShader = spaceMaterial;
        }
        if ((this.Components.ClearShader == null) || (this.Components.ClearShader.name != resources.ClearMaterial.name))
        {
            spaceMaterial = resources.ClearMaterial;
            this.Components.ClearRenderer.sharedMaterial = spaceMaterial;
            this.Components.ClearShader = spaceMaterial;
        }
        if ((this.Components.CloudShader == null) || (this.Components.CloudShader.name != cloudMaterialFastest.name))
        {
            spaceMaterial = cloudMaterialFastest;
            this.Components.CloudRenderer.sharedMaterial = spaceMaterial;
            this.Components.CloudShader = spaceMaterial;
        }
        if ((this.Components.ShadowShader == null) || (this.Components.ShadowShader.name != shadowMaterialFastest.name))
        {
            spaceMaterial = shadowMaterialFastest;
            this.Components.ShadowProjector.material = spaceMaterial;
            this.Components.ShadowShader = spaceMaterial;
        }
        if ((this.Components.SunShader == null) || (this.Components.SunShader.name != resources.SunMaterial.name))
        {
            spaceMaterial = resources.SunMaterial;
            this.Components.SunRenderer.sharedMaterial = spaceMaterial;
            this.Components.SunShader = spaceMaterial;
        }
        if ((this.Components.MoonShader == null) || (this.Components.MoonShader.name != resources.MoonMaterial.name))
        {
            spaceMaterial = resources.MoonMaterial;
            this.Components.MoonRenderer.sharedMaterial = spaceMaterial;
            this.Components.MoonShader = spaceMaterial;
        }
        if (this.Components.SpaceMeshFilter.sharedMesh != icosphereLow)
        {
            this.Components.SpaceMeshFilter.mesh = icosphereLow;
        }
        if (this.Components.AtmosphereMeshFilter.sharedMesh != icosphereMedium)
        {
            this.Components.AtmosphereMeshFilter.mesh = icosphereMedium;
        }
        if (this.Components.ClearMeshFilter.sharedMesh != mesh3)
        {
            this.Components.ClearMeshFilter.mesh = mesh3;
        }
        if (this.Components.CloudMeshFilter.sharedMesh != halfIcosphereLow)
        {
            this.Components.CloudMeshFilter.mesh = halfIcosphereLow;
        }
        if (this.Components.SunMeshFilter.sharedMesh != quad)
        {
            this.Components.SunMeshFilter.mesh = quad;
        }
        if (this.Components.MoonMeshFilter.sharedMesh != sphereLow)
        {
            this.Components.MoonMeshFilter.mesh = sphereLow;
        }
    }

    private void SetupScattering()
    {
        float num5 = 0.001f + (this.Atmosphere.RayleighMultiplier * this.Atmosphere.ScatteringColor.r);
        float num6 = 0.001f + (this.Atmosphere.RayleighMultiplier * this.Atmosphere.ScatteringColor.g);
        float num7 = 0.001f + (this.Atmosphere.RayleighMultiplier * this.Atmosphere.ScatteringColor.b);
        this.betaRayleigh.x = 5.8E-06f * num5;
        this.betaRayleigh.y = 1.35E-05f * num6;
        this.betaRayleigh.z = 3.31E-05f * num7;
        this.betaRayleighTheta.x = (0.000116f * num5) * 0.0596831f;
        this.betaRayleighTheta.y = (0.00027f * num6) * 0.0596831f;
        this.betaRayleighTheta.z = (0.000662f * num7) * 0.0596831f;
        this.opticalDepth.x = 8000f * Mathf.Exp((-this.World.ViewerHeight * 50000f) / 8000f);
        float num12 = 0.001f + (this.Atmosphere.MieMultiplier * this.Atmosphere.ScatteringColor.r);
        float num13 = 0.001f + (this.Atmosphere.MieMultiplier * this.Atmosphere.ScatteringColor.g);
        float num14 = 0.001f + (this.Atmosphere.MieMultiplier * this.Atmosphere.ScatteringColor.b);
        float directionality = this.Atmosphere.Directionality;
        float num17 = (0.2387324f * (1f - (directionality * directionality))) / (2f + (directionality * directionality));
        this.betaMie.x = 2E-06f * num12;
        this.betaMie.y = 2E-06f * num13;
        this.betaMie.z = 2E-06f * num14;
        this.betaMieTheta.x = (4E-05f * num12) * num17;
        this.betaMieTheta.y = (4E-05f * num13) * num17;
        this.betaMieTheta.z = (4E-05f * num14) * num17;
        this.betaMiePhase.x = 1f + (directionality * directionality);
        this.betaMiePhase.y = 2f * directionality;
        this.opticalDepth.y = 1200f * Mathf.Exp((-this.World.ViewerHeight * 50000f) / 1200f);
        this.oneOverBeta = this.Inverse(this.betaMie + this.betaRayleigh);
        this.betaNight = Vector3.Scale(this.betaRayleighTheta + ((Vector3) (this.betaMieTheta / Mathf.Pow(this.betaMiePhase.x, 1.5f))), this.oneOverBeta);
    }

    private void SetupSunAndMoon()
    {
        float f = 0.01745329f * this.Cycle.Latitude;
        float num2 = Mathf.Sin(f);
        float num3 = Mathf.Cos(f);
        float longitude = this.Cycle.Longitude;
        float num5 = ((((0x16f * this.Cycle.Year) - ((7 * (this.Cycle.Year + ((this.Cycle.Month + 9) / 12))) / 4)) + ((0x113 * this.Cycle.Month) / 9)) + this.Cycle.Day) - 0xb25a2;
        float num6 = this.Cycle.Hour - this.Cycle.UTC;
        float num7 = 23.4393f - (3.563E-07f * num5);
        float num8 = 0.01745329f * num7;
        float num9 = Mathf.Sin(num8);
        float num10 = Mathf.Cos(num8);
        float num15 = 282.9404f + (4.70935E-05f * num5);
        float num16 = 0.016709f - (1.151E-09f * num5);
        float num17 = 356.047f + (0.9856002f * num5);
        float num18 = 0.01745329f * num17;
        float num19 = Mathf.Sin(num18);
        float num20 = Mathf.Cos(num18);
        float num21 = num17 + (((num16 * 57.29578f) * num19) * (1f + (num16 * num20)));
        float num22 = 0.01745329f * num21;
        float num23 = Mathf.Sin(num22);
        float x = Mathf.Cos(num22) - num16;
        float y = num23 * Mathf.Sqrt(1f - (num16 * num16));
        float num27 = 57.29578f * Mathf.Atan2(y, x);
        float num28 = Mathf.Sqrt((x * x) + (y * y));
        float num29 = num27 + num15;
        float num30 = 0.01745329f * num29;
        float num31 = Mathf.Sin(num30);
        float num32 = Mathf.Cos(num30);
        float num33 = num28 * num32;
        float num34 = num28 * num31;
        float num35 = num33;
        float num36 = num34 * num10;
        float num37 = num34 * num9;
        float num38 = Mathf.Atan2(num36, num35);
        float num39 = 57.29578f * num38;
        float num40 = Mathf.Atan2(num37, Mathf.Sqrt((num35 * num35) + (num36 * num36)));
        float num41 = Mathf.Sin(num40);
        float num42 = Mathf.Cos(num40);
        float num43 = (num27 + num15) + 180f;
        float num44 = num43 + (num6 * 15f);
        float num45 = num44 + longitude;
        float num46 = num45 - num39;
        float num47 = 0.01745329f * num46;
        float num48 = Mathf.Sin(num47);
        float num50 = Mathf.Cos(num47) * num42;
        float num51 = num48 * num42;
        float num52 = num41;
        float num53 = (num50 * num2) - (num52 * num3);
        float num54 = num51;
        float num55 = (num50 * num3) + (num52 * num2);
        float num12 = Mathf.Atan2(num54, num53) + 3.141593f;
        float num11 = Mathf.Atan2(num55, Mathf.Sqrt((num53 * num53) + (num54 * num54)));
        float theta = 1.570796f - num11;
        float phi = num12;
        Vector3 vector = this.OrbitalToLocal(theta, phi);
        this.Components.SunTransform.localPosition = vector;
        this.Components.SunTransform.LookAt(this.Components.DomeTransform.position, this.Components.SunTransform.up);
        if (this.Components.CameraTransform != null)
        {
            Vector3 eulerAngles = this.Components.CameraTransform.rotation.eulerAngles;
            Vector3 localEulerAngles = this.Components.SunTransform.localEulerAngles;
            localEulerAngles.z = (((2f * Time.time) + Mathf.Abs(eulerAngles.x)) + Mathf.Abs(eulerAngles.y)) + Mathf.Abs(eulerAngles.z);
            this.Components.SunTransform.localEulerAngles = localEulerAngles;
        }
        Vector3 vector4 = this.OrbitalToLocal(theta + 3.141593f, phi);
        this.Components.MoonTransform.localPosition = vector4;
        this.Components.MoonTransform.LookAt(this.Components.DomeTransform.position, this.Components.MoonTransform.up);
        float num56 = 4f * Mathf.Tan(0.008726646f * this.Day.SunMeshSize);
        float num57 = 2f * num56;
        Vector3 vector5 = new Vector3(num57, num57, num57);
        this.Components.SunTransform.localScale = vector5;
        float num58 = 2f * Mathf.Tan(0.008726646f * this.Night.MoonMeshSize);
        float num59 = 2f * num58;
        Vector3 vector6 = new Vector3(num59, num59, num59);
        this.Components.MoonTransform.localScale = vector6;
        this.SunZenith = 57.29578f * theta;
        this.MoonZenith = Mathf.PingPong(this.SunZenith + 180f, 180f);
        bool flag = this.Components.SunTransform.localPosition.y > -0.5f;
        bool flag2 = this.Components.MoonTransform.localPosition.y > -0.1f;
        bool flag3 = this.SampleAtmosphere(Vector3.up, false).a < 1.1f;
        bool flag4 = this.Clouds.Density > 0f;
        this.Components.SunRenderer.enabled = flag;
        this.Components.MoonRenderer.enabled = flag2;
        this.Components.SpaceRenderer.enabled = flag3;
        this.Components.CloudRenderer.enabled = flag4;
        this.SetupLightSource(theta, phi);
    }

    protected void Update()
    {
        if ((this.Components.SunShafts != null) && this.Components.SunShafts.enabled)
        {
            if (!this.Components.ClearRenderer.enabled)
            {
                this.Components.ClearRenderer.enabled = true;
            }
        }
        else if (this.Components.ClearRenderer.enabled)
        {
            this.Components.ClearRenderer.enabled = false;
        }
        this.Cycle.CheckRange();
        this.SetupQualitySettings();
        this.SetupSunAndMoon();
        this.SetupScattering();
        if (this.World.SetFogColor)
        {
            RenderSettings.fogColor = this.SampleFogColor();
        }
        if (this.World.SetAmbientLight)
        {
            RenderSettings.ambientLight = this.AmbientColor;
        }
        Vector4 vector = this.Components.Animation.CloudUV + this.Components.Animation.OffsetUV;
        Shader.SetGlobalFloat("TOD_Gamma", this.Gamma);
        Shader.SetGlobalFloat("TOD_OneOverGamma", this.OneOverGamma);
        Shader.SetGlobalColor("TOD_LightColor", this.LightColor);
        Shader.SetGlobalColor("TOD_CloudColor", this.CloudColor);
        Shader.SetGlobalColor("TOD_SunColor", this.SunColor);
        Shader.SetGlobalColor("TOD_MoonColor", this.MoonColor);
        Shader.SetGlobalColor("TOD_AdditiveColor", this.AdditiveColor);
        Shader.SetGlobalColor("TOD_MoonHaloColor", this.MoonHaloColor);
        Shader.SetGlobalVector("TOD_SunDirection", this.SunDirection);
        Shader.SetGlobalVector("TOD_MoonDirection", this.MoonDirection);
        Shader.SetGlobalVector("TOD_LightDirection", this.LightDirection);
        Shader.SetGlobalVector("TOD_LocalSunDirection", this.Components.DomeTransform.InverseTransformDirection(this.SunDirection));
        Shader.SetGlobalVector("TOD_LocalMoonDirection", this.Components.DomeTransform.InverseTransformDirection(this.MoonDirection));
        Shader.SetGlobalVector("TOD_LocalLightDirection", this.Components.DomeTransform.InverseTransformDirection(this.LightDirection));
        if (this.Components.AtmosphereShader != null)
        {
            this.Components.AtmosphereShader.SetFloat("_Contrast", this.Atmosphere.Contrast * this.OneOverGamma);
            this.Components.AtmosphereShader.SetFloat("_Haziness", this.Atmosphere.Haziness);
            this.Components.AtmosphereShader.SetFloat("_Fogginess", this.Atmosphere.Fogginess);
            this.Components.AtmosphereShader.SetFloat("_Horizon", this.World.HorizonOffset);
            this.Components.AtmosphereShader.SetVector("_OpticalDepth", this.opticalDepth);
            this.Components.AtmosphereShader.SetVector("_OneOverBeta", this.oneOverBeta);
            this.Components.AtmosphereShader.SetVector("_BetaRayleigh", this.betaRayleigh);
            this.Components.AtmosphereShader.SetVector("_BetaRayleighTheta", this.betaRayleighTheta);
            this.Components.AtmosphereShader.SetVector("_BetaMie", this.betaMie);
            this.Components.AtmosphereShader.SetVector("_BetaMieTheta", this.betaMieTheta);
            this.Components.AtmosphereShader.SetVector("_BetaMiePhase", this.betaMiePhase);
            this.Components.AtmosphereShader.SetVector("_BetaNight", this.betaNight);
        }
        if (this.Components.CloudShader != null)
        {
            float num = (1f - this.Atmosphere.Fogginess) * this.LerpValue;
            float num2 = ((1f - this.Atmosphere.Fogginess) * 0.6f) * (1f - Mathf.Abs(this.Cycle.MoonPhase));
            this.Components.CloudShader.SetFloat("_SunGlow", num);
            this.Components.CloudShader.SetFloat("_MoonGlow", num2);
            this.Components.CloudShader.SetFloat("_CloudDensity", this.Clouds.Density);
            this.Components.CloudShader.SetFloat("_CloudSharpness", this.Clouds.Sharpness);
            this.Components.CloudShader.SetVector("_CloudScale1", this.Clouds.Scale1);
            this.Components.CloudShader.SetVector("_CloudScale2", this.Clouds.Scale2);
            this.Components.CloudShader.SetVector("_CloudUV", vector);
        }
        if (this.Components.SpaceShader != null)
        {
            Vector2 vector2 = new Vector2(this.Stars.Tiling, this.Stars.Tiling);
            this.Components.SpaceShader.mainTextureScale = vector2;
            this.Components.SpaceShader.SetFloat("_Subtract", 1f - Mathf.Pow(this.Stars.Density, 0.1f));
        }
        if (this.Components.SunShader != null)
        {
            this.Components.SunShader.SetColor("_Color", (Color) ((this.Day.SunMeshColor * this.LerpValue) * (1f - this.Atmosphere.Fogginess)));
        }
        if (this.Components.MoonShader != null)
        {
            this.Components.MoonShader.SetColor("_Color", this.Night.MoonMeshColor);
            this.Components.MoonShader.SetFloat("_Phase", this.Cycle.MoonPhase);
        }
        if (this.Components.ShadowShader != null)
        {
            float num3 = this.Clouds.ShadowStrength * Mathf.Clamp01(1f - (this.LightZenith / 90f));
            this.Components.ShadowShader.SetFloat("_Alpha", num3);
            this.Components.ShadowShader.SetFloat("_CloudDensity", this.Clouds.Density);
            this.Components.ShadowShader.SetFloat("_CloudSharpness", this.Clouds.Sharpness);
            this.Components.ShadowShader.SetVector("_CloudScale1", this.Clouds.Scale1);
            this.Components.ShadowShader.SetVector("_CloudScale2", this.Clouds.Scale2);
            this.Components.ShadowShader.SetVector("_CloudUV", vector);
        }
        if (this.Components.ShadowProjector != null)
        {
            bool flag = (this.Clouds.ShadowStrength != 0f) && (this.Components.ShadowShader != null);
            float num4 = this.Radius * 2f;
            float radius = this.Radius;
            this.Components.ShadowProjector.enabled = flag;
            this.Components.ShadowProjector.farClipPlane = num4;
            this.Components.ShadowProjector.orthographicSize = radius;
        }
    }

    internal Color AdditiveColor { get; private set; }

    internal Color AmbientColor { get; private set; }

    internal Color CloudColor { get; private set; }

    internal TOD_Components Components { get; private set; }

    internal Color FogColor
    {
        get
        {
            return (!this.World.SetFogColor ? this.SampleFogColor() : RenderSettings.fogColor);
        }
    }

    internal float Gamma
    {
        get
        {
            return ((((this.UnityColorSpace != ColorSpaceDetection.Auto) || (QualitySettings.activeColorSpace != ColorSpace.Linear)) && (this.UnityColorSpace != ColorSpaceDetection.Linear)) ? 2.2f : 1f);
        }
    }

    internal bool IsDay
    {
        get
        {
            return (this.LerpValue > 0f);
        }
    }

    internal bool IsNight
    {
        get
        {
            return (this.LerpValue == 0f);
        }
    }

    internal float LerpValue { get; private set; }

    internal Color LightColor
    {
        get
        {
            return this.Components.LightSource.color;
        }
    }

    internal Vector3 LightDirection
    {
        get
        {
            return Vector3.Lerp(this.MoonDirection, this.SunDirection, this.LerpValue * this.LerpValue);
        }
    }

    internal float LightIntensity
    {
        get
        {
            return this.Components.LightSource.intensity;
        }
    }

    internal float LightZenith
    {
        get
        {
            return Mathf.Min(this.SunZenith, this.MoonZenith);
        }
    }

    internal Color MoonColor { get; private set; }

    internal Vector3 MoonDirection
    {
        get
        {
            return this.Components.MoonTransform.forward;
        }
    }

    internal Color MoonHaloColor { get; private set; }

    internal float MoonZenith { get; private set; }

    internal float OneOverGamma
    {
        get
        {
            return ((((this.UnityColorSpace != ColorSpaceDetection.Auto) || (QualitySettings.activeColorSpace != ColorSpace.Linear)) && (this.UnityColorSpace != ColorSpaceDetection.Linear)) ? 0.4545454f : 1f);
        }
    }

    internal float Radius
    {
        get
        {
            return this.Components.DomeTransform.localScale.x;
        }
    }

    internal Color SunColor { get; private set; }

    internal Vector3 SunDirection
    {
        get
        {
            return this.Components.SunTransform.forward;
        }
    }

    internal Color SunShaftColor { get; private set; }

    internal float SunZenith { get; private set; }

    public enum CloudQualityType
    {
        Fastest,
        Density,
        Bumped
    }

    public enum ColorSpaceDetection
    {
        Auto,
        Linear,
        Gamma
    }

    public enum MeshQualityType
    {
        Low,
        Medium,
        High
    }
}

