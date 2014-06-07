using Facepunch;
using System;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

[AddComponentMenu("")]
public sealed class NetInstance : IDLocal
{
    [NonSerialized]
    public CustomInstantiationArgs args;
    private static readonly DisposeCallbackList<NetInstance, CallbackFunction>.Function callbackFire = new DisposeCallbackList<NetInstance, CallbackFunction>.Function(NetInstance.CallbackFire);
    [NonSerialized]
    internal bool destroying;
    [NonSerialized]
    public NetworkMessageInfo info;
    [NonSerialized]
    public bool local;
    [NonSerialized]
    public IDRemote localAppendage;
    [NonSerialized]
    public bool madeLocalAppendage;
    [NonSerialized]
    public NetworkView networkView;
    private DisposeCallbackList<NetInstance, CallbackFunction> postCreate;
    private DisposeCallbackList<NetInstance, CallbackFunction> preCreate;
    private DisposeCallbackList<NetInstance, CallbackFunction> preDestroy;
    [NonSerialized]
    public bool prepared;

    public event CallbackFunction onPostCreate
    {
        add
        {
            this.postCreate.Add(value);
        }
        remove
        {
            this.postCreate.Remove(value);
        }
    }

    public event CallbackFunction onPreCreate
    {
        add
        {
            this.preCreate.Add(value);
        }
        remove
        {
            this.preCreate.Remove(value);
        }
    }

    public event CallbackFunction onPreDestroy
    {
        add
        {
            this.preDestroy.Add(value);
        }
        remove
        {
            this.preDestroy.Remove(value);
        }
    }

    public NetInstance()
    {
        this.preDestroy = new DisposeCallbackList<NetInstance, CallbackFunction>(this, callbackFire);
        this.postCreate = new DisposeCallbackList<NetInstance, CallbackFunction>(this, callbackFire);
        this.preCreate = new DisposeCallbackList<NetInstance, CallbackFunction>(this, callbackFire);
    }

    private static void CallbackFire(NetInstance instance, CallbackFunction func)
    {
        func(instance);
    }

    public static bool IsCurrentlyDestroying(IDLocal local)
    {
        NetInstance current = NetInstance.current;
        return ((current != null) && (current.idMain == local.idMain));
    }

    public static bool IsCurrentlyDestroying(IDMain main)
    {
        NetInstance current = NetInstance.current;
        return ((current != null) && (current.idMain == main));
    }

    public static bool IsCurrentlyDestroying(IDRemote remote)
    {
        NetInstance current = NetInstance.current;
        return ((current != null) && (current.idMain == remote.idMain));
    }

    private void OnDestroy()
    {
        this.postCreate = this.preCreate = this.preDestroy = DisposeCallbackList<NetInstance, CallbackFunction>.invalid;
    }

    internal void zzz___onpostcreate()
    {
        this.postCreate.Dispose();
    }

    internal void zzz___onprecreate()
    {
        this.preCreate.Dispose();
    }

    internal void zzz___onpredestroy()
    {
        this.preDestroy.Dispose();
    }

    public bool clientSide
    {
        get
        {
            return this.args.client;
        }
    }

    public static NetInstance current
    {
        get
        {
            return NetMainPrefab.zzz__currentNetInstance;
        }
    }

    public IPrefabCustomInstantiate customeInstantiateCreator
    {
        get
        {
            return this.args.customInstantiate;
        }
    }

    public bool isProxy
    {
        get
        {
            return ((this.prepared && this.local) && !this.args.server);
        }
    }

    public NetMainPrefab netMain
    {
        get
        {
            return this.args.netMain;
        }
    }

    public IDMain prefab
    {
        get
        {
            return this.args.prefab;
        }
    }

    public NetworkView prefabNetworkView
    {
        get
        {
            return this.args.prefabNetworkView;
        }
    }

    public bool serverSide
    {
        get
        {
            return this.args.server;
        }
    }

    public bool wasCreatedByCustomInstantiate
    {
        get
        {
            return this.args.hasCustomInstantiator;
        }
    }

    public delegate void CallbackFunction(NetInstance instance);
}

