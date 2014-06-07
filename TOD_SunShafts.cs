using System;
using UnityEngine;

[AddComponentMenu("Time of Day/Camera Sun Shafts"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
internal class TOD_SunShafts : TOD_PostEffectsBase
{
    public SunShaftsBlendMode BlendMode;
    public float MaxRadius = 1f;
    private const int PASS_ADD = 4;
    private const int PASS_DEPTH = 2;
    private const int PASS_NODEPTH = 3;
    private const int PASS_RADIAL = 1;
    private const int PASS_SCREEN = 0;
    public int RadialBlurIterations = 2;
    public SunShaftsResolution Resolution = SunShaftsResolution.Normal;
    private Material screenClearMaterial;
    public Shader ScreenClearShader;
    public TOD_Sky sky;
    public float SunShaftBlurRadius = 2f;
    public float SunShaftIntensity = 1f;
    private Material sunShaftsMaterial;
    public Shader SunShaftsShader;
    public bool UseDepthTexture = true;

    protected override bool CheckResources()
    {
        base.CheckSupport(this.UseDepthTexture);
        this.sunShaftsMaterial = base.CheckShaderAndCreateMaterial(this.SunShaftsShader, this.sunShaftsMaterial);
        this.screenClearMaterial = base.CheckShaderAndCreateMaterial(this.ScreenClearShader, this.screenClearMaterial);
        if (!base.isSupported)
        {
            base.ReportAutoDisable();
        }
        return base.isSupported;
    }

    protected void OnDisable()
    {
        if (this.sunShaftsMaterial != null)
        {
            Object.DestroyImmediate(this.sunShaftsMaterial);
        }
        if (this.screenClearMaterial != null)
        {
            Object.DestroyImmediate(this.screenClearMaterial);
        }
    }

    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!this.CheckResources() || (this.sky == null))
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            int width;
            int height;
            this.sky.Components.SunShafts = this;
            if (this.UseDepthTexture)
            {
                Camera camera = base.camera;
                camera.depthTextureMode |= DepthTextureMode.Depth;
            }
            if (this.Resolution == SunShaftsResolution.High)
            {
                width = source.width;
                height = source.height;
            }
            else if (this.Resolution == SunShaftsResolution.Normal)
            {
                width = source.width / 2;
                height = source.height / 2;
            }
            else
            {
                width = source.width / 4;
                height = source.height / 4;
            }
            Vector3 vector = base.camera.WorldToViewportPoint(this.sky.Components.SunTransform.position);
            this.sunShaftsMaterial.SetVector("_BlurRadius4", (Vector4) (new Vector4(1f, 1f, 0f, 0f) * this.SunShaftBlurRadius));
            this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.MaxRadius));
            RenderTexture dest = RenderTexture.GetTemporary(width, height, 0);
            RenderTexture texture2 = RenderTexture.GetTemporary(width, height, 0);
            if (this.UseDepthTexture)
            {
                Graphics.Blit(source, dest, this.sunShaftsMaterial, 2);
            }
            else
            {
                Graphics.Blit(source, dest, this.sunShaftsMaterial, 3);
            }
            base.DrawBorder(dest, this.screenClearMaterial);
            float x = this.SunShaftBlurRadius * 0.001302083f;
            this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(x, x, 0f, 0f));
            this.sunShaftsMaterial.SetVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, this.MaxRadius));
            for (int i = 0; i < this.RadialBlurIterations; i++)
            {
                Graphics.Blit(dest, texture2, this.sunShaftsMaterial, 1);
                x = (this.SunShaftBlurRadius * (((i * 2f) + 1f) * 6f)) / 768f;
                this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(x, x, 0f, 0f));
                Graphics.Blit(texture2, dest, this.sunShaftsMaterial, 1);
                x = (this.SunShaftBlurRadius * (((i * 2f) + 2f) * 6f)) / 768f;
                this.sunShaftsMaterial.SetVector("_BlurRadius4", new Vector4(x, x, 0f, 0f));
            }
            Vector4 vector2 = (vector.z < 0.0) ? Vector4.zero : ((Vector4) (((1f - this.sky.Atmosphere.Fogginess) * this.SunShaftIntensity) * this.sky.SunShaftColor));
            this.sunShaftsMaterial.SetVector("_SunColor", vector2);
            this.sunShaftsMaterial.SetTexture("_ColorBuffer", dest);
            if (this.BlendMode == SunShaftsBlendMode.Screen)
            {
                Graphics.Blit(source, destination, this.sunShaftsMaterial, 0);
            }
            else
            {
                Graphics.Blit(source, destination, this.sunShaftsMaterial, 4);
            }
            RenderTexture.ReleaseTemporary(dest);
            RenderTexture.ReleaseTemporary(texture2);
        }
    }

    public enum SunShaftsBlendMode
    {
        Screen,
        Add
    }

    public enum SunShaftsResolution
    {
        Low,
        Normal,
        High
    }
}

