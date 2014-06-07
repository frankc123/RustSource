﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class dfMarkupImageCache
{
    private static Dictionary<string, Texture> cache = new Dictionary<string, Texture>();

    public static void Clear()
    {
        cache.Clear();
    }

    public static Texture Load(string path)
    {
        path = path.ToLowerInvariant();
        if (cache.ContainsKey(path))
        {
            return cache[path];
        }
        Texture texture = Resources.Load(path) as Texture;
        if (texture != null)
        {
            cache[path] = texture;
        }
        return texture;
    }

    public static void Load(string name, Texture image)
    {
        cache[name.ToLowerInvariant()] = image;
    }

    public static void Unload(string name)
    {
        cache.Remove(name.ToLowerInvariant());
    }
}

