using System;

public class WeaponImpact
{
    public readonly WeaponDataBlock dataBlock;
    public readonly IWeaponItem item;
    public readonly ItemRepresentation itemRep;

    public WeaponImpact(WeaponDataBlock dataBlock, IWeaponItem item, ItemRepresentation itemRep)
    {
        this.dataBlock = dataBlock;
        this.item = item;
        this.itemRep = itemRep;
    }
}

