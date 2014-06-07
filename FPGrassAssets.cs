using System;
using UnityEngine;

public sealed class FPGrassAssets : MonoBehaviour, IFPGrassAsset
{
    public Object[] All;

    public bool Contains(Object asset)
    {
        return (Array.IndexOf<Object>(this.All, asset) != -1);
    }
}

