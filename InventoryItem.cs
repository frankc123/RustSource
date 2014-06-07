using InventoryExtensions;
using System;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public abstract class InventoryItem
{
    public readonly int datablockUniqueID;
    public readonly IInventoryItem iface;
    public const int MAX_SUPPORTED_ITEM_MODS = 5;
    public readonly int maxUses;

    internal InventoryItem(ItemDataBlock datablock)
    {
        this.maxUses = datablock._maxUses;
        this.datablockUniqueID = datablock.uniqueID;
        this.iface = this as IInventoryItem;
    }

    public int AddUses(int count)
    {
        int num;
        int num2;
        if ((count <= 0) || ((num = this.uses) == this.maxUses))
        {
            return 0;
        }
        if ((num2 = num + count) >= this.maxUses)
        {
            this.uses = this.maxUses;
            this.MarkDirty();
            return (this.maxUses - num);
        }
        this.uses = num2;
        this.MarkDirty();
        return count;
    }

    public void BreakIntoPieces()
    {
    }

    public virtual bool CanMoveToSlot(Inventory toinv, int toslot)
    {
        return true;
    }

    public virtual void ConditionChanged(float oldCondition)
    {
    }

    public bool Consume(ref int numWant)
    {
        int uses = this.uses;
        if (uses == 0)
        {
            return true;
        }
        if (numWant != 0)
        {
            if (uses <= numWant)
            {
                numWant -= uses;
                this.uses = 0;
                this.MarkDirty();
                return true;
            }
            this.uses = uses - numWant;
            numWant = 0;
            this.MarkDirty();
        }
        return false;
    }

    public void Deserialize(BitStream stream)
    {
        this.OnBitStreamRead(stream);
    }

    protected static void DeserializeSharedProperties(BitStream stream, InventoryItem item, ItemDataBlock db)
    {
        item.uses = stream.ReadInvInt();
        if (item.datablock.DoesLoseCondition())
        {
            item.condition = stream.ReadSingle();
            item.maxcondition = stream.ReadSingle();
        }
    }

    public float GetConditionForBreak()
    {
        return 0f;
    }

    public float GetConditionPercent()
    {
        return (this.condition / this.maxcondition);
    }

    public virtual string GetConditionString()
    {
        if (!this.datablock.doesLoseCondition)
        {
            return string.Empty;
        }
        if (this.condition > 1f)
        {
            return "Artifact";
        }
        if (this.condition >= 0.8f)
        {
            return "Perfect";
        }
        if (this.condition >= 0.6f)
        {
            return "Quality";
        }
        if (this.condition >= 0.5f)
        {
            return string.Empty;
        }
        if (this.condition >= 0.4f)
        {
            return "Shoddy";
        }
        if (this.condition > 0.0)
        {
            return "Bad";
        }
        if (this.IsBroken())
        {
            return "Broken";
        }
        return "ERROR";
    }

    public bool IsBroken()
    {
        return (this.condition <= this.GetConditionForBreak());
    }

    public bool IsDamaged()
    {
        return ((this.maxcondition - this.condition) > 0.001f);
    }

    public bool MarkDirty()
    {
        Inventory inventory = this.inventory;
        return ((inventory != null) && inventory.MarkSlotDirty(this.slot));
    }

    public virtual void MaxConditionChanged(float oldCondition)
    {
    }

    public virtual void OnAddedTo(Inventory inv, int slot)
    {
        this.inventory = inv;
        this.slot = slot;
    }

    protected abstract void OnBitStreamRead(BitStream stream);
    protected abstract void OnBitStreamWrite(BitStream stream);
    public abstract MenuItemResult OnMenuOption(MenuItem option);
    public abstract void OnMovedTo(Inventory inv, int slot);
    public void Serialize(BitStream stream)
    {
        this.OnBitStreamWrite(stream);
    }

    protected static void SerializeSharedProperties(BitStream stream, InventoryItem item, ItemDataBlock db)
    {
        stream.WriteInvInt(item.uses);
        if (item.datablock.DoesLoseCondition())
        {
            stream.WriteSingle(item.condition);
            stream.WriteSingle(item.maxcondition);
        }
    }

    public void SetCondition(float newcondition)
    {
        float condition = this.condition;
        this.condition = Mathf.Clamp(newcondition, 0f, this.maxcondition);
        this.ConditionChanged(condition);
        this.MarkDirty();
    }

    public void SetMaxCondition(float newmaxcondition)
    {
        float maxcondition = this.maxcondition;
        this.maxcondition = Mathf.Clamp(newmaxcondition, 0.01f, 1f);
        this.MaxConditionChanged(maxcondition);
        this.MarkDirty();
    }

    public void SetUses(int count)
    {
        int uses = this.uses;
        if ((count < 0) || (count > this.maxUses))
        {
            count = this.maxUses;
        }
        if (count != uses)
        {
            this.uses = count;
            this.MarkDirty();
        }
    }

    public abstract MergeResult TryCombine(IInventoryItem other);
    public bool TryConditionLoss(float probability, float percentLoss)
    {
        return false;
    }

    public abstract MergeResult TryStack(IInventoryItem other);

    protected abstract ItemDataBlock __infrastructure_db { get; }

    public bool active
    {
        get
        {
            Inventory inventory = this.inventory;
            return ((inventory != null) && (inventory.activeItem == this));
        }
    }

    public Character character
    {
        get
        {
            Inventory inventory = this.inventory;
            return ((inventory == null) ? null : (inventory.idMain as Character));
        }
    }

    public float condition { get; private set; }

    public Controllable controllable
    {
        get
        {
            Character character;
            Inventory inventory = this.inventory;
            return (((inventory == null) || ((character = inventory.idMain as Character) == null)) ? null : character.controllable);
        }
    }

    public Controller controller
    {
        get
        {
            Character character;
            Inventory inventory = this.inventory;
            return (((inventory == null) || ((character = inventory.idMain as Character) == null)) ? null : character.controller);
        }
    }

    public ItemDataBlock datablock
    {
        get
        {
            return this.__infrastructure_db;
        }
    }

    public bool dirty
    {
        get
        {
            return ((this.inventory != null) && this.inventory.IsSlotDirty(this.slot));
        }
    }

    public IDMain idMain
    {
        get
        {
            Inventory inventory = this.inventory;
            return ((inventory == null) ? null : inventory.idMain);
        }
    }

    public Inventory inventory { get; private set; }

    public bool isInLocalInventory
    {
        get
        {
            Character character;
            Inventory inventory = this.inventory;
            return (((inventory != null) && ((character = inventory.idMain as Character) != null)) && character.localPlayerControlled);
        }
    }

    public float lastUseTime { get; set; }

    public float maxcondition { get; private set; }

    public int slot { get; private set; }

    public abstract string toolTip { get; }

    public int uses { get; private set; }

    public enum ItemEvent
    {
        None,
        Equipped,
        UnEquipped,
        Combined,
        Used
    }

    public enum MenuItem : byte
    {
        Consume = 8,
        Drink = 7,
        Eat = 6,
        Info = 1,
        Split = 5,
        Status = 2,
        Study = 4,
        Unload = 9,
        Use = 3
    }

    public enum MenuItemResult : byte
    {
        Complete = 4,
        DoneOnClient = 3,
        DoneOnServer = 1,
        DoneOnServerNotYetClient = 2,
        Unhandled = 0
    }

    public enum MergeResult
    {
        Failed,
        Merged,
        Combined
    }
}

