using Facepunch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public static class NetCull
{
    public const bool canDestroy = false;
    public const bool canRemoveRPCs = false;
    private static readonly NetworkInstantiator.Destroyer destroyerFreeViewIDOnly = new NetworkInstantiator.Destroyer(NetCull.FreeViewIDOnly_Destroyer);
    private const bool ensureCanDestroy = false;
    private const bool ensureCanRemoveRPCS = false;
    public const bool kClient = true;
    public const bool kServer = false;

    public static void CloseConnection(NetworkPlayer target, bool sendDisconnectionNotification)
    {
        Network.CloseConnection(target, sendDisconnectionNotification, 3);
    }

    public static NetError Connect(string host, int remotePort, string password, params object[] loginData)
    {
        return Network.Connect(host, remotePort, password, loginData).ToNetError();
    }

    public static void Disconnect()
    {
        Network.Disconnect();
    }

    public static void Disconnect(int timeout)
    {
        Network.Disconnect(timeout);
    }

    public static void DisconnectImmediate()
    {
        Network.DisconnectImmediate();
    }

    public static void DontDestroyWithNetwork(MonoBehaviour behaviour)
    {
        if (behaviour != null)
        {
            DontDestroyWithNetwork((NetworkView) behaviour.networkView);
        }
    }

    public static void DontDestroyWithNetwork(NetworkView view)
    {
        if (view != null)
        {
            view.instantiator.destroyer = destroyerFreeViewIDOnly;
        }
    }

    public static void DontDestroyWithNetwork(Component component)
    {
        if (component != null)
        {
            DontDestroyWithNetwork(component.GetComponent<uLinkNetworkView>());
        }
    }

    public static void DontDestroyWithNetwork(GameObject go)
    {
        if (go != null)
        {
            DontDestroyWithNetwork(go.GetComponent<uLinkNetworkView>());
        }
    }

    public static bool Found(this PrefabSearch search)
    {
        return (((int) search) != 0);
    }

    private static void FreeViewIDOnly_Destroyer(NetworkView instance)
    {
    }

    public static bool IsNet(this PrefabSearch search)
    {
        return (((int) search) > 1);
    }

    public static bool IsNetAutoPrefab(this PrefabSearch search)
    {
        return (((int) search) == 3);
    }

    public static bool IsNetMainPrefab(this PrefabSearch search)
    {
        return (((int) search) == 2);
    }

    public static bool IsNGC(this PrefabSearch search)
    {
        return (((int) search) == 1);
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        GameObject obj2;
        if (((int) LoadPrefab(prefabName, out obj2)) == 0)
        {
            throw new MissingReferenceException(prefabName);
        }
        return obj2;
    }

    public static PrefabSearch LoadPrefab(string prefabName, out GameObject prefab)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            prefab = null;
            return PrefabSearch.Missing;
        }
        if (prefabName.StartsWith(":"))
        {
            try
            {
                prefab = NetMainPrefab.Lookup<GameObject>(prefabName);
                return ((prefab == null) ? PrefabSearch.Missing : PrefabSearch.NetMain);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                prefab = null;
                return PrefabSearch.Missing;
            }
        }
        if (prefabName.StartsWith(";"))
        {
            try
            {
                NGC.Prefab prefab2;
                if (!NGC.Prefab.Register.Find(prefabName, out prefab2))
                {
                    prefab = null;
                    return PrefabSearch.Missing;
                }
                NGCView view = prefab2.prefab;
                if (view != null)
                {
                    prefab = view.gameObject;
                    return ((prefab == null) ? PrefabSearch.Missing : PrefabSearch.NGC);
                }
                prefab = null;
                return PrefabSearch.Missing;
            }
            catch (Exception exception2)
            {
                Debug.LogException(exception2);
                prefab = null;
                return PrefabSearch.Missing;
            }
        }
        try
        {
            uLinkNetworkView view2;
            if ((!AutoPrefabs.all.TryGetValue(prefabName, out view2) || (view2 == null)) || ((prefab = view2.gameObject) == null))
            {
                prefab = null;
                return PrefabSearch.Missing;
            }
            return PrefabSearch.NetAuto;
        }
        catch (Exception exception3)
        {
            Debug.LogException(exception3);
            prefab = null;
            return PrefabSearch.Missing;
        }
    }

    public static T LoadPrefabComponent<T>(string prefabName) where T: Component
    {
        T local;
        if (((int) LoadPrefabComponent<T>(prefabName, out local)) == 0)
        {
            throw new MissingReferenceException(prefabName);
        }
        return local;
    }

    public static PrefabSearch LoadPrefabComponent<T>(string prefabName, out T component) where T: Component
    {
        MonoBehaviour behaviour;
        PrefabSearch missing = LoadPrefabView(prefabName, out behaviour);
        if (((int) missing) == 0)
        {
            component = null;
            return missing;
        }
        if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T)) && (behaviour is T))
        {
            component = (T) behaviour;
            return missing;
        }
        component = behaviour.GetComponent<T>();
        if (((T) component) == null)
        {
            missing = PrefabSearch.Missing;
        }
        return missing;
    }

    public static T LoadPrefabScript<T>(string prefabName) where T: MonoBehaviour
    {
        T local;
        if (((int) LoadPrefabScript<T>(prefabName, out local)) == 0)
        {
            throw new MissingReferenceException(prefabName);
        }
        return local;
    }

    public static PrefabSearch LoadPrefabScript<T>(string prefabName, out T script) where T: MonoBehaviour
    {
        MonoBehaviour behaviour;
        PrefabSearch missing = LoadPrefabView(prefabName, out behaviour);
        if (((int) missing) == 0)
        {
            script = null;
            return missing;
        }
        if (behaviour is T)
        {
            script = (T) behaviour;
            return missing;
        }
        script = behaviour.GetComponent<T>();
        if (((T) script) == null)
        {
            missing = PrefabSearch.Missing;
        }
        return missing;
    }

    public static MonoBehaviour LoadPrefabView(string prefabName)
    {
        MonoBehaviour behaviour;
        if (((int) LoadPrefabView(prefabName, out behaviour)) == 0)
        {
            throw new MissingReferenceException(prefabName);
        }
        return behaviour;
    }

    public static PrefabSearch LoadPrefabView(string prefabName, out MonoBehaviour prefabView)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            prefabView = null;
            return PrefabSearch.Missing;
        }
        if (prefabName.StartsWith(":"))
        {
            try
            {
                prefabView = NetMainPrefab.Lookup<uLinkNetworkView>(prefabName);
                return ((prefabView == null) ? PrefabSearch.Missing : PrefabSearch.NetMain);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                prefabView = null;
                return PrefabSearch.Missing;
            }
        }
        if (prefabName.StartsWith(";"))
        {
            try
            {
                NGC.Prefab prefab;
                if (!NGC.Prefab.Register.Find(prefabName, out prefab) || ((prefabView = prefab.prefab) == null))
                {
                    prefabView = null;
                    return PrefabSearch.Missing;
                }
                return PrefabSearch.NGC;
            }
            catch (Exception exception2)
            {
                Debug.LogException(exception2);
                prefabView = null;
                return PrefabSearch.Missing;
            }
        }
        try
        {
            uLinkNetworkView view;
            if (!AutoPrefabs.all.TryGetValue(prefabName, out view) || (view == null))
            {
                prefabView = view;
                return PrefabSearch.Missing;
            }
            prefabView = null;
            return PrefabSearch.NetAuto;
        }
        catch (Exception exception3)
        {
            Debug.LogException(exception3);
            prefabView = null;
            return PrefabSearch.Missing;
        }
    }

    public static bool Missing(this PrefabSearch search)
    {
        return (((int) search) == 0);
    }

    private static void OnPostUpdatePostCallbacks()
    {
        Interpolator.SyncronizeAll();
        CharacterInterpolatorBase.SyncronizeAll();
    }

    private static void OnPostUpdatePreCallbacks()
    {
    }

    private static void OnPreUpdatePostCallbacks()
    {
    }

    private static void OnPreUpdatePreCallbacks()
    {
    }

    public static void RegisterNetAutoPrefab(uLinkNetworkView viewPrefab)
    {
        if (viewPrefab != null)
        {
            string name = viewPrefab.name;
            try
            {
                AutoPrefabs.all[name] = viewPrefab;
            }
            catch
            {
                Debug.LogError("skipped duplicate prefab named " + name, viewPrefab);
                return;
            }
            NetworkInstantiator.AddPrefab(viewPrefab.gameObject);
        }
    }

    public static void ResynchronizeClock(double durationInSeconds)
    {
        Network.ResynchronizeClock(durationInSeconds);
    }

    [Obsolete("void NetCull.ResynchronizeClock(ulong) is deprecated, Bla bla bla don't use this", true)]
    public static void ResynchronizeClock(ulong intervalMillis)
    {
        Network.ResynchronizeClock(intervalMillis);
    }

    public static void RPC(NetworkView view, string messageName, NetworkPlayer target)
    {
        view.RPC(messageName, target, new object[0]);
    }

    public static void RPC(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        view.RPC(messageName, targets, new object[0]);
    }

    public static void RPC(NetworkView view, string messageName, RPCMode rpcMode)
    {
        view.RPC(messageName, rpcMode, new object[0]);
    }

    public static bool RPC(NetEntityID entID, string messageName, NetworkPlayer target)
    {
        if (!entID.isNGC)
        {
            return RPC((NetworkViewID) entID, messageName, target);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC(ngcView, messageName, target);
        return true;
    }

    public static bool RPC(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        if (!entID.isNGC)
        {
            return RPC((NetworkViewID) entID, messageName, targets);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC(ngcView, messageName, targets);
        return true;
    }

    public static bool RPC(NetEntityID entID, string messageName, RPCMode rpcMode)
    {
        if (!entID.isNGC)
        {
            return RPC((NetworkViewID) entID, messageName, rpcMode);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC(ngcView, messageName, rpcMode);
        return true;
    }

    public static void RPC(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        view.RPC(messageName, targets);
    }

    public static void RPC(NGCView view, string messageName, NetworkPlayer target)
    {
        view.RPC(messageName, target);
    }

    public static void RPC(NGCView view, string messageName, RPCMode rpcMode)
    {
        view.RPC(messageName, rpcMode);
    }

    public static bool RPC(NetworkViewID viewID, string messageName, NetworkPlayer target)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC(view, messageName, target);
        return true;
    }

    public static bool RPC(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC(view, messageName, targets);
        return true;
    }

    public static bool RPC(NetworkViewID viewID, string messageName, RPCMode rpcMode)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC(view, messageName, rpcMode);
        return true;
    }

    public static bool RPC(Component entityComponent, string messageName, NetworkPlayer target)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, target);
    }

    public static bool RPC(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, targets);
    }

    public static bool RPC(Component entityComponent, string messageName, RPCMode rpcMode)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, rpcMode);
    }

    public static bool RPC(GameObject entity, string messageName, NetworkPlayer target)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, target);
    }

    public static bool RPC(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, targets);
    }

    public static bool RPC(GameObject entity, string messageName, RPCMode rpcMode)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, rpcMode);
    }

    public static bool RPC(MonoBehaviour entityScript, string messageName, NetworkPlayer target)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, target);
    }

    public static bool RPC(MonoBehaviour entityScript, string messageName, RPCMode rpcMode)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, rpcMode);
    }

    public static bool RPC(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, messageName, targets);
    }

    public static void RPC<P0>(NetworkView view, string messageName, NetworkPlayer target, P0 p0)
    {
        view.RPC<P0>(messageName, target, p0);
    }

    public static void RPC<P0>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        view.RPC<P0>(messageName, targets, p0);
    }

    public static void RPC<P0>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0)
    {
        view.RPC<P0>(messageName, rpcMode, p0);
    }

    public static void RPC(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target)
    {
        view.RPC(flags, messageName, target, new object[0]);
    }

    public static void RPC(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode)
    {
        view.RPC(flags, messageName, rpcMode, new object[0]);
    }

    public static void RPC(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        view.RPC(flags, messageName, targets, new object[0]);
    }

    public static bool RPC<P0>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0)
    {
        if (!entID.isNGC)
        {
            return RPC<P0>((NetworkViewID) entID, messageName, target, p0);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0>(ngcView, messageName, target, p0);
        return true;
    }

    public static bool RPC<P0>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0)
    {
        if (!entID.isNGC)
        {
            return RPC<P0>((NetworkViewID) entID, messageName, rpcMode, p0);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0>(ngcView, messageName, rpcMode, p0);
        return true;
    }

    public static bool RPC<P0>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        if (!entID.isNGC)
        {
            return RPC<P0>((NetworkViewID) entID, messageName, targets, p0);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0>(ngcView, messageName, targets, p0);
        return true;
    }

    public static bool RPC(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target)
    {
        if (!entID.isNGC)
        {
            return RPC((NetworkViewID) entID, flags, messageName, target);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC(ngcView, flags, messageName, target);
        return true;
    }

    public static bool RPC(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        if (!entID.isNGC)
        {
            return RPC((NetworkViewID) entID, flags, messageName, targets);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC(ngcView, flags, messageName, targets);
        return true;
    }

    public static bool RPC(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode)
    {
        if (!entID.isNGC)
        {
            return RPC((NetworkViewID) entID, flags, messageName, rpcMode);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC(ngcView, flags, messageName, rpcMode);
        return true;
    }

    public static void RPC<P0>(NGCView view, string messageName, RPCMode rpcMode, P0 p0)
    {
        view.RPC<P0>(messageName, rpcMode, p0);
    }

    public static void RPC<P0>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        view.RPC<P0>(messageName, targets, p0);
    }

    public static void RPC<P0>(NGCView view, string messageName, NetworkPlayer target, P0 p0)
    {
        view.RPC<P0>(messageName, target, p0);
    }

    public static void RPC(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target)
    {
        view.RPC(flags, messageName, target);
    }

    public static void RPC(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        view.RPC(flags, messageName, targets);
    }

    public static void RPC(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode)
    {
        view.RPC(flags, messageName, rpcMode);
    }

    public static bool RPC<P0>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0>(view, messageName, targets, p0);
        return true;
    }

    public static bool RPC<P0>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0>(view, messageName, target, p0);
        return true;
    }

    public static bool RPC<P0>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0>(view, messageName, rpcMode, p0);
        return true;
    }

    public static bool RPC(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC(view, flags, messageName, targets);
        return true;
    }

    public static bool RPC(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC(view, flags, messageName, target);
        return true;
    }

    public static bool RPC(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC(view, flags, messageName, rpcMode);
        return true;
    }

    public static bool RPC<P0>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, rpcMode, p0);
    }

    public static bool RPC<P0>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, target, p0);
    }

    public static bool RPC<P0>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, targets, p0);
    }

    public static bool RPC(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, targets);
    }

    public static bool RPC(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, target);
    }

    public static bool RPC(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, rpcMode);
    }

    public static bool RPC<P0>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, targets, p0);
    }

    public static bool RPC<P0>(GameObject entity, string messageName, NetworkPlayer target, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, target, p0);
    }

    public static bool RPC<P0>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, rpcMode, p0);
    }

    public static bool RPC(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, target);
    }

    public static bool RPC(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, rpcMode);
    }

    public static bool RPC(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, targets);
    }

    public static bool RPC<P0>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, target, p0);
    }

    public static bool RPC<P0>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, targets, p0);
    }

    public static bool RPC<P0>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, messageName, rpcMode, p0);
    }

    public static bool RPC(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, target);
    }

    public static bool RPC(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, targets);
    }

    public static bool RPC(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC(yid, flags, messageName, rpcMode);
    }

    public static void RPC<P0, P1>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        object[] args = new object[] { p0, p1 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        object[] args = new object[] { p0, p1 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        object[] args = new object[] { p0, p1 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        view.RPC<P0>(flags, messageName, targets, p0);
    }

    public static void RPC<P0>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0)
    {
        view.RPC<P0>(flags, messageName, target, p0);
    }

    public static void RPC<P0>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0)
    {
        view.RPC<P0>(flags, messageName, rpcMode, p0);
    }

    public static bool RPC<P0, P1>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1>((NetworkViewID) entID, messageName, targets, p0, p1);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1>(ngcView, messageName, targets, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1>((NetworkViewID) entID, messageName, target, p0, p1);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1>(ngcView, messageName, target, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1>((NetworkViewID) entID, messageName, rpcMode, p0, p1);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1>(ngcView, messageName, rpcMode, p0, p1);
        return true;
    }

    public static bool RPC<P0>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0)
    {
        if (!entID.isNGC)
        {
            return RPC<P0>((NetworkViewID) entID, flags, messageName, target, p0);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0>(ngcView, flags, messageName, target, p0);
        return true;
    }

    public static bool RPC<P0>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0)
    {
        if (!entID.isNGC)
        {
            return RPC<P0>((NetworkViewID) entID, flags, messageName, rpcMode, p0);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0>(ngcView, flags, messageName, rpcMode, p0);
        return true;
    }

    public static bool RPC<P0>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        if (!entID.isNGC)
        {
            return RPC<P0>((NetworkViewID) entID, flags, messageName, targets, p0);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0>(ngcView, flags, messageName, targets, p0);
        return true;
    }

    public static void RPC<P0, P1>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        view.RPC<P0, P1>(messageName, target, p0, p1);
    }

    public static void RPC<P0, P1>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        view.RPC<P0, P1>(messageName, rpcMode, p0, p1);
    }

    public static void RPC<P0, P1>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        view.RPC<P0, P1>(messageName, targets, p0, p1);
    }

    public static void RPC<P0>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0)
    {
        view.RPC<P0>(flags, messageName, target, p0);
    }

    public static void RPC<P0>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0)
    {
        view.RPC<P0>(flags, messageName, rpcMode, p0);
    }

    public static void RPC<P0>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        view.RPC<P0>(flags, messageName, targets, p0);
    }

    public static bool RPC<P0, P1>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1>(view, messageName, target, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1>(view, messageName, rpcMode, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1>(view, messageName, targets, p0, p1);
        return true;
    }

    public static bool RPC<P0>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0>(view, flags, messageName, target, p0);
        return true;
    }

    public static bool RPC<P0>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0>(view, flags, messageName, targets, p0);
        return true;
    }

    public static bool RPC<P0>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0>(view, flags, messageName, rpcMode, p0);
        return true;
    }

    public static bool RPC<P0, P1>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, target, p0, p1);
    }

    public static bool RPC<P0, P1>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, targets, p0, p1);
    }

    public static bool RPC<P0, P1>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, rpcMode, p0, p1);
    }

    public static bool RPC<P0>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, target, p0);
    }

    public static bool RPC<P0>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, targets, p0);
    }

    public static bool RPC<P0>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, rpcMode, p0);
    }

    public static bool RPC<P0, P1>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, target, p0, p1);
    }

    public static bool RPC<P0, P1>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, targets, p0, p1);
    }

    public static bool RPC<P0, P1>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, rpcMode, p0, p1);
    }

    public static bool RPC<P0>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, targets, p0);
    }

    public static bool RPC<P0>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, target, p0);
    }

    public static bool RPC<P0>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, rpcMode, p0);
    }

    public static bool RPC<P0, P1>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, target, p0, p1);
    }

    public static bool RPC<P0, P1>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, targets, p0, p1);
    }

    public static bool RPC<P0, P1>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, messageName, rpcMode, p0, p1);
    }

    public static bool RPC<P0>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, targets, p0);
    }

    public static bool RPC<P0>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, target, p0);
    }

    public static bool RPC<P0>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0>(yid, flags, messageName, rpcMode, p0);
    }

    public static void RPC<P0, P1, P2>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        object[] args = new object[] { p0, p1, p2 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        object[] args = new object[] { p0, p1, p2 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        object[] args = new object[] { p0, p1, p2 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        object[] args = new object[] { p0, p1 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        object[] args = new object[] { p0, p1 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        object[] args = new object[] { p0, p1 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static bool RPC<P0, P1, P2>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2>((NetworkViewID) entID, messageName, targets, p0, p1, p2);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(ngcView, messageName, targets, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2>((NetworkViewID) entID, messageName, target, p0, p1, p2);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(ngcView, messageName, target, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(ngcView, messageName, rpcMode, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1>((NetworkViewID) entID, flags, messageName, target, p0, p1);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1>(ngcView, flags, messageName, target, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1>(ngcView, flags, messageName, rpcMode, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1>((NetworkViewID) entID, flags, messageName, targets, p0, p1);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1>(ngcView, flags, messageName, targets, p0, p1);
        return true;
    }

    public static void RPC<P0, P1, P2>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        view.RPC<P0, P1, P2>(messageName, targets, p0, p1, p2);
    }

    public static void RPC<P0, P1, P2>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        view.RPC<P0, P1, P2>(messageName, target, p0, p1, p2);
    }

    public static void RPC<P0, P1, P2>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        view.RPC<P0, P1, P2>(messageName, rpcMode, p0, p1, p2);
    }

    public static void RPC<P0, P1>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        view.RPC<P0, P1>(flags, messageName, target, p0, p1);
    }

    public static void RPC<P0, P1>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        view.RPC<P0, P1>(flags, messageName, rpcMode, p0, p1);
    }

    public static void RPC<P0, P1>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        view.RPC<P0, P1>(flags, messageName, targets, p0, p1);
    }

    public static bool RPC<P0, P1, P2>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(view, messageName, targets, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(view, messageName, target, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(view, messageName, rpcMode, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1>(view, flags, messageName, target, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1>(view, flags, messageName, rpcMode, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1>(view, flags, messageName, targets, p0, p1);
        return true;
    }

    public static bool RPC<P0, P1, P2>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, targets, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, target, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, rpcMode, p0, p1, p2);
    }

    public static bool RPC<P0, P1>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, target, p0, p1);
    }

    public static bool RPC<P0, P1>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, rpcMode, p0, p1);
    }

    public static bool RPC<P0, P1>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, targets, p0, p1);
    }

    public static bool RPC<P0, P1, P2>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, rpcMode, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, target, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, targets, p0, p1, p2);
    }

    public static bool RPC<P0, P1>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, target, p0, p1);
    }

    public static bool RPC<P0, P1>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, targets, p0, p1);
    }

    public static bool RPC<P0, P1>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, rpcMode, p0, p1);
    }

    public static bool RPC<P0, P1, P2>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, target, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, targets, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, messageName, rpcMode, p0, p1, p2);
    }

    public static bool RPC<P0, P1>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, target, p0, p1);
    }

    public static bool RPC<P0, P1>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, rpcMode, p0, p1);
    }

    public static bool RPC<P0, P1>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1>(yid, flags, messageName, targets, p0, p1);
    }

    public static void RPC<P0, P1, P2, P3>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        object[] args = new object[] { p0, p1, p2, p3 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        object[] args = new object[] { p0, p1, p2, p3 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        object[] args = new object[] { p0, p1, p2, p3 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        object[] args = new object[] { p0, p1, p2 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        object[] args = new object[] { p0, p1, p2 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1, P2>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        object[] args = new object[] { p0, p1, p2 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static bool RPC<P0, P1, P2, P3>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(ngcView, messageName, target, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(ngcView, messageName, rpcMode, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(ngcView, messageName, targets, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(ngcView, flags, messageName, target, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(ngcView, flags, messageName, targets, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(ngcView, flags, messageName, rpcMode, p0, p1, p2);
        return true;
    }

    public static void RPC<P0, P1, P2, P3>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        view.RPC<P0, P1, P2, P3>(messageName, target, p0, p1, p2, p3);
    }

    public static void RPC<P0, P1, P2, P3>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        view.RPC<P0, P1, P2, P3>(messageName, rpcMode, p0, p1, p2, p3);
    }

    public static void RPC<P0, P1, P2, P3>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        view.RPC<P0, P1, P2, P3>(messageName, targets, p0, p1, p2, p3);
    }

    public static void RPC<P0, P1, P2>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        view.RPC<P0, P1, P2>(flags, messageName, targets, p0, p1, p2);
    }

    public static void RPC<P0, P1, P2>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        view.RPC<P0, P1, P2>(flags, messageName, target, p0, p1, p2);
    }

    public static void RPC<P0, P1, P2>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        view.RPC<P0, P1, P2>(flags, messageName, rpcMode, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2, P3>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(view, messageName, target, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(view, messageName, rpcMode, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(view, messageName, targets, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(view, flags, messageName, target, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(view, flags, messageName, targets, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2>(view, flags, messageName, rpcMode, p0, p1, p2);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, targets, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, target, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, rpcMode, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, rpcMode, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, target, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, targets, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2, P3>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, targets, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, target, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, rpcMode, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, target, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, rpcMode, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, targets, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2, P3>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, target, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, targets, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, messageName, rpcMode, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, target, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, targets, p0, p1, p2);
    }

    public static bool RPC<P0, P1, P2>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2>(yid, flags, messageName, rpcMode, p0, p1, p2);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        object[] args = new object[] { p0, p1, p2, p3 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        object[] args = new object[] { p0, p1, p2, p3 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        object[] args = new object[] { p0, p1, p2, p3 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(ngcView, messageName, targets, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(ngcView, messageName, target, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(ngcView, flags, messageName, target, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(ngcView, flags, messageName, targets, p0, p1, p2, p3);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        view.RPC<P0, P1, P2, P3, P4>(messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        view.RPC<P0, P1, P2, P3, P4>(messageName, target, p0, p1, p2, p3, p4);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        view.RPC<P0, P1, P2, P3, P4>(messageName, targets, p0, p1, p2, p3, p4);
    }

    public static void RPC<P0, P1, P2, P3>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        view.RPC<P0, P1, P2, P3>(flags, messageName, targets, p0, p1, p2, p3);
    }

    public static void RPC<P0, P1, P2, P3>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        view.RPC<P0, P1, P2, P3>(flags, messageName, target, p0, p1, p2, p3);
    }

    public static void RPC<P0, P1, P2, P3>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        view.RPC<P0, P1, P2, P3>(flags, messageName, rpcMode, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(view, messageName, target, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(view, messageName, rpcMode, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(view, messageName, targets, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(view, flags, messageName, target, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(view, flags, messageName, rpcMode, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3>(view, flags, messageName, targets, p0, p1, p2, p3);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, targets, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, target, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, targets, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, target, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, rpcMode, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, target, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, targets, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, targets, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, target, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, rpcMode, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, targets, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, target, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, target, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, targets, p0, p1, p2, p3);
    }

    public static bool RPC<P0, P1, P2, P3>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3>(yid, flags, messageName, rpcMode, p0, p1, p2, p3);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4 };
        view.RPC(flags, messageName, target, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4, p5);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(ngcView, messageName, targets, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4, p5);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(ngcView, messageName, target, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        view.RPC<P0, P1, P2, P3, P4, P5>(messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        view.RPC<P0, P1, P2, P3, P4, P5>(messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        view.RPC<P0, P1, P2, P3, P4, P5>(messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        view.RPC<P0, P1, P2, P3, P4>(flags, messageName, targets, p0, p1, p2, p3, p4);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        view.RPC<P0, P1, P2, P3, P4>(flags, messageName, target, p0, p1, p2, p3, p4);
    }

    public static void RPC<P0, P1, P2, P3, P4>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        view.RPC<P0, P1, P2, P3, P4>(flags, messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(view, messageName, targets, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(view, messageName, target, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(view, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(view, flags, messageName, targets, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(view, flags, messageName, target, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, targets, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, target, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, target, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, targets, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, target, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4);
    }

    public static bool RPC<P0, P1, P2, P3, P4>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4>(yid, flags, messageName, targets, p0, p1, p2, p3, p4);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5 };
        view.RPC(flags, messageName, targets, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(ngcView, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4, p5, p6);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(ngcView, messageName, target, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4, p5);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4, p5);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6>(messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6>(messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6>(messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        view.RPC<P0, P1, P2, P3, P4, P5>(flags, messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        view.RPC<P0, P1, P2, P3, P4, P5>(flags, messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        view.RPC<P0, P1, P2, P3, P4, P5>(flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(view, messageName, target, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(view, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(view, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(view, flags, messageName, target, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5>(view, flags, messageName, targets, p0, p1, p2, p3, p4, p5);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6 };
        view.RPC(flags, messageName, targets, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(ngcView, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(ngcView, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6>(flags, messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6>(flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6>(flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(view, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(view, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(view, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(view, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(view, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        view.RPC(flags, messageName, targets, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(ngcView, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(ngcView, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7>(flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(view, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(view, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(view, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(view, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7>(view, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(ngcView, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(ngcView, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(view, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(view, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(view, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(view, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(view, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(ngcView, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(ngcView, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(view, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(view, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(view, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(view, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(view, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 };
        view.RPC(messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 };
        view.RPC(messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 };
        view.RPC(messageName, rpcMode, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetEntityID entID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>((NetworkViewID) entID, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(ngcView, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetEntityID entID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>((NetworkViewID) entID, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(ngcView, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetEntityID entID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>((NetworkViewID) entID, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(ngcView, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NGCView view, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NGCView view, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NGCView view, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkViewID viewID, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(view, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkViewID viewID, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(view, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkViewID viewID, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(view, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(view, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(view, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(Component entityComponent, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(Component entityComponent, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(Component entityComponent, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(GameObject entity, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(GameObject entity, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(GameObject entity, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(MonoBehaviour entityScript, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(MonoBehaviour entityScript, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(MonoBehaviour entityScript, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 };
        view.RPC(flags, messageName, target, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 };
        view.RPC(flags, messageName, targets, args);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        object[] args = new object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11 };
        view.RPC(flags, messageName, rpcMode, args);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetEntityID entID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>((NetworkViewID) entID, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(ngcView, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetEntityID entID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>((NetworkViewID) entID, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(ngcView, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetEntityID entID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        if (!entID.isNGC)
        {
            return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>((NetworkViewID) entID, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }
        NGCView ngcView = entID.ngcView;
        if (ngcView == null)
        {
            Debug.LogError(string.Format("No NGC View with id {0} to send RPC \"{1}\"", entID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(ngcView, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NGCView view, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NGCView view, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static void RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NGCView view, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        view.RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkViewID viewID, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(view, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkViewID viewID, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(view, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(NetworkViewID viewID, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetworkView view = NetworkView.Find(viewID);
        if (view == null)
        {
            Debug.LogError(string.Format("No Network View with id {0} to send RPC \"{1}\"", viewID, messageName));
            return false;
        }
        RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(view, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        return true;
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(Component entityComponent, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(Component entityComponent, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(Component entityComponent, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityComponent, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(GameObject entity, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(GameObject entity, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(GameObject entity, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entity, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, IEnumerable<NetworkPlayer> targets, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, targets, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, NetworkPlayer target, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, target, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    public static bool RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(MonoBehaviour entityScript, NetworkFlags flags, string messageName, RPCMode rpcMode, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11)
    {
        NetEntityID yid;
        if (((int) NetEntityID.Of(entityScript, out yid)) == 0)
        {
            return false;
        }
        return RPC<P0, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(yid, flags, messageName, rpcMode, p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
    }

    [Conditional("SERVER")]
    public static void VerifyRPC(ref NetworkMessageInfo info, bool skipOwnerCheck = false)
    {
    }

    public static BitStream approvalData
    {
        get
        {
            return Network.approvalData;
        }
    }

    public static NetworkConfig config
    {
        get
        {
            return Network.config;
        }
    }

    public static NetworkPlayer[] connections
    {
        get
        {
            return Network.connections;
        }
    }

    [Obsolete("Use #if CLIENT (unless your trying to check if the client is connected.. then use NetCull.isClientRunning")]
    public static bool isClient
    {
        get
        {
            return isClientRunning;
        }
    }

    public static bool isClientRunning
    {
        get
        {
            return Network.isClient;
        }
    }

    public static bool isMessageQueueRunning
    {
        get
        {
            return Network.isMessageQueueRunning;
        }
        set
        {
            Network.isMessageQueueRunning = value;
        }
    }

    public static bool isNotRunning
    {
        get
        {
            return (!Network.isClient && !Network.isServer);
        }
    }

    public static bool isRunning
    {
        get
        {
            return (Network.isClient || Network.isServer);
        }
    }

    [Obsolete("Use #if SERVER (unless your trying to check if the server is running.. then use NetCull.isServerRunning")]
    public static bool isServer
    {
        get
        {
            return isServerRunning;
        }
    }

    public static bool isServerRunning
    {
        get
        {
            return Network.isServer;
        }
    }

    public static NetError lastError
    {
        get
        {
            return Network.lastError.ToNetError();
        }
        set
        {
            Network.lastError = value._uLink();
        }
    }

    public static int listenPort
    {
        get
        {
            return Network.listenPort;
        }
    }

    public static double localTime
    {
        get
        {
            return Network.localTime;
        }
    }

    public static ulong localTimeInMillis
    {
        get
        {
            return Network.localTimeInMillis;
        }
    }

    public static NetworkPlayer player
    {
        get
        {
            return Network.player;
        }
    }

    public static double sendInterval
    {
        get
        {
            return Send.Interval;
        }
    }

    public static float sendIntervalF
    {
        get
        {
            return Send.IntervalF;
        }
    }

    public static float sendRate
    {
        get
        {
            return Network.sendRate;
        }
        set
        {
            Network.sendRate = value;
            Send.Rate = Network.sendRate;
            Send.Interval = 1.0 / ((double) Send.Rate);
            Send.IntervalF = (float) Send.Interval;
            Interpolation.sendRate = Send.Rate;
        }
    }

    public static NetworkStatus status
    {
        get
        {
            return Network.status;
        }
    }

    public static double time
    {
        get
        {
            return Network.time;
        }
    }

    public static ulong timeInMillis
    {
        get
        {
            return Network.timeInMillis;
        }
    }

    private static class AutoPrefabs
    {
        public static Dictionary<string, uLinkNetworkView> all = new Dictionary<string, uLinkNetworkView>();
    }

    public static class Callbacks
    {
        private static InternalHelper internalHelper;
        private static bool MADE_POST;
        private static bool MADE_PRE;
        private static NetPostUpdate netPostUpdate;
        private static NetPreUpdate netPreUpdate;

        public static  event NetCull.UpdateFunctor afterEveryUpdate
        {
            add
            {
                POST.DELEGATE.Add(value, false);
            }
            remove
            {
                if (MADE_POST)
                {
                    POST.DELEGATE.Remove(value);
                }
            }
        }

        public static  event NetCull.UpdateFunctor afterNextUpdate
        {
            add
            {
                POST.DELEGATE.Add(value, true);
            }
            remove
            {
                if (MADE_POST)
                {
                    POST.DELEGATE.Remove(value);
                }
            }
        }

        public static  event NetCull.UpdateFunctor beforeEveryUpdate
        {
            add
            {
                PRE.DELEGATE.Add(value, false);
            }
            remove
            {
                if (MADE_PRE)
                {
                    PRE.DELEGATE.Remove(value);
                }
            }
        }

        public static  event NetCull.UpdateFunctor beforeNextUpdate
        {
            add
            {
                PRE.DELEGATE.Add(value, true);
            }
            remove
            {
                if (MADE_PRE)
                {
                    PRE.DELEGATE.Remove(value);
                }
            }
        }

        internal static void BindUpdater(NetPostUpdate netUpdate)
        {
            Replace<NetPostUpdate>(ref netPostUpdate, netUpdate);
        }

        internal static void BindUpdater(NetPreUpdate netUpdate)
        {
            Replace<NetPreUpdate>(ref netPreUpdate, netUpdate);
        }

        internal static void FirePostUpdate(NetPostUpdate postUpdate)
        {
            if ((postUpdate == netPostUpdate) && Updating())
            {
                NetCull.OnPostUpdatePreCallbacks();
                if (MADE_POST)
                {
                    try
                    {
                        POST.DELEGATE.Invoke();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, postUpdate);
                    }
                }
                NetCull.OnPostUpdatePostCallbacks();
            }
        }

        internal static void FirePreUpdate(NetPreUpdate preUpdate)
        {
            if ((preUpdate == netPreUpdate) && Updating())
            {
                NetCull.OnPreUpdatePreCallbacks();
                if (MADE_PRE)
                {
                    try
                    {
                        PRE.DELEGATE.Invoke();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, preUpdate);
                    }
                }
                NetCull.OnPreUpdatePostCallbacks();
            }
        }

        private static void Replace<T>(ref T current, T replacement) where T: MonoBehaviour
        {
            if (((T) current) != replacement)
            {
                if (((T) current) != null)
                {
                    Debug.LogWarning(((replacement == null) ? "Destroying " : "Replacing ") + typeof(T), current.gameObject);
                    T local = current;
                    Resign<T>(ref current, current);
                    if (local != null)
                    {
                        Object.Destroy(local);
                    }
                    if (replacement != null)
                    {
                        Debug.LogWarning("With " + typeof(T), replacement);
                    }
                }
                current = replacement;
            }
        }

        private static void Resign<T>(ref T current, T resigning) where T: MonoBehaviour
        {
            if (((T) current) == resigning)
            {
                current = null;
            }
        }

        internal static void ResignUpdater(NetPostUpdate netUpdate)
        {
            Resign<NetPostUpdate>(ref netPostUpdate, netUpdate);
        }

        internal static void ResignUpdater(NetPreUpdate netUpdate)
        {
            Resign<NetPreUpdate>(ref netPreUpdate, netUpdate);
        }

        private static bool Updating()
        {
            if (internalHelper == null)
            {
                GameObject obj2 = GameObject.Find("uLinkInternalHelper");
                if (obj2 == null)
                {
                    return false;
                }
                internalHelper = obj2.GetComponent<InternalHelper>();
                if (internalHelper == null)
                {
                    return false;
                }
            }
            return internalHelper.enabled;
        }

        private static class POST
        {
            public static readonly NetCull.Callbacks.UpdateDelegate DELEGATE = new NetCull.Callbacks.UpdateDelegate();

            static POST()
            {
                NetCull.Callbacks.MADE_POST = true;
            }
        }

        private static class PRE
        {
            public static readonly NetCull.Callbacks.UpdateDelegate DELEGATE = new NetCull.Callbacks.UpdateDelegate();

            static PRE()
            {
                NetCull.Callbacks.MADE_PRE = true;
            }
        }

        private class UpdateDelegate
        {
            private int count;
            private bool guarded;
            private readonly HashSet<NetCull.UpdateFunctor> hashSet = new HashSet<NetCull.UpdateFunctor>();
            private readonly List<NetCull.UpdateFunctor> invokation = new List<NetCull.UpdateFunctor>();
            private int iterPosition;
            private readonly List<NetCull.UpdateFunctor> list = new List<NetCull.UpdateFunctor>();
            private readonly HashSet<NetCull.UpdateFunctor> once1 = new HashSet<NetCull.UpdateFunctor>();
            private readonly HashSet<NetCull.UpdateFunctor> once2 = new HashSet<NetCull.UpdateFunctor>();
            private bool onceSwap;
            private readonly HashSet<int> skip = new HashSet<int>();

            public bool Add(NetCull.UpdateFunctor functor, bool oneTimeOnly)
            {
                if (!this.hashSet.Add(functor))
                {
                    return false;
                }
                this.list.Add(functor);
                if (oneTimeOnly)
                {
                    (!this.onceSwap ? this.once1 : this.once2).Add(functor);
                }
                return true;
            }

            private bool HandleRemoval(NetCull.UpdateFunctor functor)
            {
                if (this.guarded)
                {
                    int index = this.invokation.IndexOf(functor);
                    if (index != -1)
                    {
                        this.invokation[index] = null;
                        if (this.iterPosition < index)
                        {
                            this.skip.Add(index);
                            return true;
                        }
                    }
                }
                return false;
            }

            public void Invoke()
            {
                if (!this.guarded && ((this.count = this.list.Count) != 0))
                {
                    this.iterPosition = -1;
                    try
                    {
                        this.guarded = true;
                        this.iterPosition = -1;
                        this.invokation.AddRange(this.list);
                        HashSet<NetCull.UpdateFunctor> other = !this.onceSwap ? this.once1 : this.once2;
                        HashSet<NetCull.UpdateFunctor> set2 = !this.onceSwap ? this.once2 : this.once1;
                        set2.Clear();
                        set2.UnionWith(other);
                        this.onceSwap = !this.onceSwap;
                        foreach (NetCull.UpdateFunctor functor in other)
                        {
                            if (this.hashSet.Remove(functor))
                            {
                                this.list.Remove(functor);
                            }
                        }
                        other.Clear();
                        while (++this.iterPosition < this.count)
                        {
                            if (!this.skip.Remove(this.iterPosition))
                            {
                                NetCull.UpdateFunctor functor2 = this.invokation[this.iterPosition];
                                try
                                {
                                    functor2();
                                    continue;
                                }
                                catch (Exception exception)
                                {
                                    Object target;
                                    try
                                    {
                                        target = functor2.Target as Object;
                                    }
                                    catch
                                    {
                                        target = null;
                                    }
                                    Debug.LogException(exception, target);
                                    continue;
                                }
                            }
                        }
                    }
                    finally
                    {
                        try
                        {
                            this.invokation.Clear();
                        }
                        finally
                        {
                            this.guarded = false;
                        }
                    }
                }
            }

            public bool Remove(NetCull.UpdateFunctor functor)
            {
                if (this.hashSet.Remove(functor))
                {
                    this.list.Remove(functor);
                    (!this.onceSwap ? this.once1 : this.once2).Remove(functor);
                    this.HandleRemoval(functor);
                    return true;
                }
                return ((!this.onceSwap ? this.once1 : this.once2).Remove(functor) && this.HandleRemoval(functor));
            }
        }
    }

    public enum PrefabSearch : sbyte
    {
        Missing = 0,
        NetAuto = 3,
        NetMain = 2,
        NGC = 1
    }

    [Serializable]
    public class RPCVerificationDropException : NetCull.RPCVerificationException
    {
        internal RPCVerificationDropException()
        {
        }
    }

    [Serializable]
    public abstract class RPCVerificationException : Exception
    {
        internal RPCVerificationException()
        {
        }
    }

    [Serializable]
    public class RPCVerificationLateException : NetCull.RPCVerificationDropException
    {
        internal RPCVerificationLateException()
        {
        }
    }

    [Serializable]
    public class RPCVerificationSenderException : NetCull.RPCVerificationException
    {
        public readonly NetworkPlayer Sender;

        internal RPCVerificationSenderException(NetworkPlayer Sender)
        {
            this.Sender = Sender;
        }
    }

    [Serializable]
    public class RPCVerificationWrongSenderException : NetCull.RPCVerificationSenderException
    {
        public readonly NetworkPlayer Owner;

        internal RPCVerificationWrongSenderException(NetworkPlayer Sender, NetworkPlayer Owner) : base(Sender)
        {
            this.Owner = Owner;
        }
    }

    private static class Send
    {
        public static double Interval = (1.0 / ((double) NetCull.sendRate));
        public static float IntervalF = ((float) Interval);
        public static float Rate = Network.sendRate;
    }

    public delegate void UpdateFunctor();
}

