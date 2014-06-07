using Facepunch;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

[StructLayout(LayoutKind.Explicit, Size=4)]
public struct NetEntityID : IEquatable<NetworkViewID>, IEquatable<NetEntityID>, IComparable<NetworkViewID>, IComparable<NetEntityID>
{
    [FieldOffset(0)]
    private NetworkViewID _viewID;
    private static readonly uLink.BitStreamCodec.Deserializer deserializer;
    [FieldOffset(2)]
    private ushort p1;
    [FieldOffset(0)]
    private ushort p2;
    private static readonly uLink.BitStreamCodec.Serializer serializer;
    [FieldOffset(0)]
    private int v;

    static NetEntityID()
    {
        serializer = new uLink.BitStreamCodec.Serializer(NetEntityID.Serializer);
        deserializer = new uLink.BitStreamCodec.Deserializer(NetEntityID.Deserializer);
        BitStreamCodec.AddAndMakeArray<NetEntityID>(deserializer, serializer);
    }

    public NetEntityID(NGCView view)
    {
        this = new NetEntityID();
        if (view != null)
        {
            this.v = view.id;
        }
    }

    public NetEntityID(NetworkView view)
    {
        this = new NetEntityID();
        if (view != null)
        {
            this._viewID = view.viewID;
        }
    }

    public NetEntityID(NetworkViewID viewID)
    {
        this = new NetEntityID();
        this._viewID = viewID;
    }

    public int CompareTo(NetEntityID other)
    {
        return this.v.CompareTo(other.v);
    }

    public int CompareTo(NetworkViewID other)
    {
        return this.v.CompareTo(other.id);
    }

    private static object Deserializer(BitStream bs, params object[] codecOptions)
    {
        NetEntityID yid = new NetEntityID {
            p1 = bs.Read<ushort>(codecOptions)
        };
        if (yid.p1 == 0)
        {
            yid._viewID = bs.Read<NetworkViewID>(codecOptions);
            return yid;
        }
        yid.p2 = bs.Read<ushort>(codecOptions);
        return yid;
    }

    public bool Equals(NetEntityID obj)
    {
        return (this.v == obj.v);
    }

    public override bool Equals(object obj)
    {
        return (!(obj is NetEntityID) ? ((this.isNet && (obj is NetworkViewID)) && this.Equals((NetworkViewID) obj)) : this.Equals((NetEntityID) obj));
    }

    public bool Equals(NetworkViewID obj)
    {
        return ((this.p1 == 0) && (this._viewID == obj));
    }

    public static NetEntityID Get(NetworkViewID id)
    {
        return new NetEntityID(id);
    }

    public static NetEntityID Get(Component entityComponent)
    {
        return Get(entityComponent, false);
    }

    public static NetEntityID Get(GameObject entity)
    {
        return Get(entity, false);
    }

    public static NetEntityID Get(MonoBehaviour entityScript)
    {
        return Get(entityScript, false);
    }

    public static NetEntityID Get(Component entityComponent, bool throwIfNotFound)
    {
        NetEntityID yid;
        if (((int) Of(entityComponent, out yid)) != 0)
        {
            return yid;
        }
        if (throwIfNotFound)
        {
            throw new InvalidOperationException("no recognizable net entity id");
        }
        return unassigned;
    }

    public static NetEntityID Get(GameObject entity, bool throwIfNotFound)
    {
        NetEntityID yid;
        if (((int) Of(entity, out yid)) != 0)
        {
            return yid;
        }
        if (throwIfNotFound)
        {
            throw new InvalidOperationException("no recognizable net entity id");
        }
        return unassigned;
    }

    public static NetEntityID Get(MonoBehaviour entityScript, bool throwIfNotFound)
    {
        NetEntityID yid;
        if (((int) Of(entityScript, out yid)) != 0)
        {
            return yid;
        }
        if (throwIfNotFound)
        {
            throw new InvalidOperationException("no recognizable net entity id");
        }
        return unassigned;
    }

    public TComponent GetComponent<TComponent>() where TComponent: Component
    {
        MonoBehaviour view = this.view;
        return ((view == null) ? null : view.GetComponent<TComponent>());
    }

    public bool GetComponent<TComponent>(out TComponent component) where TComponent: Component
    {
        TComponent local;
        MonoBehaviour view = this.view;
        if (view == null)
        {
            component = null;
            return false;
        }
        if (view is TComponent)
        {
            component = (TComponent) view;
            return true;
        }
        component = local = view.GetComponent<TComponent>();
        return local;
    }

    public override int GetHashCode()
    {
        return ((this.p1 != 0) ? (this.v ^ -65536) : this.p2.GetHashCode());
    }

    public static Kind Of(Component component)
    {
        NetEntityID yid;
        MonoBehaviour behaviour;
        return Of(component, out yid, out behaviour);
    }

    public static Kind Of(GameObject entity)
    {
        NetEntityID yid;
        MonoBehaviour behaviour;
        return Of(entity, out yid, out behaviour);
    }

