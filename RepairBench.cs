using System;
using uLink;
using UnityEngine;

[NGCAutoAddScript]
public class RepairBench : IDLocal
{
    public bool CanRepair(Inventory ingredientInv)
    {
        BlueprintDataBlock block;
        IInventoryItem repairItem = this.GetRepairItem();
        if ((repairItem == null) || !repairItem.datablock.isRepairable)
        {
            return false;
        }
        if (!repairItem.IsDamaged())
        {
            return false;
        }
        if (!BlueprintDataBlock.FindBlueprintForItem<BlueprintDataBlock>(repairItem.datablock, out block))
        {
            return false;
        }
        for (int i = 0; i < block.ingredients.Length; i++)
        {
            BlueprintDataBlock.IngredientEntry entry = block.ingredients[i];
            int useCount = Mathf.CeilToInt(block.ingredients[i].amount * this.GetResourceScalar());
            if ((useCount > 0) && (ingredientInv.CanConsume(block.ingredients[i].Ingredient, useCount) <= 0))
            {
                return false;
            }
        }
        return true;
    }

    public bool CompleteRepair(Inventory ingredientInv)
    {
        BlueprintDataBlock block;
        if (!this.CanRepair(ingredientInv))
        {
            return false;
        }
        IInventoryItem repairItem = this.GetRepairItem();
        if (!BlueprintDataBlock.FindBlueprintForItem<BlueprintDataBlock>(repairItem.datablock, out block))
        {
            return false;
        }
        for (int i = 0; i < block.ingredients.Length; i++)
        {
            BlueprintDataBlock.IngredientEntry entry = block.ingredients[i];
            int count = Mathf.RoundToInt(block.ingredients[i].amount * this.GetResourceScalar());
            if (count > 0)
            {
                while (count > 0)
                {
                    int totalNum = 0;
                    IInventoryItem item2 = ingredientInv.FindItem(entry.Ingredient, out totalNum);
                    if (item2 != null)
                    {
                        if (item2.Consume(ref count))
                        {
                            ingredientInv.RemoveItem(item2.slot);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        float num4 = repairItem.maxcondition - repairItem.condition;
        float num5 = (num4 * 0.2f) + 0.05f;
        repairItem.SetMaxCondition(repairItem.maxcondition - num5);
        repairItem.SetCondition(repairItem.maxcondition);
        return true;
    }

    [RPC]
    protected void DoRepair(NetworkMessageInfo info)
    {
    }

    public IInventoryItem GetRepairItem()
    {
        IInventoryItem item;
        base.GetComponent<Inventory>().GetItem(0, out item);
        return item;
    }

    public float GetResourceScalar()
    {
        IInventoryItem repairItem = this.GetRepairItem();
        if (repairItem == null)
        {
            return 0f;
        }
        return ((repairItem.maxcondition - repairItem.condition) * 0.5f);
    }

    public bool HasRepairItem()
    {
        return (this.GetRepairItem() != null);
    }
}

