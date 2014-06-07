using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class BulletWeaponItem<T> : WeaponItem<T> where T: BulletWeaponDataBlock
{
    private int cachedNumReloads;
    public float nextAimTime;
    private float reloadStartTime;

    protected BulletWeaponItem(T db) : base(db)
    {
        this.reloadStartTime = -1f;
    }

    public virtual void ActualReload()
    {
        this.ActualReload_COD();
    }

    public virtual void ActualReload_COD()
    {
        this.reloadStartTime = Time.time;
        base.nextPrimaryAttackTime = Time.time + base.datablock.reloadDuration;
        Inventory inventory = base.inventory;
        int uses = base.uses;
        int maxClipAmmo = base.datablock.maxClipAmmo;
        if (uses != maxClipAmmo)
        {
            int count = maxClipAmmo - uses;
            int num4 = 0;
            while (uses < maxClipAmmo)
            {
                IInventoryItem item = inventory.FindItem(base.datablock.ammoType);
                if (item == null)
                {
                    break;
                }
                int num5 = count;
                if (item.Consume(ref count))
                {
                    inventory.RemoveItem(item.slot);
                }
                num4 += num5 - count;
                if (count == 0)
                {
                    break;
                }
            }
            if (num4 > 0)
            {
                base.AddUses(num4);
            }
            inventory.Refresh();
        }
    }

    public virtual void CacheReloads()
    {
        this.cachedNumReloads = 0;
    }

    protected override bool CanAim()
    {
        return ((!this.IsReloading() && base.CanAim()) && (Time.time > this.nextAimTime));
    }

    protected override bool CanSetActivate(bool value)
    {
        return (base.CanSetActivate(value) && (value || (base.nextPrimaryAttackTime <= Time.time)));
    }

    public virtual bool IsReloading()
    {
        return ((this.reloadStartTime != -1f) && (Time.time < (this.reloadStartTime + base.datablock.reloadDuration)));
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        if ((sample.attack && (this.clipAmmo == 0)) && (base.nextPrimaryAttackTime <= Time.time))
        {
            base.datablock.Local_DryFire(base.viewModelInstance, base.itemRepresentation);
            base.nextPrimaryAttackTime = Time.time + 1f;
            sample.attack = false;
        }
        base.ItemPreFrame(ref sample);
        if (sample.aim && (base.datablock.aimSway > 0f))
        {
            float x = Time.time * base.datablock.aimSwaySpeed;
            float num2 = base.datablock.aimSway * (!sample.crouch ? 1f : 0.6f);
            sample.yaw += ((Mathf.PerlinNoise(x, x) - 0.5f) * num2) * Time.deltaTime;
            sample.pitch += ((Mathf.PerlinNoise(x + 0.1f, x + 0.2f) - 0.5f) * num2) * Time.deltaTime;
        }
    }

    public override void PrimaryAttack(ref HumanController.InputSample sample)
    {
        base.nextPrimaryAttackTime = Time.time + base.datablock.fireRate;
        if (base.datablock.NoAimingAfterShot)
        {
            this.nextAimTime = Time.time + base.datablock.fireRate;
        }
        ViewModel viewModelInstance = base.viewModelInstance;
        if (actor.forceThirdPerson)
        {
            viewModelInstance = null;
        }
        base.datablock.Local_FireWeapon(viewModelInstance, base.itemRepresentation, base.iface as IBulletWeaponItem, ref sample);
    }

    public override void Reload(ref HumanController.InputSample sample)
    {
        base.datablock.Local_Reload(base.viewModelInstance, base.itemRepresentation, base.iface as IBulletWeaponItem, ref sample);
        this.ActualReload();
        base.inventory.Refresh();
    }

    public int cachedCasings { get; set; }

    public override bool canPrimaryAttack
    {
        get
        {
            return (base.canPrimaryAttack && (this.clipAmmo > 0));
        }
    }

    public override bool canReload
    {
        get
        {
            if ((base.nextPrimaryAttackTime <= Time.time) && (this.clipAmmo < base.datablock.maxClipAmmo))
            {
                IInventoryItem item = base.inventory.FindItem(base.datablock.ammoType);
                if ((item != null) && (item.uses > 0))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public int clipAmmo
    {
        get
        {
            return base.uses;
        }
        set
        {
            base.SetUses(value);
        }
    }

    public MagazineDataBlock clipType { get; protected set; }

    public float nextCasingsTime { get; set; }

    public override int possibleReloadCount
    {
        get
        {
            return this.cachedNumReloads;
        }
    }
}

