using System;
using System.Runtime.InteropServices;
using uLink;
using UnityEngine;

public class BulletWeaponDataBlock : WeaponDataBlock
{
    public float aimingRecoilSubtract = 0.5f;
    public float aimSway;
    public float aimSwaySpeed = 1f;
    public AmmoItemDataBlock ammoType;
    public float bulletRange = 200f;
    public float crouchRecoilSubtract = 0.2f;
    public AudioClip dryFireSound;
    public AudioClip fireSound;
    public AudioClip fireSound_Far;
    public AudioClip fireSound_Silenced;
    public AudioClip fireSound_SilencedFar;
    public float fireSoundRange = 300f;
    private static bool headRecoil = true;
    public AudioClip headshotSound;
    public const int hitMask = 0x183e1411;
    private const byte kDidHitNetworkViewWithoutBodyPart = 0xfe;
    private const byte kDidNotHitNetworkView = 0xff;
    public int maxClipAmmo;
    public int maxEligableSlots = 5;
    public GameObject muzzleflashVM;
    public GameObject muzzleFlashWorld;
    public bool NoAimingAfterShot;
    public float recoilDuration;
    public float recoilPitchMax;
    public float recoilPitchMin;
    public float recoilYawMax;
    public float recoilYawMin;
    public float reloadDuration = 1.5f;
    public AudioClip reloadSound;
    public BobEffect shotBob;
    public GameObject tracerPrefab;
    private static bool weaponRecoil = true;

    public void Awake()
    {
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public override void DoAction1(BitStream stream, ItemRepresentation rep, ref NetworkMessageInfo info)
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
        if (flag3)
        {
            this.headshotSound.Play(obj2.transform.position, (float) 1f, (float) 4f, (float) 30f);
        }
        this.DoWeaponEffects(rep.transform.parent, rep.muzzle.position, vector, rep.muzzle, false, (part == null) ? ((obj2 == null) ? null : ((Component) obj2.GetComponentInChildren<CapsuleCollider>())) : ((Component) part), flag && (((part == null) || part2.IsDefined()) || (obj2.GetComponent<TakeDamage>() != null)), rep);
    }

    public override void DoAction2(BitStream stream, ItemRepresentation itemRep, ref NetworkMessageInfo info)
    {
    }

    public virtual void DoWeaponEffects(Transform soundTransform, Vector3 startPos, Vector3 endPos, Socket muzzleSocket, bool firstPerson, Component hitComponent, bool allowBlood, ItemRepresentation itemRep)
    {
        Vector3 forward = endPos - startPos;
        forward.Normalize();
        bool flag = this.IsSilenced(itemRep);
        Tracer component = ((GameObject) Object.Instantiate(this.tracerPrefab, startPos, Quaternion.LookRotation(forward))).GetComponent<Tracer>();
        if (component != null)
        {
            component.Init(hitComponent, 0x183e1411, this.GetBulletRange(itemRep), allowBlood);
        }
        if (flag)
        {
            component.startScale = Vector3.zero;
        }
        this.PlayFireSound(soundTransform, firstPerson, itemRep);
        if (!flag)
        {
            Object.Destroy(muzzleSocket.InstantiateAsChild(!firstPerson ? this.muzzleFlashWorld : this.muzzleflashVM, false), 1f);
        }
    }

    public override InventoryItem.MenuItemResult ExecuteMenuOption(InventoryItem.MenuItem option, IInventoryItem item)
    {
        if (option != InventoryItem.MenuItem.Unload)
        {
            return base.ExecuteMenuOption(option, item);
        }
        return InventoryItem.MenuItemResult.DoneOnServer;
    }

    public virtual float GetBulletRange(ItemRepresentation itemRep)
    {
        if (itemRep == null)
        {
            return this.bulletRange;
        }
        return (this.bulletRange * (!this.IsSilenced(itemRep) ? 1f : 0.75f));
    }

