using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class ConsumableDataBlock : ItemDataBlock
{
    public float antiRads;
    public int burnTemp = 10;
    public float calories;
    public bool cookable;
    public ItemDataBlock cookedVersion;
    public int cookHeatRequirement = 1;
    public float healthToHeal;
    public float litresOfWater;
    public int numToCookPerTick;
    public float poisonAmount;

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override InventoryItem.MenuItemResult ExecuteMenuOption(InventoryItem.MenuItem option, IInventoryItem item)
    {
        if (option == this.GetConsumeMenuItem())
        {
            return InventoryItem.MenuItemResult.DoneOnServer;
        }
        return base.ExecuteMenuOption(option, item);
    }

    public InventoryItem.MenuItem GetConsumeMenuItem()
    {
        if ((this.calories > 0f) && (this.litresOfWater <= 0f))
        {
            return InventoryItem.MenuItem.Eat;
        }
        if ((this.litresOfWater > 0f) && (this.calories <= 0f))
        {
            return InventoryItem.MenuItem.Drink;
        }
        return InventoryItem.MenuItem.Consume;
    }

    public override string GetItemDescription()
    {
        string str = string.Empty;
        if ((this.calories > 0f) && (this.litresOfWater > 0f))
        {
            str = str + "This is a food item, consuming it (via right click) will replenish your food and water. ";
        }
        else if (this.calories > 0f)
        {
            str = str + "This is a food item, eating it will satisfy some of your hunger. ";
        }
        else if (this.litresOfWater > 0f)
        {
            str = str + "This is a beverage, drinking it will quench some of your thirst. ";
        }
        if (this.antiRads > 0f)
        {
            str = str + "This item has some anti-radioactive properties, consuming it will lower your radiation level. ";
        }
        if (this.healthToHeal > 0f)
        {
            str = str + "It will also provide minor healing";
        }
        return str;
    }

    public override void PopulateInfoWindow(ItemToolTip infoWindow, IInventoryItem tipItem)
    {
        infoWindow.AddItemTitle(this, tipItem, 0f);
        infoWindow.AddSectionTitle("Consumable", 15f);
        if (this.calories > 0f)
        {
            infoWindow.AddBasicLabel(this.calories + " Calories", 15f);
        }
        if (this.litresOfWater > 0f)
        {
            infoWindow.AddBasicLabel(this.litresOfWater + "L Water", 15f);
        }
        if (this.antiRads > 0f)
        {
            infoWindow.AddBasicLabel("-" + this.antiRads + " Rads", 15f);
        }
        if (this.healthToHeal != 0f)
        {
            infoWindow.AddBasicLabel(((this.healthToHeal <= 0f) ? string.Empty : "+") + this.healthToHeal + " Health", 15f);
        }
        infoWindow.AddItemDescription(this, 15f);
        infoWindow.FinishPopulating();
    }

    public override int RetreiveMenuOptions(IInventoryItem item, InventoryItem.MenuItem[] results, int offset)
    {
        offset = base.RetreiveMenuOptions(item, results, offset);
        if (item.isInLocalInventory)
        {
            results[offset++] = this.GetConsumeMenuItem();
        }
        return offset;
    }

    public virtual void UseItem(IConsumableItem item)
    {
        Inventory inventory = item.inventory;
        Metabolism local = inventory.GetLocal<Metabolism>();
        if ((local != null) && local.CanConsumeYet())
        {
            local.MarkConsumptionTime();
            float numCalories = Mathf.Min(local.GetRemainingCaloricSpace(), this.calories);
            if (this.calories > 0f)
            {
                local.AddCalories(numCalories);
            }
            if (this.litresOfWater > 0f)
            {
                local.AddWater(this.litresOfWater);
            }
            if (this.antiRads > 0f)
            {
                local.AddAntiRad(this.antiRads);
            }
            if (this.healthToHeal != 0f)
            {
                HumanBodyTakeDamage damage = inventory.GetLocal<HumanBodyTakeDamage>();
                if (damage != null)
                {
                    if (this.healthToHeal > 0f)
                    {
                        damage.HealOverTime(this.healthToHeal);
                    }
                    else
                    {
                        TakeDamage.HurtSelf(inventory.idMain, Mathf.Abs(this.healthToHeal), null);
                    }
                }
            }
            if (this.poisonAmount > 0f)
            {
                local.AddPoison(this.poisonAmount);
            }
            int count = 1;
            if (item.Consume(ref count))
            {
                inventory.RemoveItem(item.slot);
            }
        }
    }

    private sealed class ITEM_TYPE : ConsumableItem<ConsumableDataBlock>, IConsumableItem, ICookableItem, IInventoryItem
    {
        public ITEM_TYPE(ConsumableDataBlock BLOCK) : base(BLOCK)
        {
        }

        bool ICookableItem.GetCookableInfo(out int consumeCount, out ItemDataBlock cookedVersion, out int cookedCount, out int cookTempMin, out int burnTemp)
        {
            return base.GetCookableInfo(out consumeCount, out cookedVersion, out cookedCount, out cookTempMin, out burnTemp);
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

