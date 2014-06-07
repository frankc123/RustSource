using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Volume")]
public class TweenVolume : UITweener
{
    public float from;
    private AudioSource mSource;
    public float to = 1f;

    public static TweenVolume Begin(GameObject go, float duration, float targetVolume)
    {
        TweenVolume volume = UITweener.Begin<TweenVolume>(go, duration);
        volume.from = volume.volume;
        volume.to = targetVolume;
        return volume;
    }

    protected override void OnUpdate(float factor)
    {
        this.volume = (this.from * (1f - factor)) + (this.to * factor);
        this.mSource.enabled = this.mSource.volume > 0.01f;
    }

    public AudioSource audioSource
    {
        get
        {
            if (this.mSource == null)
            {
                this.mSource = base.audio;
                if (this.mSource == null)
                {
                    this.mSource = base.GetComponentInChildren<AudioSource>();
                    if (this.mSource == null)
                    {
                        Debug.LogError("TweenVolume needs an AudioSource to work with", this);
                        base.enabled = false;
                    }
                }
            }
            return this.mSource;
        }
    }

    public float volume
    {
        get
        {
            return this.audioSource.volume;
        }
        set
        {
            this.audioSource.volume = value;
        }
    }
}

