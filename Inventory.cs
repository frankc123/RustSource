using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using uLink;
using UnityEngine;

[NGCAutoAddScript]
public class Inventory : IDLocal
{
    [NonSerialized]
    public InventoryItem _activeItem;
    [NonSerialized]
    private Collection<InventoryItem> _collection_;
    [NonSerialized]
    private bool _collection_made_;
    [NonSerialized]
    private CacheRef<EquipmentWearer> _equipmentWearer;
    [NonSerialized]
    private CacheRef<InventoryHolder> _inventoryHolder;
    [NonSerialized]
    private bool _locked;
    [NonSerialized]
    private SlotFlags[] _slotFlags;
    private const string Client_ItemEvent = "CLEV";
    private const string ConfigureArmor_RPC = "CFAR";
    private const string DeactivateItem_RPC = "ITDE";
    private const string DoItemAction_RPC = "IACT";
    private const string GetNetUpdate_RPC = "GNUP";
    private const RPCMode ItemAction_RPCMode = RPCMode.Server;
    private const string ItemMove_RPC = "ITMV";
    private const string ItemMoveSelf_RPC = "ISMV";
    [NonSerialized]
    private ArmorModelMemberMap<ArmorDataBlock> lastNetworkedArmorDatablocks;
    private const string MergeItems_RPC = "ITMG";
    private const string MergeItemsSelf_RPC = "ITSM";
    private const string Server_Request_Inventory_Update_Cell = "SVUC";
    private const string Server_Request_Inventory_Update_Full = "SVUF";
    private const string SetActiveItem_RPC = "IAST";
    private const SlotOperations SlotOperations_Mask = (SlotOperations.Combine | SlotOperations.Move | SlotOperations.Stack);
    private const SlotOperations SlotOperations_Operations = (SlotOperations.Combine | SlotOperations.Move | SlotOperations.Stack);
    private const SlotOperations SlotOperations_Options = ((SlotOperations) 0);
    [NonSerialized]
    private Slot.KindDictionary<Slot.Range> slotRanges;
    private const string SplitStack_RPCName = "ITSP";

    public AddExistingItemResult AddExistingItem(IInventoryItem iitem, bool forbidStacking)
    {
        return this.AddExistingItem(iitem, forbidStacking, false);
    }

    private AddExistingItemResult AddExistingItem(IInventoryItem iitem, bool forbidStacking, bool mustBeUnassigned)
    {
        InventoryItem objA = iitem as InventoryItem;
        if (object.ReferenceEquals(objA, null) || (mustBeUnassigned && (objA.inventory != null)))
        {
            return AddExistingItemResult.BadItemArgument;
        }
        ItemDataBlock datablock = objA.datablock;
        Addition addition = new Addition {
            ItemDataBlock = datablock,
            UsesQuantity = objA.uses,
            SlotPreference = Slot.Preference.Define(Slot.Kind.Default, !forbidStacking && datablock.IsSplittable(), Slot.Kind.Belt)
        };
        Payload.Opt flags = Payload.Opt.IgnoreSlotOffset | Payload.Opt.ReuseItem;
        if (forbidStacking)
        {
            flags = (Payload.Opt) ((byte) (flags | Payload.Opt.DoNotStack));
        }
        Payload.Result result = this.AssignItem(ref addition, flags, objA);
        if (((byte) (result.flags & Payload.Result.Flags.Complete)) == 0x80)
        {
            if (((byte) (result.flags & Payload.Result.Flags.AssignedInstance)) == 0x40)
            {
                return AddExistingItemResult.Moved;
            }
            if (((byte) (result.flags & (Payload.Result.Flags.OptionsResultedInNoOp | Payload.Result.Flags.Stacked))) == 0x20)
            {
                objA.SetUses(0);
                return AddExistingItemResult.CompletlyStacked;
            }
            Debug.LogWarning("unhandled", this);
            return AddExistingItemResult.Failed;
        }
        if (((byte) (result.flags & (Payload.Result.Flags.OptionsResultedInNoOp | Payload.Result.Flags.Stacked))) == 0x20)
        {
            objA.SetUses(result.usesRemaining);
            return AddExistingItemResult.PartiallyStacked;
        }
        return AddExistingItemResult.Failed;
    }

    public IInventoryItem AddItem(ref Addition itemAdd)
    {
        return this.AddItem(ref itemAdd, 0, null);
    }

    private IInventoryItem AddItem(ref Addition addition, Payload.Opt flags, InventoryItem reuse)
    {
        return ResultToItem(ref this.AssignItem(ref addition, flags, reuse), flags);
    }

    public IInventoryItem AddItem(ItemDataBlock datablock, Slot.Preference slot, Uses.Quantity uses)
    {
        Datablock.Ident ident = (Datablock.Ident) datablock;
        return this.AddItem(ref ident, slot, uses);
    }

