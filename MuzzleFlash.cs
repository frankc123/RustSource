using System;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    private float initialIntensity;
    public Light myLight;
    private float startTime;

    private void Start()
    {
        this.startTime = Time.time;
        this.initialIntensity = this.myLight.intensity;
    }

    private void Update()
    {
        float num = Mathf.Clamp((float) (1f - ((Time.time - this.startTime) / 0.1f)), (float) 0f, (float) 1f);
        this.myLight.intensity = this.initialIntensity * num;
    }
}

