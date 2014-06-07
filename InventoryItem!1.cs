using System;
using uLink;
using UnityEngine;

public abstract class InventoryItem<DB> : InventoryItem where DB: ItemDataBlock
{
    public readonly DB datablock;

    protected InventoryItem(DB datablock) : base(datablock)
    {
        this.datablock = datablock;
    }

    protected override void OnBitStreamRead(BitStream stream)
    {
        InventoryItem.DeserializeSharedProperties(stream, this, this.datablock);
    }

    protected override void OnBitStreamWrite(BitStream stream)
    {
        InventoryItem.SerializeSharedProperties(stream, this, this.datablock);
    }

    public override InventoryItem.MenuItemResult OnMenuOption(InventoryItem.MenuItem option)
    {
        InventoryItem.MenuItemResult result = this.datablock.ExecuteMenuOption(option, base.iface);
        switch (result)
        {
            case InventoryItem.MenuItemResult.Unhandled:
            case InventoryItem.MenuItemResult.DoneOnServer:
                base.inventory.NetworkItemAction(base.slot, option);
                break;
        }
        return result;
    }

    public override void OnMovedTo(Inventory inv, int slot)
    {
    }

    public override string ToString()
    {
        Inventory inventory = base.inventory;
        string str = (this.datablock == null) ? tostringhelper<DB>.nullDatablockString : this.datablock.name;
        if (inventory != null)
        {
            object[] args = new object[] { str, inventory.name, base.slot, base.uses };
            return string.Format("[{0} (on {1}[{2}]) with ({3} uses)]", args);
        }
        return string.Format("[{0} (unbound slot {1}) with ({2} uses)]", str, base.slot, base.uses);
    }

    public override InventoryItem.MergeResult TryCombine(IInventoryItem other)
    {
        ItemDataBlock datablock = other.datablock;
        ItemDataBlock.CombineRecipe matchingRecipe = this.datablock.GetMatchingRecipe(datablock);
        if (matchingRecipe != null)
        {
            int uses = other.uses;
            if (uses < matchingRecipe.amountToLoseOther)
            {
                return InventoryItem.MergeResult.Failed;
            }
            if (base.uses < matchingRecipe.amountToLose)
            {
                return InventoryItem.MergeResult.Failed;
            }
            Inventory inventory = other.inventory;
            int amount = 0;
            int a = base.uses / matchingRecipe.amountToLose;
            int b = uses / matchingRecipe.amountToLoseOther;
            amount = Mathf.Min(a, b);
            int num5 = 0;
            if (matchingRecipe.resultItem.IsSplittable())
            {
                num5 = Mathf.CeilToInt(((float) amount) / ((float) num5));
            }
            else
            {
                num5 = amount;
            }
            int vacantSlotCount = inventory.vacantSlotCount;
            if (num5 <= vacantSlotCount)
            {
                int count = amount * matchingRecipe.amountToLoseOther;
                if (other.Consume(ref count))
                {
                    inventory.RemoveItem(other.slot);
                }
                inventory.AddItemAmount(matchingRecipe.resultItem, amount, Inventory.AmountMode.Default);
                int numWant = amount * matchingRecipe.amountToLose;
                if (base.Consume(ref numWant))
                {
                    base.inventory.RemoveItem(base.slot);
                }
            }
        }
        return InventoryItem.MergeResult.Failed;
    }

    public override InventoryItem.MergeResult TryStack(IInventoryItem other)
    {
        int uses = base.uses;
        if (uses != 0)
        {
            DB datablock = other.datablock as DB;
            if ((datablock != null) && (datablock == this.datablock))
            {
                if (other.uses == base.maxUses)
                {
                    return InventoryItem.MergeResult.Failed;
                }
                if (this.datablock.IsSplittable())
                {
                    IInventoryItem item = other;
                    int numWant = item.AddUses(uses);
                    if (numWant == 0)
                    {
                        return InventoryItem.MergeResult.Failed;
                    }
                    if (item2.Consume(ref numWant))
                    {
                        item2.inventory.RemoveItem(item2.slot);
                    }
                    return InventoryItem.MergeResult.Merged;
                }
            }
        }
        return InventoryItem.MergeResult.Failed;
    }

    protected sealed override ItemDataBlock __infrastructure_db
    {
        get
        {
            return this.datablock;
        }
    }

    public bool doNotSave
    {
        get
        {
            return ((this.datablock != null) && this.datablock.doesNotSave);
        }
    }

    public override string toolTip
    {
        get
        {
            string conditionString = this.GetConditionString();
            if (string.IsNullOrEmpty(conditionString))
            {
                return this.datablock.name;
            }
            return (conditionString + " " + this.datablock.name);
        }
    }

    private static class tostringhelper
    {
        public static readonly string nullDatablockString;

        static tostringhelper()
        {
            InventoryItem<DB>.tostringhelper.nullDatablockString = string.Format("NULL<{0}>", typeof(DB).FullName);
        }
    }
}

