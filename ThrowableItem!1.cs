using System;
using UnityEngine;

public abstract class ThrowableItem<T> : WeaponItem<T> where T: ThrowableItemDataBlock
{
    private bool _holdingBack;
    private float _holdingStartTime;
    private float _minReleaseTime;

    protected ThrowableItem(T db) : base(db)
    {
        this._minReleaseTime = 1.25f;
    }

    public virtual void BeginHoldingBack()
    {
        this.holdingStartTime = Time.time;
        this.holdingBack = true;
    }

    public virtual void EndHoldingBack()
    {
        this.holdingBack = false;
        this.holdingStartTime = 0f;
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        base.ItemPreFrame(ref sample);
        if ((this.holdingBack && !sample.attack) && ((Time.time - this.holdingStartTime) > this.minReleaseTime))
        {
            base.datablock.AttackReleased(base.viewModelInstance, base.itemRepresentation, base.iface as IThrowableItem, ref sample);
            this.holdingBack = false;
        }
    }

    protected override void OnSetActive(bool isActive)
    {
        this.EndHoldingBack();
        base.OnSetActive(isActive);
    }

    public override void PrimaryAttack(ref HumanController.InputSample sample)
    {
        base.nextPrimaryAttackTime = Time.time + base.datablock.fireRate;
        base.datablock.PrimaryAttack(base.viewModelInstance, base.itemRepresentation, base.iface as IThrowableItem, ref sample);
    }

    public override void SecondaryAttack(ref HumanController.InputSample sample)
    {
        base.nextSecondaryAttackTime = Time.time + base.datablock.fireRateSecondary;
        base.datablock.SecondaryAttack(base.viewModelInstance, base.itemRepresentation, base.iface as IThrowableItem, ref sample);
    }

    public virtual float heldThrowStrength
    {
        get
        {
            float num = Time.time - this.holdingStartTime;
            return Mathf.Clamp(num * base.datablock.throwStrengthPerSec, base.datablock.throwStrengthMin, base.datablock.throwStrengthMin);
        }
    }

    public bool holdingBack
    {
        get
        {
            return this._holdingBack;
        }
        set
        {
            this._holdingBack = value;
        }
    }

    public float holdingStartTime
    {
        get
        {
            return this._holdingStartTime;
        }
        set
        {
            this._holdingStartTime = value;
        }
    }

    public float minReleaseTime
    {
        get
        {
            return this._minReleaseTime;
        }
        set
        {
            this._minReleaseTime = value;
        }
    }
}

