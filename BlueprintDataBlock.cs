using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class BlueprintDataBlock : ToolDataBlock
{
    public static bool chancesInitalized;
    public float craftingDuration = 20f;
    public static SlotChanceWeightedEntry[] defaultSlotChances;
    public IngredientEntry[] ingredients;
    private List<int> lastCanWorkIngredientCount;
    private List<int> lastCanWorkResult;
    public int numResultItem = 1;
    public bool RequireWorkbench;
    public ItemDataBlock resultItem;

    public BlueprintDataBlock()
    {
        base.icon = "Items/BlueprintIcon";
    }

    public virtual bool CanWork(int amount, Inventory workbenchInv)
    {
        if (this.lastCanWorkResult == null)
        {
            this.lastCanWorkResult = new List<int>();
        }
        else
        {
            this.lastCanWorkResult.Clear();
        }
        if (this.lastCanWorkIngredientCount == null)
        {
            this.lastCanWorkIngredientCount = new List<int>(this.ingredients.Length);
        }
        else
        {
            this.lastCanWorkIngredientCount.Clear();
        }
        if (this.RequireWorkbench)
        {
            CraftingInventory component = workbenchInv.GetComponent<CraftingInventory>();
            if ((component == null) || !component.AtWorkBench())
            {
                return false;
            }
        }
        foreach (IngredientEntry entry in this.ingredients)
        {
            if (entry.amount != 0)
            {
                int item = workbenchInv.CanConsume(entry.Ingredient, entry.amount * amount, this.lastCanWorkResult);
                if (item <= 0)
                {
                    this.lastCanWorkResult.Clear();
                    this.lastCanWorkIngredientCount.Clear();
                    return false;
                }
                this.lastCanWorkIngredientCount.Add(item);
            }
        }
        return true;
    }

    public virtual bool CompleteWork(int amount, Inventory workbenchInv)
    {
        if (!this.CanWork(amount, workbenchInv))
        {
            return false;
        }
        int num = 0;
        for (int i = 0; i < this.ingredients.Length; i++)
        {
            int count = this.ingredients[i].amount * amount;
            if (count != 0)
            {
                int num4 = this.lastCanWorkIngredientCount[i];
                for (int j = 0; j < num4; j++)
                {
                    IInventoryItem item;
                    int slot = this.lastCanWorkResult[num++];
                    if (workbenchInv.GetItem(slot, out item) && item.Consume(ref count))
                    {
                        workbenchInv.RemoveItem(slot);
                    }
                }
            }
        }
        workbenchInv.AddItemAmount(this.resultItem, amount * this.numResultItem);
        return true;
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public virtual void DefaultChancesInit()
    {
        if (!chancesInitalized)
        {
            chancesInitalized = true;
            defaultSlotChances = new SlotChanceWeightedEntry[5];
            defaultSlotChances[0].numSlots = 1;
            defaultSlotChances[1].numSlots = 2;
            defaultSlotChances[2].numSlots = 3;
            defaultSlotChances[3].numSlots = 4;
            defaultSlotChances[4].numSlots = 5;
            defaultSlotChances[0].weight = 50f;
            defaultSlotChances[1].weight = 40f;
            defaultSlotChances[2].weight = 30f;
            defaultSlotChances[3].weight = 20f;
            defaultSlotChances[4].weight = 10f;
        }
    }

    public override InventoryItem.MenuItemResult ExecuteMenuOption(InventoryItem.MenuItem option, IInventoryItem item)
    {
        if (option != InventoryItem.MenuItem.Study)
        {
            return base.ExecuteMenuOption(option, item);
        }
        return InventoryItem.MenuItemResult.DoneOnServer;
    }

    public static bool FindBlueprintForItem(ItemDataBlock item)
    {
        BlueprintDataBlock block;
        return FindBlueprintForItem<BlueprintDataBlock>(item, out block);
    }

    public static bool FindBlueprintForItem<T>(ItemDataBlock item, out T blueprint) where T: BlueprintDataBlock
    {
        foreach (ItemDataBlock block in DatablockDictionary.All)
        {
            T local = block as T;
            if ((local != null) && (local.resultItem == item))
            {
                blueprint = local;
                return true;
            }
        }
        Debug.LogWarning("Could not find blueprint foritem");
        blueprint = null;
        return false;
    }

    public override string GetItemDescription()
    {
        return "This is an item Blueprint. Study it to learn how to craft the item it represents!";
    }

    public override float GetWorkDuration(IToolItem tool)
    {
        return this.craftingDuration;
    }

    public override void InstallData(IInventoryItem item)
    {
        base.InstallData(item);
    }

    public virtual int MaxAmount(Inventory workbenchInv)
    {
        int num = 0x7fffffff;
        foreach (IngredientEntry entry in this.ingredients)
        {
            int totalNum = 0;
            if (workbenchInv.FindItem(entry.Ingredient, out totalNum) != null)
            {
                int num4 = totalNum / entry.amount;
                if (num4 < num)
                {
                    num = num4;
                }
            }
        }
        return ((num != 0x7fffffff) ? num : 0);
    }

    public override void PopulateInfoWindow(ItemToolTip infoWindow, IInventoryItem tipItem)
    {
        infoWindow.AddItemTitle(this, tipItem, 0f);
        infoWindow.AddSectionTitle("Ingredients", 15f);
        for (int i = 0; i < this.ingredients.Length; i++)
        {
            string name = this.ingredients[i].Ingredient.name;
            if (this.ingredients[i].amount > 1)
            {
                name = name + " x" + this.ingredients[i].amount;
            }
            infoWindow.AddBasicLabel(name, 15f);
        }
        infoWindow.AddSectionTitle("Result Item", 15f);
        infoWindow.AddBasicLabel(this.resultItem.name, 15f);
        infoWindow.AddItemDescription(this, 15f);
        infoWindow.FinishPopulating();
    }

    public override int RetreiveMenuOptions(IInventoryItem item, InventoryItem.MenuItem[] results, int offset)
    {
        offset = base.RetreiveMenuOptions(item, results, offset);
        if (item.isInLocalInventory)
        {
            results[offset++] = InventoryItem.MenuItem.Study;
        }
        return offset;
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<float>(this.craftingDuration, new object[0]);
        if (this.ingredients != null)
        {
            foreach (IngredientEntry entry in this.ingredients)
            {
                if (entry != null)
                {
                    if (entry.Ingredient != null)
                    {
                        stream.Write<int>(entry.Ingredient.uniqueID ^ entry.amount, new object[0]);
                    }
                    else
                    {
                        stream.Write<int>(entry.amount, new object[0]);
                    }
                }
            }
        }
        if (this.resultItem != null)
        {
            stream.Write<int>(this.resultItem.uniqueID, new object[0]);
        }
        if (defaultSlotChances != null)
        {
            foreach (SlotChanceWeightedEntry entry2 in defaultSlotChances)
            {
                stream.Write<float>(entry2.weight, new object[0]);
            }
            foreach (SlotChanceWeightedEntry entry3 in defaultSlotChances)
            {
                stream.Write<byte>(entry3.numSlots, new object[0]);
            }
        }
    }

    public virtual void UseItem(IBlueprintItem item)
    {
    }

    [Serializable]
    public class IngredientEntry
    {
        public int amount;
        public ItemDataBlock Ingredient;
    }

    private sealed class ITEM_TYPE : BlueprintItem<BlueprintDataBlock>, IBlueprintItem, IInventoryItem, IToolItem
    {
        public ITEM_TYPE(BlueprintDataBlock BLOCK) : base(BLOCK)
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

    [Serializable]
    public class SlotChanceWeightedEntry : WeightSelection.WeightedEntry
    {
        public byte numSlots;
    }
}

