using System;
using uLink;
using UnityEngine;

public class StructureComponentDataBlock : HeldItemDataBlock
{
    [NonSerialized]
    private bool _loadedStructureToPlace;
    [NonSerialized]
    private StructureComponent _structureToPlace;
    public Material overrideMat;
    public string structureToPlaceName;

    public bool CheckBlockers(Vector3 pos)
    {
        if (this._structureToPlace.type == StructureComponent.StructureComponentType.Foundation)
        {
            foreach (Collider collider in Physics.OverlapSphere(pos, 12f, 0x10360401))
            {
                IDMain main = IDBase.GetMain(collider.gameObject);
                if (main != null)
                {
                    float num2 = TransformHelpers.Dist2D(main.transform.position, pos);
                    if ((main.GetComponent<SpikeWall>() != null) && (num2 < 5f))
                    {
                        return false;
                    }
                }
            }
        }
        if (!NoPlacementZone.ValidPos(pos))
        {
            return false;
        }
        return true;
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
    }

    public bool MasterFromRay(Ray ray)
    {
        foreach (StructureMaster master in StructureMaster.RayTestStructures(ray))
        {
            if (master != null)
            {
                return true;
            }
        }
        return false;
    }

    public StructureComponent structureToPlacePrefab
    {
        get
        {
            if (!this._loadedStructureToPlace && Application.isPlaying)
            {
                NetCull.LoadPrefabScript<StructureComponent>(this.structureToPlaceName, out this._structureToPlace);
                this._loadedStructureToPlace = true;
            }
            return this._structureToPlace;
        }
    }

    private sealed class ITEM_TYPE : StructureComponentItem<StructureComponentDataBlock>, IHeldItem, IInventoryItem, IStructureComponentItem
    {
        public ITEM_TYPE(StructureComponentDataBlock BLOCK) : base(BLOCK)
        {
        }

        void IHeldItem.AddMod(ItemModDataBlock mod)
        {
            base.AddMod(mod);
        }

        int IHeldItem.FindMod(ItemModDataBlock mod)
        {
            return base.FindMod(mod);
        }

        bool IHeldItem.get_canActivate()
        {
            return base.canActivate;
        }

        bool IHeldItem.get_canDeactivate()
        {
            return base.canDeactivate;
        }

        int IHeldItem.get_freeModSlots()
        {
            return base.freeModSlots;
        }

        ItemModDataBlock[] IHeldItem.get_itemMods()
        {
            return base.itemMods;
        }

        ItemRepresentation IHeldItem.get_itemRepresentation()
        {
            return base.itemRepresentation;
        }

        ItemModFlags IHeldItem.get_modFlags()
        {
            return base.modFlags;
        }

        int IHeldItem.get_totalModSlots()
        {
            return base.totalModSlots;
        }

        int IHeldItem.get_usedModSlots()
        {
            return base.usedModSlots;
        }

        ViewModel IHeldItem.get_viewModelInstance()
        {
            return base.viewModelInstance;
        }

        void IHeldItem.OnActivate()
        {
            base.OnActivate();
        }

        void IHeldItem.OnDeactivate()
        {
            base.OnDeactivate();
        }

        void IHeldItem.set_itemRepresentation(ItemRepresentation value)
        {
            base.itemRepresentation = value;
        }

        void IHeldItem.SetTotalModSlotCount(int count)
        {
            base.SetTotalModSlotCount(count);
        }

        void IHeldItem.SetUsedModSlotCount(int count)
        {
            base.SetUsedModSlotCount(count);
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

