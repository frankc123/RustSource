using System;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class FPGrassProbabilities : ScriptableObject, IFPGrassAsset
{
    [NonSerialized]
    private int applyLock;
    [NonSerialized]
    private bool applyQueued;
    [NonSerialized]
    private bool enabled;
    private const int kBillnearPixelSize = 4;
    private const TextureFormat kDetailProbabilityFormat = TextureFormat.Alpha8;
    private const int kHeight = 2;
    private const sbyte kTOpt_Default = 0;
    private const sbyte kTOpt_NoApplyPixelsOnCreate = 3;
    private const sbyte kTOpt_NoSetPixelsOnCreate = 1;
    private const sbyte kTOpt_ReCreate = 4;
    private const int kWidth = 0x10;
    [NonSerialized]
    private bool linear;
    [HideInInspector, SerializeField]
    private Color[] pixels;
    [Obsolete, SerializeField]
    private Texture2D probabilityTexture;
    [NonSerialized]
    private Texture2D texture;
    [NonSerialized]
    private bool updateQueued;

    public int GetDetailID(int splatChannel, int detailIndex)
    {
        return (int) (this.GetPixels()[(4 * splatChannel) + detailIndex].a * 256f);
    }

    public float GetDetailProbability(int splatChannel, int detailIndex)
    {
        return this.GetPixels()[((4 * splatChannel) + detailIndex) + 0x10].a;
    }

    private Color[] GetPixels()
    {
        if (this.probabilityTexture != null)
        {
            Debug.LogWarning("ProbabilityTexture is now created at runtime. Saved the pixels off the texture and now dereferencing it", this.probabilityTexture);
            this.pixels = this.probabilityTexture.GetPixels(0, 0, 0x10, 2, 0);
            this.probabilityTexture = null;
        }
        else if (object.ReferenceEquals(this.pixels, null) || (this.pixels.Length != 0x20))
        {
            this.pixels = new Color[0x20];
            try
            {
                this.StartEditing();
                for (int i = 0; i < 4; i++)
                {
                    this.SetDetailProperty(i, 0, 0, 1f);
                }
            }
            finally
            {
                this.StopEditing();
            }
        }
        return this.pixels;
    }

    public Texture2D GetTexture()
    {
        return this.GetTexture(0);
    }

    private Texture2D GetTexture(sbyte TOpt)
    {
        Texture2D textured;
        if (this.texture != null)
        {
            if ((TOpt & 4) == 0)
            {
                return this.texture;
            }
            Object.DestroyImmediate(this.texture, false);
            this.texture = null;
        }
        if (FPGrass.Support.DetailProbabilityFilterMode == FilterMode.Point)
        {
            textured = new Texture2D(0x10, 2, TextureFormat.Alpha8, false, false) {
                hideFlags = HideFlags.DontSave,
                name = "FPGrass Detail Probability (Point)",
                anisoLevel = 0,
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            this.texture = textured;
            this.linear = false;
        }
        else
        {
            textured = new Texture2D(0x40, 8, TextureFormat.Alpha8, false, false) {
                hideFlags = HideFlags.DontSave,
                name = "FPGrass Detail Probability (Linear)",
                anisoLevel = 0,
                filterMode = FPGrass.Support.DetailProbabilityFilterMode,
                wrapMode = TextureWrapMode.Clamp
            };
            this.texture = textured;
            this.linear = true;
        }
        if ((TOpt & 1) == 0)
        {
            this.UpdatePixels((TOpt & 3) == 0);
        }
        return this.texture;
    }

    public void Initialize()
    {
        if (this.probabilityTexture != null)
        {
            this.pixels = null;
            this.GetPixels();
        }
    }

    private void OnDisable()
    {
        if (this.enabled)
        {
            this.enabled = false;
            if (this.texture != null)
            {
                Object.DestroyImmediate(this.texture, false);
                this.texture = null;
            }
        }
    }

    private void OnEnable()
    {
        if (!this.enabled)
        {
            this.enabled = true;
            this.Initialize();
        }
    }

    public void SetDetailProperty(int splatChannel, int detailIndex, int detailID, float probability)
    {
        Color[] pixels = this.GetPixels();
        int index = (4 * splatChannel) + detailIndex;
        int num2 = index + 0x10;
        float num3 = ((detailID >= 0) ? ((detailID <= 0x100) ? ((float) detailID) : ((float) 0x100)) : ((float) 0)) / 256f;
        float num4 = (probability >= 0f) ? ((probability <= 1f) ? probability : 1f) : 0f;
        bool flag = false;
        if (SetDif(ref pixels[index].a, num3))
        {
            flag = true;
        }
        if (SetDif(ref pixels[num2].a, num4))
        {
            flag = true;
        }
        if (flag)
        {
            this.UpdatePixels(true);
        }
    }

    private static bool SetDif(ref float current, float value)
    {
        if (current != value)
        {
            current = value;
            return true;
        }
        return false;
    }

    public void StartEditing()
    {
        this.applyLock++;
    }

    public void StopEditing()
    {
        if (--this.applyLock <= 0)
        {
            this.applyLock = 0;
            if (this.updateQueued)
            {
                this.updateQueued = false;
                bool applyQueued = this.applyQueued;
                this.applyQueued = false;
                this.UpdatePixels(applyQueued);
            }
        }
    }

    private void UpdatePixels(bool apply = false)
    {
        if (this.applyLock == 0)
        {
            Color[] pixels = this.GetPixels();
            Texture2D texture = this.GetTexture(1);
            if (this.linear)
            {
                Linear.SetLinearPixels(pixels, texture);
            }
            else
            {
                texture.SetPixels(0, 0, 0x10, 2, pixels);
            }
            if (apply)
            {
                texture.Apply();
            }
        }
        else
        {
            this.updateQueued = true;
            this.applyQueued |= apply;
        }
    }

    private static class Linear
    {
        private static readonly Color[] B = new Color[0x200];
        private const int kB = 4;
        private const int kB_Stride = 0x40;
        private const int kH = 2;
        private const int kP_Stride = 0x10;
        private const int kW = 0x10;

        public static void SetLinearPixels(Color[] P, Texture2D Dest)
        {
            for (int i = 0; i < 0x10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int num3 = i * 4;
                    int num4 = 0;
                    while (num4 < 4)
                    {
                        int num5 = j * 4;
                        int num6 = 0;
                        while (num6 < 4)
                        {
                            int index = (j * 0x10) + i;
                            int num8 = (num5 * 0x40) + num3;
                            B[num8].r = P[index].r;
                            B[num8].g = P[index].g;
                            B[num8].b = P[index].b;
                            B[num8].a = P[index].a;
                            num6++;
                            num5++;
                        }
                        num4++;
                        num3++;
                    }
                }
            }
            Dest.SetPixels(0, 0, 0x40, 8, B, 0);
        }
    }
}

