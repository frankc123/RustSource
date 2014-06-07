using System;
using UnityEngine;

[ExecuteInEditMode]
public class TOD_Components : MonoBehaviour
{
    internal TOD_Animation Animation;
    public GameObject Atmosphere;
    internal MeshFilter AtmosphereMeshFilter;
    internal Renderer AtmosphereRenderer;
    internal Material AtmosphereShader;
    internal Transform CameraTransform;
    public GameObject Clear;
    internal MeshFilter ClearMeshFilter;
    internal Renderer ClearRenderer;
    internal Material ClearShader;
    internal MeshFilter CloudMeshFilter;
    internal Renderer CloudRenderer;
    public GameObject Clouds;
    internal Material CloudShader;
    internal Transform DomeTransform;
    public GameObject Light;
    internal UnityEngine.Light LightSource;
    internal Transform LightTransform;
    public GameObject Moon;
    internal MeshFilter MoonMeshFilter;
    internal Renderer MoonRenderer;
    internal Material MoonShader;
    internal Transform MoonTransform;
    public GameObject Projector;
    internal TOD_Resources Resources;
    internal UnityEngine.Projector ShadowProjector;
    internal Material ShadowShader;
    internal TOD_Sky Sky;
    public GameObject Space;
    internal MeshFilter SpaceMeshFilter;
    internal Renderer SpaceRenderer;
    internal Material SpaceShader;
    public GameObject Sun;
    internal MeshFilter SunMeshFilter;
    internal Renderer SunRenderer;
    internal Material SunShader;
    internal TOD_SunShafts SunShafts;
    internal Transform SunTransform;
    internal TOD_Time Time;
    internal TOD_Weather Weather;

    protected void OnEnable()
    {
        this.DomeTransform = base.transform;
        if (Camera.main != null)
        {
            this.CameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("Main camera does not exist or is not tagged 'MainCamera'.");
        }
        this.Sky = base.GetComponent<TOD_Sky>();
        this.Animation = base.GetComponent<TOD_Animation>();
        this.Time = base.GetComponent<TOD_Time>();
        this.Weather = base.GetComponent<TOD_Weather>();
        this.Resources = base.GetComponent<TOD_Resources>();
        if (this.Space != null)
        {
            this.SpaceRenderer = this.Space.renderer;
            this.SpaceShader = this.SpaceRenderer.sharedMaterial;
            this.SpaceMeshFilter = this.Space.GetComponent<MeshFilter>();
        }
        else
        {
            Debug.LogError("Space reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
            return;
        }
        if (this.Atmosphere != null)
        {
            this.AtmosphereRenderer = this.Atmosphere.renderer;
            this.AtmosphereShader = this.AtmosphereRenderer.sharedMaterial;
            this.AtmosphereMeshFilter = this.Atmosphere.GetComponent<MeshFilter>();
        }
        else
        {
            Debug.LogError("Atmosphere reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
            return;
        }
        if (this.Clear != null)
        {
            this.ClearRenderer = this.Clear.renderer;
            this.ClearShader = this.ClearRenderer.sharedMaterial;
            this.ClearMeshFilter = this.Clear.GetComponent<MeshFilter>();
        }
        else
        {
            Debug.LogError("Clear reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
            return;
        }
        if (this.Clouds != null)
        {
            this.CloudRenderer = this.Clouds.renderer;
            this.CloudShader = this.CloudRenderer.sharedMaterial;
            this.CloudMeshFilter = this.Clouds.GetComponent<MeshFilter>();
        }
        else
        {
            Debug.LogError("Clouds reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
            return;
        }
        if (this.Projector != null)
        {
            this.ShadowProjector = this.Projector.GetComponent<UnityEngine.Projector>();
            this.ShadowShader = this.ShadowProjector.material;
        }
        else
        {
            Debug.LogError("Projector reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
            return;
        }
        if (this.Light != null)
        {
            this.LightTransform = this.Light.transform;
            this.LightSource = this.Light.light;
        }
        else
        {
            Debug.LogError("Light reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
            return;
        }
        if (this.Sun != null)
        {
            this.SunTransform = this.Sun.transform;
            this.SunRenderer = this.Sun.renderer;
            this.SunShader = this.SunRenderer.sharedMaterial;
            this.SunMeshFilter = this.Sun.GetComponent<MeshFilter>();
        }
        else
        {
            Debug.LogError("Sun reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
            return;
        }
        if (this.Moon != null)
        {
            this.MoonTransform = this.Moon.transform;
            this.MoonRenderer = this.Moon.renderer;
            this.MoonShader = this.MoonRenderer.sharedMaterial;
            this.MoonMeshFilter = this.Moon.GetComponent<MeshFilter>();
        }
        else
        {
            Debug.LogError("Moon reference not set. Disabling TOD_Sky script.");
            this.Sky.enabled = false;
        }
    }
}

