using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class ItemModDataBlock : ItemDataBlock
{
    private readonly Type minimumModRepresentationType;
    public ItemModFlags modFlag;
    [SerializeField]
    private string modRepresentationTypeName;
    public AudioClip offSound;
    public AudioClip onSound;

    public ItemModDataBlock() : this(typeof(ItemModRepresentation))
    {
    }

    protected ItemModDataBlock(Type minimumModRepresentationType)
    {
        this.modRepresentationTypeName = "ItemModRepresentation";
        if (!typeof(ItemModRepresentation).IsAssignableFrom(minimumModRepresentationType))
        {
            throw new ArgumentOutOfRangeException("minimumModRepresentationType", minimumModRepresentationType, "!typeof(ItemModRepresentation).IsAssignableFrom(minimumModRepresentationType)");
        }
        this.minimumModRepresentationType = minimumModRepresentationType;
    }

    internal bool AddModRepresentationComponent(GameObject gameObject, out ItemModRepresentation rep)
    {
        if (this.hasModRepresentation)
        {
            g.TypePair pair;
            if (!g.cachedTypeLookup.TryGetValue(base.name, out pair) || (pair.typeString != this.modRepresentationTypeName))
            {
                pair = new g.TypePair {
                    typeString = this.modRepresentationTypeName
                };
                pair.type = Types.GetType(pair.typeString, "Assembly-CSharp");
                if (pair.type == null)
                {
                    Debug.LogError(string.Format("modRepresentationTypeName:{0} resolves to no type", pair.typeString), this);
                }
                else if (!this.minimumModRepresentationType.IsAssignableFrom(pair.type))
                {
                    Debug.LogError(string.Format("modRepresentationTypeName:{0} resolved to {1} but {1} is not a {2}", pair.typeString, pair.type, this.minimumModRepresentationType), this);
                    pair.type = null;
                }
                g.cachedTypeLookup[base.name] = pair;
            }
            if (pair.type != null)
            {
                rep = (ItemModRepresentation) gameObject.AddComponent(pair.type);
                if (rep != null)
                {
                    this.CustomizeItemModRepresentation(rep);
                    if (rep != null)
                    {
                        return true;
                    }
                }
            }
        }
        rep = null;
        return false;
    }

    internal void BindAsLocal(ref ModViewModelAddArgs a)
    {
        this.InstallToViewModel(ref a);
    }

    internal void BindAsProxy(ItemModRepresentation rep)
    {
        this.InstallToItemModRepresentation(rep);
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    protected virtual void CustomizeItemModRepresentation(ItemModRepresentation rep)
    {
    }

    protected virtual void InstallToItemModRepresentation(ItemModRepresentation rep)
    {
    }

    protected virtual bool InstallToViewModel(ref ModViewModelAddArgs a)
    {
        return false;
    }

    protected void OnDestroy()
    {
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<ItemModFlags>(this.modFlag, new object[0]);
        stream.Write<string>(this.modRepresentationTypeName, new object[0]);
    }

    internal void UnBindAsLocal(ref ModViewModelRemoveArgs a)
    {
        this.UninstallFromViewModel(ref a);
    }

    internal void UnBindAsProxy(ItemModRepresentation rep)
    {
        this.UninstallFromItemModRepresentation(rep);
    }

    protected virtual void UninstallFromItemModRepresentation(ItemModRepresentation rep)
    {
    }

    protected virtual void UninstallFromViewModel(ref ModViewModelRemoveArgs a)
    {
    }

    public bool hasModRepresentation
    {
        get
        {
            return !string.IsNullOrEmpty(this.modRepresentationTypeName);
        }
    }

    private static class g
    {
        public static Dictionary<string, TypePair> cachedTypeLookup = new Dictionary<string, TypePair>();

        public class TypePair
        {
            public Type type;
            public string typeString;
        }
    }

    private sealed class ITEM_TYPE : ItemModItem<ItemModDataBlock>, IInventoryItem, IItemModItem
    {
        public ITEM_TYPE(ItemModDataBlock BLOCK) : base(BLOCK)
        {
        }

        int IInventoryItem.AddUses(int count)
        {
            return base.AddUses(count);
        }

        bool IInventoryItem.Consume(ref int count)
        {
            return base.Consume(ref count);
        }

        void IInventoryItem.Deserialize(BitStream stream)
        {
            base.Deserialize(stream);
        }

        bool IInventoryItem.get_active()
        {
            return base.active;
        }

        Character IInventoryItem.get_character()
        {
            return base.character;
        }

        float IInventoryItem.get_condition()
        {
            return base.condition;
        }

        Controllable IInventoryItem.get_controllable()
        {
            return base.controllable;
        }

        Controller IInventoryItem.get_controller()
        {
            return base.controller;
        }

        bool IInventoryItem.get_dirty()
        {
            return base.dirty;
        }

        bool IInventoryItem.get_doNotSave()
        {
            return base.doNotSave;
        }

        IDMain IInventoryItem.get_idMain()
        {
            return base.idMain;
        }

        Inventory IInventoryItem.get_inventory()
        {
            return base.inventory;
        }

        bool IInventoryItem.get_isInLocalInventory()
        {
            return base.isInLocalInventory;
        }

        float IInventoryItem.get_lastUseTime()
        {
            return base.lastUseTime;
        }

        float IInventoryItem.get_maxcondition()
        {
            return base.maxcondition;
        }

        int IInventoryItem.get_slot()
        {
            return base.slot;
        }

        int IInventoryItem.get_uses()
        {
            return base.uses;
        }

        float IInventoryItem.GetConditionPercent()
        {
            return base.GetConditionPercent();
        }

        bool IInventoryItem.IsBroken()
        {
            return base.IsBroken();
        }

        bool IInventoryItem.IsDamaged()
        {
            return base.IsDamaged();
        }

        bool IInventoryItem.MarkDirty()
        {
            return base.MarkDirty();
        }

        void IInventoryItem.Serialize(BitStream stream)
        {
            base.Serialize(stream);
        }

        void IInventoryItem.set_lastUseTime(float value)
        {
            base.lastUseTime = value;
        }

        void IInventoryItem.SetCondition(float condition)
        {
            base.SetCondition(condition);
        }

        void IInventoryItem.SetMaxCondition(float condition)
        {
            base.SetMaxCondition(condition);
        }

        void IInventoryItem.SetUses(int count)
        {
            base.SetUses(count);
        }

        bool IInventoryItem.TryConditionLoss(float probability, float percentLoss)
        {
            return base.TryConditionLoss(probability, percentLoss);
        }

        ItemDataBlock IInventoryItem.datablock
        {
            get
            {
                return base.datablock;
            }
        }
    }
}

