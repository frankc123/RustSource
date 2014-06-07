using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTarget : MonoBehaviour
{
    [NonSerialized]
    private bool _initialized;
    public float gatherEfficiencyMultiplier = 1f;
    private float gatherProgress;
    [SerializeField]
    public List<ResourceGivePair> resourcesAvailable;
    private int startingTotal;
    public ResourceTargetType type;

    public enum ResourceTargetType
    {
        Animal = 0,
        LAST = 5,
        Rock1 = 3,
        Rock2 = 4,
        Rock3 = 5,
        StaticTree = 2,
        WoodPile = 1
    }
}

