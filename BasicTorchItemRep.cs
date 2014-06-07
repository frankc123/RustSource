using Facepunch;
using System;
using uLink;
using UnityEngine;

public class BasicTorchItemRep : ItemRepresentation
{
    public GameObject _myLight;
    public GameObject _myLightPrefab;
    private const bool defaultLit = false;
    private bool lit;

    private void KillLight()
    {
        if (this._myLight != null)
        {
            Object.Destroy(this._myLight);
            this._myLight = null;
        }
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
}

