using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct CustomInstantiationArgs
{
    public readonly NetMainPrefab netMain;
    public readonly IDMain prefab;
    public readonly NetworkView prefabNetworkView;
    public readonly NetworkInstantiateArgs args;
    public readonly IPrefabCustomInstantiate customInstantiate;
    public readonly bool server;
    public readonly bool hasCustomInstantiator;
    public CustomInstantiationArgs(NetMainPrefab netMain, IDMain prefab, ref NetworkInstantiateArgs args, bool server) : this(netMain, null, prefab, ref args, server, false)
    {
    }

    public CustomInstantiationArgs(NetMainPrefab netMain, Object customInstantiator, IDMain prefab, ref NetworkInstantiateArgs args, bool server) : this(netMain, customInstantiator, prefab, ref args, server, true)
    {
    }

    private CustomInstantiationArgs(NetMainPrefab netMain, Object customInstantiator, IDMain prefab, ref NetworkInstantiateArgs args, bool server, bool checkCustomInstantitorArgument)
    {
        this.netMain = netMain;
        this.prefab = prefab;
        this.prefabNetworkView = prefab.networkView;
        this.args = args;
        this.server = server;
        if (checkCustomInstantitorArgument && (customInstantiator != null))
        {
            this.customInstantiate = customInstantiator as IPrefabCustomInstantiate;
            if (this.customInstantiate == null)
            {
                this.hasCustomInstantiator = CheckNetworkViewCustomInstantiator(this.prefabNetworkView, this.prefab, out this.customInstantiate);
            }
            else
            {
                this.hasCustomInstantiator = true;
            }
        }
        else
        {
            this.hasCustomInstantiator = CheckNetworkViewCustomInstantiator(this.prefabNetworkView, this.prefab, out this.customInstantiate);
        }
    }

    public BitStream initialData
    {
        get
        {
            return this.args.initialData;
        }
    }
    public Vector3 position
    {
        get
        {
            return this.args.position;
        }
    }
    public Quaternion rotation
    {
        get
        {
            return this.args.rotation;
        }
    }
    public bool client
    {
        get
        {
            return !this.server;
        }
    }
    private static bool CheckNetworkViewCustomInstantiator(NetworkView view, out IPrefabCustomInstantiate custom)
    {
        custom = view.observed as IPrefabCustomInstantiate;
        return (custom != null);
    }

    private static bool CheckNetworkViewCustomInstantiator(IDMain character, out IPrefabCustomInstantiate custom)
    {
        custom = character as IPrefabCustomInstantiate;
        return (custom != null);
    }

    private static bool CheckNetworkViewCustomInstantiator(NetworkView view, IDMain character, out IPrefabCustomInstantiate custom)
    {
        return (CheckNetworkViewCustomInstantiator(view, out custom) || CheckNetworkViewCustomInstantiator(character, out custom));
    }
}

