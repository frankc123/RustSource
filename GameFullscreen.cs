using System;
using System.Runtime.InteropServices;
using UnityEngine;

public sealed class GameFullscreen : PostEffectsBase
{
    public Color fadeColor = new Color(0f, 0f, 0f, 1f);
    private const int kDefaultOverlayPass = 1;
    private const ScaleMode kDefaultScaleMode = ScaleMode.StretchToFill;
    private Material material;
    public readonly Overlay[] overlays;
    public Shader shader;
    private const float sqrtOf3 = 1.732051f;
    public Color tintColor = Color.white;

    public GameFullscreen()
    {
        Overlay[] overlayArray1 = new Overlay[4];
        Overlay overlay = new Overlay {
            pass = 1
        };
        overlayArray1[0] = overlay;
        Overlay overlay2 = new Overlay {
            pass = 1
        };
        overlayArray1[1] = overlay2;
        Overlay overlay3 = new Overlay {
            pass = 1
        };
        overlayArray1[2] = overlay3;
        Overlay overlay4 = new Overlay {
            pass = 1
        };
        overlayArray1[3] = overlay4;
        this.overlays = overlayArray1;
    }

    public override bool CheckResources()
    {
        this.CheckSupport(false);
        this.material = this.CheckShaderAndCreateMaterial(this.shader, this.material);
        if (!base.isSupported)
        {
            this.ReportAutoDisable();
        }
        return base.isSupported;
    }

    protected void OnDisable()
    {
        if (this.material != null)
        {
            Object.DestroyImmediate(this.material);
        }
    }

