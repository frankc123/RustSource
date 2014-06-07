using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DatablockDictionary
{
    private static ItemDataBlock[] _all;
    private static Dictionary<string, int> _dataBlocks;
    private static Dictionary<int, int> _dataBlocksByUniqueID;
    public static Dictionary<string, LootSpawnList> _lootSpawnLists;
    private const int expectedDBListLength = 14;
    private static bool initializedAtLeastOnce;

    public static TArmorModel GetArmorModelByUniqueID<TArmorModel>(int uniqueID) where TArmorModel: ArmorModel, new()
    {
        ArmorDataBlock byUniqueID = GetByUniqueID(uniqueID) as ArmorDataBlock;
        if (byUniqueID == null)
        {
            return null;
        }
        return byUniqueID.GetArmorModel<TArmorModel>();
    }

    public static ArmorModel GetArmorModelByUniqueID(int uniqueID, ArmorModelSlot slot)
    {
        ArmorDataBlock byUniqueID = GetByUniqueID(uniqueID) as ArmorDataBlock;
        if (byUniqueID == null)
        {
            return null;
        }
        return byUniqueID.GetArmorModel(slot);
    }

    public static ItemDataBlock GetByName(string name)
    {
        int num;
        if (!_dataBlocks.TryGetValue(name, out num))
        {
            return null;
        }
        return _all[num];
    }

    public static ItemDataBlock GetByUniqueID(int uniqueID)
    {
        int num;
        if (!_dataBlocksByUniqueID.TryGetValue(uniqueID, out num))
        {
            return null;
        }
        return _all[num];
    }

    public static LootSpawnList GetLootSpawnListByName(string name)
    {
        LootSpawnList list;
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        if (!_lootSpawnLists.TryGetValue(name, out list))
        {
            Debug.LogError("Theres no loot spawn list with name " + name);
        }
        return list;
    }

    public static void Initialize()
    {
        _dataBlocks = new Dictionary<string, int>();
        _dataBlocksByUniqueID = new Dictionary<int, int>();
        _lootSpawnLists = new Dictionary<string, LootSpawnList>();
        List<ItemDataBlock> list = new List<ItemDataBlock>();
        HashSet<ItemDataBlock> set = new HashSet<ItemDataBlock>();
        foreach (ItemDataBlock block in Bundling.LoadAll<ItemDataBlock>())
        {
            if (set.Add(block))
            {
                int count = list.Count;
                _dataBlocks.Add(block.name, count);
                _dataBlocksByUniqueID.Add(block.uniqueID, count);
                list.Add(block);
            }
        }
        _all = list.ToArray();
        foreach (LootSpawnList list2 in Bundling.LoadAll<LootSpawnList>())
        {
            _lootSpawnLists.Add(list2.name, list2);
        }
        initializedAtLeastOnce = true;
    }

    public static void TryInitialize()
    {
        if (!initializedAtLeastOnce)
        {
            Initialize();
        }
    }

    public static ItemDataBlock[] All
    {
        get
        {
            return _all;
        }
    }
}

