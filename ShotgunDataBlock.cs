using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class ShotgunDataBlock : BulletWeaponDataBlock
{
    public int numPellets = 6;
    public float xSpread = 8f;
    public float ySpread = 8f;

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
        this.DoWeaponEffects(rep.transform.parent, rep.muzzle, false, rep);
        float bulletRange = this.GetBulletRange(rep);
        for (int i = 0; i < this.numPellets; i++)
        {
            GameObject obj2;
            NetEntityID yid;
            IDRemoteBodyPart part;
            bool flag;
            bool flag2;
            bool flag3;
            BodyPart part2;
            Vector3 vector;
            Vector3 vector2;
            Transform transform;
            this.ReadHitInfo(stream, out obj2, out flag, out flag2, out part2, out part, out yid, out transform, out vector, out vector2, out flag3);
            Component component = (part == null) ? ((obj2 == null) ? null : ((Component) obj2.GetComponentInChildren<CapsuleCollider>())) : ((Component) part);
            this.MakeTracer(rep.muzzle.position, vector, bulletRange, component, flag);
        }
    }

    public virtual void DoWeaponEffects(Transform soundTransform, Socket muzzleSocket, bool firstPerson, ItemRepresentation itemRep)
    {
        this.PlayFireSound(soundTransform, firstPerson, itemRep);
        Object.Destroy(muzzleSocket.InstantiateAsChild(!firstPerson ? base.muzzleFlashWorld : base.muzzleflashVM, false), 1f);
    }

    public virtual void FireSingleBullet(BitStream sendStream, Ray ray, ItemRepresentation itemRep, out Component hitComponent, out bool allowBlood)
    {
        RaycastHit2 hit;
        Vector3 point;
        IDMain main;
        NetEntityID unassigned = NetEntityID.unassigned;
        bool hitNetworkView = false;
        bool didHit = Physics2.Raycast2(ray, out hit, this.GetBulletRange(itemRep), 0x183e1411);
        if (didHit)
        {
            point = hit.point;
            IDBase id = hit.id;
            hitComponent = (hit.remoteBodyPart == null) ? ((Component) hit.collider) : ((Component) hit.remoteBodyPart);
            main = (id == null) ? null : id.idMain;
            if (main != null)
            {
                unassigned = NetEntityID.Get((MonoBehaviour) main);
                if (!unassigned.isUnassigned)
                {
                    TakeDamage component = main.GetComponent<TakeDamage>();
                    if (component != null)
                    {
                        hitNetworkView = true;
                        if (component.ShouldPlayHitNotification())
                        {
                            this.PlayHitNotification(point, null);
                        }
                    }
                }
            }
        }
        else
        {
            main = null;
            point = ray.GetPoint(this.GetBulletRange(itemRep));
            hitComponent = null;
        }
        this.WriteHitInfo(sendStream, ref ray, didHit, ref hit, hitNetworkView, unassigned);
        allowBlood = didHit && hitNetworkView;
    }

    public override float GetGUIDamage()
    {
        return (base.GetGUIDamage() * this.numPellets);
    }

    public override void Local_FireWeapon(ViewModel vm, ItemRepresentation itemRep, IBulletWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        Character character = itemInstance.character;
        if ((character != null) && (itemInstance.clipAmmo > 0))
        {
            Socket muzzle;
            Ray eyesRay = character.eyesRay;
            int count = 1;
            itemInstance.Consume(ref count);
            BitStream sendStream = new BitStream(false);
            float bulletRange = this.GetBulletRange(itemRep);
            for (int i = 0; i < this.numPellets; i++)
            {
                bool flag;
                Ray ray = eyesRay;
                ray.direction = (Vector3) ((Quaternion.LookRotation(eyesRay.direction) * Quaternion.Euler(Random.Range(-this.xSpread, this.xSpread), Random.Range(-this.ySpread, this.ySpread), 0f)) * Vector3.forward);
                Component hitComponent = null;
                this.FireSingleBullet(sendStream, ray, itemRep, out hitComponent, out flag);
                this.MakeTracer(ray.origin, ray.origin + ((Vector3) (ray.direction * bulletRange)), bulletRange, hitComponent, flag);
            }
            itemRep.ActionStream(1, RPCMode.Server, sendStream);
            bool firstPerson = (bool) vm;
            if (firstPerson)
            {
                muzzle = vm.muzzle;
            }
            else
            {
                muzzle = itemRep.muzzle;
            }
            this.DoWeaponEffects(character.transform, muzzle, firstPerson, itemRep);
            if (firstPerson)
            {
                vm.PlayFireAnimation();
            }
            float num4 = 1f;
            if (sample.aim)
            {
                num4 -= base.aimingRecoilSubtract;
            }
            else if (sample.crouch)
            {
                num4 -= base.crouchRecoilSubtract;
            }
            float pitch = Random.Range(base.recoilPitchMin, base.recoilPitchMax) * num4;
            float yaw = Random.Range(base.recoilYawMin, base.recoilYawMax) * num4;
            RecoilSimulation recoilSimulation = character.recoilSimulation;
            if (recoilSimulation != null)
            {
                recoilSimulation.AddRecoil(base.recoilDuration, pitch, yaw);
            }
            HeadBob bob = CameraMount.current.GetComponent<HeadBob>();
            if ((bob != null) && (base.shotBob != null))
            {
                bob.AddEffect(base.shotBob);
            }
        }
    }

    public virtual void MakeTracer(Vector3 startPos, Vector3 endPos, float range, Component component, bool allowBlood)
    {
        Vector3 forward = endPos - startPos;
        forward.Normalize();
        Tracer tracer = ((GameObject) Object.Instantiate(base.tracerPrefab, startPos, Quaternion.LookRotation(forward))).GetComponent<Tracer>();
        if (tracer != null)
        {
            tracer.Init(component, 0x183e1411, range, allowBlood);
        }
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<float>(this.xSpread, new object[0]);
        stream.Write<int>(this.numPellets, new object[0]);
        stream.Write<float>(this.ySpread, new object[0]);
    }

    private sealed class ITEM_TYPE : BulletWeaponItem<ShotgunDataBlock>, IBulletWeaponItem, IHeldItem, IInventoryItem, IWeaponItem
    {
        public ITEM_TYPE(ShotgunDataBlock BLOCK) : base(BLOCK)
        {
        }

        int IBulletWeaponItem.get_cachedCasings()
        {
            return base.cachedCasings;
        }

        int IBulletWeaponItem.get_clipAmmo()
        {
            return base.clipAmmo;
        }

        MagazineDataBlock IBulletWeaponItem.get_clipType()
        {
            return base.clipType;
        }

        float IBulletWeaponItem.get_nextCasingsTime()
        {
            return base.nextCasingsTime;
        }

        void IBulletWeaponItem.set_cachedCasings(int value)
        {
            base.cachedCasings = value;
        }

        void IBulletWeaponItem.set_clipAmmo(int value)
        {
            base.clipAmmo = value;
        }

        void IBulletWeaponItem.set_nextCasingsTime(float value)
        {
            base.nextCasingsTime = value;
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

