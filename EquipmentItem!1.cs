using System;

public abstract class EquipmentItem<T> : InventoryItem<T> where T: EquipmentDataBlock
{
    protected EquipmentItem(T db) : base(db)
    {
    }

    public void OnEquipped()
    {
        base.datablock.OnEquipped(base.iface as IEquipmentItem);
    }

    public void OnUnEquipped()
    {
        base.datablock.OnUnEquipped(base.iface as IEquipmentItem);
    }
}

