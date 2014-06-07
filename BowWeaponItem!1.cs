using Rust;
using System;
using UnityEngine;

public abstract class BowWeaponItem<T> : WeaponItem<T> where T: BowWeaponDataBlock
{
    private bool _arrowDrawn;
    private float _completeDrawTime;
    private int _currentArrowID;
    private bool _tired;

    protected BowWeaponItem(T db) : base(db)
    {
        this._completeDrawTime = -1f;
    }

    public void ArrowReportHit(IDMain hitMain, ArrowMovement arrow)
    {
        base.datablock.ArrowReportHit(hitMain, arrow, base.itemRepresentation, base.iface as IBowWeaponItem);
    }

    public void ArrowReportMiss(ArrowMovement arrow)
    {
        base.datablock.ArrowReportMiss(arrow, base.itemRepresentation);
    }

    protected override bool CanAim()
    {
        return (!this.IsReloading() && base.CanAim());
    }

    protected override bool CanSetActivate(bool value)
    {
        return (base.CanSetActivate(value) && (value || (base.nextPrimaryAttackTime <= Time.time)));
    }

    public void ClearArrowID()
    {
        this.currentArrowID = 0;
    }

    public IInventoryItem FindAmmo()
    {
        return base.inventory.FindItem(this.GetDesiredArrow());
    }

    public int GenerateArrowID()
    {
        return Random.Range(1, 0xffff);
    }

    public ItemDataBlock GetDesiredArrow()
    {
        return base.datablock.defaultAmmo;
    }

    public bool IsArrowDrawing()
    {
        return !(this.completeDrawTime == -1f);
    }

    public bool IsArrowDrawingOrDrawn()
    {
        return (this.IsArrowDrawn() || this.IsArrowDrawing());
    }

    public bool IsArrowDrawn()
    {
        return this.arrowDrawn;
    }

    public virtual bool IsReloading()
    {
        return false;
    }

    public override void ItemPreFrame(ref HumanController.InputSample sample)
    {
        ViewModel viewModelInstance = base.viewModelInstance;
        if (sample.attack && (base.nextPrimaryAttackTime <= Time.time))
        {
            if (this.IsArrowDrawn())
            {
                float num = Time.time - this.completeDrawTime;
                if (num > 1f)
                {
                    base.datablock.Local_GetTired(viewModelInstance, base.itemRepresentation, base.iface as IBowWeaponItem, ref sample);
                    this.tired = true;
                }
                if (num > base.datablock.tooTiredLength)
                {
                    base.datablock.Local_CancelArrow(viewModelInstance, base.itemRepresentation, base.iface as IBowWeaponItem, ref sample);
                }
            }
            else if (!this.IsArrowDrawn() && !this.IsArrowDrawing())
            {
                if (this.FindAmmo() == null)
                {
                    Notice.Popup("", "No Arrows!", 4f);
                    this.MakeReadyIn(2f);
                }
                else
                {
                    base.datablock.Local_ReadyArrow(viewModelInstance, base.itemRepresentation, base.iface as IBowWeaponItem, ref sample);
                }
            }
            else if (this.completeDrawTime < Time.time)
            {
                this.arrowDrawn = true;
            }
            if (this.IsArrowDrawingOrDrawn() && ((Time.time - (this.completeDrawTime - 1f)) > 0.5f))
            {
                sample.aim = true;
            }
        }
        else
        {
            if (this.IsArrowDrawn())
            {
                IInventoryItem item2 = this.FindAmmo();
                if (item2 == null)
                {
                    Notice.Popup("", "No Arrows!", 4f);
                    base.datablock.Local_CancelArrow(viewModelInstance, base.itemRepresentation, base.iface as IBowWeaponItem, ref sample);
                }
                else
                {
                    int count = 1;
                    if (item2.Consume(ref count))
                    {
                        base.inventory.RemoveItem(item2.slot);
                    }
                    base.datablock.Local_FireArrow(viewModelInstance, base.itemRepresentation, base.iface as IBowWeaponItem, ref sample);
                }
            }
            else if (this.IsArrowDrawingOrDrawn())
            {
                base.datablock.Local_CancelArrow(viewModelInstance, base.itemRepresentation, base.iface as IBowWeaponItem, ref sample);
            }
            sample.aim = false;
        }
        if (sample.aim)
        {
            sample.yaw *= base.datablock.aimSensitivtyPercent;
            sample.pitch *= base.datablock.aimSensitivtyPercent;
        }
    }

    public void MakeReadyIn(float delay)
    {
        base.nextPrimaryAttackTime = Time.time + delay;
        this.tired = false;
        this.arrowDrawn = false;
        this.completeDrawTime = -1f;
    }

    protected override void OnSetActive(bool isActive)
    {
        if (!isActive)
        {
            this.MakeReadyIn(2f);
        }
        base.OnSetActive(isActive);
    }

    public bool arrowDrawn
    {
        get
        {
            return this._arrowDrawn;
        }
        set
        {
            this._arrowDrawn = value;
        }
    }

    public override bool canPrimaryAttack
    {
        get
        {
            return (Time.time >= base.nextPrimaryAttackTime);
        }
    }

    public override bool canReload
    {
        get
        {
            return true;
        }
    }

    public float completeDrawTime
    {
        get
        {
            return this._completeDrawTime;
        }
        set
        {
            this._completeDrawTime = value;
        }
    }

    public int currentArrowID
    {
        get
        {
            return this._currentArrowID;
        }
        set
        {
            this._currentArrowID = value;
        }
    }

    public bool tired
    {
        get
        {
            return this._tired;
        }
        set
        {
            this._tired = value;
        }
    }
}

