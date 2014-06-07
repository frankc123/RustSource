using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public sealed class Loadout : ScriptableObject
{
    [SerializeField]
    private Entry[] _belt;
    [NonSerialized]
    private Inventory.Addition[] _blankInventoryLoadout;
    [SerializeField]
    private BlueprintDataBlock[] _defaultBlueprints;
    [SerializeField]
    private Entry[] _inventory;
    [NonSerialized]
    private Entry[] _minimumRequirements;
    [SerializeField]
    private Entry[] _wearable;

    [DebuggerHidden]
    private static IEnumerable<Inventory.Addition> EnumerateAdditions(Entry[][] arrays)
    {
        return new <EnumerateAdditions>c__Iterator3E { arrays = arrays, <$>arrays = arrays, $PC = -2 };
    }

    [DebuggerHidden]
    private static IEnumerable<Entry> EnumerateRequired(Entry[][] arrays)
    {
        return new <EnumerateRequired>c__Iterator3F { arrays = arrays, <$>arrays = arrays, $PC = -2 };
    }

    private void GetAdditionArray(ref Inventory.Addition[] array, bool forceUpdate)
    {
        if (forceUpdate || (array == null))
        {
            array = new List<Inventory.Addition>(EnumerateAdditions(this.GetEntryArrays())).ToArray();
        }
    }

    private Entry[][] GetEntryArrays()
    {
        return new Entry[][] { LoadEntryArray(this._inventory, Inventory.Slot.Kind.Default), LoadEntryArray(this._belt, Inventory.Slot.Kind.Belt), LoadEntryArray(this._wearable, Inventory.Slot.Kind.Armor) };
    }

    private void GetMinimumRequirementArray(ref Entry[] array, bool forceUpdate)
    {
        if (forceUpdate || (array == null))
        {
            array = new List<Entry>(EnumerateRequired(this.GetEntryArrays())).ToArray();
        }
    }

    private static Entry[] LoadEntryArray(Entry[] array, Inventory.Slot.Kind kind)
    {
        if (array == null)
        {
        }
        array = Empty.EntryArray;
        for (int i = 0; i < array.Length; i++)
        {
            Entry entry = array[i];
            entry.inferredSlotKind = kind;
            entry.inferredSlotOfKind = i;
        }
        return array;
    }

    public BlueprintDataBlock[] defaultBlueprints
    {
        get
        {
            if (this._defaultBlueprints == null)
            {
            }
            return Empty.BlueprintArray;
        }
    }

    private Inventory.Addition[] emptyInventoryAdditions
    {
        get
        {
            this.GetAdditionArray(ref this._blankInventoryLoadout, false);
            return this._blankInventoryLoadout;
        }
    }

    private Entry[] minimumRequirements
    {
        get
        {
            this.GetMinimumRequirementArray(ref this._minimumRequirements, false);
            return this._minimumRequirements;
        }
    }

    [CompilerGenerated]
    private sealed class <EnumerateAdditions>c__Iterator3E : IDisposable, IEnumerator, IEnumerable, IEnumerable<Inventory.Addition>, IEnumerator<Inventory.Addition>
    {
        internal Inventory.Addition $current;
        internal int $PC;
        internal Loadout.Entry[][] <$>arrays;
        internal Loadout.Entry[][] <$s_477>__0;
        internal int <$s_478>__1;
        internal Loadout.Entry[] <$s_479>__3;
        internal int <$s_480>__4;
        internal Loadout.Entry[] <array>__2;
        internal Inventory.Addition <current>__6;
        internal Loadout.Entry <entry>__5;
        internal Loadout.Entry[][] arrays;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<$s_477>__0 = this.arrays;
                    this.<$s_478>__1 = 0;
                    while (this.<$s_478>__1 < this.<$s_477>__0.Length)
                    {
                        this.<array>__2 = this.<$s_477>__0[this.<$s_478>__1];
                        this.<$s_479>__3 = this.<array>__2;
                        this.<$s_480>__4 = 0;
                        while (this.<$s_480>__4 < this.<$s_479>__3.Length)
                        {
                            this.<entry>__5 = this.<$s_479>__3[this.<$s_480>__4];
                            if (this.<entry>__5.GetInventoryAddition(out this.<current>__6))
                            {
                                this.$current = this.<current>__6;
                                this.$PC = 1;
                                return true;
                            }
                        Label_00A5:
                            this.<$s_480>__4++;
                        }
                        this.<$s_478>__1++;
                    }
                    this.$PC = -1;
                    break;

                case 1:
                    goto Label_00A5;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Inventory.Addition> IEnumerable<Inventory.Addition>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Loadout.<EnumerateAdditions>c__Iterator3E { arrays = this.<$>arrays };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Inventory.Addition>.GetEnumerator();
        }

        Inventory.Addition IEnumerator<Inventory.Addition>.Current
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

    [CompilerGenerated]
    private sealed class <EnumerateRequired>c__Iterator3F : IDisposable, IEnumerator, IEnumerable, IEnumerable<Loadout.Entry>, IEnumerator<Loadout.Entry>
    {
        internal Loadout.Entry $current;
        internal int $PC;
        internal Loadout.Entry[][] <$>arrays;
        internal Loadout.Entry[][] <$s_481>__0;
        internal int <$s_482>__1;
        internal Loadout.Entry[] <$s_483>__3;
        internal int <$s_484>__4;
        internal Loadout.Entry[] <array>__2;
        internal Loadout.Entry <entry>__5;
        internal Loadout.Entry[][] arrays;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<$s_481>__0 = this.arrays;
                    this.<$s_482>__1 = 0;
                    while (this.<$s_482>__1 < this.<$s_481>__0.Length)
                    {
                        this.<array>__2 = this.<$s_481>__0[this.<$s_482>__1];
                        this.<$s_483>__3 = this.<array>__2;
                        this.<$s_484>__4 = 0;
                        while (this.<$s_484>__4 < this.<$s_483>__3.Length)
                        {
                            this.<entry>__5 = this.<$s_483>__3[this.<$s_484>__4];
                            if (this.<entry>__5.minimumRequirement)
                            {
                                this.$current = this.<entry>__5;
                                this.$PC = 1;
                                return true;
                            }
                        Label_009F:
                            this.<$s_484>__4++;
                        }
                        this.<$s_482>__1++;
                    }
                    this.$PC = -1;
                    break;

                case 1:
                    goto Label_009F;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        IEnumerator<Loadout.Entry> IEnumerable<Loadout.Entry>.GetEnumerator()
        {
            if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
            {
                return this;
            }
            return new Loadout.<EnumerateRequired>c__Iterator3F { arrays = this.<$>arrays };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<Loadout.Entry>.GetEnumerator();
        }

        Loadout.Entry IEnumerator<Loadout.Entry>.Current
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

    private static class Empty
    {
        public static readonly BlueprintDataBlock[] BlueprintArray = new BlueprintDataBlock[0];
        public static readonly Loadout.Entry[] EntryArray = new Loadout.Entry[0];
    }

    [Serializable]
    private class Entry
    {
        [SerializeField]
        private bool _minimumRequirement;
        [SerializeField]
        private int _useCount;
        [SerializeField]
        private bool enabled;
        [NonSerialized]
        internal Inventory.Slot.Kind inferredSlotKind;
        [NonSerialized]
        internal int inferredSlotOfKind;
        public ItemDataBlock item;

        public bool GetInventoryAddition(out Inventory.Addition addition)
        {
            if (this.allowed)
            {
                addition = new Inventory.Addition();
                Inventory.Addition addition2 = addition;
                addition2.Ident = (Datablock.Ident) this.item;
                addition2.SlotPreference = Inventory.Slot.Preference.Define(this.inferredSlotKind, this.inferredSlotOfKind);
                addition2.UsesQuantity = this.useCount;
                addition = addition2;
                return true;
            }
            addition = new Inventory.Addition();
            return false;
        }

        public bool allowed
        {
            get
            {
                return ((this.enabled && (this.item != null)) && (!this.item.IsSplittable() || (this._useCount > 0)));
            }
        }

        public bool forEmptyInventories
        {
            get
            {
                return (!this._minimumRequirement && this.allowed);
            }
        }

        public bool minimumRequirement
        {
            get
            {
                return (this._minimumRequirement && this.allowed);
            }
        }

        public int useCount
        {
            get
            {
                return (!this.allowed ? 0 : ((this.item._maxUses >= this._useCount) ? ((byte) this._useCount) : this.item._maxUses));
            }
        }
    }
}

