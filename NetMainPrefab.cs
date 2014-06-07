using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class NetMainPrefab : ScriptableObject
{
    private static NetInstance _currentNetInstance;
    [SerializeField]
    private Object _customInstantiator;
    [SerializeField]
    private IDRemote _localAppend;
    [SerializeField]
    private IDMain _localPrefab;
    private string _name;
    private string _originalName;
    [SerializeField]
    private string _pathToLocalAppend;
    [SerializeField]
    private IDMain _proxyPrefab;
    [SerializeField]
    private IDMain _serverPrefab;
    private readonly NetworkInstantiator.Creator creator;
    private readonly NetworkInstantiator.Destroyer destroyer;
    private static bool ginit;
    [NonSerialized]
    public readonly Type MinimumTypeAllowed;
    public const char prefixChar = ':';
    public const string prefixCharString = ":";

    public NetMainPrefab() : this(typeof(IDMain), false)
    {
    }

    protected NetMainPrefab(Type minimumType) : this(minimumType, true)
    {
    }

    private NetMainPrefab(Type minimumType, bool typeCheck)
    {
        if (typeCheck && !typeof(IDMain).IsAssignableFrom(minimumType))
        {
            throw new ArgumentOutOfRangeException("minimumType", "must be assignable to IDMain");
        }
        this.MinimumTypeAllowed = minimumType;
        this.CollectCallbacks(out this.creator, out this.destroyer);
    }

    protected NetworkView _Creator(string prefabName, NetworkInstantiateArgs args, NetworkMessageInfo info)
    {
        NetInstance instance = this.Summon(this.proxyPrefab, false, ref args);
        if (instance == null)
        {
            return null;
        }
        NetworkView networkView = instance.networkView;
        if (networkView == null)
        {
            return null;
        }
        info = new NetworkMessageInfo(info, networkView);
        NetInstance instance2 = _currentNetInstance;
        try
        {
            _currentNetInstance = instance;
            instance.info = info;
            instance.prepared = true;
            instance.local = args.viewID.isMine;
            bool didAppend = false;
            IDRemote appended = null;
            if (instance.local)
            {
                IDRemote localAppend = this.localAppend;
                if (localAppend != null)
                {
                    appended = DoLocalAppend(localAppend, instance.idMain, this.GetLocalAppendTransform(instance.idMain));
                    didAppend = true;
                }
            }
            instance.zzz___onprecreate();
            this.StandardInitialization(didAppend, appended, instance, networkView, ref info);
            instance.zzz___onpostcreate();
        }
        finally
        {
            _currentNetInstance = instance2;
        }
        return networkView;
    }

    protected void _Destroyer(NetworkView networkView)
    {
        NetInstance instance = _currentNetInstance;
        try
        {
            NetInstance component = networkView.GetComponent<NetInstance>();
            _currentNetInstance = component;
            if (component != null)
            {
                component.zzz___onpredestroy();
            }
            Object.Destroy(networkView.gameObject);
        }
        finally
        {
            _currentNetInstance = instance;
        }
    }

    protected virtual void CollectCallbacks(out NetworkInstantiator.Creator creator, out NetworkInstantiator.Destroyer destroyer)
    {
        creator = new NetworkInstantiator.Creator(this._Creator);
        destroyer = new NetworkInstantiator.Destroyer(this._Destroyer);
    }

    private NetworkView Create(ref CustomInstantiationArgs args, out IDMain instance)
    {
        NetworkView view3;
        if ((float.IsNaN(args.position.x) || float.IsNaN(args.position.y)) || float.IsNaN(args.position.z))
        {
            Debug.LogWarning("NetMainPrefab -> Create -  args.position = " + args.position);
            Debug.LogWarning("This means you're creating an object with a bad position!");
        }
        NetInstance instance2 = _currentNetInstance;
        try
        {
            _currentNetInstance = null;
            if (args.hasCustomInstantiator)
            {
                NetworkView networkView;
                instance = null;
                try
                {
                    instance = args.customInstantiate.CustomInstantiatePrefab(ref args);
                }
                catch (Exception exception)
                {
                    Debug.LogError(string.Format("Thrown Exception during custom instantiate via '{0}' with instantiation '{2}'\r\ndefault instantiation will now occur --  exception follows..\r\n{1}", args.customInstantiate, exception, this), this);
                    if (instance != null)
                    {
                        Object.Destroy(instance);
                    }
                    instance = null;
                }
                try
                {
                    networkView = instance.networkView;
                    if (networkView == null)
                    {
                        Debug.LogWarning(string.Format("The custom instantiator '{0}' with instantiation '{1}' did not return a idmain with a network view. so its being added", args.customInstantiate, this), this);
                        networkView = instance.gameObject.AddComponent<uLinkNetworkView>();
                    }
                }
                catch (Exception exception2)
                {
                    networkView = null;
                    Debug.LogError(string.Format("The custom instantiator '{0}' did not instantiate a IDMain with a networkview or something else with instantiation '{2}'.. \r\n {1}", args.customInstantiate, exception2, this), this);
                }
                if (networkView != null)
                {
                    return networkView;
                }
            }
            NetworkView view2 = (NetworkView) NetworkInstantiatorUtility.Instantiate(args.prefabNetworkView, args.args);
            instance = view2.GetComponent<IDMain>();
            view3 = view2;
        }
        finally
        {
            _currentNetInstance = instance2;
        }
        return view3;
    }

    public static IDRemote DoLocalAppend(IDRemote localAppend, IDMain instance, Transform appendPoint)
    {
        Transform transform = localAppend.transform;
        if (localAppend.transform != localAppend.transform.root)
        {
            Debug.LogWarning("The localAppend transform was not a root");
        }
        IDRemote remote = (IDRemote) Object.Instantiate(localAppend, appendPoint.TransformPoint(transform.localPosition), appendPoint.rotation * transform.localRotation);
        Transform transform2 = remote.transform;
        transform2.parent = appendPoint;
        transform2.localPosition = transform.localPosition;
        transform2.localRotation = transform.localRotation;
        transform2.localScale = transform.localScale;
        remote.idMain = instance;
        foreach (IDRemote remote2 in instance.GetComponentsInChildren<IDRemote>())
        {
            if (remote2.idMain == null)
            {
                remote2.idMain = instance;
            }
        }
        return remote;
    }

    public static string DressName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("name cannot be null or empty", "name");
        }
        if (name[0] != ':')
        {
            for (int j = name.Length - 1; j >= 0; j--)
            {
                if (char.IsUpper(name, j))
                {
                    Debug.LogWarning(string.Format("the name \":{0}\" contains upper case characters. it should not.", name));
                    return (":" + name.ToLower());
                }
            }
            return (":" + name);
        }
        int length = name.Length;
        if (length == 1)
        {
            throw new ArgumentException("if name includes the prefix char it must be followed by at least one more char.", "name");
        }
        for (int i = length - 1; i > 0; i--)
        {
            if (char.IsUpper(name, i))
            {
                Debug.LogWarning(string.Format("the name \"{0}\" contains upper case characters. it should not.", name));
                return name.ToLower();
            }
        }
        string str = name.ToLower();
        if (str != name)
        {
            Debug.LogWarning(string.Format("the name \"{0}\" contains upper case characters. it should not.", name));
        }
        if (str[0] == ':')
        {
            return str;
        }
        return (":" + str);
    }

    public static void EnsurePrefabName(string name)
    {
        NetMainPrefabNameException exception;
        if (!ValidatePrefabNameOrMakeException(name, out exception))
        {
            throw exception;
        }
    }

    public Transform GetLocalAppendTransform(IDMain instanceOrPrefab)
    {
        return GetLocalAppendTransform(instanceOrPrefab, this._pathToLocalAppend);
    }

    public static Transform GetLocalAppendTransform(IDMain instanceOrPrefab, string _pathToLocalAppend)
    {
        if (instanceOrPrefab == null)
        {
            return null;
        }
        if (string.IsNullOrEmpty(_pathToLocalAppend))
        {
            return instanceOrPrefab.transform;
        }
        Transform transform = instanceOrPrefab.transform.FindChild(_pathToLocalAppend);
        if (transform == null)
        {
            Debug.LogError("The transform path:\"" + _pathToLocalAppend + "\" is no longer valid for given transform. returning the transform of the main", instanceOrPrefab);
            transform = instanceOrPrefab.transform;
        }
        return transform;
    }

    public static void IssueLocallyAppended(IDRemote appended, IDMain instance)
    {
        appended.BroadcastMessage("OnLocallyAppended", instance, SendMessageOptions.DontRequireReceiver);
    }

    public static T Lookup<T>(string key) where T: Object
    {
        NetMainPrefab prefab;
        if (!ginit)
        {
            return null;
        }
        if (!g.dict.TryGetValue(key, out prefab))
        {
            Debug.LogWarning("There was no registered proxy with key " + key);
            return null;
        }
        if (typeof(NetMainPrefab).IsAssignableFrom(typeof(T)))
        {
            return (T) prefab;
        }
        if (typeof(GameObject).IsAssignableFrom(typeof(T)))
        {
            return (T) prefab.prefab.gameObject;
        }
        if (!typeof(Component).IsAssignableFrom(typeof(T)))
        {
            return null;
        }
        if (typeof(IDMain).IsAssignableFrom(typeof(T)))
        {
            return (T) prefab.prefab;
        }
        return (T) prefab.prefab.GetComponent(typeof(T));
    }

    public static T LookupInChildren<T>(string key) where T: Component
    {
        NetMainPrefab prefab;
        if (!ginit)
        {
            return null;
        }
        if (!g.dict.TryGetValue(key, out prefab))
        {
            Debug.LogWarning("There was no registered proxy with key " + key);
            return null;
        }
        return prefab.prefab.GetComponentInChildren<T>();
    }

    public void Register(bool forceReplace)
    {
        NetworkInstantiator.Add(this.name, this.creator, this.destroyer, forceReplace);
        g.dict[this.name] = this;
    }

    private bool ShouldDoStandardInitialization(NetInstance instance)
    {
        bool flag;
        Object obj2 = this._customInstantiator;
        try
        {
            this._customInstantiator = instance;
            if (instance.args.hasCustomInstantiator)
            {
                try
                {
                    return instance.args.customInstantiate.InitializePrefabInstance(instance);
                }
                catch (Exception exception)
                {
                    object[] args = new object[] { instance.args.customInstantiate, this, instance.args.prefab, exception };
                    Debug.LogError(string.Format("A exception was thrown during InitializePrefabInstance with '{0}' as custom instantiate, prefab '{1}' instance '{2}'.\r\ndoing standard initialization..\r\n{3}", args), instance);
                }
            }
            flag = true;
        }
        finally
        {
            this._customInstantiator = obj2;
        }
        return flag;
    }

    protected virtual void StandardInitialization(bool didAppend, IDRemote appended, NetInstance instance, NetworkView view, ref NetworkMessageInfo info)
    {
        if (didAppend)
        {
            IssueLocallyAppended(appended, instance.idMain);
        }
        if (this.ShouldDoStandardInitialization(instance))
        {
            NetworkInstantiatorUtility.BroadcastOnNetworkInstantiate(view, "uLink_OnNetworkInstantiate", info);
        }
    }

    private NetInstance Summon(IDMain prefab, bool isServer, ref NetworkInstantiateArgs niargs)
    {
        IDMain main;
        CustomInstantiationArgs args = new CustomInstantiationArgs(this, this._customInstantiator, prefab, ref niargs, isServer);
        NetworkView view = this.Create(ref args, out main);
        NetInstance instance = view.gameObject.AddComponent<NetInstance>();
        instance.args = args;
        instance.idMain = main;
        instance.prepared = false;
        instance.networkView = view;
        return instance;
    }

    public static bool ValidatePrefabNameOrMakeException(string name, out NetMainPrefabNameException e)
    {
        if (name == null)
        {
            e = new NetMainPrefabNameException("name", name, "null");
        }
        else if (name.Length < 2)
        {
            e = new NetMainPrefabNameException("name", name, "name must include the prefix character and at least one other after");
        }
        else if (name[0] != ':')
        {
            e = new NetMainPrefabNameException("name", name, "name did not begin with the prefix character");
        }
        else
        {
            e = null;
            return true;
        }
        return false;
    }

    private IDRemote localAppend
    {
        get
        {
            return this._localAppend;
        }
    }

    public Transform localAppendTransformInPrefab
    {
        get
        {
            return this.GetLocalAppendTransform(this.proxyPrefab);
        }
    }

    public IDMain localPrefab
    {
        get
        {
            return ((this._localPrefab == null) ? this.proxyPrefab : this._localPrefab);
        }
    }

    public string name
    {
        get
        {
            string name = base.name;
            if (name != this._originalName)
            {
                if (Application.isPlaying && !string.IsNullOrEmpty(this._originalName))
                {
                    Debug.LogWarning("You can't rename proxy instantiations at runtime!", this);
                }
                else
                {
                    this._originalName = name;
                    this._name = DressName(name);
                }
            }
            return this._name;
        }
    }

    public IDMain prefab
    {
        get
        {
            return this.proxyPrefab;
        }
    }

    public IDMain proxyPrefab
    {
        get
        {
            return ((this._proxyPrefab == null) ? this._serverPrefab : this._proxyPrefab);
        }
    }

    public IDMain serverPrefab
    {
        get
        {
            return ((this._serverPrefab == null) ? this.proxyPrefab : this._serverPrefab);
        }
    }

    internal static NetInstance zzz__currentNetInstance
    {
        get
        {
            return _currentNetInstance;
        }
    }

    private static class g
    {
        public static Dictionary<string, NetMainPrefab> dict = new Dictionary<string, NetMainPrefab>();

        static g()
        {
            NetMainPrefab.ginit = true;
        }
    }
}

