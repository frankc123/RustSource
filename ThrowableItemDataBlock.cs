using System;
using uLink;
using UnityEngine;

public class ThrowableItemDataBlock : WeaponDataBlock
{
    public GameObject throwObjectPrefab;
    public float throwStrengthMax = 10f;
    public float throwStrengthMin = 10f;
    public float throwStrengthPerSec = 10f;

    public virtual void AttackReleased(ViewModel vm, ItemRepresentation itemRep, IThrowableItem itemInstance, ref HumanController.InputSample sample)
    {
        Debug.Log("Throwable attack released");
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public virtual void DoActualThrow(ItemRepresentation itemRep, IThrowableItem itemInstance, ViewModel vm)
    {
        Character component = PlayerClient.GetLocalPlayer().controllable.GetComponent<Character>();
        Vector3 eyesOrigin = component.eyesOrigin;
        Vector3 forward = component.eyesAngles.forward;
        if (vm != null)
        {
            vm.PlayQueued("deploy");
        }
        int count = 1;
        if (itemInstance.Consume(ref count))
        {
            itemInstance.inventory.RemoveItem(itemInstance.slot);
        }
        BitStream argument = new BitStream(false);
        argument.WriteVector3(eyesOrigin);
        argument.WriteVector3(forward);
        itemRep.Action<BitStream>(1, RPCMode.Server, argument);
    }

    public IThrowableItem GetThrowableInstance(IInventoryItem itemInstance)
    {
        return (itemInstance as IThrowableItem);
    }

    public override void InstallData(IInventoryItem item)
    {
        base.InstallData(item);
    }

    public virtual void PrimaryAttack(ViewModel vm, ItemRepresentation itemRep, IThrowableItem itemInstance, ref HumanController.InputSample sample)
    {
        this.GetThrowableInstance(itemInstance).BeginHoldingBack();
    }

    public virtual void SecondaryAttack(ViewModel vm, ItemRepresentation itemRep, IThrowableItem itemInstance, ref HumanController.InputSample sample)
    {
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<float>(this.throwStrengthMin, new object[0]);
        stream.Write<float>(this.throwStrengthPerSec, new object[0]);
        stream.Write<float>(this.throwStrengthMax, new object[0]);
    }

    protected virtual GameObject SpawnThrowItem(NetworkViewID owningViewID, GameObject prefab, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        return null;
    }

    protected virtual GameObject ThrowItem(ItemRepresentation rep, IThrowableItem item, Vector3 origin, Vector3 forward, NetworkViewID owner)
    {
        Vector3 velocity = (Vector3) (forward * item.heldThrowStrength);
        Vector3 position = origin + ((Vector3) (forward * 1f));
        Quaternion rotation = Quaternion.LookRotation(Vector3.up);
        return this.SpawnThrowItem(owner, this.throwObjectPrefab, position, rotation, velocity);
    }

    private sealed class ITEM_TYPE : ThrowableItem<ThrowableItemDataBlock>, IHeldItem, IInventoryItem, IThrowableItem, IWeaponItem
    {
        public ITEM_TYPE(ThrowableItemDataBlock BLOCK) : base(BLOCK)
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

