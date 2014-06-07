using Facepunch;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class InventoryHolder : IDLocalCharacter
{
    [NonSerialized]
    private string _animationGroupNameCached;
    [NonSerialized]
    private CacheRef<Inventory> _inventory;
    [NonSerialized]
    private bool hasItem;
    [NonSerialized]
    private bool isPlayerInventory;
    [NonSerialized]
    private ItemRepresentation itemRep;
    [NonSerialized]
    private ulong lastItemUseTime;
    private const string TossItem_RPC = "TOSS";

    public bool BeltUse(int beltNum)
    {
        PlayerInventory inventory;
        IInventoryItem item;
        IHeldItem item2;
        if (!base.dead && (((this.GetPlayerInventory(out inventory) && inventory.GetItem(30 + beltNum, out item)) && (!(item is IHeldItem) || (!(item2 = (IHeldItem) item).active ? item2.canActivate : item2.canDeactivate))) && this.ValidateAntiBeltSpam(NetCull.timeInMillis)))
        {
            base.networkView.RPC<int>("DoBeltUse", RPCMode.Server, beltNum);
            return true;
        }
        return false;
    }

    internal void ClearItemRepresentation(ItemRepresentation value)
    {
        if (this.hasItem && (this.itemRep == value))
        {
            this.itemRep = null;
            this.hasItem = false;
            this._animationGroupNameCached = null;
        }
    }

    [RPC]
    protected void DoBeltUse(int beltNum)
    {
    }

    private bool GetPlayerInventory(out PlayerInventory inventory)
    {
        inventory = this.inventory as PlayerInventory;
        if (inventory == null)
        {
            inventory = null;
            return false;
        }
        inventory = (PlayerInventory) this.inventory;
        return (bool) inventory;
    }

    public void InventoryModified()
    {
        if (base.localControlled)
        {
            RPOS.LocalInventoryModified();
        }
    }

    public void InvokeInputItemPostFrame(object item, ref HumanController.InputSample sample)
    {
        IHeldItem item2 = item as IHeldItem;
        if (item2 != null)
        {
            item2.ItemPostFrame(ref sample);
        }
    }

    public object InvokeInputItemPreFrame(ref HumanController.InputSample sample)
    {
        IHeldItem inputItem = this.inputItem as IHeldItem;
        if (inputItem != null)
        {
            inputItem.ItemPreFrame(ref sample);
        }
        return inputItem;
    }

    public void InvokeInputItemPreRender()
    {
        IHeldItem inputItem = this.inputItem as IHeldItem;
        if (inputItem != null)
        {
            inputItem.PreCameraRender();
        }
    }

    internal void SetItemRepresentation(ItemRepresentation value)
    {
        if (this.itemRep != value)
        {
            this.itemRep = value;
            this.hasItem = (bool) this.itemRep;
            if (this.hasItem)
            {
                this._animationGroupNameCached = this.itemRep.worldAnimationGroupName;
                if ((this._animationGroupNameCached != null) && (this._animationGroupNameCached.Length == 1))
                {
                    this._animationGroupNameCached = null;
                }
            }
            else
            {
                this._animationGroupNameCached = null;
            }
        }
    }

    [RPC, NGCRPCSkip]
    protected void TOSS(BitStream stream, NetworkMessageInfo info)
    {
    }

    public bool TossItem(int slot)
    {
        IInventoryItem item;
        NetworkView networkView = base.networkView;
        if ((networkView == null) || !networkView.isMine)
        {
            return false;
        }
        Inventory inventory = this.inventory;
        if ((inventory == null) || !inventory.GetItem(slot, out item))
        {
            return false;
        }
        NetCull.RPC<byte>((MonoBehaviour) this, "TOSS", RPCMode.Server, Inventory.RPCInteger(slot));
        inventory.NULL_SLOT_FIX_ME(slot);
        return true;
    }

    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        if (info.networkView.isMine)
        {
            this.inventory.RequestFullUpdate();
        }
    }

    private bool ValidateAntiBeltSpam(ulong timestamp)
    {
        ulong timeInMillis = NetCull.timeInMillis;
        if ((timeInMillis + ((ulong) 800L)) >= this.lastItemUseTime)
        {
            this.lastItemUseTime = timeInMillis;
            return true;
        }
        return false;
    }

    public string animationGroupName
    {
        get
        {
            return this._animationGroupNameCached;
        }
    }

    public bool hasItemRepresentation
    {
        get
        {
            return this.hasItem;
        }
    }

    public IInventoryItem inputItem
    {
        get
        {
            Inventory inventory = this.inventory;
            return ((inventory == null) ? null : inventory.activeItem);
        }
    }

    public Inventory inventory
    {
        get
        {
            if (!this._inventory.cached)
            {
                this._inventory = base.GetLocal<Inventory>();
            }
            return this._inventory.value;
        }
    }

    public ItemRepresentation itemRepresentation
    {
        get
        {
            return this.itemRep;
        }
    }

    public ItemModFlags modFlags
    {
        get
        {
            if (this.hasItem && (this.itemRep != null))
            {
                return this.itemRep.modFlags;
            }
            IHeldItem inputItem = this.inputItem as IHeldItem;
            if (!object.ReferenceEquals(inputItem, null))
            {
                return inputItem.modFlags;
            }
            return ItemModFlags.Other;
        }
    }
}

