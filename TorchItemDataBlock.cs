using System;
using uLink;
using UnityEngine;

public class TorchItemDataBlock : ThrowableItemDataBlock
{
    public GameObject FirstPersonLightPrefab;
    public AudioClip StrikeSound;
    public GameObject ThirdPersonLightPrefab;

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
        (rep as TorchItemRep).RepExtinguish();
    }

    public override void DoAction2(BitStream stream, ItemRepresentation itemRep, ref NetworkMessageInfo info)
    {
        this.Ignite(null, itemRep, null);
    }

    public override void DoAction3(BitStream stream, ItemRepresentation itemRep, ref NetworkMessageInfo info)
    {
    }

    public void DoActualIgnite(ItemRepresentation itemRep, IThrowableItem itemInstance, ViewModel vm)
    {
        this.Ignite(vm, itemRep, this.GetTorchInstance(itemInstance));
        itemRep.Action(2, RPCMode.Server);
    }

    public override void DoActualThrow(ItemRepresentation itemRep, IThrowableItem itemInstance, ViewModel vm)
    {
        Character component = PlayerClient.GetLocalPlayer().controllable.GetComponent<Character>();
        Vector3 eyesOrigin = component.eyesOrigin;
        Vector3 forward = component.eyesAngles.forward;
        if (vm != null)
        {
            vm.PlayQueued("deploy");
        }
        this.GetTorchInstance(itemInstance).Extinguish();
        int count = 1;
        if (itemInstance.Consume(ref count))
        {
            itemInstance.inventory.RemoveItem(itemInstance.slot);
        }
        BitStream stream = new BitStream(false);
        stream.WriteVector3(eyesOrigin);
        stream.WriteVector3(forward);
        itemRep.ActionStream(1, RPCMode.Server, stream);
    }

    public ITorchItem GetTorchInstance(IThrowableItem itemInstance)
    {
        return (itemInstance as ITorchItem);
    }

    public TorchItemRep GetTorchRep(ItemRepresentation rep)
    {
        return (rep as TorchItemRep);
    }

    public void Ignite(ViewModel vm, ItemRepresentation itemRep, ITorchItem torchItem)
    {
        if (torchItem != null)
        {
            torchItem.Ignite();
        }
        bool flag = vm != null;
        GameObject obj2 = null;
        if (flag)
        {
            this.StrikeSound.Play();
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
                ((TorchItemRep) itemRep)._myLightPrefab = this.ThirdPersonLightPrefab;
            }
            ((TorchItemRep) itemRep).RepIgnite();
            if ((((TorchItemRep) itemRep)._myLight != null) && (torchItem != null))
            {
                torchItem.light = ((TorchItemRep) itemRep)._myLight;
            }
        }
    }

    public void OnExtinguish(ViewModel vm, ItemRepresentation itemRep, ITorchItem torchItem)
    {
    }

    public override void PrimaryAttack(ViewModel vm, ItemRepresentation itemRep, IThrowableItem itemInstance, ref HumanController.InputSample sample)
    {
        ITorchItem torchInstance = this.GetTorchInstance(itemInstance);
        if (!torchInstance.isLit)
        {
            if (vm != null)
            {
                vm.Play("ignite");
            }
            torchInstance.realIgniteTime = Time.time + 0.8f;
            torchInstance.nextPrimaryAttackTime = Time.time + 1.5f;
            torchInstance.nextSecondaryAttackTime = Time.time + 1.5f;
        }
    }

    public override void SecondaryAttack(ViewModel vm, ItemRepresentation itemRep, IThrowableItem itemInstance, ref HumanController.InputSample sample)
    {
        ITorchItem torchInstance = this.GetTorchInstance(itemInstance);
        if (!torchInstance.isLit)
        {
            this.PrimaryAttack(vm, itemRep, itemInstance, ref sample);
            torchInstance.forceSecondaryTime = Time.time + 1.51f;
        }
        else
        {
            if (vm != null)
            {
                vm.Play("throw");
            }
            torchInstance.realThrowTime = Time.time + 0.5f;
            torchInstance.nextPrimaryAttackTime = Time.time + 1.5f;
            torchInstance.nextSecondaryAttackTime = Time.time + 1.5f;
        }
    }

    private sealed class ITEM_TYPE : TorchItem<TorchItemDataBlock>, IHeldItem, IInventoryItem, IThrowableItem, ITorchItem, IWeaponItem
    {
        public ITEM_TYPE(TorchItemDataBlock BLOCK) : base(BLOCK)
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

        bool IThrowableItem.get_holdingBack()
        {
            return base.holdingBack;
        }

        float IThrowableItem.get_holdingStartTime()
        {
            return base.holdingStartTime;
        }

        float IThrowableItem.get_minReleaseTime()
        {
            return base.minReleaseTime;
        }

        void IThrowableItem.set_holdingBack(bool value)
        {
            base.holdingBack = value;
        }

        void IThrowableItem.set_holdingStartTime(float value)
        {
            base.holdingStartTime = value;
        }

        void IThrowableItem.set_minReleaseTime(float value)
        {
            base.minReleaseTime = value;
        }

        void ITorchItem.Extinguish()
        {
            base.Extinguish();
        }

        float ITorchItem.get_forceSecondaryTime()
        {
            return base.forceSecondaryTime;
        }

        bool ITorchItem.get_isLit()
        {
            return base.isLit;
        }

        GameObject ITorchItem.get_light()
        {
            return base.light;
        }

        float ITorchItem.get_realIgniteTime()
        {
            return base.realIgniteTime;
        }

        float ITorchItem.get_realThrowTime()
        {
            return base.realThrowTime;
        }

        void ITorchItem.Ignite()
        {
            base.Ignite();
        }

        void ITorchItem.set_forceSecondaryTime(float value)
        {
            base.forceSecondaryTime = value;
        }

        void ITorchItem.set_light(GameObject value)
        {
            base.light = value;
        }

        void ITorchItem.set_realIgniteTime(float value)
        {
            base.realIgniteTime = value;
        }

        void ITorchItem.set_realThrowTime(float value)
        {
            base.realThrowTime = value;
        }

        bool IWeaponItem.get_canAim()
        {
            return base.canAim;
        }

        float IWeaponItem.get_deployFinishedTime()
        {
            return base.deployFinishedTime;
        }

        float IWeaponItem.get_nextPrimaryAttackTime()
        {
            return base.nextPrimaryAttackTime;
        }

        float IWeaponItem.get_nextSecondaryAttackTime()
        {
            return base.nextSecondaryAttackTime;
        }

        void IWeaponItem.set_deployFinishedTime(float value)
        {
            base.deployFinishedTime = value;
        }

        void IWeaponItem.set_nextPrimaryAttackTime(float value)
        {
            base.nextPrimaryAttackTime = value;
        }

        void IWeaponItem.set_nextSecondaryAttackTime(float value)
        {
            base.nextSecondaryAttackTime = value;
        }

        bool IWeaponItem.ValidatePrimaryMessageTime(double timestamp)
        {
            return base.ValidatePrimaryMessageTime(timestamp);
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