    public virtual float GetDamage(ItemRepresentation itemRep)
    {
        return (Random.Range(base.damageMin, base.damageMax) * (!this.IsSilenced(itemRep) ? 1f : 0.8f));
    }

    public virtual AudioClip GetFarFireSound(ItemRepresentation itemRep)
    {
        if (this.IsSilenced(itemRep))
        {
            return this.fireSound_SilencedFar;
        }
        return this.fireSound_Far;
    }

    public virtual float GetFarFireSoundRangeMax()
    {
        return this.fireSoundRange;
    }

    public virtual float GetFarFireSoundRangeMin()
    {
        return (this.GetFireSoundRangeMax() * 0.5f);
    }

    public virtual AudioClip GetFireSound(ItemRepresentation itemRep)
    {
        if (this.IsSilenced(itemRep))
        {
            return this.fireSound_Silenced;
        }
        return this.fireSound;
    }

    public virtual float GetFireSoundRangeMax()
    {
        return 60f;
    }

    public virtual float GetFireSoundRangeMin()
    {
        return 2f;
    }

    public virtual float GetGUIDamage()
    {
        return (base.damageMin + ((base.damageMax - base.damageMin) * 0.5f));
    }

    public override string GetItemDescription()
    {
        return "This is a weapon. Drag it to your belt (right side of screen) and press the corresponding number key to use it.";
    }

    public override byte GetMaxEligableSlots()
    {
        return (byte) this.maxEligableSlots;
    }

    public override void InstallData(IInventoryItem item)
    {
        base.InstallData(item);
        IBulletWeaponItem item2 = item as IBulletWeaponItem;
        base._maxUses = this.maxClipAmmo;
        item2.clipAmmo = this.maxClipAmmo;
    }

    public virtual bool IsSilenced(ItemRepresentation itemRep)
    {
        return ((itemRep.modFlags & ItemModFlags.Audio) == ItemModFlags.Audio);
    }

    public virtual void Local_DryFire(ViewModel vm, ItemRepresentation itemRep)
    {
        this.dryFireSound.PlayLocal(itemRep.transform, Vector3.zero, 1f, 0);
    }

