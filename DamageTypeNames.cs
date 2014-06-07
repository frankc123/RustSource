using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class DamageTypeNames
{
    private static readonly string[] Flags;
    private static readonly DamageTypeFlags Mask;
    private static readonly char[] SplitCharacters = new char[] { '|' };
    private static readonly string[] Strings = new string[6];
    private static readonly Dictionary<string, DamageTypeIndex> Values = new Dictionary<string, DamageTypeIndex>(6);

    static DamageTypeNames()
    {
        for (DamageTypeIndex index = DamageTypeIndex.damage_generic; index < DamageTypeIndex.damage_last; index += 1)
        {
            string str2;
            Strings[(int) index] = str2 = index.ToString().Substring("damage_".Length);
            Values.Add(str2, index);
        }
        uint num = 0x3f;
        Mask = (DamageTypeFlags) num;
        Flags = new string[num];
        Flags[0] = "none";
        for (uint i = 1; i < num; i++)
        {
            uint num3 = i;
            for (int j = 0; j < 6; j++)
            {
                if ((i & (((int) 1) << j)) == (((int) 1) << j))
                {
                    string str = Strings[j];
                    num3 &= (uint) ~(((int) 1) << j);
                    if (num3 != 0)
                    {
                        while (++j < 6L)
                        {
                            if ((i & (((int) 1) << j)) == (((int) 1) << j))
                            {
                                str = str + "|" + Strings[j];
                                num3 &= (uint) ~(((int) 1) << j);
                                if (num3 == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    public static bool Convert(string[] names, out DamageTypeFlags flags)
    {
        for (int i = 0; i < names.Length; i++)
        {
            DamageTypeIndex index;
            if (Values.TryGetValue(names[i], out index))
            {
                flags = (DamageTypeFlags) (((int) 1) << index);
                while (++i < names.Length)
                {
                    if (Values.TryGetValue(names[i], out index))
                    {
                        flags |= ((int) 1) << index;
                    }
                }
                return true;
            }
        }
        flags = 0;
        return false;
    }

    public static bool Convert(DamageTypeIndex index, out DamageTypeFlags flags)
    {
        flags = (DamageTypeFlags) (((int) 1) << index);
        return ((flags & Mask) == flags);
    }

    public static bool Convert(DamageTypeIndex index, out string name)
    {
        if ((index == DamageTypeIndex.damage_generic) || ((index > DamageTypeIndex.damage_generic) && (index < DamageTypeIndex.damage_last)))
        {
            name = Strings[(int) index];
            return true;
        }
        name = null;
        return false;
    }

    public static bool Convert(string name, out DamageTypeFlags flags)
    {
        DamageTypeIndex index;
        if (Values.TryGetValue(name, out index))
        {
            flags = (DamageTypeFlags) (((int) 1) << index);
            return true;
        }
        if ((name.Length != 0) && !(name == "none"))
        {
            return Convert(name.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries), out flags);
        }
        flags = 0;
        return true;
    }

    public static bool Convert(string name, out DamageTypeIndex index)
    {
        return Values.TryGetValue(name, out index);
    }
}

