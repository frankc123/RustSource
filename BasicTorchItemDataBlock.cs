using System;
using uLink;
using UnityEngine;

public class BasicTorchItemDataBlock : HeldItemDataBlock
{
    public GameObject FirstPersonLightPrefab;
    public GameObject ThirdPersonLightPrefab;

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction2(BitStream stream, ItemRepresentation itemRep, ref NetworkMessageInfo info)
    {
        this.Ignite(null, itemRep, null);
    }

    public override void DoAction3(BitStream stream, ItemRepresentation itemRep, ref NetworkMessageInfo info)
    {
        this.Extinguish(itemRep);
    }

    public void DoActualExtinguish(ItemRepresentation itemRep, IBasicTorchItem itemInstance, ViewModel vm)
    {
        if (itemInstance == null)
        {
            Debug.Log("inst null");
        }
        if (itemRep == null)
        {
            Debug.Log("rep null");
        }
        if (vm == null)
        {
            Debug.Log("vm null ");
        }
        itemInstance.Extinguish();
    }

    public void DoActualIgnite(ItemRepresentation itemRep, IBasicTorchItem itemInstance, ViewModel vm)
    {
        this.Ignite(vm, itemRep, itemInstance);
        itemRep.Action(2, RPCMode.Server);
    }

    public void Extinguish(ItemRepresentation itemRep)
    {
        (itemRep as TorchItemRep).RepExtinguish();
    }

    public void Ignite(ViewModel vm, ItemRepresentation itemRep, IBasicTorchItem torchItem)
    {
        if (torchItem != null)
        {
            torchItem.Ignite();
        }
        bool flag = vm != null;
        GameObject obj2 = null;
        if (flag)
        {
            Socket.Slot slot = vm.socketMap["muzzle"];
            obj2 = slot.socket.InstantiateAsChild(this.FirstPersonLightPrefab, false);
            if (torchItem != null)
            {
                torchItem.light = obj2;
            }
        }
        else if (((torchItem == null) || (torchItem.light == null)) && (!itemRep.networkView.isMine || actor.forceThirdPerson))
        {
            if (this.ThirdPersonLightPrefab != null)
            {
                ((BasicTorchItemRep) itemRep)._myLightPrefab = this.ThirdPersonLightPrefab;
            }
            ((BasicTorchItemRep) itemRep).RepIgnite();
            if ((((BasicTorchItemRep) itemRep)._myLight != null) && (torchItem != null))
            {
                torchItem.light = ((BasicTorchItemRep) itemRep)._myLight;
            }
        }
    }

    private sealed class ITEM_TYPE : BasicTorchItem<BasicTorchItemDataBlock>, IBasicTorchItem, IHeldItem, IInventoryItem
    {
        public ITEM_TYPE(BasicTorchItemDataBlock BLOCK) : base(BLOCK)
        {
        }

        bool IBasicTorchItem.get_isLit()
        {
            return base.isLit;
        }

        GameObject IBasicTorchItem.get_light()
        {
            return base.light;
        }

        void IBasicTorchItem.Ignite()
        {
            base.Ignite();
        }

        void IBasicTorchItem.set_isLit(bool value)
        {
            base.isLit = value;
        }

        void IBasicTorchItem.set_light(GameObject value)
        {
            base.light = value;
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

