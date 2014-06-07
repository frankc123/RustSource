using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class TraitMap<Key, Implementation> : TraitMap<Key> where Key: TraitKey where Implementation: TraitMap<Key, Implementation>
{
    private static bool anyRegistry;
    [SerializeField, HideInInspector]
    private Implementation B;

    protected TraitMap()
    {
    }

    internal sealed override void BindToRegistry()
    {
        LookupRegister<Key, Implementation>.Add((Implementation) this);
    }

    public static Implementation ByName(string name)
    {
        Implementation local;
        return ((!TraitMap<Key, Implementation>.anyRegistry || !LookupRegister<Key, Implementation>.dict.TryGetValue(name, out local)) ? null : local);
    }

    public static bool ByName(string name, out Implementation map)
    {
        if (!TraitMap<Key, Implementation>.anyRegistry)
        {
            map = null;
            return false;
        }
        return LookupRegister<Key, Implementation>.dict.TryGetValue(name, out map);
    }

    internal sealed override TraitMap<Key> __baseMap
    {
        get
        {
            return this.B;
        }
    }

    public static ICollection<Implementation> AllRegistered
    {
        get
        {
            if (!TraitMap<Key, Implementation>.anyRegistry)
            {
                return new Implementation[0];
            }
            return LookupRegister<Key, Implementation>.dict.Values;
        }
    }

    public static bool AnyRegistered
    {
        get
        {
            return TraitMap<Key, Implementation>.anyRegistry;
        }
    }

    private static class LookupRegister
    {
        public static readonly Dictionary<string, Implementation> dict;

        static LookupRegister()
        {
            TraitMap<Key, Implementation>.LookupRegister.dict = new Dictionary<string, Implementation>(StringComparer.InvariantCultureIgnoreCase);
            TraitMap<Key, Implementation>.anyRegistry = true;
        }

        public static void Add(Implementation implementation)
        {
            TraitMap<Key, Implementation>.LookupRegister.dict[implementation.name] = implementation;
        }
    }
}

