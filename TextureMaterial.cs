using System;
using System.Collections.Generic;
using UnityEngine;

public static class TextureMaterial
{
    private static Dictionary<Material, Dictionary<Texture, Material>> dict = new Dictionary<Material, Dictionary<Texture, Material>>();

    public static Material GetMaterial(Material skeleton, Texture mainTex)
    {
        Dictionary<Texture, Material> dictionary;
        Material material2;
        if (skeleton == null)
        {
            return null;
        }
        if (!dict.TryGetValue(skeleton, out dictionary))
        {
            Material material = new Material(skeleton) {
                mainTexture = mainTex
            };
            dictionary = new Dictionary<Texture, Material>();
            dictionary.Add(mainTex, material);
            dict.Add(skeleton, dictionary);
            return material;
        }
        if (!dictionary.TryGetValue(mainTex, out material2))
        {
            Material material3 = new Material(skeleton) {
                mainTexture = mainTex
            };
            dictionary.Add(mainTex, material3);
            return material3;
        }
        return material2;
    }
}

