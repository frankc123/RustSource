using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class SoundPoolFilter : MonoBehaviour
{
    private bool awake;
    private static SoundPoolFilter instance;
    private bool quitting;

    private void Awake()
    {
        if ((instance != null) && (instance != this))
        {
            Debug.LogError("ONLY HAVE ONE PLEASE", this);
        }
        else
        {
            instance = this;
            this.awake = true;
            SoundPool.enabled = base.enabled;
        }
    }

    private void OnApplicationQuit()
    {
        SoundPool.quitting = true;
        this.quitting = true;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            this.awake = false;
            instance = null;
            SoundPool.enabled = false;
            if (this.quitting)
            {
                SoundPool.Drain();
            }
        }
    }

    private void OnDisable()
    {
        if (this.awake)
        {
            SoundPool.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (this.awake)
        {
            SoundPool.enabled = true;
        }
    }

    private void OnPreCull()
    {
        SoundPool.Pump();
    }
}