    public IInventoryItem AddItem(ref Datablock.Ident ident, Slot.Preference slot, Uses.Quantity uses)
    {
        Addition itemAdd = new Addition {
            ItemDataBlock = (ItemDataBlock) ident.datablock,
            SlotPreference = slot,
            UsesQuantity = uses
        };
        return this.AddItem(ref itemAdd);
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount)
    {
        return this.AddItemAmount(datablock, amount, AmountMode.Default, (Uses.Quantity?) null, (Slot.Preference?) null);
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, AmountMode.Default, (Uses.Quantity?) null, (Slot.Preference?) null);
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount, AmountMode mode)
    {
        return this.AddItemAmount(datablock, amount, mode, (Uses.Quantity?) null, (Slot.Preference?) null);
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount, AmountMode mode)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, mode, (Uses.Quantity?) null, (Slot.Preference?) null);
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount, Slot.Preference slotPref)
    {
        return this.AddItemAmount(datablock, amount, AmountMode.Default, null, new Slot.Preference?(slotPref));
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount, Slot.Preference slotPref)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, AmountMode.Default, null, new Slot.Preference?(slotPref));
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount, Uses.Quantity perNonSplittableItemUseQuantity)
    {
        return this.AddItemAmount(datablock, amount, AmountMode.Default, new Uses.Quantity?(perNonSplittableItemUseQuantity), null);
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount, Uses.Quantity perNonSplittableItemUseQuantity)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, AmountMode.Default, new Uses.Quantity?(perNonSplittableItemUseQuantity), null);
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount, AmountMode mode, Slot.Preference slotPref)
    {
        return this.AddItemAmount(datablock, amount, mode, null, new Slot.Preference?(slotPref));
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount, AmountMode mode, Slot.Preference slotPref)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, mode, null, new Slot.Preference?(slotPref));
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount, AmountMode mode, Uses.Quantity perNonSplittableItemUseQuantity)
    {
        return this.AddItemAmount(datablock, amount, mode, new Uses.Quantity?(perNonSplittableItemUseQuantity), null);
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount, AmountMode mode, Uses.Quantity perNonSplittableItemUseQuantity)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, mode, new Uses.Quantity?(perNonSplittableItemUseQuantity), null);
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount, Uses.Quantity perNonSplittableItemUseQuantity, Slot.Preference slotPref)
    {
        return this.AddItemAmount(datablock, amount, AmountMode.Default, new Uses.Quantity?(perNonSplittableItemUseQuantity), new Slot.Preference?(slotPref));
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount, Uses.Quantity perNonSplittableItemUseQuantity, Slot.Preference slotPref)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, AmountMode.Default, new Uses.Quantity?(perNonSplittableItemUseQuantity), new Slot.Preference?(slotPref));
    }

    public int AddItemAmount(ItemDataBlock datablock, int amount, AmountMode mode, Uses.Quantity perNonSplittableItemUseQuantity, Slot.Preference slotPref)
    {
        return this.AddItemAmount(datablock, amount, mode, new Uses.Quantity?(perNonSplittableItemUseQuantity), new Slot.Preference?(slotPref));
    }

    public int AddItemAmount(ref Datablock.Ident ident, int amount, AmountMode mode, Uses.Quantity perNonSplittableItemUseQuantity, Slot.Preference slotPref)
    {
        return this.AddItemAmount((ItemDataBlock) ident.datablock, amount, mode, new Uses.Quantity?(perNonSplittableItemUseQuantity), new Slot.Preference?(slotPref));
    }

    private int AddItemAmount(ItemDataBlock datablock, int amount, AmountMode mode, Uses.Quantity? perNonSplittableItemQuantity, Slot.Preference? slotPref)
    {
        AddMultipleItemFlags mustBeNonSplittable;
        Uses.Quantity quantity;
        if (datablock == null)
        {
            return amount;
        }
        if (!datablock.IsSplittable())
        {
            if (mode == AmountMode.OnlyStack)
            {
                return amount;
            }
            mustBeNonSplittable = AddMultipleItemFlags.MustBeNonSplittable;
            quantity = !perNonSplittableItemQuantity.HasValue ? Uses.Quantity.Random : perNonSplittableItemQuantity.Value;
        }
        else
        {
            mustBeNonSplittable = AddMultipleItemFlags.MustBeSplittable;
            switch (mode)
            {
                case AmountMode.OnlyStack:
                    mustBeNonSplittable |= AddMultipleItemFlags.DoNotCreateNewSplittableStacks;
                    break;

                case AmountMode.OnlyCreateNew:
                    mustBeNonSplittable |= AddMultipleItemFlags.DoNotStackSplittables;
                    break;

                case AmountMode.IgnoreSplittables:
                    return amount;
            }
            quantity = new Uses.Quantity();
        }
        return this.AddMultipleItems(datablock, amount, quantity, mustBeNonSplittable, slotPref);
    }

    public void AddItems(Addition[] itemAdds)
    {
        for (int i = 0; i < itemAdds.Length; i++)
        {
            this.AddItem(ref itemAdds[i]);
        }
    }

    public IInventoryItem AddItemSomehow(ItemDataBlock item, Slot.Kind? slotKindPref, int slotOffset, int usesCount)
    {
        return (((item == null) || ((usesCount <= 0) && item.IsSplittable())) ? null : this.AddItemSomehowWork(item, slotKindPref, slotOffset, usesCount));
    }

    private IInventoryItem AddItemSomehowWork(ItemDataBlock item, Slot.Kind? slotKindPref, int slotOffset, int usesCount)
    {
        Slot.Kind kind;
        int num;
        bool flag;
        bool flag2;
        Addition addition;
        if (slotKindPref.HasValue)
        {
            kind = slotKindPref.Value;
            flag = this.GetSlotForKind(kind, slotOffset, out num);
            flag2 = flag || this.HasSlotsOfKind(kind);
        }
        else
        {
            num = slotOffset;
            flag2 = flag = this.GetSlotKind(num, out kind, out slotOffset);
        }
        addition.Ident = (Datablock.Ident) item;
        addition.UsesQuantity = usesCount;
        if (flag2)
        {
            if (flag)
            {
                addition.SlotPreference = Slot.Preference.Define(kind, slotOffset);
                Payload.Result result = this.AssignItem(ref addition, Payload.Opt.RestrictToOffset, null);
                if (((byte) (result.flags & Payload.Result.Flags.Complete)) == 0x80)
                {
                    return ResultToItem(ref result, Payload.Opt.RestrictToOffset);
                }
                if (((byte) (result.flags & (Payload.Result.Flags.OptionsResultedInNoOp | Payload.Result.Flags.Stacked))) == 0x20)
                {
                    addition.UsesQuantity = usesCount = result.usesRemaining;
                }
            }
            addition.SlotPreference = kind;
            Payload.Result result2 = this.AssignItem(ref addition, 0, null);
            if (((byte) (result2.flags & Payload.Result.Flags.Complete)) == 0x80)
            {
                return ResultToItem(ref result2, 0);
            }
            if (((byte) (result2.flags & (Payload.Result.Flags.OptionsResultedInNoOp | Payload.Result.Flags.Stacked))) == 0x20)
            {
                addition.UsesQuantity = usesCount = result2.usesRemaining;
            }
        }
        else if ((num >= 0) && (num < this.slotCount))
        {
            addition.SlotPreference = Slot.Preference.Define(num);
            Payload.Result result3 = this.AssignItem(ref addition, Payload.Opt.RestrictToOffset, null);
            if (((byte) (result3.flags & Payload.Result.Flags.Complete)) == 0x80)
            {
                return ResultToItem(ref result3, Payload.Opt.RestrictToOffset);
            }
            if (((byte) (result3.flags & (Payload.Result.Flags.OptionsResultedInNoOp | Payload.Result.Flags.Stacked))) == 0x20)
            {
                addition.UsesQuantity = usesCount = result3.usesRemaining;
            }
        }
        Slot.KindFlags slotsOfKinds = Slot.KindFlags.Armor | Slot.KindFlags.Belt | Slot.KindFlags.Default;
        if (flag2)
        {
            slotsOfKinds = (Slot.KindFlags) ((byte) (slotsOfKinds & ~((byte) (((int) 1) << kind))));
        }
        addition.SlotPreference = Slot.Preference.Define(slotsOfKinds);
        return this.AddItem(ref addition);
    }

    private int AddMultipleItems(ItemDataBlock itemDB, int usesOrItemCountWhenNotSplittable, Uses.Quantity nonSplittableUses, AddMultipleItemFlags amif, Slot.Preference? slotPreference)
    {
        Addition addition = new Addition {
            ItemDataBlock = itemDB
        };
        bool flag = itemDB.IsSplittable();
        if (((amif & (AddMultipleItemFlags.MustBeSplittable | AddMultipleItemFlags.MustBeNonSplittable)) | (!flag ? AddMultipleItemFlags.MustBeNonSplittable : AddMultipleItemFlags.MustBeSplittable)) != (AddMultipleItemFlags.MustBeSplittable | AddMultipleItemFlags.MustBeNonSplittable))
        {
            if (flag)
            {
                bool flag2;
                if (usesOrItemCountWhenNotSplittable == 0)
                {
                    return 0;
                }
                if ((amif & (AddMultipleItemFlags.DoNotStackSplittables | AddMultipleItemFlags.DoNotCreateNewSplittableStacks)) == (AddMultipleItemFlags.DoNotStackSplittables | AddMultipleItemFlags.DoNotCreateNewSplittableStacks))
                {
                    return usesOrItemCountWhenNotSplittable;
                }
                int num = usesOrItemCountWhenNotSplittable / itemDB._maxUses;
                Payload.Opt ignoreSlotOffset = Payload.Opt.IgnoreSlotOffset;
                if ((amif & AddMultipleItemFlags.DoNotStackSplittables) == AddMultipleItemFlags.DoNotStackSplittables)
                {
                    flag2 = true;
                    ignoreSlotOffset = (Payload.Opt) ((byte) (ignoreSlotOffset | Payload.Opt.DoNotStack));
                    if (slotPreference.HasValue)
                    {
                        addition.SlotPreference = slotPreference.Value.CloneStackChange(false);
                    }
                    else
                    {
                        addition.SlotPreference = DefaultAddMultipleItemsSlotPreference(false);
                    }
                }
                else
                {
                    flag2 = false;
                    if (slotPreference.HasValue)
                    {
                        addition.SlotPreference = slotPreference.Value;
                    }
                    else
                    {
                        addition.SlotPreference = DefaultAddMultipleItemsSlotPreference(true);
                    }
                }
                if ((amif & AddMultipleItemFlags.DoNotCreateNewSplittableStacks) == AddMultipleItemFlags.DoNotCreateNewSplittableStacks)
                {
                    ignoreSlotOffset = (Payload.Opt) ((byte) (ignoreSlotOffset | Payload.Opt.DoNotAssign));
                }
                int num2 = 0;
                if (num > 0)
                {
                    addition.UsesQuantity = itemDB._maxUses;
                    do
                    {
                        Payload.Result result = this.AssignItem(ref addition, ignoreSlotOffset, null);
                        if (((byte) (result.flags & Payload.Result.Flags.Complete)) == 0x80)
                        {
                            num2 += itemDB._maxUses;
                            if (!flag2 && (((byte) (result.flags & Payload.Result.Flags.AssignedInstance)) == 0x40))
                            {
                                ignoreSlotOffset = (Payload.Opt) ((byte) (ignoreSlotOffset | Payload.Opt.DoNotStack));
                                flag2 = true;
                            }
                        }
                        else
                        {
                            if (((byte) (result.flags & (Payload.Result.Flags.OptionsResultedInNoOp | Payload.Result.Flags.Stacked))) == 0x20)
                            {
                                num2 += itemDB._maxUses - result.usesRemaining;
                            }
                            return (usesOrItemCountWhenNotSplittable - num2);
                        }
                    }
                    while (--num > 0);
                }
                if (num2 == usesOrItemCountWhenNotSplittable)
                {
                    return 0;
                }
                int num3 = usesOrItemCountWhenNotSplittable - num2;
                addition.UsesQuantity = num3;
                Payload.Result result2 = this.AssignItem(ref addition, ignoreSlotOffset, null);
                if (((byte) (result2.flags & (Payload.Result.Flags.Complete | Payload.Result.Flags.Stacked))) != 0)
                {
                    num2 += num3 - result2.usesRemaining;
                }
                return (usesOrItemCountWhenNotSplittable - num2);
            }
            addition.UsesQuantity = nonSplittableUses;
            addition.SlotPreference = !slotPreference.HasValue ? Slot.Preference.Define(Slot.Kind.Default, false, Slot.Kind.Belt) : slotPreference.Value.CloneStackChange(false);
            while ((usesOrItemCountWhenNotSplittable > 0) && (((byte) (this.AssignItem(ref addition, Payload.Opt.DoNotStack | Payload.Opt.IgnoreSlotOffset, null).flags & Payload.Result.Flags.Complete)) == 0x80))
            {
                usesOrItemCountWhenNotSplittable--;
            }
        }
        return usesOrItemCountWhenNotSplittable;
    }

    private Payload.Result AssignItem(ref Addition addition, Payload.Opt flags, InventoryItem reuse)
    {
        return Payload.AddItem(this, ref addition, flags, reuse);
    }

    protected void BindArmorModelsFromArmorDatablockMap(ArmorModelMemberMap<ArmorDataBlock> armorDatablockMap)
    {
        this.lastNetworkedArmorDatablocks = armorDatablockMap;
        ArmorModelRenderer local = base.GetLocal<ArmorModelRenderer>();
        if (local != null)
        {
            ArmorModelMemberMap map = new ArmorModelMemberMap();
            for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
            {
                ArmorDataBlock block = armorDatablockMap[slot];
                map[slot] = (block == null) ? null : block.GetArmorModel(slot);
            }
            local.BindArmorModels(map);
        }
    }

    public int CanConsume(ItemDataBlock db, int useCount)
    {
        Collection<InventoryItem> collection = this.collection;
        if ((useCount <= 0) || collection.HasNoOccupant)
        {
            return 0;
        }
        int num = 0;
        int uniqueID = db.uniqueID;
        using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = this.collection.OccupiedEnumerator)
        {
            while (enumerator.MoveNext())
            {
                InventoryItem current = enumerator.Current;
                if (current.datablockUniqueID == uniqueID)
                {
                    useCount -= current.uses;
                    num++;
                    if (useCount <= 0)
                    {
                        return num;
                    }
                }
            }
        }
        return -useCount;
    }

    public int CanConsume(ItemDataBlock db, int useCount, List<int> storeToList)
    {
        Collection<InventoryItem> collection = this.collection;
        if (((useCount <= 0) || (db == null)) || collection.HasNoOccupant)
        {
            return 0;
        }
        if (storeToList == null)
        {
            return this.CanConsume(db, useCount);
        }
        int count = storeToList.Count;
        int num2 = 0;
        int uniqueID = db.uniqueID;
        using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = collection.OccupiedEnumerator)
        {
            while (enumerator.MoveNext())
            {
                InventoryItem current = enumerator.Current;
                if (current.datablockUniqueID == uniqueID)
                {
                    useCount -= current.uses;
                    storeToList.Add(enumerator.Slot);
                    num2++;
                    if (useCount <= 0)
                    {
                        return num2;
                    }
                }
            }
        }
        if (num2 > 0)
        {
            storeToList.RemoveRange(count, num2);
        }
        return -useCount;
    }

    public bool CanItemFit(IInventoryItem iitem)
    {
        InventoryItem item = iitem as InventoryItem;
        ItemDataBlock datablock = item.datablock;
        if (!datablock.IsSplittable())
        {
            return this.anyVacantSlots;
        }
        int uses = item.uses;
        using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = this.collection.OccupiedEnumerator)
        {
            while (enumerator.MoveNext())
            {
                InventoryItem current = enumerator.Current;
                if ((current.datablockUniqueID == item.datablockUniqueID) && (current != iitem))
                {
                    int num2 = datablock._maxUses - current.uses;
                    if (num2 >= uses)
                    {
                        return true;
                    }
                    uses -= num2;
                }
            }
        }
        return false;
    }

    [RPC]
    protected void CFAR(BitStream stream)
    {
        ArmorModelMemberMap<ArmorDataBlock> armorDatablockMap = new ArmorModelMemberMap<ArmorDataBlock>();
        for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
        {
            armorDatablockMap[slot] = DatablockDictionary.GetByUniqueID(stream.ReadInt32()) as ArmorDataBlock;
        }
        this.BindArmorModelsFromArmorDatablockMap(armorDatablockMap);
    }

    protected virtual bool CheckSlotFlags(SlotFlags itemSlotFlags, SlotFlags slotFlags)
    {
        return true;
    }

    private bool CheckSlotFlagsAgainstSlot(SlotFlags itemSlotFlags, int slot)
    {
        return this.CheckSlotFlags(itemSlotFlags, this.GetSlotFlags(slot));
    }

    public void Clear()
    {
        using (Collection<InventoryItem>.OccupiedCollection.ReverseEnumerator enumerator = this.collection.OccupiedReverseEnumerator)
        {
            while (enumerator.MoveNext())
            {
                this.DeleteItem(enumerator.Slot);
            }
        }
    }

    [RPC]
    protected void CLEV(byte itemEvent, int uniqueID)
    {
        ItemDataBlock byUniqueID = DatablockDictionary.GetByUniqueID(uniqueID);
        if (byUniqueID != null)
        {
            byUniqueID.OnItemEvent((InventoryItem.ItemEvent) itemEvent);
        }
    }

    protected virtual void ConfigureSlots(int totalCount, ref Slot.KindDictionary<Slot.Range> ranges, ref SlotFlags[] flags)
    {
    }

    public void DeactivateItem()
    {
        this.DoDeactivateItem();
    }

    private static Slot.Preference DefaultAddMultipleItemsSlotPreference(bool stack)
    {
        return Slot.Preference.Define(Slot.Kind.Default, stack, Slot.KindFlags.Belt);
    }

    private void DeleteItem(int slot)
    {
        this.RemoveItem(slot);
    }

    protected virtual void DoDeactivateItem()
    {
        this._activeItem = null;
    }

    protected virtual void DoSetActiveItem(InventoryItem item)
    {
        this._activeItem = item;
    }

    public IItemT FindItem<IItemT>() where IItemT: class, IInventoryItem
    {
        using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = this.collection.OccupiedEnumerator)
        {
            while (enumerator.MoveNext())
            {
                IItemT iface = enumerator.Current.iface as IItemT;
                if (!object.ReferenceEquals(iface, null))
                {
                    return iface;
                }
            }
        }
        return null;
    }

    public IInventoryItem FindItem(ItemDataBlock itemDB)
    {
        int totalNum = 0;
        return this.FindItem(itemDB, out totalNum);
    }

    public IInventoryItem FindItem(string itemDBName)
    {
        return this.FindItem(DatablockDictionary.GetByName(itemDBName));
    }

    public IInventoryItem FindItem(ItemDataBlock itemDB, out int totalNum)
    {
        bool flag = false;
        InventoryItem item = null;
        int num = 0;
        int num2 = -1;
        int uniqueID = itemDB.uniqueID;
        using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = this.collection.OccupiedEnumerator)
        {
            while (enumerator.MoveNext())
            {
                InventoryItem current = enumerator.Current;
                if (current.datablockUniqueID == uniqueID)
                {
                    int uses = current.uses;
                    if (!flag || (uses > num2))
                    {
                        item = current;
                        num2 = uses;
                        flag = true;
                    }
                    num += uses;
                }
            }
        }
        totalNum = num;
        return (!flag ? null : item.iface);
    }

    [DebuggerHidden]
    public IEnumerable<IItemT> FindItems<IItemT>() where IItemT: class, IInventoryItem
    {
        return new <FindItems>c__Iterator3D<IItemT> { <>f__this = this, $PC = -2 };
    }

    public T FindItemType<T>() where T: class, IInventoryItem
    {
        using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = this.collection.OccupiedEnumerator)
        {
            while (enumerator.MoveNext())
            {
                T iface = enumerator.Current.iface as T;
                if (!object.ReferenceEquals(iface, null))
                {
                    return iface;
                }
            }
        }
        return null;
    }

    public Transfer[] GenerateOptimizedInventoryListing(Slot.KindFlags fallbackPlacement)
    {
        Transfer[] transferArray;
        Collection<InventoryItem> collection = this.collection;
        if (collection.HasNoOccupant)
        {
            return new Transfer[0];
        }
        try
        {
            Report.Begin();
            using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = collection.OccupiedEnumerator)
            {
                while (enumerator.MoveNext())
                {
                    Report.Take(enumerator.Current);
                }
            }
            transferArray = Report.Build(fallbackPlacement);
        }
        finally
        {
            Report.Recover();
        }
        return transferArray;
    }

    public Transfer[] GenerateOptimizedInventoryListing(Slot.KindFlags fallbackPlacement, bool randomize)
    {
        Transfer[] array = this.GenerateOptimizedInventoryListing(fallbackPlacement);
        if (randomize && (array.Length > 0))
        {
            Shuffle.Array<Transfer>(array);
            for (int i = 0; i < array.Length; i++)
            {
                array[i].addition.SlotPreference = array[i].addition.SlotPreference.CloneOffsetChange(i);
            }
        }
        return array;
    }

    public bool GetItem(int slot, out IInventoryItem item)
    {
        InventoryItem item2;
        if (!this._collection_made_ || !this._collection_.Get(slot, out item2))
        {
            item = null;
            return false;
        }
        item = item2.iface;
        return true;
    }

    protected bool GetItem(int slot, out InventoryItem item)
    {
        if (!this._collection_made_)
        {
            item = null;
            return false;
        }
        return this._collection_.Get(slot, out item);
    }

    public SlotFlags GetSlotFlags(int slot)
    {
        return (((this._slotFlags != null) && (this._slotFlags.Length > slot)) ? this._slotFlags[slot] : ((SlotFlags) 0));
    }

    public bool GetSlotForKind(Slot.Kind kind, int offset, out int slot)
    {
        Slot.Range range;
        if (((offset >= 0) && this.slotRanges.TryGetValue(kind, out range)) && (offset < range.Count))
        {
            slot = range.Start + offset;
            return true;
        }
        slot = -1;
        return false;
    }

    public bool GetSlotKind(int slot, out Slot.Kind kind, out int offset)
    {
        if ((slot >= 0) && (slot < this.slotCount))
        {
            for (Slot.Kind kind2 = Slot.Kind.Default; kind2 < (Slot.Kind.Armor | Slot.Kind.Belt); kind2 = (Slot.Kind) (((int) kind2) + 1))
            {
                Slot.Range range;
                if (this.slotRanges.TryGetValue(kind2, out range))
                {
                    offset = range.GetOffset(slot);
                    if (offset != -1)
                    {
                        kind = kind2;
                        return true;
                    }
                }
            }
        }
        kind = Slot.Kind.Default;
        offset = -1;
        return false;
    }

    public bool GetSlotsOfKind(Slot.Kind kind, out Slot.Range range)
    {
        return this.slotRanges.TryGetValue(kind, out range);
    }

    [RPC]
    protected void GNUP(byte[] data, NetworkMessageInfo info)
    {
        this.OnNetUpdate(new BitStream(data, false));
        this.Refresh();
    }

    public bool HasSlotsOfKind(Slot.Kind kind)
    {
        return this.slotRanges.ContainsKey(kind);
    }

    [NGCRPCSkip, RPC]
    protected void IACT(byte itemIndex, byte action, NetworkMessageInfo info)
    {
        InventoryItem item;
        if (this.collection.Get(itemIndex, out item))
        {
            item.OnMenuOption((InventoryItem.MenuItem) action);
        }
    }

    [RPC, NGCRPCSkip]
    protected void IAST(byte itemIndex, NetworkViewID itemRepID, NetworkMessageInfo info)
    {
        this.SetActiveItemManually(itemIndex, !(itemRepID != NetworkViewID.unassigned) ? null : NetworkView.Find(itemRepID).GetComponent<ItemRepresentation>());
    }

    private void Initialize(int slotCount)
    {
        if (this._collection_made_)
        {
            this.Clear();
            this._collection_ = null;
            this._collection_made_ = false;
        }
        this._slotFlags = Empty.SlotFlags;
        this._collection_ = new Collection<InventoryItem>(slotCount);
        this._collection_made_ = true;
        this.slotRanges = new Slot.KindDictionary<Slot.Range>();
        this.slotRanges[Slot.Kind.Default] = new Slot.Range(0, slotCount);
        this.ConfigureSlots(slotCount, ref this.slotRanges, ref this._slotFlags);
        this._collection_.MarkCompletelyDirty();
    }

    protected bool InitializeThisFixedSizeInventory()
    {
        FixedSizeInventory objA = this as FixedSizeInventory;
        if (object.ReferenceEquals(objA, null))
        {
            return false;
        }
        int fixedSlotCount = objA.fixedSlotCount;
        if (this._collection_made_)
        {
            if (this._collection_.Capacity == fixedSlotCount)
            {
                return false;
            }
            Debug.LogError("Some how this inventory was already inititalized to a different size. It will be reinitialized. the original off size was " + this._collection_.Capacity, this);
        }
        this.Initialize(fixedSlotCount);
        return true;
    }

    [RPC]
    protected void ISMV(byte fromSlot, byte toSlot, NetworkMessageInfo info)
    {
    }

    public bool IsSlotDirty(int slot)
    {
        return this.collection.IsDirty(slot);
    }

    public bool IsSlotFree(int slot)
    {
        return this.collection.IsVacant(slot);
    }

    public bool IsSlotOccupied(int slot)
    {
        return this.collection.IsOccupied(slot);
    }

    public bool IsSlotOffsetValid(Slot.Kind kind, int offset)
    {
        int num;
        return this.GetSlotForKind(kind, offset, out num);
    }

    public bool IsSlotVacant(int slot)
    {
        return this.collection.IsVacant(slot);
    }

    public bool IsSlotWithinRange(int slot)
    {
        return this.collection.IsWithinRange(slot);
    }

    [NGCRPCSkip, RPC]
    protected void ITDE(NetworkMessageInfo info)
    {
        this.DeactivateItem();
    }

    protected virtual void ItemAdded(int slot, IInventoryItem item)
    {
        FireBarrel local = base.GetLocal<FireBarrel>();
        if (local != null)
        {
            local.InvItemAdded();
        }
    }

    public SlotOperationResult ItemCombinePredicted(int fromSlot, int toSlot)
    {
        return this.ItemMergeRPCPred(fromSlot, toSlot, true);
    }

    public SlotOperationResult ItemCombinePredicted(Inventory toInventory, int fromSlot, int toSlot)
    {
        return this.ItemCombinePredicted(NetEntityID.Get((MonoBehaviour) toInventory), fromSlot, toSlot);
    }

    public SlotOperationResult ItemCombinePredicted(NetEntityID toInvID, int fromSlot, int toSlot)
    {
        return this.ItemMergeRPCPred(toInvID, fromSlot, toSlot, true);
    }

    public static SlotOperationResult ItemCombinePredicted(NetEntityID fromInvID, NetEntityID toInvID, int fromSlot, int toSlot)
    {
        Inventory component = fromInvID.GetComponent<Inventory>();
        if (component == null)
        {
            return SlotOperationResult.Error_MissingInventory;
        }
        return component.ItemCombinePredicted(toInvID, fromSlot, toSlot);
    }

    public SlotOperationResult ItemMergePredicted(int fromSlot, int toSlot)
    {
        return this.ItemMergeRPCPred(fromSlot, toSlot, false);
    }

    public SlotOperationResult ItemMergePredicted(Inventory toInventory, int fromSlot, int toSlot)
    {
        return this.ItemMergePredicted(NetEntityID.Get((MonoBehaviour) toInventory), fromSlot, toSlot);
    }

    public SlotOperationResult ItemMergePredicted(NetEntityID toInvID, int fromSlot, int toSlot)
    {
        return this.ItemMergeRPCPred(toInvID, fromSlot, toSlot, false);
    }

    public static SlotOperationResult ItemMergePredicted(NetEntityID fromInvID, NetEntityID toInvID, int fromSlot, int toSlot)
    {
        Inventory component = fromInvID.GetComponent<Inventory>();
        if (component == null)
        {
            return SlotOperationResult.Error_MissingInventory;
        }
        return component.ItemMergePredicted(toInvID, fromSlot, toSlot);
    }

    private void ItemMergeRPC(int fromSlot, int toSlot, bool tryCombine)
    {
        NetCull.RPC<byte, byte, bool>((MonoBehaviour) this, "ITSM", RPCMode.Server, (byte) fromSlot, (byte) toSlot, tryCombine);
    }

    private void ItemMergeRPC(NetEntityID toInvID, int fromSlot, int toSlot, bool tryCombine)
    {
        NetCull.RPC<NetEntityID, byte, byte, bool>((MonoBehaviour) this, "ITMG", RPCMode.Server, toInvID, (byte) fromSlot, (byte) toSlot, tryCombine);
    }

    private SlotOperationResult ItemMergeRPCPred(int fromSlot, int toSlot, bool tryCombine)
    {
        SlotOperationResult result;
        if (((int) (result = this.SlotOperation(fromSlot, toSlot, SlotOperationsMerge(tryCombine)))) > 0)
        {
            this.ItemMergeRPC(fromSlot, toSlot, tryCombine);
        }
        return result;
    }

    private SlotOperationResult ItemMergeRPCPred(NetEntityID toInvID, int fromSlot, int toSlot, bool tryCombine)
    {
        SlotOperationResult result;
        Inventory component = toInvID.GetComponent<Inventory>();
        if (component == this)
        {
            if (((int) (result = this.SlotOperation(fromSlot, toSlot, SlotOperationsMerge(tryCombine)))) > 0)
            {
                this.ItemMergeRPC(fromSlot, toSlot, tryCombine);
            }
            return result;
        }
        if (((int) (result = this.SlotOperation(fromSlot, component, toSlot, SlotOperationsMerge(tryCombine)))) > 0)
        {
            this.ItemMergeRPC(toInvID, fromSlot, toSlot, tryCombine);
        }
        return result;
    }

    public SlotOperationResult ItemMovePredicted(int fromSlot, int toSlot)
    {
        return this.ItemMovePredicted(fromSlot, toSlot);
    }

    public SlotOperationResult ItemMovePredicted(Inventory toInventory, int fromSlot, int toSlot)
    {
        return this.ItemMovePredicted(NetEntityID.Get((MonoBehaviour) toInventory), fromSlot, toSlot);
    }

    public SlotOperationResult ItemMovePredicted(NetEntityID toInvID, int fromSlot, int toSlot)
    {
        return this.ItemMoveRPCPred(toInvID, fromSlot, toSlot);
    }

    public static SlotOperationResult ItemMovePredicted(NetEntityID fromInvID, NetEntityID toInvID, int fromSlot, int toSlot)
    {
        Inventory component = fromInvID.GetComponent<Inventory>();
        if (component == null)
        {
            return SlotOperationResult.Error_MissingInventory;
        }
        return component.ItemMovePredicted(toInvID, fromSlot, toSlot);
    }

    private void ItemMoveRPC(int fromSlot, int toSlot)
    {
        NetCull.RPC<byte, byte>((MonoBehaviour) this, "ISMV", RPCMode.Server, (byte) fromSlot, (byte) toSlot);
    }

    private void ItemMoveRPC(NetEntityID toInvID, int fromSlot, int toSlot)
    {
        NetCull.RPC<NetEntityID, byte, byte>((MonoBehaviour) this, "ITMV", RPCMode.Server, toInvID, (byte) fromSlot, (byte) toSlot);
    }

    private SlotOperationResult ItemMoveRPCPred(int fromSlot, int toSlot)
    {
        SlotOperationResult result;
        if (((int) (result = this.SlotOperation(fromSlot, toSlot, 4))) > 0)
        {
            this.ItemMoveRPC(fromSlot, toSlot);
        }
        return result;
    }

    private SlotOperationResult ItemMoveRPCPred(NetEntityID toInvID, int fromSlot, int toSlot)
    {
        SlotOperationResult result;
        Inventory component = toInvID.GetComponent<Inventory>();
        if (component == this)
        {
            if (((int) (result = this.SlotOperation(fromSlot, toSlot, 4))) > 0)
            {
                this.ItemMoveRPC(fromSlot, toSlot);
            }
            return result;
        }
        if (((int) (result = this.SlotOperation(fromSlot, component, toSlot, 4))) > 0)
        {
            this.ItemMoveRPC(toInvID, fromSlot, toSlot);
        }
        return result;
    }

    protected virtual void ItemRemoved(int slot, IInventoryItem item)
    {
        FireBarrel local = base.GetLocal<FireBarrel>();
        if (local != null)
        {
            local.InvItemRemoved();
        }
    }

    [RPC]
    protected void ITMG(NetEntityID toInvID, byte fromSlot, byte toSlot, bool tryCombine, NetworkMessageInfo info)
    {
    }

    [RPC]
    protected void ITMV(NetEntityID toInvID, byte fromSlot, byte toSlot, NetworkMessageInfo info)
    {
    }

    [RPC]
    protected void ITSM(byte fromSlot, byte toSlot, bool tryCombine, NetworkMessageInfo info)
    {
    }

    [RPC]
    protected void ITSP(byte slotNumber, NetworkMessageInfo info)
    {
    }

    public bool MarkSlotClean(int slot)
    {
        return this.collection.MarkClean(slot);
    }

    public bool MarkSlotDirty(int slot)
    {
        return this.collection.MarkDirty(slot);
    }

    public bool MoveItemAtSlotToEmptySlot(Inventory toInv, int fromSlot, int toSlot)
    {
        InventoryItem item;
        if (toInv == null)
        {
            return false;
        }
        if ((toInv == this) && (fromSlot == toSlot))
        {
            return false;
        }
        Collection<InventoryItem> collection = this.collection;
        if (collection.HasNoOccupant)
        {
            return false;
        }
        if (!collection.Get(fromSlot, out item))
        {
            return false;
        }
        ItemDataBlock datablock = item.datablock;
        Addition addition = new Addition {
            ItemDataBlock = datablock,
            UsesQuantity = item.uses,
            SlotPreference = Slot.Preference.Define(toSlot, datablock.IsSplittable())
        };
        return !object.ReferenceEquals(toInv.AddItem(ref addition, Payload.Opt.DoNotStack | Payload.Opt.RestrictToOffset | Payload.Opt.ReuseItem, item), null);
    }

    public bool NetworkItemAction(int slot, InventoryItem.MenuItem option)
    {
        NetworkView networkView = base.networkView;
        if (networkView != null)
        {
            object[] args = new object[] { (byte) slot, (byte) option };
            networkView.RPC("IACT", RPCMode.Server, args);
            return true;
        }
        return false;
    }

    [Obsolete("This isnt right")]
    public void NULL_SLOT_FIX_ME(int slot)
    {
        this.DeleteItem(slot);
    }

    private void OnNetSlotUpdate(Collection<InventoryItem> _collection, int slot, bool occupied, BitStream invdata)
    {
        if (occupied)
        {
            InventoryItem item;
            int num = invdata.ReadInt32();
            bool flag = _collection.Get(slot, out item);
            if (flag && (item.datablockUniqueID != num))
            {
                this.DeleteItem(slot);
                flag = false;
                item = null;
            }
            if (!flag)
            {
                Addition addition = new Addition {
                    UniqueID = num,
                    UsesQuantity = Uses.Quantity.Maximum,
                    SlotPreference = Slot.Preference.Define(slot, false)
                };
                item = this.AddItem(ref addition, Payload.Opt.DoNotStack | Payload.Opt.RestrictToOffset, null) as InventoryItem;
            }
            item.Deserialize(invdata);
            if (flag)
            {
                _collection.MarkDirty(slot);
            }
        }
        else
        {
            this.DeleteItem(slot);
        }
    }

    protected void OnNetUpdate(BitStream invdata)
    {
        Collection<InventoryItem> collection;
        int num3;
        int slotCount = invdata.ReadByte();
        if (this._collection_made_)
        {
            collection = this._collection_;
        }
        else
        {
            this.Initialize(slotCount);
            collection = this._collection_;
        }
        int capacity = collection.Capacity;
        if (slotCount != capacity)
        {
            this.Initialize(slotCount);
        }
        if (invdata.ReadBoolean())
        {
            num3 = slotCount;
            for (int i = 0; i < num3; i++)
            {
                bool occupied = invdata.ReadBoolean();
                this.OnNetSlotUpdate(collection, i, occupied, invdata);
            }
        }
        else
        {
            num3 = invdata.ReadByte();
            int num5 = 0;
            try
            {
                for (int j = 0; j < num3; j++)
                {
                    num5++;
                    bool flag3 = invdata.ReadBoolean();
                    int slot = invdata.ReadByte();
                    this.OnNetSlotUpdate(collection, slot, flag3, invdata);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Debug.Log(string.Format("numItemsInUpdate = {0}, iterated pos = {1}", num3, num5), this);
            }
        }
    }

    public virtual void Refresh()
    {
    }

    public bool RemoveItem(IInventoryItem item)
    {
        return this.RemoveItem(item as InventoryItem);
    }

    public bool RemoveItem(InventoryItem item)
    {
        if (object.ReferenceEquals(item, null))
        {
            return false;
        }
        if (item.inventory != this)
        {
            return false;
        }
        return this.RemoveItem(item.slot, item, true);
    }

    public bool RemoveItem(int slot)
    {
        return this.RemoveItem(slot, null, false);
    }

    private bool RemoveItem(int slot, InventoryItem match, bool mustMatch)
    {
        InventoryItem item;
        Collection<InventoryItem> collection = this.collection;
        if ((!mustMatch || (collection.Get(slot, out item) && object.ReferenceEquals(item, match))) && collection.Evict(slot, out item))
        {
            if (item == this._activeItem)
            {
                this.DeactivateItem();
            }
            this.ItemRemoved(slot, item.iface);
            this.MarkSlotDirty(slot);
            return true;
        }
        return false;
    }

    protected void RequestCellUpdate(int cell)
    {
        NetCull.RPC<byte>((MonoBehaviour) this, "SVUC", RPCMode.Server, RPCInteger(cell));
    }

    public void RequestFullUpdate()
    {
        NetCull.RPC((MonoBehaviour) this, "SVUF", RPCMode.Server);
    }

    public void ResetToReport(Transfer[] items)
    {
        if (this._collection_made_)
        {
            this.Clear();
        }
        this.Initialize(items.Length);
        for (int i = 0; i < items.Length; i++)
        {
            this.AssignItem(ref items[i].addition, Payload.Opt.DoNotStack | Payload.Opt.RestrictToOffset | Payload.Opt.ReuseItem, items[i].item);
        }
    }

    private static IInventoryItem ResultToItem(ref Payload.Result result, Payload.Opt flags)
    {
        if (((byte) (result.flags & Payload.Result.Flags.AssignedInstance)) == 0x40)
        {
            return result.item.iface;
        }
        if ((((byte) (flags & Payload.Opt.AllowStackedItemsToBeReturned)) == 0x20) && (((byte) (result.flags & (Payload.Result.Flags.OptionsResultedInNoOp | Payload.Result.Flags.Stacked))) == 0x20))
        {
            return result.item.iface;
        }
        return null;
    }

    public static byte RPCInteger(byte i)
    {
        return i;
    }

    public static byte RPCInteger(int i)
    {
        return (byte) i;
    }

    public static byte RPCInteger(BitStream stream)
    {
        return stream.Read<byte>(new object[0]);
    }

    public void SetActiveItemManually(int itemIndex, ItemRepresentation itemRep)
    {
        IInventoryItem item;
        this.GetItem(itemIndex, out item);
        ((IHeldItem) item).itemRepresentation = itemRep;
        this.DoSetActiveItem((InventoryItem) item);
    }

    private SlotOperationResult SlotOperation(int fromSlot, int toSlot, SlotOperationsInfo info)
    {
        return this.SlotOperation(fromSlot, this, toSlot, info);
    }

    private SlotOperationResult SlotOperation(int fromSlot, Inventory toInventory, int toSlot, SlotOperationsInfo info)
    {
        InventoryItem item;
        InventoryItem item2;
        if (((byte) ((SlotOperations.Combine | SlotOperations.Move | SlotOperations.Stack) & info.SlotOperations)) == 0)
        {
            return SlotOperationResult.Error_NoOpArgs;
        }
        if ((this == null) || (toInventory == null))
        {
            return SlotOperationResult.Error_MissingInventory;
        }
        if ((this == toInventory) && (toSlot == fromSlot))
        {
            return SlotOperationResult.Error_SameSlot;
        }
        if (!this.GetItem(fromSlot, out item))
        {
            return SlotOperationResult.Error_EmptySourceSlot;
        }
        if (toInventory.GetItem(toSlot, out item2))
        {
            InventoryItem.MergeResult failed;
            this.MarkSlotDirty(fromSlot);
            toInventory.MarkSlotDirty(toSlot);
            if ((((byte) ((SlotOperations.Combine | SlotOperations.Stack) & info.SlotOperations)) == 1) && (item.datablockUniqueID == item2.datablockUniqueID))
            {
                failed = item.iface.TryStack(item2.iface);
            }
            else if (((byte) ((SlotOperations.Combine | SlotOperations.Stack) & info.SlotOperations)) != 0)
            {
                failed = item.iface.TryCombine(item2.iface);
            }
            else
            {
                failed = InventoryItem.MergeResult.Failed;
            }
            switch (failed)
            {
                case InventoryItem.MergeResult.Merged:
                    return SlotOperationResult.Success_Stacked;

                case InventoryItem.MergeResult.Combined:
                    return SlotOperationResult.Success_Combined;
            }
            if (((byte) (SlotOperations.Move & info.SlotOperations)) == 4)
            {
                return SlotOperationResult.Error_OccupiedDestination;
            }
            return SlotOperationResult.NoOp;
        }
        if (((byte) (SlotOperations.Move & info.SlotOperations)) == 0)
        {
            return SlotOperationResult.Error_EmptyDestinationSlot;
        }
        if (!this.MoveItemAtSlotToEmptySlot(toInventory, fromSlot, toSlot))
        {
            return SlotOperationResult.Error_Failed;
        }
        if (this != null)
        {
            this.MarkSlotDirty(fromSlot);
        }
        if (toInventory != null)
        {
            toInventory.MarkSlotDirty(toSlot);
        }
        return SlotOperationResult.Success_Moved;
    }

    private static SlotOperations SlotOperationsMerge(bool tryCombine)
    {
        return (tryCombine ? (SlotOperations.Combine | SlotOperations.Stack) : SlotOperations.Stack);
    }

    public bool SplitStack(int slotNumber)
    {
        InventoryItem item;
        if (this.GetItem(slotNumber, out item))
        {
            int uses = item.uses;
            if (((uses > 1) && this.anyVacantSlots) && item.datablock.IsSplittable())
            {
                int amount = uses / 2;
                int num3 = amount - this.AddItemAmount(item.datablock, amount, AmountMode.OnlyCreateNew);
                if (num3 > 0)
                {
                    uses -= num3;
                    item.SetUses(uses);
                    NetCull.RPC<byte>((MonoBehaviour) this, "ITSP", RPCMode.Server, (byte) slotNumber);
                    return true;
                }
            }
        }
        return false;
    }

    [RPC]
    protected void SVUC(byte cell, NetworkMessageInfo info)
    {
    }

    [RPC]
    protected void SVUF(NetworkMessageInfo info)
    {
    }

    public IngredientList<ItemDataBlock> ToIngredientList()
    {
        Collection<InventoryItem> collection = this.collection;
        ItemDataBlock[] array = new ItemDataBlock[collection.OccupiedCount];
        using (Collection<InventoryItem>.OccupiedCollection.Enumerator enumerator = collection.OccupiedEnumerator)
        {
            int newSize = 0;
            while (enumerator.MoveNext())
            {
                array[newSize++] = enumerator.Current.datablock;
            }
            Array.Resize<ItemDataBlock>(ref array, newSize);
            int num = newSize;
        }
        return new IngredientList<ItemDataBlock>(array);
    }

    public IInventoryItem activeItem
    {
        get
        {
            return ((this._activeItem != null) ? this._activeItem.iface : null);
        }
    }

    public bool anyOccupiedSlots
    {
        get
        {
            return this.collection.HasAnyOccupant;
        }
    }

    public bool anyVacantSlots
    {
        get
        {
            return this.collection.HasVacancy;
        }
    }

    private Collection<InventoryItem> collection
    {
        get
        {
            if (!this._collection_made_)
            {
                return Collection<InventoryItem>.Default.Empty;
            }
            return this._collection_;
        }
    }

    public float? craftingCompletePercent
    {
        get
        {
            CraftingInventory inventory = this as CraftingInventory;
            if (inventory != null)
            {
                return inventory.craftingCompletePercent;
            }
            return null;
        }
    }

    public float? craftingSecondsRemaining
    {
        get
        {
            CraftingInventory inventory = this as CraftingInventory;
            if (inventory != null)
            {
                return inventory.craftingSecondsRemaining;
            }
            return null;
        }
    }

    public float craftingSpeed
    {
        get
        {
            CraftingInventory inventory = this as CraftingInventory;
            if (inventory == null)
            {
                return 0f;
            }
            return inventory.craftingSpeedPerSec;
        }
    }

    public int dirtySlotCount
    {
        get
        {
            return this.collection.DirtyCount;
        }
    }

    public EquipmentWearer equipmentWearer
    {
        get
        {
            if (!this._equipmentWearer.cached)
            {
                this._equipmentWearer = base.GetLocal<EquipmentWearer>();
            }
            return this._equipmentWearer.value;
        }
    }

    protected InventoryItem firstInventoryItem
    {
        get
        {
            InventoryItem item;
            if (this.collection.GetByOrder(0, out item))
            {
                return item;
            }
            return null;
        }
    }

    public IInventoryItem firstItem
    {
        get
        {
            InventoryItem item;
            if (this.collection.GetByOrder(0, out item))
            {
                return item.iface;
            }
            return null;
        }
    }

    protected HumanController hackyNeedToFixHumanControllGetValue
    {
        get
        {
            Character idMain = base.idMain as Character;
            return ((idMain == null) ? null : (idMain.controller as HumanController));
        }
    }

    public bool initialized
    {
        get
        {
            return this._collection_made_;
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

    public bool isCrafting
    {
        get
        {
            CraftingInventory inventory = this as CraftingInventory;
            return ((inventory != null) && inventory.isCrafting);
        }
    }

    public bool isCraftingInventory
    {
        get
        {
            return (this is CraftingInventory);
        }
    }

    public bool locked
    {
        get
        {
            return this._locked;
        }
        set
        {
            this._locked = value;
        }
    }

    public bool noOccupiedSlots
    {
        get
        {
            return this.collection.HasNoOccupant;
        }
    }

    public bool noVacantSlots
    {
        get
        {
            return this.collection.HasNoVacancy;
        }
    }

    public OccupiedIterator occupiedIterator
    {
        get
        {
            return new OccupiedIterator(this);
        }
    }

    public OccupiedReverseIterator occupiedReverseIterator
    {
        get
        {
            return new OccupiedReverseIterator(this);
        }
    }

    public int occupiedSlotCount
    {
        get
        {
            return this.collection.OccupiedCount;
        }
    }

    public int slotCount
    {
        get
        {
            return this.collection.Capacity;
        }
    }

    public VacantIterator vacantIterator
    {
        get
        {
            return new VacantIterator(this);
        }
    }

    public int vacantSlotCount
    {
        get
        {
            return this.collection.VacantCount;
        }
    }

    [CompilerGenerated]
    private sealed class <FindItems>c__Iterator3D<IItemT> : IDisposable, IEnumerator, IEnumerable, IEnumerable<IItemT>, IEnumerator<IItemT> where IItemT: class, IInventoryItem
    {
        internal IItemT $current;
        internal int $PC;
        internal Inventory <>f__this;
        internal Inventory.Collection<InventoryItem>.OccupiedCollection.Enumerator <enumerator>__0;
        internal IItemT <item>__1;

        [DebuggerHidden]
        public void Dispose()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 1:
                    try
                    {
                    }
                    finally
                    {
                        this.<enumerator>__0.Dispose();
                    }
                    break;
            }
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            bool flag = false;
            switch (num)
            {
                case 0:
                    this.<enumerator>__0 = this.<>f__this.collection.OccupiedEnumerator;
                    num = 0xfffffffd;
                    break;

                case 1:
                    break;

                default:
                    goto Label_00CE;
            }
            try
            {
                while (this.<enumerator>__0.MoveNext())
                {
                    this.<item>__1 = this.<enumerator>__0.Current.iface as IItemT;
                    if (!object.ReferenceEquals(this.<item>__1, null))
                    {
                        this.$current = this.<item>__1;
                        this.$PC = 1;
                        flag = true;
                        return true;
                    }
                }
            }
            finally
            {
                if (!flag)
                {
                }
                this.<enumerator>__0.Dispose();
            }
            this.$PC = -1;
        Label_00CE:
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<IItemT> IEnumerable<IItemT>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Inventory.<FindItems>c__Iterator3D<IItemT> { <>f__this = this.<>f__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<IItemT>.GetEnumerator();
        }

        IItemT IEnumerator<IItemT>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    public enum AddExistingItemResult
    {
        CompletlyStacked,
        Moved,
        PartiallyStacked,
        Failed,
        BadItemArgument
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Addition
    {
        public Datablock.Ident Ident;
        public Inventory.Uses.Quantity UsesQuantity;
        public Inventory.Slot.Preference SlotPreference;
        public ItemDataBlock ItemDataBlock
        {
            get
            {
                return (ItemDataBlock) this.Ident.datablock;
            }
            set
            {
                this.Ident = (Datablock.Ident) value;
            }
        }
        public string Name
        {
            get
            {
                ItemDataBlock itemDataBlock = this.ItemDataBlock;
                return ((itemDataBlock == null) ? null : itemDataBlock.name);
            }
            set
            {
                this.Ident = value;
            }
        }
        public int UniqueID
        {
            get
            {
                ItemDataBlock itemDataBlock = this.ItemDataBlock;
                return ((itemDataBlock == null) ? 0 : itemDataBlock.uniqueID);
            }
            set
            {
                this.Ident = value;
            }
        }
    }

    [Flags]
    private enum AddMultipleItemFlags
    {
        DoNotCreateNewSplittableStacks = 4,
        DoNotStackSplittables = 8,
        MustBeNonSplittable = 1,
        MustBeSplittable = 2
    }

    public enum AmountMode
    {
        Default,
        OnlyStack,
        OnlyCreateNew,
        IgnoreSplittables
    }

    private sealed class Collection<T>
    {
        [NonSerialized]
        private T[] array;
        [NonSerialized]
        private int capacity;
        [NonSerialized]
        private int count;
        [NonSerialized]
        private int countDirty;
        [NonSerialized]
        private Inventory.Mask dirty;
        [NonSerialized]
        private bool forcedDirty;
        [NonSerialized]
        private byte[] indices;
        [NonSerialized]
        private Inventory.Mask occupied;
        [NonSerialized]
        private OccupiedCollection<T> occupiedCollection;
        [NonSerialized]
        private VacantCollection<T> vacantCollection;

        public Collection(int Capacity)
        {
            if ((Capacity < 0) || (Capacity > 0x100))
            {
                throw new ArgumentOutOfRangeException();
            }
            this.capacity = Capacity;
            this.count = 0;
            this.array = new T[Capacity];
            this.indices = new byte[Capacity];
        }

        public bool Clean(out Inventory.Mask dirtyMask, out int numDirty)
        {
            return this.Clean(out dirtyMask, out numDirty, false);
        }

        public bool Clean(out Inventory.Mask dirtyMask, out int numDirty, bool dontActuallyClean)
        {
            if (this.countDirty > 0)
            {
                dirtyMask = this.dirty;
                numDirty = this.countDirty;
                if (!dontActuallyClean)
                {
                    this.dirty = new Inventory.Mask();
                    this.countDirty = 0;
                    this.forcedDirty = false;
                }
                return true;
            }
            dirtyMask = new Inventory.Mask();
            numDirty = 0;
            if (!this.forcedDirty)
            {
                return false;
            }
            if (!dontActuallyClean)
            {
                this.forcedDirty = false;
            }
            return true;
        }

        public void Contract()
        {
            this.Contract(new Inventory.Slot.Range(0, this.capacity));
        }

        public void Contract(Inventory.Slot.Range range)
        {
            int start = range.Start;
            int num2 = start + range.Count;
            if ((start < 0) || (num2 > this.capacity))
            {
                throw new ArgumentOutOfRangeException();
            }
            if ((this.count != this.capacity) && (start != num2))
            {
                for (int i = 0; i < this.count; i++)
                {
                    if (this.indices[i] >= start)
                    {
                        if (this.indices[i] >= num2)
                        {
                            break;
                        }
                        do
                        {
                            int index = start++;
                            if (index != this.indices[i])
                            {
                                this.array[index] = this.array[this.indices[i]];
                                T local = default(T);
                                this.array[this.indices[i]] = local;
                                if (this.dirty.On(this.indices[i]))
                                {
                                    this.countDirty++;
                                }
                                this.indices[i] = (byte) index;
                                if (this.dirty.On(i))
                                {
                                    this.countDirty++;
                                }
                                if (start == num2)
                                {
                                    break;
                                }
                            }
                        }
                        while ((++i < this.count) && (this.indices[i] < num2));
                    }
                }
            }
        }

        private bool DoReplace(bool equalityCheck, int slot, T value, out T replacedValue)
        {
            replacedValue = this.array[slot];
            if (equalityCheck && object.Equals((T) replacedValue, value))
            {
                return false;
            }
            this.array[slot] = value;
            if (this.dirty.On(slot))
            {
                this.countDirty++;
            }
            return true;
        }

        private void DoSet(int slot, T value)
        {
            if ((this.count == 0) || (this.indices[0] > slot))
            {
                int count = this.count;
                for (int i = this.count - 1; i >= 0; i--)
                {
                    this.indices[count] = this.indices[i];
                    count--;
                }
                this.indices[0] = (byte) slot;
            }
            else
            {
                for (int j = this.count - 1; j >= 0; j--)
                {
                    if (this.indices[j] > slot)
                    {
                        this.indices[j + 1] = this.indices[j];
                    }
                    else
                    {
                        this.indices[j + 1] = (byte) slot;
                        break;
                    }
                }
            }
            this.array[slot] = value;
            this.count++;
            if (this.dirty.On(slot))
            {
                this.countDirty++;
            }
        }

        public bool Evict(int slot, out T value)
        {
            if ((slot < 0) || (slot >= this.capacity))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (this.occupied.Off(slot))
            {
                for (int i = 0; i < this.count; i++)
                {
                    if (this.indices[i] == slot)
                    {
                        for (int j = i + 1; j < this.count; j++)
                        {
                            this.indices[i] = this.indices[j];
                            i++;
                        }
                        this.indices[--this.count] = 0;
                        value = this.array[slot];
                        this.array[slot] = default(T);
                        if (this.dirty.On(slot))
                        {
                            this.countDirty++;
                        }
                        return true;
                    }
                }
                throw new InvalidOperationException();
            }
            value = default(T);
            return false;
        }

        public bool Get(int slot, out T value)
        {
            if ((slot < 0) || (slot >= this.capacity))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (this.occupied[slot])
            {
                value = this.array[slot];
                return true;
            }
            value = default(T);
            return false;
        }

        public bool GetByOrder(int index, out T value)
        {
            if (index < this.count)
            {
                value = this.array[this.indices[index]];
                return true;
            }
            value = default(T);
            return false;
        }

        public bool IsDirty(int slot)
        {
            return (((slot >= 0) && (slot < this.capacity)) && this.dirty[slot]);
        }

        public bool IsOccupied(int slot)
        {
            return (((slot >= 0) && (slot < this.capacity)) && this.occupied[slot]);
        }

        public bool IsVacant(int slot)
        {
            return (((slot >= 0) && (slot < this.capacity)) && !this.occupied[slot]);
        }

        public bool IsWithinRange(int slot)
        {
            return ((slot >= 0) && (slot < this.capacity));
        }

        public bool MarkClean(int slot)
        {
            if (((slot >= 0) && (slot < this.capacity)) && this.dirty.Off(slot))
            {
                this.countDirty--;
                return true;
            }
            return false;
        }

        public void MarkCompletelyClean()
        {
            this.dirty = new Inventory.Mask();
            this.countDirty = 0;
        }

        public void MarkCompletelyDirty()
        {
            this.dirty = new Inventory.Mask(0, this.capacity);
            this.countDirty = this.capacity;
        }

        public bool MarkDirty(int slot)
        {
            if (((slot >= 0) && (slot < this.capacity)) && this.dirty.On(slot))
            {
                this.countDirty++;
                return true;
            }
            return false;
        }

        public T[] OccupiedToArray()
        {
            T[] localArray = new T[this.count];
            for (int i = 0; i < this.count; i++)
            {
                localArray[i] = this.array[this.indices[i]];
            }
            return localArray;
        }

        public bool Occupy(int slot, T occupant)
        {
            if ((slot < 0) || (slot >= this.capacity))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (this.occupied.On(slot))
            {
                this.DoSet(slot, occupant);
                return true;
            }
            return false;
        }

        public bool Supplant(int slot, T value, out T replacedValue, bool equalityCheck)
        {
            if ((slot < 0) || (slot >= this.capacity))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!this.occupied.On(slot))
            {
                return this.DoReplace(equalityCheck, slot, value, out replacedValue);
            }
            replacedValue = default(T);
            return false;
        }

        public bool SupplantOrOccupy(int slot, T occupant, out T replacedValue, bool equalityCheck)
        {
            if ((slot < 0) || (slot >= this.capacity))
            {
                throw new ArgumentOutOfRangeException();
            }
            if (this.occupied.On(slot))
            {
                replacedValue = default(T);
                this.DoSet(slot, occupant);
                return false;
            }
            return this.DoReplace(equalityCheck, slot, occupant, out replacedValue);
        }

        public bool AnyVacantOrOccupied
        {
            get
            {
                return (this.capacity > 0);
            }
        }

        public int Capacity
        {
            get
            {
                return this.capacity;
            }
        }

        public bool CompletelyDirty
        {
            get
            {
                return ((this.countDirty == this.capacity) && (this.capacity > 0));
            }
        }

        public int DirtyCount
        {
            get
            {
                return this.countDirty;
            }
        }

        public int FirstOccupied
        {
            get
            {
                if (this.count > 0)
                {
                    return this.indices[0];
                }
                return -1;
            }
        }

        public int FirstVacancy
        {
            get
            {
                if (this.count == this.capacity)
                {
                    return -1;
                }
                for (int i = 0; i < 0x100; i++)
                {
                    if (!this.occupied[i])
                    {
                        return i;
                    }
                }
                throw new InvalidOperationException();
            }
        }

        public bool ForcedDirty
        {
            get
            {
                return this.forcedDirty;
            }
            set
            {
                if ((value != this.forcedDirty) && (this.capacity > 0))
                {
                    this.forcedDirty = value;
                }
            }
        }

        public bool HasAnyOccupant
        {
            get
            {
                return (this.count > 0);
            }
        }

        public bool HasNoOccupant
        {
            get
            {
                return (this.count == 0);
            }
        }

        public bool HasNoVacancy
        {
            get
            {
                return (this.count == this.capacity);
            }
        }

        public bool HasVacancy
        {
            get
            {
                return (this.count < this.capacity);
            }
        }

        public bool IsCompletelyVacant
        {
            get
            {
                return ((this.count == 0) && (this.capacity > 0));
            }
        }

        public int LastOccupied
        {
            get
            {
                if (this.count > 0)
                {
                    return this.indices[this.count - 1];
                }
                return -1;
            }
        }

        public bool MarkedDirty
        {
            get
            {
                return (this.forcedDirty || (this.countDirty > 0));
            }
        }

        public OccupiedCollection<T> Occupied
        {
            get
            {
                if (this.occupiedCollection == null)
                {
                }
                return (this.occupiedCollection = new OccupiedCollection<T>((Inventory.Collection<T>) this));
            }
        }

        public int OccupiedCount
        {
            get
            {
                return this.count;
            }
        }

        public OccupiedCollection<T>.Enumerator OccupiedEnumerator
        {
            get
            {
                return new OccupiedCollection<T>.Enumerator((Inventory.Collection<T>) this);
            }
        }

        public OccupiedCollection<T>.ReverseEnumerator OccupiedReverseEnumerator
        {
            get
            {
                return new OccupiedCollection<T>.ReverseEnumerator((Inventory.Collection<T>) this);
            }
        }

        public VacantCollection<T> Vacant
        {
            get
            {
                if (this.vacantCollection == null)
                {
                }
                return (this.vacantCollection = new VacantCollection<T>((Inventory.Collection<T>) this));
            }
        }

        public int VacantCount
        {
            get
            {
                return (this.capacity - this.count);
            }
        }

        public VacantCollection<T>.Enumerator VacantEnumerator
        {
            get
            {
                return new VacantCollection<T>.Enumerator((Inventory.Collection<T>) this);
            }
        }

        public static class Default
        {
            public static readonly Inventory.Collection<T> Empty;

            static Default()
            {
                Inventory.Collection<T>.Default.Empty = new Inventory.Collection<T>(0);
            }
        }

        public sealed class OccupiedCollection : IEnumerable, IEnumerable<T>
        {
            public readonly Inventory.Collection<T> Collection;

            internal OccupiedCollection(Inventory.Collection<T> collection)
            {
                this.Collection = collection;
            }

            public Enumerator<T> GetEnumerator()
            {
                return new Enumerator<T>(this.Collection);
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public T[] ToArray()
            {
                return this.Collection.OccupiedToArray();
            }

            public int Count
            {
                get
                {
                    return this.Collection.count;
                }
            }

            public bool Empty
            {
                get
                {
                    return (this.Collection.count == 0);
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
            {
                private Inventory.Collection<T> collection;
                private int indexPosition;
                internal Enumerator(Inventory.Collection<T> collection)
                {
                    this.collection = collection;
                    this.indexPosition = -1;
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this.collection.array[this.collection.indices[this.indexPosition]];
                    }
                }
                public bool MoveNext()
                {
                    return (++this.indexPosition < this.collection.count);
                }

                public T Current
                {
                    get
                    {
                        return this.collection.array[this.collection.indices[this.indexPosition]];
                    }
                }
                public int Slot
                {
                    get
                    {
                        return this.collection.indices[this.indexPosition];
                    }
                }
                public void Reset()
                {
                    this.indexPosition = -1;
                }

                public void Dispose()
                {
                    this.collection = null;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct ReverseEnumerator : IDisposable, IEnumerator, IEnumerator<T>
            {
                private Inventory.Collection<T> collection;
                private int indexPosition;
                internal ReverseEnumerator(Inventory.Collection<T> collection)
                {
                    this.collection = collection;
                    this.indexPosition = collection.count;
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this.collection.array[this.collection.indices[this.indexPosition]];
                    }
                }
                public bool MoveNext()
                {
                    return (--this.indexPosition >= 0);
                }

                public T Current
                {
                    get
                    {
                        return this.collection.array[this.collection.indices[this.indexPosition]];
                    }
                }
                public int Slot
                {
                    get
                    {
                        return this.collection.indices[this.indexPosition];
                    }
                }
                public void Reset()
                {
                    this.indexPosition = this.collection.count;
                }

                public void Dispose()
                {
                    this.collection = null;
                }
            }
        }

        public sealed class VacantCollection : IEnumerable, IEnumerable<int>
        {
            public readonly Inventory.Collection<T> Collection;

            internal VacantCollection(Inventory.Collection<T> collection)
            {
                this.Collection = collection;
            }

            public Enumerator<T> GetEnumerator()
            {
                return new Enumerator<T>(this.Collection);
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public int Count
            {
                get
                {
                    return (this.Collection.capacity - this.Collection.count);
                }
            }

            public bool Empty
            {
                get
                {
                    return (this.Collection.count == this.Collection.capacity);
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Enumerator : IDisposable, IEnumerator, IEnumerator<int>
            {
                private Inventory.Collection<T> collection;
                private int slotPosition;
                internal Enumerator(Inventory.Collection<T> collection)
                {
                    this.collection = collection;
                    this.slotPosition = -1;
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this.slotPosition;
                    }
                }
                public bool MoveNext()
                {
                    while (++this.slotPosition < this.collection.capacity)
                    {
                        if (!this.collection.occupied[this.slotPosition])
                        {
                            return true;
                        }
                    }
                    return false;
                }

                public int Current
                {
                    get
                    {
                        return this.slotPosition;
                    }
                }
                public void Reset()
                {
                    this.slotPosition = -1;
                }

                public void Dispose()
                {
                    this.collection = null;
                }
            }
        }
    }

    public static class Constants
    {
        public const int MaximumSlotCount = 0x100;
    }

    private static class Empty
    {
        public static readonly Inventory.SlotFlags[] SlotFlags = new Inventory.SlotFlags[0];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Mask
    {
        public int a;
        public int b;
        public int c;
        public int d;
        public int e;
        public int f;
        public int g;
        public int h;
        public Mask(bool defaultOn)
        {
            int num = !defaultOn ? 0 : -1;
            this.a = this.b = this.c = this.d = this.e = this.f = this.g = this.h = num;
        }

        public Mask(int onStart, int onCount) : this(false)
        {
            int num = onStart;
            int num2 = onStart + onCount;
            while ((num < 0x100) && (num < num2))
            {
                this[num] = true;
                num++;
            }
        }

        public bool any
        {
            get
            {
                return (((((this.a != 0) || (this.b != 0)) || ((this.c != 0) || (this.d != 0))) || (((this.e != 0) || (this.f != 0)) || (this.g != 0))) || (this.h != 0));
            }
        }
        public int firstOnBit
        {
            get
            {
                int num = 0;
                int h = 0;
                if (this.a == 0)
                {
                    num++;
                    if (this.b == 0)
                    {
                        num++;
                        if (this.c == 0)
                        {
                            num++;
                            if (this.d == 0)
                            {
                                num++;
                                if (this.e == 0)
                                {
                                    num++;
                                    if (this.f == 0)
                                    {
                                        num++;
                                        if (this.g == 0)
                                        {
                                            num++;
                                            if (this.h == 0)
                                            {
                                                num++;
                                                h = 0;
                                            }
                                            else
                                            {
                                                h = this.h;
                                            }
                                        }
                                        else
                                        {
                                            h = this.g;
                                        }
                                    }
                                    else
                                    {
                                        h = this.f;
                                    }
                                }
                                else
                                {
                                    h = this.e;
                                }
                            }
                            else
                            {
                                h = this.d;
                            }
                        }
                        else
                        {
                            h = this.c;
                        }
                    }
                    else
                    {
                        h = this.b;
                    }
                }
                else
                {
                    h = this.a;
                }
                int num3 = 0;
                for (int i = 0; i < 0x20; i++)
                {
                    if ((h & (((int) 1) << i)) == (((int) 1) << i))
                    {
                        break;
                    }
                    num3++;
                }
                return ((num * 0x20) + num3);
            }
        }
        public int lastOnBit
        {
            get
            {
                int num = 7;
                int a = 0;
                if (this.h == 0)
                {
                    num--;
                    if (this.g == 0)
                    {
                        num--;
                        if (this.f == 0)
                        {
                            num--;
                            if (this.e == 0)
                            {
                                num--;
                                if (this.d == 0)
                                {
                                    num--;
                                    if (this.c == 0)
                                    {
                                        num--;
                                        if (this.b == 0)
                                        {
                                            num--;
                                            if (this.a == 0)
                                            {
                                                return -1;
                                            }
                                            a = this.a;
                                        }
                                        else
                                        {
                                            a = this.b;
                                        }
                                    }
                                    else
                                    {
                                        a = this.c;
                                    }
                                }
                                else
                                {
                                    a = this.d;
                                }
                            }
                            else
                            {
                                a = this.e;
                            }
                        }
                        else
                        {
                            a = this.f;
                        }
                    }
                    else
                    {
                        a = this.g;
                    }
                }
                else
                {
                    a = this.h;
                }
                int num3 = 0;
                for (int i = 0x1f; i >= 0; i--)
                {
                    if ((a & (((int) 1) << i)) == (((int) 1) << i))
                    {
                        break;
                    }
                    num3++;
                }
                return ((num * 0x20) + num3);
            }
        }
        public bool this[int bit]
        {
            get
            {
                if (bit < 0x80)
                {
                    if (bit < 0x40)
                    {
                        if (bit < 0x20)
                        {
                            return ((this.a & (((int) 1) << bit)) != 0);
                        }
                        return ((this.b & (((int) 1) << (bit - 0x20))) != 0);
                    }
                    if (bit < 0x60)
                    {
                        return ((this.c & (((int) 1) << (bit - 0x40))) != 0);
                    }
                    return ((this.d & (((int) 1) << (bit - 0x60))) != 0);
                }
                if (bit < 0xc0)
                {
                    if (bit < 160)
                    {
                        return ((this.e & (((int) 1) << (bit - 0x80))) != 0);
                    }
                    return ((this.f & (((int) 1) << (bit - 160))) != 0);
                }
                if (bit < 0xe0)
                {
                    return ((this.g & (((int) 1) << (bit - 0xc0))) != 0);
                }
                return ((this.h & (((int) 1) << (bit - 0xe0))) != 0);
            }
            set
            {
                if (value)
                {
                    if (bit < 0x80)
                    {
                        if (bit < 0x40)
                        {
                            if (bit < 0x20)
                            {
                                this.a |= ((int) 1) << bit;
                            }
                            else
                            {
                                this.b |= ((int) 1) << (bit - 0x20);
                            }
                        }
                        else if (bit < 0x60)
                        {
                            this.c |= ((int) 1) << (bit - 0x40);
                        }
                        else
                        {
                            this.d |= ((int) 1) << (bit - 0x60);
                        }
                    }
                    else if (bit < 0xc0)
                    {
                        if (bit < 160)
                        {
                            this.e |= ((int) 1) << (bit - 0x80);
                        }
                        else
                        {
                            this.f |= ((int) 1) << (bit - 160);
                        }
                    }
                    else if (bit < 0xe0)
                    {
                        this.g |= ((int) 1) << (bit - 0xc0);
                    }
                    else
                    {
                        this.h |= ((int) 1) << (bit - 0xe0);
                    }
                }
                else if (bit < 0x80)
                {
                    if (bit < 0x40)
                    {
                        if (bit < 0x20)
                        {
                            this.a &= ~(((int) 1) << bit);
                        }
                        else
                        {
                            this.b &= ~(((int) 1) << (bit - 0x20));
                        }
                    }
                    else if (bit < 0x60)
                    {
                        this.c &= ~(((int) 1) << (bit - 0x40));
                    }
                    else
                    {
                        this.d &= ~(((int) 1) << (bit - 0x60));
                    }
                }
                else if (bit < 0xc0)
                {
                    if (bit < 160)
                    {
                        this.e &= ~(((int) 1) << (bit - 0x80));
                    }
                    else
                    {
                        this.f &= ~(((int) 1) << (bit - 160));
                    }
                }
                else if (bit < 0xe0)
                {
                    this.g &= ~(((int) 1) << (bit - 0xc0));
                }
                else
                {
                    this.h &= ~(((int) 1) << (bit - 0xe0));
                }
            }
        }
        public bool On(int bit)
        {
            int num;
            if (bit < 0x80)
            {
                if (bit < 0x40)
                {
                    if (bit < 0x20)
                    {
                        num = ((int) 1) << bit;
                        if ((num != 0) && ((this.a & num) == 0))
                        {
                            this.a |= num;
                            return true;
                        }
                        return false;
                    }
                    num = ((int) 1) << (bit - 0x20);
                    if ((num != 0) && ((this.b & num) == 0))
                    {
                        this.b |= num;
                        return true;
                    }
                    return false;
                }
                if (bit < 0x60)
                {
                    num = ((int) 1) << (bit - 0x40);
                    if ((num != 0) && ((this.c & num) == 0))
                    {
                        this.c |= num;
                        return true;
                    }
                    return false;
                }
                num = ((int) 1) << (bit - 0x60);
                if ((num != 0) && ((this.d & num) == 0))
                {
                    this.d |= num;
                    return true;
                }
                return false;
            }
            if (bit < 0xc0)
            {
                if (bit < 160)
                {
                    num = ((int) 1) << (bit - 0x80);
                    if ((num != 0) && ((this.e & num) == 0))
                    {
                        this.e |= num;
                        return true;
                    }
                    return false;
                }
                num = ((int) 1) << (bit - 160);
                if ((num != 0) && ((this.f & num) == 0))
                {
                    this.f |= num;
                    return true;
                }
                return false;
            }
            if (bit < 0xe0)
            {
                num = ((int) 1) << (bit - 0xc0);
                if ((num != 0) && ((this.g & num) == 0))
                {
                    this.g |= num;
                    return true;
                }
                return false;
            }
            num = ((int) 1) << (bit - 0xe0);
            if ((num != 0) && ((this.h & num) == 0))
            {
                this.h |= num;
                return true;
            }
            return false;
        }

        public bool Off(int bit)
        {
            int num;
            if (bit < 0x80)
            {
                if (bit < 0x40)
                {
                    if (bit < 0x20)
                    {
                        num = ((int) 1) << bit;
                        if ((num != 0) && ((this.a & num) == num))
                        {
                            this.a &= ~num;
                            return true;
                        }
                        return false;
                    }
                    num = ((int) 1) << (bit - 0x20);
                    if ((num != 0) && ((this.b & num) == num))
                    {
                        this.b &= ~num;
                        return true;
                    }
                    return false;
                }
                if (bit < 0x60)
                {
                    num = ((int) 1) << (bit - 0x40);
                    if ((num != 0) && ((this.c & num) == num))
                    {
                        this.c &= ~num;
                        return true;
                    }
                    return false;
                }
                num = ((int) 1) << (bit - 0x60);
                if ((num != 0) && ((this.d & num) == num))
                {
                    this.d &= ~num;
                    return true;
                }
                return false;
            }
            if (bit < 0xc0)
            {
                if (bit < 160)
                {
                    num = ((int) 1) << (bit - 0x80);
                    if ((num != 0) && ((this.e & num) == num))
                    {
                        this.e &= ~num;
                        return true;
                    }
                    return false;
                }
                num = ((int) 1) << (bit - 160);
                if ((num != 0) && ((this.f & num) == num))
                {
                    this.f &= ~num;
                    return true;
                }
                return false;
            }
            if (bit < 0xe0)
            {
                num = ((int) 1) << (bit - 0xc0);
                if ((num != 0) && ((this.g & num) == num))
                {
                    this.g &= ~num;
                    return true;
                }
                return false;
            }
            num = ((int) 1) << (bit - 0xe0);
            if ((num != 0) && ((this.h & num) == num))
            {
                this.h &= ~num;
                return true;
            }
            return false;
        }

        public int CountOnBits()
        {
            uint a;
            int num = 0;
            if (this.a != 0)
            {
                a = (uint) this.a;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            if (this.b != 0)
            {
                a = (uint) this.b;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            if (this.c != 0)
            {
                a = (uint) this.c;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            if (this.d != 0)
            {
                a = (uint) this.d;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            if (this.e != 0)
            {
                a = (uint) this.e;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            if (this.f != 0)
            {
                a = (uint) this.f;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            if (this.g != 0)
            {
                a = (uint) this.g;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            if (this.h != 0)
            {
                a = (uint) this.h;
                while (a != 0)
                {
                    a &= a - 1;
                    num++;
                }
            }
            return num;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OccupiedIterator : IDisposable
    {
        private Inventory.Collection<InventoryItem>.OccupiedCollection.Enumerator baseEnumerator;
        public OccupiedIterator(Inventory inventory)
        {
            this.baseEnumerator = inventory.collection.OccupiedEnumerator;
        }

        public void Reset()
        {
            this.baseEnumerator.Reset();
        }

        public IInventoryItem item
        {
            get
            {
                return this.baseEnumerator.Current.iface;
            }
        }
        internal InventoryItem inventoryItem
        {
            get
            {
                return this.baseEnumerator.Current;
            }
        }
        public int slot
        {
            get
            {
                return this.baseEnumerator.Slot;
            }
        }
        public bool Next()
        {
            return this.baseEnumerator.MoveNext();
        }

        public void Dispose()
        {
            this.baseEnumerator.Dispose();
        }

        internal bool Next(out InventoryItem item, out int slot)
        {
            if (this.Next())
            {
                slot = this.baseEnumerator.Slot;
                item = this.baseEnumerator.Current;
                return true;
            }
            slot = -1;
            item = null;
            return false;
        }

        internal bool Next(int datablockUniqueID, out InventoryItem item, out int slot)
        {
            while (this.Next(out item, out slot))
            {
                if (item.datablockUniqueID == datablockUniqueID)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool Next(ItemDataBlock datablock, out InventoryItem item, out int slot)
        {
            return this.Next(datablock.uniqueID, out item, out slot);
        }

        public bool Next(out IInventoryItem item, out int slot)
        {
            InventoryItem item2;
            if (this.Next(out item2, out slot))
            {
                item = item2.iface;
                return true;
            }
            item = null;
            return false;
        }

        public bool Next(int datablockUniqueID, out IInventoryItem item, out int slot)
        {
            InventoryItem item2;
            if (this.Next(datablockUniqueID, out item2, out slot))
            {
                item = item2.iface;
                return true;
            }
            item = null;
            return false;
        }

        internal bool Next(ItemDataBlock datablock, out IInventoryItem item, out int slot)
        {
            return this.Next(datablock.uniqueID, out item, out slot);
        }

        public bool Next<TItemInterface>(out TItemInterface item, out int slot) where TItemInterface: class, IInventoryItem
        {
            IInventoryItem item2;
            while (this.Next(out item2, out slot))
            {
                if (item2 is TItemInterface)
                {
                    item = (TItemInterface) this.inventoryItem.iface;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool Next<TItemInterface>(int datablockUniqueID, out TItemInterface item, out int slot) where TItemInterface: class, IInventoryItem
        {
            IInventoryItem item2;
            while (this.Next(datablockUniqueID, out item2, out slot))
            {
                if (item2 is TItemInterface)
                {
                    item = (TItemInterface) this.inventoryItem.iface;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool Next<TItemInterface>(ItemDataBlock datablock, out TItemInterface item, out int slot) where TItemInterface: class, IInventoryItem
        {
            return this.Next<TItemInterface>(datablock.uniqueID, out item, out slot);
        }

        public bool Next(out int slot)
        {
            InventoryItem item;
            return this.Next(out item, out slot);
        }

        public bool Next(int datablockUniqueID, out int slot)
        {
            InventoryItem item;
            return this.Next(out item, out slot);
        }

        public bool Next(ItemDataBlock datablock, out int slot)
        {
            InventoryItem item;
            return this.Next(datablock.uniqueID, out item, out slot);
        }

        public bool Next<TItemInterface>(out int slot) where TItemInterface: class, IInventoryItem
        {
            TItemInterface local;
            return this.Next<TItemInterface>(out local, out slot);
        }

        public bool Next<TItemInterface>(int datablockUniqueID, out int slot) where TItemInterface: class, IInventoryItem
        {
            TItemInterface local;
            return this.Next<TItemInterface>(out local, out slot);
        }

        public bool Next<TItemInterface>(ItemDataBlock datablock, out int slot) where TItemInterface: class, IInventoryItem
        {
            TItemInterface local;
            return this.Next<TItemInterface>(datablock.uniqueID, out local, out slot);
        }

        internal bool Next(out InventoryItem item)
        {
            int num;
            return this.Next(out item, out num);
        }

        internal bool Next(int datablockUniqueID, out InventoryItem item)
        {
            int num;
            return this.Next(datablockUniqueID, out item, out num);
        }

        internal bool Next(ItemDataBlock datablock, out InventoryItem item)
        {
            int num;
            return this.Next(datablock.uniqueID, out item, out num);
        }

        public bool Next(out IInventoryItem item)
        {
            int num;
            return this.Next(out item, out num);
        }

        public bool Next(int datablockUniqueID, out IInventoryItem item)
        {
            int num;
            return this.Next(datablockUniqueID, out item, out num);
        }

        internal bool Next(ItemDataBlock datablock, out IInventoryItem item)
        {
            int num;
            return this.Next(datablock.uniqueID, out item, out num);
        }

        public bool Next<TItemInterface>(out TItemInterface item) where TItemInterface: class, IInventoryItem
        {
            int num;
            return this.Next<TItemInterface>(out item, out num);
        }

        public bool Next<TItemInterface>(int datablockUniqueID, out TItemInterface item) where TItemInterface: class, IInventoryItem
        {
            int num;
            return this.Next<TItemInterface>(datablockUniqueID, out item, out num);
        }

        public bool Next<TItemInterface>(ItemDataBlock datablock, out TItemInterface item) where TItemInterface: class, IInventoryItem
        {
            int num;
            return this.Next<TItemInterface>(datablock.uniqueID, out item, out num);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OccupiedReverseIterator : IDisposable
    {
        private Inventory.Collection<InventoryItem>.OccupiedCollection.ReverseEnumerator baseEnumerator;
        public OccupiedReverseIterator(Inventory inventory)
        {
            this.baseEnumerator = inventory.collection.OccupiedReverseEnumerator;
        }

        public void Reset()
        {
            this.baseEnumerator.Reset();
        }

        public IInventoryItem item
        {
            get
            {
                return this.baseEnumerator.Current.iface;
            }
        }
        internal InventoryItem inventoryItem
        {
            get
            {
                return this.baseEnumerator.Current;
            }
        }
        public int slot
        {
            get
            {
                return this.baseEnumerator.Slot;
            }
        }
        public bool Next()
        {
            return this.baseEnumerator.MoveNext();
        }

        public void Dispose()
        {
            this.baseEnumerator.Dispose();
        }

        internal bool Next(out InventoryItem item, out int slot)
        {
            if (this.Next())
            {
                slot = this.baseEnumerator.Slot;
                item = this.baseEnumerator.Current;
                return true;
            }
            slot = -1;
            item = null;
            return false;
        }

        internal bool Next(int datablockUniqueID, out InventoryItem item, out int slot)
        {
            while (this.Next(out item, out slot))
            {
                if (item.datablockUniqueID == datablockUniqueID)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool Next(ItemDataBlock datablock, out InventoryItem item, out int slot)
        {
            return this.Next(datablock.uniqueID, out item, out slot);
        }

        public bool Next(out IInventoryItem item, out int slot)
        {
            InventoryItem item2;
            if (this.Next(out item2, out slot))
            {
                item = item2.iface;
                return true;
            }
            item = null;
            return false;
        }

        public bool Next(int datablockUniqueID, out IInventoryItem item, out int slot)
        {
            InventoryItem item2;
            if (this.Next(datablockUniqueID, out item2, out slot))
            {
                item = item2.iface;
                return true;
            }
            item = null;
            return false;
        }

        internal bool Next(ItemDataBlock datablock, out IInventoryItem item, out int slot)
        {
            return this.Next(datablock.uniqueID, out item, out slot);
        }

        public bool Next<TItemInterface>(out TItemInterface item, out int slot) where TItemInterface: class, IInventoryItem
        {
            IInventoryItem item2;
            while (this.Next(out item2, out slot))
            {
                if (item2 is TItemInterface)
                {
                    item = (TItemInterface) this.inventoryItem.iface;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool Next<TItemInterface>(int datablockUniqueID, out TItemInterface item, out int slot) where TItemInterface: class, IInventoryItem
        {
            IInventoryItem item2;
            while (this.Next(datablockUniqueID, out item2, out slot))
            {
                if (item2 is TItemInterface)
                {
                    item = (TItemInterface) this.inventoryItem.iface;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public bool Next<TItemInterface>(ItemDataBlock datablock, out TItemInterface item, out int slot) where TItemInterface: class, IInventoryItem
        {
            return this.Next<TItemInterface>(datablock.uniqueID, out item, out slot);
        }

        public bool Next(out int slot)
        {
            InventoryItem item;
            return this.Next(out item, out slot);
        }

        public bool Next(int datablockUniqueID, out int slot)
        {
            InventoryItem item;
            return this.Next(out item, out slot);
        }

        public bool Next(ItemDataBlock datablock, out int slot)
        {
            InventoryItem item;
            return this.Next(datablock.uniqueID, out item, out slot);
        }

        public bool Next<TItemInterface>(out int slot) where TItemInterface: class, IInventoryItem
        {
            TItemInterface local;
            return this.Next<TItemInterface>(out local, out slot);
        }

        public bool Next<TItemInterface>(int datablockUniqueID, out int slot) where TItemInterface: class, IInventoryItem
        {
            TItemInterface local;
            return this.Next<TItemInterface>(out local, out slot);
        }

        public bool Next<TItemInterface>(ItemDataBlock datablock, out int slot) where TItemInterface: class, IInventoryItem
        {
            TItemInterface local;
            return this.Next<TItemInterface>(datablock.uniqueID, out local, out slot);
        }

        internal bool Next(out InventoryItem item)
        {
            int num;
            return this.Next(out item, out num);
        }

        internal bool Next(int datablockUniqueID, out InventoryItem item)
        {
            int num;
            return this.Next(datablockUniqueID, out item, out num);
        }

        internal bool Next(ItemDataBlock datablock, out InventoryItem item)
        {
            int num;
            return this.Next(datablock.uniqueID, out item, out num);
        }

        public bool Next(out IInventoryItem item)
        {
            int num;
            return this.Next(out item, out num);
        }

        public bool Next(int datablockUniqueID, out IInventoryItem item)
        {
            int num;
            return this.Next(datablockUniqueID, out item, out num);
        }

        internal bool Next(ItemDataBlock datablock, out IInventoryItem item)
        {
            int num;
            return this.Next(datablock.uniqueID, out item, out num);
        }

        public bool Next<TItemInterface>(out TItemInterface item) where TItemInterface: class, IInventoryItem
        {
            int num;
            return this.Next<TItemInterface>(out item, out num);
        }

        public bool Next<TItemInterface>(int datablockUniqueID, out TItemInterface item) where TItemInterface: class, IInventoryItem
        {
            int num;
            return this.Next<TItemInterface>(datablockUniqueID, out item, out num);
        }

        public bool Next<TItemInterface>(ItemDataBlock datablock, out TItemInterface item) where TItemInterface: class, IInventoryItem
        {
            int num;
            return this.Next<TItemInterface>(datablock.uniqueID, out item, out num);
        }
    }

    private static class Payload
    {
        private const Opt NoOp1_Mask = (Opt.DoNotAssign | Opt.DoNotStack);
        private const Opt NoOp2_Mask = (Opt.IgnoreSlotOffset | Opt.RestrictToOffset);

        public static Result AddItem(Inventory inventory, ref Inventory.Addition addition, Opt options, InventoryItem reuseItem)
        {
            Result result;
            Inventory.Slot.Range range;
            int count;
            StackResult noneNotMarked;
            InventoryItem item;
            if ((((byte) (options & (Opt.DoNotAssign | Opt.DoNotStack))) == 3) || (((byte) (options & (Opt.IgnoreSlotOffset | Opt.RestrictToOffset))) == 12))
            {
                result.item = null;
                result.flags = Result.Flags.OptionsResultedInNoOp;
                result.usesRemaining = 0;
                return result;
            }
            ItemDataBlock itemDataBlock = addition.ItemDataBlock;
            if (itemDataBlock == null)
            {
                result.item = null;
                result.flags = Result.Flags.NoItemDatablock;
                result.usesRemaining = 0;
                return result;
            }
            Inventory.Slot.KindFlags primaryKindFlags = addition.SlotPreference.PrimaryKindFlags;
            Inventory.Slot.KindFlags secondaryKindFlags = addition.SlotPreference.SecondaryKindFlags;
            if (((byte) (options & Opt.IgnoreSlotOffset)) == 4)
            {
                range = new Inventory.Slot.Range();
            }
            else
            {
                range = RangeArray.CalculateExplicitSlotPosition(inventory, ref addition.SlotPreference);
            }
            bool flag = ((byte) (options & Opt.RestrictToOffset)) == 8;
            bool any = range.Any;
            if (flag && !any)
            {
                result.item = null;
                result.flags = Result.Flags.MissingRequiredOffset;
                result.usesRemaining = 0;
                return result;
            }
            if (flag)
            {
                RangeArray.FillTemporaryRanges(ref RangeArray.Primary, inventory, 0, range, true);
                RangeArray.FillTemporaryRanges(ref RangeArray.Secondary, inventory, 0, range, false);
            }
            else
            {
                RangeArray.FillTemporaryRanges(ref RangeArray.Primary, inventory, primaryKindFlags, range, true);
                RangeArray.FillTemporaryRanges(ref RangeArray.Secondary, inventory, secondaryKindFlags, range, false);
            }
            if (RangeArray.Primary.Count == 0)
            {
                primaryKindFlags = 0;
                if (RangeArray.Secondary.Count == 0)
                {
                    secondaryKindFlags = 0;
                    count = 0;
                }
                else
                {
                    count = RangeArray.Secondary.Count;
                }
            }
            else if (RangeArray.Secondary.Count == 0)
            {
                secondaryKindFlags = 0;
                count = RangeArray.Primary.Count;
            }
            else
            {
                count = RangeArray.Primary.Count + RangeArray.Secondary.Count;
            }
            if ((count == 0) || (!any && (((byte) (((byte) (primaryKindFlags | secondaryKindFlags)) & 7)) == 0)))
            {
                result.item = null;
                result.flags = Result.Flags.MissingRequiredOffset | Result.Flags.NoItemDatablock;
                result.usesRemaining = 0;
                return result;
            }
            int num2 = itemDataBlock._maxUses;
            bool flag3 = ((byte) (options & Opt.ReuseItem)) == 0x10;
            if (flag3 && (object.ReferenceEquals(reuseItem, null) || (itemDataBlock.untransferable && (reuseItem.inventory != inventory))))
            {
                result.flags = Result.Flags.FailedToCreate | Result.Flags.NoItemDatablock;
                result.item = null;
                result.usesRemaining = 0;
                return result;
            }
            Inventory.Collection<InventoryItem> collection = inventory.collection;
            result.usesRemaining = !flag3 ? addition.UsesQuantity.CalculateCount(itemDataBlock) : reuseItem.uses;
            if ((((byte) (options & Opt.DoNotStack)) == 1) || (((byte) (addition.SlotPreference.Flags & (Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Stack))) != 8))
            {
                item = null;
                noneNotMarked = StackResult.NoneNotMarked;
            }
            else
            {
                StackResult result4;
                InventoryItem item2;
                InventoryItem item3;
                StackArguments arguments;
                arguments.collection = collection;
                arguments.datablockUID = itemDataBlock.uniqueID;
                arguments.splittable = itemDataBlock.IsSplittable();
                arguments.useCount = result.usesRemaining;
                arguments.prefFlags = addition.SlotPreference.Flags;
                StackResult result3 = StackUses(ref arguments, ref RangeArray.Primary, out item2);
                switch (result3)
                {
                    case StackResult.NoneUnsplittable:
                    case StackResult.Complete:
                        item = item3 = item2;
                        noneNotMarked = result4 = result3;
                        break;

                    default:
                        result4 = StackUses(ref arguments, ref RangeArray.Secondary, out item3);
                        if (result3 > result4)
                        {
                            if (item2 != null)
                            {
                                item = item2;
                            }
                            else
                            {
                                item = item3;
                            }
                            noneNotMarked = result3;
                        }
                        else
                        {
                            if (item2 != null)
                            {
                                item = item2;
                            }
                            else
                            {
                                item = item3;
                            }
                            noneNotMarked = result4;
                        }
                        break;
                }
                result.usesRemaining = arguments.useCount;
            }
            switch (noneNotMarked)
            {
                case StackResult.Complete:
                    result.item = item;
                    result.flags = Result.Flags.Complete | Result.Flags.Stacked;
                    return result;

                case StackResult.Partial:
                    result.item = item;
                    result.flags = Result.Flags.OptionsResultedInNoOp | Result.Flags.Stacked;
                    break;

                default:
                    result.flags = Result.Flags.OptionsResultedInNoOp;
                    break;
            }
            if (((byte) (options & Opt.DoNotAssign)) != 2)
            {
                Assignment assignment;
                bool flag4;
                if (collection.HasNoVacancy)
                {
                    result.item = item;
                    result.flags = (Result.Flags) ((byte) (result.flags | Result.Flags.NoVacancy));
                    return result;
                }
                assignment.inventory = inventory;
                assignment.collection = collection;
                assignment.fresh = !flag3;
                assignment.item = !assignment.fresh ? reuseItem : (itemDataBlock.CreateItem() as InventoryItem);
                assignment.uses = result.usesRemaining;
                assignment.datablock = itemDataBlock;
                if (!flag3 && object.ReferenceEquals(assignment.item, null))
                {
                    result.item = item;
                    result.flags = (Result.Flags) ((byte) (result.flags | (!assignment.fresh ? (Result.Flags.FailedToCreate | Result.Flags.NoItemDatablock) : Result.Flags.FailedToCreate)));
                    return result;
                }
                assignment.slot = -1;
                assignment.attemptsMade = 0;
                using (Inventory.Collection<InventoryItem>.VacantCollection.Enumerator enumerator = collection.VacantEnumerator)
                {
                    flag4 = AssignItemInsideRanges(ref enumerator, ref RangeArray.Primary, ref assignment) || AssignItemInsideRanges(ref enumerator, ref RangeArray.Secondary, ref assignment);
                }
                if (flag4)
                {
                    result.flags = (Result.Flags) ((byte) (result.flags | Result.Flags.AssignedInstance | Result.Flags.Complete));
                    result.item = assignment.item;
                    result.usesRemaining -= result.item.uses;
                    return result;
                }
                if (assignment.attemptsMade > 0)
                {
                    result.flags = (Result.Flags) ((byte) (result.flags | Result.Flags.NoVacancy));
                    result.item = item;
                    return result;
                }
                result.flags = (Result.Flags) ((byte) (result.flags | Result.Flags.MissingRequiredOffset | Result.Flags.NoItemDatablock));
                result.item = item;
                return result;
            }
            result.item = item;
            if (result.flags == Result.Flags.OptionsResultedInNoOp)
            {
                result.flags = Result.Flags.MissingRequiredOffset;
            }
            return result;
        }

        private static bool AssignItem(ref Assignment args)
        {
            if (args.inventory.CheckSlotFlagsAgainstSlot(args.datablock._itemFlags, args.slot) && args.item.CanMoveToSlot(args.inventory, args.slot))
            {
                args.attemptsMade++;
                if (args.collection.Occupy(args.slot, args.item))
                {
                    if (!args.fresh && (args.item.inventory != null))
                    {
                        args.item.inventory.RemoveItem(args.item.slot);
                    }
                    args.item.SetUses(args.uses);
                    args.item.OnAddedTo(args.inventory, args.slot);
                    args.inventory.ItemAdded(args.slot, args.item.iface);
                    return true;
                }
            }
            return false;
        }

        private static bool AssignItemInsideRanges(ref Inventory.Collection<InventoryItem>.VacantCollection.Enumerator enumerator, ref RangeArray.Holder ranges, ref Assignment args)
        {
            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges.Range[i].Count == 1)
                {
                    args.slot = ranges.Range[i].Start;
                    if (args.collection.IsOccupied(args.slot))
                    {
                        continue;
                    }
                    if (AssignItem(ref args))
                    {
                        return true;
                    }
                }
                enumerator.Reset();
                while (enumerator.MoveNext())
                {
                    bool flag;
                    args.slot = enumerator.Current;
                    switch ((ranges.Range[i].ContainEx(args.slot) + 1))
                    {
                        case 0:
                        {
                            continue;
                        }
                        case 2:
                            break;

                        default:
                            flag = false;
                            goto Label_00B8;
                    }
                    flag = true;
                Label_00B8:
                    if (flag)
                    {
                        break;
                    }
                    if (AssignItem(ref args))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static StackResult StackUses(ref StackArguments args, ref RangeArray.Holder ranges, out InventoryItem item)
        {
            if (ranges.Count == 0)
            {
                item = null;
                return StackResult.NoRange;
            }
            if (((byte) (args.prefFlags & (Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Stack))) == 8)
            {
                if (args.splittable)
                {
                    StackWork work;
                    work.gotFirstUsage = false;
                    work.firstUsage = null;
                    int useCount = args.useCount;
                    bool flag = false;
                    int slot = -1;
                    Inventory.Collection<InventoryItem>.OccupiedCollection.Enumerator occupiedEnumerator = new Inventory.Collection<InventoryItem>.OccupiedCollection.Enumerator();
                    try
                    {
                        for (int i = 0; i < ranges.Count; i++)
                        {
                            bool flag2;
                            if (ranges.Range[i].Count == 1)
                            {
                                if (args.collection.Get(work.slot = ranges.Range[i].Start, out work.instance) && StackUsesSlot(ref args, ref work))
                                {
                                    item = !work.gotFirstUsage ? work.instance : work.firstUsage;
                                    return StackResult.Complete;
                                }
                                continue;
                            }
                            if (flag)
                            {
                                if (ranges.Range[i].Start >= slot)
                                {
                                    if (ranges.Range[i].Start == slot)
                                    {
                                        work.slot = slot;
                                        work.instance = occupiedEnumerator.Current;
                                        if (StackUsesSlot(ref args, ref work))
                                        {
                                            item = !work.gotFirstUsage ? work.instance : work.firstUsage;
                                            return StackResult.Complete;
                                        }
                                    }
                                }
                                else
                                {
                                    occupiedEnumerator.Reset();
                                }
                            }
                            else
                            {
                                occupiedEnumerator = args.collection.OccupiedEnumerator;
                                flag = true;
                            }
                            while (flag2 = occupiedEnumerator.MoveNext())
                            {
                                slot = occupiedEnumerator.Slot;
                                if (ranges.Range[i].Start <= slot)
                                {
                                    if ((slot - ranges.Range[i].Start) >= ranges.Range[i].Count)
                                    {
                                        break;
                                    }
                                    work.slot = slot;
                                    work.instance = occupiedEnumerator.Current;
                                    if (StackUsesSlot(ref args, ref work))
                                    {
                                        item = !work.gotFirstUsage ? work.instance : work.firstUsage;
                                        return StackResult.Complete;
                                    }
                                }
                            }
                            if (!flag2)
                            {
                                slot = 0x101;
                            }
                        }
                    }
                    finally
                    {
                        if (flag)
                        {
                            occupiedEnumerator.Dispose();
                        }
                    }
                    if (work.gotFirstUsage)
                    {
                        item = work.firstUsage;
                        return ((args.useCount >= useCount) ? StackResult.None_FoundFull : StackResult.Partial);
                    }
                    item = null;
                    return StackResult.None;
                }
                item = null;
                return StackResult.NoneUnsplittable;
            }
            item = null;
            return StackResult.NoneNotMarked;
        }

        private static bool StackUsesSlot(ref StackArguments args, ref StackWork work)
        {
            if (work.instance.datablockUniqueID == args.datablockUID)
            {
                int useCount = args.useCount;
                args.useCount -= work.instance.AddUses(args.useCount);
                if (useCount != args.useCount)
                {
                    args.collection.MarkDirty(work.slot);
                    if (args.useCount == 0)
                    {
                        return true;
                    }
                    if (!work.gotFirstUsage)
                    {
                        work.firstUsage = work.instance;
                        work.gotFirstUsage = true;
                    }
                }
            }
            return false;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Assignment
        {
            public Inventory.Collection<InventoryItem> collection;
            public Inventory inventory;
            public InventoryItem item;
            public ItemDataBlock datablock;
            public int slot;
            public int uses;
            public bool fresh;
            public int attemptsMade;
        }

        [Flags]
        public enum Opt : byte
        {
            AllowStackedItemsToBeReturned = 0x20,
            DoNotAssign = 2,
            DoNotStack = 1,
            IgnoreSlotOffset = 4,
            RestrictToOffset = 8,
            ReuseItem = 0x10
        }

        private static class RangeArray
        {
            private const int ArrayElementCount = 6;
            public static Holder Primary = new Holder(new Inventory.Slot.Range[6]);
            public static Holder Secondary = new Holder(new Inventory.Slot.Range[6]);

            public static Inventory.Slot.Range CalculateExplicitSlotPosition(Inventory inventory, ref Inventory.Slot.Preference pref)
            {
                Inventory.Slot.Offset offset = pref.Offset;
                if (offset.Specified)
                {
                    Inventory.Slot.Range range;
                    if (offset.HasOffsetOfKind)
                    {
                        if (!inventory.slotRanges.TryGetValue(offset.OffsetOfKind, out range))
                        {
                            return new Inventory.Slot.Range();
                        }
                    }
                    else
                    {
                        range = new Inventory.Slot.Range(0, inventory.slotCount);
                    }
                    int slotOffset = offset.SlotOffset;
                    if (range.Count > slotOffset)
                    {
                        return new Inventory.Slot.Range(range.Start + slotOffset, 1);
                    }
                }
                return new Inventory.Slot.Range();
            }

            private static bool CheckSlotKindFlag(Inventory inventory, Inventory.Slot.KindFlags flags, Inventory.Slot.KindFlags flag, Inventory.Slot.Kind kind, ref int start, ref int count)
            {
                Inventory.Slot.Range range;
                if ((((((byte) (flags & flag)) == flag) && inventory.slotRanges.TryGetValue(kind, out range)) && range.Any) && (range.End <= inventory.slotCount))
                {
                    start = range.Start;
                    count = range.Count;
                    return true;
                }
                return false;
            }

            public static void FillTemporaryRanges(ref Holder temp, Inventory inventory, Inventory.Slot.KindFlags kindFlags, Inventory.Slot.Range explicitSlot, bool insertExplicitSlot)
            {
                int num3;
                kindFlags = (Inventory.Slot.KindFlags) ((byte) (kindFlags & (Inventory.Slot.KindFlags.Armor | Inventory.Slot.KindFlags.Belt | Inventory.Slot.KindFlags.Default)));
                temp.Count = 0;
                int start = 0;
                int count = 0;
                if (explicitSlot.Any)
                {
                    if (insertExplicitSlot)
                    {
                        temp.Range[temp.Count++] = explicitSlot;
                    }
                    num3 = explicitSlot.Start;
                }
                else
                {
                    num3 = -1;
                }
                for (Inventory.Slot.Kind kind = Inventory.Slot.Kind.Default; kind < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt); kind = (Inventory.Slot.Kind) (((int) kind) + 1))
                {
                    Inventory.Slot.KindFlags flag = (Inventory.Slot.KindFlags) ((byte) (((int) 1) << kind));
                    if (CheckSlotKindFlag(inventory, kindFlags, flag, kind, ref start, ref count))
                    {
                        temp.Insert(ref start, ref count, num3);
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Holder
            {
                public int Count;
                public readonly Inventory.Slot.Range[] Range;
                public Holder(Inventory.Slot.Range[] array)
                {
                    this.Count = 0;
                    this.Range = array;
                }

                public void Insert(ref int start, ref int count, int gougeIndex)
                {
                    Inventory.Slot.Range range = new Inventory.Slot.Range(start, count);
                    if (gougeIndex != -1)
                    {
                        Inventory.Slot.RangePair pair;
                        switch (range.Gouge(gougeIndex, out pair))
                        {
                            case 1:
                                this.Range[this.Count++] = pair.A;
                                break;

                            case 2:
                                this.Range[this.Count++] = pair.A;
                                this.Range[this.Count++] = pair.B;
                                break;
                        }
                    }
                    else
                    {
                        this.Range[this.Count++] = range;
                    }
                    start = count = 0;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Result
        {
            public InventoryItem item;
            public Flags flags;
            public int usesRemaining;
            [Flags]
            public enum Flags : byte
            {
                AssignedInstance = 0x40,
                Complete = 0x80,
                DidNotCreate = 6,
                FailedToCreate = 4,
                FailedToReuse = 5,
                MissingRequiredOffset = 2,
                NoItemDatablock = 1,
                NoSlotRanges = 3,
                NoVacancy = 0x10,
                OptionsResultedInNoOp = 0,
                Stacked = 0x20
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StackArguments
        {
            public Inventory.Collection<InventoryItem> collection;
            public Inventory.Slot.PreferenceFlags prefFlags;
            public int useCount;
            public int datablockUID;
            public bool splittable;
        }

        private enum StackResult : byte
        {
            Complete = 6,
            None = 0,
            None_FoundFull = 4,
            NoneNotMarked = 1,
            NoneUnsplittable = 2,
            NoRange = 3,
            Partial = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StackWork
        {
            public bool gotFirstUsage;
            public InventoryItem firstUsage;
            public int slot;
            public InventoryItem instance;
        }
    }

    private class Report
    {
        private int amount;
        private static bool begun;
        private ItemDataBlock datablock;
        private static readonly Dictionary<int, Inventory.Report> dict = new Dictionary<int, Inventory.Report>();
        private bool Disposed;
        private static Inventory.Report dump;
        private Inventory.Report dumpNext;
        private static int dumpSize;
        private Inventory.Report first;
        private InventoryItem item;
        private int length;
        private int maxUses;
        private bool splittable;
        private static int totalItemCount;
        private Inventory.Report typeNext;

        public static void Begin()
        {
            if (begun)
            {
                throw new InvalidOperationException();
            }
            begun = true;
            totalItemCount = 0;
        }

        public static Inventory.Transfer[] Build(Inventory.Slot.KindFlags fallbackKindFlags)
        {
            if (!begun)
            {
                throw new InvalidOperationException();
            }
            Inventory.Transfer[] transferArray = new Inventory.Transfer[totalItemCount];
            int slotNumber = 0;
            foreach (KeyValuePair<int, Inventory.Report> pair in dict)
            {
                Inventory.Transfer transfer;
                Inventory.Report first = pair.Value;
                transfer.addition.Ident = (Datablock.Ident) first.datablock;
                int length = first.length;
                first = first.first;
                bool splittable = first.splittable;
                for (int i = 0; i < length; i++)
                {
                    transfer.addition.SlotPreference = Inventory.Slot.Preference.Define(slotNumber, false, fallbackKindFlags);
                    transfer.addition.UsesQuantity = Inventory.Uses.Quantity.Manual(first.amount);
                    transfer.item = first.item;
                    transferArray[slotNumber++] = transfer;
                    Inventory.Report report2 = first;
                    first = first.typeNext;
                    if (!report2.Disposed)
                    {
                        report2.Disposed = true;
                        report2.dumpNext = dump;
                        report2.first = (Inventory.Report) (report2.typeNext = null);
                        report2.datablock = null;
                        report2.item = null;
                        dump = report2;
                        dumpSize++;
                    }
                }
            }
            dict.Clear();
            begun = false;
            return transferArray;
        }

        private static Inventory.Report Create()
        {
            if (dumpSize > 0)
            {
                Inventory.Report dump = Inventory.Report.dump;
                if (--dumpSize == 0)
                {
                    Inventory.Report.dump = null;
                }
                else
                {
                    Inventory.Report.dump = dump.dumpNext;
                }
                dump.dumpNext = null;
                dump.Disposed = false;
                dump.amount = 0;
                return dump;
            }
            return new Inventory.Report();
        }

        public static void Recover()
        {
            if (begun)
            {
                foreach (Inventory.Report report in dict.Values)
                {
                    if (!report.Disposed)
                    {
                        report.Disposed = true;
                        report.dumpNext = dump;
                        report.first = (Inventory.Report) (report.typeNext = null);
                        report.datablock = null;
                        report.item = null;
                        dump = report;
                        dumpSize++;
                    }
                }
                dict.Clear();
            }
        }

        public static void Take(InventoryItem item)
        {
            Inventory.Report report;
            int uses = item.uses;
            int datablockUniqueID = item.datablockUniqueID;
            if (dict.TryGetValue(datablockUniqueID, out report))
            {
                Inventory.Report first = report.first;
                if (report.splittable)
                {
                    int num3 = first.amount + uses;
                    if (num3 > item.maxUses)
                    {
                        Inventory.Report report3 = Create();
                        report3.typeNext = first;
                        report3.amount = num3 - report.maxUses;
                        report3.item = item;
                        first.amount = report.maxUses;
                        report.first = report3;
                        report.length++;
                        totalItemCount++;
                    }
                    else
                    {
                        report.first.amount = num3;
                    }
                }
                else
                {
                    Inventory.Report report4 = Create();
                    report4.typeNext = first;
                    report4.amount = uses;
                    report4.item = item;
                    report.first = report4;
                    report.length++;
                    totalItemCount++;
                }
            }
            else
            {
                ItemDataBlock datablock = item.datablock;
                if (datablock.transferable)
                {
                    Inventory.Report report5 = Create();
                    report5.amount = uses;
                    report5.splittable = datablock.IsSplittable();
                    report5.first = report5;
                    report5.length = 1;
                    report5.datablock = datablock;
                    report5.item = item;
                    if (report5.splittable)
                    {
                        report5.maxUses = item.maxUses;
                    }
                    dict.Add(item.datablockUniqueID, report5);
                    totalItemCount++;
                }
            }
        }
    }

    private static class Shuffle
    {
        private static readonly Random r = new Random();

        public static void Array<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int index = r.Next(i);
                if (index != i)
                {
                    T local = array[i];
                    array[i] = array[index];
                    array[index] = local;
                }
            }
        }
    }

    public static class Slot
    {
        private const Kind HiddenKind_Explicit = ((Kind) 4);
        private const Kind HiddenKind_Null = ((Kind) 5);
        public const Kind KindBegin = Kind.Default;
        public const int KindCount = 3;
        public const Kind KindEnd = (Kind.Armor | Kind.Belt);
        public const Kind KindFirst = Kind.Default;
        public const KindFlags KindFlagsMask_Kind = (KindFlags.Armor | KindFlags.Belt | KindFlags.Default);
        public const Kind KindLast = Kind.Armor;
        public const int NumberOfKinds = 3;
        private const int PrimaryShift = 4;

        public enum Kind : byte
        {
            Armor = 2,
            Belt = 1,
            Default = 0
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KindDictionary<TValue> : IEnumerable, IDictionary<Inventory.Slot.Kind, TValue>, ICollection<KeyValuePair<Inventory.Slot.Kind, TValue>>, IEnumerable<KeyValuePair<Inventory.Slot.Kind, TValue>>
        {
            private Member<TValue> mDefault;
            private Member<TValue> mBelt;
            private Member<TValue> mArmor;
            private sbyte count;
            void IDictionary<Inventory.Slot.Kind, TValue>.Add(Inventory.Slot.Kind key, TValue value)
            {
                if (this.GetMember(key).Defined)
                {
                    throw new ArgumentException("Key was already set to a value");
                }
                this.SetMember(key, new Member<TValue>(value));
                this.count = (sbyte) (this.count + 1);
            }

            ICollection<Inventory.Slot.Kind> IDictionary<Inventory.Slot.Kind, TValue>.Keys
            {
                get
                {
                    Inventory.Slot.Kind[] kindArray = new Inventory.Slot.Kind[(int) this.count];
                    int num = 0;
                    for (Inventory.Slot.Kind kind = Inventory.Slot.Kind.Default; kind < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt); kind = (Inventory.Slot.Kind) (((int) kind) + 1))
                    {
                        if (this.GetMember(kind).Defined)
                        {
                            kindArray[num++] = kind;
                        }
                    }
                    return kindArray;
                }
            }
            void ICollection<KeyValuePair<Inventory.Slot.Kind, TValue>>.Add(KeyValuePair<Inventory.Slot.Kind, TValue> item)
            {
                this[item.Key] = item.Value;
            }

            ICollection<TValue> IDictionary<Inventory.Slot.Kind, TValue>.Values
            {
                get
                {
                    TValue[] localArray = new TValue[(int) this.count];
                    int num = 0;
                    for (Inventory.Slot.Kind kind = Inventory.Slot.Kind.Default; kind < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt); kind = (Inventory.Slot.Kind) (((int) kind) + 1))
                    {
                        Member<TValue> member = this.GetMember(kind);
                        if (member.Defined)
                        {
                            localArray[num++] = member.Value;
                        }
                    }
                    return localArray;
                }
            }
            bool ICollection<KeyValuePair<Inventory.Slot.Kind, TValue>>.Contains(KeyValuePair<Inventory.Slot.Kind, TValue> item)
            {
                try
                {
                    Member<TValue> member = this.GetMember(item.Key);
                    if (!member.Defined)
                    {
                        return false;
                    }
                    KeyValuePair<Inventory.Slot.Kind, TValue> objA = new KeyValuePair<Inventory.Slot.Kind, TValue>(item.Key, member.Value);
                    return object.Equals(objA, item);
                }
                catch
                {
                    return false;
                }
            }

            void ICollection<KeyValuePair<Inventory.Slot.Kind, TValue>>.CopyTo(KeyValuePair<Inventory.Slot.Kind, TValue>[] array, int arrayIndex)
            {
                for (Inventory.Slot.Kind kind = Inventory.Slot.Kind.Default; kind < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt); kind = (Inventory.Slot.Kind) (((int) kind) + 1))
                {
                    Member<TValue> member = this.GetMember(kind);
                    if (member.Defined)
                    {
                        array[arrayIndex++] = new KeyValuePair<Inventory.Slot.Kind, TValue>(kind, member.Value);
                    }
                }
            }

            bool ICollection<KeyValuePair<Inventory.Slot.Kind, TValue>>.IsReadOnly
            {
                get
                {
                    return false;
                }
            }
            bool ICollection<KeyValuePair<Inventory.Slot.Kind, TValue>>.Remove(KeyValuePair<Inventory.Slot.Kind, TValue> item)
            {
                try
                {
                    Member<TValue> member = this.GetMember(item.Key);
                    if (member.Defined)
                    {
                        KeyValuePair<Inventory.Slot.Kind, TValue> objA = new KeyValuePair<Inventory.Slot.Kind, TValue>(item.Key, member.Value);
                        if (object.Equals(objA, item))
                        {
                            this.SetMember(item.Key, new Member<TValue>());
                            return true;
                        }
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }

            IEnumerator<KeyValuePair<Inventory.Slot.Kind, TValue>> IEnumerable<KeyValuePair<Inventory.Slot.Kind, TValue>>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private Member<TValue> GetMember(Inventory.Slot.Kind kind)
            {
                switch (kind)
                {
                    case Inventory.Slot.Kind.Default:
                        return this.mDefault;

                    case Inventory.Slot.Kind.Belt:
                        return this.mBelt;

                    case Inventory.Slot.Kind.Armor:
                        return this.mArmor;
                }
                throw new ArgumentNullException("Unimplemented kind");
            }

            private void SetMember(Inventory.Slot.Kind kind, Member<TValue> member)
            {
                switch (kind)
                {
                    case Inventory.Slot.Kind.Default:
                        this.mDefault = member;
                        break;

                    case Inventory.Slot.Kind.Belt:
                        this.mBelt = member;
                        break;

                    case Inventory.Slot.Kind.Armor:
                        this.mArmor = member;
                        break;

                    default:
                        throw new ArgumentNullException("Unimplemented kind");
                }
            }

            public int Count
            {
                get
                {
                    return (int) this.count;
                }
            }
            public TValue this[Inventory.Slot.Kind kind]
            {
                get
                {
                    Member<TValue> member = this.GetMember(kind);
                    if (!member.Defined)
                    {
                        throw new KeyNotFoundException();
                    }
                    return member.Value;
                }
                set
                {
                    if (!this.GetMember(kind).Defined)
                    {
                        this.SetMember(kind, new Member<TValue>(value));
                        this.count = (sbyte) (this.count + 1);
                    }
                    else
                    {
                        this.SetMember(kind, new Member<TValue>(value));
                    }
                }
            }
            public bool ContainsKey(Inventory.Slot.Kind key)
            {
                return (((key >= Inventory.Slot.Kind.Default) && (key < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt))) && this.GetMember(key).Defined);
            }

            public bool Remove(Inventory.Slot.Kind key)
            {
                if (this.GetMember(key).Defined)
                {
                    this.SetMember(key, new Member<TValue>());
                    this.count = (sbyte) (this.count - 1);
                    return true;
                }
                return false;
            }

            public void Clear()
            {
                for (Inventory.Slot.Kind kind = Inventory.Slot.Kind.Default; (this.count > 0) && (kind < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt)); kind = (Inventory.Slot.Kind) (((int) kind) + 1))
                {
                    this.Remove(kind);
                }
            }

            public bool TryGetValue(Inventory.Slot.Kind key, out TValue value)
            {
                Member<TValue> member;
                try
                {
                    member = this.GetMember(key);
                }
                catch (ArgumentNullException)
                {
                    value = default(TValue);
                    return false;
                }
                if (member.Defined)
                {
                    value = member.Value;
                    return true;
                }
                value = default(TValue);
                return false;
            }

            public unsafe Enumerator<TValue> GetEnumerator()
            {
                return new Enumerator<TValue>(*((Inventory.Slot.KindDictionary<TValue>*) this));
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<Inventory.Slot.Kind, TValue>>
            {
                private Inventory.Slot.KindDictionary<TValue> dict;
                private int kind;
                public Enumerator(Inventory.Slot.KindDictionary<TValue> dict)
                {
                    this.dict = dict;
                    this.kind = -1;
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }
                public void Reset()
                {
                    this.kind = -1;
                }

                public void Dispose()
                {
                    this.dict = new Inventory.Slot.KindDictionary<TValue>();
                }

                public KeyValuePair<Inventory.Slot.Kind, TValue> Current
                {
                    get
                    {
                        Inventory.Slot.KindDictionary<TValue>.Member member = this.dict.GetMember((Inventory.Slot.Kind) ((byte) this.kind));
                        return new KeyValuePair<Inventory.Slot.Kind, TValue>((Inventory.Slot.Kind) ((byte) this.kind), member.Value);
                    }
                }
                public bool MoveNext()
                {
                    Inventory.Slot.Kind kind;
                    while ((kind = (Inventory.Slot.Kind) ((byte) (++this.kind))) < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt))
                    {
                        if (this.dict.GetMember(kind).Defined)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct Member
            {
                public TValue Value;
                public bool Defined;
                public Member(TValue value)
                {
                    this.Value = value;
                    this.Defined = true;
                }
            }
        }

        [Flags]
        public enum KindFlags : byte
        {
            Armor = 4,
            Belt = 2,
            Default = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Offset
        {
            private Inventory.Slot.Kind kind;
            private byte offset;
            public Offset(int offset)
            {
                this.offset = (byte) offset;
                this.kind = (Inventory.Slot.Kind) 4;
            }

            public Offset(Inventory.Slot.Kind kind, int offset)
            {
                this.kind = kind;
                this.offset = (byte) offset;
            }

            public static Inventory.Slot.Offset None
            {
                get
                {
                    return new Inventory.Slot.Offset((Inventory.Slot.Kind) 5, 0);
                }
            }
            public bool Specified
            {
                get
                {
                    return ((this.kind < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt)) || ((this.kind >= ((Inventory.Slot.Kind) 4)) && (this.kind < ((Inventory.Slot.Kind) 5))));
                }
            }
            public bool HasOffsetOfKind
            {
                get
                {
                    return (this.kind < (Inventory.Slot.Kind.Armor | Inventory.Slot.Kind.Belt));
                }
            }
            public bool ExplicitSlot
            {
                get
                {
                    return (this.kind == ((Inventory.Slot.Kind) 4));
                }
            }
            public Inventory.Slot.Kind OffsetOfKind
            {
                get
                {
                    if (!this.HasOffsetOfKind)
                    {
                        throw new InvalidOperationException("You must check HasOffsetOfKind == true before requesting this value");
                    }
                    return this.kind;
                }
            }
            public int SlotOffset
            {
                get
                {
                    return this.offset;
                }
            }
            public override string ToString()
            {
                if (!this.Specified)
                {
                    return "[Unspecified]";
                }
                if (this.HasOffsetOfKind)
                {
                    return string.Format("[{0}+{1}]", this.OffsetOfKind, this.SlotOffset);
                }
                return string.Format("[{0}]", this.SlotOffset);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Preference
        {
            private const bool kDefaultStack = true;
            public readonly Inventory.Slot.PreferenceFlags Flags;
            private readonly byte offset;
            private Preference(Inventory.Slot.PreferenceFlags preferenceFlags, int primaryOffset)
            {
                this.Flags = preferenceFlags;
                this.offset = (byte) primaryOffset;
            }

            public bool IsUndefined
            {
                get
                {
                    return (((byte) (this.Flags & (Inventory.Slot.PreferenceFlags.Offset | Inventory.Slot.PreferenceFlags.Primary_Armor | Inventory.Slot.PreferenceFlags.Primary_Belt | Inventory.Slot.PreferenceFlags.Primary_Default | Inventory.Slot.PreferenceFlags.Secondary_Armor | Inventory.Slot.PreferenceFlags.Secondary_Belt | Inventory.Slot.PreferenceFlags.Secondary_Default))) == 0);
                }
            }
            public bool IsDefined
            {
                get
                {
                    return (((byte) (this.Flags & (Inventory.Slot.PreferenceFlags.Offset | Inventory.Slot.PreferenceFlags.Primary_Armor | Inventory.Slot.PreferenceFlags.Primary_Belt | Inventory.Slot.PreferenceFlags.Primary_Default | Inventory.Slot.PreferenceFlags.Secondary_Armor | Inventory.Slot.PreferenceFlags.Secondary_Belt | Inventory.Slot.PreferenceFlags.Secondary_Default))) != 0);
                }
            }
            public Inventory.Slot.KindFlags PrimaryKindFlags
            {
                get
                {
                    return (Inventory.Slot.KindFlags) ((byte) (((byte) (((byte) this.Flags) >> 4)) & 7));
                }
            }
            public Inventory.Slot.KindFlags SecondaryKindFlags
            {
                get
                {
                    return (Inventory.Slot.KindFlags) ((byte) (this.Flags & (Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Secondary_Armor | Inventory.Slot.PreferenceFlags.Secondary_Belt | Inventory.Slot.PreferenceFlags.Secondary_Default)));
                }
            }
            public bool HasOffset
            {
                get
                {
                    return (((byte) (this.Flags & Inventory.Slot.PreferenceFlags.Offset)) == 0x80);
                }
            }
            public bool Stack
            {
                get
                {
                    return (((byte) (this.Flags & (Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Stack))) == 8);
                }
            }
            public Inventory.Slot.Offset Offset
            {
                get
                {
                    if (((byte) (this.Flags & Inventory.Slot.PreferenceFlags.Offset)) == 0x80)
                    {
                        uint num = (uint) (((byte) (this.Flags & 0x7f)) >> 4);
                        if (num == 0)
                        {
                            return new Inventory.Slot.Offset(this.offset);
                        }
                        if ((num & (num - 1)) == 0)
                        {
                            Inventory.Slot.Kind kind = Inventory.Slot.Kind.Default;
                            while ((num = num >> 1) != 0)
                            {
                                kind = (Inventory.Slot.Kind) (((int) kind) + 1);
                            }
                            return new Inventory.Slot.Offset(kind, this.offset);
                        }
                    }
                    return Inventory.Slot.Offset.None;
                }
            }
            public Inventory.Slot.Preference CloneOffsetChange(int newOffset)
            {
                return new Inventory.Slot.Preference(this.Flags, newOffset);
            }

            public Inventory.Slot.Preference CloneStackChange(bool stack)
            {
                if (stack)
                {
                    return new Inventory.Slot.Preference((Inventory.Slot.PreferenceFlags) ((byte) (this.Flags | Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Stack)), this.offset);
                }
                return new Inventory.Slot.Preference((Inventory.Slot.PreferenceFlags) ((byte) (this.Flags & (Inventory.Slot.PreferenceFlags.Offset | Inventory.Slot.PreferenceFlags.Primary_Armor | Inventory.Slot.PreferenceFlags.Primary_Belt | Inventory.Slot.PreferenceFlags.Primary_Default | Inventory.Slot.PreferenceFlags.Secondary_Armor | Inventory.Slot.PreferenceFlags.Secondary_Belt | Inventory.Slot.PreferenceFlags.Secondary_Default))), this.offset);
            }

            public static Inventory.Slot.Preference Define(int slotNumber, bool stack, Inventory.Slot.KindFlags fallbackSlots)
            {
                Inventory.Slot.PreferenceFlags preferenceFlags = (Inventory.Slot.PreferenceFlags) ((byte) (fallbackSlots & (Inventory.Slot.KindFlags.Armor | Inventory.Slot.KindFlags.Belt | Inventory.Slot.KindFlags.Default)));
                if (stack)
                {
                    preferenceFlags = (Inventory.Slot.PreferenceFlags) ((byte) (preferenceFlags | (Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Stack)));
                }
                if (slotNumber >= 0)
                {
                    preferenceFlags = (Inventory.Slot.PreferenceFlags) ((byte) (preferenceFlags | Inventory.Slot.PreferenceFlags.Offset));
                }
                else
                {
                    slotNumber = 0;
                }
                return new Inventory.Slot.Preference(preferenceFlags, slotNumber);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind startSlotKind, int offsetOfSlotKind, bool stack, Inventory.Slot.KindFlags fallbackSlotKinds)
            {
                Inventory.Slot.PreferenceFlags flags = (Inventory.Slot.PreferenceFlags) ((byte) (fallbackSlotKinds & (Inventory.Slot.KindFlags.Armor | Inventory.Slot.KindFlags.Belt | Inventory.Slot.KindFlags.Default)));
                if (stack)
                {
                    flags = (Inventory.Slot.PreferenceFlags) ((byte) (flags | (Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Stack)));
                }
                if (offsetOfSlotKind >= 0)
                {
                    flags = (Inventory.Slot.PreferenceFlags) ((byte) (flags | Inventory.Slot.PreferenceFlags.Offset));
                }
                else
                {
                    offsetOfSlotKind = 0;
                }
                Inventory.Slot.PreferenceFlags flags2 = (Inventory.Slot.PreferenceFlags) ((byte) (((int) 1) << startSlotKind));
                flags = (Inventory.Slot.PreferenceFlags) ((byte) (flags & ((byte) ~flags2)));
                return new Inventory.Slot.Preference((Inventory.Slot.PreferenceFlags) ((byte) (flags | ((byte) (((byte) flags2) << 4)))), offsetOfSlotKind);
            }

            public static Inventory.Slot.Preference Define(int offsetOfSlotKind, bool stack)
            {
                return Define(offsetOfSlotKind, stack, 0);
            }

            public static Inventory.Slot.Preference Define(int offsetOfSlotKind, Inventory.Slot.KindFlags fallbackSlotKinds)
            {
                return Define(offsetOfSlotKind, true, fallbackSlotKinds);
            }

            public static Inventory.Slot.Preference Define(int offsetOfSlotKind, Inventory.Slot.Kind fallbackSlotKind)
            {
                return Define(offsetOfSlotKind, true, (Inventory.Slot.KindFlags) ((byte) (((int) 1) << fallbackSlotKind)));
            }

            public static Inventory.Slot.Preference Define(int offsetOfSlotKind)
            {
                return Define(offsetOfSlotKind, true, 0);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind startSlotKind, int offsetOfSlotKind, bool stack)
            {
                return Define(startSlotKind, offsetOfSlotKind, stack, 0);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind startSlotKind, int offsetOfSlotKind, Inventory.Slot.KindFlags fallbackSlotKinds)
            {
                return Define(startSlotKind, offsetOfSlotKind, true, fallbackSlotKinds);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind startSlotKind, int offsetOfSlotKind, Inventory.Slot.Kind fallbackSlotKind)
            {
                return Define(startSlotKind, offsetOfSlotKind, true, (Inventory.Slot.KindFlags) ((byte) (((int) 1) << fallbackSlotKind)));
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind startSlotKind, int offsetOfSlotKind)
            {
                return Define(startSlotKind, offsetOfSlotKind, true, 0);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.KindFlags firstPreferenceSlotKinds, bool stack, Inventory.Slot.KindFlags secondPreferenceSlotKinds)
            {
                Inventory.Slot.PreferenceFlags flags = (Inventory.Slot.PreferenceFlags) ((byte) (((byte) (secondPreferenceSlotKinds & (Inventory.Slot.KindFlags.Armor | Inventory.Slot.KindFlags.Belt | Inventory.Slot.KindFlags.Default))) & ((byte) ~firstPreferenceSlotKinds)));
                if (stack)
                {
                    flags = (Inventory.Slot.PreferenceFlags) ((byte) (flags | (Inventory.Slot.PreferenceFlags.Primary_ExplicitSlot | Inventory.Slot.PreferenceFlags.Stack)));
                }
                return new Inventory.Slot.Preference((Inventory.Slot.PreferenceFlags) ((byte) (flags | ((byte) (((byte) firstPreferenceSlotKinds) << 4)))), 0);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind firstPreferenceSlotKind, bool stack, Inventory.Slot.KindFlags secondPreferenceSlotKinds)
            {
                return Define((Inventory.Slot.KindFlags) ((byte) (((int) 1) << firstPreferenceSlotKind)), stack, secondPreferenceSlotKinds);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind firstPreferenceSlotKind, bool stack, Inventory.Slot.Kind secondPreferenceSlotKind)
            {
                return Define((Inventory.Slot.KindFlags) ((byte) (((int) 1) << firstPreferenceSlotKind)), stack, (Inventory.Slot.KindFlags) ((byte) (((int) 1) << secondPreferenceSlotKind)));
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.KindFlags firstPreferenceSlotKind, bool stack, Inventory.Slot.Kind secondPreferenceSlotKind)
            {
                return Define(firstPreferenceSlotKind, stack, (Inventory.Slot.KindFlags) ((byte) (((int) 1) << secondPreferenceSlotKind)));
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind slotsOfKind, bool stack)
            {
                return Define(slotsOfKind, stack, 0);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.KindFlags slotsOfKinds, bool stack)
            {
                return Define(slotsOfKinds, stack, 0);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind firstPreferenceSlotKind, Inventory.Slot.Kind secondPreferenceSlotKind)
            {
                return Define(firstPreferenceSlotKind, true, secondPreferenceSlotKind);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.KindFlags firstPreferenceSlotKinds, Inventory.Slot.KindFlags secondPreferenceSlotKinds)
            {
                return Define(firstPreferenceSlotKinds, true, secondPreferenceSlotKinds);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind firstPreferenceSlotKind, Inventory.Slot.KindFlags secondPreferenceSlotKinds)
            {
                return Define(firstPreferenceSlotKind, true, secondPreferenceSlotKinds);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.KindFlags firstPreferenceSlotKinds, Inventory.Slot.Kind secondPreferenceSlotKind)
            {
                return Define(firstPreferenceSlotKinds, true, secondPreferenceSlotKind);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.Kind slotsOfKind)
            {
                return Define(slotsOfKind, true, 0);
            }

            public static Inventory.Slot.Preference Define(Inventory.Slot.KindFlags slotsOfKinds)
            {
                return Define(slotsOfKinds, true, 0);
            }

            public override string ToString()
            {
                Inventory.Slot.KindFlags primaryKindFlags = this.PrimaryKindFlags;
                Inventory.Slot.KindFlags secondaryKindFlags = this.SecondaryKindFlags;
                Inventory.Slot.Offset offset = this.Offset;
                if (((int) secondaryKindFlags) != 0)
                {
                    if (offset.Specified)
                    {
                        if (offset.HasOffsetOfKind)
                        {
                            if (this.Stack)
                            {
                                return string.Format("[{0}+{1}|{2} (stack)]", offset.OffsetOfKind, offset.SlotOffset, secondaryKindFlags);
                            }
                            return string.Format("[{0}+{1}|{2}]", offset.OffsetOfKind, offset.SlotOffset, secondaryKindFlags);
                        }
                        if (this.Stack)
                        {
                            return string.Format("[{0}|{1} (stack)]", offset.SlotOffset, secondaryKindFlags);
                        }
                        return string.Format("[{0}|{1}]", offset.SlotOffset, secondaryKindFlags);
                    }
                    if (((int) primaryKindFlags) != 0)
                    {
                        if (this.Stack)
                        {
                            return string.Format("[{0}|{1} (stack)]", primaryKindFlags, secondaryKindFlags);
                        }
                        return string.Format("[{0}|{1}]", primaryKindFlags, secondaryKindFlags);
                    }
                    if (this.Stack)
                    {
                        return string.Format("[|{1} (stack)]", secondaryKindFlags);
                    }
                    return string.Format("[|{1}]", secondaryKindFlags);
                }
                if (offset.Specified)
                {
                    if (offset.HasOffsetOfKind)
                    {
                        if (this.Stack)
                        {
                            return string.Format("[{0}+{1} (stack)]", offset.OffsetOfKind, offset.SlotOffset);
                        }
                        return string.Format("[{0}+{1}]", offset.OffsetOfKind, offset.SlotOffset);
                    }
                    if (this.Stack)
                    {
                        return string.Format("[{0} (stack)]", offset.SlotOffset);
                    }
                    return string.Format("[{0}]", offset.SlotOffset);
                }
                if (((int) primaryKindFlags) == 0)
                {
                    return "[Undefined]";
                }
                if (this.Stack)
                {
                    return string.Format("[{0} (stack)]", primaryKindFlags);
                }
                return string.Format("[{0}]", primaryKindFlags);
            }

            public static implicit operator Inventory.Slot.Preference(int slot)
            {
                return new Inventory.Slot.Preference(Inventory.Slot.PreferenceFlags.Offset | Inventory.Slot.PreferenceFlags.Stack, (byte) slot);
            }

            public static implicit operator Inventory.Slot.Preference(Inventory.Slot.Kind kind)
            {
                return new Inventory.Slot.Preference((Inventory.Slot.PreferenceFlags) ((byte) (((byte) (((byte) (((byte) (1 << (kind & 0x1f))) & 7)) << 4)) | 8)), 0);
            }

            public static implicit operator Inventory.Slot.Preference(Inventory.Slot.KindFlags kindFlags)
            {
                return new Inventory.Slot.Preference((Inventory.Slot.PreferenceFlags) ((byte) (((byte) (((byte) (kindFlags & 7)) << 4)) | 8)), 0);
            }
        }

        [Flags]
        public enum PreferenceFlags : byte
        {
            Offset = 0x80,
            Primary_Armor = 0x40,
            Primary_Belt = 0x20,
            Primary_Default = 0x10,
            Primary_ExplicitSlot = 0,
            Secondary_Armor = 4,
            Secondary_Belt = 2,
            Secondary_Default = 1,
            Stack = 8
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Range
        {
            public readonly int Start;
            public readonly int Count;
            public Range(int start, int length)
            {
                this.Start = start;
                this.Count = length;
            }

            public int End
            {
                get
                {
                    return (this.Start + this.Count);
                }
            }
            public int Last
            {
                get
                {
                    return ((this.Count > 1) ? (this.Start + (this.Count - 1)) : this.Start);
                }
            }
            public bool Any
            {
                get
                {
                    return (this.Count > 0);
                }
            }
            public bool Contains(int i)
            {
                return ((this.Count > 0) && ((this.Start == i) || ((this.Start < i) && ((this.Start + this.Count) > i))));
            }

            public sbyte ContainEx(int i)
            {
                if (this.Start > i)
                {
                    return -1;
                }
                if ((i - this.Start) < this.Count)
                {
                    return 0;
                }
                return 1;
            }

            public int Gouge(int i, out Inventory.Slot.RangePair pair)
            {
                if ((this.Count <= 0) || ((this.Count == 1) && (i == this.Start)))
                {
                    pair = new Inventory.Slot.RangePair();
                    return 0;
                }
                if ((i < this.Start) || (i >= (this.Start + this.Count)))
                {
                    pair = new Inventory.Slot.RangePair(this);
                    return 1;
                }
                if (i == this.Start)
                {
                    pair = new Inventory.Slot.RangePair(new Inventory.Slot.Range(this.Start + 1, this.Count - 1));
                    return 1;
                }
                if (i == ((this.Start + this.Count) - 1))
                {
                    pair = new Inventory.Slot.RangePair(new Inventory.Slot.Range(this.Start, this.Count - 1));
                    return 1;
                }
                pair = new Inventory.Slot.RangePair(new Inventory.Slot.Range(this.Start, i - this.Start), new Inventory.Slot.Range(i + 1, this.Count - ((i - this.Start) + 1)));
                return 2;
            }

            public int Index(int offset)
            {
                int i = this.Start + offset;
                return (!this.Contains(i) ? -1 : i);
            }

            public int GetOffset(int i)
            {
                if (this.Contains(i))
                {
                    return (i - this.Start);
                }
                return -1;
            }

            public override string ToString()
            {
                return string.Format("[{0}:{1}]", this.Start, this.Count);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RangePair
        {
            public readonly Inventory.Slot.Range A;
            public readonly Inventory.Slot.Range B;
            public RangePair(Inventory.Slot.Range A, Inventory.Slot.Range B)
            {
                this.A = A;
                this.B = B;
            }

            public RangePair(Inventory.Slot.Range AB)
            {
                this.A = AB;
                this.B = AB;
            }
        }
    }

    [Flags]
    public enum SlotFlags
    {
        Belt = 1,
        Chest = 0x10,
        Cooked = 0x400,
        Debris = 0x100,
        Equip = 4,
        Feet = 0x40,
        FuelBasic = 0x80,
        Head = 8,
        Legs = 0x20,
        Raw = 0x200,
        Safe = -2147483648,
        Storage = 2
    }

    public enum SlotOperationResult : sbyte
    {
        Error_EmptyDestinationSlot = -4,
        Error_EmptySourceSlot = -5,
        Error_Failed = -1,
        Error_MissingInventory = -6,
        Error_NoOpArgs = -2,
        Error_OccupiedDestination = -8,
        Error_SameSlot = -7,
        Error_SlotRange = -3,
        NoOp = 0,
        Success_Combined = 2,
        Success_Moved = 4,
        Success_Stacked = 1
    }

    private enum SlotOperations : byte
    {
        Combine = 2,
        Move = 4,
        Stack = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SlotOperationsInfo
    {
        [NonSerialized]
        public readonly Inventory.SlotOperations SlotOperations;
        public SlotOperationsInfo(Inventory.SlotOperations SlotOperations)
        {
            this.SlotOperations = SlotOperations;
        }

        public override string ToString()
        {
            return this.SlotOperations.ToString();
        }

        public override bool Equals(object obj)
        {
            return ((obj is Inventory.SlotOperationsInfo) && this.Equals((Inventory.SlotOperationsInfo) obj));
        }

        public override int GetHashCode()
        {
            return (((byte) (this.SlotOperations & 7)) << 0x10);
        }

        public bool Equals(Inventory.SlotOperationsInfo other)
        {
            return (((byte) (this.SlotOperations & (Inventory.SlotOperations.Combine | Inventory.SlotOperations.Move | Inventory.SlotOperations.Stack))) == ((byte) (other.SlotOperations & (Inventory.SlotOperations.Combine | Inventory.SlotOperations.Move | Inventory.SlotOperations.Stack))));
        }

        public static implicit operator Inventory.SlotOperationsInfo(Inventory.SlotOperations ops)
        {
            return new Inventory.SlotOperationsInfo(ops);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Transfer
    {
        public InventoryItem item;
        public Inventory.Addition addition;
    }

    public static class Uses
    {
        public enum Quantifier : byte
        {
            Default = 0,
            Manual = 1,
            Maximum = 3,
            Minimum = 2,
            Random = 5,
            StackSize = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Quantity
        {
            public readonly Inventory.Uses.Quantifier Quantifier;
            private readonly byte manualAmount;
            public static readonly Inventory.Uses.Quantity Default;
            public static readonly Inventory.Uses.Quantity Minimum;
            public static readonly Inventory.Uses.Quantity Maximum;
            public static readonly Inventory.Uses.Quantity Random;
            private Quantity(Inventory.Uses.Quantifier quantifier, byte manualAmount)
            {
                this.Quantifier = quantifier;
                this.manualAmount = manualAmount;
            }

            static Quantity()
            {
                Default = new Inventory.Uses.Quantity(Inventory.Uses.Quantifier.Default, 0);
                Minimum = new Inventory.Uses.Quantity(Inventory.Uses.Quantifier.Minimum, 0);
                Maximum = new Inventory.Uses.Quantity(Inventory.Uses.Quantifier.Maximum, 0);
                Random = new Inventory.Uses.Quantity(Inventory.Uses.Quantifier.Random, 0);
            }

            public int ManualAmount
            {
                get
                {
                    if (this.Quantifier == Inventory.Uses.Quantifier.Manual)
                    {
                        return this.manualAmount;
                    }
                    return -1;
                }
            }
            public static Inventory.Uses.Quantity Manual(int amount)
            {
                return new Inventory.Uses.Quantity(Inventory.Uses.Quantifier.Manual, (byte) amount);
            }

            public int CalculateCount(ItemDataBlock datablock)
            {
                switch (this.Quantifier)
                {
                    case Inventory.Uses.Quantifier.Default:
                        return (datablock._spawnUsesMin + ((datablock._spawnUsesMax - datablock._spawnUsesMin) / 2));

                    case Inventory.Uses.Quantifier.Manual:
                        return ((this.manualAmount != 0) ? ((this.manualAmount <= datablock._maxUses) ? this.manualAmount : datablock._maxUses) : 1);

                    case Inventory.Uses.Quantifier.Minimum:
                        return datablock._spawnUsesMin;

                    case Inventory.Uses.Quantifier.Maximum:
                        return datablock._spawnUsesMax;

                    case Inventory.Uses.Quantifier.StackSize:
                        return datablock._maxUses;

                    case Inventory.Uses.Quantifier.Random:
                        return UnityEngine.Random.Range(datablock._spawnUsesMin, datablock._spawnUsesMax + 1);
                }
                throw new NotImplementedException();
            }

            public override string ToString()
            {
                if (this.Quantifier == Inventory.Uses.Quantifier.Manual)
                {
                    return this.manualAmount.ToString();
                }
                return this.Quantifier.ToString();
            }

            public static bool TryParse(string text, out Inventory.Uses.Quantity uses)
            {
                int num;
                bool flag;
                if (int.TryParse(text, out num))
                {
                    if (num == 0)
                    {
                        uses = Random;
                    }
                    else if (num < 0)
                    {
                        uses = Minimum;
                    }
                    else if (num > 0xff)
                    {
                        uses = Maximum;
                    }
                    else
                    {
                        uses = num;
                    }
                    return true;
                }
                if (string.Equals(text, "min", StringComparison.InvariantCultureIgnoreCase))
                {
                    uses = Minimum;
                    return true;
                }
                if (string.Equals(text, "max", StringComparison.InvariantCultureIgnoreCase))
                {
                    uses = Maximum;
                    return true;
                }
                try
                {
                    switch (((Inventory.Uses.Quantifier) ((byte) Enum.Parse(typeof(Inventory.Uses.Quantifier), text, true))))
                    {
                        case Inventory.Uses.Quantifier.Default:
                            uses = Default;
                            return true;

                        case Inventory.Uses.Quantifier.Minimum:
                            uses = Minimum;
                            return true;

                        case Inventory.Uses.Quantifier.Maximum:
                            uses = Maximum;
                            return true;

                        case Inventory.Uses.Quantifier.Random:
                            uses = Random;
                            return true;
                    }
                    throw new NotImplementedException();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    uses = Default;
                    flag = false;
                }
                return flag;
            }

            public static implicit operator Inventory.Uses.Quantity(int amount)
            {
                return Manual(amount);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VacantIterator : IDisposable
    {
        private Inventory.Collection<InventoryItem>.VacantCollection.Enumerator baseEnumerator;
        public VacantIterator(Inventory inventory)
        {
            this.baseEnumerator = inventory.collection.VacantEnumerator;
        }

        public void Reset()
        {
            this.baseEnumerator.Reset();
        }

        public int slot
        {
            get
            {
                return this.baseEnumerator.Current;
            }
        }
        public bool Next()
        {
            return this.baseEnumerator.MoveNext();
        }

        public void Dispose()
        {
            this.baseEnumerator.Dispose();
        }

        public bool Next(out int slot)
        {
            if (this.Next())
            {
                slot = this.baseEnumerator.Current;
                return true;
            }
            slot = -1;
            return false;
        }
    }
}

