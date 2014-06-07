using System;
using UnityEngine;

public class EffectSoundPlayer : MonoBehaviour
{
    public AudioClipArray sounds;

    private void Start()
    {
        this.sounds[Random.Range(0, this.sounds.Length)].Play(base.transform.position, (float) 1f, (float) 1f, (float) 10f);
    }
}

