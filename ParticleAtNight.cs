﻿using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAtNight : MonoBehaviour
{
    public float fadeTime = 1f;
    private float lerpTime;
    private ParticleSystem particleComponent;
    private float particleEmission;
    public TOD_Sky sky;

    protected void OnEnable()
    {
        if (this.sky == null)
        {
            Debug.LogError("Sky instance reference not set. Disabling script.");
            base.enabled = false;
        }
        this.particleComponent = base.particleSystem;
        this.particleEmission = this.particleComponent.emissionRate;
    }

    protected void Update()
    {
        int num = !this.sky.IsNight ? -1 : 1;
        this.lerpTime = Mathf.Clamp01(this.lerpTime + ((num * Time.deltaTime) / this.fadeTime));
        this.particleComponent.emissionRate = Mathf.Lerp(0f, this.particleEmission, this.lerpTime);
    }
}

