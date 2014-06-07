using System;
using UnityEngine;

public abstract class StrikeGunItem<T> : BulletWeaponItem<T> where T: StrikeGunDataBlock
{
    public float actualFireTime;
    public bool beganFiring;

    protected StrikeGunItem(T db) : base(db)
    {
    }

    public virtual void CancelAttack(ref HumanController.InputSample sample)
    {
        if (this.beganFiring)
        {
            ViewModel viewModelInstance = base.viewModelInstance;
            base.datablock.Local_CancelStrikes(base.viewModelInstance, base.itemRepresentation, base.iface as IStrikeGunItem, ref sample);
            base.nextPrimaryAttackTime = Time.time + 1f;
            this.ResetFiring();
        }
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        base.ItemPreFrame(ref sample);
        if (!sample.attack && this.beganFiring)
        {
            this.CancelAttack(ref sample);
            sample.attack = false;
        }
        if ((sample.attack && (base.clipAmmo == 0)) && this.canReload)
        {
            this.Reload(ref sample);
        }
        if ((this.beganFiring && sample.attack) && (Time.time > this.actualFireTime))
        {
            base.PrimaryAttack(ref sample);
            this.ResetFiring();
        }
    }

    protected override void OnSetActive(bool isActive)
    {
        this.ResetFiring();
        base.OnSetActive(isActive);
    }

    public override void PrimaryAttack(ref HumanController.InputSample sample)
    {
        if (!this.beganFiring)
        {
            int numStrikes = Mathf.Clamp(Random.Range(1, base.datablock.strikeDurations.Length + 1), 1, base.datablock.strikeDurations.Length);
            this.actualFireTime = Time.time + base.datablock.strikeDurations[numStrikes - 1];
            base.datablock.Local_BeginStrikes(numStrikes, base.viewModelInstance, base.itemRepresentation, base.iface as IStrikeGunItem, ref sample);
            this.beganFiring = true;
        }
    }

    public void ResetFiring()
    {
        this.actualFireTime = 0f;
        this.beganFiring = false;
    }
}

