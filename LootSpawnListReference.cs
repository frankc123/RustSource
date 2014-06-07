using System;
using UnityEngine;

[Serializable]
public class LootSpawnListReference
{
    [NonSerialized]
    private LootSpawnList _list;
    [SerializeField]
    private string name = string.Empty;
    [NonSerialized]
    private bool once;

    public static explicit operator LootSpawnList(LootSpawnListReference reference)
    {
        if (object.ReferenceEquals(reference, null))
        {
            return null;
        }
        return reference.list;
    }

    public static bool operator false(LootSpawnListReference reference)
    {
        return (object.ReferenceEquals(reference, null) || (reference.list == 0));
    }

    public static bool operator true(LootSpawnListReference reference)
    {
        return (!object.ReferenceEquals(reference, null) && ((bool) reference.list));
    }

    public LootSpawnList list
    {
        get
        {
            if (!this.once)
            {
                this.once = true;
                if (this.name == null)
                {
                }
                this._list = DatablockDictionary.GetLootSpawnListByName(string.Empty);
            }
            return this._list;
        }
        set
        {
            this.name = (value == null) ? string.Empty : value.name;
            this._list = value;
            this.once = true;
        }
    }
}

