using System;
using UnityEngine;

public class NetDoppler : MonoBehaviour
{
    private float? lastVolume;
    public float minPitch = 0.5f;

    public void Update()
    {
        MountedCamera main = MountedCamera.main;
        if (main != null)
        {
            float num = Vector3.Distance(base.transform.position, main.transform.position);
            float num2 = 1f - Mathf.Clamp01(num / base.audio.maxDistance);
            float num3 = 1f + (this.minPitch * num2);
            base.audio.pitch = num3;
            if (this.lastVolume.HasValue)
            {
                base.audio.volume = this.lastVolume.Value;
                this.lastVolume = null;
            }
        }
        else
        {
            this.lastVolume = new float?(base.audio.volume);
        }
    }
}

