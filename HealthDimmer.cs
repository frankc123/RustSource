using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct HealthDimmer
{
    [NonSerialized]
    private float averageRGB;
    [NonSerialized]
    private float? percent;
    [NonSerialized]
    private bool initialized;
    [NonSerialized]
    private bool valid;
    [NonSerialized]
    private bool structureStyle;
    [NonSerialized]
    private MeshRenderer[] renderers;
    [NonSerialized]
    private MaterialPropertyBlock propBlock;
    [NonSerialized]
    private TakeDamage takeDamage;
    [NonSerialized]
    public bool disabled;
    private void Initialize(IDBase self)
    {
        Material material;
        MeshRenderer[] rendererArray;
        TakeDamage damage;
        if (((self == null) || ((damage = self.GetLocal<TakeDamage>()) == null)) || !GetFirstMaterial<MeshRenderer>(rendererArray = self.GetComponentsInChildren<MeshRenderer>(true), out material))
        {
            this.renderers = null;
            this.valid = false;
            this.takeDamage = null;
        }
        else
        {
            this.renderers = rendererArray;
            this.takeDamage = damage;
            this.valid = true;
            this.structureStyle = self.idMain is StructureComponent;
            Color color = material.GetColor(PropOnce._Color);
            this.averageRGB = ((color.r + color.g) + color.b) * 0.3333333f;
            this.propBlock = new MaterialPropertyBlock();
            this.percent = null;
        }
    }

    private void MakeColor(float percent, out Color color)
    {
        float num;
        if (this.structureStyle)
        {
            num = 0.35f + ((this.averageRGB - 0.35f) * percent);
        }
        else
        {
            float num2 = this.averageRGB * 0.33f;
            num = num2 + ((this.averageRGB - num2) * percent);
        }
        color.r = color.g = color.b = num;
        color.a = 1f;
    }

    public void Reset()
    {
        this.percent = null;
        if (this.initialized)
        {
            if (this.propBlock != null)
            {
                this.propBlock.Clear();
            }
            if (this.valid)
            {
                foreach (MeshRenderer renderer in this.renderers)
                {
                    if (renderer != null)
                    {
                        renderer.SetPropertyBlock(null);
                    }
                }
            }
        }
    }

    public void UpdateHealthAmount(IDBase self, float newHealth, bool force = false)
    {
        if (!this.initialized)
        {
            this.initialized = true;
            this.Initialize(self);
        }
        if (this.takeDamage != null)
        {
            this.takeDamage.health = newHealth;
            if (!this.disabled && this.valid)
            {
                float num = this.takeDamage.health / this.takeDamage.maxHealth;
                if ((force || !this.percent.HasValue) || (this.percent.Value != num))
                {
                    Color color;
                    this.percent = new float?(num);
                    this.MakeColor(num, out color);
                    this.propBlock.Clear();
                    this.propBlock.AddColor(PropOnce._Color, color);
                    foreach (MeshRenderer renderer in this.renderers)
                    {
                        if (renderer != null)
                        {
                            renderer.SetPropertyBlock(this.propBlock);
                        }
                    }
                }
            }
        }
    }

    private static bool GetFirstMaterial<TRenderer>(TRenderer[] renderers, out Material material) where TRenderer: Renderer
    {
        int num;
        if ((renderers != null) && ((num = renderers.Length) > 0))
        {
            for (int i = 0; i < num; i++)
            {
                TRenderer local;
                Material material2;
                if ((((local = renderers[i]) != null) && ((material2 = local.sharedMaterial) != null)) && material2.HasProperty(PropOnce._Color))
                {
                    material = material2;
                    return true;
                }
            }
        }
        material = null;
        return false;
    }
    private static class PropOnce
    {
        public static readonly int _Color = Shader.PropertyToID("_Color");
    }
}

