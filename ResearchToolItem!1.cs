using Rust;
using System;

public abstract class ResearchToolItem<T> : ToolItem<T> where T: ToolDataBlock
{
    protected ResearchToolItem(T db) : base(db)
    {
    }

    public override InventoryItem.MergeResult TryCombine(IInventoryItem otherItem)
    {
        PlayerInventory inventory = base.inventory as PlayerInventory;
        if ((inventory != null) && (otherItem.inventory == inventory))
        {
            ItemDataBlock datablock = otherItem.datablock;
            if ((datablock != null) && datablock.isResearchable)
            {
                BlueprintDataBlock block2;
                if (!inventory.AtWorkBench())
                {
                    Notice.Popup("", "You must be at a workbench to do this.", 4f);
                    return InventoryItem.MergeResult.Failed;
                }
                if (!BlueprintDataBlock.FindBlueprintForItem<BlueprintDataBlock>(otherItem.datablock, out block2))
                {
                    Notice.Popup("", "You can't research this.. No Blueprint Available!...", 4f);
                    return InventoryItem.MergeResult.Failed;
                }
                if (inventory.KnowsBP(block2))
                {
                    Notice.Popup("", "You already know how to make this!", 4f);
                    return InventoryItem.MergeResult.Failed;
                }
                return InventoryItem.MergeResult.Combined;
            }
            Notice.Popup("", "You can't research this", 4f);
        }
        return InventoryItem.MergeResult.Failed;
    }
}