    public static Kind Of(MonoBehaviour script)
    {
        NetEntityID yid;
        MonoBehaviour behaviour;
        return Of(script, out yid, out behaviour);
    }

    public static Kind Of(Component component, out NetEntityID entID)
    {
        MonoBehaviour behaviour;
        return Of(component, out entID, out behaviour);
    }

    public static Kind Of(Component component, out MonoBehaviour view)
    {
        NetEntityID yid;
        return Of(component, out yid, out view);
    }

    public static Kind Of(GameObject entity, out NetEntityID entID)
    {
        MonoBehaviour behaviour;
        return Of(entity, out entID, out behaviour);
    }

    public static Kind Of(GameObject entity, out MonoBehaviour view)
    {
        NetEntityID yid;
        return Of(entity, out yid, out view);
    }

    public static Kind Of(MonoBehaviour script, out NetEntityID entID)
    {
        MonoBehaviour behaviour;
        return Of(script, out entID, out behaviour);
    }

    public static Kind Of(MonoBehaviour script, out MonoBehaviour view)
    {
        NetEntityID yid;
        return Of(script, out yid, out view);
    }

    public static Kind Of(Component component, out NetEntityID entID, out MonoBehaviour view)
    {
        if (component is MonoBehaviour)
        {
            return Of((MonoBehaviour) component, out entID, out view);
        }
        if (component != null)
        {
            return Of(component.gameObject, out entID, out view);
        }
        entID = unassigned;
        view = null;
        return Kind.Missing;
    }

    public static Kind Of(GameObject entity, out NetEntityID entID, out MonoBehaviour view)
    {
        if (entity == null)
        {
            entID = unassigned;
            view = null;
            return Kind.Missing;
        }
        NetworkView component = entity.GetComponent<NetworkView>();
        if (component != null)
        {
            entID = new NetEntityID(component.viewID);
            view = component;
            return Kind.Net;
        }
        NGCView view3 = entity.GetComponent<NGCView>();
        if (view3 != null)
        {
            entID = new NetEntityID(view3);
            view = view3;
            return Kind.NGC;
        }
        entID = unassigned;
        view = null;
        return Kind.Missing;
    }

    public static Kind Of(MonoBehaviour script, out NetEntityID entID, out MonoBehaviour view)
    {
        if (script == null)
        {
            entID = unassigned;
            view = null;
            return Kind.Missing;
        }
        if (script is NetworkView)
        {
            view = script;
            entID = ((NetworkView) script).viewID;
            return Kind.Net;
        }
        if (script is NGCView)
        {
            view = script;
            entID = new NetEntityID((NGCView) script);
            return Kind.NGC;
        }
        return Of(script.gameObject, out entID, out view);
    }

    public static bool operator ==(NetEntityID lhs, NetEntityID rhs)
    {
        return (lhs.v == rhs.v);
    }

    public static bool operator ==(NetEntityID lhs, NetworkViewID rhs)
    {
        return ((lhs.p1 == 0) && (lhs.p2 == rhs.id));
    }

    public static bool operator ==(NetworkViewID lhs, NetEntityID rhs)
    {
        return ((rhs.p1 == 0) && (rhs.p2 == lhs.id));
    }

    public static explicit operator NetworkViewID(NetEntityID viewID)
    {
        if (viewID.p1 != 0)
        {
            throw new InvalidCastException("The NetEntityID did not represet a NetworkViewID");
        }
        return viewID._viewID;
    }

    public static bool operator false(NetEntityID id)
    {
        return (id.v == 0);
    }

    public static bool operator >(NetEntityID lhs, NetEntityID rhs)
    {
        return (lhs.v > rhs.v);
    }

    public static bool operator >(NetEntityID lhs, NetworkViewID rhs)
    {
        return (lhs.v > rhs.id);
    }

    public static bool operator >(NetworkViewID lhs, NetEntityID rhs)
    {
        return (lhs.id > rhs.v);
    }

    public static bool operator >=(NetEntityID lhs, NetEntityID rhs)
    {
        return (lhs.v >= rhs.v);
    }

    public static bool operator >=(NetEntityID lhs, NetworkViewID rhs)
    {
        return (lhs.v >= rhs.id);
    }

    public static bool operator >=(NetworkViewID lhs, NetEntityID rhs)
    {
        return (lhs.id >= rhs.v);
    }

    public static implicit operator NetEntityID(NetworkViewID viewID)
    {
        return new NetEntityID { _viewID = viewID };
    }

    public static bool operator !=(NetEntityID lhs, NetEntityID rhs)
    {
        return (lhs.v != rhs.v);
    }

    public static bool operator !=(NetEntityID lhs, NetworkViewID rhs)
    {
        return ((lhs.p1 != 0) || (lhs.p2 != rhs.id));
    }

    public static bool operator !=(NetworkViewID lhs, NetEntityID rhs)
    {
        return ((rhs.p1 != 0) || (rhs.p2 != lhs.id));
    }

    public static bool operator <(NetEntityID lhs, NetEntityID rhs)
    {
        return (lhs.v < rhs.v);
    }

