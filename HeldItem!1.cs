using InventoryExtensions;
using System;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public abstract class HeldItem<T> : InventoryItem<T> where T: HeldItemDataBlock
{
    protected ItemModDataBlock[] _itemMods;
    private ItemRepresentation _itemRep;
    private ViewModel _vm;

    public HeldItem(T datablock) : base(datablock)
    {
        this._itemMods = new ItemModDataBlock[5];
    }

    public void AddMod(ItemModDataBlock mod)
    {
        this.RecalculateMods();
        int usedModSlots = this.usedModSlots;
        this._itemMods[usedModSlots] = mod;
        this.RecalculateMods();
        this.OnModAdded(mod);
        base.MarkDirty();
    }

    protected virtual bool CanAim()
    {
        return true;
    }

    protected virtual bool CanSetActivate(bool value)
    {
        if (value && base.IsBroken())
        {
            return false;
        }
        return true;
    }

    public override void ConditionChanged(float oldCondition)
    {
    }

    protected virtual void CreateViewModel()
    {
        this.DestroyViewModel();
        if ((base.datablock._viewModelPrefab != null) && !actor.forceThirdPerson)
        {
            this._vm = (ViewModel) Object.Instantiate(base.datablock._viewModelPrefab);
            this._vm.PlayDeployAnimation();
            if (base.datablock.deploySound != null)
            {
                base.datablock.deploySound.Play((float) 1f);
            }
            CameraFX.ReplaceViewModel(this._vm, this._itemRep, base.iface as IHeldItem, false);
        }
    }

    protected virtual void DestroyViewModel()
    {
        if (this._vm != null)
        {
            CameraFX.RemoveViewModel(ref this._vm, true, false);
        }
    }

    public int FindMod(ItemModDataBlock mod)
    {
        if (mod != null)
        {
            for (int i = 0; i < 5; i++)
            {
                if (this._itemMods[i] == mod)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public virtual void ItemPostFrame(ref HumanController.InputSample sample)
    {
    }

    public virtual void ItemPreFrame(ref HumanController.InputSample sample)
    {
        if ((sample.attack2 && base.datablock.secondaryFireAims) && this.CanAim())
        {
            sample.attack2 = false;
            sample.aim = true;
            sample.yaw *= base.datablock.aimSensitivtyPercent;
            sample.pitch *= base.datablock.aimSensitivtyPercent;
        }
    }

    public void OnActivate()
    {
        this.OnSetActive(true);
    }

    protected override void OnBitStreamRead(BitStream stream)
    {
        base.OnBitStreamRead(stream);
        this.SetTotalModSlotCount(stream.ReadInvInt());
        this.SetUsedModSlotCount(stream.ReadInvInt());
        int usedModSlots = this.usedModSlots;
        for (int i = 0; i < 5; i++)
        {
            if (i < usedModSlots)
            {
                this._itemMods[i] = DatablockDictionary.GetByUniqueID(stream.ReadInt32()) as ItemModDataBlock;
            }
            else
            {
                this._itemMods[i] = null;
            }
        }
    }

    protected override void OnBitStreamWrite(BitStream stream)
    {
        base.OnBitStreamWrite(stream);
        stream.WriteInvInt(this.totalModSlots);
        int usedModSlots = this.usedModSlots;
        stream.WriteInvInt(usedModSlots);
        for (int i = 0; i < usedModSlots; i++)
        {
            stream.WriteInt32(this._itemMods[i].uniqueID);
        }
    }

    public void OnDeactivate()
    {
        this.OnSetActive(false);
    }

    protected virtual void OnModAdded(ItemModDataBlock mod)
    {
    }

    public override void OnMovedTo(Inventory toInv, int toSlot)
    {
        if (base.active)
        {
            base.inventory.DeactivateItem();
        }
    }

    protected virtual void OnSetActive(bool isActive)
    {
        if (isActive)
        {
            this.CreateViewModel();
        }
        else
        {
            this.DestroyViewModel();
        }
    }

    public virtual void PreCameraRender()
    {
    }

    private void RecalculateMods()
    {
        int num = 0;
        for (int i = 0; i < 5; i++)
        {
            if (this._itemMods[i] != null)
            {
                num++;
            }
        }
        this.usedModSlots = num;
    }

    protected virtual void SetItemRepresentation(ItemRepresentation itemRep)
    {
        this._itemRep = itemRep;
        if (this._itemRep != null)
        {
            if (this._itemRep.datablock != base.datablock)
            {
                Debug.Log("yea the code below wasn't pointless..");
                this._itemRep.SetDataBlockFromHeldItem<T>((HeldItem<T>) this);
            }
            this._itemRep.SetParent(base.inventory.gameObject);
        }
    }

    public void SetTotalModSlotCount(int count)
    {
        this.totalModSlots = count;
    }

    public void SetUsedModSlotCount(int count)
    {
        this.usedModSlots = count;
    }

    public bool canActivate
    {
        get
        {
            return this.CanSetActivate(true);
        }
    }

    public bool canAim
    {
        get
        {
            return this.CanAim();
        }
    }

    public bool canDeactivate
    {
        get
        {
            return this.CanSetActivate(false);
        }
    }

    public int freeModSlots
    {
        get
        {
            return (this.totalModSlots - this.usedModSlots);
        }
    }

    public ItemModDataBlock[] itemMods
    {
        get
        {
            return this._itemMods;
        }
    }

    public ItemRepresentation itemRepresentation
    {
        get
        {
            return this._itemRep;
        }
        set
        {
            this.SetItemRepresentation(value);
        }
    }

    public ItemModFlags modFlags
    {
        get
        {
            ItemModFlags other = ItemModFlags.Other;
            if (this._itemMods != null)
            {
                foreach (ItemModDataBlock block in this._itemMods)
                {
                    if (block != null)
                    {
                        other |= block.modFlag;
                    }
                }
            }
            return other;
        }
    }

    public int totalModSlots { get; private set; }

    public int usedModSlots { get; private set; }

    public ViewModel viewModelInstance
    {
        get
        {
            return this._vm;
        }
        protected set
        {
            this._vm = value;
        }
    }
}

