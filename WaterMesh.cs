using System;
using UnityEngine;

public class WaterMesh : MonoBehaviour
{
    public float minDistance = 2f;
    public bool reverseOrder;
    public WaterMesher root;
    public int sensitivity = 0x100;
    public bool smooth;
    public float underFlow;
}

