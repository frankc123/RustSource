using System;
using uLink;
using UnityEngine;

public class BaseWeaponModDataBlock : ItemModDataBlock
{
    public GameObject attachObjectRep;
    public GameObject attachObjectVM;
    public string attachSocketName;
    public bool isMesh;
    public bool modifyZoomOffset;
    public float punchScalar;
    public string socketOverrideName;
    public float zoomOffsetZ;

    public BaseWeaponModDataBlock() : this(typeof(WeaponModRep))
    {
    }

    protected BaseWeaponModDataBlock(Type minimumItemModRepresentationType) : base(minimumItemModRepresentationType)
    {
        this.attachSocketName = "muzzle";
        this.socketOverrideName = string.Empty;
        this.punchScalar = 1f;
        if (!typeof(WeaponModRep).IsAssignableFrom(minimumItemModRepresentationType))
        {
            throw new ArgumentOutOfRangeException("minimumItemModRepresentationType", minimumItemModRepresentationType, "!typeof(WeaponModRep).IsAssignableFrom(minimumItemModRepresentationType)");
        }
    }

    protected override IInventoryItem ConstructItem()
    {
        return new ITEM_TYPE(this);
    }

    public Socket GetSocketByName(Socket.Mapped vm, string name)
    {
        Socket.Slot slot = vm.socketMap[name];
        return slot.socket;
    }

    protected override void InstallToItemModRepresentation(ItemModRepresentation modRep)
    {
        base.InstallToItemModRepresentation(modRep);
        if (this.attachObjectRep != null)
        {
            GameObject attached = modRep.itemRep.muzzle.InstantiateAsChild(this.attachObjectRep, false);
            attached.name = this.attachObjectRep.name;
            ((WeaponModRep) modRep).SetAttached(attached, false);
        }
    }

    protected override bool InstallToViewModel(ref ModViewModelAddArgs a)
    {
        if (this.isMesh && !a.isMesh)
        {
            return base.InstallToViewModel(ref a);
        }
        if (!this.isMesh && a.isMesh)
        {
            return base.InstallToViewModel(ref a);
        }
        if (a.vm == null)
        {
            Debug.Log("Viewmodel null for item attachment...");
        }
        if (this.attachObjectVM != null)
        {
            GameObject obj2;
            WeaponModRep modRep = (WeaponModRep) a.modRep;
            if (a.isMesh)
            {
                Socket socketByName = this.GetSocketByName(a.vm, this.attachSocketName);
                obj2 = Object.Instantiate(this.attachObjectVM, socketByName.offset, Quaternion.Euler(socketByName.eulerRotate)) as GameObject;
                obj2.transform.parent = socketByName.parent;
                obj2.transform.localPosition = socketByName.offset;
                obj2.transform.localEulerAngles = socketByName.eulerRotate;
            }
            else
            {
                obj2 = this.GetSocketByName(a.vm, this.attachSocketName).InstantiateAsChild(this.attachObjectVM, true);
            }
            obj2.name = this.attachObjectVM.name;
            modRep.SetAttached(obj2, true);
            ViewModelAttachment component = obj2.GetComponent<ViewModelAttachment>();
            if (component != null)
            {
                if ((this.socketOverrideName != string.Empty) && (component is VMAttachmentSocketOverride))
                {
                    VMAttachmentSocketOverride @override = (VMAttachmentSocketOverride) component;
                    this.SetSocketByname(a.vm, this.socketOverrideName, @override.socketOverride);
                    if (this.modifyZoomOffset)
                    {
                        a.vm.punchScalar = this.punchScalar;
                        a.vm.zoomOffset.z = this.zoomOffsetZ;
                    }
                }
                component.viewModel = a.vm;
            }
        }
        return true;
    }

    protected override void SecureWriteMemberValues(BitStream stream)
    {
        base.SecureWriteMemberValues(stream);
        stream.Write<string>(this.socketOverrideName, new object[0]);
        stream.Write<float>(this.zoomOffsetZ, new object[0]);
        stream.Write<bool>(this.isMesh, new object[0]);
        stream.Write<float>(this.punchScalar, new object[0]);
        stream.Write<bool>(this.modifyZoomOffset, new object[0]);
    }

    public void SetSocketByname(Socket.Mapped vm, string name, Socket newSocket)
    {
        vm.socketMap.ReplaceSocket(name, newSocket);
    }

    protected override void UninstallFromItemModRepresentation(ItemModRepresentation rep)
    {
        WeaponModRep rep2 = (WeaponModRep) rep;
        GameObject attached = rep2.attached;
        if (attached != null)
        {
            rep2.SetAttached(null, false);
            Object.Destroy(attached);
        }
        base.UninstallFromItemModRepresentation(rep);
    }

    protected override void UninstallFromViewModel(ref ModViewModelRemoveArgs a)
    {
        if (this.attachObjectVM != null)
        {
            WeaponModRep modRep = (WeaponModRep) a.modRep;
            GameObject attached = modRep.attached;
            ViewModelAttachment component = attached.GetComponent<ViewModelAttachment>();
            if (component != null)
            {
                component.viewModel = null;
            }
            Socket socketByName = this.GetSocketByName(a.vm, this.attachSocketName);
            if (socketByName.attachParent == null)
            {
                Transform parent = socketByName.parent;
            }
            if (attached != null)
            {
                modRep.SetAttached(null, true);
                Object.Destroy(attached.gameObject);
            }
        }
    }

    private sealed class ITEM_TYPE : ItemModItem<BaseWeaponModDataBlock>, IInventoryItem, IItemModItem
    {
        public ITEM_TYPE(BaseWeaponModDataBlock BLOCK) : base(BLOCK)
        {
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

