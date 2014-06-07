using Facepunch;
using System;
using uLink;
using UnityEngine;

[InterfaceDriverComponent(typeof(IActivatable), "_implementation", "implementation", SearchRoute=InterfaceSearchRoute.GameObject, UnityType=typeof(MonoBehaviour), AlwaysSaveDisabled=true)]
public sealed class Activatable : MonoBehaviour, IComponentInterfaceDriver<IActivatable, MonoBehaviour, Activatable>
{
    [NonSerialized]
    private bool _awoke;
    [SerializeField]
    private MonoBehaviour _implementation;
    [NonSerialized]
    private bool _implemented;
    private IActivatable act;
    private IActivatableToggle actToggle;
    private bool canAct;
    private bool canToggle;
    private MonoBehaviour implementation;
    private ActivatableInfo info;

    private ActivationResult Act(Character instigator, ulong timestamp)
    {
        return (!this.canAct ? ActivationResult.Error_Implementation : ((this.implementation == null) ? ActivationResult.Error_Destroyed : this.act.ActTrigger(instigator, timestamp)));
    }

    private ActivationResult Act(Character instigator, ActivationToggleState state, ulong timestamp)
    {
        return (!this.canToggle ? ActivationResult.Error_Implementation : ((this.implementation == null) ? ActivationResult.Error_Destroyed : this.actToggle.ActTrigger(instigator, state, timestamp)));
    }

    public ActivationResult Activate()
    {
        return this.Activate((Character) null, NetCull.timeInMillis);
    }

    public ActivationResult Activate(bool on)
    {
        return this.Activate(on, null, NetCull.timeInMillis);
    }

    public ActivationResult Activate(ulong timestamp)
    {
        return this.Activate((Character) null, timestamp);
    }

    public ActivationResult Activate(ref NetworkMessageInfo info)
    {
        return this.ActRoute(null, info.sender, info.timestampInMillis);
    }

    public ActivationResult Activate(Character instigator, ulong timestamp)
    {
        throw new NotSupportedException("Server only");
    }

    public ActivationResult Activate(bool on, Character instigator)
    {
        return this.Activate(on, instigator, NetCull.timeInMillis);
    }

    public ActivationResult Activate(bool on, ulong timestamp)
    {
        return this.Activate(on, null, timestamp);
    }

    public ActivationResult Activate(bool on, ref NetworkMessageInfo info)
    {
        return this.ActRoute(new bool?(on), info.sender, info.timestampInMillis);
    }

    public ActivationResult Activate(bool on, Character instigator, ulong timestamp)
    {
        throw new NotSupportedException("Server only");
    }

    private ActivationResult ActRoute(bool? on, Character character, ulong timestamp)
    {
        if (on.HasValue)
        {
            return this.Activate(on.Value, character, timestamp);
        }
        return this.Activate(character, timestamp);
    }

    private ActivationResult ActRoute(bool? on, Controllable controllable, ulong timestamp)
    {
        return this.ActRoute(on, (controllable == null) ? null : controllable.GetComponent<Character>(), timestamp);
    }

    private ActivationResult ActRoute(bool? on, PlayerClient sender, ulong timestamp)
    {
        return this.ActRoute(on, ((sender == null) || (sender.controllable == null)) ? null : sender.controllable, timestamp);
    }

    private ActivationResult ActRoute(bool? on, NetworkPlayer sender, ulong timestamp)
    {
        PlayerClient client;
        ServerManagement management = ServerManagement.Get();
        if (management != null)
        {
            management.GetPlayerClient(sender, out client);
        }
        else
        {
            client = null;
        }
        return this.ActRoute(on, client, timestamp);
    }

    private void Awake()
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
    }

    private void OnDestroy()
    {
        if (this.implementation != null)
        {
            IActivatableFill implementation = this.implementation as IActivatableFill;
            if (implementation != null)
            {
                implementation.ActivatableChanged(this, false);
            }
        }
        this.implementation = null;
        this.canAct = false;
        this.canToggle = false;
        this.act = null;
        this.actToggle = null;
        this.info = new ActivatableInfo();
    }

    private void Refresh()
    {
        this.implementation = this._implementation;
        this._implementation = null;
        this.act = this.implementation as IActivatable;
        this.canAct = this.act != null;
        if (this.canAct)
        {
            this.actToggle = this.implementation as IActivatableToggle;
            this.canToggle = this.actToggle != null;
            IActivatableFill implementation = this.implementation as IActivatableFill;
            if (implementation != null)
            {
                implementation.ActivatableChanged(this, true);
            }
            IActivatableInfo info = this.implementation as IActivatableInfo;
            if (info != null)
            {
                info.ActInfo(out this.info);
            }
        }
        else
        {
            Debug.LogWarning("implementation is null or does not implement IActivatable", this);
        }
    }

    private void Reset()
    {
        if (!this.canAct)
        {
            foreach (MonoBehaviour behaviour in base.GetComponents<MonoBehaviour>())
            {
                if ((behaviour != this) && (behaviour is IActivatable))
                {
                    this._implementation = behaviour;
                    break;
                }
            }
        }
    }

    public Activatable driver
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

    public IActivatable @interface
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
            return this.act;
        }
    }

    public bool isToggle
    {
        get
        {
            return this.canToggle;
        }
    }

    public ActivationToggleState toggleState
    {
        get
        {
            return ((!this.canToggle || (this.implementation == null)) ? ActivationToggleState.Unspecified : this.actToggle.ActGetToggleState());
        }
    }
}

