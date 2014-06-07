using System;
using System.Collections.Generic;
using uLink;
using UnityEngine;

[AddComponentMenu("")]
internal sealed class NGCInternalView : uLinkNetworkView
{
    [NonSerialized]
    private NGC ngc;

    private void Awake()
    {
        this.ngc = base.GetComponent<NGC>();
        this.ngc.networkView = this;
        try
        {
            base.observed = this.ngc;
            base.rpcReceiver = RPCReceiver.OnlyObservedComponent;
            base.stateSynchronization = NetworkStateSynchronization.Off;
            base.securable = NetworkSecurable.None;
        }
        finally
        {
            try
            {
                base.Awake();
            }
            finally
            {
                this.ngc.networkViewID = base.viewID;
            }
        }
    }

    internal NGC GetNGC()
    {
        return this.ngc;
    }

    protected override bool OnRPC(string rpcName, BitStream stream, NetworkMessageInfo info)
    {
        string str;
        char key = rpcName[0];
        if (!Hack.actionToRPCName.TryGetValue(key, out str))
        {
            Hack.actionToRPCName[key] = str = "NGC:" + key;
        }
        return base.OnRPC(str, stream, info);
    }

    private static class Hack
    {
        public static Dictionary<char, string> actionToRPCName = new Dictionary<char, string>();
    }
}