    public virtual void Local_FireWeapon(ViewModel vm, ItemRepresentation itemRep, IBulletWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        Component component;
        Vector3 point;
        RaycastHit2 hit;
        IDMain main;
        bool flag4;
        Socket muzzle;
        bool flag5;
        Character shooterOrNull = itemInstance.character;
        if (shooterOrNull == null)
        {
            return;
        }
        if (itemInstance.clipAmmo <= 0)
        {
            return;
        }
        Ray eyesRay = shooterOrNull.eyesRay;
        NetEntityID unassigned = NetEntityID.unassigned;
        bool hitNetworkView = false;
        int count = 1;
        itemInstance.Consume(ref count);
        bool didHit = Physics2.Raycast2(eyesRay, out hit, this.GetBulletRange(itemRep), 0x183e1411);
        TakeDamage damage = null;
        if (!didHit)
        {
            main = null;
            point = eyesRay.GetPoint(1000f);
            component = null;
            goto Label_019E;
        }
        point = hit.point;
        IDBase id = hit.id;
        component = (hit.remoteBodyPart == null) ? ((Component) hit.collider) : ((Component) hit.remoteBodyPart);
        main = (id == null) ? null : id.idMain;
        if (main == null)
        {
            goto Label_019E;
        }
        unassigned = NetEntityID.Get((MonoBehaviour) main);
        if (unassigned.isUnassigned)
        {
            goto Label_019E;
        }
        hitNetworkView = true;
        damage = main.GetComponent<TakeDamage>();
        if ((damage != null) && damage.ShouldPlayHitNotification())
        {
            this.PlayHitNotification(point, shooterOrNull);
        }
        bool flag3 = false;
        if (hit.remoteBodyPart != null)
        {
            BodyPart bodyPart = hit.remoteBodyPart.bodyPart;
            switch (bodyPart)
            {
                case BodyPart.Brain:
                case BodyPart.L_Eye:
                case BodyPart.R_Eye:
                    break;

                default:
                    switch (bodyPart)
                    {
                        case BodyPart.Head:
                        case BodyPart.Jaw:
                            break;

                        case BodyPart.Scalp:
                        case BodyPart.Nostrils:
                            goto Label_016C;

                        default:
                            goto Label_016C;
                    }
                    break;
            }
            flag3 = true;
        }
        goto Label_0174;
    Label_016C:
        flag3 = false;
    Label_0174:
        if (flag3)
        {
            this.headshotSound.Play();
        }
    Label_019E:
        flag4 = didHit && ((!hit.isHitboxHit || hit.bodyPart.IsDefined()) || (damage != null));
        if (vm != null)
        {
            Socket.Slot slot = vm.socketMap["muzzle"];
            muzzle = slot.socket;
            flag5 = true;
        }
        else
        {
            muzzle = itemRep.muzzle;
            flag5 = false;
        }
        Vector3 position = muzzle.position;
        this.DoWeaponEffects(shooterOrNull.transform, position, point, muzzle, flag5, component, flag4, itemRep);
        if (flag5)
        {
            vm.PlayFireAnimation();
        }
        float num2 = 1f;
        if (sample.aim && sample.crouch)
        {
            num2 -= this.aimingRecoilSubtract + (this.crouchRecoilSubtract * 0.5f);
        }
        else if (sample.aim)
        {
            num2 -= this.aimingRecoilSubtract;
        }
        else if (sample.crouch)
        {
            num2 -= this.crouchRecoilSubtract;
        }
        num2 = Mathf.Clamp01(num2);
        float pitch = Random.Range(this.recoilPitchMin, this.recoilPitchMax) * num2;
        float yaw = Random.Range(this.recoilYawMin, this.recoilYawMax) * num2;
        if (weaponRecoil && (shooterOrNull.recoilSimulation != null))
        {
            shooterOrNull.recoilSimulation.AddRecoil(this.recoilDuration, pitch, yaw);
        }
        HeadBob bob = CameraMount.current.GetComponent<HeadBob>();
        if (((bob != null) && (this.shotBob != null)) && headRecoil)
        {
            bob.AddEffect(this.shotBob);
        }
        BitStream sendStream = new BitStream(false);
        this.WriteHitInfo(sendStream, ref eyesRay, didHit, ref hit, hitNetworkView, unassigned);
        itemRep.ActionStream(1, RPCMode.Server, sendStream);
    }

    public virtual void Local_Reload(ViewModel vm, ItemRepresentation itemRep, IBulletWeaponItem itemInstance, ref HumanController.InputSample sample)
    {
        if (vm != null)
        {
            vm.PlayReloadAnimation();
        }
        this.reloadSound.PlayLocal(itemRep.transform, Vector3.zero, 1f, 0);
        itemRep.Action(3, RPCMode.Server);
    }

    public virtual void PlayFireSound(Transform soundTransform, bool firstPerson, ItemRepresentation itemRep)
    {
        bool flag = this.IsSilenced(itemRep);
        AudioClip fireSound = this.GetFireSound(itemRep);
        float num = Vector3.Distance(soundTransform.position, Camera.main.transform.position);
        float farFireSoundRangeMin = this.GetFarFireSoundRangeMin();
        fireSound.PlayLocal(soundTransform, Vector3.zero, 1f, Random.Range((float) 0.92f, (float) 1.08f), this.GetFireSoundRangeMin(), this.GetFireSoundRangeMax() * (!flag ? 1f : 1.5f), !firstPerson ? 20 : 0);
        if ((!firstPerson && (num > farFireSoundRangeMin)) && !flag)
        {
            AudioClip farFireSound = this.GetFarFireSound(itemRep);
            if (farFireSound != null)
            {
                farFireSound.PlayLocal(soundTransform, Vector3.zero, 1f, Random.Range((float) 0.9f, (float) 1.1f), 0f, this.GetFarFireSoundRangeMax(), 50);
            }
        }
    }

