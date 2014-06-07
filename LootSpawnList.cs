using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class LootSpawnList : ScriptableObject
{
    public LootWeightedEntry[] LootPackages;
    public int maxPackagesToSpawn = 1;
    public int minPackagesToSpawn = 1;
    public bool noDuplicates;
    public bool spawnOneOfEach;

    public void PopulateInventory(Inventory inven)
    {
        RecursiveInventoryPopulateArgs args;
        args.inventory = inven;
        args.spawnCount = 0;
        args.inventoryExausted = inven.noVacantSlots;
        if (!args.inventoryExausted)
        {
            this.PopulateInventory_Recurse(ref args);
        }
    }

    private void PopulateInventory_Recurse(ref RecursiveInventoryPopulateArgs args)
    {
        if (this.maxPackagesToSpawn > this.LootPackages.Length)
        {
            this.maxPackagesToSpawn = this.LootPackages.Length;
        }
        int length = 0;
        if (this.spawnOneOfEach)
        {
            length = this.LootPackages.Length;
        }
        else
        {
            length = Random.Range(this.minPackagesToSpawn, this.maxPackagesToSpawn);
        }
        for (int i = 0; !args.inventoryExausted && (i < length); i++)
        {
            LootWeightedEntry entry = null;
            if (this.spawnOneOfEach)
            {
                entry = this.LootPackages[i];
            }
            else
            {
                entry = WeightSelection.RandomPickEntry(this.LootPackages) as LootWeightedEntry;
            }
            if (entry == null)
            {
                Debug.Log("Massive fuckup...");
                return;
            }
            Object obj2 = entry.obj;
            if (obj2 != null)
            {
                if (obj2 is ItemDataBlock)
                {
                    ItemDataBlock datablock = obj2 as ItemDataBlock;
                    if (!object.ReferenceEquals(args.inventory.AddItem(datablock, Inventory.Slot.Preference.Define(args.spawnCount, false, Inventory.Slot.KindFlags.Belt | Inventory.Slot.KindFlags.Default), Random.Range(entry.amountMin, entry.amountMax + 1)), null))
                    {
                        args.spawnCount++;
                        if (args.inventory.noVacantSlots)
                        {
                            args.inventoryExausted = true;
                        }
                    }
                }
                else if (obj2 is LootSpawnList)
                {
                    ((LootSpawnList) obj2).PopulateInventory_Recurse(ref args);
                }
            }
        }
    }

    [Serializable]
    public class LootWeightedEntry : WeightSelection.WeightedEntry
    {
        public int amountMax = 1;
        public int amountMin;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RecursiveInventoryPopulateArgs
    {
        public Inventory inventory;
        public int spawnCount;
        public bool inventoryExausted;
    }
}

