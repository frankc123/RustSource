using System;
using UnityEngine;

public class TimedExplosive : IDLocal
{
    public float damage = 100f;
    public GameObject explosionEffect;
    public float explosionRadius = 4f;
    public float fuseLength = 5f;
    private NGCView testView;
    public AudioClip tickSound;

    private void Awake()
    {
        this.testView = base.GetComponent<NGCView>();
        if (this.tickSound != null)
        {
            base.InvokeRepeating("TickSound", 0f, 1f);
        }
    }

    [RPC]
    public void ClientExplode()
    {
        Object.Instantiate(this.explosionEffect, base.transform.position, base.transform.rotation);
        base.CancelInvoke();
    }

    public void OnDestroy()
    {
        base.CancelInvoke();
    }

    public void TickSound()
    {
        this.tickSound.Play(base.transform.position, (float) 1f, (float) 3f, (float) 20f);
    }
}

