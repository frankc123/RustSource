using System;
using UnityEngine;

[Serializable]
public abstract class dfFontBase : MonoBehaviour
{
    protected dfFontBase()
    {
    }

    public abstract dfFontRendererBase ObtainRenderer();

    public abstract int FontSize { get; set; }

    public abstract bool IsValid { get; }

    public abstract int LineHeight { get; set; }

    public abstract UnityEngine.Material Material { get; set; }

    public abstract UnityEngine.Texture Texture { get; }
}

