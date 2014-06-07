using System;
using System.Collections.Generic;
using uLink;
using UnityEngine;

public sealed class NGCView : MonoBehaviour
{
    [NonSerialized]
    public NetworkMessageInfo creation;
    [NonSerialized]
    public BitStream initialData;
    [NonSerialized]
    public ushort innerID;
    [NonSerialized]
    internal NGC.Prefab.Installation.Instance install;
    [NonSerialized]
    private NGC.EventCallback onPreDestroy;
    [NonSerialized]
    public NGC outer;
    [NonSerialized]
    private bool preDestroying;
    [NonSerialized]
    public NGC.Prefab prefab;
    [SerializeField]
    internal MonoBehaviour[] scripts;
    [NonSerialized]
    internal Vector3 spawnPosition;
    [NonSerialized]
    internal Quaternion spawnRotation;

    public event NGC.EventCallback OnPreDestroy
    {
        add
        {
            if (this.preDestroying)
            {
                value(this);
            }
            else
            {
                this.onPreDestroy = (NGC.EventCallback) Delegate.Combine(this.onPreDestroy, value);
            }
        }
        remove
        {
            this.onPreDestroy = (NGC.EventCallback) Delegate.Remove(this.onPreDestroy, value);
        }
    }

    private NGC EnsureCall()
    {
        return this.outer;
    }

    public static NGCView Find(int id)
    {
        return NGC.Find(id);
    }

    internal void PostInstantiate()
    {
        base.BroadcastMessage("NGC_OnInstantiate", this, SendMessageOptions.DontRequireReceiver);
    }

    internal void PreDestroy()
    {
        if (!this.preDestroying)
        {
            this.preDestroying = true;
            if (this.onPreDestroy != null)
            {
                NGC.EventCallback onPreDestroy = this.onPreDestroy;
                this.onPreDestroy = null;
                try
                {
                    onPreDestroy(this);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }
    }

    public void RPC(string message, IEnumerable<NetworkPlayer> target)
    {
        this.EnsureCall().NGCViewRPC(target, this, this.prefab.MessageIndex(message), null, 0, 0);
    }

    public void RPC(string message, NetworkPlayer target)
    {
        this.EnsureCall().NGCViewRPC(target, this, this.prefab.MessageIndex(message), null, 0, 0);
    }

    public void RPC(string message, RPCMode mode)
    {
        this.EnsureCall().NGCViewRPC(mode, this, this.prefab.MessageIndex(message), null, 0, 0);
    }

    public void RPC<P0, P1, P2>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<T>(string message, NetworkPlayer target, T arg)
    {
        this.RPC_Stream(message, target, ToStream<T>(arg));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1>(string messageName, NetworkPlayer target, NGC.callf<P0, P1>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, target, data);
    }

    public void RPC<T>(string message, IEnumerable<NetworkPlayer> target, T arg)
    {
        this.RPC_Stream(message, target, ToStream<T>(arg));
    }

    public void RPC<T>(string message, RPCMode mode, T arg)
    {
        this.RPC_Stream(message, mode, ToStream<T>(arg));
    }

    public void RPC<P0, P1>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1>(string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block>(block, new object[0]);
        this.RPC_Stream(messageName, rpcMode, data);
    }

    public void RPC(NetworkFlags flags, string message, IEnumerable<NetworkPlayer> target)
    {
        this.EnsureCall().NGCViewRPC(flags, target, this, this.prefab.MessageIndex(message), null, 0, 0);
    }

    public void RPC(NetworkFlags flags, string message, NetworkPlayer target)
    {
        this.EnsureCall().NGCViewRPC(flags, target, this, this.prefab.MessageIndex(message), null, 0, 0);
    }

    public void RPC(NetworkFlags flags, string message, RPCMode mode)
    {
        this.EnsureCall().NGCViewRPC(flags, mode, this, this.prefab.MessageIndex(message), null, 0, 0);
    }

    public void RPC<P0, P1>(string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        this.RPC<P0, P1>(messageName, target, NGC.BlockArgs<P0, P1>(p0, p1));
    }

    public void RPC<P0, P1>(string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        this.RPC<P0, P1>(messageName, rpcMode, NGC.BlockArgs<P0, P1>(p0, p1));
    }

    public void RPC<P0, P1>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        this.RPC<P0, P1>(messageName, targets, NGC.BlockArgs<P0, P1>(p0, p1));
    }

    public void RPC<P0, P1>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<T>(NetworkFlags flags, string message, IEnumerable<NetworkPlayer> target, T arg)
    {
        this.RPC_Stream(flags, message, target, ToStream<T>(arg));
    }

    public void RPC<P0, P1, P2, P3>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<T>(NetworkFlags flags, string message, NetworkPlayer target, T arg)
    {
        this.RPC_Stream(flags, message, target, ToStream<T>(arg));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<T>(NetworkFlags flags, string message, RPCMode mode, T arg)
    {
        this.RPC_Stream(flags, message, mode, ToStream<T>(arg));
    }

    public void RPC<P0, P1, P2, P3>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkFlags flags, string messageName, NetworkPlayer target, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, target, data);
    }

