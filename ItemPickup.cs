using System;
using System.Runtime.InteropServices;
using UnityEngine;

[NGCAutoAddScript, RequireComponent(typeof(Inventory))]
public class ItemPickup : RigidObj, IContextRequestable, IContextRequestableQuick, IContextRequestableText, IContextRequestablePointText, IComponentInterface<IContextRequestable, MonoBehaviour, Contextual>, IComponentInterface<IContextRequestable, MonoBehaviour>, IComponentInterface<IContextRequestable>
{
    [NonSerialized]
    private PickupInfo? info;
    private const string ItemInfo_RPC = "PKIF";
    private const string ItemInfoOne_RPC = "PKIS";
    [NonSerialized]
    private PickupInfo? lastInfo;
    [NonSerialized]
    private string lastString;

    public ItemPickup() : base(RigidObj.FeatureFlags.StreamInitialVelocity)
    {
    }

    public string ContextText(Controllable localControllable)
    {
        if (!base.renderer.enabled)
        {
            return string.Empty;
        }
        if (!this.info.HasValue)
        {
            return "Loading...";
        }
        if (!this.lastInfo.HasValue || !this.lastInfo.Value.Equals(this.info.Value))
        {
            this.lastInfo = this.info;
            this.lastString = string.Format("Take '{0}'", this.info.Value);
        }
        return this.lastString;
    }

    bool IContextRequestablePointText.ContextTextPoint(out Vector3 worldPoint)
    {
        ContextRequestable.PointUtil.SpriteOrOrigin(this, out worldPoint);
        return true;
    }

    protected override void OnDone()
    {
    }

    protected override void OnHide()
    {
        if (base.renderer != null)
        {
            base.renderer.enabled = false;
        }
    }

    protected override void OnShow()
    {
        if (base.renderer != null)
        {
            base.renderer.enabled = true;
        }
    }

    [RPC]
    protected void PKIF(int itemName, byte itemAmount)
    {
        this.StoreItemInfo(DatablockDictionary.GetByUniqueID(itemName), itemAmount);
    }

    [RPC]
    protected void PKIS(int itemName)
    {
        this.StoreItemInfo(DatablockDictionary.GetByUniqueID(itemName), 1);
    }

    private void StoreItemInfo(ItemDataBlock datablock, int uses)
    {
        PickupInfo info;
        info.datablock = datablock;
        info.amount = uses;
        this.info = new PickupInfo?(info);
        info.datablock.ConfigureItemPickup(this, uses);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PickupInfo : IEquatable<ItemPickup.PickupInfo>
    {
        public ItemDataBlock datablock;
        public int amount;
        public bool Equals(ItemPickup.PickupInfo other)
        {
            return ((this.datablock == other.datablock) && (this.amount == other.amount));
        }

        public override int GetHashCode()
        {
            return ((this.datablock == null) ? this.amount : (this.datablock.GetHashCode() ^ this.amount));
        }

        public override bool Equals(object obj)
        {
            return ((obj is ItemPickup.PickupInfo) && this.Equals((ItemPickup.PickupInfo) obj));
        }

        public override string ToString()
        {
            if (this.datablock != null)
            {
                if ((this.amount > 1) && this.datablock.IsSplittable())
                {
                    return string.Format("{0} x{1}", this.datablock.name, this.amount);
                }
                return this.datablock.name;
            }
            if (this.amount > 1)
            {
                return string.Format("null x{0}", this.amount);
            }
            return "null";
        }
    }
}

