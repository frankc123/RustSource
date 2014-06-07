using Facepunch;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class ItemDataBlock : Datablock, IComparable<ItemDataBlock>
{
    [HideInInspector]
    public Inventory.SlotFlags _itemFlags;
    public float _maxCondition = 1f;
    public int _maxUses = 1;
    public int _minUsesForDisplay = 1;
    public int _spawnUsesMax = 1;
    public int _spawnUsesMin = 1;
    public bool _splittable;
    public ItemCategory category = ItemCategory.Misc;
    public CombineRecipe[] Combinations;
    public AudioClip combinedSound;
    public bool doesLoseCondition;
    public AudioClip equippedSound;
    public string icon;
    [NonSerialized, HideInInspector]
    public Texture iconTex;
    public bool isRecycleable = true;
    public bool isRepairable = true;
    public bool isResearchable = true;
    public string itemDescriptionOverride = string.Empty;
    public const string kUnknownIconPath = "content/item/tex/unknown";
    public TransientMode transientMode;
    public AudioClip unEquippedSound;
    public AudioClip UsedSound;

    public void ConfigureItemPickup(ItemPickup pickup, int amount)
    {
    }

    protected virtual IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public IInventoryItem CreateItem()
    {
        IInventoryItem item = this.ConstructItem();
        this.InstallData(item);
        return item;
    }

    public bool DoesLoseCondition()
    {
        return this.doesLoseCondition;
    }

    public virtual InventoryItem.MenuItemResult ExecuteMenuOption(InventoryItem.MenuItem option, IInventoryItem item)
    {
        InventoryItem.MenuItem item2 = option;
        if (item2 != InventoryItem.MenuItem.Info)
        {
            if (item2 != InventoryItem.MenuItem.Split)
            {
                return InventoryItem.MenuItemResult.Unhandled;
            }
        }
        else
        {
            RPOS.OpenInfoWindow(this);
            return InventoryItem.MenuItemResult.DoneOnClient;
        }
        item.inventory.SplitStack(item.slot);
        return InventoryItem.MenuItemResult.Complete;
    }

    public Texture GetIconTexture()
    {
        if ((this.iconTex == null) && !Bundling.Load<Texture>(this.icon, out this.iconTex))
        {
            Bundling.Load<Texture>("content/item/tex/unknown", out this.iconTex);
        }
        return this.iconTex;
    }

    public virtual string GetItemDescription()
    {
        if (this.itemDescriptionOverride.Length > 0)
        {
            return this.itemDescriptionOverride;
        }
        return "No item description available";
    }

    public CombineRecipe GetMatchingRecipe(ItemDataBlock db)
    {
        if ((this.Combinations != null) && (this.Combinations.Length != 0))
        {
            foreach (CombineRecipe recipe in this.Combinations)
            {
                if (recipe.droppedOnType == db)
                {
                    return recipe;
                }
            }
        }
        return null;
    }

    public virtual byte GetMaxEligableSlots()
    {
        return 0;
    }

    public int GetMinUsesForDisplay()
    {
        return this._minUsesForDisplay;
    }

    public int GetRandomSpawnUses()
    {
        return Random.Range(this._spawnUsesMin, this._spawnUsesMax + 1);
    }

    public virtual void InstallData(IInventoryItem item)
    {
        item.SetUses(1);
        item.SetMaxCondition(1f);
        item.SetCondition(1f);
    }

    public virtual bool IsSplittable()
    {
        return this._splittable;
    }

    public static bool LoadIconOrUnknown<TTex>(string iconPath, ref TTex tex) where TTex: Texture
    {
        return ((((TTex) tex) != null) || LoadIconOrUnknownForced<TTex>(iconPath, out tex));
    }

    public static bool LoadIconOrUnknownForced<TTex>(string iconPath, out TTex tex) where TTex: Texture
    {
        if (!Bundling.Load<TTex>(iconPath, out tex))
        {
            return Bundling.Load<TTex>("content/item/tex/unknown", out tex);
        }
        return true;
    }

    public virtual void OnItemEvent(InventoryItem.ItemEvent itemEvent)
    {
        switch (itemEvent)
        {
            case InventoryItem.ItemEvent.Equipped:
                if (this.equippedSound != null)
                {
                    this.equippedSound.Play((float) 1f);
                }
                break;

            case InventoryItem.ItemEvent.UnEquipped:
                if (this.unEquippedSound != null)
                {
                    this.unEquippedSound.Play((float) 1f);
                }
                break;

            case InventoryItem.ItemEvent.Combined:
                if (this.combinedSound != null)
                {
                    this.combinedSound.Play((float) 1f);
                }
                break;

            case InventoryItem.ItemEvent.Used:
                if (this.UsedSound != null)
                {
                    this.UsedSound.Play((float) 1f);
                }
                break;
        }
    }

    public virtual void PopulateInfoWindow(ItemToolTip infoWindow, IInventoryItem item)
    {
        infoWindow.AddItemTitle(this, item, 0f);
        infoWindow.AddConditionInfo(item);
        infoWindow.AddItemDescription(this, 15f);
        infoWindow.FinishPopulating();
    }

    protected virtual void PostInstallJsonProperties(IInventoryItem item)
    {
    }

    protected virtual void PreInstallJsonProperties(IInventoryItem item)
    {
    }

    public virtual int RetreiveMenuOptions(IInventoryItem item, InventoryItem.MenuItem[] results, int offset)
    {
        if ((this._splittable && (item.uses > 1)) && item.isInLocalInventory)
        {
            results[offset++] = InventoryItem.MenuItem.Split;
        }
        return offset;
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<int>((int) this._itemFlags, new object[0]);
        stream.Write<int>(this._maxUses, new object[0]);
        stream.Write<bool>(this._splittable, new object[0]);
        stream.Write<byte>((byte) this.transientMode, new object[0]);
        stream.Write<bool>(this.isResearchable, new object[0]);
        stream.Write<bool>(this.isResearchable, new object[0]);
        stream.Write<bool>(this.isRecycleable, new object[0]);
    }

    int IComparable<ItemDataBlock>.CompareTo(ItemDataBlock other)
    {
        return this.CompareTo(other);
    }

    public bool doesNotSave
    {
        get
        {
            return ((this.transientMode & TransientMode.DoesNotSave) == TransientMode.DoesNotSave);
        }
    }

    public bool saves
    {
        get
        {
            return ((this.transientMode & TransientMode.DoesNotSave) != TransientMode.DoesNotSave);
        }
    }

    public bool transferable
    {
        get
        {
            return ((this.transientMode & TransientMode.Untransferable) != TransientMode.Untransferable);
        }
    }

    public bool untransferable
    {
        get
        {
            return ((this.transientMode & TransientMode.Untransferable) == TransientMode.Untransferable);
        }
    }

    [Serializable]
    public class CombineRecipe
    {
        public int amountToGive = 1;
        public int amountToLose = 1;
        public int amountToLoseOther = 1;
        public ItemDataBlock droppedOnType;
        public ItemDataBlock resultItem;
    }

    private sealed class ITEM_TYPE : InventoryItem<ItemDataBlock>, IInventoryItem
    {
        public ITEM_TYPE(ItemDataBlock BLOCK) : base(BLOCK)
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
    public enum ItemCategory
    {
        Survival,
        Weapons,
        Ammo,
        Misc,
        Medical,
        Armor,
        Blueprint,
        Food,
        Tools,
        Mods,
        Parts,
        Resource
    }

    public enum TransientMode
    {
        Full,
        DoesNotSave,
        Untransferable,
        None
    }
}

