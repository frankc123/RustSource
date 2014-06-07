using System;
using UnityEngine;

public class OpaqueCapture : PostEffectsBase
{
    private RenderTexture captureRT;
    private int d = -1;
    private RenderTextureFormat fmt;
    private int h = -1;
    private int w = -1;

    public override bool CheckResources()
    {
        this.CheckSupport(false);
        if (!base.isSupported)
        {
            this.ReportAutoDisable();
        }
        return base.isSupported;
    }

    private void CleanupCaptureRT()
    {
        if (this.captureRT != null)
        {
            Object.DestroyImmediate(this.captureRT);
        }
        this.w = -1;
        this.h = -1;
        this.d = -1;
    }

    protected void OnDisable()
    {
        this.CleanupCaptureRT();
    }

    [ImageEffectOpaque]
    protected void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (!this.CheckResources())
        {
            Graphics.Blit(src, dst);
        }
        else
        {
            int width = src.width;
            int height = src.height;
            int depth = src.depth;
            RenderTextureFormat format = src.format;
            if (((width != this.w) || (height != this.h)) || ((depth != this.d) || (format != this.fmt)))
            {
                this.CleanupCaptureRT();
                RenderTexture texture = new RenderTexture(width, height, depth, format) {
                    hideFlags = HideFlags.DontSave
                };
                this.captureRT = texture;
                if (!this.captureRT.Create() && !this.captureRT.IsCreated())
                {
                    Graphics.Blit(src, dst);
                    return;
                }
                this.captureRT.SetGlobalShaderProperty("_OpaqueFrame");
                this.w = width;
                this.h = height;
                this.d = depth;
                this.fmt = format;
            }
            Graphics.Blit(src, this.captureRT);
            Graphics.Blit(src, dst);
        }
    }
}

