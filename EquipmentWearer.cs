using System;
using uLink;
using UnityEngine;

public class EquipmentWearer : IDLocalCharacter
{
    [NonSerialized]
    private CacheRef<ArmorModelRenderer> _armorModelRenderer;
    [NonSerialized]
    private CacheRef<InventoryHolder> _inventoryHolder;
    [NonSerialized]
    private CacheRef<ProtectionTakeDamage> _protectionTakeDamage;

    [RPC]
    protected void ArmorData(byte[] data)
    {
        DamageTypeList armor = new DamageTypeList();
        BitStream stream = new BitStream(data, false);
        for (int i = 0; i < 6; i++)
        {
            armor[i] = stream.ReadSingle();
        }
        ProtectionTakeDamage takeDamage = this.takeDamage;
        if (takeDamage != null)
        {
            takeDamage.SetArmorValues(armor);
        }
        if (base.localPlayerControlled)
        {
            RPOS.SetEquipmentDirty();
        }
    }

    public void CalculateArmor()
    {
        InventoryHolder inventoryHolder = this.inventoryHolder;
        ProtectionTakeDamage takeDamage = this.takeDamage;
        if ((inventoryHolder != null) && (takeDamage != null))
        {
            DamageTypeList damageList = new DamageTypeList();
            for (int i = 0x24; i < 40; i++)
            {
                ArmorDataBlock block;
                IInventoryItem item;
                if (inventoryHolder.inventory.GetItem(i, out item) && ((block = item.datablock as ArmorDataBlock) != null))
                {
                    block.AddToDamageTypeList(damageList);
                }
            }
            if (takeDamage != null)
            {
                takeDamage.SetArmorValues(damageList);
            }
        }
    }

    public void EquipmentUpdate()
    {
        this.CalculateArmor();
    }

    public ArmorModelRenderer armorModelRenderer
    {
        get
        {
            if (!this._armorModelRenderer.cached)
            {
                this._armorModelRenderer = base.GetLocal<ArmorModelRenderer>();
            }
            return this._armorModelRenderer.value;
        }
    }

    public InventoryHolder inventoryHolder
    {
        get
        {
            if (!this._inventoryHolder.cached)
            {
                this._inventoryHolder = base.GetLocal<InventoryHolder>();
            }
            return this._inventoryHolder.value;
        }
    }

    public ProtectionTakeDamage takeDamage
    {
        get
        {
            if (!this._protectionTakeDamage.cached)
            {
                this._protectionTakeDamage = base.takeDamage as ProtectionTakeDamage;
            }
            return this._protectionTakeDamage.value;
        }
    }
}

