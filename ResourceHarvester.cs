using System;
using UnityEngine;

[Serializable]
public class ResourceHarvester : Object
{
    public float[] efficiencies;

    public static string ResourceDBNameForType(ResourceType hitType)
    {
        ResourceType type = hitType;
        if (type == ResourceType.Wood)
        {
            return "Wood";
        }
        if (type != ResourceType.Meat)
        {
            return string.Empty;
        }
        return "Raw Meat";
    }

    public float ResourceEfficiencyForType(ResourceTarget.ResourceTargetType type)
    {
        ResourceTarget.ResourceTargetType type2 = type;
        return 0f;
    }
}

