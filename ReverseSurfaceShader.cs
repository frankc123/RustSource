using System;
using UnityEngine;

public class ReverseSurfaceShader : ScriptableObject
{
    public Shader inputShader;
    public ShaderMod[] mods;
    public Shader outputShader;
    public string outputShaderName;
    public bool pragmaDebug;
}

