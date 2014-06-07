using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerSave : MonoBehaviour
{
    [SerializeField]
    private bool autoNetSerialize = true;
    [NonSerialized]
    private Reged registered;
    private static Dictionary<int, string> StructureDictionary;

    internal Reged REGED
    {
        get
        {
            return this.registered;
        }
    }

    internal enum Reged : sbyte
    {
        None = 0,
        ToNet = 1,
        ToNGC = 2
    }
}

