using System;
using UnityEngine;

public static class Resources
{
    private const string kDontUse = "Do not use Resources. Use Bundles.";
    private const bool kErrorNotWarning = false;

    public static Object[] FindObjectsOfTypeAll(Type type)
    {
        return Resources.FindObjectsOfTypeAll(type);
    }

    [Obsolete("Do not use Resources. Use Bundles.", false)]
    public static Object Load(string path)
    {
        return Resources.Load(path);
    }

    [Obsolete("Do not use Resources. Use Bundles.", false)]
    public static Object Load(string path, Type type)
    {
        return Resources.Load(path, type);
    }

    [Obsolete("Do not use Resources. Use Bundles.", false)]
    public static Object[] LoadAll(string path)
    {
        return Resources.LoadAll(path);
    }

    [Obsolete("Do not use Resources. Use Bundles.", false)]
    public static Object[] LoadAll(string path, Type type)
    {
        return Resources.LoadAll(path, type);
    }

    public static void UnloadAsset(Object assetToUnload)
    {
        Resources.UnloadAsset(assetToUnload);
    }

    public static AsyncOperation UnloadUnusedAssets()
    {
        return Resources.UnloadUnusedAssets();
    }
}