    public void RPC<P0, P1>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, targets, data);
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkFlags flags, string messageName, RPCMode rpcMode, NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block block)
    {
        BitStream data = new BitStream(false);
        data.Write<NGC.callf<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>.Block>(block, new object[0]);
        this.RPC_Stream(flags, messageName, rpcMode, data);
    }

    public void RPC<P0, P1, P2>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        this.RPC<P0, P1, P2>(messageName, target, NGC.BlockArgs<P0, P1, P2>(p0, p1, p2));
    }

    public void RPC<P0, P1, P2>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        this.RPC<P0, P1, P2>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2>(p0, p1, p2));
    }

    public void RPC<P0, P1, P2>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        this.RPC<P0, P1, P2>(messageName, targets, NGC.BlockArgs<P0, P1, P2>(p0, p1, p2));
    }

    public void RPC<P0, P1>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        this.RPC<P0, P1>(flags, messageName, target, NGC.BlockArgs<P0, P1>(p0, p1));
    }

    public void RPC<P0, P1>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        this.RPC<P0, P1>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1>(p0, p1));
    }

    public void RPC<P0, P1>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        this.RPC<P0, P1>(flags, messageName, targets, NGC.BlockArgs<P0, P1>(p0, p1));
    }

    public void RPC<P0, P1, P2, P3>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        this.RPC<P0, P1, P2, P3>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3>(p0, p1, p2, p3));
    }

    public void RPC<P0, P1, P2, P3>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        this.RPC<P0, P1, P2, P3>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3>(p0, p1, p2, p3));
    }

    public void RPC<P0, P1, P2, P3>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        this.RPC<P0, P1, P2, P3>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3>(p0, p1, p2, p3));
    }

    public void RPC<P0, P1, P2>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        this.RPC<P0, P1, P2>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2>(p0, p1, p2));
    }

    public void RPC<P0, P1, P2>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        this.RPC<P0, P1, P2>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2>(p0, p1, p2));
    }

    public void RPC<P0, P1, P2>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        this.RPC<P0, P1, P2>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2>(p0, p1, p2));
    }

    public void RPC<P0, P1, P2, P3, P4>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        this.RPC<P0, P1, P2, P3, P4>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4>(p0, p1, p2, p3, p4));
    }

    public void RPC<P0, P1, P2, P3, P4>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        this.RPC<P0, P1, P2, P3, P4>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4>(p0, p1, p2, p3, p4));
    }

    public void RPC<P0, P1, P2, P3, P4>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        this.RPC<P0, P1, P2, P3, P4>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4>(p0, p1, p2, p3, p4));
    }

    public void RPC<P0, P1, P2, P3>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        this.RPC<P0, P1, P2, P3>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3>(p0, p1, p2, p3));
    }

    public void RPC<P0, P1, P2, P3>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        this.RPC<P0, P1, P2, P3>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3>(p0, p1, p2, p3));
    }

    public void RPC<P0, P1, P2, P3>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        this.RPC<P0, P1, P2, P3>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3>(p0, p1, p2, p3));
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        this.RPC<P0, P1, P2, P3, P4, P5>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5>(p0, p1, p2, p3, p4, p5));
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        this.RPC<P0, P1, P2, P3, P4, P5>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5>(p0, p1, p2, p3, p4, p5));
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        this.RPC<P0, P1, P2, P3, P4, P5>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5>(p0, p1, p2, p3, p4, p5));
    }

    public void RPC<P0, P1, P2, P3, P4>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        this.RPC<P0, P1, P2, P3, P4>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4>(p0, p1, p2, p3, p4));
    }

    public void RPC<P0, P1, P2, P3, P4>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        this.RPC<P0, P1, P2, P3, P4>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4>(p0, p1, p2, p3, p4));
    }

    public void RPC<P0, P1, P2, P3, P4>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        this.RPC<P0, P1, P2, P3, P4>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4>(p0, p1, p2, p3, p4));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6>(p0, p1, p2, p3, p4, p5, p6));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6>(p0, p1, p2, p3, p4, p5, p6));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6>(p0, p1, p2, p3, p4, p5, p6));
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        this.RPC<P0, P1, P2, P3, P4, P5>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5>(p0, p1, p2, p3, p4, p5));
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        this.RPC<P0, P1, P2, P3, P4, P5>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5>(p0, p1, p2, p3, p4, p5));
    }

    public void RPC<P0, P1, P2, P3, P4, P5>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        this.RPC<P0, P1, P2, P3, P4, P5>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5>(p0, p1, p2, p3, p4, p5));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7>(p0, p1, p2, p3, p4, p5, p6, p7));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7>(p0, p1, p2, p3, p4, p5, p6, p7));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7>(p0, p1, p2, p3, p4, p5, p6, p7));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6>(p0, p1, p2, p3, p4, p5, p6));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6>(p0, p1, p2, p3, p4, p5, p6));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6>(p0, p1, p2, p3, p4, p5, p6));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8>(p0, p1, p2, p3, p4, p5, p6, p7, p8));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8>(p0, p1, p2, p3, p4, p5, p6, p7, p8));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8>(p0, p1, p2, p3, p4, p5, p6, p7, p8));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7>(p0, p1, p2, p3, p4, p5, p6, p7));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7>(p0, p1, p2, p3, p4, p5, p6, p7));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7>(p0, p1, p2, p3, p4, p5, p6, p7));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8>(p0, p1, p2, p3, p4, p5, p6, p7, p8));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8>(p0, p1, p2, p3, p4, p5, p6, p7, p8));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8>(p0, p1, p2, p3, p4, p5, p6, p7, p8));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(flags, messageName, targets, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(flags, messageName, target, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
    }

    public void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        this.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(flags, messageName, rpcMode, NGC.BlockArgs<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
    }

    public void RPC_Bytes(string message, IEnumerable<NetworkPlayer> target, byte[] data)
    {
        this.EnsureCall().NGCViewRPC(target, this, this.prefab.MessageIndex(message), data, 0, (data != null) ? data.Length : 0);
    }

    public void RPC_Bytes(string message, RPCMode mode, byte[] data)
    {
        this.EnsureCall().NGCViewRPC(mode, this, this.prefab.MessageIndex(message), data, 0, (data != null) ? data.Length : 0);
    }

    public void RPC_Bytes(string message, NetworkPlayer target, byte[] data)
    {
        this.EnsureCall().NGCViewRPC(target, this, this.prefab.MessageIndex(message), data, 0, (data != null) ? data.Length : 0);
    }

    public void RPC_Bytes(NetworkFlags flags, string message, IEnumerable<NetworkPlayer> target, byte[] data)
    {
        this.EnsureCall().NGCViewRPC(flags, target, this, this.prefab.MessageIndex(message), data, 0, (data != null) ? data.Length : 0);
    }

    public void RPC_Bytes(NetworkFlags flags, string message, NetworkPlayer target, byte[] data)
    {
        this.EnsureCall().NGCViewRPC(flags, target, this, this.prefab.MessageIndex(message), data, 0, (data != null) ? data.Length : 0);
    }

    public void RPC_Bytes(NetworkFlags flags, string message, RPCMode mode, byte[] data)
    {
        this.EnsureCall().NGCViewRPC(flags, mode, this, this.prefab.MessageIndex(message), data, 0, (data != null) ? data.Length : 0);
    }

    public void RPC_Stream(string message, NetworkPlayer target, BitStream data)
    {
        this.RPC_Bytes(message, target, data.GetDataByteArray());
    }

    public void RPC_Stream(string message, RPCMode mode, BitStream data)
    {
        this.RPC_Bytes(message, mode, data.GetDataByteArray());
    }

    public void RPC_Stream(string message, IEnumerable<NetworkPlayer> target, BitStream data)
    {
        this.RPC_Bytes(message, target, data.GetDataByteArray());
    }

    public void RPC_Stream(NetworkFlags flags, string message, NetworkPlayer target, BitStream data)
    {
        this.RPC_Bytes(flags, message, target, data.GetDataByteArray());
    }

    public void RPC_Stream(NetworkFlags flags, string message, RPCMode mode, BitStream data)
    {
        this.RPC_Bytes(flags, message, mode, data.GetDataByteArray());
    }

    public void RPC_Stream(NetworkFlags flags, string message, IEnumerable<NetworkPlayer> target, BitStream data)
    {
        this.RPC_Bytes(flags, message, target, data.GetDataByteArray());
    }

    private static BitStream ToStream<T>(T arg)
    {
        BitStream stream = new BitStream(false);
        stream.Write<T>(arg, new object[0]);
        return stream;
    }

    public Vector3 creationPosition
    {
        get
        {
            return this.spawnPosition;
        }
    }

    public Quaternion creationRotation
    {
        get
        {
            return this.spawnRotation;
        }
    }

    public NetEntityID entityID
    {
        get
        {
            return new NetEntityID(this);
        }
    }

    internal int id
    {
        get
        {
            if ((this.innerID > 0) && (this.outer != null))
            {
                return NGC.PackID(this.outer.groupNumber, this.innerID);
            }
            return 0;
        }
    }
}

