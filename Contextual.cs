using Facepunch;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[InterfaceDriverComponent(typeof(IContextRequestable), "_implementation", "implementation", SearchRoute=InterfaceSearchRoute.GameObject, UnityType=typeof(MonoBehaviour), AlwaysSaveDisabled=true)]
public sealed class Contextual : MonoBehaviour, IComponentInterfaceDriver<IContextRequestable, MonoBehaviour, Contextual>
{
    [NonSerialized]
    private bool _awoke;
    [SerializeField]
    private MonoBehaviour _implementation;
    [NonSerialized]
    private bool _implemented;
    [NonSerialized]
    private bool? _isMenu;
    [NonSerialized]
    private bool? _isQuick;
    [NonSerialized]
    private bool? _isSoleAccess;
    [NonSerialized]
    private IContextRequestable _requestable;
    [NonSerialized]
    private MonoBehaviour implementation;

    public bool AsMenu(out IContextRequestableMenu menu)
    {
        if (this.isMenu)
        {
            menu = this.@interface as IContextRequestableMenu;
            return (bool) this.implementor;
        }
        menu = null;
        return false;
    }

    public bool AsMenu<IContextRequestableMenuType>(out IContextRequestableMenuType menu) where IContextRequestableMenuType: class, IContextRequestableMenu
    {
        IContextRequestableMenu menu2;
        if (this.AsMenu(out menu2))
        {
            IContextRequestableMenuType local;
            menu = local = menu2 as IContextRequestableMenuType;
            return !object.ReferenceEquals(local, null);
        }
        menu = null;
        return false;
    }

    public bool AsQuick(out IContextRequestableQuick quick)
    {
        if (this.isQuick)
        {
            quick = this.@interface as IContextRequestableQuick;
            return (bool) this.implementor;
        }
        quick = null;
        return false;
    }

    public bool AsQuick<IContextRequestableQuickType>(out IContextRequestableQuickType quick) where IContextRequestableQuickType: class, IContextRequestableQuick
    {
        IContextRequestableQuick quick2;
        if (this.AsQuick(out quick2))
        {
            IContextRequestableQuickType local;
            quick = local = quick2 as IContextRequestableQuickType;
            return !object.ReferenceEquals(local, null);
        }
        quick = null;
        return false;
    }

    public static bool ContextOf(NetworkView networkView, out Contextual contextual)
    {
        return GetMB(networkView, out contextual);
    }

    public static bool ContextOf(NetEntityID entityID, out Contextual contextual)
    {
        return GetMB(entityID.view, out contextual);
    }

    public static bool ContextOf(NGCView networkView, out Contextual contextual)
    {
        return GetMB(networkView, out contextual);
    }

    public static bool ContextOf(NetworkViewID networkViewID, out Contextual contextual)
    {
        return GetMB(NetworkView.Find(networkViewID), out contextual);
    }

    public static bool ContextOf(Component component, out Contextual contextual)
    {
        MonoBehaviour behaviour;
        if (((int) NetEntityID.Of(component, out behaviour)) == 0)
        {
            contextual = null;
            return false;
        }
        return GetMB(behaviour, out contextual);
    }

    public static bool ContextOf(GameObject gameObject, out Contextual contextual)
    {
        MonoBehaviour behaviour;
        if (((int) NetEntityID.Of(gameObject, out behaviour)) == 0)
        {
            contextual = null;
            return false;
        }
        return GetMB(behaviour, out contextual);
    }

    public static bool FindUp(Transform transform, out Contextual contextual)
    {
        while (transform != null)
        {
            if ((contextual = transform.GetComponent<Contextual>()) != null)
            {
                return true;
            }
            transform = transform.parent;
        }
        contextual = null;
        return false;
    }

    private static bool GetMB(MonoBehaviour networkView, out Contextual contextual)
    {
        if ((networkView != null) && ((contextual = networkView.GetComponent<Contextual>()) != null))
        {
            return contextual.exists;
        }
        contextual = null;
        return false;
    }

    private void Refresh()
    {
        this.implementation = this._implementation;
        this._implementation = null;
        this._requestable = this.implementation as IContextRequestable;
        this._implemented = this._requestable != null;
        if (!this._implemented)
        {
            Debug.LogWarning("implementation is null or does not implement IContextRequestable", this);
        }
    }

    public Contextual driver
    {
        get
        {
            return this;
        }
    }

    public bool exists
    {
        get
        {
            if (!this._awoke)
            {
                try
                {
                    this.Refresh();
                }
                finally
                {
                    this._awoke = true;
                }
            }
            return (this._implemented && (this._implemented = (bool) this.implementation));
        }
    }

    public MonoBehaviour implementor
    {
        get
        {
            if (!this._awoke)
            {
                try
                {
                    this.Refresh();
                }
                finally
                {
                    this._awoke = true;
                }
            }
            return this.implementation;
        }
    }

    public IContextRequestable @interface
    {
        get
        {
            if (!this._awoke)
            {
                try
                {
                    this.Refresh();
                }
                finally
                {
                    this._awoke = true;
                }
            }
            return this._requestable;
        }
    }

    public bool isMenu
    {
        get
        {
            bool? nullable2;
            bool? nullable = this._isMenu;
            return (!nullable.HasValue ? (nullable2 = nullable2).Value : nullable.Value);
        }
    }

    public bool isQuick
    {
        get
        {
            bool? nullable2;
            bool? nullable = this._isQuick;
            return (!nullable.HasValue ? (nullable2 = nullable2).Value : nullable.Value);
        }
    }

    public bool isSoleAccess
    {
        get
        {
            bool? nullable2;
            bool? nullable = this._isSoleAccess;
            return (!nullable.HasValue ? (nullable2 = nullable2).Value : nullable.Value);
        }
    }
}

