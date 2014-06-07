using Facepunch;
using System;
using uLink;
using UnityEngine;

public class WeaponDataBlock : HeldItemDataBlock
{
    private static HUDHitIndicator _hitIndicator;
    public static AudioClip _hitNotify;
    public float damageMax = 5f;
    public float damageMin = 5f;
    public float deployLength = 0.75f;
    public float fireRate = 1f;
    public float fireRateSecondary = 1f;
    public bool isSemiAuto;

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public virtual float GetDamage()
    {
        return Random.Range(this.damageMin, this.damageMax);
    }

    public override void InstallData(IInventoryItem item)
    {
        base.InstallData(item);
    }

    protected virtual void PlayHitNotification(Vector3 point, Character shooterOrNull)
    {
        if ((_hitNotify != null) || Bundling.Load<AudioClip>("content/shared/sfx/hitnotification", out _hitNotify))
        {
            _hitNotify.PlayLocal(Camera.main.transform, Vector3.zero, 1f, 1);
        }
        if ((_hitIndicator != null) || Bundling.Load<HUDHitIndicator>("content/hud/HUDHitIndicator", out _hitIndicator))
        {
            bool followPoint = (shooterOrNull == null) || !shooterOrNull.stateFlags.aim;
            HUDHitIndicator.CreateIndicator(point, followPoint, _hitIndicator);
        }
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<float>(this.deployLength, new object[0]);
        stream.Write<float>(this.damageMin, new object[0]);
        stream.Write<float>(this.damageMax, new object[0]);
        stream.Write<float>(this.fireRate, new object[0]);
        stream.Write<float>(this.fireRateSecondary, new object[0]);
        stream.Write<bool>(this.isSemiAuto, new object[0]);
    }

    private sealed class ITEM_TYPE : WeaponItem<WeaponDataBlock>, IHeldItem, IInventoryItem, IWeaponItem
    {
        public ITEM_TYPE(WeaponDataBlock BLOCK) : base(BLOCK)
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

