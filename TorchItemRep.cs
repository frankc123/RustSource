using Facepunch;
using System;
using uLink;
using UnityEngine;

public class TorchItemRep : ItemRepresentation
{
    public GameObject _myLight;
    public GameObject _myLightPrefab;
    private const bool defaultLit = false;
    private bool lit;
    public AudioClip StrikeSound;

    private void KillLight()
    {
        if (this._myLight != null)
        {
            Object.Destroy(this._myLight);
            this._myLight = null;
        }
    }

    protected void OnDestroy()
    {
        this.KillLight();
        base.OnDestroy();
    }

    [RPC]
    protected void OnStatus(bool on)
    {
        if (on != this.lit)
        {
            if (on)
            {
                this.RepIgnite();
            }
            else
            {
                this.RepExtinguish();
            }
            this.lit = on;
        }
    }

    public void RepExtinguish()
    {
        if (this.lit)
        {
            this.lit = false;
            this.KillLight();
        }
    }

    public void RepIgnite()
    {
        if (!this.lit)
        {
            this.lit = true;
            this.StrikeSound.Play(base.transform.position, (float) 1f, (float) 2f, (float) 8f);
            this._myLight = base.muzzle.InstantiateAsChild(this._myLightPrefab, false);
        }
    }

    private void ServerRPC_Status(bool lit)
    {
        RPCMode othersExceptOwner;
        NetworkView networkView = base.networkView;
        if (!lit)
        {
            othersExceptOwner = RPCMode.OthersExceptOwner;
        }
        else
        {
            othersExceptOwner = RPCMode.OthersExceptOwnerBuffered;
        }
        networkView.RPC<bool>("OnStatus", othersExceptOwner, lit);
        this.lit = lit;
    }

    protected void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        base.uLink_OnNetworkInstantiate(info);
        this.OnStatus(false);
    }
}

