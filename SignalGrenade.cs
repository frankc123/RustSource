using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public class SignalGrenade : RigidObj
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map9;
    public AudioClip bounceSound;
    public GameObject explosionEffect;
    private float fuseLength;
    private float lastBounceTime;

    public SignalGrenade() : base(RigidObj.FeatureFlags.ServerCollisions | RigidObj.FeatureFlags.StreamInitialVelocity | RigidObj.FeatureFlags.StreamOwnerViewID)
    {
        this.fuseLength = 3f;
    }

    [RPC]
    private void ClientBounce(NetworkMessageInfo info)
    {
        InterpTimedEvent.Queue(this, "bounce", ref info);
    }

    protected override void OnDone()
    {
        Object.Destroy(Object.Instantiate(this.explosionEffect, base.transform.position, Quaternion.LookRotation(Vector3.up)), 60f);
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
            if (<>f__switch$map9 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                dictionary.Add("bounce", 0);
                <>f__switch$map9 = dictionary;
            }
            if (<>f__switch$map9.TryGetValue(tag, out num) && (num == 0))
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

