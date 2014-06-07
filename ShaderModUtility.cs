using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class ShaderModUtility
{
    public static IEnumerable<ShaderMod.KV> MergeKeyValues(this ShaderMod[] mods, ShaderMod.Replacement replacement)
    {
        return null;
    }

    public static int Replace(this ShaderMod[] mods, ShaderMod.Replacement replacement, string incoming, ref string outgoing)
    {
        if (mods != null)
        {
            int length = mods.Length;
            for (int i = 0; i < length; i++)
            {
                if ((mods[i] != null) && mods[i].Replace(replacement, incoming, ref outgoing))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public static int ReplaceReverse(this ShaderMod[] mods, ShaderMod.Replacement replacement, string incoming, ref string outgoing)
    {
        if (mods != null)
        {
            int length = mods.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                if ((mods[i] != null) && mods[i].Replace(replacement, incoming, ref outgoing))
                {
                    return i;
                }
            }
        }
        return -1;
    }
}

