using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAtDay : MonoBehaviour
{
    private AudioSource audioComponent;
    private float audioVolume;
    public float fadeTime = 1f;
    private float lerpTime;
    public TOD_Sky sky;

    protected void OnEnable()
    {
        if (this.sky == null)
        {
            Debug.LogError("Sky instance reference not set. Disabling script.");
            base.enabled = false;
        }
        this.audioComponent = base.audio;
        this.audioVolume = this.audioComponent.volume;
    }

    protected void Update()
    {
        int num = !this.sky.IsDay ? -1 : 1;
        this.lerpTime = Mathf.Clamp01(this.lerpTime + ((num * Time.deltaTime) / this.fadeTime));
        this.audioComponent.volume = Mathf.Lerp(0f, this.audioVolume, this.lerpTime);
    }
}

