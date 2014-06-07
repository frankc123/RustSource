using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public abstract class RigidObj : IDMain, IInterpTimedEventReceiver
{
    private bool __calling_from_do_network;
    private bool __done;
    private bool __hiding;
    private NetworkView __ownerView;
    private RigidbodyInterpolator _interp;
    private RigidObjServerCollision _serverCollision;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map5;
    [NonSerialized]
    protected readonly FeatureFlags featureFlags;
    private bool hasInterp;
    protected Vector3 initialVelocity;
    private const string kDoNetworkMethodName = "__invoke_do_network";
    protected NetworkViewID ownerViewID;
    [NonSerialized]
    public Rigidbody rigidbody;
    private double serverLastUpdateTimestamp;
    protected double spawnTime;
    private double updateInterval;
    [SerializeField]
    private float updateRate;
    protected NetworkView view;

    protected RigidObj(FeatureFlags classFeatures) : base(IDFlags.Item)
    {
        this.updateRate = 2f;
        this.featureFlags = classFeatures;
    }

    private void __invoke_do_network()
    {
        if (!this.__calling_from_do_network)
        {
            try
            {
                this.__calling_from_do_network = true;
                this.DoNetwork();
            }
            finally
            {
                this.__calling_from_do_network = false;
            }
        }
    }

    protected void Awake()
    {
        this.rigidbody = base.rigidbody;
        this._interp = base.GetComponent<RigidbodyInterpolator>();
    }

    protected virtual void DoNetwork()
    {
        object[] args = new object[] { this.rigidbody.position, this.rigidbody.rotation };
        base.networkView.RPC("RecieveNetwork", RPCMode.AllExceptOwner, args);
        this.serverLastUpdateTimestamp = NetCull.time;
    }

    void IInterpTimedEventReceiver.OnInterpTimedEvent()
    {
        if (!this.OnInterpTimedEvent())
        {
            InterpTimedEvent.MarkUnhandled();
        }
    }

    protected abstract void OnDone();
    protected abstract void OnHide();
    protected virtual bool OnInterpTimedEvent()
    {
        string tag = InterpTimedEvent.Tag;
        if (tag != null)
        {
            int num;
            if (<>f__switch$map5 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                dictionary.Add("_init", 0);
                dictionary.Add("_done", 1);
                <>f__switch$map5 = dictionary;
            }
            if (<>f__switch$map5.TryGetValue(tag, out num))
            {
                if (num == 0)
                {
                    this.showing = true;
                    if (this.expectsInitialVelocity)
                    {
                        this.rigidbody.isKinematic = false;
                        this.rigidbody.velocity = this.initialVelocity;
                    }
                    return true;
                }
                if (num == 1)
                {
                    try
                    {
                        this.OnDone();
                    }
                    finally
                    {
                        try
                        {
                            this.showing = false;
                        }
                        finally
                        {
                            Object.Destroy(base.gameObject);
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }

    internal void OnServerCollision(byte kind, Collision collision)
    {
        switch (kind)
        {
            case 0:
                this.OnServerCollisionEnter(collision);
                break;

            case 1:
                this.OnServerCollisionExit(collision);
                break;

            case 2:
                this.OnServerCollisionStay(collision);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    protected virtual void OnServerCollisionEnter(Collision collision)
    {
    }

    protected virtual void OnServerCollisionExit(Collision collision)
    {
    }

    protected virtual void OnServerCollisionStay(Collision collision)
    {
    }

    protected abstract void OnShow();
    [RPC]
    protected void RecieveNetwork(Vector3 pos, Quaternion rot, NetworkMessageInfo info)
    {
        if (this.hasInterp && (this._interp != null))
        {
            PosRot rot2;
            rot2.position = pos;
            rot2.rotation = rot;
            this.rigidbody.isKinematic = true;
            this._interp.SetGoals(rot2, info.timestamp);
            this._interp.running = true;
        }
    }

    [RPC, Obsolete("Do not call manually")]
    protected void RODone(NetworkMessageInfo info)
    {
        if (!this.__done)
        {
            NetCull.DontDestroyWithNetwork((MonoBehaviour) this);
            InterpTimedEvent.Queue(this, "_done", ref info);
        }
    }

    protected void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        this.view = (NetworkView) info.networkView;
        BitStream initialData = this.view.initialData;
        if (this.expectsInitialVelocity)
        {
            this.initialVelocity = initialData.ReadVector3();
        }
        if (this.expectsOwner)
        {
            this.ownerViewID = initialData.ReadNetworkViewID();
        }
        this.spawnTime = info.timestamp;
        this.updateInterval = 1.0 / (NetCull.sendRate * Mathf.Max(1f, this.updateRate));
        this.hasInterp = (bool) this._interp;
        if (this.hasInterp)
        {
            this._interp.running = false;
        }
        this.rigidbody.isKinematic = true;
        this.__hiding = this.spawnTime > Interpolation.time;
        if (this.__hiding)
        {
            this.OnHide();
            if (this.hasInterp)
            {
                PosRot rot;
                rot.position = this.view.position;
                rot.rotation = this.view.rotation;
                this._interp.SetGoals(rot, this.spawnTime);
            }
            InterpTimedEvent.Queue(this, "_init", ref info);
        }
        else
        {
            this.OnShow();
        }
    }

    public bool expectsInitialVelocity
    {
        get
        {
            return (((byte) (this.featureFlags & FeatureFlags.StreamInitialVelocity)) == 1);
        }
    }

    public bool expectsOwner
    {
        get
        {
            return (((byte) (this.featureFlags & FeatureFlags.StreamOwnerViewID)) == 2);
        }
    }

    public NetworkView ownerView
    {
        get
        {
            return ((this.__ownerView == null) ? (this.__ownerView = NetworkView.Find(this.ownerViewID)) : this.__ownerView);
        }
    }

    public bool serverSideCollisions
    {
        get
        {
            return (((byte) (this.featureFlags & FeatureFlags.ServerCollisions)) == 0x80);
        }
    }

    public bool showing
    {
        get
        {
            return !this.__hiding;
        }
        protected set
        {
            if (this.__hiding == value)
            {
                this.__hiding = !value;
                if (this.__hiding)
                {
                    this.OnHide();
                }
                else
                {
                    this.OnShow();
                }
            }
        }
    }

    [Flags]
    protected enum FeatureFlags : byte
    {
        ServerCollisions = 0x80,
        StreamInitialVelocity = 1,
        StreamOwnerViewID = 2
    }
}

