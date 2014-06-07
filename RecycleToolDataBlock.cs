using System;
using uLink;
using UnityEngine;

public class RecycleToolDataBlock : ToolDataBlock
{
    public override bool CanWork(IToolItem tool, Inventory workbenchInv)
    {
        if (workbenchInv.occupiedSlotCount > 2)
        {
            Debug.Log("Too many items for recycle");
            return false;
        }
        IInventoryItem firstItemNotTool = base.GetFirstItemNotTool(tool, workbenchInv);
        if (!firstItemNotTool.datablock.isRecycleable)
        {
            return false;
        }
        if (!BlueprintDataBlock.FindBlueprintForItem(firstItemNotTool.datablock))
        {
            return false;
        }
        return true;
    }

    public override bool CompleteWork(IToolItem tool, Inventory workbenchInv)
    {
        BlueprintDataBlock block;
        int num5;
        if (!this.CanWork(tool, workbenchInv))
        {
            return false;
        }
        IInventoryItem firstItemNotTool = base.GetFirstItemNotTool(tool, workbenchInv);
        BlueprintDataBlock.FindBlueprintForItem<BlueprintDataBlock>(firstItemNotTool.datablock, out block);
        int uses = 1;
        if (firstItemNotTool.datablock.IsSplittable())
        {
            uses = firstItemNotTool.uses;
        }
        for (int i = 0; i < uses; i++)
        {
            foreach (BlueprintDataBlock.IngredientEntry entry in block.ingredients)
            {
                int num4 = Random.Range(0, 4);
                if ((num4 != 0) && (((num4 == 1) || (num4 == 2)) || (num4 == 3)))
                {
                    workbenchInv.AddItemAmount(entry.Ingredient, entry.amount);
                }
            }
        }
        if (!firstItemNotTool.datablock.IsSplittable())
        {
            num5 = firstItemNotTool.uses;
        }
        else
        {
            num5 = uses;
        }
        if (firstItemNotTool.Consume(ref num5))
        {
            firstItemNotTool.inventory.RemoveItem(firstItemNotTool.slot);
        }
        return true;
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override string GetItemDescription()
    {
        return "This doesn't do anything.. yet";
    }

    public override float GetWorkDuration(IToolItem tool)
    {
        return 15f;
    }

    private sealed class ITEM_TYPE : ResearchToolItem<RecycleToolDataBlock>, IInventoryItem, IResearchToolItem, IToolItem
    {
        public ITEM_TYPE(RecycleToolDataBlock BLOCK) : base(BLOCK)
        {
        }

        int IInventoryItem.AddUses(int count)
        {
            return base.AddUses(count);
        }

        bool IInventoryItem.Consume(ref int count)
        {
            return base.Consume(ref count);
        }

        void IInventoryItem.Deserialize(BitStream stream)
        {
            base.Deserialize(stream);
        }

        bool IInventoryItem.get_active()
        {
            return base.active;
        }

        Character IInventoryItem.get_character()
        {
            return base.character;
        }

        float IInventoryItem.get_condition()
        {
            return base.condition;
        }

        Controllable IInventoryItem.get_controllable()
        {
            return base.controllable;
        }

        Controller IInventoryItem.get_controller()
        {
            return base.controller;
        }

        bool IInventoryItem.get_dirty()
        {
            return base.dirty;
        }

        bool IInventoryItem.get_doNotSave()
        {
            return base.doNotSave;
        }

        IDMain IInventoryItem.get_idMain()
        {
            return base.idMain;
        }

        Inventory IInventoryItem.get_inventory()
        {
            return base.inventory;
        }

        bool IInventoryItem.get_isInLocalInventory()
        {
            return base.isInLocalInventory;
        }

        float IInventoryItem.get_lastUseTime()
        {
            return base.lastUseTime;
        }

        float IInventoryItem.get_maxcondition()
        {
            return base.maxcondition;
        }

        int IInventoryItem.get_slot()
        {
            return base.slot;
        }

        int IInventoryItem.get_uses()
        {
            return base.uses;
        }

        float IInventoryItem.GetConditionPercent()
        {
            return base.GetConditionPercent();
        }

        bool IInventoryItem.IsBroken()
        {
            return base.IsBroken();
        }

        bool IInventoryItem.IsDamaged()
        {
            return base.IsDamaged();
        }

        bool IInventoryItem.MarkDirty()
        {
            return base.MarkDirty();
        }

        void IInventoryItem.Serialize(BitStream stream)
        {
            base.Serialize(stream);
        }

        void IInventoryItem.set_lastUseTime(float value)
        {
            base.lastUseTime = value;
        }

        void IInventoryItem.SetCondition(float condition)
        {
            base.SetCondition(condition);
        }

        void IInventoryItem.SetMaxCondition(float condition)
        {
            base.SetMaxCondition(condition);
        }

        void IInventoryItem.SetUses(int count)
        {
            base.SetUses(count);
        }

        bool IInventoryItem.TryConditionLoss(float probability, float percentLoss)
        {
            return base.TryConditionLoss(probability, percentLoss);
        }

        ItemDataBlock IInventoryItem.datablock
        {
            get
            {
                return base.datablock;
            }
        }
    }
}