    public override void PopulateInfoWindow(ItemToolTip infoWindow, IInventoryItem tipItem)
    {
        infoWindow.AddItemTitle(this, tipItem, 0f);
        infoWindow.AddConditionInfo(tipItem);
        infoWindow.AddSectionTitle("Weapon Stats", 20f);
        float currentAmount = this.recoilPitchMax + this.recoilYawMax;
        float maxAmount = 60f;
        float num3 = 1f / base.fireRate;
        if (base.isSemiAuto)
        {
            infoWindow.AddBasicLabel("Semi Automatic Weapon", 15f);
        }
        else
        {
            infoWindow.AddProgressStat("Fire Rate", num3, 12f, 15f);
        }
        infoWindow.AddProgressStat("Damage", this.GetGUIDamage(), 100f, 15f);
        infoWindow.AddProgressStat("Recoil", currentAmount, maxAmount, 15f);
        infoWindow.AddProgressStat("Range", this.GetBulletRange(null), 200f, 15f);
        infoWindow.AddItemDescription(this, 15f);
        infoWindow.FinishPopulating();
    }

    protected virtual void ReadHitInfo(BitStream stream, out GameObject hitObj, out bool hitNetworkObj, out bool hitBodyPart, out BodyPart bodyPart, out IDRemoteBodyPart remoteBodyPart, out NetEntityID hitViewID, out Transform fromTransform, out Vector3 endPos, out Vector3 offset, out bool isHeadshot)
    {
        bool flag;
        byte num = stream.ReadByte();
        if (num < 0xff)
        {
            hitNetworkObj = true;
            if (num < 120)
            {
                hitBodyPart = true;
                bodyPart = (BodyPart) num;
            }
            else
            {
                hitBodyPart = false;
                bodyPart = BodyPart.Undefined;
            }
        }
        else
        {
            hitNetworkObj = false;
            hitBodyPart = false;
            bodyPart = BodyPart.Undefined;
        }
        if (hitNetworkObj)
        {
            hitViewID = stream.Read<NetEntityID>(new object[0]);
            if (!hitViewID.isUnassigned)
            {
                hitObj = hitViewID.gameObject;
                if (hitObj != null)
                {
                    IDBase base2 = IDBase.Get(hitObj);
                    if (base2 != null)
                    {
                        IDMain idMain = base2.idMain;
                        if (idMain != null)
                        {
                            HitBoxSystem hitBoxSystem;
                            if (idMain is Character)
                            {
                                hitBoxSystem = ((Character) idMain).hitBoxSystem;
                            }
                            else
                            {
                                hitBoxSystem = idMain.GetRemote<HitBoxSystem>();
                            }
                            if (hitBoxSystem != null)
                            {
                                hitBoxSystem.bodyParts.TryGetValue(bodyPart, out remoteBodyPart);
                            }
                            else
                            {
                                remoteBodyPart = null;
                            }
                        }
                        else
                        {
                            remoteBodyPart = null;
                        }
                    }
                    else
                    {
                        remoteBodyPart = null;
                    }
                }
                else
                {
                    remoteBodyPart = null;
                }
            }
            else
            {
                hitObj = null;
                remoteBodyPart = null;
            }
        }
        else
        {
            hitViewID = NetEntityID.unassigned;
            hitObj = null;
            remoteBodyPart = null;
        }
        endPos = stream.ReadVector3();
        offset = Vector3.zero;
        if (remoteBodyPart == null)
        {
            if (hitObj != null)
            {
                fromTransform = hitObj.transform;
                flag = true;
                isHeadshot = false;
            }
            else
            {
                fromTransform = null;
                flag = false;
                isHeadshot = false;
            }
        }
        else
        {
            flag = false;
            fromTransform = remoteBodyPart.transform;
            BodyPart part = bodyPart;
            switch (part)
            {
                case BodyPart.Brain:
                case BodyPart.L_Eye:
                case BodyPart.R_Eye:
                    break;

                default:
                    switch (part)
                    {
                        case BodyPart.Head:
                        case BodyPart.Jaw:
                            break;

                        case BodyPart.Scalp:
                        case BodyPart.Nostrils:
                            goto Label_01AC;

                        default:
                            goto Label_01AC;
                    }
                    break;
            }
            isHeadshot = true;
        }
        goto Label_01E7;
    Label_01AC:
        isHeadshot = false;
    Label_01E7:
        if (fromTransform != null)
        {
            offset = endPos;
            endPos = fromTransform.TransformPoint(endPos);
        }
    }

