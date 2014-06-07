using System;

public class CampfireInventory : Inventory, IServerSaveable, FixedSizeInventory
{
    protected override bool CheckSlotFlags(Inventory.SlotFlags itemSlotFlags, Inventory.SlotFlags slotFlags)
    {
        return ((itemSlotFlags & slotFlags) != 0);
    }

    protected override unsafe void ConfigureSlots(int totalCount, ref Inventory.Slot.KindDictionary<Inventory.Slot.Range> ranges, ref Inventory.SlotFlags[] flags)
    {
        Inventory.Slot.KindDictionary<Inventory.Slot.Range> dictionary = new Inventory.Slot.KindDictionary<Inventory.Slot.Range>();
        dictionary[Inventory.Slot.Kind.Belt] = new Inventory.Slot.Range(0, 3);
        dictionary[Inventory.Slot.Kind.Default] = new Inventory.Slot.Range(3, totalCount - 3);
        ranges = dictionary;
        Inventory.SlotFlags[] flagsArray = new Inventory.SlotFlags[totalCount];
        for (int i = 0; i < 3; i++)
        {
            *((int*) &(flagsArray[i])) |= 0x400;
        }
        for (int j = 3; j < 6; j++)
        {
            *((int*) &(flagsArray[j])) |= 0x200;
        }
        *((int*) &(flagsArray[6])) |= 0x80;
        *((int*) &(flagsArray[7])) |= 0x100;
        flags = flagsArray;
    }

    public int fixedSlotCount
    {
        get
        {
            return 8;
        }
    }
}

