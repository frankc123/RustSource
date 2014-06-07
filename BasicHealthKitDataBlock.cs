using System;
using uLink;

public class BasicHealthKitDataBlock : ItemDataBlock
{
    public float healthAddMax = 1f;
    public float healthAddMin = 1f;
    public bool stopsBleeding;

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override InventoryItem.MenuItemResult ExecuteMenuOption(InventoryItem.MenuItem option, IInventoryItem item)
    {
        if (option != InventoryItem.MenuItem.Use)
        {
            return base.ExecuteMenuOption(option, item);
        }
        return InventoryItem.MenuItemResult.DoneOnServer;
    }

    public override string GetItemDescription()
    {
        return "This is a Medical item. Right click, or put in your belt and press the corresponding number key to use it.";
    }

    public virtual IBasicHealthKit ItemAsHealthKit(IInventoryItem item)
    {
        return (item as IBasicHealthKit);
    }

    public override void PopulateInfoWindow(ItemToolTip infoWindow, IInventoryItem tipItem)
    {
        infoWindow.AddItemTitle(this, tipItem, 0f);
        infoWindow.AddSectionTitle("Medical", 15f);
        string text = string.Empty;
        if (this.healthAddMin == this.healthAddMax)
        {
            text = "Heals " + this.healthAddMin + " health.";
        }
        else
        {
            object[] objArray1 = new object[] { "Heals ", this.healthAddMin, " to ", this.healthAddMax, " health." };
            text = string.Concat(objArray1);
        }
        infoWindow.AddBasicLabel(text, 15f);
        infoWindow.FinishPopulating();
    }

    public override int RetreiveMenuOptions(IInventoryItem item, InventoryItem.MenuItem[] results, int offset)
    {
        offset = base.RetreiveMenuOptions(item, results, offset);
        if (item.isInLocalInventory)
        {
            results[offset++] = InventoryItem.MenuItem.Use;
        }
        return offset;
    }

    public virtual void UseItem(IBasicHealthKit hk)
    {
    }

    private sealed class ITEM_TYPE : BasicHealthKit<BasicHealthKitDataBlock>, IBasicHealthKit, IInventoryItem
    {
        public ITEM_TYPE(BasicHealthKitDataBlock BLOCK) : base(BLOCK)
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

