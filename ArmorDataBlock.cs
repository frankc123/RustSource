using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class ArmorDataBlock : EquipmentDataBlock
{
    [SerializeField]
    protected ArmorModel armorModel;
    public DamageTypeList armorValues;

    public void AddToDamageTypeList(DamageTypeList damageList)
    {
        for (int i = 0; i < 6; i++)
        {
            DamageTypeList list;
            int num2;
            float num3 = list[num2];
            (list = damageList)[num2 = i] = num3 + this.armorValues[i];
        }
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public TArmorModel GetArmorModel<TArmorModel>() where TArmorModel: ArmorModel, new()
    {
        return (TArmorModel) this.GetArmorModel(ArmorModelSlotUtility.GetArmorModelSlotForClass<TArmorModel>());
    }

    public ArmorModel GetArmorModel(ArmorModelSlot slot)
    {
        if (this.armorModel == null)
        {
            Debug.LogWarning("No armorModel set to datablock " + this, this);
            return null;
        }
        if (this.armorModel.slot != slot)
        {
            Debug.LogError(string.Format("The armor model for {0} is {1}. Its not for slot {2}", this, this.armorModel.slot, slot), this);
            return null;
        }
        return this.armorModel;
    }

    public bool GetArmorModelSlot(out ArmorModelSlot slot)
    {
        if (this.armorModel == null)
        {
            slot = (ArmorModelSlot) 4;
        }
        else
        {
            slot = this.armorModel.slot;
        }
        return (slot < ((ArmorModelSlot) 4));
    }

    public override string GetItemDescription()
    {
        return "This is an piece of armor. Drag it to it's corresponding slot in the armor window and it will provide additional protection";
    }

    public override void OnEquipped(IEquipmentItem item)
    {
    }

    public override void OnUnEquipped(IEquipmentItem item)
    {
    }

    public override void PopulateInfoWindow(ItemToolTip infoWindow, IInventoryItem tipItem)
    {
        infoWindow.AddItemTitle(this, tipItem, 0f);
        infoWindow.AddConditionInfo(tipItem);
        infoWindow.AddSectionTitle("Protection", 0f);
        for (int i = 0; i < 6; i++)
        {
            if (this.armorValues[i] != 0f)
            {
                float contentHeight = infoWindow.GetContentHeight();
                GameObject obj2 = infoWindow.AddBasicLabel(TakeDamage.DamageIndexToString((DamageTypeIndex) i), 0f);
                GameObject obj3 = infoWindow.AddBasicLabel("+" + ((int) this.armorValues[i]).ToString("N0"), 0f);
                obj3.transform.SetLocalPositionX(145f);
                obj3.GetComponentInChildren<UILabel>().color = Color.green;
                obj2.transform.SetLocalPositionY(-(contentHeight + 10f));
                obj3.transform.SetLocalPositionY(-(contentHeight + 10f));
            }
        }
        infoWindow.AddSectionTitle("Equipment Slot", 20f);
        string text = "Head, Chest, Legs, Feet";
        if ((base._itemFlags & Inventory.SlotFlags.Head) == Inventory.SlotFlags.Head)
        {
            text = "Head";
        }
        else if ((base._itemFlags & Inventory.SlotFlags.Chest) == Inventory.SlotFlags.Chest)
        {
            text = "Chest";
        }
        infoWindow.AddBasicLabel(text, 10f);
        infoWindow.AddItemDescription(this, 15f);
        infoWindow.FinishPopulating();
    }

    private sealed class ITEM_TYPE : ArmorItem<ArmorDataBlock>, IArmorItem, IEquipmentItem, IInventoryItem
    {
        public ITEM_TYPE(ArmorDataBlock BLOCK) : base(BLOCK)
        {
        }

        void IEquipmentItem.OnEquipped()
        {
            base.OnEquipped();
        }

        void IEquipmentItem.OnUnEquipped()
        {
            base.OnUnEquipped();
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