    public override int RetreiveMenuOptions(IInventoryItem item, InventoryItem.MenuItem[] results, int offset)
    {
        offset = base.RetreiveMenuOptions(item, results, offset);
        if (item.isInLocalInventory)
        {
            results[offset++] = InventoryItem.MenuItem.Unload;
        }
        return offset;
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<int>(0x183e1411, new object[0]);
        stream.Write<float>(this.crouchRecoilSubtract, new object[0]);
        stream.Write<int>(this.maxClipAmmo, new object[0]);
        stream.Write<float>(this.recoilPitchMin, new object[0]);
        stream.Write<float>(this.recoilPitchMax, new object[0]);
        stream.Write<float>(this.recoilYawMin, new object[0]);
        stream.Write<float>(this.recoilYawMax, new object[0]);
        stream.Write<float>(this.recoilDuration, new object[0]);
        stream.Write<float>(this.aimingRecoilSubtract, new object[0]);
        stream.Write<int>((this.ammoType == null) ? 0 : this.ammoType.uniqueID, new object[0]);
    }

    protected void WriteHitInfo(BitStream sendStream, ref Ray ray, bool didHit, ref RaycastHit2 hit)
    {
        bool flag;
        NetEntityID unassigned;
        if (didHit)
        {
            IDBase id = hit.id;
            if ((id != null) && (id.idMain != null))
            {
                unassigned = NetEntityID.Get((MonoBehaviour) id.idMain);
                flag = !unassigned.isUnassigned;
            }
            else
            {
                flag = false;
                unassigned = NetEntityID.unassigned;
            }
        }
        else
        {
            unassigned = NetEntityID.unassigned;
            flag = false;
        }
        this.WriteHitInfo(sendStream, ref ray, didHit, ref hit, flag, unassigned);
    }

    protected virtual void WriteHitInfo(BitStream sendStream, ref Ray ray, bool didHit, ref RaycastHit2 hit, bool hitNetworkView, NetEntityID hitView)
    {
        Vector3 point;
        if (didHit)
        {
            if (hitNetworkView)
            {
                Transform transform;
                IDRemoteBodyPart remoteBodyPart = hit.remoteBodyPart;
                if (remoteBodyPart != null)
                {
                    sendStream.WriteByte((byte) remoteBodyPart.bodyPart);
                    transform = remoteBodyPart.transform;
                }
                else
                {
                    sendStream.WriteByte(0xfe);
                    transform = hitView.transform;
                }
                sendStream.Write<NetEntityID>(hitView, new object[0]);
                point = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                sendStream.WriteByte(0xff);
                point = hit.point;
            }
        }
        else
        {
            sendStream.WriteByte(0xff);
            point = ray.GetPoint(1000f);
        }
        sendStream.WriteVector3(point);
    }

    private sealed class ITEM_TYPE : BulletWeaponItem<BulletWeaponDataBlock>, IBulletWeaponItem, IHeldItem, IInventoryItem, IWeaponItem
    {
        public ITEM_TYPE(BulletWeaponDataBlock BLOCK) : base(BLOCK)
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

