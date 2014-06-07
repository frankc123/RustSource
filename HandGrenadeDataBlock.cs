using System;
using uLink;
using UnityEngine;

public class HandGrenadeDataBlock : ThrowableItemDataBlock
{
    public AudioClip pullPinSound;

    public override void AttackReleased(ViewModel vm, ItemRepresentation itemRep, IThrowableItem itemInstance, ref HumanController.InputSample sample)
    {
        Debug.Log("Attack released!!!");
        vm.Play("throw");
        vm.PlayQueued("deploy");
        this.GetHandGrenadeItemInstance(itemInstance).nextPrimaryAttackTime = Time.time + 1f;
        this.GetHandGrenadeItemInstance(itemInstance).nextSecondaryAttackTime = Time.time + 1f;
        Character component = PlayerClient.GetLocalPlayer().controllable.GetComponent<Character>();
        Vector3 eyesOrigin = component.eyesOrigin;
        Vector3 forward = component.eyesAngles.forward;
        BitStream stream = new BitStream(false);
        stream.WriteVector3(eyesOrigin);
        stream.WriteVector3((Vector3) (forward * this.GetHandGrenadeItemInstance(itemInstance).heldThrowStrength));
        Debug.Log("Throw strength is : " + this.GetHandGrenadeItemInstance(itemInstance).heldThrowStrength);
        this.GetHandGrenadeItemInstance(itemInstance).EndHoldingBack();
        itemRep.ActionStream(1, RPCMode.Server, stream);
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
    }

    public IHandGrenadeItem GetHandGrenadeItemInstance(IInventoryItem itemInstance)
    {
        return (itemInstance as IHandGrenadeItem);
    }

    public override void PrimaryAttack(ViewModel vm, ItemRepresentation itemRep, IThrowableItem itemInstance, ref HumanController.InputSample sample)
    {
        base.PrimaryAttack(vm, itemRep, itemInstance, ref sample);
        vm.Play("pull_pin");
        this.pullPinSound.Play();
        this.GetHandGrenadeItemInstance(itemInstance).nextPrimaryAttackTime = Time.time + 1000f;
        this.GetHandGrenadeItemInstance(itemInstance).nextSecondaryAttackTime = Time.time + 1000f;
    }

    protected override GameObject ThrowItem(ItemRepresentation rep, IThrowableItem item, Vector3 origin, Vector3 forward, NetworkViewID owner)
    {
        forward.Normalize();
        Vector3 velocity = (Vector3) (forward * 20f);
        Vector3 position = origin + ((Vector3) (forward * 0.5f));
        return this.SpawnThrowItem(owner, base.throwObjectPrefab, position, Quaternion.LookRotation(Vector3.up), velocity);
    }

    private sealed class ITEM_TYPE : HandGrenadeItem<HandGrenadeDataBlock>, IHandGrenadeItem, IHeldItem, IInventoryItem, IThrowableItem, IWeaponItem
    {
        public ITEM_TYPE(HandGrenadeDataBlock BLOCK) : base(BLOCK)
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

