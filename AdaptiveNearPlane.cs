using System;
using UnityEngine;

public class AdaptiveNearPlane : MonoBehaviour
{
    public LayerMask forceLayers = 0;
    public LayerMask ignoreLayers = 0;
    public float maxNear = 0.65f;
    public float minNear = 0.22f;
    public float threshold = 0.05f;
}

