using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class PlayerInventory : CraftingInventory, FixedSizeInventory
{
    private const int _beltSpace = 6;
    private List<BlueprintDataBlock> _boundBPs;
    [NonSerialized]
    private EquipmentWearer _equipmentWearer;
    private const int _equipSpace = 4;
    private const int _storageSpace = 30;
    public const int BeltEnd = 0x24;
    public const int BeltStart = 30;
    public bool bpDirty = true;
    public const int EquipmentEnd = 40;
    public const int EquipmentStart = 0x24;
    public const int NumBeltItems = 6;
    public const int NumEquipItems = 4;
    public const int NumStorageItems = 30;
    public const int StorageEnd = 30;
    public const int StorageStart = 0;
    private const int TotalSlotCount = 40;

    protected override bool CheckSlotFlags(Inventory.SlotFlags itemSlotFlags, Inventory.SlotFlags slotFlags)
    {
        return (base.CheckSlotFlags(itemSlotFlags, slotFlags) && (((slotFlags & Inventory.SlotFlags.Equip) != Inventory.SlotFlags.Equip) || ((itemSlotFlags & slotFlags) == slotFlags)));
    }

    protected override void ConfigureSlots(int totalCount, ref Inventory.Slot.KindDictionary<Inventory.Slot.Range> ranges, ref Inventory.SlotFlags[] flags)
    {
        if (totalCount != 40)
        {
            Debug.LogError("Invalid size for player inventory " + totalCount, this);
        }
        ranges = LateLoaded.SlotRanges;
        flags = LateLoaded.EveryPlayerInventory;
        if (base.networkView.isMine)
        {
            this._boundBPs = new List<BlueprintDataBlock>();
        }
    }

    protected override void DoDeactivateItem()
    {
        if (base._activeItem != null)
        {
            IHeldItem item = base._activeItem as IHeldItem;
            if (item != null)
            {
                item.OnDeactivate();
            }
        }
        base._activeItem = null;
        base.DoDeactivateItem();
    }

    protected override void DoSetActiveItem(InventoryItem item)
    {
        InventoryItem item2 = base._activeItem;
        base._activeItem = item;
        if (item2 != null)
        {
            IHeldItem iface = item2.iface as IHeldItem;
            if (iface != null)
            {
                iface.OnDeactivate();
            }
        }
        if (base._activeItem != null)
        {
            IHeldItem item4 = base._activeItem as IHeldItem;
            if (item4 != null)
            {
                item4.OnActivate();
            }
        }
    }

    public bool GetArmorItem<IArmorItem>(ArmorModelSlot slot, out IArmorItem item) where IArmorItem: class, IInventoryItem
    {
        int num;
        IInventoryItem item2;
        switch (slot)
        {
            case ArmorModelSlot.Feet:
                num = 0x27;
                break;

            case ArmorModelSlot.Legs:
                num = 0x26;
                break;

            case ArmorModelSlot.Torso:
                num = 0x25;
                break;

            case ArmorModelSlot.Head:
                num = 0x24;
                break;

            default:
                item = null;
                return false;
        }
        if (base.GetItem(num, out item2))
        {
            IArmorItem local;
            item = local = item2 as IArmorItem;
            return !object.ReferenceEquals(local, null);
        }
        item = null;
        return false;
    }

    public List<BlueprintDataBlock> GetBoundBPs()
    {
        return this._boundBPs;
    }

    public static bool IsBeltSlot(int slot)
    {
        return ((slot >= 30) && (slot < 0x24));
    }

    public static bool IsEquipmentSlot(int slot)
    {
        return ((slot >= 0x24) && (slot < 40));
    }

    protected override void ItemAdded(int slot, IInventoryItem item)
    {
        if (IsEquipmentSlot(slot))
        {
            IEquipmentItem item2 = item as IEquipmentItem;
            if (item2 != null)
            {
                item2.OnEquipped();
                this.UpdateEquipment();
            }
        }
    }

    protected override void ItemRemoved(int slot, IInventoryItem item)
    {
        if (IsEquipmentSlot(slot))
        {
            IEquipmentItem item2 = item as IEquipmentItem;
            if (item2 != null)
            {
                item2.OnUnEquipped();
                this.UpdateEquipment();
            }
        }
    }

    public bool KnowsBP(BlueprintDataBlock bp)
    {
        return ((bp != null) && this._boundBPs.Contains(bp));
    }

    public void MakeBPsDirty()
    {
        this.bpDirty = true;
    }

    [RPC, NGCRPCSkip]
    public void ReceiveBoundBPs(byte[] data, NetworkMessageInfo info)
    {
        if (this._boundBPs == null)
        {
        }
        this._boundBPs = new List<BlueprintDataBlock>();
        this._boundBPs.Clear();
        BitStream stream = new BitStream(data, false);
        int num = stream.ReadInt32();
        for (int i = 0; i < num; i++)
        {
            ItemDataBlock byUniqueID = DatablockDictionary.GetByUniqueID(stream.ReadInt32());
            if (byUniqueID != null)
            {
                BlueprintDataBlock item = byUniqueID as BlueprintDataBlock;
                this._boundBPs.Add(item);
            }
        }
        this.Refresh();
    }

    public override void Refresh()
    {
        InventoryHolder inventoryHolder = base.inventoryHolder;
        if (inventoryHolder != null)
        {
            inventoryHolder.InventoryModified();
        }
    }

    protected void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        if (info.networkView.isMine)
        {
            base.InitializeThisFixedSizeInventory();
        }
    }

    private void UpdateEquipment()
    {
        EquipmentWearer equipmentWearer = this.equipmentWearer;
        if (equipmentWearer != null)
        {
            equipmentWearer.EquipmentUpdate();
        }
    }

    private EquipmentWearer equipmentWearer
    {
        get
        {
            return ((this._equipmentWearer == null) ? (this._equipmentWearer = base.GetLocal<EquipmentWearer>()) : this._equipmentWearer);
        }
    }

    public int fixedSlotCount
    {
        get
        {
            return 40;
        }
    }

    private static class LateLoaded
    {
        public static readonly Inventory.SlotFlags[] EveryPlayerInventory = new Inventory.SlotFlags[40];
        public static Inventory.Slot.KindDictionary<Inventory.Slot.Range> SlotRanges;

        static LateLoaded()
        {
            for (int i = 0; i < 40; i++)
            {
                Inventory.SlotFlags flags = 0;
                if (PlayerInventory.IsBeltSlot(i))
                {
                    flags |= Inventory.SlotFlags.Belt;
                }
                if (i == 30)
                {
                    flags |= Inventory.SlotFlags.Safe;
                }
                if (PlayerInventory.IsEquipmentSlot(i))
                {
                    flags |= Inventory.SlotFlags.Equip;
                    switch (i)
                    {
                        case 0x24:
                            flags |= Inventory.SlotFlags.Head;
                            break;

                        case 0x25:
                            flags |= Inventory.SlotFlags.Chest;
                            break;

                        case 0x26:
                            flags |= Inventory.SlotFlags.Legs;
                            break;

                        case 0x27:
                            flags |= Inventory.SlotFlags.Feet;
                            break;
                    }
                }
                EveryPlayerInventory[i] = flags;
            }
            SlotRanges[Inventory.Slot.Kind.Default] = new Inventory.Slot.Range(0, 30);
            SlotRanges[Inventory.Slot.Kind.Belt] = new Inventory.Slot.Range(30, 6);
            SlotRanges[Inventory.Slot.Kind.Armor] = new Inventory.Slot.Range(0x24, 4);
        }
    }
}

