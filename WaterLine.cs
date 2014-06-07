using System;
using UnityEngine;

public class WaterLine : MonoBehaviour
{
    public static float Height;

    public void OnDestroy()
    {
        Height = 0f;
    }

    public void Start()
    {
    }

    public void Update()
    {
        Height = base.transform.position.y;
    }
}

