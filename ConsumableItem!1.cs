using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class ConsumableItem<T> : InventoryItem<T> where T: ConsumableDataBlock
{
    protected ConsumableItem(T db) : base(db)
    {
    }

    public bool GetCookableInfo(out int consumeCount, out ItemDataBlock cookedVersion, out int cookedCount, out int cookTempMin, out int burnTemp)
    {
        burnTemp = base.datablock.burnTemp;
        cookTempMin = base.datablock.cookHeatRequirement;
        cookedVersion = base.datablock.cookedVersion;
        if (base.datablock.cookable && (cookedVersion != null))
        {
            cookedCount = consumeCount = Mathf.Min(2, base.uses);
            return (consumeCount > 0);
        }
        cookedCount = consumeCount = 0;
        return false;
    }
}

