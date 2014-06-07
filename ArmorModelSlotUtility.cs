using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class ArmorModelSlotUtility
{
    public const ArmorModelSlotMask All = (ArmorModelSlotMask.Head | ArmorModelSlotMask.Torso | ArmorModelSlotMask.Legs | ArmorModelSlotMask.Feet);
    public const ArmorModelSlot Begin = ArmorModelSlot.Feet;
    public const int Count = 4;
    public const ArmorModelSlot End = ((ArmorModelSlot) 4);
    public const ArmorModelSlot First = ArmorModelSlot.Feet;
    public const ArmorModelSlot Last = ArmorModelSlot.Head;
    public const ArmorModelSlotMask None = 0;

    public static bool Contains(this ArmorModelSlot slot, ArmorModelSlotMask slotMask)
    {
        return ((slot < ((ArmorModelSlot) 4)) && ((slotMask & (((int) 1) << slot)) != 0));
    }

    public static bool Contains(this ArmorModelSlotMask slotMask, ArmorModelSlot slot)
    {
        return ((slot < ((ArmorModelSlot) 4)) && ((slotMask & (((int) 1) << slot)) != 0));
    }

    public static ArmorModelSlot[] EnumerateSlots(this ArmorModelSlotMask slotMask)
    {
        return Mask2SlotArray.FlagToSlotArray[((int) slotMask) & 15];
    }

    public static ArmorModelSlot GetArmorModelSlotForClass<T>() where T: ArmorModel, new()
    {
        return ClassToArmorModelSlot<T>.ArmorModelSlot;
    }

    public static Type GetArmorModelType(this ArmorModelSlot slot)
    {
        return ((slot >= ((ArmorModelSlot) 4)) ? null : ClassToArmorModelSlot.ArmorModelSlotToType[slot]);
    }

    public static int GetMaskedSlotCount(this ArmorModelSlot slot)
    {
        return ((slot >= ((ArmorModelSlot) 4)) ? 0 : 1);
    }

    public static int GetMaskedSlotCount(this ArmorModelSlotMask slotMask)
    {
        uint num = (uint) (slotMask & (ArmorModelSlotMask.Head | ArmorModelSlotMask.Torso | ArmorModelSlotMask.Legs | ArmorModelSlotMask.Feet));
        int num2 = 0;
        while (num != 0)
        {
            num2++;
            num &= num - 1;
        }
        return num2;
    }

    public static string GetRendererName(this ArmorModelSlot slot)
    {
        return ((slot >= ((ArmorModelSlot) 4)) ? "Armor Renderer" : RendererNames.Array[(int) slot]);
    }

    public static int GetUnmaskedSlotCount(this ArmorModelSlot slot)
    {
        return ((slot >= ((ArmorModelSlot) 4)) ? 4 : 3);
    }

    public static int GetUnmaskedSlotCount(this ArmorModelSlotMask slotMask)
    {
        uint num = (uint) (~slotMask & (ArmorModelSlotMask.Head | ArmorModelSlotMask.Torso | ArmorModelSlotMask.Legs | ArmorModelSlotMask.Feet));
        int num2 = 0;
        while (num != 0)
        {
            num2++;
            num &= num - 1;
        }
        return num2;
    }

    public static ArmorModelSlot[] ToArray(this ArmorModelSlotMask slotMask)
    {
        ArmorModelSlot[] slotArray = Mask2SlotArray.FlagToSlotArray[((int) slotMask) & 15];
        ArmorModelSlot[] slotArray2 = new ArmorModelSlot[slotArray.Length];
        for (int i = 0; i < slotArray.Length; i++)
        {
            slotArray2[i] = slotArray[i];
        }
        return slotArray2;
    }

    public static ArmorModelSlotMask ToMask(this ArmorModelSlot slot)
    {
        return (((ArmorModelSlotMask) (((int) 1) << slot)) & (ArmorModelSlotMask.Head | ArmorModelSlotMask.Torso | ArmorModelSlotMask.Legs | ArmorModelSlotMask.Feet));
    }

    public static ArmorModelSlotMask ToNotMask(this ArmorModelSlot slot)
    {
        return (((ArmorModelSlotMask) ~(((int) 1) << slot)) & (ArmorModelSlotMask.Head | ArmorModelSlotMask.Torso | ArmorModelSlotMask.Legs | ArmorModelSlotMask.Feet));
    }

    private static class ClassToArmorModelSlot
    {
        public static readonly Dictionary<ArmorModelSlot, Type> ArmorModelSlotToType;

        static ClassToArmorModelSlot()
        {
            List<Type> list = new List<Type>();
            foreach (Type type in typeof(ArmorModelSlotUtility.ClassToArmorModelSlot).Assembly.GetTypes())
            {
                if ((type.IsSubclassOf(typeof(ArmorModel)) && !type.IsAbstract) && type.IsDefined(typeof(ArmorModelSlotClassAttribute), false))
                {
                    list.Add(type);
                }
            }
            ArmorModelSlotToType = new Dictionary<ArmorModelSlot, Type>(list.Count);
            foreach (Type type2 in list)
            {
                ArmorModelSlotClassAttribute customAttribute = (ArmorModelSlotClassAttribute) Attribute.GetCustomAttribute(type2, typeof(ArmorModelSlotClassAttribute));
                ArmorModelSlotToType.Add(customAttribute.ArmorModelSlot, type2);
            }
        }
    }

    private static class ClassToArmorModelSlot<T> where T: ArmorModel, new()
    {
        public static readonly ArmorModelSlot ArmorModelSlot;

        static ClassToArmorModelSlot()
        {
            ArmorModelSlotUtility.ClassToArmorModelSlot<T>.ArmorModelSlot = ((ArmorModelSlotClassAttribute) Attribute.GetCustomAttribute(typeof(T), typeof(ArmorModelSlotClassAttribute))).ArmorModelSlot;
        }
    }

    private static class Mask2SlotArray
    {
        public static readonly ArmorModelSlot[][] FlagToSlotArray = new ArmorModelSlot[0x10][];

        static Mask2SlotArray()
        {
            for (int i = 0; i <= 15; i++)
            {
                int num2 = 0;
                for (int j = 0; j < 4; j++)
                {
                    if ((i & (((int) 1) << j)) == (((int) 1) << j))
                    {
                        num2++;
                    }
                }
                FlagToSlotArray[i] = new ArmorModelSlot[num2];
                int num4 = 0;
                for (int k = 0; k < 4; k++)
                {
                    if ((i & (((int) 1) << k)) == (((int) 1) << k))
                    {
                        FlagToSlotArray[i][num4++] = (ArmorModelSlot) ((byte) k);
                    }
                }
            }
        }
    }

    private static class RendererNames
    {
        public static readonly string[] Array = new string[4];

        static RendererNames()
        {
            for (ArmorModelSlot slot = ArmorModelSlot.Feet; slot < ((ArmorModelSlot) 4); slot = (ArmorModelSlot) (((int) slot) + 1))
            {
                Array[(int) slot] = string.Format("{0} Renderer", slot);
            }
        }
    }
}