    public static bool operator <(NetEntityID lhs, NetworkViewID rhs)
    {
        return (lhs.v < rhs.id);
    }

    public static bool operator <(NetworkViewID lhs, NetEntityID rhs)
    {
        return (lhs.id < rhs.v);
    }

    public static bool operator <=(NetEntityID lhs, NetEntityID rhs)
    {
        return (lhs.v <= rhs.v);
    }

    public static bool operator <=(NetEntityID lhs, NetworkViewID rhs)
    {
        return (lhs.v <= rhs.id);
    }

    public static bool operator <=(NetworkViewID lhs, NetEntityID rhs)
    {
        return (lhs.id <= rhs.v);
    }

    public static bool operator true(NetEntityID id)
    {
        return (id.v != 0);
    }

    private static void Serializer(BitStream bs, object value, params object[] codecOptions)
    {
        NetEntityID yid = (NetEntityID) value;
        bs.Write<ushort>(yid.p1, codecOptions);
        if (yid.p1 == 0)
        {
            bs.Write<NetworkViewID>(yid._viewID, codecOptions);
        }
        else
        {
            bs.Write<ushort>(yid.p2, new object[0]);
        }
    }

    public override string ToString()
    {
        if (this.v == 0)
        {
            return "Unassigned";
        }
        if (this.p1 == 0)
        {
            return this._viewID.ToString();
        }
        return string.Format("NGC ViewID {0} ({1}:{2})", this.v, this.p1, this.p2 + 1);
    }

    public Collider collider
    {
        get
        {
            MonoBehaviour view = this.view;
            return ((view == null) ? null : view.collider);
        }
    }

    public GameObject gameObject
    {
        get
        {
            MonoBehaviour view = this.view;
            return ((view == null) ? null : view.gameObject);
        }
    }

    public int id
    {
        get
        {
            return this.v;
        }
    }

    public IDBase idBase
    {
        get
        {
            if (this.p1 == 0)
            {
                if (this.p2 != 0)
                {
                    NetworkView networkView = NetworkView.Find(this._viewID);
                    if (networkView != null)
                    {
                        return IDBase.Get(networkView);
                    }
                }
                return null;
            }
            NGCView view2 = NGC.Find(this.v);
            if (view2 != null)
            {
                return IDBase.Get(view2.gameObject);
            }
            return null;
        }
    }

    public bool isAllocated
    {
        get
        {
            if (this.p1 == 0)
            {
                return this._viewID.isAllocated;
            }
            return true;
        }
    }

    public bool isManual
    {
        get
        {
            return ((this.p1 == 0) && this._viewID.isManual);
        }
    }

    public bool isMine
    {
        get
        {
            return ((this.p1 == 0) && this._viewID.isMine);
        }
    }

    public bool isNet
    {
        get
        {
            return ((this.p1 == 0) && (this._viewID != NetworkViewID.unassigned));
        }
    }

    public bool isNGC
    {
        get
        {
            return (this.p1 != 0);
        }
    }

    public bool isUnassigned
    {
        get
        {
            return (this.v == 0);
        }
    }

    public IDMain main
    {
        get
        {
            if (this.p1 == 0)
            {
                if (this.p2 != 0)
                {
                    NetworkView networkView = NetworkView.Find(this._viewID);
                    if (networkView == null)
                    {
                        return null;
                    }
                    IDBase base2 = IDBase.Get(networkView);
                    if (base2 != null)
                    {
                        return base2.idMain;
                    }
                }
                return null;
            }
            NGCView view2 = NGC.Find(this.v);
            if (view2 != null)
            {
                return IDBase.GetMain(view2.gameObject);
            }
            return null;
        }
    }

    public NetworkView networkView
    {
        get
        {
            if (this.p1 == 0)
            {
                return NetworkView.Find(this._viewID);
            }
            return null;
        }
    }

    public NGCView ngcView
    {
        get
        {
            if (this.p1 == 0)
            {
                return null;
            }
            return NGC.Find(this.v);
        }
    }

    public NetworkPlayer owner
    {
        get
        {
            if (this.p1 == 0)
            {
                return this._viewID.owner;
            }
            return NetworkPlayer.server;
        }
    }

    public Renderer renderer
    {
        get
        {
            MonoBehaviour view = this.view;
            return ((view == null) ? null : view.renderer);
        }
    }

    public Rigidbody rigidbody
    {
        get
        {
            MonoBehaviour view = this.view;
            return ((view == null) ? null : view.rigidbody);
        }
    }

    public Transform transform
    {
        get
        {
            MonoBehaviour view = this.view;
            return ((view == null) ? null : view.transform);
        }
    }

    public static NetEntityID unassigned
    {
        get
        {
            return new NetEntityID();
        }
    }

    public MonoBehaviour view
    {
        get
        {
            if (this.p1 != 0)
            {
                return NGC.Find(this.v);
            }
            if (this.p2 == 0)
            {
                return null;
            }
            return NetworkView.Find(this._viewID);
        }
    }

    public enum Kind : sbyte
    {
        Missing = 0,
        Net = 1,
        NGC = -1
    }
}

