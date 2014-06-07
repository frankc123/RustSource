using System;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float initialLightIntensity = 2f;
    public Light myLight;
    public float startTime;

    public virtual void Start()
    {
        this.startTime = Time.time;
        Object.Destroy(base.gameObject, 3f);
        base.audio.pitch = Random.Range((float) 0.9f, (float) 1f);
        base.audio.Play();
    }

    public virtual void Update()
    {
        float num = Time.time - this.startTime;
        if (this.myLight != null)
        {
            this.myLight.intensity = Mathf.Clamp(this.initialLightIntensity * (1f - (num / 0.25f)), 0f, this.initialLightIntensity);
            if (this.myLight.intensity <= 0f)
            {
                Object.Destroy(this.myLight.gameObject);
                this.myLight = null;
            }
        }
    }
}

