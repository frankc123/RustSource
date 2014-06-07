using Facepunch.Movement;
using Rust;
using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class MeleeWeaponDataBlock : WeaponDataBlock
{
    [SerializeField]
    protected string _swingingMovementAnimationGroupName;
    public float caloriesPerSwing = 5f;
    public float[] efficiencies;
    public float gatherPerHitAmount = 0.25f;
    public bool gathersResources;
    public const int hitMask = 0x183e1411;
    public GameObject impactEffect;
    public GameObject impactEffectFlesh;
    public GameObject impactEffectWood;
    public AudioClip[] impactSoundFlesh;
    public AudioClip impactSoundGeneric;
    public AudioClip impactSoundWood;
    public float midSwingDelay = 0.35f;
    public float range = 2f;
    public AudioClip swingSound;
    public float swingSoundDelay = 0.2f;
    public float worldSwingAnimationSpeed = 1f;

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
        GameObject gameObject;
        NetEntityID unassigned;
        if (stream.ReadBoolean())
        {
            unassigned = stream.Read<NetEntityID>(new object[0]);
            if (!unassigned.isUnassigned)
            {
                gameObject = unassigned.gameObject;
                if (gameObject == null)
                {
                    unassigned = NetEntityID.unassigned;
                }
            }
            else
            {
                gameObject = null;
            }
        }
        else
        {
            unassigned = NetEntityID.unassigned;
            gameObject = null;
        }
        Vector3 pos = stream.ReadVector3();
        bool flag2 = stream.ReadBoolean();
        this.EndSwingWorldAnimations(rep);
        if (gameObject != null)
        {
            Vector3 vector2 = rep.transform.position - pos;
            Quaternion rot = Quaternion.LookRotation(vector2.normalized);
            this.DoMeleeEffects(rep.transform.position, pos, rot, gameObject);
        }
    }

    public override void DoAction2(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
    {
        NetEntityID yid = stream.Read<NetEntityID>(new object[0]);
        if (!yid.isUnassigned && (yid.idBase == null))
        {
        }
    }

    public override void DoAction3(BitStream stream, ItemRepresentation itemRep, ref NetworkMessageInfo info)
    {
        this.StartSwingWorldAnimations(itemRep);
    }

    public virtual void DoMeleeEffects(Vector3 fromPos, Vector3 pos, Quaternion rot, GameObject hitObj)
    {
        if (hitObj.CompareTag("Tree Collider"))
        {
            GameObject obj2 = Object.Instantiate(this.impactEffectWood, pos, rot) as GameObject;
            Object.Destroy(obj2, 1.5f);
            this.impactSoundWood.Play(pos, (float) 1f, (float) 2f, (float) 10f);
        }
        else
        {
            SurfaceInfo.DoImpact(hitObj, SurfaceInfoObject.ImpactType.Melee, pos, rot);
        }
    }

    private void EndSwingWorldAnimations(ItemRepresentation itemRep)
    {
        if (!string.IsNullOrEmpty(this._swingingMovementAnimationGroupName) && (this._swingingMovementAnimationGroupName != base.animationGroupName))
        {
            itemRep.OverrideAnimationGroupName(null);
        }
    }

    public override float GetDamage()
    {
        return Random.Range(base.damageMin, base.damageMax);
    }

    public virtual float GetRange()
    {
        return this.range;
    }

    public virtual void Local_FireWeapon(ViewModel vm, ItemRepresentation itemRep, IMeleeWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        this.StartSwingWorldAnimations(itemRep);
        if (vm != null)
        {
            vm.PlayFireAnimation();
        }
        itemInstance.QueueSwingSound(Time.time + this.swingSoundDelay);
        itemInstance.QueueMidSwing(Time.time + this.midSwingDelay);
        if (itemRep.networkViewIsMine)
        {
            itemRep.Action(3, RPCMode.Server);
        }
    }

    public virtual void Local_MidSwing(ViewModel vm, ItemRepresentation itemRep, IMeleeWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        Character shooterOrNull = itemInstance.character;
        if (shooterOrNull != null)
        {
            BodyPart part;
            Ray eyesRay = shooterOrNull.eyesRay;
            bool flag = false;
            Collider hitCollider = null;
            Vector3 zero = Vector3.zero;
            Vector3 up = Vector3.up;
            NetEntityID unassigned = NetEntityID.unassigned;
            bool flag2 = false;
            flag = this.Physics2SphereCast(eyesRay, 0.3f, this.GetRange(), 0x183e1411, out zero, out up, out hitCollider, out part);
            bool flag3 = false;
            TakeDamage component = null;
            if (flag)
            {
                IDBase base2;
                TransformHelpers.GetIDBaseFromCollider(hitCollider, out base2);
                IDMain main = (base2 == null) ? null : base2.idMain;
                if (main != null)
                {
                    unassigned = NetEntityID.Get((MonoBehaviour) main);
                    flag2 = !unassigned.isUnassigned;
                    component = main.GetComponent<TakeDamage>();
                    if ((component != null) && component.ShouldPlayHitNotification())
                    {
                        this.PlayHitNotification(zero, shooterOrNull);
                    }
                }
                flag3 = hitCollider.gameObject.CompareTag("Tree Collider");
                if (flag3)
                {
                    WoodBlockerTemp blockerForPoint = WoodBlockerTemp.GetBlockerForPoint(zero);
                    if (!blockerForPoint.HasWood())
                    {
                        flag3 = false;
                        Notice.Popup("", "There's no wood left here", 2f);
                    }
                    else
                    {
                        blockerForPoint.ConsumeWood(this.efficiencies[2]);
                    }
                }
                this.DoMeleeEffects(eyesRay.origin, zero, Quaternion.LookRotation(up), hitCollider.gameObject);
                if ((vm != null) && ((component != null) || flag3))
                {
                    vm.CrossFade("pull_out", 0.05f, PlayMode.StopSameLayer, 1.1f);
                }
            }
            BitStream stream = new BitStream(false);
            if (flag2)
            {
                stream.WriteBoolean(flag2);
                stream.Write<NetEntityID>(unassigned, new object[0]);
                stream.WriteVector3(zero);
            }
            else
            {
                stream.WriteBoolean(false);
                stream.WriteVector3(zero);
            }
            stream.WriteBoolean(flag3);
            itemRep.ActionStream(1, RPCMode.Server, stream);
            this.EndSwingWorldAnimations(itemRep);
        }
    }

    public virtual void Local_SecondaryFire(ViewModel vm, ItemRepresentation itemRep, IMeleeWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        RaycastHit hit;
        Character character = itemInstance.character;
        if ((character != null) && Physics.SphereCast(character.eyesRay, 0.3f, out hit, this.GetRange(), 0x183e1411))
        {
            IDBase component = hit.collider.gameObject.GetComponent<IDBase>();
            if (component != null)
            {
                NetEntityID argument = NetEntityID.Get((MonoBehaviour) component);
                if ((component.GetLocal<RepairReceiver>() != null) && (argument != NetEntityID.unassigned))
                {
                    if (vm != null)
                    {
                        vm.PlayFireAnimation();
                    }
                    itemInstance.QueueSwingSound(Time.time + this.swingSoundDelay);
                    itemRep.Action<NetEntityID>(2, RPCMode.Server, argument);
                }
            }
        }
    }

    public bool Physics2SphereCast(Ray ray, float radius, float range, int hitMask, out Vector3 point, out Vector3 normal, out Collider hitCollider, out BodyPart part)
    {
        RaycastHit2 hit;
        RaycastHit hit3;
        RaycastHit hit4 = new RaycastHit();
        bool flag = false;
        bool flag2 = false;
        if (Physics.SphereCast(ray, radius, out hit3, range - radius, hitMask & -131073))
        {
            RaycastHit hit2;
            flag2 = true;
            hit4 = hit3;
            if (Physics.Raycast(ray, out hit2, range, hitMask & -131073))
            {
                flag = true;
                if (hit2.distance < hit3.distance)
                {
                    hit4 = hit2;
                }
            }
        }
        bool flag3 = flag2 || flag;
        if (Physics2.Raycast2(ray, out hit, range, hitMask) && (!flag3 || (hit.distance < hit4.distance)))
        {
            point = hit.point;
            normal = hit.normal;
            hitCollider = hit.collider;
            part = hit.bodyPart;
            return true;
        }
        if (!flag3)
        {
            Collider[] colliderArray = Physics.OverlapSphere(ray.origin, 0.3f, 0x20000);
            if (colliderArray.Length > 0)
            {
                point = ray.origin + ((Vector3) (ray.direction * 0.5f));
                normal = (Vector3) (ray.direction * -1f);
                hitCollider = colliderArray[0];
                part = BodyPart.Undefined;
                return true;
            }
        }
        if (!flag3)
        {
            point = ray.origin + ((Vector3) (ray.direction * range));
            normal = (Vector3) (ray.direction * -1f);
            hitCollider = null;
            part = BodyPart.Undefined;
            return false;
        }
        point = hit4.point;
        normal = hit4.normal;
        hitCollider = hit4.collider;
        part = BodyPart.Undefined;
        return true;
    }

    private void StartSwingWorldAnimations(ItemRepresentation itemRep)
    {
        if (!string.IsNullOrEmpty(this._swingingMovementAnimationGroupName) && (this._swingingMovementAnimationGroupName != base.animationGroupName))
        {
            itemRep.OverrideAnimationGroupName(this._swingingMovementAnimationGroupName);
        }
        itemRep.PlayWorldAnimation(GroupEvent.Attack, this.worldSwingAnimationSpeed);
    }

    public virtual void SwingSound()
    {
        this.swingSound.PlayLocal(Camera.main.transform, Vector3.zero, 1f, Random.Range((float) 0.85f, (float) 1f), 3f, 20f, 0);
    }

    private sealed class ITEM_TYPE : MeleeWeaponItem<MeleeWeaponDataBlock>, IHeldItem, IInventoryItem, IMeleeWeaponItem, IWeaponItem
    {
        public ITEM_TYPE(MeleeWeaponDataBlock BLOCK) : base(BLOCK)
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

        float IMeleeWeaponItem.get_queuedSwingAttackTime()
        {
            return base.queuedSwingAttackTime;
        }

        float IMeleeWeaponItem.get_queuedSwingSoundTime()
        {
            return base.queuedSwingSoundTime;
        }

        void IMeleeWeaponItem.set_queuedSwingAttackTime(float value)
        {
            base.queuedSwingAttackTime = value;
        }

        void IMeleeWeaponItem.set_queuedSwingSoundTime(float value)
        {
            base.queuedSwingSoundTime = value;
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

