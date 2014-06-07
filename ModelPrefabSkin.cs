using System;
using UnityEngine;

public class ModelPrefabSkin : ScriptableObject
{
    [NonSerialized]
    public object editorData;
    public bool once;
    public Part[] parts;
    public string prefab;

    [Serializable]
    public class Part
    {
        public string[] materials;
        public string mesh = string.Empty;
        public string path = string.Empty;
    }
}

