using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class ResourceTypeItem<T> : InventoryItem<T> where T: ResourceTypeItemDataBlock
{
    protected float _lastUseTime;

    protected ResourceTypeItem(T db) : base(db)
    {
    }

    public bool GetCookableInfo(out int consumeCount, out ItemDataBlock cookedVersion, out int cookedCount, out int cookTempMin, out int burnTemp)
    {
        burnTemp = 0x3b9ac9ff;
        cookTempMin = base.datablock.cookHeatRequirement;
        cookedVersion = base.datablock.cookedVersion;
        if (base.datablock.cookable && (cookedVersion != null))
        {
            consumeCount = Mathf.Min(2, base.uses);
            cookedCount = consumeCount * Random.Range(base.datablock.numToGiveCookedMin, base.datablock.numToGiveCookedMax + 1);
            if (cookedCount == 0)
            {
                consumeCount = 0;
                return false;
            }
            return true;
        }
        cookedCount = consumeCount = 0;
        return false;
    }

    public bool flammable
    {
        get
        {
            return base.datablock.flammable;
        }
    }
}

