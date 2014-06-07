using System;
using UnityEngine;

public abstract class MeleeWeaponItem<T> : WeaponItem<T> where T: MeleeWeaponDataBlock
{
    private float _queuedSwingAttackTime;
    private float _queuedSwingSoundTime;

    protected MeleeWeaponItem(T db) : base(db)
    {
        this._queuedSwingAttackTime = -1f;
        this._queuedSwingSoundTime = -1f;
    }

    protected override bool CanSetActivate(bool wantsTrue)
    {
        return (base.CanSetActivate(wantsTrue) && (wantsTrue || (base.nextPrimaryAttackTime <= Time.time)));
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        base.ItemPreFrame(ref sample);
        if ((this.queuedSwingAttackTime > 0f) && (this.queuedSwingAttackTime < Time.time))
        {
            base.datablock.Local_MidSwing(base.viewModelInstance, base.itemRepresentation, base.iface as IMeleeWeaponItem, ref sample);
            this.queuedSwingAttackTime = -1f;
        }
        if ((this.queuedSwingSoundTime > 0f) && (this.queuedSwingSoundTime < Time.time))
        {
            base.datablock.SwingSound();
            this.queuedSwingSoundTime = -1f;
        }
    }

    protected override void OnSetActive(bool isActive)
    {
        this.queuedSwingSoundTime = -1f;
        this.queuedSwingAttackTime = -1f;
        base.OnSetActive(isActive);
    }

    public override void PrimaryAttack(ref HumanController.InputSample sample)
    {
        float fireRate = base.datablock.fireRate;
        Metabolism local = base.inventory.GetLocal<Metabolism>();
        if ((local != null) && (local.GetCalorieLevel() <= 0f))
        {
            fireRate = base.datablock.fireRate * 2f;
        }
        float num2 = Time.time + fireRate;
        base.nextSecondaryAttackTime = num2;
        base.nextPrimaryAttackTime = num2;
        base.datablock.Local_FireWeapon(base.viewModelInstance, base.itemRepresentation, base.iface as IMeleeWeaponItem, ref sample);
    }

    public virtual void QueueMidSwing(float time)
    {
        this.queuedSwingAttackTime = time;
    }

    public virtual void QueueSwingSound(float time)
    {
        this.queuedSwingSoundTime = time;
    }

    public override void SecondaryAttack(ref HumanController.InputSample sample)
    {
        float num = Time.time + base.datablock.fireRate;
        base.nextPrimaryAttackTime = num;
        base.nextSecondaryAttackTime = num;
    }

    public float queuedSwingAttackTime
    {
        get
        {
            return this._queuedSwingAttackTime;
        }
        set
        {
            this._queuedSwingAttackTime = value;
        }
    }

    public float queuedSwingSoundTime
    {
        get
        {
            return this._queuedSwingSoundTime;
        }
        set
        {
            this._queuedSwingSoundTime = value;
        }
    }
}

