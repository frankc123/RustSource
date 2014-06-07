using System;
using uLink;
using UnityEngine;

public sealed class FlareObj : RigidObj
{
    private GameObject lightInstance;
    public GameObject lightPrefab;
    public AudioClip StrikeSound;

    public FlareObj() : base(RigidObj.FeatureFlags.StreamInitialVelocity)
    {
    }

    protected override void OnDone()
    {
    }

    protected override void OnHide()
    {
        if (this.lightInstance != null)
        {
            this.lightInstance.SetActive(false);
        }
        if (base.renderer != null)
        {
            base.renderer.enabled = false;
        }
    }

    protected override void OnShow()
    {
        if (this.lightInstance != null)
        {
            this.lightInstance.SetActive(true);
        }
        if (base.renderer != null)
        {
            base.renderer.enabled = true;
        }
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this.lightInstance = Object.Instantiate(this.lightPrefab, base.transform.position + new Vector3(0f, 0.1f, 0f), Quaternion.identity) as GameObject;
        this.lightInstance.transform.parent = base.transform;
        base.uLink_OnNetworkInstantiate(info);
    }
}

