using System;
using UnityEngine;

public class QuickLight : MonoBehaviour
{
    public float duration = 0.25f;
    public float range = 1f;

    public void Update()
    {
        Light light = base.light;
        light.range -= Time.deltaTime / this.duration;
        if (base.light.range <= 0f)
        {
            base.light.range = 0f;
            base.light.intensity = 0f;
        }
    }
}

