using System;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public abstract class TOD_PostEffectsBase : MonoBehaviour
{
    protected bool isSupported = true;

    protected TOD_PostEffectsBase()
    {
    }

    protected abstract bool CheckResources();
    protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
    {
        if (shader == null)
        {
            Debug.Log("Missing shader in " + this.ToString());
            base.enabled = false;
            return null;
        }
        if ((shader.isSupported && (material != null)) && (material.shader == shader))
        {
            return material;
        }
        if (!shader.isSupported)
        {
            this.NotSupported();
            Debug.LogError("The shader " + shader.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
            return null;
        }
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        return ((material == null) ? null : material);
    }

    protected bool CheckSupport(bool needDepth)
    {
        this.isSupported = true;
        if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
        {
            this.NotSupported();
            return false;
        }
        if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
        {
            this.NotSupported();
            return false;
        }
        if (needDepth)
        {
            Camera camera = base.camera;
            camera.depthTextureMode |= DepthTextureMode.Depth;
        }
        return true;
    }

    protected bool CheckSupport(bool needDepth, bool needHdr)
    {
        if (!this.CheckSupport(needDepth))
        {
            return false;
        }
        if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        {
            this.NotSupported();
            return false;
        }
        return true;
    }

    protected Material CreateMaterial(Shader shader, Material material)
    {
        if (shader == null)
        {
            Debug.Log("Missing shader in " + this.ToString());
            return null;
        }
        if (((material != null) && (material.shader == shader)) && shader.isSupported)
        {
            return material;
        }
        if (!shader.isSupported)
        {
            return null;
        }
        material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        return ((material == null) ? null : material);
    }

    protected void DrawBorder(RenderTexture dest, Material material)
    {
        RenderTexture.active = dest;
        bool flag = true;
        GL.PushMatrix();
        GL.LoadOrtho();
        for (int i = 0; i < material.passCount; i++)
        {
            float num6;
            float num7;
            material.SetPass(i);
            if (flag)
            {
                num6 = 1f;
                num7 = 0f;
            }
            else
            {
                num6 = 0f;
                num7 = 1f;
            }
            float x = 0f;
            float num2 = 0f + (1f / (dest.width * 1f));
            float y = 0f;
            float num4 = 1f;
            GL.Begin(7);
            GL.TexCoord2(0f, num6);
            GL.Vertex3(x, y, 0.1f);
            GL.TexCoord2(1f, num6);
            GL.Vertex3(num2, y, 0.1f);
            GL.TexCoord2(1f, num7);
            GL.Vertex3(num2, num4, 0.1f);
            GL.TexCoord2(0f, num7);
            GL.Vertex3(x, num4, 0.1f);
            x = 1f - (1f / (dest.width * 1f));
            num2 = 1f;
            y = 0f;
            num4 = 1f;
            GL.TexCoord2(0f, num6);
            GL.Vertex3(x, y, 0.1f);
            GL.TexCoord2(1f, num6);
            GL.Vertex3(num2, y, 0.1f);
            GL.TexCoord2(1f, num7);
            GL.Vertex3(num2, num4, 0.1f);
            GL.TexCoord2(0f, num7);
            GL.Vertex3(x, num4, 0.1f);
            x = 0f;
            num2 = 1f;
            y = 0f;
            num4 = 0f + (1f / (dest.height * 1f));
            GL.TexCoord2(0f, num6);
            GL.Vertex3(x, y, 0.1f);
            GL.TexCoord2(1f, num6);
            GL.Vertex3(num2, y, 0.1f);
            GL.TexCoord2(1f, num7);
            GL.Vertex3(num2, num4, 0.1f);
            GL.TexCoord2(0f, num7);
            GL.Vertex3(x, num4, 0.1f);
            x = 0f;
            num2 = 1f;
            y = 1f - (1f / (dest.height * 1f));
            num4 = 1f;
            GL.TexCoord2(0f, num6);
            GL.Vertex3(x, y, 0.1f);
            GL.TexCoord2(1f, num6);
            GL.Vertex3(num2, y, 0.1f);
            GL.TexCoord2(1f, num7);
            GL.Vertex3(num2, num4, 0.1f);
            GL.TexCoord2(0f, num7);
            GL.Vertex3(x, num4, 0.1f);
            GL.End();
        }
        GL.PopMatrix();
    }

    protected void NotSupported()
    {
        base.enabled = false;
        this.isSupported = false;
    }

    protected void OnEnable()
    {
        this.isSupported = true;
    }

    protected void ReportAutoDisable()
    {
        Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
    }

    protected void Start()
    {
        this.CheckResources();
    }
}

