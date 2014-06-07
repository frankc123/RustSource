using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public class TimedGrenade : RigidObj
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$mapA;
    public AudioClip bounceSound;
    public float damage;
    public GameObject explosionEffect;
    public float explosionRadius;
    private float fuseLength;
    private float lastBounceTime;
    public IDMain myOwner;

    public TimedGrenade() : base(RigidObj.FeatureFlags.ServerCollisions | RigidObj.FeatureFlags.StreamInitialVelocity | RigidObj.FeatureFlags.StreamOwnerViewID)
    {
        this.fuseLength = 3f;
        this.explosionRadius = 30f;
        this.damage = 200f;
    }

    [RPC]
    private void ClientBounce(NetworkMessageInfo info)
    {
        InterpTimedEvent.Queue(this, "bounce", ref info);
    }

    protected override void OnDone()
    {
        base.collider.enabled = false;
        Vector3 position = base.rigidbody.position;
        if (this.explosionEffect != null)
        {
            Object.Instantiate(this.explosionEffect, position, Quaternion.identity);
        }
        foreach (Collider collider in Physics.OverlapSphere(position, this.explosionRadius, 0x8000000))
        {
            Rigidbody attachedRigidbody = collider.attachedRigidbody;
            if ((attachedRigidbody != null) && !attachedRigidbody.isKinematic)
            {
                attachedRigidbody.AddExplosionForce(500f, position, this.explosionRadius, 2f);
            }
        }
    }

    protected override void OnHide()
    {
        if (base.renderer != null)
        {
            base.renderer.enabled = false;
        }
    }

    protected override bool OnInterpTimedEvent()
    {
        string tag = InterpTimedEvent.Tag;
        if (tag != null)
        {
            int num;
            if (<>f__switch$mapA == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                dictionary.Add("bounce", 0);
                <>f__switch$mapA = dictionary;
            }
            if (<>f__switch$mapA.TryGetValue(tag, out num) && (num == 0))
            {
                this.PlayClientBounce();
                return true;
            }
        }
        return base.OnInterpTimedEvent();
    }

    protected override void OnShow()
    {
        if (base.renderer != null)
        {
            base.renderer.enabled = true;
        }
    }

    private void PlayClientBounce()
    {
        this.bounceSound.Play(base.rigidbody.position, (float) 0.25f, Random.Range((float) 0.85f, (float) 1.15f), 1f, (float) 18f);
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        base.uLink_OnNetworkInstantiate(info);
    }
}

