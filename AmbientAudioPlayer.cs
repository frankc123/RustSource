using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(AudioSource))]
public class AmbientAudioPlayer : MonoBehaviour
{
    public AudioClip daySound;
    public AudioSource daySource;
    public AudioClip nightSound;
    public AudioSource nightSource;

    private void Awake()
    {
        base.InvokeRepeating("CheckTimeChange", 0f, 5f);
        this.daySource.clip = this.daySound;
        this.nightSource.clip = this.nightSound;
        this.daySource.volume = 0f;
        this.nightSource.volume = 0f;
        this.daySource.Stop();
        this.nightSource.Stop();
        this.daySource.enabled = false;
        this.nightSource.enabled = false;
    }

    private void CheckTimeChange()
    {
        if (this.NeedsAudioUpdate())
        {
            base.Invoke("UpdateVolume", 0f);
        }
    }

    public bool NeedsAudioUpdate()
    {
        if (EnvironmentControlCenter.Singleton != null)
        {
            if (((EnvironmentControlCenter.Singleton != null) && !EnvironmentControlCenter.Singleton.IsNight()) && ((this.daySource.volume < 1f) || (this.nightSource.volume > 0f)))
            {
                return true;
            }
            if (((EnvironmentControlCenter.Singleton == null) || !EnvironmentControlCenter.Singleton.IsNight()) || ((this.nightSource.volume >= 1f) && (this.daySource.volume <= 0f)))
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateVolume()
    {
        if (this.NeedsAudioUpdate())
        {
            base.Invoke("UpdateVolume", Time.deltaTime);
        }
        else
        {
            return;
        }
        bool flag = (EnvironmentControlCenter.Singleton == null) || !EnvironmentControlCenter.Singleton.IsNight();
        AudioSource source = !flag ? this.nightSource : this.daySource;
        AudioSource source2 = !flag ? this.daySource : this.nightSource;
        if (!source.isPlaying)
        {
            source.enabled = true;
            source.Play();
        }
        source.volume += 0.2f * Time.deltaTime;
        source2.volume -= 0.2f * Time.deltaTime;
        if (source.volume > 1f)
        {
            source.volume = 1f;
        }
        if (source2.volume < 0f)
        {
            source2.volume = 0f;
            source2.Stop();
            source2.enabled = false;
        }
    }
}

