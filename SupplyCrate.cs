using System;
using uLink;
using UnityEngine;

public class SupplyCrate : IDMain, IInterpTimedEventReceiver
{
    public RigidbodyInterpolator _interp;
    protected bool _landed;
    protected bool _landing;
    public GameObject bubbleWrap;
    public SupplyParachute chute;
    public GameObject landedEffect;
    public LootableObject lootableObject;
    protected RPCMode updateRPCMode;

    public SupplyCrate() : this(IDFlags.Unknown)
    {
    }

    protected SupplyCrate(IDFlags idFlags) : base(idFlags)
    {
        this.updateRPCMode = RPCMode.Others;
    }

    [RPC]
    protected void GetNetworkUpdate(Vector3 pos, Quaternion rot, NetworkMessageInfo info)
    {
        this._interp.SetGoals(pos, rot, info.timestamp);
    }

    void IInterpTimedEventReceiver.OnInterpTimedEvent()
    {
        if (InterpTimedEvent.Tag == "LAND")
        {
            this.LandShared();
            GameObject obj2 = Object.Instantiate(this.landedEffect, base.transform.position, base.transform.rotation) as GameObject;
            Object.Destroy(obj2, 2.5f);
            this._landed = true;
            this.chute.Landed();
        }
        else
        {
            InterpTimedEvent.MarkUnhandled();
        }
    }

    [RPC]
    public void Landed(NetworkMessageInfo info)
    {
        InterpTimedEvent.Queue(this, "LAND", ref info);
    }

    private void LandShared()
    {
        this._landed = true;
        if (this.lootableObject != null)
        {
            this.lootableObject.accessLocked = false;
        }
        Object.Destroy(this.bubbleWrap);
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this.lootableObject.accessLocked = true;
        this._interp.running = true;
        base.rigidbody.isKinematic = true;
    }
}

