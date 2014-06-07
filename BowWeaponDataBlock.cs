using Facepunch;
using System;
using uLink;
using UnityEngine;

public class BowWeaponDataBlock : WeaponDataBlock
{
    private static HUDHitIndicator _hitIndicator;
    public string arrowPickupString;
    public GameObject arrowPrefab;
    public float arrowSpeed;
    public AudioClip cancelArrowSound;
    public ItemDataBlock defaultAmmo;
    public AudioClip drawArrowSound;
    public float drawLength = 2f;
    public AudioClip fireArrowSound;
    public float tooTiredLength = 8f;

    public void ArrowReportHit(IDMain hitMain, ArrowMovement arrow, ItemRepresentation itemRepresentation, IBowWeaponItem itemInstance)
    {
        if (hitMain != null)
        {
            TakeDamage component = hitMain.GetComponent<TakeDamage>();
            if (component != null)
            {
                BitStream stream = new BitStream(false);
                stream.Write<NetEntityID>(NetEntityID.Get((MonoBehaviour) hitMain), new object[0]);
                stream.Write<Vector3>(hitMain.transform.position, new object[0]);
                itemRepresentation.ActionStream(2, RPCMode.Server, stream);
                Character shooterOrNull = itemInstance.character;
                if ((component != null) && component.ShouldPlayHitNotification())
                {
                    this.PlayHitNotification(arrow.transform.position, shooterOrNull);
                }
            }
        }
    }

    public void ArrowReportMiss(ArrowMovement arrow, ItemRepresentation itemRepresentation)
    {
        BitStream stream = new BitStream(false);
        stream.Write<Vector3>(arrow.transform.position, new object[0]);
        itemRepresentation.ActionStream(3, RPCMode.Server, stream);
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
        Vector3 pos = stream.ReadVector3();
        Quaternion ang = stream.ReadQuaternion();
        this.FireArrow(pos, ang, rep, null);
    }

    public virtual void DoWeaponEffects(Transform soundTransform, Vector3 startPos, Vector3 endPos, Socket muzzleSocket, bool firstPerson, Component hitComponent, bool allowBlood, ItemRepresentation itemRep)
    {
    }

    public void FireArrow(Vector3 pos, Quaternion ang, ItemRepresentation itemRep, IBowWeaponItem itemInstance)
    {
        GameObject obj2 = (GameObject) Object.Instantiate(this.arrowPrefab, pos, ang);
        obj2.GetComponent<ArrowMovement>().Init(this.arrowSpeed, itemRep, itemInstance, false);
        this.fireArrowSound.Play(pos, (float) 1f, (float) 2f, (float) 10f);
    }

    public virtual float GetGUIDamage()
    {
        return 999f;
    }

    public override string GetItemDescription()
    {
        return "This is a weapon. Drag it to your belt (right side of screen) and press the corresponding number key to use it.";
    }

    public override byte GetMaxEligableSlots()
    {
        return 0;
    }

    public override void InstallData(IInventoryItem item)
    {
        base.InstallData(item);
    }

    public virtual void Local_CancelArrow(ViewModel vm, ItemRepresentation itemRep, IBowWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        if (vm != null)
        {
            vm.CrossFade("cancelarrow", 0.15f);
        }
        this.MakeReadyIn(itemInstance, base.fireRate * 3f);
        this.cancelArrowSound.PlayLocal(Camera.main.transform, Vector3.zero, 1f, 1f, 3f, 20f, 0);
    }

    public virtual void Local_FireArrow(ViewModel vm, ItemRepresentation itemRep, IBowWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        if (vm != null)
        {
            vm.Play("fire_1", PlayMode.StopSameLayer);
        }
        this.MakeReadyIn(itemInstance, base.fireRate);
        Character character = itemInstance.character;
        if (character != null)
        {
            Ray eyesRay = character.eyesRay;
            Vector3 eyesOrigin = character.eyesOrigin;
            this.FireArrow(eyesOrigin, character.eyesRotation, itemRep, itemInstance);
            BitStream stream = new BitStream(false);
            stream.WriteVector3(eyesOrigin);
            stream.WriteQuaternion(character.eyesRotation);
            itemRep.ActionStream(1, RPCMode.Server, stream);
        }
    }

    public virtual void Local_GetTired(ViewModel vm, ItemRepresentation itemRep, IBowWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        if (!itemInstance.tired && (vm != null))
        {
            vm.CrossFade("tiredloop", 5f);
        }
    }

    public virtual void Local_ReadyArrow(ViewModel vm, ItemRepresentation itemRep, IBowWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        if (vm != null)
        {
            vm.Play("drawarrow", PlayMode.StopSameLayer);
        }
        itemInstance.completeDrawTime = Time.time + this.drawLength;
        this.drawArrowSound.PlayLocal(Camera.main.transform, Vector3.zero, 1f, 1f, 3f, 20f, 0);
    }

    public virtual void MakeReadyIn(IBowWeaponItem itemInstance, float delay)
    {
        itemInstance.MakeReadyIn(delay);
    }

    protected virtual void PlayHitNotification(Vector3 point, Character shooterOrNull)
    {
        if ((WeaponDataBlock._hitNotify != null) || Bundling.Load<AudioClip>("content/shared/sfx/hitnotification", out WeaponDataBlock._hitNotify))
        {
            WeaponDataBlock._hitNotify.PlayLocal(Camera.main.transform, Vector3.zero, 1f, 1);
        }
        if ((_hitIndicator != null) || Bundling.Load<HUDHitIndicator>("content/hud/HUDHitIndicator", out _hitIndicator))
        {
            bool followPoint = true;
            HUDHitIndicator.CreateIndicator(point, followPoint, _hitIndicator);
        }
    }

    public override void PopulateInfoWindow(ItemToolTip infoWindow, IInventoryItem tipItem)
    {
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
    }

    private sealed class ITEM_TYPE : BowWeaponItem<BowWeaponDataBlock>, IBowWeaponItem, IHeldItem, IInventoryItem, IWeaponItem
    {
        public ITEM_TYPE(BowWeaponDataBlock BLOCK) : base(BLOCK)
        {
        }

        void IBowWeaponItem.ArrowReportHit(IDMain hitMain, ArrowMovement arrow)
        {
            base.ArrowReportHit(hitMain, arrow);
        }

        void IBowWeaponItem.ArrowReportMiss(ArrowMovement arrow)
        {
            base.ArrowReportMiss(arrow);
        }

        IInventoryItem IBowWeaponItem.FindAmmo()
        {
            return base.FindAmmo();
        }

        bool IBowWeaponItem.get_arrowDrawn()
        {
            return base.arrowDrawn;
        }

        float IBowWeaponItem.get_completeDrawTime()
        {
            return base.completeDrawTime;
        }

        int IBowWeaponItem.get_currentArrowID()
        {
            return base.currentArrowID;
        }

        bool IBowWeaponItem.get_tired()
        {
            return base.tired;
        }

        void IBowWeaponItem.MakeReadyIn(float delay)
        {
            base.MakeReadyIn(delay);
        }

        void IBowWeaponItem.set_arrowDrawn(bool value)
        {
            base.arrowDrawn = value;
        }

        void IBowWeaponItem.set_completeDrawTime(float value)
        {
            base.completeDrawTime = value;
        }

        void IBowWeaponItem.set_currentArrowID(int value)
        {
            base.currentArrowID = value;
        }

        void IBowWeaponItem.set_tired(bool value)
        {
            base.tired = value;
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