    protected void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (!this.CheckResources() || !this.run)
        {
            Graphics.Blit(src, dst);
        }
        else if ((this.tintColor.a > 0f) || (this.fadeColor.a > 0f))
        {
            this.material.SetColor("_FadeColor", this.tintColor);
            this.material.SetColor("_SolidColor", this.fadeColor);
            for (int i = 0; i < this.overlays.Length; i++)
            {
                if (this.overlays[i].willRender)
                {
                    this.material.SetFloat("_Blend", this.overlays[i].alpha);
                    this.material.SetTexture("_OverlayTex", this.overlays[i].texture);
                    RenderTexture dest = RenderTexture.GetTemporary(src.width, src.height, 0);
                    RenderTexture texture2 = RenderTexture.GetTemporary(src.width, src.height, 0);
                    try
                    {
                        Graphics.Blit(src, dest, this.material, this.overlays[i].pass);
                        while (++i < this.overlays.Length)
                        {
                            if (this.overlays[i].willRender)
                            {
                                this.material.SetFloat("_Blend", this.overlays[i].alpha);
                                this.material.SetTexture("_OverlayTex", this.overlays[i].texture);
                                Graphics.Blit(dest, texture2, this.material, this.overlays[i].pass);
                                RenderTexture texture3 = dest;
                                dest = texture2;
                                texture2 = texture3;
                            }
                        }
                        Graphics.Blit(dest, dst, this.material, 0);
                    }
                    finally
                    {
                        RenderTexture.ReleaseTemporary(dest);
                        RenderTexture.ReleaseTemporary(texture2);
                    }
                    return;
                }
            }
            Graphics.Blit(src, dst, this.material, 0);
        }
        else
        {
            for (int j = 0; j < this.overlays.Length; j++)
            {
                if (this.overlays[j].willRender)
                {
                    this.material.SetFloat("_Blend", this.overlays[j].alpha);
                    this.material.SetTexture("_OverlayTex", this.overlays[j].texture);
                    int pass = this.overlays[j].pass;
                    while (++j < this.overlays.Length)
                    {
                        if (this.overlays[j].willRender)
                        {
                            RenderTexture texture4 = RenderTexture.GetTemporary(src.width, src.height, 0);
                            RenderTexture texture5 = RenderTexture.GetTemporary(src.width, src.height, 0);
                            try
                            {
                                Graphics.Blit(src, texture4, this.material, pass);
                                this.material.SetFloat("_Blend", this.overlays[j].alpha);
                                this.material.SetTexture("_OverlayTex", this.overlays[j].texture);
                                pass = this.overlays[j].pass;
                                while (++j < this.overlays.Length)
                                {
                                    if (this.overlays[j].willRender)
                                    {
                                        Graphics.Blit(texture4, texture5, this.material, pass);
                                        RenderTexture texture6 = texture4;
                                        texture4 = texture5;
                                        texture5 = texture6;
                                        this.material.SetFloat("_Blend", this.overlays[j].alpha);
                                        this.material.SetTexture("_OverlayTex", this.overlays[j].texture);
                                        pass = this.overlays[j].pass;
                                    }
                                }
                                Graphics.Blit(texture4, dst, this.material, pass);
                            }
                            finally
                            {
                                RenderTexture.ReleaseTemporary(texture4);
                                RenderTexture.ReleaseTemporary(texture5);
                            }
                            return;
                        }
                    }
                    Graphics.Blit(src, dst, this.material, pass);
                    return;
                }
            }
        }
    }

    public Color autoFadeColor
    {
        get
        {
            return this.fadeColor;
        }
        set
        {
            this.fadeColor.r = value.r;
            this.fadeColor.g = value.g;
            this.fadeColor.b = value.b;
            if ((value.r == value.g) && (value.r == value.b))
            {
                this.tintColor.r = this.tintColor.g = this.tintColor.b = 1f;
            }
            else
            {
                float f = Mathf.Atan2(1.732051f * (value.g - value.b), ((2f * value.r) - value.g) - value.b) * 57.29578f;
                if (float.IsNaN(f) || float.IsInfinity(f))
                {
                    this.tintColor.r = this.tintColor.g = this.tintColor.b = 1f;
                }
                else
                {
                    float num2 = ((f >= 0f) ? f : (f + 360f)) / 60f;
                    float num3 = 1f * (1f - Mathf.Abs((float) ((num2 % 2f) - 1f)));
                    switch ((Mathf.FloorToInt(num2) % 6))
                    {
                        case 1:
                            this.tintColor.r = num3;
                            this.tintColor.g = 1f;
                            this.tintColor.b = 0f;
                            goto Label_02AE;

                        case 2:
                            this.tintColor.r = 0f;
                            this.tintColor.g = 1f;
                            this.tintColor.b = num3;
                            goto Label_02AE;

                        case 3:
                            this.tintColor.r = 0f;
                            this.tintColor.g = num3;
                            this.tintColor.b = 1f;
                            goto Label_02AE;

                        case 4:
                            this.tintColor.r = num3;
                            this.tintColor.g = 0f;
                            this.tintColor.b = 1f;
                            goto Label_02AE;

                        case 5:
                            this.tintColor.r = 1f;
                            this.tintColor.g = 0f;
                            this.tintColor.b = num3;
                            goto Label_02AE;
                    }
                    this.tintColor.r = 1f;
                    this.tintColor.g = num3;
                    this.tintColor.b = 0f;
                }
            }
        Label_02AE:
            this.tintColor.a = Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 0.5f, value.a)));
            this.fadeColor.a = Mathf.Clamp01(Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 1f, value.a)));
        }
    }

    private bool run
    {
        get
        {
            if ((this.fadeColor.a > 0f) || (this.tintColor.a > 0f))
            {
                return true;
            }
            for (int i = 0; i < this.overlays.Length; i++)
            {
                if (this.overlays[i].willRender)
                {
                    return true;
                }
            }
            return false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Overlay
    {
        public ScaleMode scaleMode;
        public int pass;
        private Texture2D _texture;
        private float _alpha;
        private bool hasTex;
        private bool hasAlpha;
        private bool shouldDraw;
        public bool willRender
        {
            get
            {
                if (this.shouldDraw && (this._texture == null))
                {
                    this.hasTex = false;
                    this._texture = null;
                    this.shouldDraw = false;
                }
                return this.shouldDraw;
            }
        }
        public float alpha
        {
            get
            {
                return this._alpha;
            }
            set
            {
                this._alpha = value;
                bool hasAlpha = this.hasAlpha;
                this.hasAlpha = value > 0f;
                if (hasAlpha != this.hasAlpha)
                {
                    this.shouldDraw = this.hasAlpha && this.hasTex;
                }
            }
        }
        public Texture2D texture
        {
            get
            {
                return this._texture;
            }
            set
            {
                this._texture = value;
                bool hasTex = this.hasTex;
                this.hasTex = (bool) this._texture;
                if (hasTex != this.hasTex)
                {
                    this.shouldDraw = this.hasTex && this.hasAlpha;
                }
            }
        }
    }
}

