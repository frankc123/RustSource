using System;
using UnityEngine;

public class MultiLODGroupConfig : ScriptableObject
{
    [SerializeField]
    private LODGroup[] a;
    [SerializeField]
    public float[] l;
    public const string LODFractionArray = "l";
    public const string LODGroupArray = "a";
}

