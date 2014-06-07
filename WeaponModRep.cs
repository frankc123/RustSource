using System;
using UnityEngine;

public abstract class WeaponModRep : ItemModRepresentation
{
    private GameObject _attached;
    private bool _on;
    protected readonly bool defaultsOn;

    protected WeaponModRep(ItemModRepresentation.Caps caps) : this(caps, false)
    {
    }

    protected WeaponModRep(ItemModRepresentation.Caps caps, bool defaultsOn) : base(caps)
    {
        this.defaultsOn = defaultsOn;
        this._on = defaultsOn;
    }

    protected abstract void DisableMod(ItemModRepresentation.Reason reason);
    protected abstract void EnableMod(ItemModRepresentation.Reason reason);
    protected virtual void OnAddAttached()
    {
    }

    protected virtual void OnRemoveAttached()
    {
    }

    public virtual void SetAttached(GameObject attached, bool vm)
    {
        this.attached = attached;
    }

    protected void SetOn(bool on, ItemModRepresentation.Reason reason)
    {
        if (this._on != on)
        {
            this._on = on;
            if (this._attached != null)
            {
                if (on)
                {
                    this.EnableMod(reason);
                }
                else
                {
                    this.DisableMod(reason);
                }
            }
        }
    }

    protected virtual bool VerifyCompatible(GameObject attachment)
    {
        return true;
    }

    public GameObject attached
    {
        get
        {
            return this._attached;
        }
        protected set
        {
            if (value != this._attached)
            {
                if (value != null)
                {
                    if (!this.VerifyCompatible(value))
                    {
                        throw new ArgumentOutOfRangeException("value", "incompatible");
                    }
                    if (this._attached != null)
                    {
                        this.OnRemoveAttached();
                    }
                    this._attached = value;
                    this.OnAddAttached();
                    if (this._on)
                    {
                        this.EnableMod(ItemModRepresentation.Reason.Implicit);
                    }
                    else
                    {
                        this.DisableMod(ItemModRepresentation.Reason.Implicit);
                    }
                }
                else
                {
                    if (this._attached != null)
                    {
                        this.OnRemoveAttached();
                    }
                    this._attached = null;
                }
            }
            this._attached = value;
        }
    }

    public bool on
    {
        get
        {
            return this._on;
        }
        protected set
        {
            this.SetOn(value, ItemModRepresentation.Reason.Explicit);
        }
    }
}

