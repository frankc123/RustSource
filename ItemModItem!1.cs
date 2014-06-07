using System;

public abstract class ItemModItem<T> : InventoryItem<T> where T: ItemModDataBlock
{
    protected ItemModItem(T db) : base(db)
    {
    }

    public override InventoryItem.MergeResult TryCombine(IInventoryItem otherItem)
    {
        IHeldItem item = otherItem as IHeldItem;
        if (item == null)
        {
            return InventoryItem.MergeResult.Failed;
        }
        if (item.freeModSlots <= 0)
        {
            return InventoryItem.MergeResult.Failed;
        }
        if (!(otherItem.datablock is BulletWeaponDataBlock))
        {
            return base.TryCombine(otherItem);
        }
        IHeldItem item2 = otherItem as IHeldItem;
        if (item2.FindMod(base.datablock) != -1)
        {
            return InventoryItem.MergeResult.Failed;
        }
        return InventoryItem.MergeResult.Combined;
    }

    public override InventoryItem.MergeResult TryStack(IInventoryItem otherItem)
    {
        InventoryItem.MergeResult result = this.TryCombine(otherItem);
        if (result == InventoryItem.MergeResult.Failed)
        {
            return base.TryStack(otherItem);
        }
        return result;
    }
}

