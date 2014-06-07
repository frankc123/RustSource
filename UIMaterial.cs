using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UIMaterial : ScriptableObject
{
    private const string alpha = " (AlphaClip)";
    private const string hard = " (HardClip)";
    private int hashCode;
    private const UIDrawCall.Clipping kBeginClipping = UIDrawCall.Clipping.None;
    private const UIDrawCall.Clipping kEndClipping = ((UIDrawCall.Clipping) 4);
    private Material key;
    private ClippingFlags madeMats;
    private Material matAlphaClip;
    private Material matFirst;
    private Material matHardClip;
    private Material matNone;
    private Material matSoftClip;
    private const string soft = " (SoftClip)";

    public UIMaterial Clone()
    {
        Material key = new Material(this.key) {
            hideFlags = HideFlags.DontSave
        };
        return Create(key, true);
    }

    public void CopyPropertiesFromMaterial(Material material)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            if (material == this.key)
            {
                return;
            }
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).CopyPropertiesFromMaterial(material);
            }
        }
    }

    public void CopyPropertiesFromOriginal()
    {
        if (this.madeMats != ((ClippingFlags) 0))
        {
            this.CopyPropertiesFromMaterial(this.key);
        }
    }

    public static UIMaterial Create(Material key)
    {
        UIMaterial material;
        if (key == null)
        {
            return null;
        }
        if (!g.keyedMaterials.TryGetValue(key, out material))
        {
            if (g.generatedMaterials.TryGetValue(key, out material))
            {
                return material;
            }
            material = ScriptableObject.CreateInstance<UIMaterial>();
            material.key = key;
            material.hashCode = ++g.hashCodeIterator;
            if (material.hashCode == 0x7fffffff)
            {
                g.hashCodeIterator = -2147483648;
            }
            g.keyedMaterials.Add(key, material);
        }
        return material;
    }

    public static UIMaterial Create(Material key, bool manageKeyDestruction)
    {
        return Create(key, manageKeyDestruction, UIDrawCall.Clipping.None);
    }

    public static UIMaterial Create(Material key, bool manageKeyDestruction, UIDrawCall.Clipping useAsClipping)
    {
        UIMaterial material;
        if (!manageKeyDestruction)
        {
            return Create(key);
        }
        if (key == null)
        {
            return null;
        }
        if (g.keyedMaterials.TryGetValue(key, out material))
        {
            throw new InvalidOperationException("That material is registered and cannot be used with manageKeyDestruction");
        }
        if (!g.generatedMaterials.TryGetValue(key, out material))
        {
            material = ScriptableObject.CreateInstance<UIMaterial>();
            material.key = key;
            material.hashCode = ++g.hashCodeIterator;
            if (material.hashCode == 0x7fffffff)
            {
                g.hashCodeIterator = -2147483648;
            }
            g.generatedMaterials.Add(key, material);
            material.matFirst = key;
            switch (useAsClipping)
            {
                case UIDrawCall.Clipping.None:
                    material.matNone = key;
                    break;

                case UIDrawCall.Clipping.HardClip:
                    material.matHardClip = key;
                    break;

                case UIDrawCall.Clipping.AlphaClip:
                    material.matAlphaClip = key;
                    break;

                case UIDrawCall.Clipping.SoftClip:
                    material.matSoftClip = key;
                    break;

                default:
                    throw new NotImplementedException();
            }
            material.madeMats = (ClippingFlags) (((int) 1) << useAsClipping);
        }
        return material;
    }

    private static Material CreateMaterial(Shader shader)
    {
        return new Material(shader) { hideFlags = HideFlags.NotEditable | HideFlags.DontSave };
    }

    private Material FastGet(UIDrawCall.Clipping clipping)
    {
        switch (clipping)
        {
            case UIDrawCall.Clipping.None:
                return this.matNone;

            case UIDrawCall.Clipping.HardClip:
                return this.matHardClip;

            case UIDrawCall.Clipping.AlphaClip:
                return this.matAlphaClip;

            case UIDrawCall.Clipping.SoftClip:
                return this.matSoftClip;
        }
        throw new NotImplementedException();
    }

    private static Shader GetClippingShader(Shader original, UIDrawCall.Clipping clipping)
    {
        if (original == null)
        {
            return null;
        }
        string name = original.name;
        switch (clipping)
        {
            case UIDrawCall.Clipping.None:
            {
                string str2 = name.Replace(" (HardClip)", string.Empty).Replace(" (AlphaClip)", string.Empty).Replace(" (SoftClip)", string.Empty);
                if (!(str2 == name))
                {
                    name = str2;
                    break;
                }
                return original;
            }
            case UIDrawCall.Clipping.HardClip:
                if (ShaderNameDecor(ref name, " (AlphaClip)", " (SoftClip)", " (HardClip)"))
                {
                    break;
                }
                return original;

            case UIDrawCall.Clipping.AlphaClip:
                if (ShaderNameDecor(ref name, " (SoftClip)", " (HardClip)", " (AlphaClip)"))
                {
                    break;
                }
                return original;

            case UIDrawCall.Clipping.SoftClip:
                if (ShaderNameDecor(ref name, " (HardClip)", " (AlphaClip)", " (SoftClip)"))
                {
                    break;
                }
                return original;

            default:
                throw new NotImplementedException();
        }
        Shader shader = Shader.Find(name);
        if (shader == null)
        {
            throw new MissingReferenceException("Theres no shader named " + name);
        }
        return shader;
    }

    public sealed override int GetHashCode()
    {
        return this.hashCode;
    }

    public bool HasProperty(string property)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            return this.key.HasProperty(property);
        }
        return this.matFirst.HasProperty(property);
    }

    private void MakeDefaultMaterial()
    {
        this.MakeMaterial(ShaderClipping(this.key.shader.name));
    }

    private Material MakeMaterial(UIDrawCall.Clipping clipping)
    {
        Material material;
        Material matNone;
        Shader clippingShader;
        bool flag = this.madeMats == ((ClippingFlags) 0);
        switch (clipping)
        {
            case UIDrawCall.Clipping.None:
                clippingShader = this.key.shader;
                matNone = this.matNone;
                material = this.matNone = CreateMaterial(clippingShader);
                this.madeMats |= ClippingFlags.None;
                break;

            case UIDrawCall.Clipping.HardClip:
                clippingShader = GetClippingShader(this.key.shader, UIDrawCall.Clipping.HardClip);
                matNone = this.matHardClip;
                material = this.matHardClip = CreateMaterial(clippingShader);
                this.madeMats |= ClippingFlags.HardClip;
                break;

            case UIDrawCall.Clipping.AlphaClip:
                clippingShader = GetClippingShader(this.key.shader, UIDrawCall.Clipping.AlphaClip);
                matNone = this.matAlphaClip;
                material = this.matAlphaClip = CreateMaterial(clippingShader);
                this.madeMats |= ClippingFlags.AlphaClip;
                break;

            case UIDrawCall.Clipping.SoftClip:
                clippingShader = GetClippingShader(this.key.shader, UIDrawCall.Clipping.SoftClip);
                matNone = this.matSoftClip;
                material = this.matSoftClip = CreateMaterial(clippingShader);
                this.madeMats |= ClippingFlags.SoftClip;
                break;

            default:
                throw new NotImplementedException();
        }
        g.generatedMaterials.Add(material, this);
        if (flag)
        {
            this.matFirst = material;
            material.CopyPropertiesFromMaterial(this.key);
        }
        else
        {
            material.CopyPropertiesFromMaterial(this.matFirst);
        }
        if (matNone != null)
        {
            Object.DestroyImmediate(matNone);
        }
        return material;
    }

    private void OnDestroy()
    {
        if (this.madeMats != ((ClippingFlags) 0))
        {
            for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
            {
                if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
                {
                    Material key = this.FastGet(clipping);
                    g.generatedMaterials.Remove(key);
                    Object.DestroyImmediate(key);
                }
            }
        }
        g.keyedMaterials.Remove(this.key);
        this.matNone = this.matFirst = this.matHardClip = this.matSoftClip = this.matAlphaClip = (Material) (this.key = null);
    }

    public static explicit operator Material(UIMaterial uimat)
    {
        return ((uimat == null) ? null : uimat.key);
    }

    public static explicit operator UIMaterial(Material key)
    {
        return Create(key);
    }

    public void Set(string property, float value)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetFloat(property, value);
            }
        }
    }

    public void Set(string property, Color color)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetColor(property, color);
            }
        }
    }

    public void Set(string property, Matrix4x4 mat)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetMatrix(property, mat);
            }
        }
    }

    public void Set(string property, Texture texture)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetTexture(property, texture);
            }
        }
    }

    public void Set(string property, Vector2 value)
    {
        Vector4 vector;
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        vector.x = value.x;
        vector.y = value.y;
        vector.z = vector.w = 0f;
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetVector(property, vector);
            }
        }
    }

    public void Set(string property, Vector3 value)
    {
        Vector4 vector;
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        vector.x = value.x;
        vector.y = value.y;
        vector.z = vector.w = 0f;
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetVector(property, vector);
            }
        }
    }

    public void Set(string property, Vector4 value)
    {
        Vector4 vector;
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        vector.x = value.x;
        vector.y = value.y;
        vector.z = vector.w = 0f;
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetVector(property, vector);
            }
        }
    }

    public void SetPass(int pass)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetPass(pass);
            }
        }
    }

    public void SetTextureOffset(string property, Vector2 offset)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetTextureOffset(property, offset);
            }
        }
    }

    public void SetTextureScale(string property, Vector2 scale)
    {
        if (this.madeMats == ((ClippingFlags) 0))
        {
            this.MakeDefaultMaterial();
        }
        for (UIDrawCall.Clipping clipping = UIDrawCall.Clipping.None; clipping < ((UIDrawCall.Clipping) 4); clipping += 1)
        {
            if ((this.madeMats & (((int) 1) << clipping)) != ((ClippingFlags) 0))
            {
                this.FastGet(clipping).SetTextureScale(property, scale);
            }
        }
    }

    private static UIDrawCall.Clipping ShaderClipping(string shaderName)
    {
        if (shaderName.EndsWith(" (SoftClip)"))
        {
            return UIDrawCall.Clipping.SoftClip;
        }
        if (shaderName.EndsWith(" (HardClip)"))
        {
            return UIDrawCall.Clipping.HardClip;
        }
        if (shaderName.EndsWith(" (AlphaClip)"))
        {
            return UIDrawCall.Clipping.AlphaClip;
        }
        return UIDrawCall.Clipping.None;
    }

    private static bool ShaderNameDecor(ref string shaderName, string not1, string not2, string suffix)
    {
        string str = shaderName.Replace(not1, string.Empty).Replace(not2, string.Empty);
        if (str != shaderName)
        {
            if (!str.EndsWith(suffix))
            {
                shaderName = str + suffix;
            }
            return true;
        }
        if (!shaderName.EndsWith(suffix))
        {
            shaderName = shaderName + suffix;
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return ((this.key == null) ? "destroyed" : this.key.ToString());
    }

    public Material this[UIDrawCall.Clipping clipping]
    {
        get
        {
            ClippingFlags flags = (ClippingFlags) (((int) 1) << clipping);
            if ((flags & this.madeMats) != flags)
            {
                return this.MakeMaterial(clipping);
            }
            switch (clipping)
            {
                case UIDrawCall.Clipping.None:
                    return this.matNone;

                case UIDrawCall.Clipping.HardClip:
                    return this.matHardClip;

                case UIDrawCall.Clipping.AlphaClip:
                    return this.matAlphaClip;

                case UIDrawCall.Clipping.SoftClip:
                    return this.matSoftClip;
            }
            throw new NotImplementedException();
        }
    }

    public Texture mainTexture
    {
        get
        {
            return ((this.madeMats != ((ClippingFlags) 0)) ? this.matFirst.mainTexture : this.key.mainTexture);
        }
        set
        {
            if (this.madeMats == ((ClippingFlags) 0))
            {
                this.MakeDefaultMaterial();
            }
            this.Set("_MainTex", value);
        }
    }

    private enum ClippingFlags
    {
        AlphaClip = 4,
        HardClip = 2,
        None = 1,
        SoftClip = 8
    }

    private static class g
    {
        public static readonly Dictionary<Material, UIMaterial> generatedMaterials = new Dictionary<Material, UIMaterial>();
        public static int hashCodeIterator = -2147483648;
        public static readonly Dictionary<Material, UIMaterial> keyedMaterials = new Dictionary<Material, UIMaterial>();
    }
}

